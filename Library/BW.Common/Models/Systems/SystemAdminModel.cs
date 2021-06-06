using Newtonsoft.Json;
using SP.StudioCore.Cache.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Systems
{
    /// <summary>
    /// 系统管理员登录信息
    /// </summary>
    public struct SystemAdminModel
    {
        public int ID;

        public string Name;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static implicit operator RedisValue(SystemAdminModel admin)
        {
            return admin.ToString().GetRedisValue();
        }

        public static implicit operator SystemAdminModel(RedisValue value)
        {
            if (value.IsNullOrEmpty) return default;
            return JsonConvert.DeserializeObject<SystemAdminModel>(value.GetRedisValue<string>());
        }

        public static implicit operator bool(SystemAdminModel admin)
        {
            return admin.ID != 0;
        }
    }
}
