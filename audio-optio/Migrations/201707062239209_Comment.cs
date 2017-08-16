namespace audio_optio.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Comment : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Orders", new[] { "contact_Id" });
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false, maxLength: 255),
                        DateSubmitted = c.DateTime(nullable: false),
                        Contact_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.Contact_Id)
                .Index(t => t.Contact_Id);
            
            AddColumn("dbo.Orders", "Size", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "Contact_Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "Contact_Id", "dbo.Contacts");
            DropIndex("dbo.Orders", new[] { "Contact_Id" });
            DropIndex("dbo.Comments", new[] { "Contact_Id" });
            DropColumn("dbo.Orders", "Size");
            DropTable("dbo.Comments");
            CreateIndex("dbo.Orders", "contact_Id");
        }
    }
}
