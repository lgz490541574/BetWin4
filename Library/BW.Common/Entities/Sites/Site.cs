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

namespace BW.Common.Entities.Sites
{
    /// <summary>
    /// 商户
    /// </summary>
    [Table("Site")]
    public partial class Site
    {

        #region  ========  構造函數  ========
        public Site() { }

        public Site(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "SiteID":
                        this.ID = (int)reader[i];
                        break;
                    case "SiteName":
                        this.Name = (string)reader[i];
                        break;
                    case "Prefix":
                        this.Prefix = (string)reader[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)reader[i];
                        break;
                    case "Status":
                        this.Status = (SiteStatus)reader[i];
                        break;
                    case "SecretKey":
                        this.SecretKey = (Guid)reader[i];
                        break;
                    case "WhiteIP":
                        this.WhiteIP = (string)reader[i];
                        break;
                }
            }
        }


        public Site(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "SiteID":
                        this.ID = (int)dr[i];
                        break;
                    case "SiteName":
                        this.Name = (string)dr[i];
                        break;
                    case "Prefix":
                        this.Prefix = (string)dr[i];
                        break;
                    case "CreateAt":
                        this.CreateAt = (long)dr[i];
                        break;
                    case "Status":
                        this.Status = (SiteStatus)dr[i];
                        break;
                    case "SecretKey":
                        this.SecretKey = (Guid)dr[i];
                        break;
                    case "WhiteIP":
                        this.WhiteIP = (string)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        /// <summary>
        /// 商户ID
        /// </summary>
        [Column("SiteID"), DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ID { get; set; }


        /// <summary>
        /// 商户名
        /// </summary>
        [Column("SiteName")]
        public string Name { get; set; }


        /// <summary>
        /// 用户前缀
        /// </summary>
        [Column("Prefix")]
        public string Prefix { get; set; }


        [Column("CreateAt")]
        public long CreateAt { get; set; }


        /// <summary>
        /// 商户状态
        /// </summary>
        [Column("Status")]
        public SiteStatus Status { get; set; }


        /// <summary>
        /// API通信密钥
        /// </summary>
        [Column("SecretKey")]
        public Guid SecretKey { get; set; }


        /// <summary>
        /// 白名单IP
        /// </summary>
        [Column("WhiteIP")]
        public string WhiteIP { get; set; }

        #endregion


        #region  ========  扩展方法  ========

        public static implicit operator SiteModel(Site site)
        {
            return new SiteModel
            {
                ID = site.ID,
                Name = site.Name,
                Prefix = site.Prefix,
                SecretKey = site.SecretKey,
                Status = site.Status
            };
        }
        #endregion

    }

}
