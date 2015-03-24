namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addonCleanup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addon", "Name", c => c.String());
            AddColumn("dbo.Addon", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Surcharge", "Name", c => c.String());
            CreateIndex("dbo.Addon", "EventureId");
            AddForeignKey("dbo.Addon", "EventureId", "dbo.Eventure", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addon", "EventureId", "dbo.Eventure");
            DropIndex("dbo.Addon", new[] { "EventureId" });
            DropColumn("dbo.Surcharge", "Name");
            DropColumn("dbo.Addon", "DateCreated");
            DropColumn("dbo.Addon", "Name");
        }
    }
}
