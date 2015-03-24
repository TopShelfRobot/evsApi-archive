namespace evs.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bundle : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.EventureListBundle", new[] { "EventureList_Id" });
            RenameColumn(table: "dbo.EventureListBundle", name: "EventureList_Id", newName: "EventureListId");
            AddColumn("dbo.EventureListBundle", "ChildEventureListId", c => c.Int(nullable: false));
            AlterColumn("dbo.EventureListBundle", "EventureListId", c => c.Int(nullable: false));
            CreateIndex("dbo.EventureListBundle", "EventureListId");
            DropColumn("dbo.EventureListBundle", "EvenureListId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EventureListBundle", "EvenureListId", c => c.Int(nullable: false));
            DropIndex("dbo.EventureListBundle", new[] { "EventureListId" });
            AlterColumn("dbo.EventureListBundle", "EventureListId", c => c.Int());
            DropColumn("dbo.EventureListBundle", "ChildEventureListId");
            RenameColumn(table: "dbo.EventureListBundle", name: "EventureListId", newName: "EventureList_Id");
            CreateIndex("dbo.EventureListBundle", "EventureList_Id");
        }
    }
}
