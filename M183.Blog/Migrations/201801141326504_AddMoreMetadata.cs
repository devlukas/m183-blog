namespace M183.Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMoreMetadata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tokens", "DeletedDate", c => c.DateTime());
            AddColumn("dbo.Userlogs", "Metadata_CreationUser", c => c.String());
            AddColumn("dbo.Userlogs", "Metadata_CreationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Userlogs", "Metadata_ModifiedUser", c => c.String());
            AddColumn("dbo.Userlogs", "Metadata_ModifiedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Userlogs", "Metadata_ModifiedDate");
            DropColumn("dbo.Userlogs", "Metadata_ModifiedUser");
            DropColumn("dbo.Userlogs", "Metadata_CreationDate");
            DropColumn("dbo.Userlogs", "Metadata_CreationUser");
            DropColumn("dbo.Tokens", "DeletedDate");
        }
    }
}
