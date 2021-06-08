using BW.Common.Caching;
using BW.Common.Entities.Systems;
using BW.Common.Models.Enums;
using BW.Common.Properties;
using BW.Common.Utils;
using BW.Games;
using SP.StudioCore.Data;
using SP.StudioCore.Enums;
using SP.StudioCore.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BW.Common.Agent.Systems
{
    /// <summary>
    /// 全局的配置
    /// </summary>
    public class ConfigAgent : AgentBase<ConfigAgent>
    {
        /// <summary>
        /// 获取枚举
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> GetEnums()
        {
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            foreach (Assembly assembly in new[] { Assembly.Load("BW.Common"), Assembly.Load("BW.Games"), Assembly.Load("SP.StudioCore") })
            {
                foreach (var item in assembly.GetEnums())
                {
                    dic.Add(item.Key, item.Value);
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取菜单（不区分权限）
        /// </summary>
        /// <returns></returns>
        public string GetMenu()
        {
            XElement root = XElement.Parse(Resources.SystemPermission);
            return new AdminMenu(root, false, null).ToString();
        }

        /// <summary>
        /// 存入系统配置内容
        /// </summary>
        /// <returns></returns>
        public bool SaveSystemConfig(SystemConfig config)
        {
            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                if (db.Exists(config))
                {
                    db.Update(config);
                }
                else
                {
                    db.Insert(config);
                }

                db.AddCallback(() =>
                {
                    SystemCaching.Instance().SaveSystemConfig(config.Type, config.Value);
                });

                db.Commit();
            }

            return true;
        }

        public string GetSystemConfig(ConfigType type)
        {
            string value = SystemCaching.Instance().GetSystemConfig(type);
            if (string.IsNullOrEmpty(value))
            {
                SystemConfig config = this.ReadDB.ReadInfo<SystemConfig>(t => t.Type == type);
                if (config != null) SystemCaching.Instance().SaveSystemConfig(config.Type, value = config.Value);
            }
            return value;
        }

        public IEnumerable<SystemConfig> GetSystemConfig()
        {
            var list = this.ReadDB.ReadList<SystemConfig>().ToList();
            foreach (ConfigType type in Enum.GetValues(typeof(ConfigType)))
            {
                if (!list.Any(t => t.Type == type))
                {
                    list.Add(new SystemConfig { Type = type });
                }
            }
            return list;
        }
    }
}
