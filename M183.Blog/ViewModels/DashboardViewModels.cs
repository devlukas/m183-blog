using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183.Blog.Models
{
    public class DashboardViewModel
    {
        public List<PostViewModel> Posts { get; set; }
        public string Search { get; set; }
    }

    public class UserViewModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class CommentViewModel
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Content { get; set; }
    }

    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Username { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}