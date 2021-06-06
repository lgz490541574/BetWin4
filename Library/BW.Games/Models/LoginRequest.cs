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
    public struct LoginRequest
    {
        /// <summary>
        /// 指定的游戏登录
        /// </summary>
        public int GameID;

        /// <summary>
        /// 游戏用户名
        /// </summary>
        public string UserName;

        /// <summary>
        /// 密码（如果需要的话）
        /// </summary>
        public string Password;

        /// <summary>
        /// 指定要执行的玩法代码
        /// </summary>
        public string PlayCode;
    }
}
