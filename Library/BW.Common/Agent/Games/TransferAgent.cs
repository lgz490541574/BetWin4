using BW.Common.Agent.Systems;
using BW.Common.Agent.Users;
using BW.Common.Models.Games;
using BW.Common.Models.Sites;
using BW.Common.Utils;
using BW.Games;
using BW.Games.Exceptions;
using BW.Games.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Agent.Games
{
    /// <summary>
    /// 转账管理
    /// </summary>
    public class TransferAgent : AgentBase<TransferAgent>
    {
        public TransferResult Recharge(int siteId, string userName, int gameId, string orderId, decimal money)
        {
            //#1 判断用户是否存在
            int userId = UserInfoAgent.Instance().GetUserID(siteId, userName);
            if (userId == 0) throw new APIResultException(APIResultType.NOUSER);

            //#1 找出用户在这个游戏里面的用户信息
            GameUserModel gameUser = GameUserAgent.Instance().GetGameUser(gameId, siteId, userId);

            // 如果本地没有当前用户，则自动注册
            if (!gameUser)
            {
                gameUser = LoginAgent.Instance().GameRegister(siteId, userId, gameId);
            }

            IGameBase game = GameUtils.GetGame(gameId);
            if (game == null) throw new APIResultException(APIResultType.MAINTENANCE);

            SiteModel site = SiteAgent.Instance().GetSiteInfo(siteId);
            orderId = string.Concat(site.Prefix, "_", orderId);

            // 数据库操作
            //#1 检查订单号是否重复
            //#2 检查商户额度是否足够
            //#3 创建转账订单，状态为进行中

            TransferResult result = game.Recharge(new TransferRequest { Money = money, OrderID = orderId, UserName = gameUser.UserName });
            if (result.Code == APIResultType.Success)
            {
                // 回写数据库，转账已成功
                return result;
            }

            if (result.Code == APIResultType.Exception)
            {
                // 发生异常，写入消息队列，异步检查转账是否成功
            }

            throw new APIResultException(result.Code);
        }

        public TransferResult Withdraw(int siteId, string userName, int gameId, string orderId, decimal money)
        {
            //#1 判断用户是否存在
            int userId = UserInfoAgent.Instance().GetUserID(siteId, userName);
            if (userId == 0) throw new APIResultException(APIResultType.NOUSER);

            //#1 找出用户在这个游戏里面的用户信息
            GameUserModel gameUser = GameUserAgent.Instance().GetGameUser(gameId, siteId, userId);

            // 如果本地没有当前用户，则自动注册
            if (!gameUser)
            {
                gameUser = LoginAgent.Instance().GameRegister(siteId, userId, gameId);
            }

            IGameBase game = GameUtils.GetGame(gameId);
            if (game == null) throw new APIResultException(APIResultType.MAINTENANCE);

            SiteModel site = SiteAgent.Instance().GetSiteInfo(siteId);
            orderId = string.Concat(site.Prefix, "_", orderId);

            // 数据库操作
            //#1 检查订单号是否重复
            //#2 检查商户额度是否足够
            //#3 创建转账订单，状态为进行中

            TransferResult result = game.Withdraw(new TransferRequest { Money = money, OrderID = orderId, UserName = gameUser.UserName });
            if (result.Code == APIResultType.Success)
            {
                // 回写数据库，转账已成功
                return result;
            }

            if (result.Code == APIResultType.Exception)
            {
                // 发生异常，写入消息队列，异步检查转账是否成功
            }

            throw new APIResultException(result.Code);
        }
    }
}
