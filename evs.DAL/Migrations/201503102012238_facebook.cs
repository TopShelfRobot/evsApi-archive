namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class facebook : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventureOrder", "OrderTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Owner", "OwnerGuid", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.Team", "TeamGuid", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.TeamMember", "TeamMemberGuid", c => c.Guid(nullable: false, identity: true));
            DropColumn("dbo.EventureOrder", "OrderType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EventureOrder", "OrderType", c => c.Int(nullable: false));
            AlterColumn("dbo.TeamMember", "TeamMemberGuid", c => c.Guid(nullable: false));
            AlterColumn("dbo.Team", "TeamGuid", c => c.Guid(nullable: false));
            AlterColumn("dbo.Owner", "OwnerGuid", c => c.Guid(nullable: false));
            DropColumn("dbo.EventureOrder", "OrderTypeId");
        }
    }
}
