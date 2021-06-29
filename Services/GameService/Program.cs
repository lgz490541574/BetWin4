using BW.Common.Agent.Games;
using BW.Common.Caching;
using BW.Common.Entities.DataContext;
using BW.Common.Entities.Games;
using BW.Common.Utils;
using BW.Games;
using BW.Games.API;
using BW.Games.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SP.StudioCore.Data.Repository;
using SP.StudioCore.Ioc;
using SP.StudioCore.Services;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new ServiceCollection()
                .AddScoped(t => new BetWinDataContext(Setting.DataContextOptions()))
                .AddScoped<IWriteRepository>(t => Setting.WriteDbExecutor())
                .AddScoped<IReadRepository>(t => Setting.ReadDbExecutor())
                .Initialize()
                .AddService();

            // 首次执行写入所有的队列
            GameCaching.Instance().SaveOrderQueue();

            // 多线程执行
            Parallel.For(0, 1, index =>
            {
                while (true)
                {
                    OrderAgent.Instance().GetOrders(2);
                }
            });
        }
    }
}
