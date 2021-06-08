﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 注册内容
    /// </summary>
    public class RegisterRequest
    {
        public RegisterRequest() { }

        public RegisterRequest(string prefix, string userName) 
        {
            this.Prefix = prefix;
            this.UserName = userName;
        }

        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 要在游戏中注册的用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码（会员注册的时候才需要赋值）
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 增加前缀的用户名
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(this.Prefix, "_", this.UserName);
        }
    }
}
