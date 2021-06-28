using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Entities.Sites
{
    /// <summary>
    /// 商户的额度锁定
    /// </summary>
    [Table("site_CreditLock")]
    public partial class CreditLock
    {

        #region  ========  構造函數  ========
        public CreditLock() { }

        public CreditLock(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "LockID":
                        this.ID = (string)reader[i];
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
                }
            }
        }


        public CreditLock(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "LockID":
                        this.ID = (string)dr[i];
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
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        /// <summary>
        /// 对应的锁定订单号
        /// </summary>
        [Column("LockID"), Key]
        public string ID { get; set; }


        [Column("SiteID")]
        public int SiteID { get; set; }


        [Column("GameID")]
        public int GameID { get; set; }


        /// <summary>
        /// 要锁定的额度
        /// </summary>
        [Column("Credit")]
        public decimal Credit { get; set; }

        #endregion


        #region  ========  扩展方法  ========


        #endregion

    }

}
