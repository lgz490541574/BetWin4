using BW.Common.Startup;
using BW.Games.Exceptions;
using BW.Games.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SP.StudioCore.Json;
using SP.StudioCore.Mvc.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.API.Filters
{
    public class APIExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<APIExceptionMiddleware> _logger;

        public APIExceptionMiddleware(RequestDelegate next, ILogger<APIExceptionMiddleware> logger)
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
            catch (APIResultException ex)
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(ex.ToString());
            }
            catch (Exception ex)
            {

                await context.Response.WriteAsync(new
                {
                    Code = APIResultType.Exception,
                    ex.Message,
                    RequestID = Guid.NewGuid().ToString("N")
                }.ToJson());
            }
        }
    }
}
