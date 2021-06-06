using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP.StudioCore.Enums;
using BW.Common.Models.Users;

namespace BW.Common.Entities.Users
{
    /// <summary>
    /// 会员
    /// </summary>
    [Table("Users")]
    public partial class User
    {

        #region  ========  構造函數  ========
        public User() { }

        public User(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "UserID":
                        this.ID = (int)reader[i];
                        break;
                    case "SiteID":
                        this.SiteID = (int)reader[i];
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
                    case "Money":
                        this.Money = (decimal)reader[i];
                        break;
                    case "Status":
                        this.Status = (UserStatus)reader[i];
                        break;
                }
            }
        }


        public User(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "UserID":
                        this.ID = (int)dr[i];
                        break;
                    case "SiteID":
                        this.SiteID = (int)dr[i];
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
                    case "Money":
                        this.Money = (decimal)dr[i];
                        break;
                    case "Status":
                        this.Status = (UserStatus)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        [Column("UserID"), DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ID { get; set; }


        [Column("SiteID")]
        public int SiteID { get; set; }


        [Column("UserName")]
        public string UserName { get; set; }


        [Column("Password")]
        public string Password { get; set; }


        [Column("CreateAt")]
        public long CreateAt { get; set; }


        [Column("Money")]
        public decimal Money { get; set; }


        /// <summary>
        /// 会员状态
        /// </summary>
        [Column("Status")]
        public UserStatus Status { get; set; }

        #endregion


        #region  ========  扩展方法  ========

        public static implicit operator UserModel(User user)
        {
            return new UserModel
            {
                ID = user.ID,
                SiteID = user.SiteID,
                UserName = user.UserName,
                CreateAt = user.CreateAt,
                Status = user.Status
            };
        }

        #endregion

    }

}
