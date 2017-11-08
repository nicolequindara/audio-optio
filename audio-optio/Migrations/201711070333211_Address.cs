namespace audio_optio.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Address : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        To = c.String(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        PostalCode = c.String(),
                        Contact_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.Contact_Id)
                .Index(t => t.Contact_Id);
            
            AddColumn("dbo.Orders", "ShippingAddress_Id", c => c.Int());
            CreateIndex("dbo.Orders", "ShippingAddress_Id");
            AddForeignKey("dbo.Orders", "ShippingAddress_Id", "dbo.Addresses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "Contact_Id", "dbo.Contacts");
            DropForeignKey("dbo.Orders", "ShippingAddress_Id", "dbo.Addresses");
            DropIndex("dbo.Orders", new[] { "ShippingAddress_Id" });
            DropIndex("dbo.Addresses", new[] { "Contact_Id" });
            DropColumn("dbo.Orders", "ShippingAddress_Id");
            DropTable("dbo.Addresses");
        }
    }
}
