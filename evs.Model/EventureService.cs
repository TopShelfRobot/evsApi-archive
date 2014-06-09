using System;

namespace evs.Model
{
    public class EventureService
    {
        public Int32 Id { get; set; }
        public Int32 VendorServiceId { get; set; }
        public string VendorServiceText { get; set; }
        public decimal Amount { get; set; }
        public Boolean IsVariable { get; set; } 
        public bool Active { get; set; }
        public Int32 EventureId { get; set; }
    }
}
