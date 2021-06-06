using BW.Common.Models.Users;
using BW.Common.Agent.Users;
using BW.Common.Models;
using BW.Games.Models;
using SP.StudioCore.Cache.Redis;
using SP.StudioCore.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Caching
{
    public sealed class UserCaching : CacheBase<UserCaching>
    {
        protected override int DB_INDEX => RedisIndex.USER;

        /// <summary>
        /// 得到游戏的登录地址
        /// </summary>
        private const string GAME_LOGIN = "GAME:LOGIN:";

        /// <summary>
        /// 保存游戏的登录地址（有效时间5分钟）
        /// </summary>
        /// <returns></returns>
        public string SaveSessionID(LoginResult loginResult)
        {
            string sessionId = Guid.NewGuid().ToString("N");
            this.NewExecutor().StringSet($"{GAME_LOGIN}{sessionId}", loginResult.ToString().GetRedisValue(), TimeSpan.FromMinutes(5));
            return sessionId;
        }

        /// <summary>
        /// 用户名与ID的对应关系
        /// </summary>
        private const string USER_NAME = "USER:NAME:";

        /// <summary>
        /// 获取用户名对应的用户ID
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int GetUserID(int siteId, string userName)
        {
            string name = $"{siteId}:{userName}";
            string key = string.Concat(USER_NAME, Encryption.toMD5(name, length: 3));
            return this.NewExecutor().HashGet(key, name).GetRedisValue<int>();
        }

        /// <summary>
        /// 保存用户ID
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        internal void SaveUserID(int siteId, string userName, int userId)
        {
            string name = $"{siteId}:{userName}";
            string key = string.Concat(USER_NAME, Encryption.toMD5(name, length: 3));
            this.NewExecutor().HashSet(key, name, userId.GetRedisValue());
        }


        /// <summary>
        /// 用户资料
        /// </summary>
        private const string USER_INFO = "USER:INFO:";

        internal UserModel SaveUserInfo(UserModel user)
        {
            string key = $"{USER_INFO}{user.ID % 1024}";
            this.NewExecutor().HashSet(key, user.ID.GetRedisValue(), user);
            this.SaveUserID(user.SiteID, user.UserName, user.ID);
            return user;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        internal UserModel GetUserInfo(int userId)
        {
            string key = $"{USER_INFO}{userId % 1024}";
            return this.NewExecutor().HashGet(key, userId);
        }
    }
}
