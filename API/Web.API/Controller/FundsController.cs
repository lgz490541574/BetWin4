using BW.Common.Agent.Games;
using BW.Games.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.API.Filters;

namespace Web.API.Controller
{
    /// <summary>
    /// 资金有关
    /// </summary>
    public class FundsController : APIControllerBase
    {
        /// <summary>
        /// 充值（转入资金)
        /// </summary>
        /// <returns></returns>
        public ContentResult Recharge([FromBody] TransferRequest transfer)
        {
            TransferResult result = TransferAgent.Instance().Recharge(this.SiteInfo, transfer.UserName, transfer.GameID, transfer.OrderID, transfer.Money);
            result.OrderID = transfer.OrderID;
            return this.GetResultContent(result);
        }
    }
}
