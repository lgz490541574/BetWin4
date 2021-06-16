using BW.Games.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SP.StudioCore.Enums;
using SP.StudioCore.Http;
using SP.StudioCore.Mvc.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Startup
{
    /// <summary>
    /// 异常中间件
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IWebHostEnvironment env)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (ResultException ex)
            {
                await ex.WriteAsync(context);
            }
            catch (APIResultException ex)
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(ex.ToString());
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.ShowError(ErrorType.Exception, ex.Message, new Dictionary<string, object>()
                {
                    {"RequestID", Guid.NewGuid().ToString("N")}
                }).WriteAsync(context).ConfigureAwait(true);
            }
        }
    }
}
