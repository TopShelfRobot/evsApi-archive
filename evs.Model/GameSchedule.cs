
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Model
{
    public class GameSchedule
    {
        public Int32 Id { get; set; }
        
        public ICollection<Game> Games { get; set; }
    }
}
