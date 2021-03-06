using BW.Common.Models.Enums;
using BW.Common.Models.Systems;
using SP.StudioCore.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Caching
{
    /// <summary>
    /// 系统管理
    /// </summary>
    internal class SystemCaching : CacheBase<SystemCaching>
    {
        protected override int DB_INDEX => RedisIndex.SYSTEM;

        private const string TOKEN = "TOKEN";

        internal Guid SaveToken(int adminId)
        {
            return base.SaveToken(TOKEN, adminId);
        }

        internal void RemoveToken(int adminId)
        {
            base.RemoveToken(TOKEN, adminId);
        }

        internal int GetAdminID(Guid token)
        {
            return base.GetTokenID(TOKEN, token);
        }

        private const string ADMIN_INFO = "ADMIN:INFO";

        internal SystemAdminModel GetAdminInfo(int adminId)
        {
            if (adminId == 0) return default;
            return this.NewExecutor().HashGet(ADMIN_INFO, adminId);
        }

        internal void SaveAdminInfo(SystemAdminModel admin)
        {
            this.NewExecutor().HashSet(ADMIN_INFO, admin.ID, admin);
        }

        /// <summary>
        /// 系统配置
        /// </summary>
        private const string SYSTEM_CONFIG = "SYSTEM:CONFIG";


        internal void SaveSystemConfig(ConfigType type,string value)
        {
            this.NewExecutor().HashSet(SYSTEM_CONFIG, type.GetRedisValue(), value.GetRedisValue());
        }

        internal string GetSystemConfig(ConfigType type)
        {
            return this.NewExecutor().HashGet(SYSTEM_CONFIG, type.GetRedisValue()).GetRedisValue<string>();
        }
    }
}
