using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

namespace evs.Model
{
    public class Team
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string Coach { get; set; }
        //public EventureList EventureList { get; set; }
        //public Boolean IsBye { get; set; }
        public Int32 RegistrationId { get; set; }

        public ICollection<TeamMember> TeamMembers { get; set; }
        public virtual Registration Registration { get; set; }


    }
}
