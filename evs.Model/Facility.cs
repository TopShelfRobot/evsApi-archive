using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Model
{
    public class Facility
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }

        public string TempTest { get; set; }

    

        //blackout times
    }

    public class Game
    {
        public Int32 Id { get; set; }

        public Int32 HomeScore { get; set; }
        public Int32 AwayScore { get; set; }

        public DateTime Date { get; set; }    //these should link to DateID
        public DateTime Time { get; set; }    //and time id

        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        //public Int32 FaciltyId { get; set; }

        public Facility Facility { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public Player Player { get; set; }
        public EventureList EventureList { get; set; }   //don't need go through schedule??
    }

    public class Player
    {
        public Int32 Id { get; set; }
        public string Notes { get; set; }
        public string AgeGroup { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }

        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        //public Participant Participant { get; set; }
    }

    public class Roster
    {
        public Int32 Id { get; set; }

        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        public ICollection<Player> Players { get; set; }
        public Team Team { get; set; }


    }

    public class GameSchedule
    {
        public Int32 Id { get; set; }

        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        public ICollection<Game> Games { get; set; }
    }
}
