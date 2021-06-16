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
            }, out JObject info);

            if (info.ContainsKey("loginUrl"))
            {
                string loginurl = info["loginUrl"].Value<string>().Replace("http://", "https://");
                return new LoginResult(loginurl);
            }

            throw new APIResulteException(result);
        }

        public override RegisterResult Register(RegisterRequest register)
        {
            APIResultType result = this.POST("/player/create", new Dictionary<string, object>()
            {
                {"agentCode",this.AgentCode },
                {"loginId",register.UserName }
            }, out JObject info);
            if (info.ContainsKey("loginId"))
            {
                return new RegisterResult(register.UserName, string.Empty);
            }
            return new RegisterResult(result);
        }

        private APIResultType POST(string method, Dictionary<string, object> data, out JObject info)
        {
            string url = $"{this.Gateway}{method}?{data.ToQueryString()}";
            string token = this.generateToken();
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                {"userCode",this.AgentCode },
                {"token",token }
            };
            string result = null;
            APIResultType status = APIResultType.Faild;
            try
            {
                result = NetAgent.DownloadData(url, Encoding.UTF8, header);
                info = JObject.Parse(result);
                return APIResultType.Success;
            }
            catch (Exception ex)
            {
                info = null;
                result += ex.Message;
                return status;
            }
            finally
            {
                this.SaveLog(url, result, status == APIResultType.Success, new PostDataModel
                {
                    Data = data,
                    Header = header
                });
            }

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
    }
}
