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
    [Table("game_Order")]
    public partial class GameOrder
    {

        #region  ========  構造函數  ========
        public GameOrder() { }

        public GameOrder(IDataReader reader)
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
                    case "UserID":
                        this.UserID = (int)reader[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)reader[i];
                        break;
                    case "FinishAt":
                        this.FinishAt = (long)reader[i];
                        break;
                    case "BetMoney":
                        this.BetMoney = (decimal)reader[i];
                        break;
                    case "Money":
                        this.Money = (decimal)reader[i];
                        break;
                    case "Game":
                        this.Game = (string)reader[i];
                        break;
                    case "UpdateAt":
                        this.UpdateAt = (long)reader[i];
                        break;
                    case "MD5":
                        this.MD5 = (string)reader[i];
                        break;
                }
            }
        }


        public GameOrder(DataRow dr)
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
                    case "UserID":
                        this.UserID = (int)dr[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)dr[i];
                        break;
                    case "FinishAt":
                        this.FinishAt = (long)dr[i];
                        break;
                    case "BetMoney":
                        this.BetMoney = (decimal)dr[i];
                        break;
                    case "Money":
                        this.Money = (decimal)dr[i];
                        break;
                    case "Game":
                        this.Game = (string)dr[i];
                        break;
                    case "UpdateAt":
                        this.UpdateAt = (long)dr[i];
                        break;
                    case "MD5":
                        this.MD5 = (string)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        /// <summary>
        /// 游戏订单号
        /// </summary>
        [Column("OrderID"), Key]
        public string OrderID { get; set; }


        /// <summary>
        /// 所属游戏
        /// </summary>
        [Column("GameID")]
        public int GameID { get; set; }


        /// <summary>
        /// 所属商户
        /// </summary>
        [Column("SiteID")]
        public int SiteID { get; set; }


        /// <summary>
        /// 本地账号
        /// </summary>
        [Column("UserID")]
        public int UserID { get; set; }


        /// <summary>
        /// 订单创建时间
        /// </summary>
        [Column("CreateAt")]
        public long CreateAt { get; set; }


        /// <summary>
        /// 订单完成时间
        /// </summary>
        [Column("FinishAt")]
        public long FinishAt { get; set; }


        /// <summary>
        /// 投注金额
        /// </summary>
        [Column("BetMoney")]
        public decimal BetMoney { get; set; }


        /// <summary>
        /// 盈亏金额
        /// </summary>
        [Column("Money")]
        public decimal Money { get; set; }


        /// <summary>
        /// 游戏代码
        /// </summary>
        [Column("Game")]
        public string Game { get; set; }


        /// <summary>
        /// 本地数据的更新时间
        /// </summary>
        [Column("UpdateAt")]
        public long UpdateAt { get; set; }


        /// <summary>
        /// 原始数据的MD5码，用于判断是否需要更新数据
        /// </summary>
        [Column("MD5")]
        public string MD5 { get; set; }

        #endregion


        #region  ========  扩展方法  ========

        #endregion

    }

}
