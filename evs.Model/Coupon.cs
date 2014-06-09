using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

namespace evs.Model
{
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
}
