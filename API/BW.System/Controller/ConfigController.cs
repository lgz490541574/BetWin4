using BW.Common.Agent.Systems;
using BW.System.Filters;
using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BW.System.Controller
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
    }
}
