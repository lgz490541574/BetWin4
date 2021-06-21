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
        /// 保存日志
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">如果是POST请求，发送的内容</param>
        /// <param name="result">返回内容</param>
        /// <param name="success">是否成功</param>
        protected void SaveLog(string url, string result, APIResultType resultType, PostDataModel data)
        {
            this.GameDelegate?.SaveLog(this.Type, url, result, resultType, data);
            //if (resultType != APIResultType.Success)
            //{
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(data.ToJson());
                Console.ResetColor();
            //}
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
        ///  转入资金
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns></returns>
        public abstract TransferResult Recharge(TransferRequest transfer);


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
