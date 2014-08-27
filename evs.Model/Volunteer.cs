using System;

namespace evs.Model
{
    public class Volunteer
    {
        public Int32 Id { get; set; }
        public Int32 ParticipantId { get; set; }
        public DateTime DateCreated { get; set; }
        //navigation
        public virtual Participant Participant { get; set; }
    }

    public class VolunteerSchedule
    {
        public Int32 Id { get; set; }
        public DateTime DateCreated { get; set; }
        public Int32 VolunteerId { get; set; }
        public Int32 EventureId { get; set; }
        //public Int32 VolunteerJobId { get; set; }
        public Int32 VolunteerShiftId { get; set; }

        //navigation
        public virtual Volunteer Volunteer { get; set; }
        public virtual VolunteerShift VolunteerShift { get; set; }
        public virtual Eventure Eventure { get; set; }
        //public VolunteerJob VolunteerJob { get; set; }
    }

    public class VolunteerJob
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String AgeRestriction { get; set; }
        public DateTime DateCreated { get; set; }
        public Int32 EventureId { get; set; }
    }

    public class VolunteerShift
    {
        public Int32 Id { get; set; }
        public DateTime TimeBegin { get; set; }
        public DateTime TimeEnd { get; set; }
        public String ShiftDisplay { get; set; }
        public DateTime DateShift { get; set; }
        public Int32 Capacity { get; set; }
        public DateTime DateCreated { get; set; }
        public Int32 VolunteerJobId { get; set; }
        //navigation
        public virtual VolunteerJob VolunteerJob { get; set; }
    }
}
