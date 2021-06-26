using BW.Common.Utils;
using BW.Games;
using BW.Games.Exceptions;
using BW.Games.Models;
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
        /// 采集订单（存入本地数据库)
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="gameId"></param>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        public List<OrderResult> GetOrders(int gameId, OrderRequest orderRequest)
        {
            IGameBase game = GameUtils.GetGame(gameId);
            if (game == null) throw new APIResultException(APIResultType.MAINTENANCE);

            List<OrderResult> list = new();
            foreach (OrderResult order in game.GetOrders(orderRequest))
            {

            }
            return list;
        }
    }
}
