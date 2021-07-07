using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Common.Models.Enums;
using BW.Common.Models.Sites;
using BW.Games.Models;

namespace BW.Common.Entities.Sites
{
    /// <summary>
    /// 商户开通的游戏
    /// </summary>
    [Table("site_Game")]
    public partial class SiteGame
    {

        #region  ========  構造函數  ========
        public SiteGame() { }

        public SiteGame(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "SiteID":
                        this.SiteID = (int)reader[i];
                        break;
                    case "Type":
                        this.Type = (GameType)reader[i];
                        break;
                    case "Status":
                        this.Status = (SiteGameStatus)reader[i];
                        break;
                    case "Credit":
                        this.Credit = (decimal)reader[i];
                        break;
                    case "Rate":
                        this.Rate = (decimal)reader[i];
                        break;
                    case "LockCredit":
                        this.LockCredit = (decimal)reader[i];
                        break;
                }
            }
        }


        public SiteGame(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "SiteID":
                        this.SiteID = (int)dr[i];
                        break;
                    case "Type":
                        this.Type = (GameType)dr[i];
                        break;
                    case "Status":
                        this.Status = (SiteGameStatus)dr[i];
                        break;
                    case "Credit":
                        this.Credit = (decimal)dr[i];
                        break;
                    case "Rate":
                        this.Rate = (decimal)dr[i];
                        break;
                    case "LockCredit":
                        this.LockCredit = (decimal)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        [Column("SiteID"), Key]
        public int SiteID { get; set; }


        /// <summary>
        /// 游戏类型
        /// </summary>
        [Column("Type"), Key]
        public GameType Type { get; set; }


        /// <summary>
        /// 商户游戏的状态
        /// </summary>
        [Column("Status")]
        public SiteGameStatus Status { get; set; }


        /// <summary>
        /// 当前额度
        /// </summary>
        [Column("Credit")]
        public decimal Credit { get; set; }


        /// <summary>
        /// 点位
        /// </summary>
        [Column("Rate")]
        public decimal Rate { get; set; }


        /// <summary>
        /// 被锁定的额度
        /// </summary>
        [Column("LockCredit")]
        public decimal LockCredit { get; set; }

        #endregion


        #region  ========  扩展方法  ========

        public static implicit operator SiteGameModel(SiteGame siteGame)
        {
            if (siteGame == null) return default;
            return new SiteGameModel
            {
                SiteID = siteGame.SiteID,
                Type = siteGame.Type,
                Rate = siteGame.Rate,
                Status = siteGame.Status
            };
        }

        #endregion

    }

}
