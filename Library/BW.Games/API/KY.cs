using BW.Games.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.API
{
    /// <summary>
    /// 开元棋牌
    /// </summary>
    public class KY : IGameBase
    {
        #region ========  字段  ========

        [Description("网关")]
        public string Gateway { get; set; }

        [Description("商户号")]
        public string Merchant { get; set; }

        [Description("Deskey")]
        public string Deskey { get; set; }

        [Description("Md5key")]
        public string Md5key { get; set; }

        #endregion

        #region ========  工具方法  ========

        internal override PostResult POST(string method, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        #endregion

        public KY(string queryString) : base(queryString)
        {
        }

        public override BalanceResult Balance(BalanceRequest balance)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<OrderResult> GetOrders(OrderRequest order)
        {
            throw new NotImplementedException();
        }

        public override LoginResult Login(LoginRequest login)
        {
            throw new NotImplementedException();
        }

        public override TransferResult Recharge(TransferRequest transfer)
        {
            throw new NotImplementedException();
        }

        public override RegisterResult Register(RegisterRequest register)
        {
            throw new NotImplementedException();
        }

        public override TransferResult Withdraw(TransferRequest transfer)
        {
            throw new NotImplementedException();
        }

      
    }
}
