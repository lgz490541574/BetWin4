using Microsoft.AspNetCore.Http;
using SP.StudioCore.Http;
using SP.StudioCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Models.Sites
{
    /// <summary>
    /// 商户的授权信息
    /// </summary>
    public struct SiteToken
    {
        public SiteToken(HttpContext context) : this()
        {
            if (!context.GetAuth(out string username, out string password))
            {
                return;
            }
            this.SiteID = username.GetValue<int>();
            this.SecretKey = password.GetValue<Guid>();
        }

        /// <summary>
        /// 商户ID
        /// </summary>
        public int SiteID;

        /// <summary>
        /// 密钥
        /// </summary>
        public Guid SecretKey;

        public override string ToString()
        {
            return $"{this.SiteID}:{this.SecretKey:N}".ToBasicAuth();
        }

        public static implicit operator bool(SiteToken token)
        {
            return token.SiteID != 0 && token.SecretKey != Guid.Empty;
        }
    }
}
