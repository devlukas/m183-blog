using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183.Blog.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Key { get; set; }
        public Metadata Metadata { get; set; }
    }
}