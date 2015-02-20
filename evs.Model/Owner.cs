using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace evs.Model
{
    public class Owner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }
        //public Int32 Id { get; set; }
        //[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OwnerGuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string TermsText { get; set; }
        public string RefundsText { get; set; }
        public string LogoImageName { get; set; }
        public string Url { get; set; }

        public decimal CardProcessorFeePercentPerCharge { get; set; }
        public Int32 CardProcessorFeeFlatPerChargeInCents { get; set; }
        public decimal LocalFeePercentOfCharge { get; set; }
        public Int32 LocalFeeFlatPerChargeInCents { get; set; }   //fix this in 3.0 (perper)  //done

        //stripe
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string StripePublishableKey { get; set; }
        public string StripeUserId { get; set; }
        public Boolean? Livemode { get; set; }
        public string Scope { get; set; }

        //cart rules
        public Boolean IsMultiRegistrationDiscountCartRule { get; set; }
        public decimal MultiRegistrationDiscountAmount { get; set; }
        public AmountType MultiRegistrationDiscountAmountType { get; set; }

        public Boolean IsMultiParticipantDiscountCartRule { get; set; }
        public decimal MultiParticipantDiscountAmount { get; set; }
        public AmountType MultiParticipantDiscountAmountType { get; set; }

        public Boolean IsDuplicateOrderAllowed { get; set; }

        public bool IsAddSingleFeeForAllRegs { get; set; }
        public String AddSingleFeeType { get; set; }
        public decimal AddSingleFeeForAllRegsPercent { get; set; }
        public decimal AddSingleFeeForAllRegsFlat { get; set; }

        //email setting
        public string SendMailEmailAddress { get; set; }
        public string SendImageHtml { get; set; }
        public string SendConfirmEmailSubject { get; set; }
        public string SendConfirmTeamEmailSubject { get; set; }
        public string SendConfirmTeamInviteEmailSubject { get; set; }

        //reg customization
        public string EventureName { get; set; }
        public string ListingName { get; set; }
        public string GroupName { get; set; }
        public string ListStatement { get; set; }
        public string ParticipantButtonText { get; set; }
        public string StripeOrderDescription { get; set; }
        public string StripeCheckoutButtonText { get; set; }
        public string RegisterButtonText { get; set; }
        public string ConfirmButtonText { get; set; }

        public bool IsRegistrationOnProfile { get; set; }
        public bool IsTeamRegistrationOnProfile { get; set; }
        public bool IsParticipantOnProfile { get; set; }
        public bool IsCaptainOnProfile { get; set; }

        public string FacebookAppId { get; set; }
        public string FacebookAppSecretId { get; set; }

        public string MainColor { get; set; }
        public string HoverColor { get; set; }
        public string HighlightColor { get; set; }
        public string NavTextColor { get; set; }

        public string SupportPhone { get; set; }
        public string SupportEmail { get; set; }
    }

    //public class PhotoViewModel
    //{
    //    public string Name { get; set; }
    //    public DateTime Created { get; set; }
    //    public DateTime Modified { get; set; }
    //    public long Size { get; set; }

    //}

    public class EventureClient
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public Int32 OwnerId { get; set; }
    }

    //public class User
    //{

    //    public Int32 Id { get; set; }
    //    public string Email { get; set; }
    //    public string AccessType { get; set; }
    //}

    public class Test
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
    }
}
