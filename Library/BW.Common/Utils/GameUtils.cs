using BW.Common.Agent.Games;
using BW.Common.Models.Games;
using BW.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BW.Games;
using BW.Games.Models;

namespace BW.Common.Utils
{
    public static class GameUtils
    {
        /// <summary>
        /// 获取游戏实现实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IGameBase GetGame(GameType type)
        {
            GameModel game = GameAgent.Instance().GetGameModel(type);
            if (!game) return null;
            return GameFactory.GetGame(game.Type, game.Setting);
        }
    }
}
