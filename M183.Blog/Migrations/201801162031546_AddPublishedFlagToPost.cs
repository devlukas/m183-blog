namespace M183.Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPublishedFlagToPost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Published", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Published");
        }
    }
}
