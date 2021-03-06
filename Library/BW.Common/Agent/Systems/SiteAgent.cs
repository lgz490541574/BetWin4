using BW.Common.Caching;
using BW.Common.Entities.Sites;
using BW.Common.Models.Enums;
using SP.StudioCore.Data;
using SP.StudioCore.Mvc.Exceptions;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BW.Common.Agent.Systems
{
    /// <summary>
    /// 总后台对于商户的管理
    /// </summary>
    public class SiteAgent : AgentBase<SiteAgent>
    {
        /// <summary>
        /// 创建一个新商户
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefix">用户名前缀</param>
        /// <param name="whiteIP"></param>
        /// <returns></returns>
        public bool CreateSite(string name, string prefix, string whiteIP)
        {
            if (string.IsNullOrEmpty(name)) throw new ResultException("请输入名字");
            if (!Regex.IsMatch(prefix, @"^[0-9a-z]{3}$")) throw new ResultException("前缀必须是字母或者数字，固定三位");

            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                if (db.Exists<Site>(t => t.Name == name))
                {
                    throw new ResultException("存在同名商户");
                }

                if (db.Exists<Site>(t => t.Prefix == prefix))
                {
                    throw new ResultException("前缀重复");
                }

                Site site = new Site
                {
                    Name = name,
                    Prefix = prefix,
                    CreateAt = WebAgent.GetTimestamps(),
                    SecretKey = Guid.NewGuid(),
                    Status = SiteStatus.Normal,
                    WhiteIP = whiteIP
                };

                db.InsertIdentity(site);

                db.AddCallback(() =>
                {
                    SiteCaching.Instance().SaveSiteInfo(site);
                    SiteCaching.Instance().SaveWhiteIP(site.ID, whiteIP.Split(','));
                });

                db.Commit();
            }

            return true;
        }

        /// <summary>
        /// 读取商户信息
        /// </summary>
        public Site GetSiteInfo(int siteId)
        {
            return this.ReadDB.ReadInfo<Site>(t => t.ID == siteId);
        }

        /// <summary>
        /// 更新白名单
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="whiteIP"></param>
        public string[] UpdateWhiteIP(int siteId, string whiteIP)
        {
            this.WriteDB.Update<Site, string>(t => t.WhiteIP, whiteIP, t => t.ID == siteId);
            return SiteCaching.Instance().SaveWhiteIP(siteId, whiteIP.Split(','));
        }

        /// <summary>
        /// 更新密钥（随机)
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public Guid UpdateSecretKey(int siteId)
        {
            Guid secretKey = Guid.NewGuid();
            this.WriteDB.Update<Site, Guid>(t => t.SecretKey, secretKey, t => t.ID == siteId);
            SiteCaching.Instance().SaveSecretKey(siteId, secretKey);
            return secretKey;
        }
    }
}
