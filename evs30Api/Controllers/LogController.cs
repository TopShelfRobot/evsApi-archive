using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using evs.DAL;
using evs.Model;

namespace evs30Api.Controllers
{
    public class LogController : Controller
    {
        //
        // GET: /Log/
        private evsContext db = new evsContext();

        public void Log()
        {
            var log = new EventureLog();
            log.Message = "starting payment";
            log.Caller = "StripePayment";
            log.Status = "Info";
            log.LogDate = System.DateTime.Now.ToLocalTime();
            log.DateCreated = System.DateTime.Now.ToLocalTime();
            db.EventureLogs.Add(log);
            db.SaveChanges();
        }

    }
}
