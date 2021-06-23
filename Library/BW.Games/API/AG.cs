using BW.Games.Exceptions;
using BW.Games.Models;
using Newtonsoft.Json.Linq;
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

        #region ========  工具方法  ========

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

        internal override PostResult POST(string method, Dictionary<string, object> data)
        {
            string apiParam = string.Join(@"/\\\\/", data.Select(t => string.Format("{0}={1}", t.Key, t.Value)));
            apiParam = this._desEncrypt(apiParam, KEY);
            string key = Encryption.toMD5(apiParam + this.Md5Key).ToLower();

            PostResult result = new()
            {
                Data = new()
                {
                    { "params", apiParam },
                    { "key", key }
                },
                Header = new()
                {
                    { "User-Agent", $"WEB_LIB_GI_{ Agent }" }
                },
                Original = data
            };
            result.Url = $"{ this.Gateway }?{ result.Data.ToQueryString() }";
            result.Result = NetAgent.DownloadData(result.Url, Encoding.UTF8, result.Header);
            XElement root = XElement.Parse(result.Result);

            if (string.IsNullOrEmpty(root.GetAttributeValue("msg", string.Empty)))
            {
                result.Code = APIResultType.Success;
            }
            else
            {
                result.Code = APIResultType.Faild;
            }
            return result;
        }

        /// <summary>
        /// 预备转账
        /// </summary>
        /// <param name="action">IN | OUT</param>
        private bool prepareTransferCredit(string userName, string password, decimal money, string action, string orderId)
        {
            Dictionary<string, object> data = new()
            {
                { "cagent", this.Agent },
                { "loginname", userName },
                { "method", "tc" },
                { "billno", orderId },
                { "type", action },
                { "actype", 1 },
                { "credit", money.ToString("0.00") },
                { "password", password },
                { "cur", this.CurMoney }
            };
            APIResultType resultType = this.POST(null, data, out _);
            if (resultType == APIResultType.Success) return true;
            throw new APIResultException(resultType);
        }

        #endregion

        public AG(string queryString) : base(queryString)
        {
        }

        public override LoginResult Login(LoginRequest login)
        {
            Dictionary<string, object> data = new()
            {
                { "cagent", this.Agent },
                { "loginname", login.UserName },
                { "password", login.Password },
                { "dm", "NO_RETURN" },
                { "sid", $"{this.Agent}{DateTime.Now:yyyyMMddHHmmss}{ WebAgent.GetRandom(1000, 9999) }" },
                { "actype", 1 },
                { "lang", 1 },
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
            string password = "a123456";
            Dictionary<string, object> data = new()
            {
                { "cagent", Agent },
                { "loginname", register.UserName },
                { "password", password },
                { "actype", Actype.ToString() },//1:代表真钱账号，0：代表试玩账号
                { "method", "lg" },
                { "oddtype", "A" },  //盘口
                { "cur", this.CurMoney }
            };
            APIResultType resultType = this.POST(null, data, out _);
            if (resultType == APIResultType.Success || resultType == APIResultType.EXISTSUSER) return new RegisterResult(register.UserName, password);
            return new RegisterResult(resultType);
        }

        public override TransferResult Recharge(TransferRequest transfer)
        {
            string sourceId = transfer.SourceID;
            sourceId = this.Agent + sourceId.PadLeft(16 - this.Agent.Length, '0');
            if (!this.prepareTransferCredit(transfer.UserName, transfer.Password, transfer.Money, "IN", sourceId)) return default;

            Dictionary<string, object> data = new()
            {
                { "cagent", this.Agent },
                { "loginname", transfer.UserName },
                { "method", "tcc" },
                { "billno", sourceId },
                { "type", "IN" },
                { "credit", transfer.Money.ToString("0.00") },
                { "actype", this.Actype.ToString() },
                { "flag", "1" },
                { "password", transfer.Password },
                { "cur", this.CurMoney }
            };

            APIResultType resultType = this.POST(null, data, out _);
            if (resultType == APIResultType.Success)
            {
                return new TransferResult(transfer.OrderID, sourceId, transfer.Money);
            }
            throw new APIResultException(resultType);
        }

        public override BalanceResult Balance(BalanceRequest balance)
        {
            throw new NotImplementedException();
        }
    }
}
