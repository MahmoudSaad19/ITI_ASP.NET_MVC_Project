using System;
using System.Linq;
using System.Threading.Tasks;
using FB_Project_DB_Model_int_Key.Data;
using FB_Project_DB_Model_int_Key.Models;
using Microsoft.AspNetCore.Authorization;
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

        public MemberController(ApplicationDbContext context, UserManager<User> userManager)
        {
            dbContext = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Profile()
        {
            await dbContext.Posts.LoadAsync();
            await dbContext.PostLikes.LoadAsync();
            await dbContext.Comments.LoadAsync();
            await dbContext.Friends.LoadAsync();
            await dbContext.FriendRequests.LoadAsync();
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

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
            dbContext.Posts.Add(newPost);
            dbContext.SaveChanges();
            return RedirectToAction("Home");
        }

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

        public IActionResult Settings()
        {
            return View();
        }
    }
}