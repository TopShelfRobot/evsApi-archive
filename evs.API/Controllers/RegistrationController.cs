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
                         && l.Capacity > l.Registrations.Count()
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
                   o.Name,
                   o.StripeCheckoutButtonText,
                   o.StripeOrderDescription, 
                   o.ConfirmButtonText, 
                   o.RegisterButtonText,
                   o.MainColor,
                   o.HoverColor,
                   o.HighlightColor,
                   o.NavTextColor,
                   o.LogoImageName,
                   o.SupportEmail,
                   o.SupportPhone
               }).ToList();
        }

        [HttpGet]
        //[Authorize]
        public object getDiscountRulesByOwnerId(int ownerId)
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
                   o.Name,
                   o.StripeCheckoutButtonText,
                   o.StripeOrderDescription,

                   o.IsMultiParticipantDiscountCartRule,
                   o.IsMultiRegistrationDiscountCartRule,

                   o.MultiParticipantDiscountAmount,
                   o.MultiParticipantDiscountAmountType,
                   o.MultiRegistrationDiscountAmount,
                   o.MultiRegistrationDiscountAmountType,

                   o.IsRegistrationOnProfile,
                   o.IsTeamRegistrationOnProfile,
                   o.IsParticipantOnProfile,
                   o.IsCaptainOnProfile
               }).ToList();
        }

        [HttpGet]
        public object OrderById(Int32 id)
        {
            //return _contextProvider.Context.Orders
            //                                 .Include("Registrations")
            //                                 .Include("Registrations.Participant")
            //                                 .Include("Surcharges")
            //                                 .Where(o => (o.Id == id));

            return _contextProvider.Context.Registrations
                        .Where(r => (r.EventureOrderId == id))
                        .Select(r => new
                        {
                            r.EventureOrder.Amount,
                            r.EventureList.DisplayName,
                            groupName = r.EventureGroup.Name ?? " ",
                            participant = r.Participant.FirstName + " " + r.Participant.LastName,
                            orderDate = r.EventureOrder.DateCreated
                        }).ToList();

        }

        [HttpGet]
        public object GetTeamInfoByRegistrationId(Int32 id)    //this will only be used on member payment receipt
        {
            return _contextProvider.Context.Teams
                     .Where(t => t.RegistrationId == id)
                     .Select(t => new
                     {
                         t.Registration.EventureOrder.Amount,
                         teamName = t.Name,
                         ListName = t.Registration.EventureList.Name
                     }).ToList();
        }

        //[Authorize]   //this is turned off because team members are not required to be logged in //mjb
        [HttpGet]
        public object GetTeamMemberPaymentInfoByTeamMemberGuid(Guid id)
        {
            return _contextProvider.Context.TeamMembers
                     .Where(t => t.TeamMemberGuid == id && t.Active)
                     .Select(m => new
                     {
                         ListName = m.Team.Registration.EventureList.DisplayName,
                         m.Team.Registration.EventureList.EventureListType,
                         m.Team.Registration.EventureList.CurrentFee,
                         RegAmount = m.Team.Registration.ListAmount,   //totalAmount??
                         userPaid = (decimal?)m.TeamMemberPayments.Sum(p => p.Amount) ?? 0,     //this is temp;  if they make multiple payments reciept will be wrong
                         m.Id,
                         m.Team.Name,
                         teamMemberId = m.Id,
                         m.Email,
                         teamId = m.Team.Id
                     }).ToList();
        }     //this will only be used on member payment receipt??

        //[Authorize]   //this is turned off because team members are not required to be logged in //mjb
        [HttpGet]
        public object GetNotPaidTeamMemberCountByTeamGuid(Guid id)
        {
            var allTeamCount = _contextProvider.Context.TeamMembers.Count(t => t.Team.TeamGuid == id && t.Active);

            var paidTeamCount = _contextProvider.Context.TeamMembers.Count(t => t.Team.TeamGuid == id && t.Active && t.TeamMemberPayments.Count > 0);

            return allTeamCount - paidTeamCount;
        }

        //[Authorize]   //this is turned off because team members are not required to be logged in //mjb
        [HttpGet]
        public object GetTeamMemberPaymentSumByTeamGuid(Guid id)
        {
            return _contextProvider.Context.TeamMembers
                                .Where(t => t.Team.TeamGuid == id && t.Active)
                                .Sum(t => (decimal?)t.TeamMemberPayments.Sum(p => (decimal?)p.Amount) ?? 0);
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

        [HttpGet]
        public IQueryable<UserAgent> UserAgents()
        {
            return _contextProvider.Context.UserAgents;
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
        public IQueryable<Addon> Addons()
        {
            return _contextProvider.Context.Addons;
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
