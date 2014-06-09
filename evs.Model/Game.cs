using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Model
{
    public class Game
    {
        public Int32 Id { get; set; }

        public Int32 HomeScore { get; set; }
        public Int32 AwayScore { get; set; }

        public DateTime Date { get; set; }    //these should link to DateID
        public DateTime Time { get; set; }    //and time id

        //public Int32 FaciltyId { get; set; }

        public Facility Facility { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public Player Player { get; set; } 
        public EventureList EventureList { get; set; }   //don't need go through schedule??
    }
}
 