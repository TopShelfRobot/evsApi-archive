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
        [Route("GetYearOverYearData/{ownerId}/{eventureId}/{startYear}/{endYear}/{valueTypeId}")]  
        public object GetYearOverYearData(Int32 ownerId, Int32 eventureId, Int32 startYear, Int32 endYear, Int32 valueTypeId)
        {
            //still have the issues of dates with no values;  we need to add a zero record
            //need to implement revenue

            //valueTypeId
            //0 - reg count
            //1 - revenue

            //var queryValidLists = db.EventureLists.Where(l => l.Eventure.OwnerId == ownerId && l.EventureId == eventureId).Select(l => l.Id);

            //var results = (from l in db.Registrations.Where(r => queryValidLists.Contains(r.EventureListId)
            //            && r.EventureList.DateEventureList.Year >= startYear && r.EventureList.DateEventureList.Year <= endYear)
            //               group l by new { l.DateCreated.Year, l.DateCreated.Month } into g
            //               select new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() }
            //                ).ToList().OrderBy(l => l.Month);

            string query = "select  month(r.dateCreated) as 'Month', yeAR(r.DateCreated) as 'Year', Count(*) as 'Registrations' from Registration r inner join eventureOrder o on r.EventureOrderId = o.Id " +
                            "where r.EventureListId in (select Id from EventureList where EventureId in ( 4,9) ) " + 
                            "group by month(r.dateCreated), yeAR(r.DateCreated) order by month(r.dateCreated) ";

            var results =   db.Database.SqlQuery<DtoYearOverYear>(query).ToList();

            //results.Contains()



            //var data = new List<DtoYearOverYear>();





            for (int i = 1; i <= 12; i++)
            {
                var newYear = results.Find(x => x.Year.Equals(2015) && x.Month.Equals(i));
                if (newYear == null)
                    results.Add(new DtoYearOverYear() { Month = i, Year = 2015, Registrations = 0 });

                var oldYear = results.Find(x => x.Year.Equals(2014) && x.Month.Equals(i));
                if (oldYear == null)
                    results.Add(new DtoYearOverYear() { Month = i, Year = 2014, Registrations = 0 });
            }

            //var stupid = true;   //nonsense for testing purposes

            //foreach (var r in results)
            //{
            //    if (valueTypeId == 0)
            //    {
            //        if (stupid)
            //            data.Add(new DtoYearOverYear("2012", r.Month.ToString() + "/1/2012", 37));
            //        stupid = !stupid;
            //    }
            //    data.Add(new DtoYearOverYear(r.Year.ToString(), r.Month.ToString() + "/1/" + r.Year.ToString(), r.Count));

            //}

           //for (x )


            //return data;

            //parts.Find(x => x.PartName.Contains("seat"))
           

            return results;
        }
        
        [HttpGet]
        [Route("GetGenderPieChartByEventureId/{ownerId}/{eventureId}")]
        public object GetGenderPieChartByEventureId(Int32 ownerId, Int32 eventureId)
        {
            string query = "select p.Gender as [key], count(*) as value " +
                             "from EventureOrder o inner join Registration r on o.Id = r.EventureOrderId inner join Participant p " +
                             "on r.ParticipantId = p.Id inner join EventureList l on r.EventureListId = l.Id where o.status = 'Complete' " +
                             " and r.EventureListId in (select Id from EventureList where EventureId = " + eventureId.ToString() + ")" +
                             "group by gender ";

            return db.Database.SqlQuery<DtoGenericKeyValue>(query).ToList();
        }

        [HttpGet]
        [Route("GetAgePieChartByEventureId/{ownerId}/{eventureId}")]     //5 year increments
        public object GetAgePieChartByEventureId(Int32 ownerId, Int32 eventureId)
        {
            string query = " SELECT CASE " +
                               "WHEN CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) < 18 THEN '[Under 18]' " +
                               "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) BETWEEN 18 AND 24 THEN '[18-24]' " +
                               "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) BETWEEN 25 AND 34 THEN '[25-34]' " +
                               "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) BETWEEN 35 AND 44 THEN '[35-44]' " +
                               "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) BETWEEN 45 AND 54 THEN '[45-54]' " +
                               "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) BETWEEN 55 AND 64 THEN '[55-64]' " +
                               "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) > 64 THEN '[55 and Up]' " +
                           "End as [Key], " +
                           "Count(*) Value " +
                           "from EventureOrder o inner join Registration r on o.Id = r.EventureOrderId inner join Participant p " +
                           "on r.ParticipantId = p.Id inner join EventureList l on r.EventureListId = l.Id where status = 'Complete' " +
                            "and l.eventureId =  " + eventureId.ToString() +
                            "group by CASE  " +
                                    "WHEN CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) < 18 THEN '[Under 18]' " +
                                    "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) BETWEEN 18 AND 24 THEN '[18-24]' " +
                                    "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) BETWEEN 25 AND 34 THEN '[25-34]' " +
                                    "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) BETWEEN 35 AND 44 THEN '[35-44]' " +
                                    "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) BETWEEN 45 AND 54 THEN '[45-54]' " +
                                    "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) BETWEEN 55 AND 64 THEN '[55-64]' " +
                                    "WHEN  CONVERT(int,DATEDIFF(hour,p.DateBirth,GETDATE())/8766.0,0) > 64 THEN '[55 and Up]' " +
                                "End ";

            return db.Database.SqlQuery<DtoGenericKeyValue>(query).ToList();
        }

        [HttpGet]
        [Route("GetLapsedRunnersByEventureId/{currentEventureId}/{previousEventId}")]
        public object GetLapsedRunnersByEventureId(Int32 currentEventureId, Int32 previousEventId)
        {
            //TODO:  remove sports comm custom
            switch (currentEventureId)
            {
                case 9:
                    previousEventId = 4;
                    break;
                case 5:
                    previousEventId = 1;
                    break;
                case 6:
                    previousEventId = 2;
                    break;
                case 7:
                    previousEventId = 3;
                    break;
                default:
                    previousEventId = currentEventureId;  //this will return empty grid
                    break;
            }
            //TODO:  remove sports comm custom

            string query = " select distinct Email, FirstName, LastName  " +
                            " from Registration r0 " +
                            " inner join Participant p0 " +
                            " on r0.ParticipantId = p0.Id " +
                            " inner join EventureList l0 " +
                            " on r0.EventureListId = l0.Id " +
                            " where l0.EventureId  = " + currentEventureId +
                            " and not exists(select 1 from  Registration r " +
                                            " inner join participant p " +
                                            " on r.ParticipantId = p.Id " +
                                            " inner join EventureList l " +
                                            " on r.EventureListId = l.Id " +
                                            " where l.EventureId =  " + previousEventId +
                                            " and p.email = p0.email " +
                                            " and p.FirstName = p0.firstname " +
                                            " and p.LastName = p0.lastname ) " +
                            " order by email ";

            return db.Database.SqlQuery<DtoLapsedRunner>(query).ToList();
        }

        [HttpGet]
        [Route("GetZipCodeBarChartByEventureId/{ownerId}/{eventureId}")]    //top 10 zips
        public object GetZipCodeBarChartByEventureId(Int32 ownerId, Int32 eventureId)
        {
            string query = "select top 20 p.zip as [key], count(*) as value " +
                            "from EventureOrder o inner join Registration r on o.Id = r.EventureOrderId inner join Participant p " +
                            "on r.ParticipantId = p.Id inner join EventureList l on r.EventureListId = l.Id where o.status = 'Complete' " +
                            " and r.EventureListId in (select Id from EventureList where EventureId = " + eventureId.ToString() + ")" +
                            "group by  p.zip order by count(*) desc ";

            return db.Database.SqlQuery<DtoGenericKeyValue>(query).ToList();
        }


        [HttpGet]
        [Route("GetStateColumnChartByEventureId/{ownerId}/{eventureId}")]
        public object GetStateColumnChartByEventureId(Int32 ownerId, Int32 eventureId)
        {
            string query = "select top 20 p.state as [key], count(*) as value " +
                           "from EventureOrder o inner join Registration r on o.Id = r.EventureOrderId inner join Participant p " +
                           "on r.ParticipantId = p.Id inner join EventureList l on r.EventureListId = l.Id where o.status = 'Complete' " +
                           " and r.EventureListId in (select Id from EventureList where EventureId = " + eventureId.ToString() + ")" +
                           "group by  p.state order by count(*) desc ";

            return db.Database.SqlQuery<DtoGenericKeyValue>(query).ToList();

        }

        

        //

        [HttpGet]
        [Route("GetCouponTotalsByEventureId/{eventureId}")]
        public object GetCouponTotalsByEventureId(Int32 eventureId)
        {
            string query = " select sum(s.amount) as 'Amount', count(*) as 'Count' " +
                           " from surcharge s " +
                           " inner join EventureOrder o " +
                           " on s.EventureOrderId = o.Id " +
                           " where ChargeType = 'coupon' " +
                           " and o.status ='Complete' " +
                           " and EventureListId in (select ID from eventurelist where eventureId = "+ eventureId.ToString() + ")" ;


            return db.Database.SqlQuery<DtoCouponTotals>(query).ToList();
        }

        public class DtoCouponTotals
        {
            public Decimal Amount { get; set; }
            public Int32 Count { get; set; }
        }

        [HttpGet]
        [Route("GetCouponGroupingsByEventureId/{eventureId}")]
        public object GetCouponGroupingsByEventureId(Int32 eventureId)
        {
            string query = " select c.Code,  sum(s.amount) as 'Amount', count(*) as 'Count' " +
                           " from surcharge s " +
                           " inner join coupon c " +
                           " on s.CouponId = c.Id " +
                           " inner join EventureOrder o " +
                           " on s.EventureOrderId = o.Id " +
                           " where ChargeType = 'coupon' " +
                           " and o.status ='Complete' " +
                           " and EventureListId in (select ID from eventurelist where eventureId = " + eventureId.ToString() + ")" +
                            " group by c.Code ";

            return db.Database.SqlQuery<DtoCouponGroups>(query).ToList();

        }

        public class DtoCouponGroups
        {
            public String Code { get; set; }
            public Decimal Amount { get; set; }
            public Int32 Count { get; set; }
        }

        [HttpGet]
        [Route("GetCouponsByEventureId/{eventureId}")]
        public object GetCouponsByEventureId( Int32 eventureId)
        {
            string query = " select p.Email,  p.FirstName, p.LastName, c.Code, o.Id as 'OrderId' ,e.Name as 'ListName', s.Amount as 'CouponAmount', s.DateCreated as 'DateCouponRedeemed'" +
                " from surcharge s " +
                " inner join coupon c " +
                " on s.CouponId = c.Id " +
                " inner join EventureOrder o " +
                " on s.EventureOrderId = o.Id " +
                " left join Participant p " +
                " on  o.HouseId = p.Id " +
                " left join Eventure e " +
                " on s.EventureListId = e.Id " +   //weird join bad design mb071315
                " and c.CouponType = 'event' " +
                " where ChargeType = 'coupon' " +
                " and o.status ='Complete' " +
                " and e.id = " + eventureId.ToString() +
                " order by s.DateCreated";

            return db.Database.SqlQuery<DtoCouponInfo>(query).ToList();

        }
        //
        public class DtoCouponInfo
        {
            public String Email { get; set; }
            public String FirstName { get; set; }
            public String LastName { get; set; }
            public String Code { get; set; }
            public Int32 OrderId { get; set; }
            public String ListName { get; set; }
            public Decimal CouponAmount { get; set; }
            public DateTime DateCouponRedeemed { get; set; }
            
        }


        public class DtoGenericKeyValue
        {
            public String Key { get; set; }
            public Int32 Value { get; set; }
        }

        public class DtoLapsedRunner
        {
            public String Email { get; set; }
            public String FirstName { get; set; }
            public String LastName { get; set; }
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
            public Int32 Month { get; set; }         //category
            public Int32 Year { get; set; }           //group by 
            public Int32 Registrations { get; set; }  //actual value


            //public DtoYearOverYear(string year, string month, Int32 registrations)
            //{
            //    Month = month;
            //    Year = year;
            //    Registrations = registrations;
            //}
        }

    }


}
