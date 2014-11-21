using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace evs.Model
{
    public class EventureLog
    {
        public Int32 Id { get; set; }
        public string  Message { get; set; }
        public string  Caller { get; set; }
        public string Status { get; set; }
        public DateTime LogDate { get; set; }
        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
    }
}
