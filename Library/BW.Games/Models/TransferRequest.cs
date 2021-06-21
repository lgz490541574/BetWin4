using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 转入资金
    /// </summary>
    public class TransferRequest
    {
        public string UserName { get; set; }

        public int GameID { get; set; }

        public string OrderID { get; set; }

        public decimal Money { get; set; }
    }
}
