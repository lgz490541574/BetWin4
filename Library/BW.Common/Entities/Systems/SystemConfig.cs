using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Common.Models.Enums;

namespace BW.Common.Entities.Systems
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [Table("sys_Config")]
    public partial class SystemConfig
    {

        #region  ========  構造函數  ========
        public SystemConfig() { }

        public SystemConfig(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "Type":
                        this.Type = (ConfigType)reader[i];
                        break;
                    case "Value":
                        this.Value = (string)reader[i];
                        break;
                }
            }
        }


        public SystemConfig(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "Type":
                        this.Type = (ConfigType)dr[i];
                        break;
                    case "Value":
                        this.Value = (string)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        /// <summary>
        /// 配置类型
        /// </summary>
        [Column("Type"), Key]
        public ConfigType Type { get; set; }


        [Column("Value")]
        public string Value { get; set; }

        #endregion


        #region  ========  扩展方法  ========

        #endregion

    }

}
