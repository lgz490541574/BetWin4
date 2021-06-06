using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Enums
{
    public enum AdminStatus : byte
    {
        [Description("正常")]
        Normal = 1,
        [Description("锁定")]
        Lock = 2
    }
}
