using BW.Common.Entities.DataContext;
using BW.Common.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SP.StudioCore.Consul;
using SP.StudioCore.Data.Repository;
using SP.StudioCore.Ioc;
using SP.StudioCore.Log;
using SP.StudioCore.Services;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Startup
{
    public abstract class BetWinStartupBase<TFilterType> where TFilterType : IFilterMetadata
    {
        /// <summary>
        /// 可写库的数据库上下文
        /// </summary>
        protected virtual IWriteDataContext WriteDataContext { get; }

        /// <summary>
        /// 项目自定义要注入的内容
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        protected virtual IServiceCollection Services(IServiceCollection services)
        {
            return services;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opt => { opt.Filters.Add<TFilterType>(); });

            services
                .AddSpLogging()
                .AddScoped(t => this.WriteDataContext)
                .AddScoped<IWriteRepository>(t => Setting.WriteDbExecutor())
                //.AddSingleton(t => Setting.NewElasticClient())
                .AddSingleton(t => new IPHeader(new[] { "X-Original-Forwarded-For" }))
                .AddCors(opt => opt.AddPolicy("Api", policy =>
                {
                    policy.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                    policy.AllowAnyHeader();
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                }))
                .Initialize()
                .AddService();

            this.Services(services);

            services
                .AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseConsul(lifetime)
                .UseHttpContext()
                .UseMiddleware<ExceptionMiddleware>()
                .UseStaticFiles()
                .UseRouting()
                .UseCors("Api")
                .UseAuthentication()
                .UseEndpoints(endpoints => { endpoints.MapControllers().RequireCors("Api"); });
        }
    }
}
