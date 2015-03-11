namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Eventure", "OwnerId", "dbo.Owner");
            DropForeignKey("dbo.Team", "EventureList_Id", "dbo.EventureList");
            DropForeignKey("dbo.Player", "Participant_Id", "dbo.Participant");
            DropIndex("dbo.Registration", new[] { "StockAnswerSetId" });
            DropIndex("dbo.Eventure", new[] { "OwnerId" });
            DropIndex("dbo.Team", new[] { "EventureList_Id" });
            DropIndex("dbo.Player", new[] { "Participant_Id" });
            DropPrimaryKey("dbo.Client");
            CreateTable(
                "dbo.Answer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AnswerText = c.String(),
                        QuestionId = c.Int(nullable: false),
                        RegistrationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Question", t => t.QuestionId)
                .ForeignKey("dbo.Registration", t => t.RegistrationId)
                .Index(t => t.QuestionId)
                .Index(t => t.RegistrationId);
            
            CreateTable(
                "dbo.Question",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionText = c.String(),
                        QuestionType = c.String(),
                        Options = c.String(),
                        Active = c.Boolean(nullable: false),
                        EventureListId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        IsRequired = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuestionOption",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OptionText = c.String(),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Question", t => t.QuestionId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.EventureListBundle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EvenureListId = c.Int(nullable: false),
                        EventureList_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventureList", t => t.EventureList_Id)
                .Index(t => t.EventureList_Id);
            
            CreateTable(
                "dbo.RegistrationPost",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RegistrationId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SurchargeId = c.Int(),
                        OwnerId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        TransactionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Registration", t => t.RegistrationId)
                .Index(t => t.RegistrationId);
            
            CreateTable(
                "dbo.Employee",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 100),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        OwnerId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedById = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventureClient",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Street = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zip = c.String(),
                        ContactName = c.String(),
                        ContactPhone = c.String(),
                        ContactEmail = c.String(),
                        OwnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeamMemberPayment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TeamMemberId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        EventureOrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EventureOrder", t => t.EventureOrderId)
                .ForeignKey("dbo.Team", t => t.TeamId)
                .ForeignKey("dbo.TeamMember", t => t.TeamMemberId)
                .Index(t => t.TeamId)
                .Index(t => t.TeamMemberId)
                .Index(t => t.EventureOrderId);
            
            CreateTable(
                "dbo.TeamMember",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamMemberGuid = c.Guid(nullable: false),
                        ParticipantId = c.Int(),
                        TeamId = c.Int(nullable: false),
                        Name = c.String(),
                        Email = c.String(),
                        Active = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        Position = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Team", t => t.TeamId)
                .Index(t => t.TeamId);
            
            CreateTable(
                "dbo.RefreshToken",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 50),
                        ClientId = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Refund",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EventureOrderId = c.Int(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        RegistrationId = c.Int(),
                        AmountRefunded = c.Int(),
                        BalanceTransaction = c.Int(nullable: false),
                        BalanceTransactionId = c.String(),
                        CustomerId = c.String(),
                        FailureMessage = c.String(),
                        Paid = c.Boolean(),
                        ReceiptEmail = c.String(),
                        Refunded = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Test",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transaction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        EventureOrderId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserAgent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParticipantId = c.Int(nullable: false),
                        BrowserCodeName = c.String(maxLength: 50),
                        BrowserName = c.String(maxLength: 50),
                        BrowserVersion = c.String(maxLength: 150),
                        CookiesEnabled = c.Boolean(nullable: false),
                        Platform = c.String(maxLength: 25),
                        Header = c.String(maxLength: 250),
                        SystemLanguage = c.String(maxLength: 50),
                        Ip = c.String(maxLength: 50),
                        Hostname = c.String(maxLength: 50),
                        Latitude = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Longitude = c.Decimal(nullable: false, precision: 18, scale: 2),
                        City = c.String(maxLength: 50),
                        Region = c.String(maxLength: 50),
                        Country = c.String(maxLength: 50),
                        Phone = c.String(maxLength: 50),
                        LoginAction = c.String(maxLength: 15),
                        OwnerId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Client", "Secret", c => c.String(nullable: false));
            AddColumn("dbo.Client", "ApplicationType", c => c.Int(nullable: false));
            AddColumn("dbo.Client", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Client", "RefreshTokenLifeTime", c => c.Int(nullable: false));
            AddColumn("dbo.Client", "AllowedOrigin", c => c.String(maxLength: 100));
            AddColumn("dbo.Coupon", "AmountType", c => c.Int(nullable: false));
            AddColumn("dbo.Coupon", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.EventureGroup", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.EventureList", "IsBundle", c => c.Boolean(nullable: false));
            AddColumn("dbo.EventureList", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.EventureList", "PaymentTerms", c => c.String());
            AddColumn("dbo.EventureList", "IsGroupRequired", c => c.Boolean(nullable: false));
            AddColumn("dbo.EventureList", "EventureListType", c => c.Int(nullable: false));
            AddColumn("dbo.EventureOrder", "PaymentTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.EventureOrder", "OrderType", c => c.Int(nullable: false));
            AddColumn("dbo.Surcharge", "SurchargeType", c => c.Int(nullable: false));
            AddColumn("dbo.Participant", "Position", c => c.String());
            AddColumn("dbo.Participant", "ShirtSize", c => c.String());
            AddColumn("dbo.EventureLog", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Eventure", "IsTeam", c => c.Boolean(nullable: false));
            AddColumn("dbo.Eventure", "Location", c => c.String());
            AddColumn("dbo.Eventure", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Owner", "Url", c => c.String());
            AddColumn("dbo.Owner", "LocalFeeFlatPerChargeInCents", c => c.Int(nullable: false));
            AddColumn("dbo.Owner", "IsMultiRegistrationDiscountCartRule", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "MultiRegistrationDiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Owner", "MultiRegistrationDiscountAmountType", c => c.Int(nullable: false));
            AddColumn("dbo.Owner", "IsMultiParticipantDiscountCartRule", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "MultiParticipantDiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Owner", "MultiParticipantDiscountAmountType", c => c.Int(nullable: false));
            AddColumn("dbo.Owner", "SendConfirmTeamEmailSubject", c => c.String());
            AddColumn("dbo.Owner", "SendConfirmTeamInviteEmailSubject", c => c.String());
            AddColumn("dbo.Owner", "ParticipantButtonText", c => c.String());
            AddColumn("dbo.Owner", "StripeOrderDescription", c => c.String());
            AddColumn("dbo.Owner", "StripeCheckoutButtonText", c => c.String());
            AddColumn("dbo.Owner", "RegisterButtonText", c => c.String());
            AddColumn("dbo.Owner", "ConfirmButtonText", c => c.String());
            AddColumn("dbo.Owner", "IsRegistrationOnProfile", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsTeamRegistrationOnProfile", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsParticipantOnProfile", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsCaptainOnProfile", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "MainColor", c => c.String());
            AddColumn("dbo.Owner", "HoverColor", c => c.String());
            AddColumn("dbo.Owner", "HighlightColor", c => c.String());
            AddColumn("dbo.Owner", "NavTextColor", c => c.String());
            AddColumn("dbo.Owner", "SupportPhone", c => c.String());
            AddColumn("dbo.Owner", "SupportEmail", c => c.String());
            AddColumn("dbo.EventureService", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.EventureExpense", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.ResourceItem", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Resource", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.ResourceItemCategory", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.FeeSchedule", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Game", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Team", "TeamGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.Team", "CoachId", c => c.Int(nullable: false));
            AddColumn("dbo.Team", "RegistrationId", c => c.Int(nullable: false));
            AddColumn("dbo.Team", "IsPaidInFull", c => c.Boolean(nullable: false));
            AddColumn("dbo.Team", "OwnerId", c => c.Int(nullable: false));
            AddColumn("dbo.Team", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Team", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Team", "Division", c => c.String());
            AddColumn("dbo.Team", "TimeFinish", c => c.String());
            AddColumn("dbo.Player", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.GameSchedule", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.EventurePlanItem", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Roster", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.VolunteerSchedule", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.VolunteerShift", "Active", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Client", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Client", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Registration", "StockAnswerSetId", c => c.Int());
            AlterColumn("dbo.EventureOrder", "PaymentType", c => c.Int(nullable: false));
            AlterColumn("dbo.Owner", "OwnerGuid", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Client", "Id");
            CreateIndex("dbo.Registration", "GroupId");
            CreateIndex("dbo.Registration", "StockAnswerSetId");
            CreateIndex("dbo.EventureOrder", "HouseId");
            CreateIndex("dbo.EventureOrder", "OwnerId");
            CreateIndex("dbo.Surcharge", "ParticipantId");
            CreateIndex("dbo.Team", "CoachId");
            CreateIndex("dbo.Team", "RegistrationId");
            CreateIndex("dbo.Team", "OwnerId");
            CreateIndex("dbo.EventureTransfer", "EventureOrderId");
            AddForeignKey("dbo.Registration", "GroupId", "dbo.EventureGroup", "Id");
            AddForeignKey("dbo.EventureOrder", "HouseId", "dbo.Participant", "Id");
            AddForeignKey("dbo.EventureOrder", "OwnerId", "dbo.Owner", "Id");
            AddForeignKey("dbo.Surcharge", "ParticipantId", "dbo.Participant", "Id");
            AddForeignKey("dbo.Team", "CoachId", "dbo.Participant", "Id");
            AddForeignKey("dbo.Team", "OwnerId", "dbo.Owner", "Id");
            AddForeignKey("dbo.Team", "RegistrationId", "dbo.Registration", "Id");
            AddForeignKey("dbo.EventureTransfer", "EventureOrderId", "dbo.EventureOrder", "Id");
            DropColumn("dbo.Addon", "IsSiblingDiscount");
            DropColumn("dbo.Client", "Street");
            DropColumn("dbo.Client", "City");
            DropColumn("dbo.Client", "State");
            DropColumn("dbo.Client", "Zip");
            DropColumn("dbo.Client", "ContactName");
            DropColumn("dbo.Client", "ContactPhone");
            DropColumn("dbo.Client", "ContactEmail");
            DropColumn("dbo.Client", "OwnerId");
            DropColumn("dbo.Coupon", "DiscountType");
            DropColumn("dbo.EventureList", "IsGroup2");
            DropColumn("dbo.Registration", "Name");
            DropColumn("dbo.EventureOrder", "Notes");
            DropColumn("dbo.EventureOrder", "GiftCardAmount");
            DropColumn("dbo.StockAnswerSet", "Overnight");
            DropColumn("dbo.StockAnswerSet", "OvernightWhere");
            DropColumn("dbo.StockAnswerSet", "OvernightDays");
            DropColumn("dbo.StockAnswerSet", "ReferralName");
            DropColumn("dbo.Eventure", "IsUsingVolunteers");
            DropColumn("dbo.Eventure", "VolunteerWaiver");
            DropColumn("dbo.Owner", "LocalFeeFlatPerPerChargeInCents");
            DropColumn("dbo.Owner", "IsMultiDiscountCartRule");
            DropColumn("dbo.Owner", "IsAllFourCartRule");
            DropColumn("dbo.Owner", "IsVolunteerDisplayedOnReg");
            DropColumn("dbo.Owner", "IsEnterpriseDisplayedOnMenu");
            DropColumn("dbo.Owner", "IsEventureDisplayedOnMenu");
            DropColumn("dbo.Owner", "IsPartDisplayedOnMenu");
            DropColumn("dbo.Owner", "IsCouponDisplayedOnMenu");
            DropColumn("dbo.Owner", "IsResourceDisplayedOnMenu");
            DropColumn("dbo.Owner", "IsReportingDisplayedOnMenu");
            DropColumn("dbo.Owner", "PartButtonText");
            DropColumn("dbo.Owner", "IsHeadfirstQuestionDisplayed");
            DropColumn("dbo.Owner", "IsSportsCommQuestionDisplayed");
            DropColumn("dbo.Team", "Coach");
            DropColumn("dbo.Team", "IsBye");
            DropColumn("dbo.Team", "EventureList_Id");
            DropColumn("dbo.Player", "Participant_Id");
            DropColumn("dbo.StockQuestionSet", "ShowGenderSpecificSizes");
            DropColumn("dbo.StockQuestionSet", "Overnight");
            DropColumn("dbo.StockQuestionSet", "OvernightWhere");
            DropColumn("dbo.StockQuestionSet", "OvernightDays");
            DropColumn("dbo.StockQuestionSet", "ReferralName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StockQuestionSet", "ReferralName", c => c.Boolean(nullable: false));
            AddColumn("dbo.StockQuestionSet", "OvernightDays", c => c.Boolean(nullable: false));
            AddColumn("dbo.StockQuestionSet", "OvernightWhere", c => c.Boolean(nullable: false));
            AddColumn("dbo.StockQuestionSet", "Overnight", c => c.Boolean(nullable: false));
            AddColumn("dbo.StockQuestionSet", "ShowGenderSpecificSizes", c => c.Boolean(nullable: false));
            AddColumn("dbo.Player", "Participant_Id", c => c.Int());
            AddColumn("dbo.Team", "EventureList_Id", c => c.Int());
            AddColumn("dbo.Team", "IsBye", c => c.Boolean(nullable: false));
            AddColumn("dbo.Team", "Coach", c => c.String());
            AddColumn("dbo.Owner", "IsSportsCommQuestionDisplayed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsHeadfirstQuestionDisplayed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "PartButtonText", c => c.String());
            AddColumn("dbo.Owner", "IsReportingDisplayedOnMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsResourceDisplayedOnMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsCouponDisplayedOnMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsPartDisplayedOnMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsEventureDisplayedOnMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsEnterpriseDisplayedOnMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsVolunteerDisplayedOnReg", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsAllFourCartRule", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "IsMultiDiscountCartRule", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owner", "LocalFeeFlatPerPerChargeInCents", c => c.Int(nullable: false));
            AddColumn("dbo.Eventure", "VolunteerWaiver", c => c.String());
            AddColumn("dbo.Eventure", "IsUsingVolunteers", c => c.Boolean(nullable: false));
            AddColumn("dbo.StockAnswerSet", "ReferralName", c => c.String());
            AddColumn("dbo.StockAnswerSet", "OvernightDays", c => c.String());
            AddColumn("dbo.StockAnswerSet", "OvernightWhere", c => c.String());
            AddColumn("dbo.StockAnswerSet", "Overnight", c => c.String());
            AddColumn("dbo.EventureOrder", "GiftCardAmount", c => c.Int(nullable: false));
            AddColumn("dbo.EventureOrder", "Notes", c => c.String());
            AddColumn("dbo.Registration", "Name", c => c.String(maxLength: 75));
            AddColumn("dbo.EventureList", "IsGroup2", c => c.Boolean(nullable: false));
            AddColumn("dbo.Coupon", "DiscountType", c => c.Int(nullable: false));
            AddColumn("dbo.Client", "OwnerId", c => c.Int(nullable: false));
            AddColumn("dbo.Client", "ContactEmail", c => c.String());
            AddColumn("dbo.Client", "ContactPhone", c => c.String());
            AddColumn("dbo.Client", "ContactName", c => c.String());
            AddColumn("dbo.Client", "Zip", c => c.String());
            AddColumn("dbo.Client", "State", c => c.String());
            AddColumn("dbo.Client", "City", c => c.String());
            AddColumn("dbo.Client", "Street", c => c.String());
            AddColumn("dbo.Addon", "IsSiblingDiscount", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventureTransfer", "EventureOrderId", "dbo.EventureOrder");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.TeamMemberPayment", "TeamMemberId", "dbo.TeamMember");
            DropForeignKey("dbo.TeamMember", "TeamId", "dbo.Team");
            DropForeignKey("dbo.TeamMemberPayment", "TeamId", "dbo.Team");
            DropForeignKey("dbo.TeamMemberPayment", "EventureOrderId", "dbo.EventureOrder");
            DropForeignKey("dbo.Team", "RegistrationId", "dbo.Registration");
            DropForeignKey("dbo.Team", "OwnerId", "dbo.Owner");
            DropForeignKey("dbo.Team", "CoachId", "dbo.Participant");
            DropForeignKey("dbo.RegistrationPost", "RegistrationId", "dbo.Registration");
            DropForeignKey("dbo.Surcharge", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.EventureOrder", "OwnerId", "dbo.Owner");
            DropForeignKey("dbo.EventureOrder", "HouseId", "dbo.Participant");
            DropForeignKey("dbo.EventureListBundle", "EventureList_Id", "dbo.EventureList");
            DropForeignKey("dbo.Registration", "GroupId", "dbo.EventureGroup");
            DropForeignKey("dbo.Answer", "RegistrationId", "dbo.Registration");
            DropForeignKey("dbo.Answer", "QuestionId", "dbo.Question");
            DropForeignKey("dbo.QuestionOption", "QuestionId", "dbo.Question");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.EventureTransfer", new[] { "EventureOrderId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.TeamMember", new[] { "TeamId" });
            DropIndex("dbo.TeamMemberPayment", new[] { "EventureOrderId" });
            DropIndex("dbo.TeamMemberPayment", new[] { "TeamMemberId" });
            DropIndex("dbo.TeamMemberPayment", new[] { "TeamId" });
            DropIndex("dbo.Team", new[] { "OwnerId" });
            DropIndex("dbo.Team", new[] { "RegistrationId" });
            DropIndex("dbo.Team", new[] { "CoachId" });
            DropIndex("dbo.RegistrationPost", new[] { "RegistrationId" });
            DropIndex("dbo.Surcharge", new[] { "ParticipantId" });
            DropIndex("dbo.EventureOrder", new[] { "OwnerId" });
            DropIndex("dbo.EventureOrder", new[] { "HouseId" });
            DropIndex("dbo.EventureListBundle", new[] { "EventureList_Id" });
            DropIndex("dbo.Registration", new[] { "StockAnswerSetId" });
            DropIndex("dbo.Registration", new[] { "GroupId" });
            DropIndex("dbo.QuestionOption", new[] { "QuestionId" });
            DropIndex("dbo.Answer", new[] { "RegistrationId" });
            DropIndex("dbo.Answer", new[] { "QuestionId" });
            DropPrimaryKey("dbo.Client");
            AlterColumn("dbo.Owner", "OwnerGuid", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.EventureOrder", "PaymentType", c => c.String());
            AlterColumn("dbo.Registration", "StockAnswerSetId", c => c.Int(nullable: false));
            AlterColumn("dbo.Client", "Name", c => c.String());
            AlterColumn("dbo.Client", "Id", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.VolunteerShift", "Active");
            DropColumn("dbo.VolunteerSchedule", "Active");
            DropColumn("dbo.Roster", "DateCreated");
            DropColumn("dbo.EventurePlanItem", "DateCreated");
            DropColumn("dbo.GameSchedule", "DateCreated");
            DropColumn("dbo.Player", "DateCreated");
            DropColumn("dbo.Team", "TimeFinish");
            DropColumn("dbo.Team", "Division");
            DropColumn("dbo.Team", "Active");
            DropColumn("dbo.Team", "DateCreated");
            DropColumn("dbo.Team", "OwnerId");
            DropColumn("dbo.Team", "IsPaidInFull");
            DropColumn("dbo.Team", "RegistrationId");
            DropColumn("dbo.Team", "CoachId");
            DropColumn("dbo.Team", "TeamGuid");
            DropColumn("dbo.Game", "DateCreated");
            DropColumn("dbo.FeeSchedule", "DateCreated");
            DropColumn("dbo.ResourceItemCategory", "DateCreated");
            DropColumn("dbo.Resource", "DateCreated");
            DropColumn("dbo.ResourceItem", "DateCreated");
            DropColumn("dbo.EventureExpense", "DateCreated");
            DropColumn("dbo.EventureService", "DateCreated");
            DropColumn("dbo.Owner", "SupportEmail");
            DropColumn("dbo.Owner", "SupportPhone");
            DropColumn("dbo.Owner", "NavTextColor");
            DropColumn("dbo.Owner", "HighlightColor");
            DropColumn("dbo.Owner", "HoverColor");
            DropColumn("dbo.Owner", "MainColor");
            DropColumn("dbo.Owner", "IsCaptainOnProfile");
            DropColumn("dbo.Owner", "IsParticipantOnProfile");
            DropColumn("dbo.Owner", "IsTeamRegistrationOnProfile");
            DropColumn("dbo.Owner", "IsRegistrationOnProfile");
            DropColumn("dbo.Owner", "ConfirmButtonText");
            DropColumn("dbo.Owner", "RegisterButtonText");
            DropColumn("dbo.Owner", "StripeCheckoutButtonText");
            DropColumn("dbo.Owner", "StripeOrderDescription");
            DropColumn("dbo.Owner", "ParticipantButtonText");
            DropColumn("dbo.Owner", "SendConfirmTeamInviteEmailSubject");
            DropColumn("dbo.Owner", "SendConfirmTeamEmailSubject");
            DropColumn("dbo.Owner", "MultiParticipantDiscountAmountType");
            DropColumn("dbo.Owner", "MultiParticipantDiscountAmount");
            DropColumn("dbo.Owner", "IsMultiParticipantDiscountCartRule");
            DropColumn("dbo.Owner", "MultiRegistrationDiscountAmountType");
            DropColumn("dbo.Owner", "MultiRegistrationDiscountAmount");
            DropColumn("dbo.Owner", "IsMultiRegistrationDiscountCartRule");
            DropColumn("dbo.Owner", "LocalFeeFlatPerChargeInCents");
            DropColumn("dbo.Owner", "Url");
            DropColumn("dbo.Eventure", "DateCreated");
            DropColumn("dbo.Eventure", "Location");
            DropColumn("dbo.Eventure", "IsTeam");
            DropColumn("dbo.EventureLog", "DateCreated");
            DropColumn("dbo.Participant", "ShirtSize");
            DropColumn("dbo.Participant", "Position");
            DropColumn("dbo.Surcharge", "SurchargeType");
            DropColumn("dbo.EventureOrder", "OrderType");
            DropColumn("dbo.EventureOrder", "PaymentTypeId");
            DropColumn("dbo.EventureList", "EventureListType");
            DropColumn("dbo.EventureList", "IsGroupRequired");
            DropColumn("dbo.EventureList", "PaymentTerms");
            DropColumn("dbo.EventureList", "DateCreated");
            DropColumn("dbo.EventureList", "IsBundle");
            DropColumn("dbo.EventureGroup", "DateCreated");
            DropColumn("dbo.Coupon", "DateCreated");
            DropColumn("dbo.Coupon", "AmountType");
            DropColumn("dbo.Client", "AllowedOrigin");
            DropColumn("dbo.Client", "RefreshTokenLifeTime");
            DropColumn("dbo.Client", "Active");
            DropColumn("dbo.Client", "ApplicationType");
            DropColumn("dbo.Client", "Secret");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UserAgent");
            DropTable("dbo.Transaction");
            DropTable("dbo.Test");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Refund");
            DropTable("dbo.RefreshToken");
            DropTable("dbo.TeamMember");
            DropTable("dbo.TeamMemberPayment");
            DropTable("dbo.EventureClient");
            DropTable("dbo.Employee");
            DropTable("dbo.RegistrationPost");
            DropTable("dbo.EventureListBundle");
            DropTable("dbo.QuestionOption");
            DropTable("dbo.Question");
            DropTable("dbo.Answer");
            AddPrimaryKey("dbo.Client", "Id");
            CreateIndex("dbo.Player", "Participant_Id");
            CreateIndex("dbo.Team", "EventureList_Id");
            CreateIndex("dbo.Eventure", "OwnerId");
            CreateIndex("dbo.Registration", "StockAnswerSetId");
            AddForeignKey("dbo.Player", "Participant_Id", "dbo.Participant", "Id");
            AddForeignKey("dbo.Team", "EventureList_Id", "dbo.EventureList", "Id");
            AddForeignKey("dbo.Eventure", "OwnerId", "dbo.Owner", "Id");
        }
    }
}
