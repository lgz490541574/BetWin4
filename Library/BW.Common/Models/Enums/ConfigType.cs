using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Enums
{
    /// <summary>
    /// 配置类型
    /// </summary>
    public enum ConfigType : byte
    {
        [Description("登录域名")]
        LoginUrl = 1
    }
}
