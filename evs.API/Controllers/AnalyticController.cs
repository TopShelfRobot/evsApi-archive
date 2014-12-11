using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace evs.API.Controllers
{
    [RoutePrefix("api/analytic")]
    public class AnalyticController : ApiController
    {

        [HttpGet]
        public object GetYearOverYearData()
        {
            ////var rList = new List<DtoCapacity>();
            //var capacity = new DtoCapacity(0, 0);

            //var capacitySum = _contextProvider.Context.EventureLists.Where(ol => ol.EventureId == id).Sum(s => (int?)s.Capacity) ?? 0;
            ////.Sum(income =>  (decimal?)income.Amount) ?? 0;

            //var queryOwnerEventures = _contextProvider.Context.Eventures.Where(e => e.Id == id).Select(e => e.Id);
            //var queryOwnersLists = _contextProvider.Context.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);
            //var regcount = _contextProvider.Context.Registrations.Count(r => queryOwnersLists.Contains(r.EventureListId) && r.EventureOrder.Status == "Complete");

            ////rList.Add(new DtoCapacity(regcount, capacitySum));
            ////rList.Add(new DtoCapacity(232, 666));

            //capacity.Regs = regcount;
            //capacity.Capacity = capacitySum;

            //return capacity;

            var data = new List<DtoYearOverYear>();
            data.Add(new DtoYearOverYear("Urban Bourbon", "November", 20, 10, 13));
            data.Add(new DtoYearOverYear("Urban Bourbon", "December", 17, 12, 34));
            data.Add(new DtoYearOverYear("Urban Bourbon", "January", 14, 17, 22));
            data.Add(new DtoYearOverYear("Urban Bourbon", "February", 22, 18, 17));



            return data;

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

        public class DtoYearOverYear    //moved
        {


            public string Eventure { get; set; }
            public string Month { get; set; }
            public Int32 Year { get; set; }
            public Int32 Yeartwo { get; set; }
            public Int32 Yearthree { get; set; }

            public DtoYearOverYear(string eventure, string month, Int32 year, Int32 yeartwo, Int32 yearthree)
            {

                Eventure = eventure;
                Month = month;
                Year = year;
                Yeartwo = yeartwo;
                Yearthree = yearthree;
            }
        }

    }


}
