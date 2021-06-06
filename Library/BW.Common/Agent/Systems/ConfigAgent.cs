using BW.Common.Properties;
using BW.Common.Utils;
using BW.Games;
using SP.StudioCore.Enums;
using SP.StudioCore.Model;
using System;
using System.Collections.Generic;
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
    }
}
