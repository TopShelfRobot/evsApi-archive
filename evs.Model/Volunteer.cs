using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Model
{
    class Volunteer
    {
        public Int32 Id { get; set; }
        public string JobName { get; set; }          
        public DateTime DateCreated { get; set; }
        public Int32 ParticipantId { get; set; }
        public Int32 EventureId { get; set; }
    }
}
