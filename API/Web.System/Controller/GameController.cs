using BW.Common.Agent.Games;
using BW.Common.Entities.Games;
using BW.Common.Models.Enums;
using BW.Common.Models.Games;
using BW.Games;
using BW.Games.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.System.Filters;

namespace Web.System.Controller
{
    /// <summary>
    /// 游戏配置
    /// </summary>
    public class GameController : SysControllerBase
    {
        /// <summary>
        /// 获取游戏配置
        /// </summary>
        /// <returns></returns>
        public ContentResult GetGameSetting([FromForm] GameType type, [FromForm] int? id)
        {
            GameModel game = GameAgent.Instance().GetGameModel(id ?? 0);
            IGameBase setting = GameFactory.GetGame(type, game.Setting);
            return this.GetResultContent(setting.ToSettingObject());
        }

        /// <summary>
        /// 保存游戏
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public ContentResult SaveGameInfo([FromForm] int id, [FromForm] GameType type, [FromForm] GameStatus status, [FromForm] string setting)
            => this.GetResultContent(GameAgent.Instance().SaveGameInfo(new Game
            {
                ID = id,
                Type = type,
                Status = status,
                Setting = setting
            }));

        /// <summary>
        /// 游戏列表
        /// </summary>
        /// <returns></returns>
        public ContentResult GetGameList()
        {
            var list = this.BDC.Game.OrderBy(t => t.ID).AsEnumerable();
            return this.GetResultList(list, t => new
            {
                t.ID,
                t.Type,
                t.Status
            });
        }

        /// <summary>
        /// 获取游戏配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContentResult GetGameInfo([FromForm] int id)
        {
            Game game = GameAgent.Instance().GetGameInfo(id) ?? new Game();
            IGameBase setting = null;
            if (game.ID != 0)
            {
                setting = GameFactory.GetGame(game.Type, game.Setting);
            }
            return this.GetResultContent(new
            {
                game.ID,
                game.Type,
                game.Status,
                Setting = setting?.ToSettingObject()
            });
        }
    }
}
