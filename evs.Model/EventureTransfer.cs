using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Model
{
    public class EventureTransfer
    {
        public Int32 Id { get; set; }
        public Int32 EventureListIdFrom { get; set; }
        public Int32 EventureListIdTo { get; set; }
        public Decimal Amount { get; set; }
        public Decimal AmountSurcharges { get; set; }
        public Int32 RegistrationId { get; set; }
        public bool IsComplete  { get; set; }
        public string Message { get; set; }
        public Decimal AmountTotal { get; set; }
        public string ParticipantId { get; set; }
        public Int32? EventureOrderId { get; set; }
        public Int32? StockAnswerSetId { get; set; }
        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
    }

}
