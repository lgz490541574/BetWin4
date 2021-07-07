using BW.Common.Caching;
using BW.Common.Entities.Games;
using BW.Common.Models.Games;
using BW.Games.Models;
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
        public Game GetGameInfo(GameType type)
        {
            return this.ReadDB.ReadInfo<Game>(t => t.Type == type);
        }

        /// <summary>
        /// 获取游戏配置对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GameModel GetGameModel(GameType type)
        {
            GameModel game = GameCaching.Instance().GetGameInfo(type);
            if (game) return game;
            game = this.GetGameInfo(type);
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
            if (!Enum.IsDefined(typeof(GameType), game.Type)) throw new ResultException("未指定游戏类型");
            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                if (db.Exists(game))
                {
                    db.Update(game, t => t.Status, t => t.Setting);
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

        public List<GameModel> UpdateCache()
        {
            List<GameModel> list = new();
            foreach (Game game in this.ReadDB.ReadList<Game>())
            {
                list.Add(GameCaching.Instance().SaveGameInfo(game));
            }
            return list;
        }
    }
}
