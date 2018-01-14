namespace M183.Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserBlock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Blocked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Blocked");
        }
    }
}
