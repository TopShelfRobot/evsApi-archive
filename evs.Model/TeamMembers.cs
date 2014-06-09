using System;

namespace evs.Model
{
    public class TeamMember
    {
        public Int32 Id { get; set; }
        public Int32? ParticipantId { get; set; }
        public Int32 TeamId { get; set; }
        public Int32? StockAnswerSetId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public virtual Team Team { get; set; }
    }
}
