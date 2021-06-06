using BW.Common.Models.Games;
using SP.StudioCore.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Caching
{
    public sealed class GameCaching : CacheBase<GameCaching>
    {
        protected override int DB_INDEX => RedisIndex.GAME;

        #region ========  游戏配置信息  ========


        private const string GAME_INFO = "GAME:INFO:";

        internal GameModel GetGameInfo(int gameId)
        {
            return this.NewExecutor().HashGet($"{GAME_INFO}{gameId}", gameId.GetRedisValue());
        }

        internal GameModel SaveGameInfo(GameModel game)
        {
            this.NewExecutor().HashSet($"{GAME_INFO}{game.ID}", game.ID.GetRedisValue(), game);
            return game;
        }

        #endregion


        #region ========  游戏账号信息  ========

        private const string GAME_USER = "GAME:USER:";

        internal GameUserModel GetGameUser(int gameId, int siteId, int userId)
        {
            string key = $"{GAME_USER}{gameId}:{userId % 100}";
            return this.NewExecutor().HashGet(key, userId);
        }

        internal GameUserModel SaveGameUser(GameUserModel gameUser)
        {
            if (!gameUser) return default;
            string key = $"{GAME_USER}{gameUser.GameID}:{gameUser.UserID % 100}";
            this.NewExecutor().HashSet(key, gameUser.UserID, gameUser);
            return gameUser;
        }

        #endregion
    }
}
