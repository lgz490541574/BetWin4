using BW.Common.Agent.Users;
using BW.Common.Models.Enums;
using BW.Common.Models.Games;
using BW.Common.Utils;
using BW.Common;
using BW.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Common.Caching;
using BW.Common.Models.Sites;
using BW.Common.Models.Users;
using SP.StudioCore.Data;
using System.Data;
using BW.Common.Entities.Games;
using SP.StudioCore.Web;
using BW.Games;
using BW.Games.Models;
using BW.Games.Exceptions;
using BW.Common.Agent.Systems;

namespace BW.Common.Agent.Games
{
    /// <summary>
    /// 游戏登录
    /// </summary>
    public class LoginAgent : AgentBase<LoginAgent>
    {
        /// <summary>
        /// 游戏登录
        /// </summary>
        /// <returns></returns>
        public string GameLogin(int siteId, string userName, GameType type, string playCode)
        {
            //#1 找到用户ID
            int userId = UserInfoAgent.Instance().GetUserID(siteId, userName);
            if (userId == 0) throw new APIResultException(APIResultType.NOUSER);

            IGameBase game = GameUtils.GetGame(type);
            if (game == null) throw new APIResultException(APIResultType.MAINTENANCE);

            //#1 找出用户在这个游戏里面的用户信息
            GameUserModel gameUser = GameUserAgent.Instance().GetGameUser(type, siteId, userId);

            // 如果本地没有当前用户，则自动注册
            if (!gameUser)
            {
                gameUser = this.GameRegister(siteId, userId, type);
            }

            LoginResult result = game.Login(new LoginRequest
            {
                UserName = gameUser.UserName,
                Password = gameUser.Password,
                PlayCode = playCode
            });

            // 如果登录失败
            if (result.Code != APIResultType.Success) { throw new APIResultException(result.Code); }

            string url = ConfigAgent.Instance().GetSystemConfig(ConfigType.LoginUrl);
            return url.Replace("${ID}", UserCaching.Instance().SaveSessionID(result));
        }

        /// <summary>
        /// 在游戏中注册账号
        /// 会员注册之后异步调用/登录的时候如果账号不存在则自动注册
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="userId"></param>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public GameUserModel GameRegister(int siteId, int userId, GameType type)
        {
            IGameBase game = GameUtils.GetGame(type);
            if (game == null) throw new APIResultException(APIResultType.MAINTENANCE);

            SiteModel site = SiteCaching.Instance().GetSiteInfo(siteId);

            UserModel user = UserInfoAgent.Instance().GetUserModel(siteId, userId);

            RegisterResult result = game.Register(new RegisterRequest(site.Prefix, user.UserName));

            if (!result) throw new APIResultException(result.Code);

            // 写入数据库
            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                GameUser gameUser = new GameUser
                {
                    Type = type,
                    SiteID = siteId,
                    UserID = userId,
                    UserName = result.UserName,
                    Password = result.Password,
                    CreateAt = WebAgent.GetTimestamps()
                };
                if (!db.Exists(gameUser))
                {
                    db.Insert(gameUser);
                }

                db.AddCallback(() =>
                {
                    GameCaching.Instance().SaveGameUser(gameUser);
                });

                db.Commit();

                return gameUser;
            }
        }
    }
}
