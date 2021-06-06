using BW.Common.Models.Enums;
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
    /// 商户资料
    /// </summary>
    public struct SiteModel
    {
        public SiteModel(HashEntry[] hashes) : this()
        {
            foreach (HashEntry hash in hashes)
            {
                switch (hash.Name.GetRedisValue<string>())
                {
                    case "ID":
                        this.ID = hash.Value.GetRedisValue<int>();
                        break;
                    case "Name":
                        this.Name = hash.Value.GetRedisValue<string>();
                        break;
                    case "Prefix":
                        this.Prefix = hash.Value.GetRedisValue<string>();
                        break;
                    case "Status":
                        this.Status = hash.Value.GetRedisValue<SiteStatus>();
                        break;
                    case "SecretKey":
                        this.SecretKey = hash.Value.GetRedisValue<Guid>();
                        break;
                }
            }
        }

        /// <summary>
        /// 商户ID
        /// </summary>
        public int ID;

        /// <summary>
        /// 商户名
        /// </summary>
        public string Name;

        /// <summary>
        /// 前缀名
        /// </summary>
        public string Prefix;

        /// <summary>
        /// 商户状态
        /// </summary>
        public SiteStatus Status;

        /// <summary>
        /// API通信密钥
        /// </summary>
        public Guid SecretKey;

        public static implicit operator SiteModel(HashEntry[] hashes)
        {
            return new SiteModel(hashes);
        }


        public static implicit operator HashEntry[](SiteModel site)
        {
            return new[]
            {
                new HashEntry("ID",site.ID.GetRedisValue()),
                new HashEntry("Name",site.Name.GetRedisValue()),
                new HashEntry("Prefix",site.Prefix.GetRedisValue()),
                new HashEntry("Status",site.Status.GetRedisValue()),
                new HashEntry("SecretKey",site.SecretKey.GetRedisValue())
            };
        }

        public static implicit operator bool(SiteModel site)
        {
            return site.ID != 0;
        }

        public static implicit operator int(SiteModel site)
        {
            return site.ID;
        }
    }
}
