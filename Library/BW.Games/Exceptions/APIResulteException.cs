using BW.Games.Models;
using SP.StudioCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BW.Games.Exceptions
{
    public class APIResulteException : Exception
    {
        /// <summary>
        /// 异常枚举（用于标签化多语言配置）
        /// </summary>
        public APIResultType Type { get; set; }

        /// <summary>
        /// 支持多语言的异常处理
        /// </summary>
        /// <param name="type">自定义异常枚举类型</param>
        public APIResulteException(APIResultType type, string message = null) : base(message ?? type.GetDescription())
        {
            Type = type;
        }

        public override string ToString()
        {
            return string.Concat("{\"Code\":\"", this.Type, "\",\"Message\":\"", HttpUtility.JavaScriptStringEncode(this.Message), "\"}");
        }
    }
}
