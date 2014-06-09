using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace evs.Model
{
    public class Roster
    {
        public Int32 Id { get; set; }

        public ICollection<Player> Players { get; set; }
        public Team Team { get; set; }
        
        
    }
}
