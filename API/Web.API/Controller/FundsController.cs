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
        /// 查询余额
        /// </summary>
        /// <param name="balance"></param>
        /// <returns></returns>
        public ContentResult Balance([FromBody] BalanceRequest balance)
        {
            BalanceResult result = FundAgent.Instance().GetBalance(this.SiteInfo, balance.UserName, balance.GameID);
            return this.GetResultContent(result);
        }

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

        /// <summary>
        /// 提现（转出资金）
        /// </summary>
        /// <returns></returns>
        public ContentResult Withdraw([FromBody] TransferRequest transfer)
        {
            TransferResult result = TransferAgent.Instance().Withdraw(this.SiteInfo, transfer.UserName, transfer.GameID, transfer.OrderID, transfer.Money);
            result.OrderID = transfer.OrderID;
            return this.GetResultContent(result);
        }

        public ContentResult Check([FromBody] CheckTransferRequest request)
        {
            CheckTransferResult result = TransferAgent.Instance().Check(this.SiteInfo, request.OrderID);
            return this.GetResultContent(result);
        }
    }
}
