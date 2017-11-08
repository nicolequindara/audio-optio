namespace audio_optio.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Address1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "BillingAddress_Id", c => c.Int());
            CreateIndex("dbo.Orders", "BillingAddress_Id");
            AddForeignKey("dbo.Orders", "BillingAddress_Id", "dbo.Addresses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "BillingAddress_Id", "dbo.Addresses");
            DropIndex("dbo.Orders", new[] { "BillingAddress_Id" });
            DropColumn("dbo.Orders", "BillingAddress_Id");
        }
    }
}
