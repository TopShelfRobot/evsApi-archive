using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Newtonsoft.Json.Linq;
using evs.DAL;
using evs.Model;
using System.Data.Entity.Core.Objects;

namespace evs.API.Controllers
{
    [BreezeController]
    [RoutePrefix("api/stat")]
    public class StatController : ApiController
    {
    }
}
