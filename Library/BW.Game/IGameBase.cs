﻿using BW.Game.Models;
using SP.StudioCore.Enums;
using SP.StudioCore.Ioc;
using SP.StudioCore.Model;
using System.Collections.Generic;

namespace BW.Game
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
        protected void SaveLog(string url, string result, bool success, PostDataModel data)
        {
            this.GameDelegate?.SaveLog(this.Type, url, result, success, data);
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
    }
}
