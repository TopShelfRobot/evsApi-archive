using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Newtonsoft.Json.Linq;
using evs.DAL;
using evs.Model;
//using System.Web.Http.Cors;

namespace evs30Api.Controllers
{
    //[EnableCors(origins: "http://imperclient.azurewebsites.net", headers: "*", methods: "*")]
    [BreezeController]
    public class BreezeController : ApiController
    {
        readonly EFContextProvider<evsContext> _contextProvider = new EFContextProvider<evsContext>();

        public class DtoCoupon
        {
            public decimal Amount { get; set; }
            public string Message { get; set; }
            public Int32 CouponId { get; set; }
            public Boolean IsOwner { get; set; }

            public DtoCoupon(decimal amount, string message, Int32 couponId, Boolean isowner)
            {
                Amount = amount;
                Message = message;
                CouponId = couponId;
                IsOwner = isowner;
            }
        }

        public class DtoTransfer
        {
            public string FromName { get; set; }
            public decimal FromAmount { get; set; }
            public string ToName { get; set; }
            public decimal ToAmount { get; set; }
            public string Message { get; set; }

            public DtoTransfer(string fromName, decimal fromAmount, string toName, decimal toAmount, string message)
            {
                FromName = fromName;
                FromAmount = fromAmount;
                ToName = toName;
                ToAmount = toAmount;
                Message = message;
            }
        }

        public class DtoRevReg
        {
            public Int32? Regs { get; set; }
            public decimal? Rev { get; set; }

            public DtoRevReg(Int32 regs, decimal rev)
            {
                Regs = regs;
                Rev = rev;
            }
        }

        public class DtoAccess
        {
            public Int32 OwnerId { get; set; }
            public String AccessType { get; set; }
            public Int32 HouseId { get; set; }

            public DtoAccess(Int32 ownerId, string accessType, Int32 houseId)
            {
                OwnerId = ownerId;
                AccessType = accessType;
                HouseId = houseId;
            }
        }

        public class DtoCapacity
        {
            public Int32 Regs { get; set; }
            public Int32 Capacity { get; set; }
            public DtoCapacity(Int32 regs, Int32 capacity)
            {

                Regs = regs;
                Capacity = capacity;
            }
        }

        public class DtoTrends
        {
            public Int32 Last30Count { get; set; }
            public Int32 Last7Count { get; set; }
            public Int32 Last1Count { get; set; }
            public Int32 TotalCount { get; set; }

            public decimal Last30Amount { get; set; }
            public decimal Last7Amount { get; set; }
            public decimal Last1Amount { get; set; }
            public decimal TotalAmount { get; set; }

            public DtoTrends(Int32 last30Count, Int32 last7Count, Int32 last1Count, Int32 totalCount, decimal? last30Amount, decimal? last7Amount, decimal? last1Amount, decimal? totalAmount)
            {
                Last30Count = last30Count;
                Last7Count = last7Count;
                Last1Count = last1Count;
                TotalCount = totalCount;
                Last30Amount = last30Amount ?? 0;
                Last7Amount = last7Amount ?? 0;
                Last1Amount = last1Amount ?? 0;
                TotalAmount = totalAmount ?? 0;

            }
        }

