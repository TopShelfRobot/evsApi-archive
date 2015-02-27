using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace evs.Model
{
    public class Team
    {
        public Team() 
        {
            this.TeamMembers = new List<TeamMember>();
            this.TeamMemberPayments = new List<TeamMemberPayment>();   //think i need to get rid of this
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TeamGuid { get; set; }
        public string Name { get; set; }
        public Int32 CoachId { get; set; }
        //public EventureList EventureList { get; set; }
        //public Boolean IsBye { get; set; }
        public Int32 RegistrationId { get; set; }
        public Boolean IsPaidInFull { get; set; }
        public Int32 OwnerId { get; set; }
        public DateTime DateCreated { get; set; }
        public Boolean Active { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
        public string Division { get; set; }
        public string TimeFinish { get; set; }

        //navigation
        public ICollection<TeamMember> TeamMembers { get; set; }
        public ICollection<TeamMemberPayment>  TeamMemberPayments { get; set; }
        public virtual Registration Registration { get; set; }
        [ForeignKey("CoachId")]
        public virtual Participant Coach { get; set; }
        public virtual Owner Owner { get; set; }
    }

    public class TeamMember
    {
        public TeamMember() 
        {
            this.TeamMemberPayments = new List<TeamMemberPayment>();   
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TeamMemberGuid { get; set; }
        public Int32? ParticipantId { get; set; }
        public Int32 TeamId { get; set; }
        //public Int32? StockAnswerSetId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Boolean Active { get; set; }
        public DateTime DateCreated { get; set; }
        public string Position { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        //navigation
        public virtual Team Team { get; set; }
        public ICollection<TeamMemberPayment> TeamMemberPayments { get; set; }
    }
    
    public class TeamMemberPayment
    {
        public Int32 Id { get; set; }
        public Int32 TeamId { get; set; }
        public decimal Amount { get; set; }
        public Int32 TeamMemberId { get; set; }
        public Boolean Active { get; set; }
        public DateTime DateCreated { get; set; }
        public Int32 EventureOrderId { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        //navigation
        public virtual TeamMember TeamMember { get; set; }
        public virtual Team Team { get; set; }
        public virtual EventureOrder EventureOrder { get; set; }
        //public Team Team { get; set; }
    }

    //public class Payment
    //{
    //    public Int32 Id { get; set; }
    //    public Int32 ParticipantId { get; set; }
    //    public Int32 EventureOrderId { get; set; }
    //    public Int32 TeamId { get; set; }

    //    //navigation
    //    public virtual EventureOrder EventureOrder { get; set; }
    //}
}
