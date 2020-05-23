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
        private SignInManager<User> _signInManager;

        public MemberController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            dbContext = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

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
            return RedirectToAction("Home");
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