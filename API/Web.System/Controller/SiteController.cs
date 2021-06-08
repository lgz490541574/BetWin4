using BW.Common.Agent.Systems;
using Microsoft.AspNetCore.Mvc;
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
        /// <returns></returns>
        public ContentResult CreateSite([FromForm] string name, [FromForm] string prefix, [FromForm] string whiteIP)
            => this.GetResultContent(SiteAgent.Instance().CreateSite(name, prefix, whiteIP));

        /// <summary>
        /// 商户列表
        /// </summary>
        /// <returns></returns>
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
    }
}
