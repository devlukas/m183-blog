using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183.Blog.Models
{
    public class Userlogin
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string UserIpAdress { get; set; }
        public string SessionId { get; set; }
        public Metadata Metadata { get; set; }
    }
}