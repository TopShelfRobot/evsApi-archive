using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace evs.Model
{
    public class Invoice
    {
        public Invoice()
        {
            this.InvoiceDetails = new HashSet<InvoiceList>();
        }
    
        public Int32 Id { get; set; }
    
        //public virtual Registration Registration { get; set; }
        public virtual ICollection<InvoiceList> InvoiceDetails { get; set; }
    }

    public class InvoiceList
    {
        public Int32 Id { get; set; }
        public Int32 InvoiceId { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
