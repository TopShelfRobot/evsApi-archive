using System;

namespace evs.Model
{
    public class Payment
    {
        public Int32 Id { get; set; }
        public Int32 ParticipantId { get; set; }
        public Int32 EventureOrderId { get; set; }
        public Int32 TeamId { get; set; }

        //navigation
        public virtual EventureOrder EventureOrder { get; set; }
    }
}
