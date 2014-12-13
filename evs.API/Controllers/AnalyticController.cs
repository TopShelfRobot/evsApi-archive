using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace evs.API.Controllers
{
    [RoutePrefix("api/analytic")]
    
    public class AnalyticController : ApiController
    {

        [HttpGet]
        [Route("GetYearOverYearData/{ownerId}/{eventureId}/{year1}/{year2}")]   //   owner
        public object GetYearOverYearData(Int32 ownerId, Int32 eventureId, string year1, string year2)
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


            data.Add(new DtoYearOverYear("2014", Convert.ToDateTime("1/1/2014"), 20));
            data.Add(new DtoYearOverYear("2013", Convert.ToDateTime("1/1/2013"), 16));
            data.Add(new DtoYearOverYear("2012", Convert.ToDateTime("1/1/2012"), 0));


            data.Add(new DtoYearOverYear("2014", Convert.ToDateTime("2/1/2014"), 14));
            data.Add(new DtoYearOverYear("2013", Convert.ToDateTime("2/1/2013"), 22));
            data.Add(new DtoYearOverYear("2012", Convert.ToDateTime("2/1/2012"), 0));

            data.Add(new DtoYearOverYear("2014", Convert.ToDateTime("3/1/2014"), 17));
            data.Add(new DtoYearOverYear("2013", Convert.ToDateTime("3/1/2013"), 20));
            data.Add(new DtoYearOverYear("2012", Convert.ToDateTime("3/1/2012"), 0));

            data.Add(new DtoYearOverYear("2014", Convert.ToDateTime("4/1/2014"), 17));
            data.Add(new DtoYearOverYear("2013", Convert.ToDateTime("4/1/2013"), 20));
            data.Add(new DtoYearOverYear("2012", Convert.ToDateTime("4/1/2012"), 66));

            data.Add(new DtoYearOverYear("2014", Convert.ToDateTime("5/1/2014"), 57));
            data.Add(new DtoYearOverYear("2013", Convert.ToDateTime("5/1/2013"), 40));
            data.Add(new DtoYearOverYear("2012", Convert.ToDateTime("5/1/2012"), 56));

            data.Add(new DtoYearOverYear("2014", Convert.ToDateTime("6/1/2014"), 66));
            data.Add(new DtoYearOverYear("2013", Convert.ToDateTime("6/1/2013"), 33));
            data.Add(new DtoYearOverYear("2012", Convert.ToDateTime("6/1/2012"), 44));

            data.Add(new DtoYearOverYear("2014", Convert.ToDateTime("7/1/2014"), 77));
            data.Add(new DtoYearOverYear("2013", Convert.ToDateTime("7/1/2013"), 99));
            data.Add(new DtoYearOverYear("2012", Convert.ToDateTime("7/1/2012"), 66));




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
            public genderPie (string gender, int share)
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
            public DateTime Month { get; set; }         //category
            public string Year { get; set; }           //group by 
            public Int32 Registrations { get; set; }  //actual value


            public DtoYearOverYear(string year, DateTime month, Int32 registrations)
            {
                Month = month;
                Year = year;
                Registrations = registrations;               
            }
        }

    }


}
