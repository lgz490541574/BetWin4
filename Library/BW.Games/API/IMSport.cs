﻿using BW.Games.Exceptions;
using BW.Games.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SP.StudioCore.Net;
using SP.StudioCore.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BW.Games.API
{
    public class IMSport : IM
    {
        public IMSport(string queryString) : base(queryString)
        {

        }       
    }
}
