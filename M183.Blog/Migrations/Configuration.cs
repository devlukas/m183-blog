namespace M183.Blog.Migrations
{
    using System;
    using Models;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Manager;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.BlogDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.BlogDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            context.Roles.Add(new Role
            {
                Key = "Default",
                Metadata = new Metadata("System"),
                Title = "Standard User"
            });
            context.Roles.Add(new Role
            {
                Title = "Administrator",
                Metadata = new Metadata("System"),
                Key = "Admin"
            });
            
            User user = context.Users.First(u => u.Id == 1);
            Post post = new Post
            {
                Title = "Wie man an gute Noten kommt",
                Content = "Alle Schüler sehnen sich nach guten Noten. " +
                    "Strategien wie man an gute Noten kommt stellen wir hier vor.",
                User = user,
                Published = true,
                Metadata = new Metadata(user.Username)
            };

            context.Posts.AddOrUpdate(
                p => p.Id, post
            );

            context.Comments.AddOrUpdate(
                c => c.Id,
                new Comment
                {
                    Post = post,
                    Content = "Werde ich direkt ausprobieren!",
                    User = user,
                    Metadata = new Metadata(user.Username)
                }
            );
        }
    }
}
