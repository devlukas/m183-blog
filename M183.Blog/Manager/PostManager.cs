using M183.Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace M183.Blog.Manager
{
    public class PostManager : Controller
    {
        private BlogDbContext db = new BlogDbContext();

        public List<PostViewModel> GetPostsByUsername(string username)
        {
            return db.Posts.Where(p => p.User.Username == username).Select(p => new PostViewModel()
            {
                Id = p.Id,
                Username = p.User.Username,
                Title = p.Title,
                Content = p.Content
            }).ToList();
        }

        public PostViewModel GetPostById(int id)
        {
            Post post = db.Posts.First(p => p.Id == id);
            return new PostViewModel()
            {
                Username = post.User.Username,
                Title = post.Title,
                Content = post.Content,
                Comments = post.Comments.ToList().Select(c => new CommentViewModel()
                {
                    Content = c.Content,
                    User = c.User.Username,
                    Id = c.Id
                }).ToList()
            };
        }
    }
}