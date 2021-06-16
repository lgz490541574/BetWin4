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
            bool? success = this._post("user/login", new Dictionary<string, object>()
             {
                 { "UserName",login.UserName }
             }, out JObject info);
            if (success == null) throw new APIResulteException(APIResultType.Faild);

            if (success.Value)
            {
                string url = info["Url"].Value<string>();
                return new LoginResult(url);
            }

            APIResultType errorCode = this.GetErrorCode(info["Error"].Value<string>());
            throw new APIResulteException(errorCode);

        }

        public override RegisterResult Register(RegisterRequest register)
        {
            string password = Guid.NewGuid().ToString("N").Substring(0, 8);
            string userName = register.ToString();
            bool? success = this._post("user/register", new Dictionary<string, object>()
            {
                {"UserName",userName },
                {"Password",password }
            }, out JObject info);
            if (success == null) throw new APIResulteException(APIResultType.Faild);

            if (success.Value) return new RegisterResult(userName, password);

            APIResultType errorCode = this.GetErrorCode(info["Error"].Value<string>());
            return errorCode switch
            {
                APIResultType.EXISTSUSER => new RegisterResult(userName, password),
                _ => throw new APIResulteException(errorCode)
            };
        }

        #region ========  工具方法  ========

        private bool? _post(string method, Dictionary<string, object> data, out JObject info)
        {
            string url = $"{this.Gateway}/api/{method}";
            string result = NetAgent.UploadData(url, data.ToQueryString(), headers: new Dictionary<string, string>()
            {
                { "Authorization",this.SecretKey }
            });
            bool? success = null;
            info = null;
            try
            {
                JObject json = JObject.Parse(result);
                success = json["success"].Value<int>() == 1;
                if (json.ContainsKey("info")) info = (JObject)json["info"];
            }
            catch
            {
                return null;
            }
            finally
            {
                this.SaveLog(url, result, success.HasValue && success.Value, new PostDataModel(data));
            }
            return success;
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
