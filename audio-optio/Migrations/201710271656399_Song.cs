namespace audio_optio.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Song : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Song", c => c.String(nullable: false));
            DropColumn("dbo.Orders", "YoutubeLink");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "YoutubeLink", c => c.String(nullable: false));
            DropColumn("dbo.Orders", "Song");
        }
    }
}
