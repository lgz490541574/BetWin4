using BW.Common.Entities.Games;
using BW.Common.Caching;
using BW.Common.Models.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Agent.Games
{
    /// <summary>
    /// 处理会员在游戏中创建的账号
    /// </summary>
    public sealed class GameUserAgent : AgentBase<GameUserAgent>
    {
        public GameUser GetGameUserInfo(int gameId, int userId)
        {
            return this.ReadDB.ReadInfo<GameUser>(t => t.GameID == gameId && t.UserID == userId);
        }

        public GameUser GetGameUserInfo(int gameId, string userName)
        {
            return this.ReadDB.ReadInfo<GameUser>(t => t.GameID == gameId && t.UserName == userName);
        }

        public GameUserModel GetGameUser(int gameId, int siteId, int userId)
        {
            GameUserModel gameUser = GameCaching.Instance().GetGameUser(gameId, userId);
            if (gameUser) return gameUser;
            gameUser = this.GetGameUserInfo(gameId, userId);
            if (!gameUser) return gameUser;
            gameUser = GameCaching.Instance().SaveGameUser(gameUser);
            if (siteId != gameUser.SiteID) return default;
            return gameUser;
        }

        /// <summary>
        /// 通过游戏中的用户名获取用户资料
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public GameUserModel GetGameUser(int gameId, string userName)
        {
            GameUserModel gameUser = GameCaching.Instance().GetGameUser(gameId, userName);
            if (gameUser) return gameUser;
            gameUser = this.GetGameUserInfo(gameId, userName);
            if (!gameUser) return gameUser;
            return GameCaching.Instance().SaveGameUser(gameUser);
        }
    }
}
