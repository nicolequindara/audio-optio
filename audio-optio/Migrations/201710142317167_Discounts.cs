namespace audio_optio.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Discounts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "DiscountCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "DiscountCode");
        }
    }
}
