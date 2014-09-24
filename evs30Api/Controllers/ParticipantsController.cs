using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
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

        public class DtoShift
        {
            public Int32 Id { get; set; }
            public string EventName { get; set; }
            public string JobName { get; set; }
            public Int32 VolunteerId { get; set; }
            public DateTime TimeBegin { get; set; }
            public DateTime TimeEnd { get; set; }

            public DtoShift(Int32 id, string eventName, string jobName, Int32 volunteerId, DateTime timeBegin, DateTime timeEnd)
            {
                Id = id;
                EventName = eventName;
                JobName = jobName;
                VolunteerId = volunteerId;
                TimeBegin = timeBegin;
                TimeEnd = timeEnd;
            }
        }

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

        public object GetVolunteersByOwnerId(int id)
        {
            return db.Volunteer.Where(v => v.Participant.OwnerId == id)
                .Select(v => new { v.Participant.FirstName, v.Participant.LastName, v.Participant.Email, v.Participant.PhoneMobile, v.Id })
                .ToList();

        }

        public object GetVolunteerScheduleByVolunteerId(int id)
        {
            var estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var queryVols = db.VolunteerSchedule.Where(s => s.VolunteerId == id)
                .Select(s => new
                {
                    s.Eventure.DisplayHeading,
                    s.VolunteerShift.VolunteerJob.Name,
                    s.VolunteerShift.TimeBegin,
                    s.VolunteerShift.TimeEnd,
                    s.VolunteerId,
                    s.Id
                });

            var shifts = new List<DtoShift>();
            foreach (var v in queryVols)
            {
                var shift = new DtoShift(v.Id, v.DisplayHeading, v.Name, v.VolunteerId, TimeZoneInfo.ConvertTimeFromUtc(v.TimeBegin, estZone), TimeZoneInfo.ConvertTimeFromUtc(v.TimeEnd, estZone));
                shifts.Add(shift);
            }
            return shifts;
        }

        public object GetVolunteersByEventureId(int id)
        {
            return db.VolunteerSchedule.Where(s => s.EventureId == id)
                .Select(s => new { s.Volunteer.Participant.FirstName, s.Volunteer.Participant.LastName, s.Volunteer.Participant.Email, s.VolunteerShift.VolunteerJob.Name, s.VolunteerShift.TimeBegin, s.VolunteerShift.TimeEnd })
                .ToList();
        }

        public class DtoVolunteer
        {
            public Int32 ScheduleId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string JobName { get; set; }
            public DateTime TimeBegin { get; set; }
            public DateTime TimeEnd { get; set; }

            public DtoVolunteer(Int32 scheduleId, string firstName, string lastName, string email, string jobName, DateTime timeBegin, DateTime timeEnd)
            {
                ScheduleId = scheduleId;
                FirstName = firstName;
                LastName = lastName;
                Email = email;
                JobName = jobName;
                TimeBegin = timeBegin;
                TimeEnd = timeEnd;
            }
        }
        //change this one to match GetVolunteerScheduleByVolunteerId
        public object GetVolunteersByVolunteerJobId(int id)
        {
            var estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var queryVols = db.VolunteerSchedule.Where(s => s.VolunteerShift.VolunteerJobId == id)
                .Select(s => new { s.Id, s.Volunteer.Participant.FirstName, s.Volunteer.Participant.LastName, s.Volunteer.Participant.Email, s.VolunteerShift.VolunteerJob.Name, s.VolunteerShift.TimeBegin, s.VolunteerShift.TimeEnd })
                .OrderBy(s => s.TimeBegin);

            var volunteers = new List<DtoVolunteer>();
            foreach (var v in queryVols)
            {
                var volunteer = new DtoVolunteer(v.Id, v.FirstName, v.LastName, v.Email, v.Name, TimeZoneInfo.ConvertTimeFromUtc(v.TimeBegin, estZone), TimeZoneInfo.ConvertTimeFromUtc(v.TimeEnd, estZone));
                volunteers.Add(volunteer);
            }
            return volunteers;

        }

        public object GetVolunteerJobsByEventureId(int id)
        {
            return db.VolunteerJob.Where(j => j.EventureId == id)
                .Select(j => new { j.Name, j.Description, j.Id })
                .ToList();
        }

        public class DtoVolunteerData
        {
            public Int32 Id { get; set; }
            public String Name { get; set; }
            public Int32 Shifts { get; set; }
            public Int32 Capacity { get; set; }
            public Int32 MaxCapacity { get; set; }
        }

        [HttpGet]
        public object GetVolunteerDataByEventureId(int id)      //mjb this needs complete
        {
            string query =
                "select j.id, j.name, (select count(*) from [VolunteerShift] where volunteerjobid = j.id) as shifts, " +
                "isnull((select sum(capacity) from [VolunteerShift] where volunteerjobid = j.id), 0) as maxcapacity,  " +
                "(select count(*) from [VolunteerSchedule] inner join [VolunteerShift] on [VolunteerSchedule].volunteershiftid = [VolunteerShift].id where [VolunteerShift].volunteerjobid = j.id) as capacity " +
                "from [VolunteerJob] j where j.eventureId =  " + id;

            return db.Database.SqlQuery<DtoVolunteerData>(query).ToList();
        }

        //AddSchedule
        [HttpPost]
        //[AllowAnonymous]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage AddSchedule(JObject saveBundle)
        {
            try
            {
                var houseId = (Int32)saveBundle["houseId"];
                var eventId = (Int32)saveBundle["eventId"];
                var shiftId = (Int32)saveBundle["shiftId"];
                var volunteerId = 0;
                var volunteer = new Volunteer();
                //verify all are > 0 

                var schedule = new VolunteerSchedule();

                schedule.EventureId = eventId;
                schedule.VolunteerShiftId = shiftId;
                schedule.DateCreated = DateTime.Now;

                if (db.Volunteer.Any(v => v.ParticipantId == houseId))
                {
                    schedule.VolunteerId = db.Volunteer.SingleOrDefault(v => v.ParticipantId == houseId).Id;
                }
                else
                {
                    volunteer.ParticipantId = houseId;
                    volunteer.DateCreated = DateTime.Now;
                    db.Volunteer.Add(volunteer);
                    schedule.VolunteerId = volunteer.Id;
                }

                db.VolunteerSchedule.Add(schedule);
                db.SaveChanges();


                //call mail
                //HttpResponseMessage result = new MailController().SendConfirmMail(order.Id);

                var resp = Request.CreateResponse(HttpStatusCode.OK);
                //resp.Content = new StringContent();
                resp.Content = new StringContent(schedule.Id.ToString(), Encoding.UTF8, "text/plain");
                return resp;



            }
            catch (Exception ex)
            {
                //send quick email
                HttpResponseMessage result = new MailController().SendInfoMessage("boone.mike@gmail.com", "Error Handler_AddSchedule: " + ex.Message + "\n\n" + ex.InnerException);

                //regular log
                var logE = new EventureLog
                {
                    Message = "Error Handler: " + ex.Message + "\n\n" + ex.InnerException + " - bundle: " + saveBundle,
                    Caller = "AddSchedule",
                    Status = "ERROR",
                    LogDate = System.DateTime.Now.ToLocalTime()
                };
                db.EventureLogs.Add(logE);
                db.SaveChanges();

                if (Request != null)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was error with your transaction, please try again.");
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }
    }
}
