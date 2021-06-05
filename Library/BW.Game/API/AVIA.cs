using BW.Game.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Game.API
{
    public class AVIA : IGameBase
    {
        #region ========  字段  ========

        [Description("网关")]
        public string Gateway { get; set; }

        [Description("商户号")]
        public int SiteID { get; set; }

        [Description("密钥")]
        public string SecretKey { get; set; }

        #endregion

        public AVIA(string queryString) : base(queryString)
        {
        }

        public override LoginResult Login(LoginRequest login)
        {
            throw new NotImplementedException();
        }

        public override RegisterResult Register(RegisterRequest register)
        {
            throw new NotImplementedException();
        }
    }
}
