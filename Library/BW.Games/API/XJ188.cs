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

        /// <summary>
        /// 美东时间
        /// </summary>
        protected override TimeSpan OffsetTime => TimeSpan.FromHours(-4);

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
                // Invalid parameters or format. Parameters could be out of range, or unacceptable values.
                "COMM0101" => APIResultType.BADMONEY,
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

        public override BalanceResult Balance(BalanceRequest balance)
        {
            APIResultType resultType = this.POST("/API/MemberBalance", new()
            {
                { "memberCode", balance.UserName }
            }, out object info);

            if (resultType != APIResultType.Success) throw new APIResultException(resultType);

            return new BalanceResult(balance.UserName, ((JObject)info)["data"]["balance"].Value<decimal>());
        }

        public override TransferResult Recharge(TransferRequest transfer)
        {
            Dictionary<string, object> data = new()
            {
                { "memberCode", transfer.UserName },
                { "balance", transfer.Money },
                { "currencyCode", Currency },
                { "refId", transfer.SourceID }
            };
            APIResultType resultType = this.POST("/API/DepositFund", data, out object info);
            if (resultType == APIResultType.Success)
            {
                return new TransferResult(transfer.OrderID, transfer.SourceID, transfer.Money, ((JObject)info)["data"]["totalBalance"].Value<decimal>());
            }
            return new(resultType);
        }

        public override TransferResult Withdraw(TransferRequest transfer)
        {

            Dictionary<string, object> data = new()
            {
                { "memberCode", transfer.UserName },
                { "balance", transfer.Money },
                { "currencyCode", Currency },
                { "refId", transfer.SourceID }
            };
            APIResultType resultType = this.POST("/API/WithdrawalFund", data, out object info);
            if (resultType == APIResultType.Success)
            {
                return new TransferResult(transfer.OrderID, transfer.SourceID, transfer.Money, ((JObject)info)["data"]["totalBalance"].Value<decimal>());
            }
            return new(resultType);
        }

        public override IEnumerable<OrderResult> GetOrders(OrderRequest order)
        {
            DateTime now = DateTime.UtcNow.Add(this.OffsetTime);

            DateTime start = order.Time == 0 ? DateTime.UtcNow.Add(this.OffsetTime).AddDays(-7) : WebAgent.GetTimestamps(order.Time, this.OffsetTime).AddMinutes(-5);
            DateTime end = start.AddHours(1);
            if (end > now) end = now;

            foreach (int isSettled in new[] { 3, 4 })
            {
                APIResultType resultType = this.POST("/API/Wagers", new()
                {
                    { "operatorId", this.OperatorID },
                    { "vendorId", this.VendoID },
                    { "from", start.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "to", end.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "isSettled", 3 }
                }, out object info);
                if (resultType != APIResultType.Success) throw new APIResultException(resultType);

                foreach (JObject data in ((JObject)info)["data"].Value<JArray>())
                {
                    string memberCode = data["memberCode"].Value<string>();
                    if (memberCode.StartsWith(this.Prefix)) memberCode = memberCode.Substring(this.Prefix.Length);
                    int wagerStatus = data["wagerStatus"].Value<int>();
                    decimal returnAmount = data["returnAmount"].Value<decimal>();
                    decimal betMoney = data["stake"].Value<decimal>();
                    decimal money = 0;

                    OrderStatus status = OrderStatus.Wait;
                    switch (wagerStatus)
                    {
                        case 1:
                            status = OrderStatus.Wait;
                            break;
                        case 2:
                            money = returnAmount - betMoney;
                            if (money > 0)
                            {
                                status = OrderStatus.Win;
                            }
                            else if (money < 0)
                            {
                                status = OrderStatus.Lose;
                            }
                            else
                            {
                                status = OrderStatus.Revoke;
                            }
                            break;
                        case 3:
                        case 4:
                            status = OrderStatus.Revoke;
                            break;
                    }

                    JArray bets = JArray.Parse(data["bets"].Value<string>());
                    yield return new OrderResult
                    {
                        OrderID = data["id"].Value<string>(),
                        UserName = memberCode,
                        CreateAt = WebAgent.GetTimestamps(data["createTime"].Value<DateTime>(), this.OffsetTime),
                        FinishAt = WebAgent.GetTimestamps(data["settleTime"].Value<DateTime>(), this.OffsetTime),
                        BetMoney = betMoney,
                        Game = bets.Count == 1 ? bets[0]["event"]["gameType"].Value<string>() : "Combo",
                        Money = money,
                        Status = status,
                        RawData = data.ToString()
                    };
                }
            }
            order.Time = WebAgent.GetTimestamps(end, this.OffsetTime);
        }
    }
}
