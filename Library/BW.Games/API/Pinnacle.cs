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
            JToken info = JToken.Parse(result.Result);
            if (info.Type == JTokenType.Array)
            {
                result.Info = (JArray)info;
                result.Code = APIResultType.Success;
            }
            else if (info.Type == JTokenType.Object)
            {
                result.Info = (JObject)info;
                result.Code = ((JObject)info).ContainsKey("code") ? this.GetResultType(((JObject)info)["code"].Value<int>()) : APIResultType.Success;
            }
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
            APIResultType resultType = this.POST("/player/info", new Dictionary<string, object>()
            {
                {"userCode",balance.UserName }
            }, out object info);
            if (resultType != APIResultType.Success) throw new APIResultException(resultType);

            return new BalanceResult(balance.UserName, ((JObject)info)["availableBalance"].Value<decimal>());

        }

        public override TransferResult Withdraw(TransferRequest transfer)
        {
            string sourceId = transfer.SourceID;
            Dictionary<string, object> data = new()
            {
                { "userCode", transfer.UserName },
                { "amount", transfer.Money },
                { "transactionId", sourceId }
            };

            APIResultType resultType = this.POST("/player/withdraw", data, out object info);
            if (resultType != APIResultType.Success) throw new APIResultException(resultType);

            transfer.Money = ((JObject)info).ContainsKey("amount") ? ((JObject)info)["amount"].Value<decimal>() : decimal.Zero;

            if (transfer.Money == decimal.Zero)
            {
                throw new APIResultException(APIResultType.Faild);
            }

            return new TransferResult(transfer.OrderID, sourceId, transfer.Money, ((JObject)info)["availableBalance"].Value<decimal>());
        }

        /// <summary>
        /// 美东事件(时差-12小时)
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public override IEnumerable<OrderResult> GetOrders(OrderRequest order)
        {
            DateTime now = DateTime.Now.AddHours(-12);

            DateTime startAt = order.Time == 0 ? DateTime.Now.AddDays(-7) : WebAgent.GetTimestamps(order.Time);
            startAt = startAt.AddMinutes(-5);
            DateTime endAt = startAt.AddHours(1);
            if (endAt > now) endAt = now;

            //{ "wagerId":1747880128,"eventName":"England-vs-Germany","parentEventName":null,"headToHead":null,
            //"wagerDateFm":"2021-06-29 12:05:20","eventDateFm":"2021-06-29 12:00:00","settleDateFm":null,
            //"status":"OPEN","homeTeam":"England","awayTeam":"Germany","selection":"Germany",
            //"handicap":0.00,"odds":3.080,"oddsFormat":1,"betType":1,"league":"UEFA - EURO",
            //"leagueId":5264,"stake":15.00,"sportId":29,"sport":"Soccer","currencyCode":"CNY","inplayScore":"0-0",
            //"inPlay":true,"homePitcher":null,"awayPitcher":null,"homePitcherName":null,"awayPitcherName":null,
            //"period":0,"cancellationStatus":null,"parlaySelections":[],"category":null,"toWin":31.2000000,
            //"toRisk":15.0000000,"product":"SB","parlayMixOdds":3.0800000,"wagerType":"single","competitors":[],
            //"userCode":"3410101PL7","loginId":"ceshi01","winLoss":0.00,"turnover":0.00,"scores":[],"result":null}


            APIResultType resultType = this.POST("/report/all-wagers", new()
            {
                { "dateFrom", startAt.ToString("yyyy-MM-dd HH:mm:ss") },
                { "dateTo", endAt.ToString("yyyy-MM-dd HH:mm:ss") },
                { "filterBy", "update_date" }
            }, out object info);

            if (resultType != APIResultType.Success) throw new APIResultException(resultType);

            foreach (JObject item in (JArray)info)
            {
                OrderStatus status = OrderStatus.Wait;
                switch (item["status"].Value<string>())
                {
                    case "PENDING":
                    case "OPEN":
                        status = OrderStatus.Wait;
                        break;
                    case "CANCELLED":
                    case "DELETED":
                        status = OrderStatus.Revoke;
                        break;
                    case "SETTLED":
                        decimal winLose = item["winLoss"].Value<decimal>();
                        if (winLose > 0M)
                        {
                            status = OrderStatus.Win;
                        }
                        else if (winLose < 0M)
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
                    OrderID = item["wagerId"].Value<string>(),
                    BetMoney = item["stake"].Value<decimal>(),
                    Money = item["winLoss"].Value<decimal>(),
                    CreateAt = WebAgent.GetTimestamps(item["wagerDateFm"].Value<DateTime>().AddHours(12)),
                    FinishAt = item["settleDateFm"].Type == JTokenType.Null ? 0 : WebAgent.GetTimestamps(item["settleDateFm"].Value<DateTime>().AddHours(12)),
                    Game = item["sportId"].Value<string>(),
                    UserName = item["loginId"].Value<string>(),
                    Status = status,
                    RawData = item.ToString()
                };
            }

            order.Time = WebAgent.GetTimestamps(endAt);
            yield break;
        }
    }
}