        [HttpGet]
        public DtoTrends GetTrendsByEventId(int id)
        {
            var queryLists = _contextProvider.Context
                                        .EventureLists
                                        .Where(el => el.EventureId == id)
                                        .Select(l => l.Id);

            var last7Date = DateTime.Now.AddDays(-7).Date;
            var reg7Count = _contextProvider.Context.Registrations.Where(r => queryLists.Contains(r.EventureListId)).Count(r => r.DateCreated > last7Date && r.EventureOrder.Status == "Complete");

            var last30Date = DateTime.Now.AddDays(-30).Date;
            var reg30Count = _contextProvider.Context.Registrations.Where(r => queryLists.Contains(r.EventureListId)).Count(r => r.DateCreated > last30Date && r.EventureOrder.Status == "Complete");

            var last1Date = DateTime.Now.AddDays(-1).Date;
            var reg1Count = _contextProvider.Context.Registrations.Where(r => queryLists.Contains(r.EventureListId)).Count(r => r.DateCreated > last1Date && r.EventureOrder.Status == "Complete");

            var totalCount = _contextProvider.Context.Registrations.Count(r => queryLists.Contains(r.EventureListId) && r.EventureOrder.Status == "Complete");


            var reg7Amount = _contextProvider.Context.Registrations.Where(r => queryLists.Contains(r.EventureListId) && r.DateCreated > last7Date && r.EventureOrder.Status == "Complete").Sum(r => (decimal?)r.ListAmount);
            var reg30Amount = _contextProvider.Context.Registrations.Where(r => queryLists.Contains(r.EventureListId) && r.DateCreated > last30Date && r.EventureOrder.Status == "Complete").Sum(r => (decimal?)r.ListAmount);
            var reg1Amount = _contextProvider.Context.Registrations.Where(r => queryLists.Contains(r.EventureListId) && r.DateCreated > last1Date && r.EventureOrder.Status == "Complete").Sum(r => (decimal?)r.ListAmount);
            var totalAmount = _contextProvider.Context.Registrations.Where(r => queryLists.Contains(r.EventureListId) && r.EventureOrder.Status == "Complete").Sum(r => (decimal?)r.ListAmount);


            return new DtoTrends(reg30Count, reg7Count, reg1Count, totalCount, reg30Amount, reg7Amount, reg1Amount, totalAmount);
        }

        [HttpGet]
        public object GetTeamMemberPaymentInfoByTeamMemberGuid(Guid id)
        {
            return _contextProvider.Context.TeamMembers
                     .Where(t => t.TeamMemberGuid == id && t.Active)
                     .Select(m => new
                     {
                         ListName = m.Team.Registration.EventureList.DisplayName,
                         m.Team.Registration.EventureList.ListingType,
                         m.Team.Registration.EventureList.CurrentFee,
                         RegAmount = m.Team.Registration.ListAmount,   //totalAmount??
                         userPaid = (decimal?)m.TeamMemberPayments.Sum(p => p.Amount) ?? 0,     //this is temp;  if they make multiple payments reciept will be wrong
                         m.Id,
                         m.Team.Name,
                         teamMemberId = m.Id,
                         teamId = m.Team.Id
                     }).ToList();
        }

        [HttpGet]
        public object GetNotPaidTeamMemberCountByTeamGuid(Guid id)
        {
            var allTeamCount = _contextProvider.Context.TeamMembers.Count(t => t.Team.TeamGuid == id && t.Active);

            var paidTeamCount = _contextProvider.Context.TeamMembers.Count(t => t.Team.TeamGuid == id && t.Active && t.TeamMemberPayments.Count > 0);

            return allTeamCount - paidTeamCount;
        }

        [HttpGet]
        public object GetTeamMemberPaymentSumByTeamGuid(Guid id)
        {
            return _contextProvider.Context.TeamMembers
                                .Where(t => t.Team.TeamGuid == id && t.Active)
                                .Sum(t => (decimal?)t.TeamMemberPayments.Sum(p => (decimal?)p.Amount) ?? 0);
        }

        [HttpGet]
        public DtoRevReg GetRegsRevByOwner(int id)
        {
            var dtoRegRev = new DtoRevReg(0, 0);

            var queryOwnerEventures = _contextProvider.Context.Eventures
                                        .Where(e => e.OwnerId == id)
                                        .Select(e => e.Id);

            var queryOwnersLists = _contextProvider.Context.EventureLists
                                        .Where(el => queryOwnerEventures.Contains(el.EventureId))
                                        .Select(l => l.Id);

            var regcount = _contextProvider.Context.Registrations
                .Where(r => queryOwnersLists.Contains(r.EventureListId) && r.EventureOrder.Status == "Complete")
                                .Sum(r => (int?)r.Quantity);

            var rev = _contextProvider.Context.Registrations
                .Where(r => queryOwnersLists.Contains(r.EventureListId) && r.EventureOrder.Status == "Complete")
                .Sum(r => (int?)r.TotalAmount);

            dtoRegRev.Regs = regcount;
            if (rev.Equals(null))
                dtoRegRev.Rev = 0;
            else
                dtoRegRev.Rev = rev;

            return dtoRegRev;
        }

