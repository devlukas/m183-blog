using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace M183.Blog.Models
{
    [ComplexType]
    public class Metadata
    {
        Metadata(string username)
        {
            this.ModifiedUser = username;
            this.CreationUser = username;
            this.CreationDate = DateTime.Now;
            this.ModifiedDate = DateTime.Now;
        }
   
        public string CreationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public string ModifiedUser { get; set; }
        public DateTime ModifiedDate { get; set; }

        public void SetModified(string username)
        {
            this.ModifiedUser = username;
            this.ModifiedDate = DateTime.Now;
        }
    }
}