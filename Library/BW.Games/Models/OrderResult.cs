using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 订单日志
    /// </summary>
    public struct OrderResult
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID;

        /// <summary>
        /// 玩家用户名
        /// </summary>
        public string UserName;

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public long CreateAt;

        /// <summary>
        /// 订单完成时间
        /// </summary>
        public long FinishAt;

        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal BetMoney;

        /// <summary>
        /// 盈亏金额
        /// </summary>
        public decimal Money;

        /// <summary>
        /// 所玩的游戏
        /// </summary>
        public string Game;

        /// <summary>
        /// 供应商的原始数据
        /// </summary>
        public string RawData;

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus Status;
    }
}
