using BW.Common.Caching;
using BW.Common.Entities.Games;
using BW.Common.Models.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Agent.Games
{
    /// <summary>
    /// 游戏代理
    /// </summary>
    public class GameAgent : AgentBase<GameAgent>
    {
        /// <summary>
        /// 获取游戏配置对象（数据库读取）
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        internal Game GetGameInfo(int gameId)
        {
            return this.ReadDB.ReadInfo<Game>(t => t.ID == gameId);
        }

        /// <summary>
        /// 获取游戏配置对象
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        internal GameModel GetGameModel(int gameId)
        {
            GameModel game = GameCaching.Instance().GetGameInfo(gameId);
            if (!game) return game;
            game = this.GetGameInfo(gameId);
            if (!game) return default;
            return GameCaching.Instance().SaveGameInfo(game);
        }
    }
}
