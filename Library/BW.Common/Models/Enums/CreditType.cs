using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Enums
{
    /// <summary>
    /// 额度账变
    /// </summary>
    public enum CreditType
    {
        /// <summary>
        /// 会员上分
        /// </summary>
        UserRecharge,
        /// <summary>
        /// 会员下分
        /// </summary>
        UserWithdraw,
        /// <summary>
        /// 余额转入
        /// </summary>
        Transfer
    }
}
