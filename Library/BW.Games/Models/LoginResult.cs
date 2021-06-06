using SP.StudioCore.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 登录之后的返回内容
    /// </summary>
    public sealed class LoginResult : ResultBase
    {
        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url"></param>
        public LoginResult(string url) : base(APIResultType.Success)
        {
            this.Url = url;
            this.Method = HttpMethod.Get;
            this.Data = null;
        }

        public LoginResult(APIResultType code) : base(code)
        {
        }

        public LoginResult(string url, Dictionary<string, object> data) : base(APIResultType.Success)
        {
            this.Url = url;
            this.Method = HttpMethod.Post;
            this.Data = data;
        }

        public string Url { get; set; }

        public HttpMethod Method { get; set; }

        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// 转化成为JSON数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Code != APIResultType.Success)
            {
                return new
                {
                    this.Code,
                }.ToJson();
            }
            if (this.Method == HttpMethod.Post)
            {
                return new
                {
                    this.Code,
                    this.Url,
                    Method = "Post",
                    this.Data
                }.ToJson();
            }

            return new
            {
                this.Code,
                this.Url
            }.ToJson();
        }
    }
}
