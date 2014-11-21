using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace evs.Model
{
    public class Report
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string ReportUrl { get; set; }
        public string IconImageName { get; set; }
        public Int32 OwnerId { get; set; }
        public Boolean Active { get; set; }
        //public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
    }
}
