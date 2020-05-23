using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FB_Project_V1._0.Models
{
    public class User : IdentityUser
    {
        public User():base()
        {
            Posts = new HashSet<Post>();
        }

        public User(string name):base(name)
        {
            Posts = new HashSet<Post>();
        }

        public Byte [] Photo { get; set; }
        public string BIO { get; set; }
        public DateTime BirthDate { get; set; }                
        public char Gender { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