        [HttpGet]
        public object GetRegEditDisplayInfo(int id)
        {
            var query = from r in _contextProvider.Context.Registrations
                        join el in _contextProvider.Context.EventureLists
                            on r.EventureListId equals el.Id
                        join q in _contextProvider.Context.StockQuestionSets
                            on el.Id equals q.EventureListId
                        //join g in _contextProvider.Context.EventureGroups
                        //    on r.GroupId equals g.Id
                        where r.Id == id
                        select
                            new
                                {
                                    el.DisplayName,
                                    el.DateEventureList,
                                    //g.Name, 
                                    q.ShirtSize,
                                    q.FinishTime,
                                    q.BibName,
                                    q.HowHear,
                                    //q.OwnRv,
                                    //q.NextRv,
                                    //q.HowHearRv,
                                    q.Notes,
                                    q.School,
                                    q.HowHearDropDown,
                                    q.EstimatedSwimTime400,
                                    q.EstimatedSwimTime,
                                    q.AnnualIncome,
                                    q.RelayTeamQuestion,
                                    q.Usat,
                                    q.ShirtUpgrade,
                                    q.Wheelchair,
                                    q.PuretapUnisex,
                                    q.NortonUnisex,
                                    q.BourbonGenderSpecific,
                                    q.HearRunathon,
                                    q.HearPure,
                                    q.HearNorton,
                                    q.HearBourbon,
                                    q.ParticipatePure,
                                    q.ParticipateNorton,
                                    q.ParticipateBourbon,
                                    q.Mile15,
                                    q.SportsEmails,
                                    q.BourbonWaiver
                                };

            return query.ToList();
        }

        [HttpGet]
        public object GetTransferInfo(int id)
        {
            var transfer = from t in _contextProvider.Context.EventureTransfers
                           join fl in _contextProvider.Context.EventureLists
                           on t.EventureListIdFrom equals fl.Id
                           join tl in _contextProvider.Context.EventureLists
                           on t.EventureListIdTo equals tl.Id
                           where t.Id == id
                           select new
                           {
                               originalList = fl.Name,
                               originalListCost = fl.CurrentFee,
                               newList = tl.Name,
                               newListCost = tl.CurrentFee,
                               regId = t.RegistrationId
                           };    //this should really look at reg for original cost
            return transfer.ToList();
        }

        [HttpGet]
        public DtoAccess GetOwnerInfo(string email, string ownerGuid)
        {
            //var rList = new List<DtoRevReg>();
            //var dtoAccess = new DtoAccess(-1, "none");

            //var queryOwnerEventures = _contextProvider.Context.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);
            //var queryOwnersLists = _contextProvider.Context.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);

            //var regcount = _contextProvider.Context.Registrations.Count(r => queryOwnersLists.Contains(r.EventureListId));
            //var regcount = _contextProvider.Context.Registrations.Where(r => queryOwnersLists.Contains(r.EventureListId)).Sum(r => r.Quantity);
            //var rev = _contextProvider.Context.Registrations.Sum(r => (int?)r.TotalAmount);

            //dtoAccess.OwnerId = 1;
            //dtoAccess.AccessType = "admin";



            var dtoAccess = new DtoAccess(1, "admin", 4896);

            //dtoRegRev.Regs = regcount;
            //if (rev.Equals(null))
            //    dtoRegRev.Rev = 0;
            //else
            //    dtoRegRev.Rev = rev;

            return dtoAccess;
        }

