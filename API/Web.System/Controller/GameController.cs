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
        public ContentResult GetGameSetting([FromForm] GameType type)
        {
            GameModel game = GameAgent.Instance().GetGameModel(type);
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
        public ContentResult SaveGameInfo([FromForm] GameType type, [FromForm] GameStatus status, [FromForm] string setting)
            => this.GetResultContent(GameAgent.Instance().SaveGameInfo(new Game
            {
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
            var list = this.BDC.Game.OrderBy(t => t.Type).ToList();
            return this.GetResultList(list, t => new
            {
                t.Type,
                t.Status
            });
        }

        /// <summary>
        /// 获取游戏配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContentResult GetGameInfo([FromForm] GameType type)
        {
            Game game = GameAgent.Instance().GetGameInfo(type);
            IGameBase setting = GameFactory.GetGame(type, game?.Setting);
            return this.GetResultContent(new
            {
                Type = type,
                game?.Status,
                Setting = setting?.ToSettingObject()
            });
        }

        /// <summary>
        /// 更新游戏配置缓存
        /// </summary>
        /// <returns></returns>
        public ContentResult UpdateCache()
            => this.GetResultContent(GameAgent.Instance().UpdateCache());
    }
}
