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

            var results = (from l in db.Registrations.Where(r => queryValidLists.Contains(r.EventureListId) && r.EventureList.DateEventureList.Year >= startYear && r.EventureList.DateEventureList.Year <= endYear)
                           // here I choose each field I want to group by
                           group l by new { l.DateCreated.Year, l.DateCreated.Month } into g
                           select new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() }
                            ).ToList().OrderBy(l => l.Month);

           var data = new List<DtoYearOverYear>();

           var stupid = true;

            foreach (var r in results)
            {
                //r.Month = r.Month + "/1/" + r.Year.ToString();
                
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

            var x = new List<genderPie>();
            x.Add(new genderPie("male", 55));
            x.Add(new genderPie("female", 45));

            //EnvironmentVariableTarget
            //var obj = new Lad
            //{
            //    firstName = "Markoff",
            //    lastName = "Chaney",

            //};
            //var json = new JavaScriptSerializer().Serialize(x);
            //Console.WriteLine(json);
            return x;

        }

        //[HttpGet]
        //[Route("GetAgePieChartByEventureId/{ownerId}/{eventureId}")]     //5 year increments
        //public object GetAgePieChartByEventureId(Int32 ownerId, Int32 eventureId)
        //{

        //}

        //[HttpGet]
        //[Route("GetZipCodeBarChartByEventureId/{ownerId}/{eventureId}")]    //top 10 zips
        //public object GetZipCodeBarChartByEventureId(Int32 ownerId, Int32 eventureId)
        //{

        //}

        //[HttpGet]
        //[Route("GetStateColumnChartByEventureId/{ownerId}/{eventureId}")]
        //public object GetStateColumnChartByEventureId(Int32 ownerId, Int32 eventureId)
        //{

        //}
        public class genderPie
        {
            public genderPie(string gender, int share)
            {
                Gender = gender;
                Share = share;
            }

            public string Gender;
            public int Share;
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
