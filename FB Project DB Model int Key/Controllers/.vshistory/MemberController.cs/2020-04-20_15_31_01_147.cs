﻿#region usings ( imports )

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
#endregion

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

        public async Task<IActionResult> Profile(int? id)
        {
            await dbContext.Posts.LoadAsync();
            await dbContext.PostLikes.LoadAsync();
            await dbContext.Comments.LoadAsync();
            await dbContext.Users.LoadAsync();
            await dbContext.Friends.LoadAsync();
            await dbContext.FriendRequests.LoadAsync();

            var selectedUser = dbContext.Users.Find(id);
            var user = await _userManager.GetUserAsync(User);
            
            if (selectedUser == null || selectedUser.Id ==user.Id)
            {
                return View(user);
            }

            return View(selectedUser);
        }
        
        #region Edit User Info

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
        
        #region Friend Relation

        public async Task<IActionResult> AddFriend(int id = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isFriend = dbContext.Friends.Any(f => f.UserId == id && f.FriendId == user.Id) && dbContext.Friends.Any(f => f.UserId == user.Id && f.FriendId == id);
            bool hasRequest = dbContext.FriendRequests.Any(req => req.ReqId == id && req.UserId == user.Id);
            bool isRequest = dbContext.FriendRequests.Any(req => req.ReqId == user.Id && req.UserId == id);
            bool isValidUser = dbContext.Users.Any(u => u.Id == id);
            bool isme = user.Id == id;
            if (isValidUser && !isFriend && !hasRequest && !isRequest && !isme)
            {
                FriendRequest request = new FriendRequest()
                {
                    ReqId = user.Id,
                    UserId = id
                };
                await dbContext.FriendRequests.AddAsync(request);
                await dbContext.SaveChangesAsync();
            }
            return Redirect("/Member/Profile/" + id);
        }

        public async Task<IActionResult> CancelRequest(int id = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            var request = dbContext.FriendRequests.Where(req => req.ReqId == user.Id && req.UserId == id);
            if (request != null && request.Count() == 1)
            {
                dbContext.FriendRequests.Remove(request.FirstOrDefault());
                await dbContext.SaveChangesAsync();
            }
            return Redirect("/Member/Profile/" + id);
        }

        public async Task<IActionResult> Accept(int id = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            var request = dbContext.FriendRequests.Where(req => req.UserId == user.Id && req.ReqId == id);
            if (request != null && request.Count() == 1)
            {
                dbContext.FriendRequests.Remove(request.FirstOrDefault());
                FriendList leftSideRelation = new FriendList()
                {
                    UserId = user.Id,
                    FriendId = id
                };
                FriendList rightSideRelation = new FriendList()
                {
                    UserId = id,
                    FriendId = user.Id
                };
                await dbContext.Friends.AddAsync(leftSideRelation);
                await dbContext.Friends.AddAsync(rightSideRelation);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Profile", id);
        }

        public async Task<IActionResult> Reject(int id = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            var request = dbContext.FriendRequests.Where(req => req.UserId == user.Id && req.ReqId == id);
            if (request != null && request.Count() == 1)
            {
                dbContext.FriendRequests.Remove(request.FirstOrDefault());
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> UnFriend(int id = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            var leftSideRelationFriend = dbContext.Friends.Where(f => f.FriendId == id && f.UserId == user.Id);
            var rightSideRelationFriend = dbContext.Friends.Where(f => f.FriendId == user.Id && f.UserId == id);
            if (leftSideRelationFriend != null && rightSideRelationFriend != null
                && leftSideRelationFriend.Count() == 1 && rightSideRelationFriend.Count() == 1)
            {
                dbContext.Friends.Remove(leftSideRelationFriend.FirstOrDefault());
                dbContext.Friends.Remove(rightSideRelationFriend.FirstOrDefault());
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Profile");
        }

        #endregion

        #region Posts & Comments & Likes

        public async Task<IActionResult> AddPost(string postText)
        {
            if (postText != null)
            {
                var user = await _userManager.GetUserAsync(User);
                Post newPost = new Post()
                {
                    BodyText = postText,
                    UserId = user.Id,
                    Date = DateTime.Now
                };
                await dbContext.Posts.AddAsync(newPost);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Profile");
        }

        public IActionResult DeleteAPost(int? id)
        {
            var post = dbContext.Posts.Find(id);
            dbContext.Posts.Remove(post);
            dbContext.SaveChanges();
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> AddComment(string comment, int id)
        {
            if (comment != null)
            {
                var user = await _userManager.GetUserAsync(User);
                Comment newComment = new Comment()
                {
                    BodyText = comment,
                    UserId = user.Id,
                    PostId = id
                };
                await dbContext.Comments.AddAsync(newComment);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Profile");
        }

        public IActionResult DeleteAComment(int? id)
        {
            var comment = dbContext.Comments.Find(id);
            dbContext.Comments.Remove(comment);
            dbContext.SaveChanges();
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> Like(int id = 0)
        {
            var post = dbContext.Posts.Find(id);
            if (post != null)
            {
                var user = await _userManager.GetUserAsync(User);
                bool isLiked = dbContext.PostLikes.Any(p => p.PostId == id && p.UserId == user.Id);
                if (!isLiked)
                {
                    PostLikes like = new PostLikes()
                    {
                        PostId = id,
                        UserId = user.Id
                    };
                    dbContext.PostLikes.Add(like);
                    await dbContext.SaveChangesAsync();
                }
            }
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> DisLike(int id = 0)
        {
            var post = dbContext.Posts.Find(id);
            if (post != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var postlike = dbContext.PostLikes.Where(p => p.PostId == id && p.UserId == user.Id);
                if (postlike != null && postlike.Count() == 1)
                {
                    dbContext.PostLikes.Remove(postlike.FirstOrDefault());
                    await dbContext.SaveChangesAsync();
                }
            }
            return RedirectToAction("Profile");
        }

        public IActionResult LikesList(int? id)
        {
            var post = dbContext.Posts.Include(p => p.PostLikes).Include(u => u.User).Where(i => i.PostID == id);
            dbContext.Users.Load();
            dbContext.Posts.Load();
            dbContext.PostLikes.Load();
            return PartialView("LikesList", post);
        }

        #endregion

        #endregion

        #region Home
        ///requirements:
        ///1.posts , comments , likes and likeslist functions
        ///2.preview posts of his friends only
        ///3.add users photo
        
        public async Task<IActionResult> Home()
        {
            var user = await _userManager.GetUserAsync(User);

            dbContext.Friends.Load();
            dbContext.Users.Load();

            List<User> users = user.Friends.Select(f => f.Friend).ToList();
            users.Add(user);
            //var posts = users.SelectMany(u => u.Posts).OrderByDescending(p => p.Date);
            //var posts = users.Select(p => p.Posts);
            var posts = from u in users
                        from p in u.Posts
                        where u.Id == p.UserId
                        orderby p.Date descending
                        select p;
            foreach (var item in posts)
            {
                Trace.WriteLine(item.User.UserName);
            }


            var posts2 = dbContext.Posts.OrderByDescending(d => d.Date)/*.Include(c => c.Comments).Include(p => p.PostLikes)*/.ToList();
            return View(posts);
        }

        #region Post
#warning not working correctly
        ///1.function post 
        ///2.let friends comment on posts
        ///3.


        public async Task<IActionResult> Post(string textPost)
        {
            var user = await _userManager.GetUserAsync(User);
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

        [HttpPost]
        public IActionResult EditPost(Post post)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Home");
            }
            var p = dbContext.Posts.Find(post.PostID);
            p.BodyText = post.BodyText;
            dbContext.Posts.Update(p);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        public IActionResult DeletePost(int? id)
        {
            var post = dbContext.Posts.Find(id);
            dbContext.Posts.Remove(post);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        #endregion
                
        #region Comment

        [HttpPost]
        public IActionResult Comment(Comment comment)
        {
            if (ModelState.IsValid)
            {
                var com = new Comment
                {
                    UserId = comment.UserId,
                    PostId = comment.PostId,
                    BodyText = comment.BodyText
                };
                dbContext.Comments.Add(com);
                dbContext.SaveChanges();
            }

            return RedirectToAction("Home");
        }

        [HttpPost]
        public IActionResult EditComment(Comment comment)
        {
            var com = dbContext.Comments.Find(comment.CID);
            com.BodyText = comment.BodyText;
            dbContext.Comments.Update(com);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        public IActionResult DeleteComment(Comment comment)
        {
            var com = dbContext.Comments.Find(comment.CID);
            dbContext.Comments.Remove(com);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

        #endregion

        #region Likes

        //[HttpGet]
        //public IActionResult LikesList(int? id)
        //{
        //    var post = dbContext.Posts.Include(p => p.PostLikes).Include(u => u.User).Where(i => i.PostID == id);
        //    dbContext.Users.Load();
        //    return PartialView(post);
        //}

        #endregion

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
            dbContext.Users.Load();
            dbContext.FriendRequests.Load();
            dbContext.Friends.Load();
            return View(names);
        }
        
        #endregion
    }
}