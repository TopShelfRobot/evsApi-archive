using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace evs.Model
{
    
    public class InvoiceList
    {
        public Int32 Id { get; set; }
        public Int32 InvoiceId { get; set; }
    
        public virtual Invoice Invoice { get; set; }
    }
}
