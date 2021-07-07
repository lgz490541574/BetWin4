using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 登录的请求内容
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 指定的游戏登录
        /// </summary>
        public GameType Game { get; set; }

        /// <summary>
        /// 游戏用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码（如果需要的话）
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 指定要执行的玩法代码
        /// </summary>
        public string PlayCode { get; set; }
    }
}
