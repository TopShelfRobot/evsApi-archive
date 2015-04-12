namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class team_eventureListId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TeamMemberPayment", "EventureListId", c => c.Int(nullable: false));
            CreateIndex("dbo.TeamMember", "ParticipantId");
            AddForeignKey("dbo.TeamMember", "ParticipantId", "dbo.Participant", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamMember", "ParticipantId", "dbo.Participant");
            DropIndex("dbo.TeamMember", new[] { "ParticipantId" });
            DropColumn("dbo.TeamMemberPayment", "EventureListId");
        }
    }
}
