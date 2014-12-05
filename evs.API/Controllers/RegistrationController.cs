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
    [BreezeController]
    [RoutePrefix("api/registration")]
    public class RegistrationController : ApiController
    {
        readonly evsContext _db = new evsContext();
        readonly EFContextProvider<evsContext> _contextProvider = new EFContextProvider<evsContext>();

        [HttpGet]
        [Authorize]
        public IEnumerable<EventureList> EventureListsByEventureId(int eventureId)
        {
            return _db.EventureLists
                        .Where(l => (l.EventureId == eventureId)
                        && (l.Active)
                        && (EntityFunctions.TruncateTime(l.DateBeginReg) <= EntityFunctions.TruncateTime(DateTime.Now))
                        && (EntityFunctions.TruncateTime(l.DateEndReg) >= EntityFunctions.TruncateTime(DateTime.Now))
                        && l.Capacity > l.Registration.Count()
                        );
        }

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


        [HttpGet]
        public IEnumerable<Participant> ParticipantsByHouseId(int houseId)
        {
            return _db.Participants
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

        //mjb this has to come out!!!!!  //mjb
        [HttpGet]
        public IQueryable<Owner> Owners()
        {
            return _contextProvider.Context.Owners;
        }

        [HttpGet]
        public IQueryable<Eventure> Eventures()
        {
            return _contextProvider.Context.Eventures;
        }

        [HttpGet]
        public IQueryable<EventureList> EventureLists()
        {
            return _contextProvider.Context.EventureLists;
        }

        [HttpGet]
        public IQueryable<Participant> Participants()
        {
            return _contextProvider.Context.Participants;
        }

        [HttpGet]
        public IQueryable<Question> Questions()
        {
            return _contextProvider.Context.Question;
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _contextProvider.SaveChanges(saveBundle);
        }

        //[AllowAnonymous]
        [HttpGet]
        public string Metadata()
        {
            return _contextProvider.Metadata();
        }
    }
}
