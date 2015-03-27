namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class surchargeQuantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Surcharge", "Quantity", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Surcharge", "Quantity");
        }
    }
}
