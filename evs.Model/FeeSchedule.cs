using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace evs.Model
{
    
    public class FeeSchedule
    {
        public Int32 Id { get; set; }
        //public Int32 EventId { get; set; }
        public DateTime DateBegin { get; set; }
        public decimal Amount { get; set; }
        public bool Active { get; set; }
        //public Int32 ListingId { get; set; }
        public Int32 EventureListId { get; set; }
        //public string Desc { get; set; }

        //public virtual EventureList EventureList { get; set; }
        }
}
