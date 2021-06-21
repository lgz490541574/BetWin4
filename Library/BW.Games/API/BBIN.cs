using BW.Games.Exceptions;
using BW.Games.Models;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Array;
using SP.StudioCore.Mvc.Exceptions;
using SP.StudioCore.Net;
using SP.StudioCore.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.API
{
    public class BBIN : IGameBase
    {
        #region ==========　参数设定　　===============

        /// <summary>
        /// 登录地址
        /// </summary>
        [Description("登录")]
        public string ForwardGame { get; set; }

        /// <summary>
        /// Login2 登录地址
        /// </summary>
        [Description("登录2")]
        public string Login2 { get; set; }

        /// <summary>
        /// API网关
        /// </summary>
        [Description("API网关")]
        public string Gateway { get; set; }

        [Description("网站名称")]
        public string WebSite { get; set; }

        [Description("上层帐号")]
        public string UpperName { get; set; }

        /// <summary>
        /// 新增会员
        /// </summary>
        [Description("CreateMember")]
        public string KEYCreateMember { get; set; }

        [Description("Login")]
        public string KEYLogin { get; set; }

        [Description("Login2")]
        public string KEYLogin2 { get; set; }

        [Description("CheckUsrBalance")]
        public string KEYCheckUsrBalance { get; set; }

        [Description("Transfer")]
        public string KEYTransfer { get; set; }

        [Description("CheckTransfer")]
        public string KEYCheckTransfer { get; set; }

        [Description("TransferRecord")]
        public string KEYTransferRecord { get; set; }

        [Description("PlayGame")]
        public string KEYPlayGame { get; set; }

        [Description("PlayGameByH5")]
        public string KEYPlayGameByH5 { get; set; }

        [Description("BetRecord")]
        public string KEYBetRecord { get; set; }

        [Description("BetRecordByModifiedDate3")]
        public string KEYBetRecordByModifiedDate3 { get; set; }

        [Description("GetJPHistory")]
        public string KEYGetJPHistory { get; set; }

        [Description("ForwardGameH5By5")]
        public string KEYForwardGameH5By5 { get; set; }

        [Description("WagersRecordBy38")]
        public string KEYWagersRecordBy38 { get; set; }

        [Description("WagersRecordBy30")]
        public string KEYWagersRecordBy30 { get; set; }

        [Description("ForwardGameH5By30")]
        public string KEYForwardGameH5By30 { get; set; }

        [Description("ForwardGameH5By38")]
        public string KEYForwardGameH5By38 { get; set; }

        [Description("GetFishEventHistory ")]
        public string KEYGetFishEventHistory { get; set; }

        [Description("FishEventUrl")]
        public string KEYFishEventUrl { get; set; }

        [Description("GetSKEventHistory")]
        public string KEYGetSKEventHistory { get; set; }

        [Description("SKEventUrl")]
        public string KEYSKEventUrl { get; set; }

        #endregion

        #region ========  工具方法  ========

        internal override PostResult POST(string method, Dictionary<string, object> data)
        {
            data.Add("website", this.WebSite);
            data.Add("uppername", this.UpperName);

            PostResult result = new()
            {
                Url = $"{this.Gateway}/{method}?{data.ToQueryString()}",
                Data = data
            };
            if (method.StartsWith("http"))
            {
                result.Url = $"{method}?{data.ToQueryString()}";
            }
            result.Result = NetAgent.DownloadData(result.Url, Encoding.UTF8);
            JObject info;
            result.Info = info = JObject.Parse(result.Result);
            if (info["result"].Value<bool>())
            {
                result.Code = APIResultType.Success;
            }
            else
            {
                if (info.ContainsKey("data"))
                {
                    info = info["data"].Value<JObject>();
                    result.Code = GetErrorCode(info["Code"].Value<int>());
                }
            }
            return result;
        }

        /// <summary>
        /// 获取密钥
        /// </summary>
        private string GetKey(int aLength, string value, Func<BBIN, string> key, int cLength)
        {
            string timesTamp = DateTime.Now.AddHours(-12).ToString("yyyyMMdd");
            string keyA = Guid.NewGuid().ToString("N").Substring(0, aLength);
            string keyC = Guid.NewGuid().ToString("N").Substring(0, cLength);
            string valueB = this.WebSite + value + key(this) + timesTamp;
            string keyB = Encryption.toMD5(valueB).ToLower();
            return string.Concat(keyA, keyB, keyC);
        }

        private APIResultType GetErrorCode(int code)
        {
            return code switch
            {
                47005 => APIResultType.BADNAME,
                44900 => APIResultType.IP,
                _ => APIResultType.Faild
            };
        }

        #endregion

        public BBIN(string queryString) : base(queryString)
        {
        }

        public override LoginResult Login(LoginRequest login)
        {
            Dictionary<string, object> data = new()
            {
                { "website", this.WebSite },
                { "username", login.UserName },
                { "uppername", this.UpperName }
            };

            data.Add("page_site", "live");
            data.Add("page_present", "live");
            data.Add("key", this.GetKey(8, login.UserName, t => t.KEYLogin, 1));
            string method = "Login";

            return new LoginResult($"{this.ForwardGame}/{method}?{data.ToQueryString()}");
        }

        public override RegisterResult Register(RegisterRequest register)
        {
            string userName = register.UserName.Replace("_", "0");
            string password = Guid.NewGuid().ToString("N").Substring(0, 8);
            Dictionary<string, object> data = new()
            {
                { "username", userName },
                { "password", password },
                { "key", this.GetKey(7, userName, t => t.KEYCreateMember, 1) }
            };
            APIResultType resultType = this.POST("CreateMember", data, out object info);
            if (resultType == APIResultType.Success || resultType == APIResultType.EXISTSUSER)
            {
                return new RegisterResult(userName, password);
            }
            throw new APIResultException(resultType);
        }

        public override TransferResult Recharge(TransferRequest transfer)
        {
            throw new NotImplementedException();
        }

    }
}
