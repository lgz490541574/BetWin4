using SP.StudioCore.Web;
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

        /// <summary>
        /// 密码（如果需要的话）
        /// </summary>
        public string Password { get; set; }

        public GameType Game { get; set; }

        public string OrderID { get; set; }

        public decimal Money { get; set; }

        private string sourceId;
        /// <summary>
        /// 转化成为纯数字的订单号（16位）
        /// </summary>
        public string SourceID
        {
            get
            {
                if (string.IsNullOrEmpty(sourceId))
                {
                    string date = DateTime.Now.ToString("yyMMddHHmmss");
                    string rnd = WebAgent.GetRandom(0, 9999).ToString().PadLeft(4, '0');
                    sourceId = string.Concat(date, rnd);
                }
                return sourceId;
            }
        }
    }
}
