using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 查询余额之后的返回内容
    /// </summary>
    public sealed class BalanceResult : ResultBase
    {
        public BalanceResult(APIResultType code) : base(code)
        {
        }

        public BalanceResult(string userName, decimal balance) : this(APIResultType.Success)
        {
            this.UserName = userName;
            this.Balance = balance;
        }

        public string UserName { get; set; }

        public decimal Balance { get; set; }
    }
}
