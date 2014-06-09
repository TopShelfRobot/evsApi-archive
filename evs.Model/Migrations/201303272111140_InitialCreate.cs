namespace evs.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Desc = c.String(),
                        dateBeginListing = c.DateTime(nullable: false),
                        dateEndListing = c.DateTime(nullable: false),
                        EventId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Capacity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Desc = c.String(),
                        dateBeginReg = c.DateTime(nullable: false),
                        dateEndReg = c.String(),
                        dateEvent = c.String(),
                        Active = c.String(),
                        isUSAT = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.EventLists", new[] { "EventId" });
            DropForeignKey("dbo.EventLists", "EventId", "dbo.Events");
            DropTable("dbo.Events");
            DropTable("dbo.EventLists");
        }
    }
}
