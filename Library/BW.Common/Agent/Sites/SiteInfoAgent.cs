using BW.Common.Caching;
using BW.Common.Agent.Sites;
using BW.Common.Models.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Common.Entities.Sites;

namespace BW.Common.Agent.Sites
{
    /// <summary>
    /// 商户资料信息管理
    /// </summary>
    public sealed class SiteInfoAgent : AgentBase<SiteInfoAgent>
    {
        public Site GetSiteInfo(int siteId)
        {
            return this.ReadDB.ReadInfo<Site>(t => t.ID == siteId);
        }

        /// <summary>
        /// 读取商户资料（缓存读取）
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public SiteModel GetSiteModel(int siteId)
        {
            SiteModel site = SiteCaching.Instance().GetSiteInfo(siteId);
            if (!site)
            {
                site = this.GetSiteInfo(siteId);
                if (site) SiteCaching.Instance().SaveSiteInfo(site);
            }
            return site;
        }
    }
}
