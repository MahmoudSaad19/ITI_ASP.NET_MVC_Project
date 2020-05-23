using System;
using System.Collections.Generic;
using System.Text;
using FB_Project_DB_Model_int_Key.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FB_Project_DB_Model_int_Key.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //#warning this is wrong because it's already defined in base abstract class that user inhert from
        //public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<FriendRequest> FriendRequests { get; set; }
        public virtual DbSet<FriendList> Friends { get; set; }
        public virtual DbSet<PostLikes> PostLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);         

            builder.Entity<User>(i => {
                i.ToTable("Users");
                i.HasKey(x => x.Id);
            });
            builder.Entity<Role>(i => {
                i.ToTable("Roles");
                i.HasKey(x => x.Id);
            });
            builder.Entity<UserRole>(i => {
                i.ToTable("UserRole");
                i.HasKey(x => new { x.RoleId, x.UserId });
            });
            builder.Entity<UserLogin>(i => {
                i.ToTable("UserLogin");
                i.HasKey(x => new { x.ProviderKey, x.LoginProvider });
            });
            builder.Entity<RoleClaim>(i => {
                i.ToTable("RoleClaims");
                i.HasKey(x => x.Id);
            });
            builder.Entity<UserClaim>(i => {
                i.ToTable("UserClaims");
                i.HasKey(x => x.Id);
            });

            builder.Entity<FriendList>().HasKey(f => new { f.UserId, f.FriendId });
            builder.Entity<FriendRequest>().HasKey(f => new { f.UserId, f.ReqId });
            
            //builder.Entity<Comment>()
            //    .HasOne<User>(u => u.User)
            //    .WithMany(c => c.Comments)
            //    .HasForeignKey(f => f.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<PostLikes>()
            //    .HasOne<User>(u => u.User)
            //    .WithMany(c => c.PostLikes)
            //    .HasForeignKey(f => f.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
