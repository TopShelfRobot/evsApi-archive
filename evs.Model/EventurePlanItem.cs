using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace evs.Model
{
    public class EventurePlanItem
    {
        public Int32 Id { get; set; }
        public string Task { get; set; }
        //public string Resource { get; set; }
        public Int32 ResourceId { get; set; }
        public DateTime DateDue { get; set; }
        public Int32 EventureId { get; set; }
        public Boolean IsCompleted { get; set; }

        //[ForeignKey("ResourceId")]
        public virtual Resource Resource { get; set; }

    }
}
