//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web;
//using System.Web.Http;
//using evs.Model;
//using evs.DAL;

//namespace evs30Api.Controllers
//{
//    public class FeeController : ApiController
//    {
//        private evsContext db = new evsContext();
        
//        public IEnumerable<FeeSchedule> GetFeesByEventureListId(int id)
//        {
//            return db.FeeSchedules.Where(f => f.EventureListId == id);
//        }
        
//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}