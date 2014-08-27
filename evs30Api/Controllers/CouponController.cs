using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;
using evs.DAL;
using evs.Model;
using Newtonsoft.Json.Linq;
using evs30Api.Filters;

namespace evs30Api.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    public class CouponController : ApiController
    {
        private evsContext db = new evsContext();

        [HttpPost]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage Post(JObject jsonObject)
        {
            Boolean isValidCoupon = false;
            DateTime currentTime = DateTime.Now;
            string message = string.Empty;
            Int32 couponId = 0;
            decimal amount = 0;

            dynamic vm = jsonObject;
            var registrations = new List<Registration>();

            //populate couponCode
            var couponCode = (string)jsonObject["couponCode"];

            //populate registrations
            foreach (dynamic reg in vm.regs)
            {
                var registration = new Registration
                    {
                        EventureListId = reg.eventureListId,
                        ParticipantId = reg.partId,
                        //Name = reg.displayList, //mjb don't use this
                        ListAmount = reg.fee,
                        Quantity = reg.quantity,
                        TotalAmount = Convert.ToDecimal(reg.fee)
                    };
                registrations.Add(registration);
            }

            //get coupon
            var coupons = db.Coupons
                .Where(c => c.DateStart < currentTime
                    && c.DateEnd > currentTime
                    && c.Active
                    && c.Code == couponCode);   //&& c.ownerId == ownerId

            if (coupons.Any())
            {
                isValidCoupon = true;
                couponId = coupons.FirstOrDefault().Id;
            }
            else
            {
                message = "valid coupon is not found";
                amount = 0;
                couponId = -1;
            }

            if (isValidCoupon) //&& notspecialcase   --either owner
            {
                foreach (var reg in registrations)
                {
                    bool isValidForThisReg = true;
                    string couponType = coupons.FirstOrDefault().CouponType;

                    //check type and verify valid for owner/event/list  
                    switch (couponType)
                    {
                        case "owner":
                            var listQuery = db.EventureLists.Where(l => l.Id == reg.EventureListId).Select(l => l.EventureId);
                            var ownerId = db.Eventures
                                                .Where(e => listQuery.Contains(e.Id))
                                                .Select(e => e.OwnerId)
                                                .SingleOrDefault();
                            if (ownerId == coupons.FirstOrDefault().CouponTypeLinkId)
                            {
                                isValidForThisReg = true;
                                //isOwnerCoupon = true;
                            }
                            else
                            {
                                message = "Coupon is invalid (O1)";
                                isValidForThisReg = false;
                            }
                            break;

                        case "event":
                            //confirm that eventureid = couponlinktypeid
                            var eventureId = db.EventureLists
                                                .Where(l => l.Id == reg.EventureListId)
                                                .Select(l => l.EventureId).SingleOrDefault();

                            if (eventureId == coupons.FirstOrDefault().CouponTypeLinkId)
                                isValidForThisReg = true;
                            else
                            {
                                message = "Coupon is invalid (E1)";
                                isValidForThisReg = false;
                            }
                            break;

                        case "list":
                            //confirm that eventurelistid = couponlinktypeid
                            if (reg.EventureListId != coupons.FirstOrDefault().CouponTypeLinkId)
                            {
                                message = "Coupon is invalid (L1)";
                                isValidForThisReg = false;
                            }
                            break;

                        default:
                            message = "Coupon is invalid (D1)";
                            isValidForThisReg = false;
                            break;
                    }

                    //check usage  
                    if (isValidForThisReg)
                    {
                        if (coupons.FirstOrDefault().Usage == 1)
                        {
                            //check if part has already redeemed coupon for this race
                        }
                    }

                    //check redeemed v. capacity
                    if (isValidForThisReg)
                    {
                        if (coupons.FirstOrDefault().Capacity <= coupons.FirstOrDefault().Redeemed)
                        {
                            message = "Coupon is invalid (C1)";
                            isValidForThisReg = false;
                        }
                    }

                    //check check owned
                    if (isValidForThisReg)
                    {
                        if (coupons.FirstOrDefault().IsOnlyForOwned)
                        {
                            //checked if eventure is owned
                            var eid = db.EventureLists.Where(l => l.Id == reg.EventureListId).Select(l => l.EventureId);
                            var isManaged =
                                db.Eventures.Where(e => eid.Contains(e.Id)).Select(e => e.Managed).FirstOrDefault();
                            if (isManaged)
                            {
                                message = "Coupon is invalid (W1)";
                                isValidForThisReg = false;
                            }
                        }
                    }

                    //calculate
                    if (isValidForThisReg)
                    {
                        //message = "valid";
                        if (coupons.FirstOrDefault().DiscountType == 0) //$ off
                        {
                            //for $off coupons we just need to return value so we break from foreach reg loop
                            amount = coupons.FirstOrDefault().Amount * -1;
                            break;
                        }
                        else //percent off
                        {
                            //decimal percent =  * -1;
                            //var listPrice = reg.
                            //db.EventureLists.Where(l => l.Id == reg.EventureListId)
                            //  .Select(l => l.CurrentFee).FirstOrDefault();
                            //var test = percent * listPrice / 100;
                            amount = amount + (coupons.FirstOrDefault().Amount * reg.TotalAmount / 100 * -1);
                            //couponDto.CouponId = coupons.FirstOrDefault().Id;
                        }
                    }
                }
            }

            var coupon = new DtoCoupon(amount, message, couponId);
            var js = new JavaScriptSerializer();
            string jsonData = js.Serialize(coupon);

            var resp = Request.CreateResponse(HttpStatusCode.OK);
            resp.Content = new StringContent(jsonData, Encoding.UTF8, "text/plain");
            return resp;
        }

        public class DtoCoupon
        {
            public decimal Amount { get; set; }
            public string Message { get; set; }
            public Int32 CouponId { get; set; }

            public DtoCoupon(decimal amount, string message, Int32 couponId)
            {
                Amount = amount;
                Message = message;
                CouponId = couponId;
            }
        }

       public IEnumerable<Coupon> GetCoupons()
        {
            return db.Coupons.ToArray();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }

        public class DtoGraph
        {
            public Int32 Id { get; set; }
            public string Month { get; set; }
            public Int32 Coupons { get; set; }


            public DtoGraph(Int32 id, string month, Int32 coupons)
            {
                Id = id;
                Month = month;
                Coupons = coupons;
            }
        }

        public Coupon GetCouponById(int id)
        {
            return db.Coupons.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<Coupon> GetCouponsByOwnerId(int id)
        {
            return db.Coupons.Where(c => c.OwnerId == id).OrderByDescending(c => c.Id);
        }
    }
}




