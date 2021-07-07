using BW.Common.Caching;
using BW.Games.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.API.Filters;

namespace Web.API.Controller
{
    public class OrderController : APIControllerBase
    {
        /// <summary>
        /// 订单日志
        /// </summary>
        /// <returns></returns>
        public ContentResult Log([FromBody] OrderRequest request)
        {
            //# 查询日志
            var list = this.BDC.GameOrder.Where(t => t.SiteID == this.SiteInfo);
            if (Enum.IsDefined(typeof(GameType), request.Game)) list = list.Where(t => t.Type == request.Game);
            list = list.Where(t => t.UpdateAt > request.Time);

            var orderlist = list.OrderBy(t => t.UpdateAt).Take(100).ToList();

            return this.GetResultContent(new
            {
                Code = APIResultType.Success,
                orderlist.Count,
                Data = orderlist.Select(t => new
                {
                    t.OrderID,
                    t.Game,
                    t.UserName,
                    t.CreateAt,
                    t.FinishAt,
                    t.Status,
                    t.UpdateAt,
                    t.BetMoney,
                    t.Money
                })
            });
        }

        /// <summary>
        /// 查询订单明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ContentResult Detail([FromForm] OrderDetailRequest[] request)
        {
            List<OrderDetailResult> list = GameCaching.Instance().GetOrderDetail(request);
            return this.GetResultContent(new
            {
                Code = APIResultType.Success,
                list.Count,
                Data = list
            });

        }
    }
}
