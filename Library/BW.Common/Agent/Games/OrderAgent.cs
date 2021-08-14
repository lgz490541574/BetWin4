using BW.Common.Agent.Users;
using BW.Common.Caching;
using BW.Common.Entities.Games;
using BW.Common.Models.Games;
using BW.Common.Utils;
using BW.Games;
using BW.Games.Exceptions;
using BW.Games.Models;
using SP.StudioCore.Data;
using SP.StudioCore.Utils;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Agent.Games
{
    public class OrderAgent : AgentBase<OrderAgent>
    {
        /// <summary>
        /// 采集订单（不区分游戏）
        /// </summary>
        /// <returns></returns>
        public void GetOrders(params GameType[] games)
        {
            GameType type = GameCaching.Instance().GetOrderQueue();
            if (!Enum.IsDefined(typeof(GameType), type)) return;
            if (games.Length != 0 && !games.Contains(type))
            {
                GameCaching.Instance().SaveOrderQueue(type);
                return;
            }
            this.GetOrders(type);
        }

        /// <summary>
        /// 采集订单（存入本地数据库)
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="type"></param>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        private List<OrderResult> GetOrders(GameType type)
        {
            IGameBase game = GameUtils.GetGame(type);
            if (game == null) throw new APIResultException(APIResultType.MAINTENANCE);
            OrderRequest orderRequest = GameCaching.Instance().GetOrderRequest(type);
            List<OrderResult> list = new();
            try
            {
                IEnumerable<OrderResult> orderlist = game.GetOrders(orderRequest);
                foreach (OrderResult order in orderlist)
                {
                    // 写入数据库
                    this.SaveOrder(type, order);
                    list.Add(order);
                }
                // 把原始数据批量写入Redis
                GameCaching.Instance().SaveOrderDetail(list.Select(t => new OrderDetailResult
                {
                    Type = type,
                    OrderID = t.OrderID,
                    Data = t.RawData
                }));
            }
            catch (NullReferenceException ex)
            {
                ConsoleHelper.WriteLine($"[GetOrders - {ex.GetType().Name}] {type}:{ex.Message}", ConsoleColor.Red);
                ConsoleHelper.WriteLine(ErrorHelper.GetExceptionContent(ex), ConsoleColor.Gray);
            }
            catch (NotImplementedException ex)
            {
                ConsoleHelper.WriteLine($"[GetOrders - {ex.GetType().Name}] {type}:{ex.Message}", ConsoleColor.Red);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine($"[GetOrders - {ex.GetType().Name}] {type}:{ex.Message}", ConsoleColor.Red);
            }
            finally
            {
                GameCaching.Instance().SaveOrderRequest(type, orderRequest);
            }
            return list;
        }

        /// <summary>
        /// 存入数据库
        /// </summary>
        /// <param name="order"></param>
        private void SaveOrder(GameType type, OrderResult order)
        {
            //#1 拿出原始数据MD5码
            string md5 = this.ReadDB.ReadInfo<GameOrder, string>(t => t.MD5, t => t.OrderID == order.OrderID && t.Type == type);
            string rawMD5 = SP.StudioCore.Security.Encryption.toMD5(order.RawData, length: 16);
            if (md5 == rawMD5) return;

            // 通过用户名去拿用户ID和商户
            GameUserModel user = GameUserAgent.Instance().GetGameUser(type, order.UserName);
            string userName = UserInfoAgent.Instance().GetUserModel(user.SiteID, user.UserID).UserName;

            GameOrder gameOrder = new GameOrder
            {
                OrderID = order.OrderID,
                Type = type,
                SiteID = user.SiteID,
                UserID = user.UserID,
                UserName = userName,
                BetMoney = order.BetMoney,
                Money = order.Money,
                CreateAt = order.CreateAt,
                FinishAt = order.FinishAt,
                Game = order.Game,
                UpdateAt = WebAgent.GetTimestamps(),
                Status = order.Status,
                MD5 = rawMD5
            };
            if (string.IsNullOrEmpty(md5))
            {
                this.WriteDB.Insert(gameOrder);
            }
            else
            {
                this.WriteDB.Update(gameOrder);
            }

            GameOrderDetail detail = new GameOrderDetail
            {
                Type = gameOrder.Type,
                OrderID = gameOrder.OrderID,
                SiteID = user.SiteID,
                RawData = order.RawData
            };
            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                if (db.Exists(detail))
                {
                    db.Update(detail);
                }
                else
                {
                    db.Insert(detail);
                }

                db.Commit();
            }
        }
    }
}
