using Newtonsoft.Json;
using SP.StudioCore.Cache.Redis;
using SP.StudioCore.Enums;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Users
{
    /// <summary>
    /// 会员基本资料
    /// </summary>
    public struct UserModel
    {
        public int ID;

        public int SiteID;

        public string UserName;

        public long CreateAt;

        public UserStatus Status;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static implicit operator RedisValue(UserModel user)
        {
            return user.ToString().GetRedisValue();
        }

        public static implicit operator UserModel(RedisValue value)
        {
            if (value.IsNullOrEmpty) return default;
            return JsonConvert.DeserializeObject<UserModel>(value.GetRedisValue<string>());
        }

        public static implicit operator bool(UserModel user)
        {
            return user.ID != 0 && !string.IsNullOrEmpty(user.UserName);
        }
    }
}
