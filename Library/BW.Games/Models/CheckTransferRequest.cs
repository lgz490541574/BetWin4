using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 请求查询转账订单状态
    /// </summary>
    public class CheckTransferRequest
    {
        public string OrderID { get; set; }
    }
}
