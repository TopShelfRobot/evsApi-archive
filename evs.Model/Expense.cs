using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Model
{
    public class EventureExpense
    {
        public Int32 Id { get; set; }
        public Int32 ResourceItemId { get; set; }
        public Int32 ResourceItemCategoryId { get; set; }
        public decimal Cost { get; set; }
        public string CostType { get; set; }
        public Int32 PerRegNumber { get; set; }
        public Int32 EventureId { get; set; }
        public virtual ResourceItem ResourceItem { get; set; }
        public virtual ResourceItemCategory ResourceItemCategory { get; set; }
    }
}
