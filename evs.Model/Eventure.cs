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
        public string  DisplayHeading { get; set; }
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
   
        // Navigation property
        public ICollection<EventureList> EventureLists { get; set; }
    }
}
