using System;
//using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace evs.Model
{
    public class Owner
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OwnerGuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string TermsText { get; set; }
        public string RefundsText { get; set; }
        public string LogoImageName { get; set; }

        public decimal CardProcessorFeePercentPerCharge { get; set; }
        public Int32 CardProcessorFeeFlatPerChargeInCents { get; set; }
        public decimal LocalFeePercentOfCharge { get; set; }
        public Int32 LocalFeeFlatPerPerChargeInCents { get; set; }   //fix this in 3.0 (perper)
  
        //public Boolean StripeConnectIsActive { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string StripePublishableKey { get; set; }
        public string StripeUserId { get; set; }
        public Boolean? Livemode { get; set; }
        public string Scope { get; set; }

        public bool IsMultiDiscountCartRule { get; set; }
        public bool IsAllFourCartRule { get; set; }

        public bool IsAddSingleFeeForAllRegs { get; set; }
        public string AddSingleFeeType { get; set; }
        public decimal AddSingleFeeForAllRegsPercent { get; set; }
        public decimal AddSingleFeeForAllRegsFlat { get; set; }

        public bool IsEnterpriseDisplayedOnMenu { get; set; }
        public bool IsEventureDisplayedOnMenu { get; set; }
        public bool IsPartDisplayedOnMenu { get; set; }
        public bool IsCouponDisplayedOnMenu { get; set; }
        public bool IsResourceDisplayedOnMenu { get; set; }
        public bool IsReportingDisplayedOnMenu { get; set; }

        public string SendMailEmailAddress { get; set; }
        public string SendImageHtml { get; set; }
        public string SendConfirmEmailSubject { get; set; }

        public string EventureName { get; set; }
        public string ListingName { get; set; }
        public string GroupName { get; set; }
        public string ListStatement { get; set; }
        public string PartButtonText { get; set; }

        public bool IsHeadfirstQuestionDisplayed { get; set; }
        public bool IsSportsCommQuestionDisplayed { get; set; }

        public string FacebookAppId { get; set; }
        public string FacebookAppSecretId { get; set; }
    }
}
