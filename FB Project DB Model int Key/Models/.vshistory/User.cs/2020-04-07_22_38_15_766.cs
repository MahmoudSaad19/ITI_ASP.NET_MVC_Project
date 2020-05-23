﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FB_Project_V2._0.Models
{
    public class User : IdentityUser
    {
        #warning check for unique index on user name and email
        
        public User():base()
        {
            Posts = new HashSet<Post>();
            PostLikes = new HashSet<PostLikes>();
            Comments = new HashSet<Comment>();
            //Friends = new HashSet<FriendList>();
            //FriendRequests = new HashSet<FriendRequest>();
//#warning add nav prop postlikes
        }

        public User(string name):base(name)
        {
            Posts = new HashSet<Post>();
            Comments = new HashSet<Comment>();
            PostLikes = new HashSet<PostLikes>();
            //Friends = new HashSet<FriendList>();
            //FriendRequests = new HashSet<FriendRequest>();
            //#warning add nav prop postlikes
        }

        public Byte [] Photo { get; set; }
        public string BIO { get; set; }
//#warning make it nullable
        public DateTime BirthDate { get; set; }
//#warning make it nullable
        public char Gender { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Post> Posts { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Comment> Comments { get; set; }

        //#warning add nav prop postlikes
        //public virtual ICollection<User> Users { get; set; }
        //#warning add friend requests list too or delete it 
        //[InverseProperty("Friend")]
        //public virtual ICollection<FriendList> Friends { get; set; }
        //[InverseProperty("reqFriend")]
        //public virtual ICollection<FriendRequest> FriendRequests { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<PostLikes> PostLikes { get; set; }

    }
}
