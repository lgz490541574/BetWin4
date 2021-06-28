using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    public class OrderRequest
    {

        /// <summary>
        /// 游戏
        /// </summary>
        public int GameID { get; set; }

        /// <summary>
        /// 时间戳起始点
        /// </summary>
        public long Time { get; set; }


    }
}
