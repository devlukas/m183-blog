using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace M183.Blog.Models
{

    public class BlogDbContext : DbContext
    {
        public BlogDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Userlog> Userlogs { get; set; }
        public DbSet<Userlogin> Userlogins { get; set; }

        public static BlogDbContext Create()
        {
            return new BlogDbContext();
        }
    }
}