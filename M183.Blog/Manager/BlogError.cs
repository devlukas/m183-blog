using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183.Blog.Manager
{
    public class BlogError: Exception
    {
        public BlogError()
        {
        }
        public BlogError(string message)
        : base(message)
    {
        }
    }
}