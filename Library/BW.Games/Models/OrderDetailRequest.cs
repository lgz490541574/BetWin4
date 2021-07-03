using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.Models
{
    /// <summary>
    /// 订单明细查询
    /// </summary>
    public class OrderDetailRequest
    {
        public int GameID { get; set; }

        public string OrderID { get; set; }

        public static implicit operator RedisKey(OrderDetailRequest request)
        {
            return $"{request.GameID}:{request.OrderID}";
        }
    }
}
