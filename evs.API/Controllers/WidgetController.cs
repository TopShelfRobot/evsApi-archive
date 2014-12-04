using evs.DAL;
using evs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace evs.API.Controllers
{
    [RoutePrefix("api/widget")]
    public class WidgetController : ApiController
    {
        readonly evsContext db = new evsContext();

        // GET api/Eventures 
        [HttpGet]
        [Authorize]
        public IEnumerable<Eventure> GetAllEventuresByOwnerId(int id)
        {
            return db.Eventures.Where(e => e.OwnerId == id).OrderByDescending(e => e.Id);
        }

        //used id instead of ownerID so i can use same route since i hope to replace this soon anyway
        public IEnumerable<Participant> GetParticipantsByOwnerId(int id)
        {
            return db.Participants.Where(p => p.OwnerId == id).OrderByDescending(p => p.Id);

        }


        //[HttpGet]
        //public object GetTeamRegistrationsByOwnerId(Int32 id)
        //{
        //    return db.Teams.Where(t => t.Registration.EventureOrder.OwnerId == id
        //                            && t.Registration.EventureOrder.Status == "Complete")
        //        .Select(t => new
        //        {
        //            t.Name,
        //            t.Id,
        //            ListName = t.Registration.EventureList.DisplayName,
        //            CoachName = t.Coach.FirstName + " " + t.Coach.LastName,
        //            Amount = (decimal?)t.TeamMemberPayments.Sum(p => p.Amount) ?? 0,
        //            Balance = t.Registration.ListAmount - ((decimal?)t.TeamMemberPayments.Sum(p => p.Amount) ?? 0),
        //            t.Registration.EventureList.EventureListType,
        //            EventName = t.Registration.EventureList.Eventure.Name
        //        })
        //        .ToList();
        //}

        public object GetVolunteersByOwnerId(int id)
        {
            return db.Volunteers.Where(v => v.Participant.OwnerId == id)
                .Select(v => new { v.Participant.FirstName, v.Participant.LastName, v.Participant.Email, v.Participant.PhoneMobile, v.Id })
                .ToList();

        }

        public IEnumerable<Resource> GetResourcesByOwnerId(int id)
        {
            //return db.Participants.Where(p => p.OwnerId == id);
            return db.Resources.Where(v => v.OwnerId == id);
        }
    }
}
