using evs.DAL;
//using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using evs.Model;
//using System.Web.Mvc;

namespace evs30Api.Controllers
{
    public class ParticipantsController : ApiController
    {
        evsContext db = new evsContext();

        public IEnumerable<Participant> GetParticipants()
        {
            return db.Participants.ToArray();
        }

        public Participant GetParticipant(int id)
        {
            return db.Participants.FirstOrDefault(p => p.Id == id);
        }

        //used id instead of ownerID so i can use same route since i hope to replace this soon anyway
        public IEnumerable<Participant> GetParticipantsByOwnerId(int id)
        {
            return db.Participants.Where(p => p.OwnerId == id).OrderByDescending(p => p.Id);

        }

        public IEnumerable<Participant> GetParticipantsByHouseId(int id)
        {
            //return db.Participants.Where(p => p.OwnerId == id);
            return db.Participants.Where(p => p.HouseId == id);

        }

        public IEnumerable<Participant> GetRegisteredParticipantsByEventureId(int id)
        {
           //var queryEventureListIdsByEventureId = db.EventureLists.Where(l => l.EventureId == id).Select(l => l.Id);
            //var queryRegPartIdsByEventureList =db.Registrations.Where(r => queryEventureListIdsByEventureId.Contains(r.EventureListId)).Select(r => r.ParticipantId);

            var queryRegPartIdsByEventureList = from r in db.Registrations
                                                join o in db.Orders
                                                on r.EventureOrderId equals o.Id
                                                join l in db.EventureLists
                                                on r.EventureListId equals l.Id
                                                where l.EventureId == id
                                                && o.Status == "Complete"
                                                select r.ParticipantId;
            
            return db.Participants.Where(p => queryRegPartIdsByEventureList.Contains(p.Id)).OrderByDescending(p => p.Id);
        }

        public IEnumerable<Participant> GetRegisteredParticipantsByEventureListId(int id)
        {
            //var queryEventureListIdsByEventureListId = db.EventureLists.Where(l => l.Id == id).Select(l => l.Id);
            //var queryRegPartIdsByEventureList = db.Registrations.Where(r => queryEventureListIdsByEventureListId.Contains(r.EventureListId)).Select(r => r.ParticipantId);
            var queryRegPartIdsByEventureList = from r in db.Registrations
                                                join o in db.Orders
                                                on r.EventureOrderId equals o.Id
                                                where r.EventureListId == id
                                                && o.Status == "Complete"
                                                select r.ParticipantId;

            return db.Participants.Where(p => queryRegPartIdsByEventureList.Contains(p.Id));
        }


        public IEnumerable<Participant> GetRegisteredParticipantsByGroupId(int id)
        {
           //need to check order.Status == 'Complete'   //mjb 
           //var queryRegPartIdsByEventureList = db.Registrations.Where(r => r.GroupId == id ).Select(r => r.ParticipantId);
            var queryRegPartIdsByEventureList = from r in db.Registrations
                                                join o in db.Orders
                                                on r.EventureOrderId equals o.Id
                                                where r.GroupId == id
                                                && o.Status == "Complete"
                                                select r.ParticipantId;

            return db.Participants.Where(p => queryRegPartIdsByEventureList.Contains(p.Id));
        }

        public void PutParticipant()
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }
    }
}