//public object GetCouponsUsedByMonthGraph(int id)
//{
//    var graph = new List<DtoGraph>();

//    //var queryOwnerEventures = db.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);
//    //var queryOwnersLists = db.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);

//    var queryOwnerEventures = db.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);
//    var queryOwnersLists = db.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);

//    var query = from s in db.Surcharges
//                where (s.ChargeType == "coupon")
//                group s by s.DateCreated.Month
//                into dateGroup
//                select new {month = dateGroup.Key, coupons = dateGroup.Count()};

//    int month = 1;
//    foreach (var g in query)
//    {
//        if (g.month != month)  //enter a zero for that month
//        {
//            do
//            {
//                //myList.Add(month, 0);
//                graph.Add(new DtoGraph(g.month, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3), 0, 0));
//                month++;

//            } while (month != g.month);
//        }
//        //myList.Add(g.regmonth, g.Count());
//        graph.Add(new DtoGraph(g.month, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.month).Substring(0, 3), g.regcount, g.revsum));
//        month++;
//    }
//    //must catch any months with 0 at end of 
//    if (month < 12)
//    {
//        do
//        {
//            //Console.Write(month + "|0---");
//            //myList.Add(month, 0);
//            graph.Add(new DtoGraph(month, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3), 0, 0));
//            month++;

//        } while (month < 13);
//    }

//    return graph;
//}

//public DtoCoupon ValidateCoupon(string couponCode, int ownerId)
//{
//    DateTime currentTime = DateTime.Now;
//    Boolean isValidCoupon = false;
//    var couponDto = new DtoCoupon(0, "");


//    // does coupon exist at all
//    var coupons = db.Coupons.Where(c => c.DateStart < currentTime && c.DateEnd > currentTime && c.Active);   //&& c.ownerId == ownerId
//    if (coupons.Any())
//    {
//        isValidCoupon = true;
//    }
//    else
//    {
//        couponDto.Message = "valid coupon is not found";
//    }

//    //check type and verify valid for owner/event/list  
//    if (isValidCoupon)
//    {
//        string couponType = coupons.FirstOrDefault().CouponType;

//        switch (couponType)
//        {
//            case "owner":
//                break;

//            case "eventure":
//                break;

//            case "list":
//                break;

//            default:
//                couponDto.Message = "associated event not event found";
//                isValidCoupon = false;
//                break;
//        }

//    }
//    //check capacity

//    if (isValidCoupon)
//    {
//        couponDto.Message = "valid";
//        couponDto.Amount = coupons.FirstOrDefault().Amount;
//    }

//    return couponDto;
//}

//// GET api/Eventures
//public IEnumerable<Coupon> GetCouponsByOwnerId(int id)
//{
//    return db.Coupons.Where(e => e.OwnerId == id);
//}

//public SaveResult SaveChanges(JObject saveBundle)
//{
//    return _contextProvider.SaveChanges(saveBundle);
//}