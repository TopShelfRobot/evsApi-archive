using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Newtonsoft.Json.Linq;
using evs.DAL;
using evs.Model;
using System.Data.Entity.Core.Objects;

namespace evs.API.Controllers
{
    //[Authorize]
    [BreezeController]
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        readonly EFContextProvider<evsContext> _contextProvider = new EFContextProvider<evsContext>();
        
        //[HttpGet]
        //public IEnumerable<EventureList> EventureListsByOwnerId(int ownerId)
        //{
        //    var queryOwnerEventures = _contextProvider.Context
        //        .Eventures.Where(e => e.OwnerId == ownerId)
        //        .Select(e => e.Id);

        //    return _contextProvider.Context.EventureLists
        //        .Where(el => queryOwnerEventures.Contains(el.EventureId)
        //            && (el.Active)
        //            && (EntityFunctions.TruncateTime(el.DateBeginReg) <= EntityFunctions.TruncateTime(DateTime.Now))
        //            && (EntityFunctions.TruncateTime(el.DateEndReg) >= EntityFunctions.TruncateTime(DateTime.Now)))
        //            .OrderBy(el => el.SortOrder).ToList();
        //}

        //[HttpGet]  //reg
        //public IQueryable<Participant> ParticipantsByHouseId(int houseId)
        //{
        //    return _contextProvider.Context.Participants
        //                           .Where(p => p.HouseId == houseId) //.Where(p => p.Hous)eId = 1);
        //                           .OrderBy(p => (p.HouseId == p.Id) ? -1 : 1);   //this should cause the one that is houseID to top
        //}

        //public IEnumerable<Eventure> GetAllEventuresByOwnerId(int id)
        //{
        //    return db.Eventures.Where(e => e.OwnerId == id).OrderByDescending(e => e.Id);

        //}

        [HttpGet]
        //[Authorize]
        public object getPublicOwnerByOwnerId(int ownerId)
        {
            return _contextProvider.Context.Owners
               .Where(o => o.Id == ownerId)
               .Select(o => new
               {
                   o.IsDuplicateOrderAllowed,
                   o.IsAddSingleFeeForAllRegs,
                   o.AddSingleFeeForAllRegsPercent,
                   o.AddSingleFeeType,
                   o.AddSingleFeeForAllRegsFlat,
                   o.EventureName,
                   o.ListingName,
                   o.GroupName,
                   o.ParticipantButtonText,
                   o.ListStatement,
                   o.TermsText,
                   o.RefundsText,
                   o.StripePublishableKey,
                   o.Name
               }).ToList();
        }



        //[AcceptVerbs("OPTIONS")]
        [AllowAnonymous]
        [HttpGet]
        public string Metadata()
        {
            return _contextProvider.Metadata();
        }

        [AllowAnonymous]
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

        //[HttpGet]
        //public object Lookups()
        //{
        //    var participants = _contextProvider.Context.Participants;
        //    //var timeslots =  _contextProvider.Context.TimeSlots;
        //    return new { participants };  //, timeslots
        //}

        [HttpGet]
        public IQueryable<Participant> Participants()
        {
            return _contextProvider.Context.Participants;
        }

        //[HttpGet]
        //public IQueryable<Addon> Addons()
        //{
        //    return _contextProvider.Context.Addons;
        //}

        //[HttpGet]
        //public IQueryable<StockQuestionSet> StockQuestionSets()
        //{
        //    return _contextProvider.Context.StockQuestionSets;
        //}

        //[HttpGet]
        //public IQueryable<StockAnswerSet> StockAnswerSets()
        //{
        //    return _contextProvider.Context.StockAnswerSets;
        //}

        [HttpGet]
        public IQueryable<Coupon> Coupons()
        {
            return _contextProvider.Context.Coupons;
        }

        [HttpGet]
        public IQueryable<EventureClient> EventureClients()
        {
            return _contextProvider.Context.EventureClients;
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

        //[HttpGet]
        //public IQueryable<Team> Teams()
        //{
        //    return _contextProvider.Context.Teams;
        //}

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

        [HttpGet]
        public IQueryable<VolunteerJob> VolunteerJobs()
        {
            return _contextProvider.Context.VolunteerJobs;
        }

        [HttpGet]
        public IQueryable<Volunteer> Volunteers()
        {
            return _contextProvider.Context.Volunteers;
        }

        [HttpGet]
        public IQueryable<VolunteerShift> VolunteerShifts()
        {
            return _contextProvider.Context.VolunteerShifts;
        }


        [HttpGet]
        public IQueryable<VolunteerSchedule> VolunteerSchedules()
        {
            return _contextProvider.Context.VolunteerSchedules;
        }

        //[AllowAnonymous]
        //[AcceptVerbs("OPTIONS")]
        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _contextProvider.SaveChanges(saveBundle);
        }
    }
}