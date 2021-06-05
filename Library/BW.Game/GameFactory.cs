using BW.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BW.Game
{
    /// <summary>
    /// 游戏实现工厂
    /// </summary>
    public static class GameFactory
    {
        public static IGameBase GetGame(GameType game, string setting)
        {
            Assembly assembly = typeof(GameFactory).Assembly;
            string typeName = $"{ typeof(GameFactory).Namespace }.API.{ game }";
            Type type = assembly.GetType(typeName);
            if (type == null) return null;
            return (IGameBase)Activator.CreateInstance(type, new[] { setting });
        }
    }
}
