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
        private UserManager<User> _user;

        public MemberController(ApplicationDbContext context, UserManager<User> userManager)
        {
            dbContext = context;
            _user = userManager;
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Home()
        {
            var posts = dbContext.Posts.OrderByDescending(d => d.Date).Include(u => u.User).Include(p => p.Comments).ToList();
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
            var user = await _user.FindByNameAsync(User.Identity.Name);
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