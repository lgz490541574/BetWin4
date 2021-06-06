﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Game.Models
{
    /// <summary>
    /// 信息返回的基类
    /// </summary>
    public abstract class ResultBase
    {

        public ResultBase(APIResultType code)
        {
            this.Code = code;
        }

        /// <summary>
        /// 返回的代码
        /// </summary>
        public APIResultType Code { get; set; }

    }
}