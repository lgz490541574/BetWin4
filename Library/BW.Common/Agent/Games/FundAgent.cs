using BW.Common.Agent.Users;
using BW.Common.Models.Games;
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
    /// <summary>
    /// 资金有关接口
    /// </summary>
    public class FundAgent : AgentBase<FundAgent>
    {
        /// <summary>
        /// 查询余额
        /// </summary>
        /// <returns></returns>
        public BalanceResult GetBalance(int siteId, string userName, GameType type)
        {
            //#1 判断用户是否存在
            int userId = UserInfoAgent.Instance().GetUserID(siteId, userName);
            if (userId == 0) throw new APIResultException(APIResultType.NOUSER);

            //#1 找出用户在这个游戏里面的用户信息
            GameUserModel gameUser = GameUserAgent.Instance().GetGameUser(type, siteId, userId);

            // 如果本地没有当前用户，则自动注册
            if (!gameUser)
            {
                gameUser = LoginAgent.Instance().GameRegister(siteId, userId, type);
            }

            IGameBase game = GameUtils.GetGame(type);
            if (game == null) throw new APIResultException(APIResultType.MAINTENANCE);

            return game.Balance(new BalanceRequest
            {
                UserName = gameUser.UserName,
                Password = gameUser.Password
            });
        }
    }
}
