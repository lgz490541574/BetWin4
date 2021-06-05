using SP.StudioCore.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BW.Game.Models
{
    /// <summary>
    /// 登录之后的返回内容
    /// </summary>
    public struct LoginResult
    {
        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url"></param>
        public LoginResult(string url)
        {
            this.Url = url;
            this.Method = HttpMethod.Get;
            this.Data = null;
        }

        public LoginResult(string url, Dictionary<string, object> data)
        {
            this.Url = url;
            this.Method = HttpMethod.Post;
            this.Data = data;
        }

        public string Url;

        public HttpMethod Method;

        public Dictionary<string, object> Data;

        /// <summary>
        /// 转化成为JSON数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Method == HttpMethod.Post)
            {
                return new
                {
                    this.Url,
                    Method = "Post",
                    this.Data
                }.ToJson();
            }

            return new
            {
                this.Url
            }.ToJson();
        }
    }
}
