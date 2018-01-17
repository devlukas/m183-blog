namespace M183.Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserloginDeletedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Userlogins", "DeletedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Userlogins", "DeletedDate");
        }
    }
}
