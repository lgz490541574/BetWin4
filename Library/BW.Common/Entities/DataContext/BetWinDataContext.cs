using BW.Common.Entities.Games;
using BW.Common.Entities.Sites;
using BW.Common.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Entities.DataContext
{
    public sealed class BetWinDataContext : DbContext, IWriteDataContext
    {
        public static readonly ILoggerFactory efLogger = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information).AddConsole();
        });

        public BetWinDataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(efLogger);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameUser>().HasKey(t => new { t.Type, t.SiteID, t.UserID });
            modelBuilder.Entity<GameOrderDetail>().HasKey(t => new { t.Type, t.OrderID });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Games.Game> Game { get; set; }

        public DbSet<GameUser> GameUser { get; set; }

        public DbSet<Site> Site { get; set; }

        public DbSet<User> User { get; set; }

        /// <summary>
        /// 游戏日志
        /// </summary>
        public DbSet<GameOrder> GameOrder { get; set; }

        /// <summary>
        /// 原始日志内容
        /// </summary>
        public DbSet<GameOrderDetail> GameOrderDetail { get; set; }

    }
}
