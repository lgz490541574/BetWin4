using BW.Common.Agent.Systems;
using BW.Common.Entities.Sites;
using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Mvc.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.System.Filters;

namespace Web.System.Controller
{
    /// <summary>
    /// 商户管理
    /// </summary>
    public class SiteController : SysControllerBase
    {
        /// <summary>
        /// 创建商户
        /// </summary>
        public ContentResult CreateSite([FromForm] string name, [FromForm] string prefix, [FromForm] string whiteIP)
            => this.GetResultContent(SiteAgent.Instance().CreateSite(name, prefix, whiteIP));

        /// <summary>
        /// 商户列表
        /// </summary>
        public ContentResult GetSiteList()
        {
            var list = this.BDC.Site;

            return this.GetResultList(list.OrderByDescending(t => t.ID), t => new
            {
                t.ID,
                t.Name,
                t.CreateAt,
                t.Status,
                t.SecretKey
            });
        }

        /// <summary>
        /// 获取商户的API信息
        /// </summary>
        public ContentResult GetAPIInfo([FromForm] int siteId)
        {
            Site site = SiteAgent.Instance().GetSiteInfo(siteId);
            if (site == null) throw new ResultException("商户不存在");
            return this.GetResultContent(new
            {
                SiteID = site.ID,
                SecretKey = site.SecretKey.ToString("N"),
                WhiteIP = site.WhiteIP.Split(',')
            });
        }

        /// <summary>
        /// 保存API信息
        /// </summary>
        public ContentResult SaveAPIInfo([FromForm] int siteId, [FromForm] string whiteIP, [FromForm] bool newSecretKey)
        {
            Guid? secretKey = null;
            if (newSecretKey)
            {
                secretKey = SiteAgent.Instance().UpdateSecretKey(siteId);
            }
            return this.GetResultContent(new
            {
                SecretKey = secretKey?.ToString("N"),
                WhiteIP = SiteAgent.Instance().UpdateWhiteIP(siteId, whiteIP)
            });
        }
    }
}
