﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FB_Project_DB_Model_int_Key.Models
{
    public class Role : IdentityRole<int>
    {
        public Role():base()
        {
                
        }

        public Role(string roleName, string roleDescribtion):base(roleName)
        {
            Description = roleDescribtion;
        }

        public string Description { get; set; }
    }
}
