﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FB_Project_DB_Model_int_Key.Data;
using FB_Project_DB_Model_int_Key.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FB_Project_DB_Model_int_Key.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private ApplicationDbContext dbContext;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public MemberController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            dbContext = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #region Profile
        ///requirements:
        ///1.other users profile ( remove his friend request , post creation , add buttons for adding him ) 
        ///2.posts, comments ,likes and likes list for current user
        ///3.photo for every thing in point 2 ^^
        ///4.friend relation buttons for current user

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            await dbContext.Posts.LoadAsync();
            await dbContext.PostLikes.LoadAsync();
            await dbContext.Comments.LoadAsync();
            await dbContext.Friends.LoadAsync();
            await dbContext.FriendRequests.LoadAsync();

            return View(user);
        }

        public async Task<IActionResult> EditProfile(User user)
        {
            User u = await dbContext.Users.FindAsync(user.Id);
            u.UserName = user.UserName;
            u.Gender = user.Gender;
            u.BirthDate = user.BirthDate;
            u.BIO = user.BIO;
            await _userManager.UpdateAsync(u);
            await _signInManager.RefreshSignInAsync(u);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Profile");
        }

        /// <summary>
        /// it needs it's own modal to display all users who liked the post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult LikesList(int? id)
        {
            var post = dbContext.Posts.Where(i => i.PostID == id);
            return PartialView("LikesList", post);
        }

        public async Task<IActionResult> UploadPic(IFormFile pic)
        {
            if (pic != null && pic.ContentType.Contains("image/"))
            {
                var user = await _userManager.GetUserAsync(User);

                using (MemoryStream ms = new MemoryStream())
                {
                    await pic.CopyToAsync(ms);
                    user.Photo = ms?.ToArray();
                }
                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);
                await dbContext.SaveChangesAsync();
            }
            
            return RedirectToAction("Profile");
        }
                      

        #endregion

        #region Home
        ///requirements:
        ///1.posts , comments , likes and likeslist functions
        ///2.preview posts of his friends only
        ///3.add users photo
               
        public IActionResult Home()
        {
            var posts = dbContext.Posts.OrderByDescending(d => d.Date).Include(u => u.User).Include(p => p.Comments).Include(p => p.PostLikes).ToList();
            return View(posts);
        }

        public IActionResult DeletePost(int? id)
        {
            var post = dbContext.Posts.Find(id);
            dbContext.Posts.Remove(post);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        public async Task<IActionResult> Post(string textPost)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Post newPost = new Post()
            {
                BodyText = textPost,
                UserId = user.Id,
                Date = DateTime.Now
            };
            await dbContext.Posts.AddAsync(newPost);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Home");
        }

        #endregion

        #region Search
        ///requirements:
        ///1.add friend relation buttons 
        ///2.add photo

        public IActionResult Search(string name)
        {
            if (name == null)
            {
                return View("_ErrorView");
            }

            var names = dbContext.Users.Where(m => m.UserName.Contains(name));

            if (names == null)
            {
                return View("_ErrorView");
            }

            return View(names);
        }

        /// <summary>
        /// kareem test fn
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddFriend(User u)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            FriendRequest fr = new FriendRequest()
            {
                User = u,
                UserId = user.Id
            };

            await dbContext.FriendRequests.AddAsync(fr);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Profile");
        }

        #endregion
    }
}