using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
//using System.ComponentModel;

namespace evs.Model
{
    public class Eventure
    {
        public Int32 Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Desc { get; set; }
        public string DisplayHeading { get; set; }
        public string DisplayDate { get; set; }
        public DateTime DateEventure { get; set; }
        public string ImageFileName { get; set; }
        public Int32 OwnerId { get; set; }
        public Boolean Active { get; set; }
        public Int32 SortOrder { get; set; }
        public bool RefundsAllowed { get; set; }
        public bool TransfersAllowed { get; set; }
        public bool DeferralsAllowed { get; set; }
        public decimal TransferFee { get; set; }
        public decimal DeferralFee { get; set; }
        public DateTime? DateTransferCutoff { get; set; }
        public DateTime? DateDeferralCutoff { get; set; }
        //public Boolean IsUsat { get; set; }
        public Boolean Managed { get; set; }
        public Int32? ClientId { get; set; }
        public Boolean IsGroupRequired { get; set; }
        public Boolean IsTeam { get; set; }
        public string Location { get; set; }
        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        // Navigation property
        public ICollection<EventureList> EventureLists { get; set; }
    }

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
        public Boolean IsBundle { get; set; }
        public string MinAge { get; set; }
        public string MaxAge { get; set; }
        public string MaxRunTime { get; set; }
        //public Boolean IsManaged { get; set; }
        //public Int32? GroupId { get; set; }
        public string Charity { get; set; }
        //public Int32 Type { get; set; }
        public DateTime DateCreated { get; set; }
        //public Int32? EventureListTypeId { get; set; }  //eventually won't be ?
        public string PaymentTerms { get; set; }
        public Boolean IsGroupRequired { get; set; }


        // Navigation property 
        //public virtual Eventure Eventure { get; set; }
        public ICollection<Registration> Registration { get; set; }
        public Eventure Eventure { get; set; }
        public EventureListType EventureListType { get; set; }
        public ICollection<EventureListBundle> EvenutreListBundles { get; set; }
        //public Result Result { get; set; }
        //public virtual string EventureName { get; set; }
        //public ICollection<Coupon> Coupons { get; set; }
        //public ICollection<Participant> Participants { get; set; }
        //public ICollection<EventureGroup> EventureGroups { get; set; }
        //public ICollection<QQuestion> Questions { get; set; }
        //public ICollection<FeeSchedule> FeeSchedules { get; set; }
    }


    public class EventureListBundle
    {
        public Int32 Id { get; set; }
        public Int32 EvenureListId { get; set; }
    }

    //public class EventureListType   //enum
    //{
    //    public Int32 Id { get; set; }
    //    public string Name { get; set; }
    //    public Int32 OwnerId { get; set; }  
    //}

    public enum EventureListType
    {
        Standard = 1,
        TeamSponsored,
        TeamSuggest,
        TeamIndividual,
        Lottery
    }

}
