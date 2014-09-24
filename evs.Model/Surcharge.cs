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

        public virtual EventureOrder EventureOrder { get; set; }
        public virtual Participant Resource { get; set; }    //will this work
    }

    public class Coupon
    {
        public Int32 Id { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public decimal Amount { get; set; }
        public Int32 DiscountType { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public Int32 Capacity { get; set; }
        public Int32 CouponTypeLinkId { get; set; }
        public string CouponType { get; set; }
        public Int32 Usage { get; set; }
        public Int32 Redeemed { get; set; }
        public Boolean IsOnlyForOwned { get; set; }
        public Int32 OwnerId { get; set; }
        //public Int32 RegistrationId { get; set; }

        //public virtual EventureList EventureList { get; set; }
        //public virtual Registration Registration { get; set; }EventureListId
    }

    public class Addon
    {
        public Int32 Id { get; set; }
        public bool Active { get; set; }
        public decimal Amount { get; set; }
        public Int32 AmountTypeId { get; set; }      // 0=$  1=%
        public string AddonType { get; set; }   //listfee,
        public string AddonDesc { get; set; }   //appears on receipt line item
        public Int32 AddonTypeLinkId { get; set; }
        public Boolean IsUsat { get; set; }
        public Boolean IsShirtUpgrade { get; set; }
        public Boolean IsOnlyForOwned { get; set; }
        public Int32 OwnerId { get; set; }
    }
}
