using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BW.Games.Models
{
    /// <summary>
    /// 统一的提交返回对象
    /// </summary>
    internal class PostResult
    {
        /// <summary>
        /// 返回代码
        /// </summary>
        public APIResultType Code { get; set; }

        /// <summary>
        /// 请求的地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 原始的返回内容
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 如果发生异常
        /// </summary>
        public Exception Ex { get; set; }

        /// <summary>
        /// 对外导出的数据
        /// </summary>
        public object Info { get; set; }

        /// <summary>
        /// 自定义的Header头
        /// </summary>
        public Dictionary<string, string> Header { get; set; }

        /// <summary>
        /// 加密之前的数据(如果有的话）
        /// </summary>
        public Dictionary<string, object> Original { get; set; }

        /// <summary>
        /// 加密之后的数据
        /// </summary>
        public Dictionary<string, object> Data { get; set; }

        public static implicit operator PostDataModel(PostResult post)
        {
            return new PostDataModel(post.Header, post.Original, post.Data);
        }

        internal string GetResult()
        {
            if (this.Ex == null) return this.Result;
            return string.Concat("{\"Exception\":\"", HttpUtility.JavaScriptStringEncode(Ex.Message),
                "\",\"Result\":\"", HttpUtility.JavaScriptStringEncode(this.Result), "\"}");
        }
    }
}
