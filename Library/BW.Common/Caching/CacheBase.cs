using BW.Common.Utils;
using SP.StudioCore.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Caching
{
    public abstract class CacheBase<T> : RedisCacheBase where T : class, new()
    {
        private static T _intance;
        /// <summary>
        /// 单例模式
        /// </summary>
        /// <returns></returns>
        public static T Instance()
        {
            if (_intance == null) _intance = new T();
            return _intance;
        }

        protected CacheBase() : base(Setting.RedisConnection)
        {
        }
    }

    internal static class RedisIndex
    {
        /// <summary>
        /// 商户库
        /// </summary>
        internal const int SITE = 0;

        internal const int USER = 1;

        internal const int GAME = 2;
    }
}
 