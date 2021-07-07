using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 查询余额
    /// </summary>
    public class BalanceRequest
    {
        /// <summary>
        /// 要查询的用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码（如果需要的话）
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 所在游戏
        /// </summary>
        public GameType Game { get; set; }
    }
}
