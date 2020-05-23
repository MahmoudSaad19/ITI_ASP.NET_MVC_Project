using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FB_Project_V1._0.Models
{
    public class User : IdentityUser
    {
        public User():base()
        {

        }

        public User(string name):base(name)
        {

        }
    }
}
