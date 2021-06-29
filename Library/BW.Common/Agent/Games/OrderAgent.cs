using BW.Common.Agent.Users;
using BW.Common.Caching;
using BW.Common.Entities.Games;
using BW.Common.Models.Games;
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
    public class OrderAgent : AgentBase<OrderAgent>
    {
        /// <summary>
        /// 采集订单（不区分游戏）
        /// </summary>
        /// <returns></returns>
        public void GetOrders(params int[] games)
        {
            int gameId = GameCaching.Instance().GetOrderQueue();
            if (gameId == 0) return;
            if (games.Length != 0 && !games.Contains(gameId))
            {
                GameCaching.Instance().SaveOrderQueue(gameId);
                return;
            }
            this.GetOrders(gameId);
        }

        /// <summary>
        /// 采集订单（存入本地数据库)
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="gameId"></param>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        private List<OrderResult> GetOrders(int gameId)
        {
            IGameBase game = GameUtils.GetGame(gameId);
            if (game == null) throw new APIResultException(APIResultType.MAINTENANCE);
            OrderRequest orderRequest = GameCaching.Instance().GetOrderRequest(gameId);
            List<OrderResult> list = new();
            try
            {
                foreach (OrderResult order in game.GetOrders(orderRequest))
                {
                    // 写入数据库
                    this.SaveOrder(gameId, order);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            finally
            {
                GameCaching.Instance().SaveOrderRequest(gameId, orderRequest);
            }
            return list;
        }

        /// <summary>
        /// 存入数据库
        /// </summary>
        /// <param name="order"></param>
        private void SaveOrder(int gameId, OrderResult order)
        {
            //#1 拿出原始数据MD5码
            string md5 = this.ReadDB.ReadInfo<GameOrder, string>(t => t.MD5, t => t.OrderID == order.OrderID && t.GameID == gameId);
            string rawMD5 = SP.StudioCore.Security.Encryption.toMD5(order.RawData, length: 16);
            if (md5 == rawMD5) return;

            // 通过用户名去拿用户ID和商户
            GameUserModel user = GameUserAgent.Instance().GetGameUser(gameId, order.UserName);
            string userName = UserInfoAgent.Instance().GetUserModel(user.SiteID, user.UserID).UserName;

            GameOrder gameOrder = new GameOrder
            {
                OrderID = order.OrderID,
                GameID = gameId,
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
        }
    }
}
