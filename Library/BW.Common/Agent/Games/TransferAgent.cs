using BW.Common.Agent.Systems;
using BW.Common.Agent.Users;
using BW.Common.Caching;
using BW.Common.Entities.Games;
using BW.Common.Entities.Users;
using BW.Common.Models.Enums;
using BW.Common.Models.Games;
using BW.Common.Models.Sites;
using BW.Common.Utils;
using BW.Games;
using BW.Games.Exceptions;
using BW.Games.Models;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
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

            //#3 创建转账订单，状态为进行中
            this.WriteDB.Insert(order);

            TransferResult result = game.Recharge(request);
            if (result.Code == APIResultType.Success)
            {
                // 回写数据库，转账已成功
                order.FinishAt = WebAgent.GetTimestamps();
                order.Status = TransferStatus.Success;
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

            this.WriteDB.Update(order, t => t.Status, t => t.FinishAt);
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

            //#2 检查商户额度是否足够
            //#3 创建转账订单，状态为进行中
            this.WriteDB.Insert(order);

            TransferResult result = game.Withdraw(request);
            if (result.Code == APIResultType.Success)
            {
                // 回写数据库，转账已成功
                order.FinishAt = WebAgent.GetTimestamps();
                order.Status = TransferStatus.Success;
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

            this.WriteDB.Update(order, t => t.Status, t => t.FinishAt);
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
        /// 商户的余额操作
        /// </summary>
        /// <returns></returns>
        public bool AddSiteCredit()
        {

        }
    }
}
