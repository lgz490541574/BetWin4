using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Common.Models.Enums;

namespace BW.Common.Entities.Sites
{
    /// <summary>
    /// 商户游戏额度账变日志
    /// </summary>
    [Table("site_CreditLog")]
    public partial class CreditLog
    {

        #region  ========  構造函數  ========
        public CreditLog() { }

        public CreditLog(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "LogID":
                        this.LogID = (int)reader[i];
                        break;
                    case "SiteID":
                        this.SiteID = (int)reader[i];
                        break;
                    case "GameID":
                        this.GameID = (int)reader[i];
                        break;
                    case "Credit":
                        this.Credit = (decimal)reader[i];
                        break;
                    case "Balance":
                        this.Balance = (decimal)reader[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)reader[i];
                        break;
                    case "SourceID":
                        this.SourceID = (string)reader[i];
                        break;
                    case "Type":
                        this.Type = (CreditType)reader[i];
                        break;
                    case "LogDesc":
                        this.Description = (string)reader[i];
                        break;
                }
            }
        }


        public CreditLog(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "LogID":
                        this.LogID = (int)dr[i];
                        break;
                    case "SiteID":
                        this.SiteID = (int)dr[i];
                        break;
                    case "GameID":
                        this.GameID = (int)dr[i];
                        break;
                    case "Credit":
                        this.Credit = (decimal)dr[i];
                        break;
                    case "Balance":
                        this.Balance = (decimal)dr[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)dr[i];
                        break;
                    case "SourceID":
                        this.SourceID = (string)dr[i];
                        break;
                    case "Type":
                        this.Type = (CreditType)dr[i];
                        break;
                    case "LogDesc":
                        this.Description = (string)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        [Column("LogID"), DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int LogID { get; set; }


        [Column("SiteID")]
        public int SiteID { get; set; }


        [Column("GameID")]
        public int GameID { get; set; }


        /// <summary>
        /// 账变额度
        /// </summary>
        [Column("Credit")]
        public decimal Credit { get; set; }


        /// <summary>
        /// 账变之后的额度
        /// </summary>
        [Column("Balance")]
        public decimal Balance { get; set; }


        [Column("CreateAt")]
        public long CreateAt { get; set; }


        /// <summary>
        /// 来源
        /// </summary>
        [Column("SourceID")]
        public string SourceID { get; set; }


        /// <summary>
        /// 额度账变类型（转入/转出/上分/下分）
        /// </summary>
        [Column("Type")]
        public CreditType Type { get; set; }


        /// <summary>
        /// 备注信息
        /// </summary>
        [Column("LogDesc")]
        public string Description { get; set; }

        #endregion


        #region  ========  扩展方法  ========


        #endregion

    }

}
