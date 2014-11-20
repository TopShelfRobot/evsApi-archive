﻿using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;
using evs.DAL.Migrations;
using evs.Model;

namespace evs.DAL
{
    public class evsContext : DbContext
        //public TourismContext() : base("name=TourismContext")
    {
        static evsContext()
            //: base("evsContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<evsContext, Configuration>());
            //Database.SetInitializer(new DropCreateDatabaseAlways<evsContext>());
        }

        public DbSet<EventureList> EventureLists { get; set; }
        public DbSet<Eventure> Eventures { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<EventureGroup> EventureGroups { get; set; }
        //public DbSet<EventureListType> EventureListTypes { get; set; }


        public DbSet<Registration> Registrations { get; set; }
        public DbSet<EventureOrder> Orders { get; set; }
       
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Addon> Addons { get; set; }
        public DbSet<Surcharge> Surcharges { get; set; }
        public DbSet<FeeSchedule> FeeSchedules { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<StockAnswerSet> StockAnswerSets { get; set; }
        public DbSet<StockQuestionSet> StockQuestionSets { get; set; }
        public DbSet<Report> Reports { get; set; }

        public DbSet<Answer> Answer { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<QuestionOption> QuestionOption { get; set; }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceItemCategory> ResourceItemCategories { get; set; }
        public DbSet<ResourceItem> ResourceItems { get; set; }
        public DbSet<EventurePlanItem> PlanItems { get; set; }
        public DbSet<EventureExpense> Expenses { get; set; }

        public DbSet<EventureClient> EventureClients { get; set; }
        public DbSet<EventureService> EventureServices { get; set; }

        public DbSet<Dc_Part> DcParts { get; set; }
        public DbSet<Dc_Reg> DcRegs { get; set; }
        public DbSet<Dc_StagePart> DcStageParts { get; set; }
        public DbSet<Dc_Order> DcOrders { get; set; }
        public DbSet<Dc_StateLookup> DcStateLookups { get; set; }

        public DbSet<EventureLog> EventureLogs { get; set; }
        public DbSet<EventureTransfer> EventureTransfers { get; set; }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Roster> Rosters { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TeamMemberPayment> TeamMemberPayments { get; set; }
        public DbSet<Facility> Facilitys { get; set; }  //spell
        public DbSet<GameSchedule> GameSchedules { get; set; }

        //these should all end in s
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<VolunteerSchedule> VolunteerSchedules { get; set; }  //mjb
        public DbSet<VolunteerJob> VolunteerJobs { get; set; }
        public DbSet<VolunteerShift> VolunteerShifts { get; set; }

        public DbSet<EventListCharge> EventListCharges { get; set; }
        //public DbSet<SurchargeType> SurchargeTypes { get; set; }


        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // modelBuilder.Entity<ResourceItem>()
            //.HasRequired(d => d.Week)
            //.WithMany(w => w.Days)
            //.WillCascadeOnDelete(false);


            //modelBuilder.Configurations.Add(new OrderMap());
            //modelBuilder.Configurations.Add(new ProductMap());

            //modelBuilder.Entity<QResult>()
            //    .HasRequired(c => c.Registration)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<QResult>()
            //    .HasRequired(s => s.Answer)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);
        }
    }
}
