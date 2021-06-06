using BW.Games.Models;
using SP.StudioCore.Array;
using SP.StudioCore.Net;
using SP.StudioCore.Security;
using SP.StudioCore.Web;
using SP.StudioCore.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BW.Games.API
{
    public sealed class AG : IGameBase
    {
        #region =======  字段  ========

        [Description("网关")]
        public string Gateway { get; set; }

        [Description("游戏地址")]
        public string ForwardGame { get; set; } = "https://gci.avia01.com/forwardGame.do";

        [Description("密钥")]
        public string KEY { get; set; }

        [Description("代理")]
        public string Agent { get; set; }

        [Description("md5Key")]
        public string Md5Key { get; set; }

        [Description("币种")]
        public string CurMoney { get; set; } = "CNY";

        /// <summary>
        /// 真钱/试玩
        /// </summary>
        [Description("真钱or试玩")]
        public int Actype { get; set; } = 1;


        #endregion

        public AG(string queryString) : base(queryString)
        {
        }

        public override LoginResult Login(LoginRequest login)
        {
            Dictionary<string, string> data = new()
            {
                { "cagent", this.Agent },
                { "loginname", login.UserName },
                { "password", login.Password },
                { "dm", "NO_RETURN" },
                { "sid", $"{this.Agent}{DateTime.Now:yyyyMMddHHmmss}{ WebAgent.GetRandom(1000, 9999) }" },
                { "actype", this.Actype.ToString() },
                { "lang", "1" },
                { "oddtype", "A" },
                { "cur", this.CurMoney }
            };

            string apiParam = this._desEncrypt(string.Join(@"/\\\\/", data.Select(t => string.Format("{0}={1}", t.Key, t.Value))), this.KEY);
            string key = Encryption.toMD5(apiParam + this.Md5Key).ToLower();

            Dictionary<string, string> postData = new()
            {
                { "params", apiParam },
                { "key", key }
            };

            return new LoginResult($"{this.ForwardGame}?{postData.ToQueryString()}");
        }


        public override RegisterResult Register(RegisterRequest register)
        {
            string userName = register.ToString();
            string password = "a123456";
            Dictionary<string, string> data = new()
            {
                { "cagent", Agent },
                { "loginname", userName },
                { "password", password },
                { "actype", Actype.ToString() },//1:代表真钱账号，0：代表试玩账号
                { "method", "lg" },
                { "oddtype", "A" },  //盘口
                { "cur", this.CurMoney }
            };
            bool? success = this._request(data, out string msg, out Dictionary<string, string> result);
            if (success.HasValue && success.Value) return new RegisterResult(userName, password);
            return new RegisterResult(APIResultType.Faild);
        }



        #region ========  工具方法  ========

        private bool? _request(Dictionary<string, string> data, out string msg, out Dictionary<string, string> result)
        {
            string url = this.Gateway + "?";
            string apiParam = string.Join(@"/\\\\/", data.Select(t => string.Format("{0}={1}", t.Key, t.Value)));
            apiParam = this._desEncrypt(apiParam, KEY);
            string key = Encryption.toMD5(apiParam + this.Md5Key).ToLower();
            Dictionary<string, string> postData = new()
            {
                { "params", apiParam },
                { "key", key }
            };
            string returnXml = null;
            result = new Dictionary<string, string>();
            Dictionary<string, string> header = new()
            {
                { "User-Agent", $"WEB_LIB_GI_{ Agent }" }
            };
            bool success = false;
            url = $"{url}{postData.ToQueryString()}";
            try
            {
                returnXml = NetAgent.DownloadData(url, Encoding.UTF8, header);
                XElement root = XElement.Parse(returnXml);
                msg = root.GetAttributeValue("msg", string.Empty);
                result.Add("info", root.GetAttributeValue("info", string.Empty));
                return success = string.IsNullOrEmpty(msg);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                this.SaveLog(url, returnXml, success, new PostDataModel(header, data, postData));
            }
        }

        private string _desEncrypt(string str, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            des.Key = Encoding.ASCII.GetBytes(key);
            des.IV = Encoding.ASCII.GetBytes(key);
            using (MemoryStream ms = new MemoryStream())
            {
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                byte[] array = ms.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    byte b = array[i];
                    ret.AppendFormat("{0:X2}", b);
                }
                ret.ToString();
                return ret.ToString();
            }
        }


        #endregion
    }
}
