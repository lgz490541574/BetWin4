using BW.Common.Caching;
using BW.Common.Entities.Sites;
using BW.Common.Models.Sites;
using SP.StudioCore.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Agent.Sites
{
    public class SiteGameAgent : AgentBase<SiteGameAgent>
    {
        /// <summary>
        /// 保存商户开通的游戏（新增或者修改)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SiteGameModel SaveSiteGame(SiteGameModel model)
        {
            SiteGame siteGame = new SiteGame
            {
                GameID = model.GameID,
                SiteID = model.SiteID,
                Status = model.Status,
                Rate = model.Rate
            };
            using (DbExecutor db = NewExecutor(IsolationLevel.ReadUncommitted))
            {
                if (db.Exists(siteGame))
                {
                    db.Update(siteGame, t => t.Status, t => t.Rate);
                }
                else
                {
                    db.Insert(siteGame);
                }

                db.Commit();
            }
            return SiteCaching.Instance().SaveSiteGame(model);
        }

        /// <summary>
        /// 获取商户开通的游戏列表
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public List<SiteGame> GetSiteGame(int siteId)
        {
            return this.ReadDB.ReadList<SiteGame>(t => t.SiteID == siteId).OrderBy(t => t.GameID).ToList();
        }

        public SiteGameModel GetSiteGameModel(int siteId, int gameId)
        {
            SiteGameModel model = SiteCaching.Instance().GetSiteGame(siteId, gameId);
            if (!model)
            {
                model = this.ReadDB.ReadInfo<SiteGame>(t => t.SiteID == siteId && t.GameID == gameId);
                if (model) SiteCaching.Instance().SaveSiteGame(model);
            }
            return model;
        }
    }
}
