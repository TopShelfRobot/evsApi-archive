using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace evs.Model
{
    public class Question
    {
        public Int32 Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string Options { get; set; }
        public Boolean Active { get; set; }
        public Int32 EventureListId { get; set; }
        public Int32 Order { get; set; }
        public bool IsRequired { get; set; }
        //public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        public ICollection<QuestionOption> QuestionOptions { get; set; }
    }

    public class Answer
    {
        public Int32 Id { get; set; }
        public string AnswerText { get; set; }
        public Int32 QuestionId { get; set; }
        public Int32 RegistrationId { get; set; }
        public virtual Registration Registration { get; set; }
        public virtual Question Question { get; set; }  
    }

    public class QuestionOption
    {
        public Int32 Id { get; set; }
        public string OptionText { get; set; }
        public Int32 QuestionId { get; set; }
        //public virtual Question Question { get; set; }
    }

    //deprecated -- only for backwards compatibility
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

    //deprecated -- only for backwards compatibility
    public class StockQuestionSet
    {
        public Int32 Id { get; set; }
        public Int32 EventureListId { get; set; }

        public Boolean ShirtSize { get; set; }
        public Boolean ShowKidsSizes { get; set; }
        public Boolean ShowAdultSizes { get; set; }
        public Boolean FinishTime { get; set; }
        public Boolean BibName { get; set; }
        public Boolean HowHear { get; set; }
        public Boolean Usat { get; set; }

        public Boolean AnnualIncome { get; set; }

        public Boolean School { get; set; }
        public Boolean HowHearDropDown { get; set; }
        public Boolean Notes { get; set; }
        public Boolean EstimatedSwimTime400 { get; set; }
        public Boolean EstimatedSwimTime { get; set; }

        public Boolean RelayTeamQuestion { get; set; }
        public Boolean TitanCategory { get; set; }
        public Boolean TitanNewsletter { get; set; }
        public Boolean TitanTraining { get; set; }

        public Boolean ShirtUpgrade { get; set; }

        //
        public Boolean Wheelchair { get; set; }
        public Boolean PuretapUnisex { get; set; }
        public Boolean NortonUnisex { get; set; }
        public Boolean BourbonGenderSpecific { get; set; }
        public Boolean HearRunathon { get; set; }
        public Boolean HearPure { get; set; }
        public Boolean HearNorton { get; set; }
        public Boolean HearBourbon { get; set; }
        public Boolean ParticipatePure { get; set; }
        public Boolean ParticipateNorton { get; set; }
        public Boolean ParticipateBourbon { get; set; }
        public Boolean Mile15 { get; set; }
        public Boolean SportsEmails { get; set; }
        public Boolean BourbonWaiver { get; set; }

        //titan

    }
}
