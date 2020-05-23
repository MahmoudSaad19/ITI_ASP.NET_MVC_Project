using System;
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

        public async Task<IActionResult> AddFriend(int id = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isFriend = dbContext.Friends.Any(f => f.UserId == id && f.FriendId == user.Id) && dbContext.Friends.Any(f => f.UserId == user.Id && f.FriendId == id);
            bool hasRequest = dbContext.FriendRequests.Any(req => req.ReqId == id && req.UserId == user.Id);
            bool isRequest = dbContext.FriendRequests.Any(req => req.ReqId == user.Id && req.UserId == id);
            bool isValidUser = dbContext.Users.Any(u => u.Id == id);
            if(isValidUser && !isFriend && !hasRequest && !isRequest)
            {
                FriendRequest request = new FriendRequest()
                {
                    ReqId = user.Id, UserId = id
                };
                await dbContext.FriendRequests.AddAsync(request);
                await dbContext.SaveChangesAsync();
            }    
            return RedirectToAction("Profile");
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
            return RedirectToAction("Profile");
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
                    UserId = user.Id, FriendId = id
                };
                FriendList rightSideRelation = new FriendList()
                {
                    UserId = id, FriendId = user.Id
                };
                await dbContext.Friends.AddAsync(leftSideRelation);
                await dbContext.Friends.AddAsync(rightSideRelation);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Profile");
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

        public async Task<IActionResult> AddPost(string postText)
        {
            if(postText != null)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
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

        public async Task<IActionResult> AddComment(string comment,int id)
        {
            if (comment != null)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
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
            //var user = await _userManager.GetUserAsync(User);
            //user.Friends.
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
        
        #endregion
    }
}