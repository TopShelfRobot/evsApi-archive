//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.Web.Mvc;
using System.Threading;

namespace evs30Api.Controllers
{
    public class ManregController : Controller
    {
        //
        // GET: /Reg/

        public ActionResult Index(string id, string email)
        {
            ViewBag.owner = id;
            ViewBag.part = email;
            return View();
        }

        //public ActionResult es(string pid)
        //{
        //    //ViewBag.owner = eid;
        //    ViewBag.part = pid;

        //    return View();
        //}

    }
}
