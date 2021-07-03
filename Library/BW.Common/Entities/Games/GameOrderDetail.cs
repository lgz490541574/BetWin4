using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Entities.Games
{
    /// <summary>
    /// 原始订单数据
    /// </summary>
    [Table("game_OrderDetail")]
    public partial class GameOrderDetail
    {

        #region  ========  構造函數  ========
        public GameOrderDetail() { }

        public GameOrderDetail(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "OrderID":
                        this.OrderID = (string)reader[i];
                        break;
                    case "GameID":
                        this.GameID = (int)reader[i];
                        break;
                    case "SiteID":
                        this.SiteID = (int)reader[i];
                        break;
                    case "RawData":
                        this.RawData = (string)reader[i];
                        break;
                }
            }
        }


        public GameOrderDetail(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "OrderID":
                        this.OrderID = (string)dr[i];
                        break;
                    case "GameID":
                        this.GameID = (int)dr[i];
                        break;
                    case "SiteID":
                        this.SiteID = (int)dr[i];
                        break;
                    case "RawData":
                        this.RawData = (string)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        [Column("OrderID"), Key]
        public string OrderID { get; set; }


        [Column("GameID"), Key]
        public int GameID { get; set; }


        [Column("SiteID")]
        public int SiteID { get; set; }


        [Column("RawData")]
        public string RawData { get; set; }

        #endregion


        #region  ========  扩展方法  ========

        #endregion

    }

}
