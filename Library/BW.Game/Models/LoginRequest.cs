using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Game.Models
{
    /// <summary>
    /// 登录的请求内容
    /// </summary>
    public struct LoginRequest
    {
        /// <summary>
        /// 游戏用户名
        /// </summary>
        public string Player;

        /// <summary>
        /// 密码（如果需要的话）
        /// </summary>
        public string Password;


    }
}
