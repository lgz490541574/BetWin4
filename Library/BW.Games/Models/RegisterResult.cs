using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 注册后的返回对象
    /// </summary>
    public class RegisterResult : ResultBase
    {
        public RegisterResult(string player, string password) :base(APIResultType.Success)
        {
            this.UserName = player;
            this.Password = password;
        }

        public RegisterResult(APIResultType code) : base(code)
        {
        }

        /// <summary>
        /// 玩家用户名
        /// </summary>
        public string UserName;

        /// <summary>
        /// 密码（如果有的话）
        /// </summary>
        public string Password;

        public static implicit operator bool(RegisterResult result)
        {
            return result.Code == APIResultType.Success;
        }
    }
}
