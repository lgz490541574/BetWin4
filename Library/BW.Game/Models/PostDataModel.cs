using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Game.Models
{
    /// <summary>
    /// 发送的数据
    /// </summary>
    public struct PostDataModel
    {
        public PostDataModel(Dictionary<string, string> data)
        {
            this.Header = null;
            this.Original = null;
            this.Data = data;
        }

        public PostDataModel(Dictionary<string, string> original, Dictionary<string, string> data)
        {
            this.Header = null;
            this.Original = original;
            this.Data = data;
        }

        public PostDataModel(Dictionary<string, string> header, Dictionary<string, string> original, Dictionary<string, string> data)
        {
            this.Header = header;
            this.Original = original;
            this.Data = data;
        }

        /// <summary>
        /// 自定义的Header头
        /// </summary>
        public Dictionary<string, string> Header;

        /// <summary>
        /// 加密之前的数据(如果有的话）
        /// </summary>
        public Dictionary<string, string> Original;

        /// <summary>
        /// 加密之后的数据
        /// </summary>
        public Dictionary<string, string> Data;
    }
}
