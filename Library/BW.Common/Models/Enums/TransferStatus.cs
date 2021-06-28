using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Enums
{
    /// <summary>
    /// 转账订单状态
    /// </summary>
    public enum TransferStatus : byte
    {
        Progress = 0,
        Success = 1,
        Faild = 2,
        /// <summary>
        /// 异常
        /// </summary>
        Exception = 10
    }
}