        [HttpGet]
        public DtoCoupon ValidateCoupon(string couponCode, Int32 participantId, Int32 eventureListId)
        {
            DateTime currentTime = DateTime.Now;
            Boolean isValidCoupon = false;
            Boolean isOwnerCoupon = false;
            var couponDto = new DtoCoupon(0, "", 0, false);

            // does coupon exist at all
            var coupons = _contextProvider.Context.Coupons
                .Where(c => c.DateStart < currentTime
                    && c.DateEnd > currentTime
                    && c.Active
                    && c.Code == couponCode);   //&& c.ownerId == ownerId

            if (coupons.Any())
            {
                isValidCoupon = true;
            }
            else
            {
                couponDto.Message = "valid coupon is not found";
                couponDto.Amount = 0;
            }

            //check type and verify valid for owner/event/list  
            if (isValidCoupon)
            {
                string couponType = coupons.FirstOrDefault().CouponType;

                switch (couponType)
                {
                    case "owner":

                        var listQuery = _contextProvider.Context.EventureLists.Where(l => l.Id == eventureListId).Select(l => l.EventureId);
                        var ownerId = _contextProvider.Context.Eventures.Where(e => listQuery.Contains(e.Id)).Select(e => e.OwnerId).SingleOrDefault();
                        if (ownerId == coupons.FirstOrDefault().CouponTypeLinkId)
                        {
                            isValidCoupon = true;
                            isOwnerCoupon = true;
                        }
                        else
                        {
                            couponDto.Message = "Coupon is invalid (O1)";
                            isValidCoupon = false;
                        }
                        break;

                    case "event":
                        //confirm that eventureid = couponlinktypeid
                        var eventureId =
                            _contextProvider.Context.EventureLists.Where(l => l.Id == eventureListId)
                                            .Select(l => l.EventureId).SingleOrDefault();
                        if (eventureId == coupons.FirstOrDefault().CouponTypeLinkId)
                            isValidCoupon = true;
                        else
                        {
                            couponDto.Message = "Coupon is invalid (E1)";
                            isValidCoupon = false;
                        }
                        break;

                    case "list":
                        //confirm that eventurelistid = couponlinktypeid
                        if (eventureListId != coupons.FirstOrDefault().CouponTypeLinkId)
                        {
                            couponDto.Message = "Coupon is invalid (L1)";
                            isValidCoupon = false;
                        }
                        break;

                    default:
                        couponDto.Message = "Coupon is invalid (D1)";
                        isValidCoupon = false;
                        break;
                }

            }

            //check usage  
            if (isValidCoupon)
            {
                if (coupons.FirstOrDefault().Usage == 1)
                {
                    //check if part has already redeemed coupon for this race

                }
                //else  //redundant
                //{
                //    isValidCoupon = true;
                //}
            }

            //check redeemed v. capacity
            if (isValidCoupon)
            {
                if (coupons.FirstOrDefault().Capacity <= coupons.FirstOrDefault().Redeemed)
                {
                    couponDto.Message = "Coupon is invalid (C1)";
                    isValidCoupon = false;
                }

                //else  //redundant
                //{
                //    isValidCoupon = true;
                //}
            }

            //check check owned
            if (isValidCoupon)
            {
                if (coupons.FirstOrDefault().IsOnlyForOwned)
                {
                    //checked if eventure is owned
                    var eid = _contextProvider.Context.EventureLists.Where(l => l.Id == eventureListId).Select(l => l.EventureId);
                    var isManaged = _contextProvider.Context.Eventures.Where(e => eid.Contains(e.Id)).Select(e => e.Managed).FirstOrDefault();
                    if (!isManaged)
                    {
                        couponDto.Message = "Coupon is invalid (O1)";
                        isValidCoupon = false;
                    }
                }
            }

            //check redeemed
            //isValidCoupon = true;
            if (isValidCoupon)
            {
                couponDto.Message = "valid";
                if (coupons.FirstOrDefault().DiscountType == 0)  //$ off
                {
                    couponDto.Amount = coupons.FirstOrDefault().Amount * -1;   //this could be off if there are two 
                    couponDto.CouponId = coupons.FirstOrDefault().Id;
                }
                else  //percent off
                {
                    decimal percent = coupons.FirstOrDefault().Amount * -1;
                    var listPrice =
                        _contextProvider.Context.EventureLists.Where(l => l.Id == eventureListId)
                                        .Select(l => l.CurrentFee).FirstOrDefault();
                    //var test = percent * listPrice / 100;
                    couponDto.Amount = percent * listPrice / 100;
                    couponDto.CouponId = coupons.FirstOrDefault().Id;
                }
            }
            else
            {
                couponDto.Amount = 0;
                if (couponDto.Message == "")
                    couponDto.Message = "Coupon is invalid (X1)";
            }

            return couponDto;
        }

