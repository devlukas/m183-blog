using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183.Blog.Models
{
    public class DetailViewModel
    {
        public PostViewModel Post { get; set; }
        public string Comment { get; set; }
    }
}