using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183.Blog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public virtual User User { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public Metadata Metadata { get; set; }
    }
}