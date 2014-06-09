using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace evs.Model
{
    public class QResult
    {
        public Int32 Id { get; set; }
        public Int32 QQuestionId { get; set; }
        public Int32 QAnswerId { get; set; }
        public Int32 RegistrationId { get; set; }

        public QQuestion Question { get; set; }
        public QAnswer Answer { get; set; }
        public Registration Registration { get; set; }
    }
        
}
