using BW.Common.Models.Sites;
using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Http;
using SP.StudioCore.Json;
using SP.StudioCore.Model;
using SP.StudioCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.API.Filters
{
    [Route("v1/[controller]/[action]"), ApiController]
    public abstract class APIControllerBase : MvcControllerBase
    {
        protected SiteModel SiteInfo => HttpContext.GetItem<SiteModel>();

        protected override Result GetResultContent(object data)
        {
            return new Result(ContentType.JSON, data.ToJson());
        }
    }
}
