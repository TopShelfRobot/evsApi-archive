using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Http;
//using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using Newtonsoft.Json.Linq;
using Stripe;
using WebMatrix.WebData;
using evs.DAL;
using evs.Model;
using System.Globalization;
using Newtonsoft.Json;

using System.Web.Security;

namespace evs30Api.Controllers
{
    public class RegistrationsController : ApiController
    {
        evsContext db = new evsContext();

        //move this stuff to its own class maybe statistics
        public class DtoGraph
        {
            public Int32 Id { get; set; }
            public string Month { get; set; }
            public Int32 Regs { get; set; }
            public decimal Rev { get; set; }

            public DtoGraph(Int32 id, string month, Int32 regs, decimal rev)
            {
                Id = id;
                Month = month;
                Regs = regs;
                Rev = rev;
            }
        }

        public class DtoGroup
        {
            public Int32 Id { get; set; }
            public string Name { get; set; }
            public Int32 Capacity { get; set; }
            public decimal Total { get; set; }

            public DtoGroup(Int32 id, string name, Int32 capacity, decimal total)
            {
                Id = id;
                Name = name;
                Capacity = capacity;
                Total = total;
            }
        }

        public class DtoCoverflow
        {
            public string title { get; set; }
            public string image { get; set; }

            public DtoCoverflow(string intitle, string inimage)
            {
                title = intitle;
                image = inimage;
            }
        }

        //public class  DtoCapacity
        //{
        //    public Int32 Regs { get; set; }
        //    public Int32 Capacity { get; set; }

        //    public DtoCapacity(Int32 regs, Int32 capacity)
        //    {
        //        Regs = regs;
        //        Capacity = capacity;
        //    }
        //}


        //[HttpPost]
        //[AllowAnonymous]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage Transfer(JObject saveBundle)
        {
            try
            {
                //int numOfRegs = 0;
                //decimal totalFees = 0;

                var transferId = (Int32)saveBundle["transferId"];
                var transferNewListName = (string)saveBundle["transferNewListName"];
                var participantId = (Int32)saveBundle["partId"];

                var log = new EventureLog();
                log.Message = "starting transfer: " + transferId;
                log.Caller = "transfer";
                log.Status = "Info";
                log.LogDate = System.DateTime.Now.ToLocalTime();
                db.EventureLogs.Add(log);
                db.SaveChanges();

                var transfer = db.EventureTransfers.Where(t => t.Id == transferId).Single();

                var order = new EventureOrder
                 {
                     DateCreated = DateTime.Now,
                     //HouseId = (Int32)saveBundle["houseId"],
                     Amount = (Decimal)saveBundle["amount"],
                     Token = (string)saveBundle["token"],   //is this safe??
                     OwnerId = (Int32)saveBundle["ownerId"],
                     Status = "init transfer",
                     Voided = false
                 };
                db.Orders.Add(order);

                Owner owner = db.Owners.Where(o => o.Id == order.OwnerId).SingleOrDefault();
                if (owner == null)
                {
                    throw new Exception("Owner Setup is Not Configured Correctly");
                }

                //validate
                //must have transferId,
                //i could process without partId  just no email

                //calulate
                order.CardProcessorFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.CardProcessorFeePercentPerCharge / 100, 0) + owner.CardProcessorFeeFlatPerChargeInCents);
                order.LocalFeeInCents = Convert.ToInt32(Math.Round(Convert.ToInt32(order.Amount * 100) * owner.LocalFeePercentOfCharge / 100, 0) + owner.LocalFeeFlatPerPerChargeInCents);
                order.LocalApplicationFee = order.LocalFeeInCents - order.CardProcessorFeeInCents;

                string custDesc = string.Empty;
                string partEmail = string.Empty;
                var part = db.Participants.Where(p => p.Id == participantId).FirstOrDefault();
                if (part != null)
                {
                    custDesc = "_transfer" + transferId;
                    partEmail = part.Email;
                }
                else
                {
                    //this should never happen  throw exception?
                    //NO house Id
                    throw new Exception("There was an issue with submission, Not signed into account.");
                }

