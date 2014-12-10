using evs.DAL;
using evs.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace evs.API.Controllers
{
    //[Authorize]   
    [RoutePrefix("api/widget")]
    [Route("{action}/{id}")]
    public class WidgetController : ApiController
    {
        readonly evsContext db = new evsContext();

        [HttpGet]
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

        [HttpGet]
        public object GetEventureGraph(Int32 id)
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



        [HttpGet]
        public IEnumerable<Eventure> GetAllEventuresByOwnerId(Int32 id)
        {
            return db.Eventures.Where(e => e.OwnerId == id).OrderByDescending(e => e.Id);
        }

        [HttpGet]
        public IEnumerable<Participant> GetParticipantsByOwnerId(Int32 id) //Int32 ownerId
        {
            return db.Participants.Where(p => p.OwnerId == id).OrderByDescending(p => p.Id);
        }

        [HttpGet]
        public object GetTeamRegistrationsByOwnerId(Int32 id)
        {
            return db.Teams.Where(t => t.Registration.EventureOrder.OwnerId == id
                                    && t.Registration.EventureOrder.Status == "Complete")
                .Select(t => new
                {
                    t.Name,
                    t.Id,
                    ListName = t.Registration.EventureList.DisplayName,
                    CoachName = t.Coach.FirstName + " " + t.Coach.LastName,
                    Amount = (decimal?)t.TeamMemberPayments.Sum(p => p.Amount) ?? 0,
                    Balance = t.Registration.ListAmount - ((decimal?)t.TeamMemberPayments.Sum(p => p.Amount) ?? 0),
                    t.Registration.EventureList.EventureListType,
                    EventName = t.Registration.EventureList.Eventure.Name
                })
                .ToList();
        }

        [HttpGet]
        public object GetVolunteersByOwnerId(Int32 id)
        {
            return db.Volunteers.Where(v => v.Participant.OwnerId == id)
                .Select(v => new { v.Participant.FirstName, v.Participant.LastName, v.Participant.Email, v.Participant.PhoneMobile, v.Id })
                .ToList();

        }

        [HttpGet]
        public IEnumerable<Resource> GetResourcesByOwnerId(Int32 id)
        {
            //return db.Participants.Where(p => p.OwnerId == id);
            return db.Resources.Where(v => v.OwnerId == id);
        }

        [HttpGet]
        public IEnumerable<Coupon> GetCouponsByOwnerId(int id)
        {
            return db.Coupons.Where(c => c.OwnerId == id).OrderByDescending(c => c.Id);
        }

        [HttpGet]
        public object GetNotificationsByOwnerId(Int32 id)
        {
            //return db.Participants.Where(p => p.OwnerId == id);
            var resource = from p in db.PlanItems
                           join r in db.Resources
                              on p.ResourceId equals r.Id
                           join e in db.Eventures
                              on p.EventureId equals e.Id
                           where e.OwnerId == id
                           orderby p.DateDue
                           select new { Task = p.Task, DateDue = p.DateDue, Resource = r.Name, Eventure = e.DisplayHeading };

            return resource.ToList();
        }

        [HttpGet]
        public IEnumerable<EventureList> GetEventureListsByEventureId(Int32 id)
        {
            return db.EventureLists.Where(e => e.EventureId == id);
        }

        [HttpGet]
        public IEnumerable<EventureService> GetEventureServiceByEventureId(Int32 id)
        {
            //lazy #2  only should be querying by owner id
            return db.EventureServices.Where(s => s.EventureId == id);
        }

        [HttpGet]
        public object GetExpensesByEventureId(Int32 id)
        {
           return db.Expenses.Where(e => e.EventureId == id)
                    .Select(e => new { e.Id, e.Cost, e.CostType, e.PerRegNumber, item = e.ResourceItem.Name, category = e.ResourceItemCategory.Name });   //category = e.ItemCategory.Name
        }

        [HttpGet]
        public IEnumerable<Participant> GetRegisteredParticipantsByEventureId(Int32 id)
        {
            //var queryEventureListIdsByEventureId = db.EventureLists.Where(l => l.EventureId == id).Select(l => l.Id);
            //var queryRegPartIdsByEventureList =db.Registrations.Where(r => queryEventureListIdsByEventureId.Contains(r.EventureListId)).Select(r => r.ParticipantId);

            var queryRegPartIdsByEventureList = from r in db.Registrations
                                                join o in db.Orders
                                                on r.EventureOrderId equals o.Id
                                                join l in db.EventureLists
                                                on r.EventureListId equals l.Id
                                                where l.EventureId == id
                                                && o.Status == "Complete"
                                                select r.ParticipantId;

            return db.Participants.Where(p => queryRegPartIdsByEventureList.Contains(p.Id)).OrderByDescending(p => p.Id);
        }

        [HttpGet]
        public object GetNotificationsByEventureId(Int32 id)
        {
            var resource = from p in db.PlanItems
                           join r in db.Resources
                           on p.ResourceId equals r.Id
                           //join e in db.Eventures
                           //on p.EventureId equals e.Id
                           where p.EventureId == id
                           orderby p.DateDue
                           select new { Id = p.Id, Task = p.Task, DateDue = p.DateDue, Resource = r.Name, IsCompleted = p.IsCompleted };

            return resource.ToList();
        }

        [HttpGet]
        public object GetVolunteerDataByEventureId(Int32 id)      //mjb this needs complete
        {
            string query =
                "select j.id, j.name, (select count(*) from [VolunteerShift] where volunteerjobid = j.id) as shifts, " +
                "isnull((select sum(capacity) from [VolunteerShift] where volunteerjobid = j.id), 0) as maxcapacity,  " +
                "(select count(*) from [VolunteerSchedule] inner join [VolunteerShift] on [VolunteerSchedule].volunteershiftid = [VolunteerShift].id where [VolunteerShift].volunteerjobid = j.id) as capacity " +
                "from [VolunteerJob] j where j.eventureId =  " + id;

            return db.Database.SqlQuery<DtoVolunteerData>(query).ToList();
        }

        public class DtoVolunteerData
        {
            public Int32 Id { get; set; }
            public String Name { get; set; }
            public Int32 Shifts { get; set; }
            public Int32 Capacity { get; set; }
            public Int32 MaxCapacity { get; set; }
        }

        public class DtoEventuresByYear
        {
            public Int32 text { get; set; }
            public List<EventPartial> items = new List<EventPartial>();

            public DtoEventuresByYear(Int32 year, List<EventPartial> eventures)
            {
                text = year;
                items = eventures;
            }
        }

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

        public class EventPartial
        {
            public Int32 Id { get; set; }
            public string text { get; set; }

            public EventPartial(Int32 id, string name)
            {
                Id = id;
                text = name;
            }

        }
    }
}
