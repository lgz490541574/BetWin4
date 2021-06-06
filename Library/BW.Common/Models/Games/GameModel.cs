using BW.Common.Models.Enums;
using BW.Common.Models;
using Newtonsoft.Json;
using SP.StudioCore.Cache.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Games.Models;

namespace BW.Common.Models.Games
{
    /// <summary>
    /// 游戏配置
    /// </summary>
    public struct GameModel
    {
        /// <summary>
        /// 游戏编号
        /// </summary>
        public int ID;

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameType Type;

        /// <summary>
        /// 参数配置
        /// </summary>
        public string Setting;

        /// <summary>
        /// 游戏接口状态
        /// </summary>
        public GameStatus Status;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static implicit operator RedisValue(GameModel game)
        {
            return game.ToString().GetRedisValue();
        }

        public static implicit operator GameModel(RedisValue value)
        {
            if (value.IsNullOrEmpty) return default;
            return JsonConvert.DeserializeObject<GameModel>(value.GetRedisValue<string>());
        }

        public static implicit operator bool(GameModel game)
        {
            return game.ID != 0;
        }
    }
}
