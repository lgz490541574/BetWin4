using BW.Games.Exceptions;
using BW.Games.Models;
using SP.StudioCore.Json;
using SP.StudioCore.Web;
using SP.StudioCore.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BW.Games.API
{
    /// <summary>
    /// AG视讯
    /// </summary>
    public sealed class AGLive : AG
    {
        public AGLive(string queryString) : base(queryString)
        {
        }

        /// <summary>
        /// 美东时间(-12)
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public override IEnumerable<OrderResult> GetOrders(OrderRequest order)
        {
            int total = 1;
            int page = 1;
            DateTime now = DateTime.UtcNow.Add(this.OffsetTime);
            DateTime startTime = order.Time == 0 ? DateTime.UtcNow.Add(this.OffsetTime).AddDays(-3) : WebAgent.GetTimestamps(order.Time, this.OffsetTime).AddMinutes(-2);
            DateTime endTime = startTime.AddMinutes(10);
            if (endTime > now) endTime = now;
            while (page <= total)
            {
                APIResultType resultType = this.POST("getorders.xml", new Dictionary<string, object>()
                {
                    { "startdate",startTime.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "enddate",endTime.ToString("yyyy-MM-dd HH:mm:ss") }
                }, out object info);
                if (resultType != APIResultType.Success) throw new APIResultException(resultType);

                XElement root = (XElement)info;
                foreach (XElement row in root.Elements("row"))
                {
                    int flag = row.GetAttributeValue("flag", 0);
                    decimal netAmount = row.GetAttributeValue("netAmount", 0M);
                    OrderStatus status = OrderStatus.Wait;
                    if (flag == 1)
                    {
                        if (netAmount > 0)
                        {
                            status = OrderStatus.Win;
                        }
                        else if (netAmount < 0)
                        {
                            status = OrderStatus.Lose;
                        }
                        else
                        {
                            status = OrderStatus.Revoke;
                        }
                    }
                    yield return new OrderResult
                    {
                        OrderID = row.GetAttributeValue("billNo"),
                        UserName = row.GetAttributeValue("playName"),
                        Game = row.GetAttributeValue("gameType"),
                        CreateAt = WebAgent.GetTimestamps(row.GetAttributeValue("betTime", DateTime.Now).AddHours(12)),
                        BetMoney = row.GetAttributeValue("betAmount", 0M),
                        Money = netAmount,
                        FinishAt = flag == 0 ? 0 : WebAgent.GetTimestamps(row.GetAttributeValue("recalcuTime", DateTime.Now).AddHours(12)),
                        Status = status,
                        RawData = row.ToJson()
                    };
                }
                page++;
            }
            order.Time = WebAgent.GetTimestamps(endTime, OffsetTime);
        }
    }
}
