using BW.Common.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SP.StudioCore.Consul;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.API.Filters;

namespace Web.API
{
    public class Startup : BetWinStartupBase<APIFilter>
    {
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseConsul(lifetime)
                .UseHttpContext()
                .UseMiddleware<APIExceptionMiddleware>()
                .UseStaticFiles()
                .UseRouting()
                .UseCors("Api")
                .UseAuthentication()
                .UseEndpoints(endpoints => { endpoints.MapControllers().RequireCors("Api"); });

        }
    }
}
