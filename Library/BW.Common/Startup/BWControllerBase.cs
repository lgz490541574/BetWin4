using BW.Common.Entities.DataContext;
using SP.StudioCore.Ioc;
using SP.StudioCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Common.Startup
{
    public abstract class BWControllerBase : MvcControllerBase
    {
        protected BetWinDataContext BDC => IocCollection.GetService<BetWinDataContext>();
    }
}
