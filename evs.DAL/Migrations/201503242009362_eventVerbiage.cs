namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eventVerbiage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Owner", "EventStatement", c => c.String());
            DropColumn("dbo.Eventure", "Desc");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Eventure", "Desc", c => c.String());
            DropColumn("dbo.Owner", "EventStatement");
        }
    }
}
