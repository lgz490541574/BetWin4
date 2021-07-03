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

        internal GameModel GetGameInfo(int gameId)
        {
            return this.NewExecutor().HashGet($"{GAME_INFO}", gameId.GetRedisValue());
        }

        internal GameModel SaveGameInfo(GameModel game)
        {
            this.NewExecutor().HashSet($"{GAME_INFO}", game.ID.GetRedisValue(), game);
            if (!this.NewExecutor().HashExists(ORDER_REQUEST, game.ID))
            {
                this.SaveOrderRequest(game.ID, new OrderRequest());
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

        internal GameUserModel GetGameUser(int gameId, int userId)
        {
            string key = $"{GAME_USER}{gameId}:{userId % 100}";
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
        internal GameUserModel GetGameUser(int gameId, string userName)
        {
            string key = $"{GAME_NAME}{gameId}";
            return this.NewExecutor().HashGet(key, userName);
        }

        internal GameUserModel SaveGameUser(GameUserModel gameUser)
        {
            if (!gameUser) return default;
            string key = $"{GAME_USER}{gameUser.GameID}:{gameUser.UserID % 100}";
            IBatch batch = this.NewExecutor().CreateBatch();
            batch.HashSetAsync(key, gameUser.UserID, gameUser);
            batch.HashSetAsync($"{GAME_NAME}{gameUser.GameID}", gameUser.UserName, gameUser);
            return gameUser;
        }



        #endregion

        #region ========  订单采集任务队列  ========

        private const string ORDER_QUEUE = "ORDER:QUEUE";
        /// <summary>
        /// 获取任务采集队列
        /// </summary>
        /// <returns></returns>
        public int GetOrderQueue()
        {
            RedisValue value = this.NewExecutor().ListLeftPop(ORDER_QUEUE);
            if (value.IsNullOrEmpty) return default;
            return value.GetRedisValue<int>();
        }

        public void SaveOrderQueue(int gameId)
        {
            this.NewExecutor().ListRightPush(ORDER_QUEUE, gameId);
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
        public void SaveOrderRequest(int gameId, OrderRequest orderRequest)
        {
            IBatch batch = this.NewExecutor().CreateBatch();
            batch.HashSetAsync(ORDER_REQUEST, gameId, JsonConvert.SerializeObject(orderRequest));
            batch.ListRightPushAsync(ORDER_QUEUE, gameId);
            batch.Execute();
        }

        public OrderRequest GetOrderRequest(int gameId)
        {
            RedisValue value = this.NewExecutor().HashGet(ORDER_REQUEST, gameId);
            if (value.IsNullOrEmpty) return default;
            return JsonConvert.DeserializeObject<OrderRequest>(value.GetRedisValue<string>());
        }

        #endregion

        #region ========  游戏日志详情（有效期30天)  ========

        /// <summary>
        /// 写入订单详情（有效时间30天）
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="orderId"></param>
        /// <param name="detail"></param>
        public void SaveOrderDetail(OrderDetailResult detail)
        {
            this.NewExecutor().StringSet(detail, detail, TimeSpan.FromDays(30));
        }

        /// <summary>
        /// 批量写入订单详情
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="orderId"></param>
        /// <param name="detail"></param>
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
