using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SP.StudioCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Utils
{
    public static class Setting
    {
        internal static readonly string DbConnection;

        internal static readonly string RedisConnection;

        static Setting()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();

            DbConnection = configuration.GetConnectionString("DbConnection");
            RedisConnection = configuration.GetConnectionString("RedisConnection");
        }

        public static DbExecutor WriteDbExecutor()
        {
            return new DbExecutor(DbConnection, DatabaseType.SqlServer);
        }

        /// <summary>
        /// 可写库的EF配置库
        /// </summary>
        /// <returns></returns>
        public static DbContextOptions WriteOptions()
        {
            return new DbContextOptionsBuilder().UseSqlServer(DbConnection).Options;
        }
    }
}
