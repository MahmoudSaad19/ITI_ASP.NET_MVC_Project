using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
            Comments = new HashSet<Comment>();
        }

        public User(string name):base(name)
        {
            Posts = new HashSet<Post>();
            Comments = new HashSet<Comment>();
        }

        public Byte [] Photo { get; set; }
        public string BIO { get; set; }
        public DateTime BirthDate { get; set; }                
        public char Gender { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Post> Posts { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Comment> Comments { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<User> Friend { get; set; }

    }
}
