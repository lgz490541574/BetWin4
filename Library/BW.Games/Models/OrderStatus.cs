using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus : byte
    {
        /// <summary>
        /// 等待开奖
        /// </summary>
        Wait,
        /// <summary>
        /// 退回本金
        /// </summary>
        Revoke,
        /// <summary>
        /// 赢
        /// </summary>
        Win,
        /// <summary>
        /// 输
        /// </summary>
        Lose
    }
}
