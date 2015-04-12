namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbGenOption : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.EventureOrder", "HouseId", "dbo.Participant");
            DropForeignKey("dbo.Surcharge", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.Registration", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.Team", "CoachId", "dbo.Participant");
            DropForeignKey("dbo.TeamMember", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.Volunteer", "ParticipantId", "dbo.Participant");
            DropPrimaryKey("dbo.Participant");
            AlterColumn("dbo.Participant", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Participant", "Id");
            AddForeignKey("dbo.EventureOrder", "HouseId", "dbo.Participant", "Id");
            AddForeignKey("dbo.Surcharge", "ParticipantId", "dbo.Participant", "Id");
            AddForeignKey("dbo.Registration", "ParticipantId", "dbo.Participant", "Id");
            AddForeignKey("dbo.Team", "CoachId", "dbo.Participant", "Id");
            AddForeignKey("dbo.TeamMember", "ParticipantId", "dbo.Participant", "Id");
            AddForeignKey("dbo.Volunteer", "ParticipantId", "dbo.Participant", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Volunteer", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.TeamMember", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.Team", "CoachId", "dbo.Participant");
            DropForeignKey("dbo.Registration", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.Surcharge", "ParticipantId", "dbo.Participant");
            DropForeignKey("dbo.EventureOrder", "HouseId", "dbo.Participant");
            DropPrimaryKey("dbo.Participant");
            AlterColumn("dbo.Participant", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Participant", "Id");
            AddForeignKey("dbo.Volunteer", "ParticipantId", "dbo.Participant", "Id");
            AddForeignKey("dbo.TeamMember", "ParticipantId", "dbo.Participant", "Id");
            AddForeignKey("dbo.Team", "CoachId", "dbo.Participant", "Id");
            AddForeignKey("dbo.Registration", "ParticipantId", "dbo.Participant", "Id");
            AddForeignKey("dbo.Surcharge", "ParticipantId", "dbo.Participant", "Id");
            AddForeignKey("dbo.EventureOrder", "HouseId", "dbo.Participant", "Id");
        }
    }
}
