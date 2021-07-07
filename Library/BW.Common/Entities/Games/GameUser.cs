using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Common.Models.Games;
using BW.Games.Models;

namespace BW.Common.Entities.Games
{
    /// <summary>
    /// 会员在游戏中的游戏名称
    /// </summary>
    [Table("game_User")]
    public partial class GameUser
    {

        #region  ========  構造函數  ========
        public GameUser() { }

        public GameUser(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "Type":
                        this.Type = (GameType)reader[i];
                        break;
                    case "SiteID":
                        this.SiteID = (int)reader[i];
                        break;
                    case "UserID":
                        this.UserID = (int)reader[i];
                        break;
                    case "UserName":
                        this.UserName = (string)reader[i];
                        break;
                    case "Password":
                        this.Password = (string)reader[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)reader[i];
                        break;
                    case "Balance":
                        this.Balance = (decimal)reader[i];
                        break;
                    case "UpdateAt":
                        this.UpdateAt = (long)reader[i];
                        break;
                }
            }
        }


        public GameUser(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "Type":
                        this.Type = (GameType)dr[i];
                        break;
                    case "SiteID":
                        this.SiteID = (int)dr[i];
                        break;
                    case "UserID":
                        this.UserID = (int)dr[i];
                        break;
                    case "UserName":
                        this.UserName = (string)dr[i];
                        break;
                    case "Password":
                        this.Password = (string)dr[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)dr[i];
                        break;
                    case "Balance":
                        this.Balance = (decimal)dr[i];
                        break;
                    case "UpdateAt":
                        this.UpdateAt = (long)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        /// <summary>
        /// 游戏类型
        /// </summary>
        [Column("Type"), Key]
        public GameType Type { get; set; }


        [Column("SiteID"), Key]
        public int SiteID { get; set; }


        [Column("UserID"), Key]
        public int UserID { get; set; }


        /// <summary>
        /// 在游戏中的账号
        /// </summary>
        [Column("UserName")]
        public string UserName { get; set; }


        /// <summary>
        /// 游戏账号密码（如果需要的话）
        /// </summary>
        [Column("Password")]
        public string Password { get; set; }


        [Column("CreateAt")]
        public long CreateAt { get; set; }


        /// <summary>
        /// 当前的余额
        /// </summary>
        [Column("Balance")]
        public decimal Balance { get; set; }


        /// <summary>
        /// 余额的更新时间
        /// </summary>
        [Column("UpdateAt")]
        public long UpdateAt { get; set; }

        #endregion


        #region  ========  扩展方法  ========

        public static implicit operator GameUserModel(GameUser gameUser)
        {
            if (gameUser == null) return default;
            return new GameUserModel
            {
                Type = gameUser.Type,
                SiteID = gameUser.SiteID,
                UserID = gameUser.UserID,
                UserName = gameUser.UserName,
                Password = gameUser.Password
            };
        }
        #endregion

    }

}
