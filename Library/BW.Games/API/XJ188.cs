using BW.Games.Exceptions;
using BW.Games.Models;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Array;
using SP.StudioCore.Net;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BW.Games.API
{
    public class XJ188 : IGameBase
    {
        #region ========  参数配置  ========

        [Description("网关")]
        public string Gateway { get; set; }

        [Description("代理")]
        public string Agent { get; set; }

        [Description("商户")]
        public string ProjectId { get; set; }

        [Description("前缀")]
        public string Prefix { get; set; }


        [Description("操作员")]
        public string OperatorID { get; set; }

        [Description("密钥")]
        public string VendoID { get; set; }

        [Description("登录地址")]
        public string LoginUrl { get; set; }

        /// <summary>
        /// 默认的币种
        /// </summary>
        public string Currency { get; set; } = "CNY";

        #endregion

        #region ========  工具方法  ========
               

        internal override PostResult POST(string method, Dictionary<string, object> data)
        {
            if (!data.ContainsKey("operatorId")) data.Add("operatorId", OperatorID);
            if (!data.ContainsKey("vendorId")) data.Add("vendorId", System.Web.HttpUtility.UrlEncode(VendoID));
            PostResult result = new()
            {
                Url = $"{this.Gateway}/{method}",
                Data = data
            };
            result.Result = NetAgent.UploadData(result.Url, result.Data.ToQueryString(), Encoding.UTF8);
            JObject info = JObject.Parse(result.Result);
            result.Info = info;
            result.Code = this.GetResultType(info["code"].Value<string>());
            return result;
        }

        private APIResultType GetResultType(string code)
        {
            return code switch
            {
                "COMM0000" => APIResultType.Success,
                "COMM0101" => APIResultType.CONTENT,
                // 會員登錄拒絕。可能是由於某些原因，例如：非合法參數
                "SSO0001" => APIResultType.CONTENT,
                // Token无效
                "SSO0003" => APIResultType.CONTENT,
                "ITGR0011" => APIResultType.NOUSER,
                // 無效的 Reference ID
                "ITGR0012" => APIResultType.CONTENT,
                // 會員編號已存在
                "ITGR0033" => APIResultType.EXISTSUSER,
                //小於請求間隔的限制
                "ITGR0036" => APIResultType.BUSY,
                // 会员币別不一致
                "ITGR0101" => APIResultType.CONTENT,
                //运营商交易參考ID已經存在
                "ITGR1001" => APIResultType.EXISTSORDER,
                // 数据不存在
                "ITGR1002" => APIResultType.NOORDER,
                // 通用错误
                "ITGR9999" => APIResultType.Faild,
                // 會員餘額不足
                "ERRG0010" => APIResultType.NOBALANCE,
                _ => APIResultType.Faild
            };
        }

        #endregion

        public XJ188(string queryString) : base(queryString)
        {
        }

        public override LoginResult Login(LoginRequest login)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "memberCode", login.UserName },
                { "channel",WebAgent.IsMobile() ? 11 : 10 }
            };
            APIResultType resultType = this.POST("/API/Launch", data, out object info);
            if (resultType != APIResultType.Success) throw new APIResultException(resultType);

            string url = ((JObject)info)["data"].Value<string>();
            return new LoginResult($"{LoginUrl}/launcher?XJlaunchURL={ HttpUtility.UrlEncode(url.Replace("http://", "https://")) }");
        }

        public override RegisterResult Register(RegisterRequest register)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
                {
                    { "memberCode", register.UserName },
                    { "currencyCode", this.Currency },
                    { "oddsType", "1"}
                };
            APIResultType resultType = this.POST("/API/Registration", data, out object info);
            if (resultType == APIResultType.Success || resultType == APIResultType.EXISTSUSER)
            {
                return new RegisterResult(register.UserName, register.Password);
            }
            throw new APIResultException(resultType);
        }

        public override TransferResult Recharge(TransferRequest transfer)
        {
            throw new NotImplementedException();
        }
    }
}
