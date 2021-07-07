using BW.Common.Models.Enums;
using BW.Games.Models;
using Newtonsoft.Json;
using SP.StudioCore.Cache.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Sites
{
    /// <summary>
    /// 商户开通的游戏
    /// </summary>
    public struct SiteGameModel
    {
        public int SiteID;

        public GameType Type;

        public SiteGameStatus Status { get; set; }

        public decimal Rate { get; set; }

        public static implicit operator RedisValue(SiteGameModel model)
        {
            return JsonConvert.SerializeObject(model).GetRedisValue();
        }

        public static implicit operator SiteGameModel(RedisValue value)
        {
            if (value.IsNullOrEmpty) return default;
            return JsonConvert.DeserializeObject<SiteGameModel>(value);
        }

        public static implicit operator bool(SiteGameModel model)
        {
            return model.SiteID != 0 && Enum.IsDefined(typeof(GameType), model.Type);
        }
    }
}
