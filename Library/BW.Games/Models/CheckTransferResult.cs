using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 检查转账
    /// </summary>
    public class CheckTransferResult : ResultBase
    {
        public CheckTransferResult(APIResultType code) : base(code)
        {
        }

        public string OrderID { get; set; }

        public int GameID { get; set; }

        public string UserName { get; set; }

        public decimal Money { get; set; }

        public long CreateAt { get; set; }

    }
}
