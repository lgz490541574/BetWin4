using BW.Common.Caching;
using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Http;
using SP.StudioCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.API.Filters;
using Web.API.Properties;

namespace Web.API.Controller
{
    public class LoginController : APIControllerBase
    {
        [Guest, Route("/login")]
        public ContentResult Login([FromQuery] string id)
        {
            string session = UserCaching.Instance().GetSession(id);
            if (string.IsNullOrEmpty(session))
            {
                return this.context.ShowError(HttpStatusCode.NotFound);
            }

            string html = Resources.login.Replace("\"${SESSION}\"", session);
            return new ContentResult()
            {
                ContentType = "text/html",
                Content = html,
                StatusCode = 200
            };
        }
    }
}
