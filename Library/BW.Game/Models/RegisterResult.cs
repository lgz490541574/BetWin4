using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Game.Models
{
    /// <summary>
    /// 注册后的返回对象
    /// </summary>
    public struct RegisterResult
    {
        public RegisterResult(string player, string password)
        {
            this.Success = true;
            this.Player = player;
            this.Password = password;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success;

        /// <summary>
        /// 玩家用户名
        /// </summary>
        public string Player;

        /// <summary>
        /// 密码（如果有的话）
        /// </summary>
        public string Password;

        public static implicit operator bool(RegisterResult result)
        {
            return result.Success;
        }
    }
}
