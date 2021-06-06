using Newtonsoft.Json;
using SP.StudioCore.Cache.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Games
{
    /// <summary>
    /// 用户在游戏中的创建的账号
    /// </summary>
    public struct GameUserModel
    {
        public int GameID;

        public int SiteID;

        public int UserID;

        public string UserName;

        public string Password;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static implicit operator RedisValue(GameUserModel gameUser)
        {
            return gameUser.ToString().GetRedisValue();
        }

        public static implicit operator GameUserModel(RedisValue value)
        {
            if (value.IsNullOrEmpty) return default;
            return JsonConvert.DeserializeObject<GameUserModel>(value.GetRedisValue<string>());
        }

        public static implicit operator bool(GameUserModel gameUser)
        {
            return gameUser.GameID != 0 && gameUser.SiteID != 0 && gameUser.UserID != 0 && !string.IsNullOrEmpty(gameUser.UserName);
        }
    }
}
