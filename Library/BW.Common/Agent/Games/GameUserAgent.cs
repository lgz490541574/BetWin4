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
        public GameUser GetGameUserInfo(int gameId, int siteId, int userId)
        {
            return this.ReadDB.ReadInfo<GameUser>(t => t.GameID == gameId && t.SiteID == siteId && t.UserID == userId);
        }

        public GameUserModel GetGameUser(int gameId, int siteId, int userId)
        {
            GameUserModel gameUser = GameCaching.Instance().GetGameUser(gameId, siteId, userId);
            if (gameUser) return gameUser;
            gameUser = this.GetGameUserInfo(gameId, siteId, userId);
            if (!gameUser) return gameUser;
            return GameCaching.Instance().SaveGameUser(gameUser);
        }
    }
}
