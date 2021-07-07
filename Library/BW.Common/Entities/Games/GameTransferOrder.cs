using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Common.Models.Enums;
using BW.Games.Models;

namespace BW.Common.Entities.Games
{
    /// <summary>
    /// 游戏转账订单
    /// </summary>
    [Table("game_TransferOrder")]
    public partial class GameTransferOrder
    {

        #region  ========  構造函數  ========
        public GameTransferOrder() { }

        public GameTransferOrder(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "OrderID":
                        this.OrderID = (string)reader[i];
                        break;
                    case "SiteID":
                        this.SiteID = (int)reader[i];
                        break;
                    case "Type":
                        this.Type = (GameType)reader[i];
                        break;
                    case "UserID":
                        this.UserID = (int)reader[i];
                        break;
                    case "SourceID":
                        this.SourceID = (string)reader[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)reader[i];
                        break;
                    case "FinishAt":
                        this.FinishAt = (long)reader[i];
                        break;
                    case "Money":
                        this.Money = (decimal)reader[i];
                        break;
                    case "Status":
                        this.Status = (TransferStatus)reader[i];
                        break;
                }
            }
        }


        public GameTransferOrder(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "OrderID":
                        this.OrderID = (string)dr[i];
                        break;
                    case "SiteID":
                        this.SiteID = (int)dr[i];
                        break;
                    case "Type":
                        this.Type = (GameType)dr[i];
                        break;
                    case "UserID":
                        this.UserID = (int)dr[i];
                        break;
                    case "SourceID":
                        this.SourceID = (string)dr[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)dr[i];
                        break;
                    case "FinishAt":
                        this.FinishAt = (long)dr[i];
                        break;
                    case "Money":
                        this.Money = (decimal)dr[i];
                        break;
                    case "Status":
                        this.Status = (TransferStatus)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        /// <summary>
        /// 商户提交的订单ID
        /// </summary>
        [Column("OrderID"), Key]
        public string OrderID { get; set; }


        /// <summary>
        /// 商户ID
        /// </summary>
        [Column("SiteID"), Key]
        public int SiteID { get; set; }


        /// <summary>
        /// 游戏类型
        /// </summary>
        [Column("Type")]
        public GameType Type { get; set; }


        [Column("UserID")]
        public int UserID { get; set; }


        /// <summary>
        /// 与游戏厂商通信的转账ID
        /// </summary>
        [Column("SourceID")]
        public string SourceID { get; set; }


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
        /// 转账金额（正数为转入，负数为转出）
        /// </summary>
        [Column("Money")]
        public decimal Money { get; set; }


        /// <summary>
        /// 转账状态 Create ,Success,Faild, Exception
        /// </summary>
        [Column("Status")]
        public TransferStatus Status { get; set; }

        #endregion


        #region  ========  扩展方法  ========


        #endregion

    }

}
