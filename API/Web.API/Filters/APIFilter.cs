using BW.Common.Agent.Sites;
using BW.Common.Caching;
using BW.Common.Models.Enums;
using BW.Common.Models.Sites;
using BW.Games.Exceptions;
using BW.Games.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SP.StudioCore.Http;
using SP.StudioCore.Model;
using SP.StudioCore.Types;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.API.Filters
{
    public class APIFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 执行之后
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

        /// <summary>
        /// 执行之前
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Request.EnableBuffering();
            ControllerActionDescriptor method = context.ActionDescriptor as ControllerActionDescriptor;
            bool isGuest = method.MethodInfo.HasAttribute<GuestAttribute>();
            if (isGuest)
            {
                base.OnActionExecuting(context);
                return;
            }

            SiteToken token = new SiteToken(context.HttpContext);
            if (!token)
            {
                throw new APIResultException(APIResultType.Authorization, "密钥为空");
            }

            // 验证密钥是否正确
            SiteModel site = SiteInfoAgent.Instance().GetSiteModel(token.SiteID);
            if (!site || site.SecretKey != token.SecretKey)
            {
                throw new APIResultException(APIResultType.Authorization, "密钥错误");
            }

            // 检查白名单IP
            string ip = IPAgent.IP;
            if (!SiteCaching.Instance().IsWhiteIP(site.ID, ip))
            {
                throw new APIResultException(APIResultType.IP, $"IP未授权 - {ip}");
            }

            if (site.Status != SiteStatus.Normal)
            {
                throw new APIResultException(APIResultType.STATUS, "商户已停止");
            }

            context.HttpContext.SetItem(site);
            base.OnActionExecuting(context);
        }
    }
}
