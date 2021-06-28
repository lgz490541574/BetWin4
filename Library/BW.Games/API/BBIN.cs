using BW.Games.Exceptions;
using BW.Games.Models;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Array;
using SP.StudioCore.Mvc.Exceptions;
using SP.StudioCore.Net;
using SP.StudioCore.Security;
using SP.StudioCore.Web;
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
                10008 => APIResultType.BADMONEY,
                47005 => APIResultType.BADNAME,
                44900 => APIResultType.IP,
                44003 => APIResultType.BUSY,
                40014 => APIResultType.DATEEROOR,
                10002 => APIResultType.NOBALANCE,
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

        protected override char UserSplit => '0';

        public override RegisterResult Register(RegisterRequest register)
        {
            string userName = this.GetUserName(register);
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
            string sourceId = transfer.SourceID;
            decimal money = transfer.Money;
            Dictionary<string, object> data = new()
            {
                { "username", transfer.UserName },
                { "remitno", sourceId },
                { "remit", money },
                { "key", this.GetKey(9, transfer.UserName + sourceId, t => t.KEYTransfer, 4) },
                { "action", "IN" }
            };
            APIResultType resultType = this.POST("Transfer", data, out _);
            if (resultType == APIResultType.Success)
            {
                return new TransferResult(transfer.OrderID, sourceId, money);
            }
            return new(resultType);
        }

        public override BalanceResult Balance(BalanceRequest balance)
        {
            APIResultType resultType = this.POST("CheckUsrBalance", new Dictionary<string, object>()
            {
                {"username",balance.UserName },
                {"key",this.GetKey(4,balance.UserName,t=>t.KEYCheckUsrBalance,7) }

            }, out object info);
            if (resultType != APIResultType.Success) throw new APIResultException(resultType);
            return new BalanceResult(balance.UserName, ((JObject)info)["data"].Value<JArray>().FirstOrDefault()["Balance"].Value<decimal>());
        }

        public override TransferResult Withdraw(TransferRequest transfer)
        {
            string sourceId = transfer.SourceID;
            decimal money = transfer.Money;
            Dictionary<string, object> data = new()
            {
                { "username", transfer.UserName },
                { "remitno", sourceId },
                { "remit", money },
                { "key", this.GetKey(9, transfer.UserName + sourceId, t => t.KEYTransfer, 4) },
                { "action", "OUT" }
            };
            APIResultType resultType = this.POST("Transfer", data, out _);
            if (resultType == APIResultType.Success)
            {
                return new TransferResult(transfer.OrderID, sourceId, money);
            }
            return new(resultType);
        }

        /// <summary>
        /// 日志拉取（美东时间）
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public override IEnumerable<OrderResult> GetOrders(OrderRequest order)
        {
            bool isNewDate = true;
            DateTime now = DateTime.Now.AddHours(-12);
            if (order.Time == 0)
            {
                order.Time = WebAgent.GetTimestamps(DateTime.Now.AddDays(-7));
            }
            // 上次的结束时间
            DateTime startAt = WebAgent.GetTimestamps(order.Time);
            if (startAt > now.AddMinutes(-5)) startAt = now.AddMinutes(-5);
            // 本次的结束时间
            DateTime endAt = startAt.AddMinutes(60);
            if (endAt > now) endAt = now;

            // 如果加上30分钟后不是同一天，则调整为23:59:59
            if (startAt.Date != endAt.Date)
            {
                endAt = startAt.Date.AddSeconds(23 * 3600 + 59 * 60 + 59);
                if (endAt < now)
                {
                    isNewDate = true;
                }
            }

            Dictionary<string, object> data = new()
            {
                { "rounddate", startAt.ToString("yyyy-MM-dd") },
                { "starttime", startAt.ToString("HH:mm:ss") },
                { "endtime", endAt.ToString("HH:mm:ss") },
                { "gamekind", 3 },
                { "key", this.GetKey(1, string.Empty, t => t.KEYBetRecord, 8) },
                { "pagelimit", 500 }
            };

            APIResultType resultType = this.POST("BetRecord", data, out object info);
            if (resultType != APIResultType.Success) throw new APIResultException(resultType);

            //进行中
            //{"UserName":"ceshi01","WagersID":"28372850177","WagersDate":"2021-06-28 12:24:01",
            //"SerialID":"227425823","RoundNo":"18-48","GameType":"3001","WagerDetail":"2,1:1,50.00,0.00",
            //"GameCode":"82","Result":"","Card":"","BetAmount":"50.00","Origin":"P",
            //"Commissionable":"0.00","Payoff":"0.0000","Currency":"RMB","ExchangeRate":"1.000000","ResultType":"0"}]

            // 已结算
            //{"UserName":"ceshi01","WagersID":"28372829681","WagersDate":"2021-06-28 12:19:34",
            //"SerialID":"227425156","RoundNo":"18-41","GameType":"3001",
            //"WagerDetail":"2,1:1,60.00,0.00","GameCode":"82","Result":"0,0",
            //"Card":"D.4,D.12,D.6*H.8,C.6,S.6","BetAmount":"60.00","Origin":"P",
            //"Commissionable":"0.00","Payoff":"0.0000","Currency":"RMB","ExchangeRate":"1.000000","ResultType":" "}

            foreach (JObject item in ((JObject)info)["data"])
            {
                OrderStatus status = OrderStatus.Wait;
                switch (item["ResultType"].Value<string>())
                {
                    case "0":
                        status = OrderStatus.Wait;
                        break;
                    case "-1":
                        status = OrderStatus.Revoke;
                        break;
                    default:
                        decimal payoff = item["Payoff"].Value<decimal>();
                        if (payoff > 0M)
                        {
                            status = OrderStatus.Win;
                        }
                        else if (payoff < 0M)
                        {
                            status = OrderStatus.Lose;
                        }
                        else
                        {
                            status = OrderStatus.Revoke;
                        }
                        break;
                }

                yield return new OrderResult
                {
                    OrderID = item["WagersID"].Value<string>(),
                    UserName = item["UserName"].Value<string>(),
                    CreateAt = WebAgent.GetTimestamps(item["WagersDate"].Value<DateTime>().AddHours(12)),
                    FinishAt = WebAgent.GetTimestamps(item["WagersDate"].Value<DateTime>().AddHours(12)),
                    BetMoney = item["BetAmount"].Value<decimal>(),
                    Money = item["Payoff"].Value<decimal>(),
                    Game = item["GameType"].Value<string>(),
                    RawData = item.ToString(),
                    Status = status
                };
            }
            order.Time = WebAgent.GetTimestamps(isNewDate ? endAt.AddSeconds(1) : endAt);
        }
    }
}
