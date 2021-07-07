using SP.StudioCore.Cache.Redis;
using SP.StudioCore.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    public struct OrderDetailResult
    {
        /// <summary>
        /// 所属游戏
        /// </summary>
        public GameType Type;

        /// <summary>
        /// 原始订单号
        /// </summary>
        public string OrderID;

        /// <summary>
        /// 原始数据
        /// </summary>
        public JsonString Data;

        public OrderDetailResult(OrderDetailRequest request, RedisValue value) : this()
        {
            this.Type = request.Game;
            this.OrderID = request.OrderID;
            this.Data = value.GetRedisValue<string>();
        }

        public static implicit operator RedisKey(OrderDetailResult result)
        {
            return $"{result.Type}:{result.OrderID}";
        }

        public static implicit operator RedisValue(OrderDetailResult result)
        {
            return ((string)result.Data).GetRedisValue();
        }

        public static implicit operator KeyValuePair<RedisKey, RedisValue>(OrderDetailResult result)
        {
            return new KeyValuePair<RedisKey, RedisValue>(result, result);
        }
    }
}
