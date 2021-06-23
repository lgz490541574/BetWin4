using BW.Games.Exceptions;
using BW.Games.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Mvc.Exceptions;
using SP.StudioCore.Net;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.API
{
    public class IMOne : IGameBase
    {
        #region ========= 配置参数 ===========


        [Description("网关")]
        public string Gateway { get; set; }

        [Description("主商家名称")]
        public string ParentMerchantName { get; set; }

        [Description("商家名称")]
        public string MerchantName { get; set; }

        [Description("商家密钥")]
        public string MerchantCode { get; set; }

        /// <summary>
        /// 用户前缀
        /// </summary>
        [Description("用户前缀")]
        public string Prefix { get; set; }

        [Description("产品钱包")]
        public int ProductWallet { get; set; }

        [Description("游戏代码")]
        public string GameCode { get; set; }

        #endregion

        #region ========  工具方法  ========

        internal override PostResult POST(string method, Dictionary<string, object> data)
        {
            PostResult result = new()
            {
                Url = $"{this.Gateway}/{method}",
                Data = data,
                Header = new Dictionary<string, string>()
                {
                    {"Content-Type", "application/json" }
                }
            };
            JObject info;
            result.Result = NetAgent.UploadData(result.Url, JsonConvert.SerializeObject(result.Data), Encoding.UTF8, headers: result.Header);
            result.Info = info = JObject.Parse(result.Result);
            result.Code = this.GetResultType(info["Code"].Value<int>());
            return result;
        }


        private APIResultType GetResultType(int state)
        {
            return state switch
            {
                0 => APIResultType.Success,
                501 => APIResultType.Authorization,
                503 => APIResultType.EXISTSUSER,
                504 => APIResultType.NOUSER,
                //Required field cannot be empty or null.
                505 => APIResultType.CONTENT,
                506 => APIResultType.BADNAME,
                //货币代码无效
                507 => APIResultType.CONTENT,
                //产品钱包无效
                508 => APIResultType.CONTENT,
                //交易码无效
                509 => APIResultType.TRANSFER_NO_ACTION,
                510 => APIResultType.NOBALANCE,
                //交易码在 IMOne 系统里是重复
                514 => APIResultType.EXISTSORDER,
                //交易码不存在于产品供应商端
                516 => APIResultType.NOORDER,
                //产品供应商正在处理该交易
                517 => APIResultType.PROCCESSING,
                //语言无效
                518 => APIResultType.CONTENT,
                //金额格式无效
                519 => APIResultType.BADMONEY,

                520 => APIResultType.PROCCESSING,
                //IP 地址无效
                522 => APIResultType.IP,
                //交易码在产品端里是重复的
                523 => APIResultType.EXISTSORDER,
                524 => APIResultType.BADPASSWORD,
                //时间格式无效
                528 => APIResultType.DATEEROOR,
                //玩家未在产品供应商端创建成功或在停用状态或在停用状态
                540 => APIResultType.USERLOCK,
                541 => APIResultType.ORDER_NOTFOUND,
                542 => APIResultType.USERLOCK,
                543 => APIResultType.BADMONEY,
                //玩家在进行游戏时，交易无法被处理
                544 => APIResultType.PROCCESSING,
                547 => APIResultType.DATEEROOR,
                556 => APIResultType.USERLOCK,
                557 => APIResultType.BUSY,
                //无数据
                558 => APIResultType.CONTENT,
                //玩家有其他交易未完成处理，此交易失败
                560 => APIResultType.PROCCESSING,
                //产品供应商内部错误
                600 => APIResultType.Faild,
                //非法产品访问
                601 => APIResultType.Faild,
                //已超过持续存款顶限
                603 => APIResultType.Faild,
                //金额超过存款顶限
                604 => APIResultType.BADMONEY,
                //金额低于最低存款额
                605 => APIResultType.BADMONEY,
                //无效的参数值
                612 => APIResultType.CONTENT,
                //系统目前无法处理您的请求。请重试
                998 => APIResultType.BUSY,
                //系统处理您的请求失败
                999 => APIResultType.Faild,
                _ => APIResultType.Faild
            };
        }

        #endregion

        public IMOne(string queryString) : base(queryString)
        {
        }

        public override LoginResult Login(LoginRequest login)
        {
            Dictionary<string, object> postData = new Dictionary<string, object>
            {
                { "MerchantCode",this.MerchantCode },
                { "PlayerId", login.UserName },
                { "GameCode", this.GameCode },
                { "Language", "ZH-CN" },
                { "IpAddress", IPAgent.IP },
                { "ProductWallet", this.ProductWallet },
                { "Http", 1 }
            };

            string action = WebAgent.IsMobile() ? "Game/NewLaunchMobileGame" : "Game/NewLaunchGame";
            APIResultType result = this.POST(action, postData, out object info);

            if (result == APIResultType.Success && ((JObject)info).ContainsKey("GameUrl"))
            {
                return new LoginResult(((JObject)info)["GameUrl"].Value<string>());
            }
            throw new APIResultException(result);
        }

        public override RegisterResult Register(RegisterRequest register)
        {
            string password = Guid.NewGuid().ToString("N").Substring(0, 8);
            Dictionary<string, object> postData = new Dictionary<string, object>
            {
                { "MerchantCode",this.MerchantCode },
                { "PlayerId", register.UserName },
                { "Currency", "CNY" },
                { "Password",password },
                { "Country", "CN" },
                { "Sex", "M" },
                { "BirthDate", "19881208" }
            };
            APIResultType result = this.POST("Player/Register", postData, out _);
            if (result == APIResultType.Success || result == APIResultType.EXISTSUSER)
            {
                return new RegisterResult(register.UserName, password);
            }
            throw new APIResultException(result);
        }

        public override TransferResult Recharge(TransferRequest transfer)
        {
            decimal money = transfer.Money;
            string sourceId = transfer.SourceID;
            Dictionary<string, object> data = new()
            {
                { "MerchantCode", this.MerchantCode },
                { "PlayerId", transfer.UserName },
                { "ProductWallet", this.ProductWallet },
                { "TransactionId", sourceId },
                { "Amount", money }
            };
            APIResultType resultType = this.POST("Transaction/PerformTransfer", data, out object info);
            if (resultType == APIResultType.Success)
            {
                return new TransferResult(transfer.OrderID, sourceId, money);
            }
            throw new APIResultException(resultType);
        }

        public override BalanceResult Balance(BalanceRequest balance)
        {
            throw new NotImplementedException();
        }
    }
}
