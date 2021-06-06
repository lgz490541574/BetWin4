using BW.Common.Models.Systems;
using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Http;
using SP.StudioCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BW.System.Filters
{
    [Route("[controller]/[action]"), ApiController]
    public class SysControllerBase : MvcControllerBase
    {
        protected SystemAdminModel AdminInfo => this.context.GetItem<SystemAdminModel>();
    }
}
