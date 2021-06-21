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

        public TransferResult(string orderId) : base(APIResultType.Success)
        {
            this.OrderID = orderId;
        }

        public string OrderID { get; set; }
    }
}
