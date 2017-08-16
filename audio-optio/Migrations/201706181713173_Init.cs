namespace audio_optio.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateSubmitted = c.DateTime(nullable: false),
                        DatePending = c.DateTime(nullable: false),
                        DateCompleted = c.DateTime(nullable: false),
                        YoutubeLink = c.String(nullable: false),
                        Comments = c.String(),
                        OrderStatus = c.Int(nullable: false),
                        contact_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.contact_Id)
                .Index(t => t.contact_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "contact_Id", "dbo.Contacts");
            DropIndex("dbo.Orders", new[] { "contact_Id" });
            DropTable("dbo.Orders");
            DropTable("dbo.Contacts");
        }
    }
}