        [HttpGet]
        public DtoCapacity GetCapacityByEventureId(int id)
        {
            //var rList = new List<DtoCapacity>();
            var capacity = new DtoCapacity(0, 0);

            var capacitySum = _contextProvider.Context.EventureLists.Where(ol => ol.EventureId == id).Sum(s => (int?)s.Capacity) ?? 0;
            //.Sum(income =>  (decimal?)income.Amount) ?? 0;

            var queryOwnerEventures = _contextProvider.Context.Eventures.Where(e => e.Id == id).Select(e => e.Id);
            var queryOwnersLists = _contextProvider.Context.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);
            var regcount = _contextProvider.Context.Registrations.Count(r => queryOwnersLists.Contains(r.EventureListId) && r.EventureOrder.Status == "Complete");

            //rList.Add(new DtoCapacity(regcount, capacitySum));
            //rList.Add(new DtoCapacity(232, 666));

            capacity.Regs = regcount;
            capacity.Capacity = capacitySum;

            return capacity;
        }

        [HttpGet]
        public DtoCapacity GetCapacityByEventureListId(int id)
        {
            var capacity = new DtoCapacity(0, 0);

            var capacitySum = _contextProvider.Context.EventureLists.Where(ol => ol.Id == id).Sum(s => (Int32?)s.Capacity);

            //var queryOwnerEventures = db.Eventures.Where(e => e.Id == id).Select(e => e.Id);
            //var queryOwnersLists = db.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);
            var regcount = _contextProvider.Context.Registrations.Count(r => r.EventureListId == id && r.EventureOrder.Status == "Complete");

            capacity.Regs = regcount;
            capacity.Capacity = capacitySum ?? 0;

            return capacity;
        }

        [HttpGet]
        public DtoCapacity GetCapacityByOwnerId(int id)
        {
            var capacity = new DtoCapacity(0, 0);

            var queryOwnerEventures = _contextProvider.Context.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);
            var capacitySum = _contextProvider.Context.EventureLists.Where(ol => queryOwnerEventures.Contains(ol.Id)).Sum(s => s.Capacity);

            var queryOwnersLists = _contextProvider.Context.EventureLists.Where(el => queryOwnerEventures.Contains(el.EventureId)).Select(l => l.Id);
            var regcount = _contextProvider.Context.Registrations.Count(r => queryOwnersLists.Contains(r.EventureListId) && r.EventureOrder.Status == "Complete");

            //var regcount = (from r in _contextProvider.Context.Registrations
            //               join o in _contextProvider.Context.Orders
            //                                    on r.EventureOrderId equals o.Id
            //                                    where r.EventureListId == id
            //                                    && o.Status == "Complete").Count()
            //                                    //select r.ParticipantId;

            capacity.Regs = regcount;
            capacity.Capacity = capacitySum;

