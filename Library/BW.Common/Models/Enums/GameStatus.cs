using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Enums
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameStatus : byte
    {
        [Description("正常")]
        Normal = 1,
        [Description("维护中")]
        Maintain = 2
    }
}
