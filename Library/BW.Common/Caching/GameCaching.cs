using BW.Common.Models.Games;
using BW.Games.Models;
using Newtonsoft.Json;
using SP.StudioCore.Cache.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Caching
{
    public sealed class GameCaching : CacheBase<GameCaching>
    {
        protected override int DB_INDEX => RedisIndex.GAME;

        #region ========  游戏配置信息  ========


        private const string GAME_INFO = "GAME:INFO";

        internal GameModel GetGameInfo(GameType type)
        {
            return this.NewExecutor().HashGet($"{GAME_INFO}", type.GetRedisValue());
        }

        internal GameModel SaveGameInfo(GameModel game)
        {
            this.NewExecutor().HashSet($"{GAME_INFO}", game.Type.GetRedisValue(), game);
            if (!this.NewExecutor().HashExists(ORDER_REQUEST, game.Type.GetRedisValue()))
            {
                this.SaveOrderRequest(game.Type, new OrderRequest());
            }
            return game;
        }

        #endregion

        #region ========  游戏账号信息  ========

        private const string GAME_USER = "GAME:USER:";

        /// <summary>
        /// 游戏账号对应的用户ID
        /// </summary>
        private const string GAME_NAME = "GAME:NAME:";

        internal GameUserModel GetGameUser(GameType type, int userId)
        {
            string key = $"{GAME_USER}{type}:{userId % 100}";
            return this.NewExecutor().HashGet(key, userId);
        }

        private const string TRNASFER = "TRANSFER:";

        /// <summary>
        /// 检查转账订单号是否可用
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        internal bool CheckTransferID(int siteId, string orderId)
        {
            string key = $"{TRNASFER}{siteId}:{orderId}";
            return this.NewExecutor().StringSet(key, true, TimeSpan.FromDays(1), When.NotExists);
        }

        internal bool RemoveTransferID(int siteId, string orderId)
        {
            string key = $"{TRNASFER}{siteId}:{orderId}";
            return this.NewExecutor().KeyDelete(key);
        }

        /// <summary>
        /// 游戏名字获取用户资料
        /// </summary>
        internal GameUserModel GetGameUser(GameType type, string userName)
        {
            string key = $"{GAME_NAME}{type}";
            return this.NewExecutor().HashGet(key, userName);
        }

        internal GameUserModel SaveGameUser(GameUserModel gameUser)
        {
            if (!gameUser) return default;
            string key = $"{GAME_USER}{gameUser.Type}:{gameUser.UserID % 100}";
            IBatch batch = this.NewExecutor().CreateBatch();
            batch.HashSetAsync(key, gameUser.UserID, gameUser);
            batch.HashSetAsync($"{GAME_NAME}{gameUser.Type}", gameUser.UserName, gameUser);
            return gameUser;
        }



        #endregion

        #region ========  订单采集任务队列  ========

        private const string ORDER_QUEUE = "ORDER:QUEUE";
        /// <summary>
        /// 获取任务采集队列
        /// </summary>
        /// <returns></returns>
        public GameType GetOrderQueue()
        {
            RedisValue value = this.NewExecutor().ListLeftPop(ORDER_QUEUE);
            if (value.IsNullOrEmpty) return default;
            return value.GetRedisValue<GameType>();
        }

        public void SaveOrderQueue(GameType type)
        {
            this.NewExecutor().ListRightPush(ORDER_QUEUE, type.GetRedisValue());
        }

        public void SaveOrderQueue()
        {
            RedisValue[] games = this.NewExecutor().HashKeys(ORDER_REQUEST);
            this.NewExecutor().KeyDelete(ORDER_QUEUE);
            this.NewExecutor().ListRightPush(ORDER_QUEUE, games);
        }

        private const string ORDER_REQUEST = "ORDER:REQUEST";
        /// <summary>
        /// 存入订单采集时间节点并且再次写入任务
        /// </summary>
        public void SaveOrderRequest(GameType type, OrderRequest orderRequest)
        {
            IBatch batch = this.NewExecutor().CreateBatch();
            batch.HashSetAsync(ORDER_REQUEST, type.GetRedisValue(), JsonConvert.SerializeObject(orderRequest));
            batch.ListRightPushAsync(ORDER_QUEUE, type.GetRedisValue());
            batch.Execute();
        }

        public OrderRequest GetOrderRequest(GameType type)
        {
            RedisValue value = this.NewExecutor().HashGet(ORDER_REQUEST, type.GetRedisValue());
            if (value.IsNullOrEmpty) return default;
            return JsonConvert.DeserializeObject<OrderRequest>(value.GetRedisValue<string>());
        }

        #endregion

        #region ========  游戏日志详情（有效期30天)  ========

        /// <summary>
        /// 写入订单详情（有效时间30天）
        /// </summary>
        public void SaveOrderDetail(OrderDetailResult detail)
        {
            this.NewExecutor().StringSet(detail, detail, TimeSpan.FromDays(30));
        }

        /// <summary>
        /// 批量写入订单详情
        /// </summary>
        public void SaveOrderDetail(IEnumerable<OrderDetailResult> details)
        {
            IBatch batch = this.NewExecutor().CreateBatch();
            foreach (OrderDetailResult detail in details)
            {
                batch.StringSetAsync(detail, detail, TimeSpan.FromDays(30));
            }
            batch.Execute();
        }

        /// <summary>
        /// 批量获取订单明细
        /// </summary>
        /// <returns></returns>
        public List<OrderDetailResult> GetOrderDetail(OrderDetailRequest[] requests)
        {
            List<OrderDetailResult> list = new();
            if (!requests.Any()) return list;
            RedisValue[] values = this.NewExecutor().StringGet(requests.Select(t => (RedisKey)t).ToArray());
            for (int index = 0; index < requests.Length; index++)
            {
                RedisValue value = values[index];
                if (value.IsNullOrEmpty) continue;
                list.Add(new OrderDetailResult(requests[index], value));
            }
            return list;
        }

        #endregion
    }
}
