using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using evs.DAL;

namespace evs.API.Controllers
{
    [RoutePrefix("api/analytic")]

    public class AnalyticController : ApiController
    {
        readonly evsContext db = new evsContext();

        [HttpGet]
        [Route("GetYearOverYearData/{ownerId}/{eventureId}/{startYear}/{endYear}/{valueTypeId}")]   //   owner
        public object GetYearOverYearData(Int32 ownerId, Int32 eventureId, Int32 startYear, Int32 endYear, Int32 valueTypeId)
        {
            //still have the issues of dates with no values;  we need to add a zero record
            //need to implement revenue

            //valueTypeId
            //0 - reg count
            //1 - revenue

            var queryValidLists = db.EventureLists.Where(l => l.Eventure.OwnerId == ownerId && l.EventureId == eventureId).Select(l => l.Id);

            var results = (from l in db.Registrations.Where(r => queryValidLists.Contains(r.EventureListId) 
                        && r.EventureList.DateEventureList.Year >= startYear && r.EventureList.DateEventureList.Year <= endYear)
                           group l by new { l.DateCreated.Year, l.DateCreated.Month } into g
                           select new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() }
                            ).ToList().OrderBy(l => l.Month);

           var data = new List<DtoYearOverYear>();

           var stupid = true;   //nonsense for testing purposes

            foreach (var r in results)
            {
               if (valueTypeId==0)
                {
                    if (stupid)
                        data.Add(new DtoYearOverYear("2012", r.Month.ToString() + "/1/2012" , 37));
                    stupid = !stupid;
                }
                data.Add(new DtoYearOverYear(r.Year.ToString(), r.Month.ToString() + "/1/" + r.Year.ToString(), r.Count));
            
            }
            return data;
        }


        [HttpGet]
        [Route("GetGenderPieChartByEventureId/{ownerId}/{eventureId}")]
        public object GetGenderPieChartByEventureId(Int32 ownerId, Int32 eventureId)
        {
            var agePieChart = new List<dtoKeyValue>();
            agePieChart.Add(new dtoKeyValue("Male", 53.4));
            agePieChart.Add(new dtoKeyValue("Female", 46.6));

            return agePieChart;
        }

        [HttpGet]
        [Route("GetAgePieChartByEventureId/{ownerId}/{eventureId}")]     //5 year increments
        public object GetAgePieChartByEventureId(Int32 ownerId, Int32 eventureId)
        {

            //var queryRegPartIdsByEventureList = from r in db.Registrations
            //                                    join o in db.Orders
            //                                    on r.EventureOrderId equals o.Id
            //                                    join l in db.EventureLists
            //                                    on r.EventureListId equals l.Id
            //                                    where l.EventureId == id
            //                                    && o.Status == "Complete"
            //                                    select r.ParticipantId;

            //var results = (from l in db.Registrations.Where(r => queryValidLists.Contains(r.EventureListId)
            //            && r.EventureList.DateEventureList.Year >= startYear && r.EventureList.DateEventureList.Year <= endYear)
            //               group l by new { l.DateCreated.Year, l.DateCreated.Month } into g
            //               select new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() }
            //                ).ToList().OrderBy(l => l.Month);


            //return db.Participants.Where(p => queryRegPartIdsByEventureList.Contains(p.Id));

            var agePieChart = new List<dtoKeyValue>();
            agePieChart.Add(new dtoKeyValue("under 20", 5.9));
            agePieChart.Add(new dtoKeyValue("21-25", 10.1));
            agePieChart.Add(new dtoKeyValue("26-30", 17.3));
            agePieChart.Add(new dtoKeyValue("31-35", 14.1));;
            agePieChart.Add(new dtoKeyValue("36-40", 12.7));
            agePieChart.Add(new dtoKeyValue("41-45", 14.1));
            agePieChart.Add(new dtoKeyValue("46-50", 12.7));
            agePieChart.Add(new dtoKeyValue("51-55", 5.1));
            agePieChart.Add(new dtoKeyValue("56-60", 4.7));
            agePieChart.Add(new dtoKeyValue("over 60", 3.3));

            return agePieChart;
           
        }

        [HttpGet]
        [Route("GetZipCodeBarChartByEventureId/{ownerId}/{eventureId}")]    //top 10 zips
        public object GetZipCodeBarChartByEventureId(Int32 ownerId, Int32 eventureId)
        {

         var zipCodeBarChart = new List<dtoKeyValue>();
         zipCodeBarChart.Add(new dtoKeyValue("40205", 25.9));
         zipCodeBarChart.Add(new dtoKeyValue("40299", 14.1));
         zipCodeBarChart.Add(new dtoKeyValue("40244", 27.3));
         zipCodeBarChart.Add(new dtoKeyValue("40242", 32.7));

         return zipCodeBarChart;
        }


        [HttpGet]
        [Route("GetStateColumnChartByEventureId/{ownerId}/{eventureId}")]
        public object GetStateColumnChartByEventureId(Int32 ownerId, Int32 eventureId)
        {
            var stateBarChart = new List<dtoKeyValue>();
            stateBarChart.Add(new dtoKeyValue("Kentucky", 365));
            stateBarChart.Add(new dtoKeyValue("Indiana", 118));
            stateBarChart.Add(new dtoKeyValue("Ohio", 9));
            stateBarChart.Add(new dtoKeyValue("Tennessee", 32));
            stateBarChart.Add(new dtoKeyValue("Michigan", 14));
            stateBarChart.Add(new dtoKeyValue("Illinois", 27));
            stateBarChart.Add(new dtoKeyValue("West Virginia", 14));
            stateBarChart.Add(new dtoKeyValue("Other", 27));



            return stateBarChart;

        }


        public class dtoKeyValue
        {
            public dtoKeyValue(string key, double value)
            {
                Key = key;
                Value = value;
            }

            public string Key;
            public double Value;
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
            public string Month { get; set; }         //category
            public string Year { get; set; }           //group by 
            public Int32 Registrations { get; set; }  //actual value


            public DtoYearOverYear(string year, string month, Int32 registrations)
            {
                Month = month;
                Year = year;
                Registrations = registrations;
            }
        }

    }


}
