//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;

using System;
using System.Linq;
using System.Web.Http;
using evs.DAL;

namespace evs30Api.Controllers
{
    public class TeamsController : ApiController
    {
        private evsContext db = new evsContext();

       
        //[AllowAnonymous]
        //[AcceptVerbs("OPTIONS")]
        //[System.Web.Mvc.ValidateAntiForgeryToken]
        [HttpGet]
        public object GetTeamRegistrationsByCoachId(int id)
        {
            return db.Teams.Where(t => t.CoachId == id
                                    && t.Registration.EventureOrder.Status == "Complete")
                .Select(t => new { 
                    t.Name, 
                    t.Id, 
                    ListName = t.Registration.EventureList.DisplayName,
                    CoachName = t.Coach.FirstName + " " + t.Coach.LastName,
                    Amount = (decimal?)t.TeamMemberPayments.Sum(p => p.Amount) ?? 0,
                    Balance = t.Registration.ListAmount - ((decimal?)t.TeamMemberPayments.Sum(p => p.Amount) ?? 0),
                    EventName = t.Registration.EventureList.Eventure.Name 
                })
                .ToList();
        }

        [HttpGet]
        public object GetTeamRegistrationsByHouseId(int id)
        {
            return db.Teams.Where(t => t.Registration.EventureOrder.HouseId == id  
                                    && t.Registration.EventureOrder.Status == "Complete")
                .Select(t => new
                {
                    t.Name,
                    CoachName = t.Coach.FirstName + " " + t.Coach.LastName,
                    t.Id,
                    ListName = t.Registration.EventureList.DisplayName,
                    t.Registration.Participant.FirstName,
                    t.Registration.Participant.LastName,
                    EventName = t.Registration.EventureList.Eventure.Name
                })
                .ToList();
        }

        [HttpGet]
        public object GetTeamRegistrationsByOwnerId(int id)
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
                    t.Registration.EventureList.ListingType,
                    EventName = t.Registration.EventureList.Eventure.Name
                })
                .ToList();
        }

        [HttpGet]
        public object GetTeamMembersByTeamId(int id)
        {
            return db.TeamMembers.Where(t => t.Team.Id == id
                                            && t.Active)
                .Select(t => new
                {
                    t.Name,
                    t.Id,
                    t.TeamId,
                    t.Email,
                    //Balance = t.Team.Registration.ListAmount - ((decimal?)t.TeamMemberPayments.Sum(p => p.Amount) ?? 0),
                    Amount = (decimal?)t.TeamMemberPayments.Sum(p => p.Amount) ?? 0,
                    EventureOrderId = t.Team.Registration.EventureOrder.Id
                })
                .ToList();
        }

      

        //[HttpGet]
        //public object GetTeamMemberPaymentSumByTeamMemberGuid(Guid id)
        //{
        //    return db.TeamMembers.Where(t => t.Team.TeamGuid == id
        //                                     && t.Active).Select(t => t.Team.TeamMemberPayments.Sum(p => p.Amount));
        //    //.Select(m => new
        //    //{
        //    //    ListName = m.Team.Registration.EventureList.DisplayName,
        //    //    RegAmount = m.Team.Registration.ListAmount,   //totalAmount??
        //    //    m.Id,
        //    //    m.Team.Name
        //    //})
        //    //.ToList();
        //}

        //[HttpGet]
        //public object GetTeamMemberCountByMemberGuid(Guid id)
        //{
        //    return db.TeamMembers.Count(t => t.Team.TeamGuid == id
        //                                     && t.Active);
        //    //.Select(m => new
        //    //{
        //    //    ListName = m.Team.Registration.EventureList.DisplayName,
        //    //    RegAmount = m.Team.Registration.ListAmount,   //totalAmount??
        //    //    m.Id,
        //    //    m.Team.Name
        //    //})
        //    //.ToList();
        //}
    }
}