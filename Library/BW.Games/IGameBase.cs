using BW.Games.Exceptions;
using BW.Games.Models;
using BW.Gamess;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Enums;
using SP.StudioCore.Ioc;
using SP.StudioCore.Json;
using SP.StudioCore.Model;
using System;
using System.Collections.Generic;

namespace BW.Games
{
    public abstract class IGameBase : ISetting
    {
        protected IGameDelegate GameDelegate => IocCollection.GetService<IGameDelegate>();

        protected GameType Type => this.GetType().Name.ToEnum<GameType>();

        /// <summary>
        /// 用户名的分隔符
        /// </summary>
        protected virtual char UserSplit => '_';

        /// <summary>
        /// 当前的时区
        /// </summary>
        protected virtual TimeSpan OffsetTime => TimeZoneInfo.Local.BaseUtcOffset;

        protected string GetUserName(RegisterRequest register)
        {
            if (string.IsNullOrEmpty(register.Prefix)) return register.UserName;
            return string.Concat(register.Prefix, this.UserSplit, register.UserName);
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">如果是POST请求，发送的内容</param>
        /// <param name="result">返回内容</param>
        /// <param name="success">是否成功</param>
        protected void SaveLog(string url, string result, APIResultType resultType, PostDataModel data)
        {
            this.GameDelegate?.SaveLog(this.Type, url, result, resultType, data);
            lock (typeof(IGameBase))
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine(url);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(data.ToJson());
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(result);
                Console.ResetColor();
                Console.WriteLine("".PadLeft(32, '='));
            }
        }

        protected IGameBase(string queryString) : base(queryString)
        {

        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public abstract LoginResult Login(LoginRequest login);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public abstract RegisterResult Register(RegisterRequest register);

        /// <summary>
        /// 查询余额
        /// </summary>
        /// <param name="balance"></param>
        /// <returns></returns>
        public abstract BalanceResult Balance(BalanceRequest balance);

        /// <summary>
        ///  转入资金
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns></returns>
        public abstract TransferResult Recharge(TransferRequest transfer);

        /// <summary>
        /// 转出资金
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns></returns>
        public abstract TransferResult Withdraw(TransferRequest transfer);

        /// <summary>
        /// 拉取订单
        /// </summary>
        /// <param name="time">时间戳条件</param>
        /// <returns></returns>
        public abstract IEnumerable<OrderResult> GetOrders(OrderRequest order);


        internal abstract PostResult POST(string method, Dictionary<string, object> data);


        protected APIResultType POST(string method, Dictionary<string, object> data, out object info)
        {
            PostResult result = null;
            info = null;
            try
            {
                result = this.POST(method, data);
                info = result.Info;
            }
            catch (APIResultException ex)
            {
                result.Ex = ex;
                result.Code = ex.Type;
            }
            catch (Exception ex)
            {
                result.Ex = ex;
                result.Code = APIResultType.Exception;
            }
            finally
            {
                this.SaveLog(result.Url, result.GetResult(), result.Code, result);
            }
            return result.Code;
        }


    }
}
