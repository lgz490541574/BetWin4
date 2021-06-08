using BW.Common.Agent.Systems;
using Web.System.Filters;
using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Json;
using SP.StudioCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.System.Controller
{
    public sealed class AdminController : SysControllerBase
    {
        [Guest]
        public ContentResult Login([FromForm] string userName, [FromForm] string password)
        {
            return this.GetResultContent(AdminAgent.Instance().Login(userName, password, out Guid session), "登录成功", new
            {
                Token = session.ToString("N")
            });
        }

        /// <summary>
        /// 当前登录的管理员信息
        /// </summary>
        /// <returns></returns>
        public ContentResult Info()
        {
            return this.GetResultContent(this.AdminInfo.ToJson());
        }
    }
}
