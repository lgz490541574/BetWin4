using BW.Common.Models.Sites;
using BW.Games.Models;
using SP.StudioCore.Cache.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Caching
{
    /// <summary>
    /// 商户缓存管理
    /// </summary>
    public class SiteCaching : CacheBase<SiteCaching>
    {
        protected override int DB_INDEX => RedisIndex.SITE;

        #region ========  商户资料  ========


        private const string SITE_INFO = "SITE:INFO:";

        /// <summary>
        /// 保存商户资料
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public SiteModel SaveSiteInfo(SiteModel site)
        {
            this.NewExecutor().HashSet($"{SITE_INFO}{site.ID}", site);
            return site;
        }

        /// <summary>
        /// 读取商户资料
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public SiteModel GetSiteInfo(int siteId)
        {
            return this.NewExecutor().HashGetAll($"{SITE_INFO}{siteId}");
        }

        /// <summary>
        /// 更新商户密钥
        /// </summary>
        internal void SaveSecretKey(int siteId, Guid secretKey)
        {
            string key = $"{SITE_INFO}{siteId}";
            this.NewExecutor().HashSet(key, "SecretKey", secretKey.GetRedisValue());
        }

        #endregion

        #region ========  白名单  ========


        /// <summary>
        /// 白名单IP
        /// </summary>
        private const string SITE_WHITE = "SITE:WHITE:";

        public string[] SaveWhiteIP(int siteId, IEnumerable<string> iplist)
        {
            IBatch batch = this.NewExecutor().CreateBatch();
            string key = $"{SITE_WHITE}{siteId}";
            batch.KeyDeleteAsync(key);
            foreach (string ip in iplist)
            {
                batch.SetAddAsync(key, ip.GetRedisValue());
            }
            batch.Execute();
            return iplist.ToArray();
        }

        public bool IsWhiteIP(int siteId, string ip)
        {
            return this.NewExecutor().SetContains($"{SITE_WHITE}{siteId}", ip);
        }

        #endregion


        #region ========  商户游戏  ========

        private const string SITE_GAME = "SITE:GAME:";

        public SiteGameModel SaveSiteGame(SiteGameModel model)
        {
            this.NewExecutor().HashSet($"{SITE_GAME}{model.SiteID}", model.Type.GetRedisValue(), model);
            return model;
        }

        public SiteGameModel GetSiteGame(int siteId, GameType type)
        {
            return this.NewExecutor().HashGet($"{SITE_GAME}{siteId}", type.GetRedisValue());
        }

        #endregion

    }
}
