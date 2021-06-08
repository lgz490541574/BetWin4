using BW.Common.Agent.Systems;
using Web.System.Filters;
using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BW.Common.Models.Enums;
using BW.Common.Entities.Systems;

namespace Web.System.Controller
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class ConfigController : SysControllerBase
    {
        /// <summary>
        /// 全局系统基本参数
        /// </summary>
        /// <returns></returns>
        [Guest]
        public ContentResult Init()
        {
            return this.GetResultContent(new
            {
                Enum = ConfigAgent.Instance().GetEnums()
            });
        }

        /// <summary>
        /// 管理员菜单
        /// </summary>
        /// <returns></returns>
        public ContentResult Menu()
        {
            return this.GetResultContent(ConfigAgent.Instance().GetMenu());
        }

        public ContentResult GetConfigList()
        {
            return this.GetResultContent(ConfigAgent.Instance().GetSystemConfig());
        }

        public ContentResult SaveConfig([FromForm] ConfigType type, [FromForm] string value)
        {
            return this.GetResultContent(ConfigAgent.Instance().SaveSystemConfig(new SystemConfig
            {
                Type = type,
                Value = value
            }));
        }
    }
}
