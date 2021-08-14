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
using SP.StudioCore.Array;
using SP.StudioCore.Data.Repository;
using SP.StudioCore.Enums;
using SP.StudioCore.Ioc;
using SP.StudioCore.Services;
using SP.StudioCore.Utils;
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
                .AddSingleton(t => new BetWinDataContext(Setting.DataContextOptions()))
                .AddTransient<IWriteRepository>(t => Setting.WriteDbExecutor())
                .AddTransient<IReadRepository>(t => Setting.ReadDbExecutor())
                .Initialize()
                .AddService();

            // �״�ִ��д�����еĶ���
            GameCaching.Instance().SaveOrderQueue();
            if (args.Contains("-test"))
            {
                ConsoleHelper.WriteLine($"���س�����ʼ", ConsoleColor.Gray);
                Console.ReadLine();
                int count = args.Get("-count", 1);
                if (args.Contains("-game"))
                {
                    GameType game = args.Get("-game").ToEnum<GameType>();
                    ConsoleHelper.WriteLine($"��ʼ�ɼ� {game}/{count}", ConsoleColor.Green);
                    for (int i = 0; i < count; i++)
                    {
                        Console.WriteLine(i);
                        OrderAgent.Instance().GetOrders(game);
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        OrderAgent.Instance().GetOrders();
                    }
                }
                return;
            }
            // ���߳�ִ��
            Parallel.For(0, 4, index =>
            {
                while (true)
                {
                    OrderAgent.Instance().GetOrders();
                }
            });
        }
    }
}
