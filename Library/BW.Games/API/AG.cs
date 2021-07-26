using BW.Games.Exceptions;
using BW.Games.Models;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Array;
using SP.StudioCore.Json;
using SP.StudioCore.Net;
using SP.StudioCore.Security;
using SP.StudioCore.Types;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BW.Games.API
{
    /// <summary>
    /// AG
    /// </summary>
    public abstract class AG : IGameBase
    {
        #region =======  字段  ========

        [Description("网关")]
        public string Gateway { get; set; }

        [Description("游戏地址")]
        public string ForwardGame { get; set; }

        [Description("密钥")]
        public string KEY { get; set; }

        /// <summary>
        /// 代理编码
        /// </summary>
        [Description("代理")]
        public string Agent { get; set; }

        [Description("md5Key")]
        public string Md5Key { get; set; }

        [Description("币种")]
        public string CurMoney { get; set; } = "CNY";

        /// <summary>
        /// 调用API数据接口所需要用到的明码
        /// </summary>
        [Description("明码")]
        public string pidtoken { get; set; }

        #endregion

        #region ========  工具方法  ========

        private APIResultType GetErrorCode(int code)
        {
            return code switch
            {
                0 => APIResultType.Success,
                60001 => APIResultType.NOUSER,
                //Credit can't be negative
                10000 => APIResultType.BADMONEY,
                //Credit not enough
                10002 => APIResultType.NOBALANCE,
                _ => APIResultType.Faild
            };
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

        private PostResult doBusiness(Dictionary<string, object> data)
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
            result.Url = $"{ this.Gateway }/doBusiness.do?{ result.Data.ToQueryString() }";
            result.Result = NetAgent.DownloadData(result.Url, Encoding.UTF8, result.Header);
            XElement root = XElement.Parse(result.Result);
            result.Info = root;
            string msg = root.GetAttributeValue("msg", string.Empty);
            if (string.IsNullOrEmpty(msg))
            {
                result.Code = APIResultType.Success;
            }
            else if (Regex.IsMatch(msg, @"^error:\d+"))
            {
                int errorCode = Regex.Match(msg, @"^error:(?<Code>\d+)").Groups["Code"].Value.GetValue<int>();
                result.Code = this.GetErrorCode(errorCode);
            }
            else
            {
                result.Code = APIResultType.Faild;
            }
            return result;
        }

        private PostResult apiLog(string method, Dictionary<string, object> data)
        {
            if (!data.ContainsKey("cagent")) data.Add("cagent", "DF6");
            if (!data.ContainsKey("gametype")) data.Add("gametype", string.Empty);
            if (!data.ContainsKey("order")) data.Add("order", "reckontime");
            if (!data.ContainsKey("by")) data.Add("by", "ASC");
            if (!data.ContainsKey("page")) data.Add("page", 1);
            if (!data.ContainsKey("perpage")) data.Add("perpage", 500);
            data.Add("key",
                Encryption.toMD5(string.Concat("DF6", data["startdate"], data["enddate"], data["gametype"], data["order"],
                data["by"], data["page"], data["perpage"], this.pidtoken)).ToLower());

            PostResult result = new()
            {
                Data = data,
                Header = new()
                {
                    { "User-Agent", $"WEB_LIB_GI_{ Agent }" }
                },
                Url = $"{this.Gateway}/{method}?{data.ToQueryString()}"
            };
            result.Result = NetAgent.DownloadData(result.Url, Encoding.UTF8, result.Header);
            XElement root = XElement.Parse(result.Result);
            result.Code = this.GetErrorCode(root.Element("info").Value.GetValue<int>());
            result.Info = root;
            return result;
        }

        internal override PostResult POST(string method, Dictionary<string, object> data)
        {
            if (!string.IsNullOrEmpty(method))
            {
                return this.apiLog(method, data);
            }
            else
            {
                return this.doBusiness(data);
            }
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
                { "credit", money },
                { "password", password },
                { "cur", this.CurMoney }
            };
            APIResultType resultType = this.POST(null, data, out _);
            if (resultType == APIResultType.Success) return true;
            throw new APIResultException(resultType);
        }

        #endregion

        protected override TimeSpan OffsetTime => TimeSpan.FromHours(-4);

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
            string userName = this.GetUserName(register);
            string password = "a123456";
            Dictionary<string, object> data = new()
            {
                { "cagent", Agent },
                { "loginname", userName },
                { "password", password },
                { "actype", 1 },//1:代表真钱账号，0：代表试玩账号
                { "method", "lg" },
                { "oddtype", "A" },  //盘口
                { "cur", this.CurMoney }
            };
            APIResultType resultType = this.POST(null, data, out _);
            if (resultType == APIResultType.Success || resultType == APIResultType.EXISTSUSER) return new RegisterResult(userName, password);
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
                { "credit", transfer.Money },
                { "actype", 1 },
                { "flag", "1" },
                { "password", transfer.Password },
                { "cur", this.CurMoney }
            };

            APIResultType resultType = this.POST(null, data, out object info);
            if (resultType == APIResultType.Success)
            {
                return new TransferResult(transfer.OrderID, sourceId, transfer.Money);
            }

            throw new APIResultException(resultType);
        }

        public override TransferResult Withdraw(TransferRequest transfer)
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
                { "type", "OUT" },
                { "credit", transfer.Money },
                { "actype", 1 },
                { "flag", "1" },
                { "password", transfer.Password },
                { "cur", this.CurMoney }
            };

            APIResultType resultType = this.POST(null, data, out object info);
            if (resultType == APIResultType.Success)
            {
                return new TransferResult(transfer.OrderID, sourceId, transfer.Money);
            }

            throw new APIResultException(resultType);
        }

        public override BalanceResult Balance(BalanceRequest balance)
        {
            APIResultType resultType = this.POST(null, new Dictionary<string, object>
            {
                {"cagent",this.Agent },
                {"loginname",balance.UserName },
                {"method","gb" },
                {"actype",1 },
                {"password",balance.Password },
                {"cur",this.CurMoney }
            }, out object info);
            if (resultType == APIResultType.NOUSER)
            {
                if (this.Register(new RegisterRequest
                {
                    UserName = balance.UserName,
                    Password = balance.Password
                }))
                {
                    return new BalanceResult(balance.UserName, decimal.Zero);
                }
            }
            if (resultType != APIResultType.Success) throw new APIResultException(resultType);

            XElement root = (XElement)info;
            return new BalanceResult(balance.UserName, root.GetAttributeValue("info", decimal.Zero));
        }


    }
}
