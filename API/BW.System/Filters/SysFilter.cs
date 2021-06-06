using BW.Common.Agent.Systems;
using BW.Common.Models.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SP.StudioCore.Enums;
using SP.StudioCore.Http;
using SP.StudioCore.Model;
using SP.StudioCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BW.System.Filters
{
    public class SysFilter : ActionFilterAttribute
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
            PermissionAttribute permission = method.MethodInfo.GetAttribute<PermissionAttribute>();
            Guid token = context.HttpContext.Head<Guid>("token");
            SystemAdminModel admin = AdminAgent.Instance().GetAdminModel(token);

            if (!isGuest && !admin)
            {
                context.Result = (ContentResult)context.HttpContext.ShowError(ErrorType.Login);
                return;
            }

            context.HttpContext.SetItem(admin);

            base.OnActionExecuting(context);
        }
    }
}
