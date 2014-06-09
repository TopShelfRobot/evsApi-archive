using System;

namespace evs.Model
{
    public class StockAnswerSet
    {
        public Int32 Id { get; set; }
        public Int32 StockQuestionSetId { get; set; }
        public string ShirtSize { get; set; }
        public string FinishTime { get; set; }
        public string BibName { get; set; }
        public string HowHear { get; set; }
        public string Usat { get; set; }
        public string School { get; set; }
        public string HowHearDropDown { get; set; }
        public string Notes { get; set; }
        public string EstimatedSwimTime400 { get; set; }
        public string EstimatedSwimTime { get; set; }
        public string AnnualIncome { get; set; }
        public string RelayTeamQuestion { get; set; }

        public string Wheelchair { get; set; }
        public string PuretapUnisex { get; set; }
        public string NortonUnisex { get; set; }
        public string BourbonGenderSpecific { get; set; }
        public string HearRunathon { get; set; }
        public string HearPure { get; set; }
        public string HearNorton { get; set; }
        public string HearBourbon { get; set; }
        public string ParticipatePure { get; set; }
        public string ParticipateNorton { get; set; }
        public string ParticipateBourbon { get; set; }
        public string Mile15 { get; set; }
        public string SportsEmails { get; set; }
        
        //public string OwnRv { get; set; }
        //public string NextRv { get; set; }

        public String TitanCategory { get; set; }
        public Boolean TitanNewsletter { get; set; }
        public Boolean TitanTraining { get; set; }

        public string ShirtUpgrade { get; set; }

        public Boolean BourbonWaiver { get; set; }
    
    }
}
