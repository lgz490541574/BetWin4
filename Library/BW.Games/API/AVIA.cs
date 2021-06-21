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
             }, out object info);
            if (resultType == APIResultType.Success)
            {
                return new LoginResult(((JObject)info)["Url"].Value<string>());
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
            }, out _);
            if (resultType == APIResultType.Success || resultType == APIResultType.EXISTSUSER)
            {
                return new RegisterResult(register.UserName, password);
            }
            throw new APIResultException(resultType);
        }

        /// <summary>
        /// 转入资金
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns></returns>
        public override TransferResult Recharge(TransferRequest transfer)
        {
            string sourceId = transfer.SourceID;
            APIResultType type = this.POST("user/transfer", new Dictionary<string, object>()
            {
                {"UserName",transfer.UserName },
                {"Type","IN" },
                {"Money",transfer.Money.ToString("0.00") },
                {"ID",sourceId }
            }, out object info);

            if (type == APIResultType.Success)
            {
                return new TransferResult(transfer.OrderID,
                    ((JObject)info)["OrderID"].Value<string>(),
                    transfer.Money,
                    ((JObject)info)["Balance"].Value<decimal>());
            }
            return new TransferResult(type);
        }

        #region ========  工具方法  ========

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

        internal override PostResult POST(string method, Dictionary<string, object> data)
        {
            PostResult result = new()
            {
                Url = $"{this.Gateway}/api/{method}",
                Data = data,
                Header = new()
                {
                    { "Authorization", this.SecretKey }
                }
            };
            result.Result = NetAgent.UploadData(result.Url, result.Data.ToQueryString(), headers: result.Header);
            JObject info = JObject.Parse(result.Result);
            if (info["success"].Value<int>() == 1)
            {
                if (info.ContainsKey("info")) result.Info = (JObject)info["info"];
                result.Code = APIResultType.Success;
            }
            else
            {
                if (info.ContainsKey("info")) result.Info = info = info["info"].Value<JObject>();
                if (info.ContainsKey("Error")) result.Code = this.GetErrorCode(info["Error"].Value<string>());
            }
            return result;
        }



        #endregion
    }
}
