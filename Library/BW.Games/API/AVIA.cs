using BW.Games.Exceptions;
using BW.Games.Models;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Array;
using SP.StudioCore.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.API
{
    public class AVIA : IGameBase
    {
        #region ========  字段  ========

        [Description("网关")]
        public string Gateway { get; set; }

        [Description("商户号")]
        public int SiteID { get; set; }

        [Description("密钥")]
        public string SecretKey { get; set; }

        #endregion

        public AVIA(string queryString) : base(queryString)
        {
        }

        public override LoginResult Login(LoginRequest login)
        {
            APIResultType resultType = this.POST("user/login", new Dictionary<string, object>()
             {
                 { "UserName",login.UserName }
             }, out JObject info);
            if (resultType == APIResultType.Success)
            {
                return new LoginResult(info["Url"].Value<string>());
            }
            throw new APIResultException(resultType);
        }

        public override RegisterResult Register(RegisterRequest register)
        {
            string password = Guid.NewGuid().ToString("N").Substring(0, 8);
            APIResultType resultType = this.POST("user/register", new Dictionary<string, object>()
            {
                {"UserName",register.UserName },
                {"Password",password }
            }, out JObject _);
            if (resultType == APIResultType.Success || resultType == APIResultType.EXISTSUSER)
            {
                return new RegisterResult(register.UserName, password);
            }
            throw new APIResultException(resultType);
        }

        #region ========  工具方法  ========

        private APIResultType POST(string method, Dictionary<string, object> data, out JObject info)
        {
            string url = $"{this.Gateway}/api/{method}";
            string result = null;
            APIResultType resultType = APIResultType.Faild;
            info = null;
            try
            {
                result = NetAgent.UploadData(url, data.ToQueryString(), headers: new Dictionary<string, string>()
                {
                    { "Authorization",this.SecretKey }
                });
                JObject json = JObject.Parse(result);
                if (json["success"].Value<int>() == 1)
                {
                    if (json.ContainsKey("info")) info = (JObject)json["info"];
                    resultType = APIResultType.Success;
                }
                else
                {
                    if (info.ContainsKey("info")) info = info["info"].Value<JObject>();
                    if (info.ContainsKey("Error")) resultType = this.GetErrorCode(info["Error"].Value<string>());
                }
                return resultType;
            }
            catch (Exception ex)
            {
                result += ex.Message;
                return resultType;
            }
            finally
            {
                this.SaveLog(url, result, resultType, new PostDataModel(data));
            }
        }

        private APIResultType GetErrorCode(string code)
        {
            return code switch
            {
                "NOUSER" => APIResultType.NOUSER,
                "BADNAME" => APIResultType.BADNAME,
                "BADPASSWORD" => APIResultType.BADPASSWORD,
                "EXISTSUSER" => APIResultType.EXISTSUSER,
                "BADMONEY" => APIResultType.BADMONEY,
                "NOORDER" => APIResultType.NOORDER,
                "EXISTSORDER" => APIResultType.EXISTSORDER,
                "TRANSFER_NO_ACTION" => APIResultType.TRANSFER_NO_ACTION,
                "IP" => APIResultType.IP,
                "USERLOCK" => APIResultType.USERLOCK,
                "NOBALANCE" => APIResultType.NOBALANCE,
                "NOCREDIT" => APIResultType.NOCREDIT,
                "Authorization" => APIResultType.Authorization,
                "Faild" => APIResultType.Faild,
                "DOMAIN" => APIResultType.DOMAIN,
                "CONTENT" => APIResultType.CONTENT,
                "Sign" => APIResultType.Sign,
                "NOSUPPORT" => APIResultType.NOSUPPORT,
                "TIMEOUT" => APIResultType.TIMEOUT,
                "STATUS" => APIResultType.STATUS,
                "CONFIGERROR" => APIResultType.CONFIGERROR,
                "DATEEROOR" => APIResultType.DATEEROOR,
                "ORDER_NOTFOUND" => APIResultType.ORDER_NOTFOUND,
                "PROCCESSING" => APIResultType.PROCCESSING,
                _ => APIResultType.Faild
            };
        }

        #endregion
    }
}
