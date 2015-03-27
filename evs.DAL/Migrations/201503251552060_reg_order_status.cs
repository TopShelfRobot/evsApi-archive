namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reg_order_status : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registration", "RegStatus", c => c.Int(nullable: false));
            AddColumn("dbo.EventureOrder", "OrderStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Refund", "RefundType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Refund", "RefundType");
            DropColumn("dbo.EventureOrder", "OrderStatus");
            DropColumn("dbo.Registration", "RegStatus");
        }
    }
}
