using BW.Common.Agent.Systems;
using BW.Common.Agent.Users;
using BW.Common.Caching;
using BW.Common.Entities.Games;
using BW.Common.Entities.Sites;
using BW.Common.Entities.Users;
using BW.Common.Models.Enums;
using BW.Common.Models.Games;
using BW.Common.Models.Sites;
using BW.Common.Utils;
using BW.Games;
using BW.Games.Exceptions;
using BW.Games.Models;
using SP.StudioCore.Data;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Agent.Games
{
    /// <summary>
    /// 转账管理
    /// </summary>
    public class TransferAgent : AgentBase<TransferAgent>
    {
        public TransferResult Recharge(int siteId, string userName, int gameId, string orderId, decimal money)
        {
            //#1 判断用户是否存在
            int userId = UserInfoAgent.Instance().GetUserID(siteId, userName);
            if (userId == 0) throw new APIResultException(APIResultType.NOUSER);

            //#1 找出用户在这个游戏里面的用户信息
            GameUserModel gameUser = GameUserAgent.Instance().GetGameUser(gameId, siteId, userId);

            // 如果本地没有当前用户，则自动注册
            if (!gameUser)
            {
                gameUser = LoginAgent.Instance().GameRegister(siteId, userId, gameId);
            }

            IGameBase game = GameUtils.GetGame(gameId);
            if (game == null) throw new APIResultException(APIResultType.MAINTENANCE);

            SiteModel site = SiteAgent.Instance().GetSiteInfo(siteId);
            TransferRequest request = new() { Money = money, OrderID = orderId, UserName = gameUser.UserName };

            // 数据库操作
            //#1 检查订单号是否重复
            if (!GameCaching.Instance().CheckTransferID(siteId, orderId) ||
                this.ReadDB.Exists<GameTransferOrder>(t => t.OrderID == orderId && t.SiteID == siteId))
            {
                throw new APIResultException(APIResultType.EXISTSORDER);
            }

            GameTransferOrder order = new GameTransferOrder
            {
                OrderID = orderId,
                SiteID = siteId,
                GameID = gameId,
                UserID = userId,
                CreateAt = WebAgent.GetTimestamps(),
                SourceID = request.SourceID,
                Status = TransferStatus.Progress,
                Money = money
            };

            //#2 检查商户额度是否足够
            this.LockSiteCredit(siteId, gameId, money * -1M, orderId);

            //#3 创建转账订单，状态为进行中
            this.WriteDB.Insert(order);

            TransferResult result = game.Recharge(request);

            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                if (result.Code == APIResultType.Success)
                {
                    // 回写数据库，转账已成功
                    order.FinishAt = WebAgent.GetTimestamps();
                    order.Status = TransferStatus.Success;

                    // 解锁额度
                    this.UnlockSiteCredit(db, siteId, orderId);

                    // 扣除额度
                    this.SaveUserCredit(db, siteId, gameId, money * -1M, CreditType.UserRecharge, orderId, "");
                }
                else if (result.Code == APIResultType.Exception)
                {
                    // 发生异常，写入消息队列，异步检查转账是否成功
                    order.Status = TransferStatus.Exception;
                }
                else
                {
                    order.Status = TransferStatus.Faild;
                    order.FinishAt = WebAgent.GetTimestamps();

                    // 解锁额度
                    this.UnlockSiteCredit(db, siteId, orderId);
                }

                db.Update(order, t => t.Status, t => t.FinishAt);
                db.Commit();
            }

            if (result.Code == APIResultType.Success) return result;

            throw new APIResultException(result.Code);
        }

        public TransferResult Withdraw(int siteId, string userName, int gameId, string orderId, decimal money)
        {
            //#1 判断用户是否存在
            int userId = UserInfoAgent.Instance().GetUserID(siteId, userName);
            if (userId == 0) throw new APIResultException(APIResultType.NOUSER);

            //#1 找出用户在这个游戏里面的用户信息
            GameUserModel gameUser = GameUserAgent.Instance().GetGameUser(gameId, siteId, userId);

            // 如果本地没有当前用户，则自动注册
            if (!gameUser)
            {
                gameUser = LoginAgent.Instance().GameRegister(siteId, userId, gameId);
            }

            IGameBase game = GameUtils.GetGame(gameId);
            if (game == null) throw new APIResultException(APIResultType.MAINTENANCE);

            SiteModel site = SiteAgent.Instance().GetSiteInfo(siteId);

            TransferRequest request = new() { Money = money, OrderID = orderId, UserName = gameUser.UserName };

            // 数据库操作
            //#1 检查订单号是否重复
            if (!GameCaching.Instance().CheckTransferID(siteId, orderId) ||
               this.ReadDB.Exists<GameTransferOrder>(t => t.OrderID == orderId && t.SiteID == siteId))
            {
                throw new APIResultException(APIResultType.EXISTSORDER);
            }

            GameTransferOrder order = new GameTransferOrder
            {
                OrderID = orderId,
                SiteID = siteId,
                GameID = gameId,
                UserID = userId,
                CreateAt = WebAgent.GetTimestamps(),
                SourceID = request.SourceID,
                Status = TransferStatus.Progress,
                Money = money * -1M
            };

            //#3 创建转账订单，状态为进行中
            this.WriteDB.Insert(order);

            TransferResult result = game.Withdraw(request);

            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                if (result.Code == APIResultType.Success)
                {
                    // 回写数据库，转账已成功
                    order.FinishAt = WebAgent.GetTimestamps();
                    order.Status = TransferStatus.Success;

                    // 增加额度
                    this.SaveUserCredit(db, siteId, gameId, money, CreditType.UserWithdraw, orderId, "");
                }
                else if (result.Code == APIResultType.Exception)
                {
                    // 发生异常，写入消息队列，异步检查转账是否成功
                    order.Status = TransferStatus.Exception;
                }
                else
                {
                    order.Status = TransferStatus.Faild;
                    order.FinishAt = WebAgent.GetTimestamps();
                }

                db.Update(order, t => t.Status, t => t.FinishAt);
                db.Commit();
            }


            if (result.Code == APIResultType.Success) return result;

            throw new APIResultException(result.Code);
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        public CheckTransferResult Check(int siteId, string orderId)
        {
            GameTransferOrder order = this.ReadDB.ReadInfo<GameTransferOrder>(t => t.OrderID == orderId && t.SiteID == siteId);

            if (order == null)
            {
                throw new APIResultException(APIResultType.ORDER_NOTFOUND);
            }
            switch (order.Status)
            {
                case TransferStatus.Progress:
                case TransferStatus.Exception:
                    throw new APIResultException(APIResultType.Exception);
                case TransferStatus.Faild:
                    throw new APIResultException(APIResultType.ORDER_FAILD);
                case TransferStatus.Success:
                    return new CheckTransferResult(APIResultType.Success)
                    {
                        CreateAt = order.CreateAt,
                        Money = order.Money,
                        OrderID = order.OrderID,
                        GameID = order.GameID,
                        UserName = UserInfoAgent.Instance().GetUserModel(siteId, order.UserID).UserName
                    };
            }

            throw new APIResultException(APIResultType.Exception);
        }

        /// <summary>
        /// 锁定商户额度
        /// </summary>
        /// <returns></returns>
        private bool LockSiteCredit(int siteId, int gameId, decimal money, string orderId)
        {
            if (money > 0) return true;
            money = Math.Abs(money);
            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                if (db.ExecuteNonQuery(CommandType.Text, $"UPDATE [{typeof(SiteGame).GetTableName()}] SET LockCredit = LockCredit + @Money WHERE SiteID = @SiteID AND GameID = @GameID AND Credit - LockCredit - @Money >= 0", new
                {
                    Money = money,
                    SiteID = siteId,
                    GameID = gameId
                }) == 0)
                {
                    throw new APIResultException(APIResultType.NOCREDIT);
                }

                CreditLock creditLock = new CreditLock
                {
                    SiteID = siteId,
                    GameID = gameId,
                    Credit = Math.Abs(money),
                    ID = orderId
                };

                db.Insert(creditLock);

                db.Commit();
            }
            return true;
        }

        /// <summary>
        /// 解锁商户额度
        /// </summary>
        /// <returns></returns>
        private void UnlockSiteCredit(DbExecutor db, int siteId, string orderId)
        {
            CreditLock creditLock = db.ReadInfo<CreditLock>(t => t.SiteID == siteId && t.ID == orderId);
            db.ExecuteNonQuery(CommandType.Text, $"UPDATE [{typeof(SiteGame).GetTableName()}] SET LockCredit = LockCredit - @Money WHERE SiteID = @SiteID AND GameID = @GameID",
                new
                {
                    SiteID = siteId,
                    GameID = creditLock.GameID,
                    Money = creditLock.Credit
                });
            db.Delete(creditLock);

        }

        /// <summary>
        /// 额度操作
        /// </summary>
        /// <returns></returns>
        private void SaveUserCredit(DbExecutor db, int siteId, int gameId, decimal money, CreditType type, string sourceId, string description)
        {
            db.ExecuteNonQuery(CommandType.Text, $"UPDATE [{typeof(SiteGame).GetTableName()}] SET Credit = Credit + @Money WHERE SiteID = @SiteID AND GameID = @GameID",
                new
                {
                    Money = money,
                    SiteID = siteId,
                    GameID = gameId
                });
            decimal credit = db.ReadInfo<SiteGame, decimal>(t => t.Credit, t => t.SiteID == siteId && t.GameID == gameId);

            CreditLog log = new CreditLog()
            {
                SiteID = siteId,
                GameID = gameId,
                Credit = money,
                Balance = credit,
                CreateAt = WebAgent.GetTimestamps(),
                Description = description,
                SourceID = sourceId,
                Type = type
            };

            db.Insert(log);
        }
    }
}
