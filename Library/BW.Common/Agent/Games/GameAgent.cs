using BW.Common.Caching;
using BW.Common.Entities.Games;
using BW.Common.Models.Games;
using SP.StudioCore.Data;
using SP.StudioCore.Mvc.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
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
        public Game GetGameInfo(int gameId)
        {
            return this.ReadDB.ReadInfo<Game>(t => t.ID == gameId);
        }

        /// <summary>
        /// 获取游戏配置对象
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public GameModel GetGameModel(int gameId)
        {
            GameModel game = GameCaching.Instance().GetGameInfo(gameId);
            if (!game) return game;
            game = this.GetGameInfo(gameId);
            if (!game) return default;
            return GameCaching.Instance().SaveGameInfo(game);
        }

        /// <summary>
        /// 保存游戏接口配置信息
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool SaveGameInfo(Game game)
        {
            if (game.ID == 0) throw new ResultException("未指定游戏ID");
            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                if (db.Exists(game))
                {
                    db.Update(game, t => t.Type, t => t.Status, t => t.Setting);
                }
                else
                {
                    db.Insert(game);
                }

                db.AddCallback(() =>
                {
                    GameCaching.Instance().SaveGameInfo(game);
                });

                db.Commit();
            }
            return true;
        }
    }
}