            return capacity;
        }

        [HttpGet]
        public IEnumerable<EventureList> EventureListsByOwnerId(int ownerId)
        {
            var queryOwnerEventures = _contextProvider.Context
                .Eventures.Where(e => e.OwnerId == ownerId)
                .Select(e => e.Id);

            return _contextProvider.Context.EventureLists
                .Where(el => queryOwnerEventures.Contains(el.EventureId)
                    && (el.Active)
                    && (EntityFunctions.TruncateTime(el.DateBeginReg) <= EntityFunctions.TruncateTime(DateTime.Now))
                    && (EntityFunctions.TruncateTime(el.DateEndReg) >= EntityFunctions.TruncateTime(DateTime.Now)))
                    .OrderBy(el => el.SortOrder).ToList();
        }

        [HttpGet]
        public IQueryable<EventureList> EventureListsByEventureId(int eventureId)
        {
            return _contextProvider.Context.EventureLists
                .Where(l => (l.EventureId == eventureId)
                    && (l.Active)
                    && (EntityFunctions.TruncateTime(l.DateBeginReg) <= EntityFunctions.TruncateTime(DateTime.Now))
                    && (EntityFunctions.TruncateTime(l.DateEndReg) >= EntityFunctions.TruncateTime(DateTime.Now))
                    && l.Capacity > l.Registration.Count()
                    );
        }

        [HttpGet]
        public IQueryable<EventureOrder> OrderById(int id)
        {
            return _contextProvider.Context.Orders
                                             .Include("Registrations")
                                             .Include("Registrations.Participant")
                                             .Include("Surcharges")
                                             .Where(o => (o.Id == id));
        }

        [HttpGet]
        public IQueryable<Participant> ParticipantsByHouseId(int houseId)
        {
            return _contextProvider.Context.Participants
                                   .Where(p => p.HouseId == houseId) //.Where(p => p.Hous)eId = 1);
                                   .OrderBy(p => (p.HouseId == p.Id) ? -1 : 1);   //this should cause the one that is houseID to top
        }

        [HttpGet]
        public IEnumerable<EventureGroup> GroupsBelowCapacity(int listId)      //mjb this needs complete
        {
            string query =
                " select distinct g.* from EventureGroup g " +
                " left join Registration r on g.Id = r.GroupId " +
                " where (select count(*) from Registration where groupid = g.id) < g.Capacity  " +
                " AND g.Active = 1" +
                " AND g.EventureListId = " + listId;

            return _contextProvider.Context.Database.SqlQuery<EventureGroup>(query);
        }

        [HttpGet]
        [AcceptVerbs("OPTIONS")]
        public string Metadata()
        {
            return _contextProvider.Metadata();
        }

        [HttpGet]
        public IQueryable<Eventure> Eventures()
        {
            return _contextProvider.Context.Eventures;
        }

        [HttpGet]
        public IQueryable<EventureGroup> EventureGroups()
        {
            return _contextProvider.Context.EventureGroups;
        }

        [HttpGet]
        public IQueryable<Resource> Resources()
        {
            return _contextProvider.Context.Resources;
        }

        [HttpGet]
        public IQueryable<Registration> Registrations()
        {
            return _contextProvider.Context.Registrations;
        }

        [HttpGet]
        public IQueryable<EventureList> EventureLists()
        {
            return _contextProvider.Context.EventureLists;
        }

        [HttpGet]
        public IQueryable<EventureTransfer> EventureTransfers()
        {
            return _contextProvider.Context.EventureTransfers;
        }

        [HttpGet]
        public object Lookups()
        {
            var participants = _contextProvider.Context.Participants;
            //var timeslots =  _contextProvider.Context.TimeSlots;
            return new { participants };  //, timeslots
        }

        [HttpGet]
        public IQueryable<Participant> Participants()
        {
            return _contextProvider.Context.Participants;
        }

        [HttpGet]
        public IQueryable<Addon> Addons()
        {
            return _contextProvider.Context.Addons;
        }

        [HttpGet]
        public IQueryable<StockQuestionSet> StockQuestionSets()
        {
            return _contextProvider.Context.StockQuestionSets;
        }

        [HttpGet]
        public IQueryable<StockAnswerSet> StockAnswerSets()
        {
            return _contextProvider.Context.StockAnswerSets;
        }

        [HttpGet]
        public IQueryable<Coupon> Coupons()
        {
            return _contextProvider.Context.Coupons;
        }

        [HttpGet]
        public IQueryable<Client> Clients()
        {
            return _contextProvider.Context.Clients;
        }

        [HttpGet]
        public IQueryable<Owner> Owners()
        {
            return _contextProvider.Context.Owners;
        }

        [HttpGet]
        public IQueryable<EventureService> EventureServices()
        {
            return _contextProvider.Context.EventureServices;
        }

        [HttpGet]
        public IQueryable<Report> Reports()
        {
            return _contextProvider.Context.Reports;
        }

        [HttpGet]
        public IQueryable<EventurePlanItem> EventurePlanItems()
        {
            return _contextProvider.Context.PlanItems;
        }

        [HttpGet]
        public IQueryable<ResourceItemCategory> ResourceItemCategories()
        {
            return _contextProvider.Context.ResourceItemCategories;
        }

        [HttpGet]
        public IQueryable<ResourceItem> ResourceItems()
        {
            return _contextProvider.Context.ResourceItems;
        }

        [HttpGet]
        public IQueryable<FeeSchedule> FeeSchedules()
        {
            return _contextProvider.Context.FeeSchedules;
        }

        [HttpGet]
        public IQueryable<Team> Teams()
        {
            return _contextProvider.Context.Teams;
        }

        [HttpGet]
        public IQueryable<TeamMember> TeamMembers()
        {
            return _contextProvider.Context.TeamMembers;
        }

        [HttpGet]
        public IQueryable<TeamMemberPayment> TeamMemberPayments()
        {
            return _contextProvider.Context.TeamMemberPayments;
        }

        [HttpGet]
        public IQueryable<Question> Questions()
        {
            return _contextProvider.Context.Question;
        }

        [HttpGet]
        public IQueryable<QuestionOption> QuestionOptions()
        {
            return _contextProvider.Context.QuestionOption;
        }

        [HttpGet]
        public IQueryable<Answer> Answers()
        {
            return _contextProvider.Context.Answer;
        }

        [HttpPost]
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _contextProvider.SaveChanges(saveBundle);
        }

    }

    public class BreezeSimpleCorsHandler : DelegatingHandler
    {
        const string Origin = "Origin";
        const string AccessControlRequestMethod = "Access-Control-Request-Method";
        const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
        const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var isCorsRequest = request.Headers.Contains(Origin);
            var isPreflightRequest = request.Method == HttpMethod.Options;
            if (isCorsRequest)
            {
                if (isPreflightRequest)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Headers.Add(AccessControlAllowOrigin,
                      request.Headers.GetValues(Origin).First());

                    var accessControlRequestMethod =
                      request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();

                    if (accessControlRequestMethod != null)
                    {
                        response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
                    }

                    var requestedHeaders = string.Join(", ",
                       request.Headers.GetValues(AccessControlRequestHeaders));

                    if (!string.IsNullOrEmpty(requestedHeaders))
                    {
                        response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
                    }

                    response.Headers.Add(AccessControlAllowCredentials, "true");

                    var tcs = new TaskCompletionSource<HttpResponseMessage>();
                    tcs.SetResult(response);
                    return tcs.Task;
                }
                return base.SendAsync(request, cancellationToken).ContinueWith(t =>
                {
                    var resp = t.Result;
                    resp.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
                    resp.Headers.Add(AccessControlAllowCredentials, "true");
                    return resp;
                });
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}




//[HttpGet]
//public EventureTransfer TransferInfo(int Id)
//{
//   var t = from e in _contextProvider.Context.EventureTransfers
//    join fl in _contextProvider.Context.EventureLists 
//    on e.EventureListIdFrom equals fl.Id
//    join tl in _contextProvider.Context.EventureLists 
//    on e.EventureListIdTo equals tl.Id
//    where e.Id == 1
//    select new
//    {	fl.Name, fl.CurrentFee, x = tl.Name, y = tl.CurrentFee	};
//}