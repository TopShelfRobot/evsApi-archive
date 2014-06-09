using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Model
{
    public class QAnswer
    {
        public Int32 Id { get; set; }
        public string Value { get; set; }
        public Int32 QQuestionId { get; set; }
    }
}
