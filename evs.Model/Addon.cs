using System;

namespace evs.Model
{
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
