using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace evs.Model
{
    
    public class EventureGroup
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public Boolean Active { get; set; }
        public Int32 Capacity { get; set; }
        public Int32 SortOrder { get; set; }
        public Int32 EventureListId { get; set; }
        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
    
        //public virtual EventureList EventureList { get; set; }
        public ICollection<Registration> Registrations { get; set; }
    }
}
