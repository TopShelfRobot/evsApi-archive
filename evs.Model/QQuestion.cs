using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Model
{
    public class QQuestion
    {
        public Int32 Id { get; set; }
        public string Text { get; set; }
        public Int32 EventureListId { get; set; }
        public EventureList EventureList { get; set; }

        public ICollection<QAnswer> Answers { get; set; }
    }
}
