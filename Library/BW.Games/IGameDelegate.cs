using BW.Games.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Gamess
{
    /// <summary>
    /// 与外部逻辑通信的规范
    /// </summary>
    public interface IGameDelegate
    {
        /// <summary>
        /// 存入日志
        /// </summary>
        /// <param name="game">游戏类型</param>
        /// <param name="url">请求地址</param>
        /// <param name="result">返回内容</param>
        /// <param name="success">是否</param>
        /// <param name="data">发送的数据</param>
        void SaveLog(GameType game, string url, string result, bool success, PostDataModel data);
    }
}
