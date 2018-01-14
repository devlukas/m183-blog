namespace M183.Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBaseStructure : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        Metadata_CreationUser = c.String(),
                        Metadata_CreationDate = c.DateTime(nullable: false),
                        Metadata_ModifiedUser = c.String(),
                        Metadata_ModifiedDate = c.DateTime(nullable: false),
                        Post_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.Post_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Post_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        Metadata_CreationUser = c.String(),
                        Metadata_CreationDate = c.DateTime(nullable: false),
                        Metadata_ModifiedUser = c.String(),
                        Metadata_ModifiedDate = c.DateTime(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Firstname = c.String(),
                        Lastname = c.String(),
                        MobileNumber = c.String(),
                        Username = c.String(),
                        Password = c.String(),
                        Metadata_CreationUser = c.String(),
                        Metadata_CreationDate = c.DateTime(nullable: false),
                        Metadata_ModifiedUser = c.String(),
                        Metadata_ModifiedDate = c.DateTime(nullable: false),
                        Role_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Roles", t => t.Role_Id)
                .Index(t => t.Role_Id);
            
            CreateTable(
                "dbo.Userlogins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserIpAdress = c.String(),
                        SessionId = c.String(),
                        Metadata_CreationUser = c.String(),
                        Metadata_CreationDate = c.DateTime(nullable: false),
                        Metadata_ModifiedUser = c.String(),
                        Metadata_ModifiedDate = c.DateTime(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Key = c.String(),
                        Metadata_CreationUser = c.String(),
                        Metadata_CreationDate = c.DateTime(nullable: false),
                        Metadata_ModifiedUser = c.String(),
                        Metadata_ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Userlogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Tokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tokenstring = c.String(),
                        Expiry = c.DateTime(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tokens", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Comments", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Posts", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Userlogs", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "Role_Id", "dbo.Roles");
            DropForeignKey("dbo.Userlogins", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Comments", "Post_Id", "dbo.Posts");
            DropIndex("dbo.Tokens", new[] { "User_Id" });
            DropIndex("dbo.Userlogs", new[] { "User_Id" });
            DropIndex("dbo.Userlogins", new[] { "User_Id" });
            DropIndex("dbo.Users", new[] { "Role_Id" });
            DropIndex("dbo.Posts", new[] { "User_Id" });
            DropIndex("dbo.Comments", new[] { "User_Id" });
            DropIndex("dbo.Comments", new[] { "Post_Id" });
            DropTable("dbo.Tokens");
            DropTable("dbo.Userlogs");
            DropTable("dbo.Roles");
            DropTable("dbo.Userlogins");
            DropTable("dbo.Users");
            DropTable("dbo.Posts");
            DropTable("dbo.Comments");
        }
    }
}
