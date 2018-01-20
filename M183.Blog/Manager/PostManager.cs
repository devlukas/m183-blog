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

        public List<PostViewModel> GetPosts()
        {
            return db.Posts.Select(p => new PostViewModel()
            {
                Id = p.Id,
                Username = p.User.Username,
                Title = p.Title,
                Content = p.Content
            }).ToList();
        }

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

        public List<PostViewModel> GetPublishedPosts()
        {
            return db.Posts.Where(p => p.Published).Select(p => new PostViewModel()
            {
                Id = p.Id,
                Username = p.User.Username,
                Title = p.Title,
                Content = p.Content
            }).ToList();
        }

        public List<PostViewModel> GetSearchedPosts(string search)
        {
            if (search == null)
            {
                search = String.Empty;
            }
            return db.Posts
                .Where(p => p.Published)
                .Where(p => p.Title.Contains(search) || p.Content.Contains(search))
                .Select(p => new PostViewModel()
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
                Id = post.Id,
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

        public Post GetPost (int id)
        {
            return db.Posts.First(p => p.Id == id);
        }

        /// <summary>
        /// Add an user comment to database
        /// </summary>
        /// <param name="comment">The comment</param>
        /// <param name="postId">The post id</param>
        /// <param name="username">The user's name</param>
        public void Comment (string comment, int postId, string username)
        {
            UserManager userManager = new UserManager();

            db.Comments.Add(new Comment()
            {
                Content = comment,
                Metadata = new Metadata(username),
                Post = GetPost(postId),
                User = userManager.GetUser(username)
            });
            db.SaveChanges();
        }
    }
}