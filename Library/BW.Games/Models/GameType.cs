using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    public enum GameType : byte
    {
        /// <summary>
        /// AG 视讯
        /// </summary>
        AGLive = 1,
        /// <summary>
        /// 泛亚电竞
        /// </summary>
        AVIA = 2,
        /// <summary>
        /// 平博
        /// </summary>
        Pinnacle = 3,
        /// <summary>
        /// IM体育
        /// </summary>
        IMSport = 4,
        /// <summary>
        /// 电竞牛
        /// </summary>
        IMOne = 5,
        /// <summary>
        /// 波音
        /// </summary>
        BBIN = 6,
        /// <summary>
        /// 小金188
        /// </summary>
        XJ188 = 7,
        /// <summary>
        /// 开元棋牌
        /// </summary>
        KY = 8
    }
}
