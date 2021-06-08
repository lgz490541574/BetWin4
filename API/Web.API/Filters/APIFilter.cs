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
            SiteToken token = new SiteToken(context.HttpContext);
            if (!token)
            {
                throw new APIResulteException(APIResultType.Authorization, "密钥为空");
            }

            // 验证密钥是否正确
            SiteModel site = SiteInfoAgent.Instance().GetSiteModel(token.SiteID);
            if (!site || site.SecretKey != token.SecretKey)
            {
                throw new APIResulteException(APIResultType.Authorization, "密钥错误");
            }

            // 检查白名单IP
            string ip = IPAgent.IP;
            if (!SiteCaching.Instance().IsWhiteIP(site.ID, ip))
            {
                throw new APIResulteException(APIResultType.IP, "IP未授权");
            }

            if (site.Status != SiteStatus.Normal)
            {
                throw new APIResulteException(APIResultType.STATUS, "商户已停止");
            }

            context.HttpContext.SetItem(site);
            base.OnActionExecuting(context);
        }
    }
}
