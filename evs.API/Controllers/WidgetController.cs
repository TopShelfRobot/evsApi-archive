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
        public IEnumerable<Eventure> GetAllEventuresByOwnerId(int id)
        {
            return db.Eventures.Where(e => e.OwnerId == id).OrderByDescending(e => e.Id);
        }

        [HttpGet]
        public IEnumerable<Participant> GetParticipantsByOwnerId(int id) //Int32 ownerId
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
        public object GetVolunteersByOwnerId(int id)
        {
            return db.Volunteers.Where(v => v.Participant.OwnerId == id)
                .Select(v => new { v.Participant.FirstName, v.Participant.LastName, v.Participant.Email, v.Participant.PhoneMobile, v.Id })
                .ToList();

        }

        [HttpGet]
        public IEnumerable<Resource> GetResourcesByOwnerId(int id)
        {
            //return db.Participants.Where(p => p.OwnerId == id);
            return db.Resources.Where(v => v.OwnerId == id);
        }

        [HttpGet]
        public IEnumerable<Coupon> GetCoupons()
        {
            return db.Coupons.ToArray();
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
