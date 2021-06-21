using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Enums
{
    public enum SiteGameStatus : byte
    {
        [Description("未开启")]
        Stop = 0,
        [Description("正常")]
        Normal = 1
    }
}
