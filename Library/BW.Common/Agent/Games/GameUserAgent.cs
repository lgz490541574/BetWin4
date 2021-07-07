using BW.Common.Entities.Games;
using BW.Common.Caching;
using BW.Common.Models.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Games.Models;

namespace BW.Common.Agent.Games
{
    /// <summary>
    /// 处理会员在游戏中创建的账号
    /// </summary>
    public sealed class GameUserAgent : AgentBase<GameUserAgent>
    {
        public GameUser GetGameUserInfo(GameType type, int userId)
        {
            return this.ReadDB.ReadInfo<GameUser>(t => t.Type == type && t.UserID == userId);
        }

        public GameUser GetGameUserInfo(GameType type, string userName)
        {
            return this.ReadDB.ReadInfo<GameUser>(t => t.Type == type && t.UserName == userName);
        }

        public GameUserModel GetGameUser(GameType type, int siteId, int userId)
        {
            GameUserModel gameUser = GameCaching.Instance().GetGameUser(type, userId);
            if (gameUser) return gameUser;
            gameUser = this.GetGameUserInfo(type, userId);
            if (!gameUser) return gameUser;
            gameUser = GameCaching.Instance().SaveGameUser(gameUser);
            if (siteId != gameUser.SiteID) return default;
            return gameUser;
        }

        /// <summary>
        /// 通过游戏中的用户名获取用户资料
        /// </summary>
        public GameUserModel GetGameUser(GameType type, string userName)
        {
            GameUserModel gameUser = GameCaching.Instance().GetGameUser(type, userName);
            if (gameUser) return gameUser;
            gameUser = this.GetGameUserInfo(type, userName);
            if (!gameUser) return gameUser;
            return GameCaching.Instance().SaveGameUser(gameUser);
        }
    }
}
