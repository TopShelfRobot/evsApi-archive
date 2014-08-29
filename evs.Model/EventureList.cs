using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;/

namespace evs.Model
{
    public class EventureList
    {
        public Int32 Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string DisplayName { get; set; }
        public Int32 EventureId { get; set; }
        public Int32 Capacity { get; set; }
        public string Desc { get; set; }
        public DateTime DateEventureList { get; set; }
        public string ImageFileName { get; set; }
        public decimal CurrentFee { get; set; }
        public Boolean Active { get; set; }
        public DateTime DateBeginReg { get; set; }
        public DateTime DateEndReg { get; set; }
        public Int32 SortOrder { get; set; }
        public string WaiverText { get; set; }
        public Boolean IsWaiverDisplayed { get; set; }
        //public Boolean IsGroupDisplayed { get; set; }
        public string MinAge { get; set; }
        public string MaxAge { get; set; }
        public string MaxRunTime { get; set; }
        //public Boolean IsManaged { get; set; }
        //public Int32? GroupId { get; set; }
        public string Charity { get; set; }
        public Int32 ListingType { get; set; }
        
    // Navigation property 
        //public virtual Eventure Eventure { get; set; }
        public ICollection<Registration> Registration { get; set; }
        public Eventure Eventure { get; set; }
        //public Result Result { get; set; }
        //public virtual string EventureName { get; set; }
        //public ICollection<Coupon> Coupons { get; set; }
        //public ICollection<Participant> Participants { get; set; }
        //public ICollection<EventureGroup> EventureGroups { get; set; }
        //public ICollection<QQuestion> Questions { get; set; }
        //public ICollection<FeeSchedule> FeeSchedules { get; set; }
    }
}