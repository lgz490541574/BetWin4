using BW.Common.Caching;
using BW.Common.Agent.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP.StudioCore.Data;
using System.Data;
using SP.StudioCore.Web;
using BW.Common.Models;
using BW.Common.Models.Users;
using SP.StudioCore.Security;
using SP.StudioCore.Enums;
using BW.Common.Entities.Users;
using BW.Games.Models;
using BW.Games.Exceptions;

namespace BW.Common.Agent.Users
{
    public sealed class UserInfoAgent : AgentBase<UserInfoAgent>
    {
        /// <summary>
        /// 根据用户名获取用户ID
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int GetUserID(int siteId, string userName)
        {
            int userId = UserCaching.Instance().GetUserID(siteId, userName);
            if (userId == 0)
            {
                userId = this.ReadDB.ReadInfo<User, int>(t => t.ID, t => t.SiteID == siteId && t.UserName == userName);
                if (userId == 0) return userId;
                UserCaching.Instance().SaveUserID(siteId, userName, userId);
            }
            return userId;
        }

        /// <summary>
        /// 注册账号
        /// 可能来自API或者用户自主注册
        /// </summary>
        public UserModel Register(int siteId, RegisterRequest register)
        {
            if (!WebAgent.IsUserName(register.UserName, 2, 12))
            {
                throw new APIResultException(APIResultType.BADNAME);
            }
            if (register.Password.Length < 5 || register.Password.Length > 16)
            {
                throw new APIResultException(APIResultType.BADPASSWORD);
            }

            string userName = register.UserName.ToLower();
            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                if (db.Exists<User>(t => t.SiteID == siteId && t.UserName == register.UserName))
                {
                    throw new APIResultException(APIResultType.EXISTSUSER);
                }

                User user = new()
                {
                    SiteID = siteId,
                    UserName = userName,
                    Password = Encryption.toMD5(register.Password),
                    CreateAt = WebAgent.GetTimestamps(),
                    Status = UserStatus.Normal
                };
                db.InsertIdentity(user);

                db.AddCallback(() =>
                {
                    UserCaching.Instance().SaveUserInfo(user);

                    // 写入消息队列，异步在每个游戏创建账号
                });

                db.Commit();

                return user;
            }
        }

        /// <summary>
        /// 获取用户资料（缓存读取)
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserModel GetUserModel(int siteId, int userId)
        {
            UserModel user = UserCaching.Instance().GetUserInfo(userId);
            if (user && user.SiteID != siteId) return default;

            if (!user)
            {
                user = this.ReadDB.ReadInfo<User>(t => t.SiteID == siteId && t.ID == userId);
                if (!user) return default;
                UserCaching.Instance().SaveUserInfo(user);
            }
            return user;
        }


    }
}
