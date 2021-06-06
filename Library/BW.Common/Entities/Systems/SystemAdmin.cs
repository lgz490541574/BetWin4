using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Common.Models.Enums;
using BW.Common.Models.Systems;

namespace BW.Common.Entities.Systems
{
    /// <summary>
    /// 总管理员
    /// </summary>
    [Table("sys_Admin")]
    public partial class SystemAdmin
    {

        #region  ========  構造函數  ========
        public SystemAdmin() { }

        public SystemAdmin(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "AdminID":
                        this.ID = (int)reader[i];
                        break;
                    case "AdminName":
                        this.AdminName = (string)reader[i];
                        break;
                    case "Password":
                        this.Password = (string)reader[i];
                        break;
                    case "LoginAt":
                        this.LoginAt = (long)reader[i];
                        break;
                    case "LoginIP":
                        this.LoginIP = (string)reader[i];
                        break;
                    case "Status":
                        this.Status = (AdminStatus)reader[i];
                        break;
                }
            }
        }


        public SystemAdmin(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "AdminID":
                        this.ID = (int)dr[i];
                        break;
                    case "AdminName":
                        this.AdminName = (string)dr[i];
                        break;
                    case "Password":
                        this.Password = (string)dr[i];
                        break;
                    case "LoginAt":
                        this.LoginAt = (long)dr[i];
                        break;
                    case "LoginIP":
                        this.LoginIP = (string)dr[i];
                        break;
                    case "Status":
                        this.Status = (AdminStatus)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        /// <summary>
        /// 管理员ID
        /// </summary>
        [Column("AdminID"), DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ID { get; set; }


        [Column("AdminName")]
        public string AdminName { get; set; }


        [Column("Password")]
        public string Password { get; set; }


        /// <summary>
        /// 上次登录时间
        /// </summary>
        [Column("LoginAt")]
        public long LoginAt { get; set; }


        /// <summary>
        /// 登录IP
        /// </summary>
        [Column("LoginIP")]
        public string LoginIP { get; set; }


        /// <summary>
        /// 管理员的状态
        /// </summary>
        [Column("Status")]
        public AdminStatus Status { get; set; }

        #endregion


        #region  ========  扩展方法  ========

        public static implicit operator SystemAdminModel(SystemAdmin admin)
        {
            if (admin == null) return default;
            return new SystemAdminModel
            {
                ID = admin.ID,
                Name = admin.AdminName
            };
        }

        #endregion

    }

}
