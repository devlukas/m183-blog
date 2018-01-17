using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183.Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
        public string Content { get; set; }
        public virtual Metadata Metadata { get; set; }
    }
}