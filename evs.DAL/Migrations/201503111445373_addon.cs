namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addon", "EventureId", c => c.Int());
            AddColumn("dbo.Addon", "ImagePath", c => c.String());
            DropColumn("dbo.Addon", "AmountTypeId");
            DropColumn("dbo.Addon", "AddonTypeLinkId");
            DropColumn("dbo.Addon", "IsUsat");
            DropColumn("dbo.Addon", "IsShirtUpgrade");
            DropColumn("dbo.Addon", "IsOnlyForOwned");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Addon", "IsOnlyForOwned", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addon", "IsShirtUpgrade", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addon", "IsUsat", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addon", "AddonTypeLinkId", c => c.Int(nullable: false));
            AddColumn("dbo.Addon", "AmountTypeId", c => c.Int(nullable: false));
            DropColumn("dbo.Addon", "ImagePath");
            DropColumn("dbo.Addon", "EventureId");
        }
    }
}