                // create customer
                var customerOptions = new StripeCustomerCreateOptions
                {
                    Email = partEmail,
                    Description = custDesc,
                    TokenId = order.Token,
                };

                var stripeCustomerService = new StripeCustomerService(owner.AccessToken);
                var customer = stripeCustomerService.Create(customerOptions);

                var stripeChargeService = new StripeChargeService(owner.AccessToken); //The token returned from the above method
                var stripeChargeOption = new StripeChargeCreateOptions()
                {
                    AmountInCents = Convert.ToInt32(order.Amount * 100),
                    Currency = "usd",
                    CustomerId = customer.Id,
                    Description = owner.Name,   //this needs to be dynamic
                    ApplicationFeeInCents = order.LocalApplicationFee
                };
                var stripeCharge = stripeChargeService.Create(stripeChargeOption);

                if (string.IsNullOrEmpty(stripeCharge.FailureCode))
                {
                    // update reg
                    var reg = db.Registrations.Where(r => r.Id == transfer.RegistrationId).Single();

                    reg.EventureListId = transfer.EventureListIdTo;
                    // mjb 060914  reg.Name = transferNewListName;
                    reg.Type = "xferup";
                    transfer.IsComplete = true;

                    order.AuthorizationCode = stripeCharge.Id;
                    //stripeCharge.
                    order.CardNumber = stripeCharge.StripeCard.Last4;
                    order.CardCvcCheck = stripeCharge.StripeCard.CvcCheck;
                    order.CardExpires = stripeCharge.StripeCard.ExpirationMonth + "/" + stripeCharge.StripeCard.ExpirationYear;
                    order.CardFingerprint = stripeCharge.StripeCard.Fingerprint;
                    //order.CardId = stripeCharge.StripeCard.;
                    order.CardName = stripeCharge.StripeCard.Name;
                    order.CardOrigin = stripeCharge.StripeCard.Country;
                    order.CardType = stripeCharge.StripeCard.Type;
                    order.Voided = false;
                    order.Status = "Complete";

                    db.SaveChanges();

                    var resp = Request.CreateResponse(HttpStatusCode.OK);
                    //resp.Content = new StringContent();
                    resp.Content = new StringContent(transferId.ToString(), Encoding.UTF8, "text/plain");    //send transferId??  just for practice??
                    return resp;

                    //var resp = Request.CreateResponse(HttpStatusCode.OK);
                    ////resp.Content = new StringContent();
                    //resp.Content = new StringContent(order.Id.ToString(), Encoding.UTF8, "text/plain");
                    //return resp;
                }
                else
                {
                    order.Status = stripeCharge.FailureMessage;
                    db.SaveChanges();  //should i save here?  
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed, stripeCharge);
                }

            }
            catch (Exception ex)
            {
                //send quick email

                HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_Payment_Post: " + ex.Message + "\n\n" + ex.InnerException);

                //regular log
                var logE = new EventureLog
                    {
                        Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException,
                        Caller = "Payment_Post",
                        Status = "ERROR",
                        LogDate = System.DateTime.Now.ToLocalTime()
                    };
                db.EventureLogs.Add(logE);
                db.SaveChanges();

                var returnMessage = "There was error with your transaction, please try again.";

                if (ex.Source == "Stripe.net")
                    returnMessage = ex.Message;

                if (Request != null)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, returnMessage);
                //return Request.CreateResponse(HttpStatusCode.InternalServerError);
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    //return new HttpResponseMessage(HttpStatusCode.InternalServerError,);
                }
            }

        }

        public IEnumerable<DtoCoverflow> GetCoverflowByParticipantId(int id)
        {
            var cList = new List<DtoCoverflow>();

            var queryReg = from r in db.Registrations
                           join l in db.EventureLists
                               on r.EventureListId equals l.Id
                           where r.ParticipantId == id
                           select new { title = l.DisplayName, image = l.ImageFileName };

            foreach (var reg in queryReg)
            {
                //string filepath = @"http://localhost:59839/Content/images/" + reg.image;
                string filepath = @"/Content/images/" + reg.image;
                cList.Add(new DtoCoverflow(reg.title, filepath));
            }

            //if (rev.Equals(null))
            //    cList.Add(new DtoCoverflow(regcount, 0));
            //else
            //    cList.Add(new DtoCoverflow(regcount, rev));

            return cList.ToArray();
        }
        
        public object DoDcStuffxset()
        {
            var graph = new List<DtoGraph>();

            //var token = WebSecurity.GeneratePasswordResetToken("boone@eventuresports.com");

            //WebSecurity.CreateUserAndAccount("kmacdonald@eventuresports.com", "reset");

            //var token = WebSecurity.GeneratePasswordResetToken("boone@eventuresports.com");


            //var partEmails = db.Participants.Where(p => p.Id == p.HouseId).Select(p => p.Email);

            //foreach (var email in partEmails)
            //{
            //    string random = "sdfaw35rdfgs3";

            //    WebSecurity.CreateUserAndAccount(email, random);

            //}


            //WebSecurity.CreateUserAndAccount("kmacdonald@firstegg.com", "234324dfrws53");

            //int[] ids = new int[] { 1, 2, 3, 45, 99 };

            var emails = (from r in db.Registrations
                          join p in db.Participants
                          on r.ParticipantId equals p.Id
                          where new[] { 3, 4, 5, 51, 52, 53 }.Contains(r.EventureListId)
                          && !new[] { "kmacdonald@eventuresports.com", "kmacdonald@firstegg.com" }.Contains(p.Email)

                          select p.Email).Distinct();

            int count = 0;
            var email = string.Empty;
            var message = string.Empty;

            foreach (var partEmail in emails)
            {

                email = partEmail;
                HttpResponseMessage result = new MailController().SendEmailStock(email, "QuickMail");
                count++;
                if (count % 5 == 0)
                {
                    //message = message + "50 ";
                    Thread.Sleep(1000);
                }
            }


            return graph;
        }

        public object GetOwnerGraph(int id)
        //need to check order.Status == 'Complete'   //mjb 
        {
            var graph = new List<DtoGraph>();

            var queryOwnerEventures = db.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);
            var queryOwnersLists = db.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);

            var query = from r in db.Registrations
                        where queryOwnersLists.Contains(r.EventureListId)
                        group r by r.DateCreated.Month
                            into reggroup
                            select new
                            {
                                regmonth = reggroup.Key,
                                regcount = reggroup.Sum(s => s.Quantity),
                                revsum = reggroup.Sum(s => s.TotalAmount)
                            };
            int month = 1;
            foreach (var g in query)
            {
                if (g.regmonth != month)  //enter a zero for that month
                {
                    do
                    {
                        //myList.Add(month, 0);
                        graph.Add(new DtoGraph(month, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3), 0, 0));
                        month++;

                    } while (month != g.regmonth);
                }
                //myList.Add(g.regmonth, g.Count());
                graph.Add(new DtoGraph(g.regmonth, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.regmonth).Substring(0, 3), g.regcount, g.revsum));
                month++;
            }
            //must catch any months with 0 at end of 
            if (month < 12)
            {
                do
                {
                    //Console.Write(month + "|0---");
                    //myList.Add(month, 0);
                    graph.Add(new DtoGraph(month, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3), 0, 0));
                    month++;

                } while (month < 13);
            }

            return graph;
        }

        public object GetEventureGraph(int id)
        {
            //need to check order.Status == 'Complete'   //mjb 
            var graph = new List<DtoGraph>();

            //var queryOwnerEventures = db.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);

            var queryLists = db.EventureLists.Where(el => el.EventureId == id).Select(l => l.Id);

            var query = from r in db.Registrations
                        where queryLists.Contains(r.EventureListId)
                        group r by r.DateCreated.Month
                            into reggroup
                            select new
                            {
                                regmonth = reggroup.Key,
                                regcount = reggroup.Sum(s => s.Quantity),
                                revsum = reggroup.Sum(s => s.TotalAmount)
                            };

            int month = 1;
            foreach (var g in query)
            {
                if (g.regmonth != month)  //enter a zero for that month
                {
                    do
                    {
                        //myList.Add(month, 0);
                        graph.Add(new DtoGraph(month, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3), 0, 0));
                        month++;

                    } while (month != g.regmonth);
                }
                //myList.Add(g.regmonth, g.Count());
                graph.Add(new DtoGraph(g.regmonth, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.regmonth).Substring(0, 3), g.regcount, g.revsum));
                month++;
            }
            //must catch any months with 0 at end of 
            if (month < 12)
            {
                do
                {
                    //Console.Write(month + "|0---");
                    //myList.Add(month, 0);
                    graph.Add(new DtoGraph(month, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3), 0, 0));
                    month++;

                } while (month < 13);
            }

            return graph;
        }

        public object GetEventureListGraph(int id)
        {
            //need to check order.Status == 'Complete'   //mjb 
            var graph = new List<DtoGraph>();

            //var queryOwnerEventures = db.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);

            var query = from r in db.Registrations
                        where r.EventureListId == id
                        group r by r.DateCreated.Month
                            into reggroup
                            select new
                            {
                                regmonth = reggroup.Key,
                                regcount = reggroup.Sum(s => s.Quantity),
                                revsum = reggroup.Sum(s => s.TotalAmount)
                            };

            int month = 1;
            foreach (var g in query)
            {
                if (g.regmonth != month)  //enter a zero for that month
                {
                    do
                    {
                        //myList.Add(month, 0);
                        graph.Add(new DtoGraph(month, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3), 0, 0));
                        month++;

                    } while (month != g.regmonth);
                }
                //myList.Add(g.regmonth, g.Count());
                graph.Add(new DtoGraph(g.regmonth, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.regmonth).Substring(0, 3), g.regcount, g.revsum));
                month++;
            }
            //must catch any months with 0 at end of 
            if (month < 12)
            {
                do
                {
                    //Console.Write(month + "|0---");
                    //myList.Add(month, 0);
                    graph.Add(new DtoGraph(month, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3), 0, 0));
                    month++;

                } while (month < 13);
            }

            return graph;
        }

        public object GetEventureGroupGraphByList(int id)
        {
            //need to check order.Status == 'Complete'   //mjb 
            var groups = from g in db.EventureGroups 
                       //where g.Capacity > g.Registration.Count()
                       //select new {g.Id, g.Name, grpCount = g.Registration.Count()};
                         join r in db.Registrations.Where(r => r.EventureOrder.Status == "Complete")
                       on g.Id equals r.GroupId into rg
                       //on r.EventureOrder.Status equals "Complete"   //&& (g.Id equals r.GroupId)
                       ////&& 
                      where g.EventureListId == id
                      orderby g.Name
                      select new { groupName = g.Name, regCount = rg.Count(), id = g.Id};
            //select new {g};   

            return groups;
 }


        public class DtoGroupGraph
        {
            public Int32 Id { get; set; }
            public string Name { get; set; }
            public decimal Count { get; set; }

            public DtoGroupGraph(Int32 id, string name, Int32 count)
            {
                Id = id;
                Name = name;
                Count = count;
            }
        }


        //[HttpGet]
        //public IEnumerable<EventureGroup> GroupsBelowCapacity(int listId)      //mjb this needs complete
        //{
        //    string query =
        //        " select distinct g.* from EventureGroup g " +
        //        " left join Registration r on g.Id = r.GroupId " +
        //        " where (select count(*) from Registration where groupid = g.id) < g.Capacity  " +
        //        " AND g.Active = 1" +
        //        " AND g.EventureListId = " + listId;

        //    return _contextProvider.Context.Database.SqlQuery<EventureGroup>(query);
        //}



        public IEnumerable<Registration> GetRegistrations()
        {
            return db.Registrations.ToArray();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }

        //is this used??
        public Registration GetRegistration(int id)
        {
            return db.Registrations.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<Registration> GetRegistrationsByPartId(int id)
        {
            //need to check order.Status == 'Complete'   //mjb 
            return db.Registrations.Where(r => r.ParticipantId == id);
        }

        public IEnumerable<Registration> GetRegistrationsByOwnerId(int id)
        {
            //need to check order.Status == 'Complete'   //mjb 
            var queryOwnerEventures = db.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);
            var queryOwnersLists = db.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);
            return db.Registrations.Where(r => queryOwnersLists.Contains(r.EventureListId) && r.EventureOrder.Status == "Complete"); //.Where(r => queryOwnersLists.Contains(r.EventureList));
        }

        public Int32 GetRegCountByEventureId(Int32 eventureId)
        {
            //need to check order.Status == 'Complete'   //mjb 
            return db.Registrations.Count(r => r.EventureListId == eventureId);
        }

        //mjb i moved this to breeze controller  but had to leave this here for legacy purposes
        //kendo is still using it

        //public IEnumerable<DtoCapacity> GetCapacityByOwnerId(int id)
        //{
        //    var rList = new List<DtoCapacity>();

        //    var queryOwnerEventures = db.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);
        //    var capacitySum = db.EventureLists.Where(ol => queryOwnerEventures.Contains(ol.Id)).Sum(s => s.Capacity);

        //    var queryOwnersLists = db.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);
        //    var regcount = db.Registrations.Count(r => queryOwnersLists.Contains(r.EventureListId));

        //    rList.Add(new DtoCapacity(regcount, capacitySum));

        //    return rList.ToArray();
        //}

        //public IEnumerable<DtoCapacity> GetCapacityByEventureId(int id)
        //{
        //    var rList = new List<DtoCapacity>();


        //    var capacitySum = db.EventureLists.Where(ol => ol.EventureId == id).Sum(s => (int?)s.Capacity) ?? 0;
        //                                                                    //.Sum(income =>  (decimal?)income.Amount) ?? 0;

        //    var queryOwnerEventures = db.Eventures.Where(e => e.Id == id).Select(e => e.Id);
        //    var queryOwnersLists = db.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);
        //    var regcount = db.Registrations.Count(r => queryOwnersLists.Contains(r.EventureListId));

        //    //rList.Add(new DtoCapacity(regcount, capacitySum));
        //    rList.Add(new DtoCapacity(232, 666));

        //    return rList.ToArray();
        //}

        //public IEnumerable<DtoCapacity> GetCapacityByEventureListId(int id)
        //{
        //    var rList = new List<DtoCapacity>();


        //    var capacitySum = db.EventureLists.Where(ol => ol.Id == id).Sum(s => s.Capacity);

        //    //var queryOwnerEventures = db.Eventures.Where(e => e.Id == id).Select(e => e.Id);
        //    //var queryOwnersLists = db.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);
        //    var regcount = db.Registrations.Count(r => r.EventureListId == id);

        //    rList.Add(new DtoCapacity(regcount, capacitySum));

        //    return rList.ToArray();
        //}


        //public IEnumerable<Registration> GetRegistrationsPerMonthByEventureId(int eventureId)       //(int EventureId, int MonthOverNumber)
        //{
        //    //using(EntitiyName db = new EntityName)
        //    //{
        //    //var sum = from p in db.Registrations

        //    var query = from r in db.Registrations
        //                group r by r.Date.Month
        //                into dateGroup
        //                select new {x = dateGroup.Key, x1 = dateGroup.Count()};


        //    //join j in db.ProductStock on ProductStock.ProductId equals Products.Id
        //    //GROUP BY WellKnownAttributes.Name
        //    //select new{ WellKnownAttributes.Name, SUM(StockCount) as total }
        //    //}

        //    //var queryOwnerEventures = db.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);
        //    //var queryOwnersLists = db.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);
        //    //return db.Registrations.Where(r => queryOwnersLists.Contains(r.ParticipantId)); //.Where(r => queryOwnersLists.Contains(r.EventureList));

        //}
        //public Registration GetRegistrationByPartID(int PartId)
        //{
        //    return db.Registrations.FirstOrDefault(r => r.ParticipantId == PartId);
        //}

        //public virtual IEnumerable<Registration> GetWithRawSql(string query, params object[] parameters)
        //{
        //    //return dbSet.SqlQuery(query, parameters).ToList();
        //    //return System.Data.Entity.DbSet.SqlQuery(query, parameters).ToList();
        //    return db.Database.SqlQuery(query, parameters).ToArray;
        //}

    }
}
