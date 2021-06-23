using BW.Games.Exceptions;
using BW.Games.Models;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Array;
using SP.StudioCore.Net;
using SP.StudioCore.Security;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.API
{
    /// <summary>
    /// 平博
    /// </summary>
    public sealed class Pinnacle : IGameBase
    {
        #region ========  字段  ========

        [Description("网关")]
        public string Gateway { get; set; } = "https://api.ps3838.com/b2b/";

        [Description("游戏地址")]
        public string GameUrl { get; set; }

        [Description("商户号")]
        public string AgentCode { get; set; }

        [Description("商户Key")]
        public string AgentKey { get; set; }

        [Description("密钥")]
        public string SecretKey { get; set; }

        #endregion

        #region ========  工具方法  ========

        private APIResultType GetResultType(int code)
        {
            return code switch
            {
                0 => APIResultType.Success,
                306 => APIResultType.BADMONEY,
                311 => APIResultType.BADMONEY,
                _ => APIResultType.Faild
            };
        }

        internal override PostResult POST(string method, Dictionary<string, object> data)
        {
            string token = this.generateToken();
            PostResult result = new()
            {
                Url = $"{this.Gateway}{method}?{data.ToQueryString()}",
                Data = data,
                Header = new()
                {
                    { "userCode", this.AgentCode },
                    { "token", token }
                }
            };
            result.Result = NetAgent.DownloadData(result.Url, Encoding.UTF8, result.Header);
            JObject info = JObject.Parse(result.Result);
            result.Info = info;
            result.Code = info.ContainsKey("code") ? this.GetResultType(info["code"].Value<int>()) : APIResultType.Success;
            return result;
        }

        /// <summary>
        /// 获取密钥
        /// </summary>
        /// <returns></returns>
        private string generateToken()
        {
            long timeStamp = WebAgent.GetTimestamps();
            string hashToken = Encryption.toMD5(this.AgentCode + timeStamp + this.AgentKey).ToLower();
            string tokenPayLoad = string.Format("{0}|{1}|{2}", this.AgentCode, timeStamp, hashToken);
            using (var csp = new AesCryptoServiceProvider())
            {
                ICryptoTransform e = GetCryptoTransform(csp, true, this.SecretKey);
                byte[] inputBuffer = Encoding.UTF8.GetBytes(tokenPayLoad);
                byte[] output = e.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                string encrypted = Convert.ToBase64String(output);
                return encrypted;
            }
        }

        private ICryptoTransform GetCryptoTransform(AesCryptoServiceProvider csp, bool encrypting, string secretKey)
        {
            string INIT_VECTOR = "RandomInitVector";
            csp.Mode = CipherMode.CBC;
            csp.Padding = PaddingMode.PKCS7;
            byte[] key = Encoding.UTF8.GetBytes(secretKey);
            csp.IV = Encoding.UTF8.GetBytes(INIT_VECTOR);
            csp.Key = key;
            if (encrypting) { return csp.CreateEncryptor(); }
            return csp.CreateDecryptor();
        }


        #endregion

        public Pinnacle(string queryString) : base(queryString)
        {
        }

        public override LoginResult Login(LoginRequest login)
        {
            APIResultType result = this.POST("/player/login", new Dictionary<string, object>()
            {
                { "userCode",login.UserName },
                { "locale","zh-cn" },
                { "oddsFormat","EU" }
            }, out object info);

            if (((JObject)info).ContainsKey("loginUrl"))
            {
                string loginurl = ((JObject)info)["loginUrl"].Value<string>().Replace("http://", "https://");
                return new LoginResult(loginurl);
            }

            throw new APIResultException(result);
        }

        public override RegisterResult Register(RegisterRequest register)
        {
            APIResultType result = this.POST("/player/create", new Dictionary<string, object>()
            {
                {"agentCode",this.AgentCode },
                {"loginId",register.UserName }
            }, out object info);
            if (result == APIResultType.Success && ((JObject)info).ContainsKey("loginId"))
            {
                return new RegisterResult(register.UserName, string.Empty);
            }
            return new RegisterResult(result);
        }

        public override TransferResult Recharge(TransferRequest transfer)
        {
            string sourceId = transfer.SourceID;
            Dictionary<string, object> data = new()
            {
                { "userCode", transfer.UserName },
                { "amount", transfer.Money },
                { "transactionId", sourceId }
            };

            APIResultType resultType = this.POST("/player/deposit", data, out object info);
            if (resultType != APIResultType.Success) throw new APIResultException(resultType);

            transfer.Money = ((JObject)info).ContainsKey("amount") ? ((JObject)info)["amount"].Value<decimal>() : decimal.Zero;

            if (transfer.Money == decimal.Zero)
            {
                throw new APIResultException(APIResultType.Faild);
            }

            return new TransferResult(transfer.OrderID, sourceId, transfer.Money, ((JObject)info)["availableBalance"].Value<decimal>());
        }

        public override BalanceResult Balance(BalanceRequest balance)
        {
            throw new NotImplementedException();
        }
    }
}
