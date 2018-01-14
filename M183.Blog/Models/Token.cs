using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183.Blog.Models
{
    public class Token
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Tokenstring { get; set; }
        public DateTime Expiry { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}