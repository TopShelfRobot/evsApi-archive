namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventureList", "IsUSAT", c => c.Boolean(nullable: false));
            AddColumn("dbo.Surcharge", "AddonId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Surcharge", "AddonId");
            DropColumn("dbo.EventureList", "IsUSAT");
        }
    }
}
