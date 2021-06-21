using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    public class TransferResult : ResultBase
    {
        public TransferResult(APIResultType code) : base(code)
        {
        }


        public TransferResult(string orderId, string sourceId, decimal money, decimal? balance = null) : base(APIResultType.Success)
        {
            this.OrderID = orderId;
            this.SourceID = sourceId;
            this.Money = money;
            this.Balance = balance;
        }

        public string OrderID { get; set; }

        public string SourceID { get; set; }

        /// <summary>
        /// 实际转账的金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 转账之后的余额（如果接口有返回的话)
        /// </summary>
        public decimal? Balance { get; set; }
    }
}
