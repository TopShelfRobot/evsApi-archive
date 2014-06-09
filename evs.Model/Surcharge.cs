using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace evs.Model
{
    public class Surcharge
    {
        public Int32 Id { get; set; }
        public Int32 EventureOrderId  { get; set; }
        public decimal Amount { get; set; }
        public string ChargeType { get; set; }   //listcoupon, listfee, eventcoupon
        public string Description { get; set; }
        //appears on receipt line item  //right now we only have list  eventcoupon is still tied to list w/diff type
        public Int32? EventureListId { get; set; }
        public Int32? ParticipantId { get; set; }
        public Int32? CouponId { get; set; }
        public DateTime DateCreated { get; set; }
        public Int32 ConvOrderId { get; set; }

        public EventureOrder EventureOrder { get; set; }
    }
}
