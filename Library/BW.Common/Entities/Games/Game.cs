using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Common.Models;
using BW.Common.Models.Enums;
using BW.Common.Models.Games;
using BW.Games.Models;

namespace BW.Common.Entities.Games
{
    /// <summary>
    /// 游戏接口配置
    /// </summary>
    [Table("Game")]
    public partial class Game
    {

        #region  ========  構造函數  ========
        public Game() { }

        public Game(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i))
                {
                    case "GameID":
                        this.ID = (int)reader[i];
                        break;
                    case "Type":
                        this.Type = (GameType)reader[i];
                        break;
                    case "Setting":
                        this.Setting = (string)reader[i];
                        break;
                    case "Status":
                        this.Status = (GameStatus)reader[i];
                        break;
                }
            }
        }


        public Game(DataRow dr)
        {
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                switch (dr.Table.Columns[i].ColumnName)
                {
                    case "GameID":
                        this.ID = (int)dr[i];
                        break;
                    case "Type":
                        this.Type = (GameType)dr[i];
                        break;
                    case "Setting":
                        this.Setting = (string)dr[i];
                        break;
                    case "Status":
                        this.Status = (GameStatus)dr[i];
                        break;
                }
            }
        }

        #endregion

        #region  ========  数据库字段  ========

        /// <summary>
        /// 游戏ID
        /// </summary>
        [Column("GameID"), DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ID { get; set; }


        /// <summary>
        /// 所属游戏类型
        /// </summary>
        [Column("Type")]
        public GameType Type { get; set; }


        /// <summary>
        /// 游戏配置
        /// </summary>
        [Column("Setting")]
        public string Setting { get; set; }


        /// <summary>
        /// 游戏状态 正常/维护
        /// </summary>
        [Column("Status")]
        public GameStatus Status { get; set; }

        #endregion


        #region  ========  扩展方法  ========

        public static implicit operator GameModel(Game game)
        {
            if (game == null) return default;
            return new GameModel
            {
                ID = game.ID,
                Type = game.Type,
                Setting = game.Setting,
                Status = game.Status
            };
        }

        #endregion

    }

}
