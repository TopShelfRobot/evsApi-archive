using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using evs.DAL;
using evs.Model;

namespace evs.API.Controllers
{
    [RoutePrefix("api/Orders")]
    public class EventuresController : ApiController
    {
        private evsContext db = new evsContext();

        public IEnumerable<Eventure> GetAllEventuresByOwnerId(int id)
        {
           return db.Eventures.Where(e => e.OwnerId == id).OrderByDescending(e => e.Id);
            
        }
    }
}
