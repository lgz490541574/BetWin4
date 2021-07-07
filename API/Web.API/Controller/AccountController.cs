using BW.Common.Models.Users;
using BW.Common.Agent.Games;
using BW.Common.Agent.Users;
using BW.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.API.Filters;
using BW.Games.Models;

namespace Web.API.Controller
{
    public class AccountController : APIControllerBase
    {

        /// <summary>
        /// 注册会员
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public ContentResult Register([FromBody] RegisterRequest register)
        {
            UserModel user = UserInfoAgent.Instance().Register(SiteInfo, register);
            return this.GetResultContent(new
            {
                Code = APIResultType.Success,
                UserID = user.ID,
                user.UserName
            });
        }

        /// <summary>
        /// 会员登录（进入游戏)
        /// </summary>
        /// <returns></returns>
        public ContentResult Login([FromBody] LoginRequest login)
        {
            string loginUrl = LoginAgent.Instance().GameLogin(this.SiteInfo, login.UserName, login.Game, login.PlayCode);
            return this.GetResultContent(new
            {
                Code = APIResultType.Success,
                Url = loginUrl 
            });
        }
    }
}
