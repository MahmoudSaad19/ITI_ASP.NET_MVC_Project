using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FB_Project_DB_Model_int_Key.Data;
using FB_Project_DB_Model_int_Key.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FB_Project_DB_Model_int_Key.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private ApplicationDbContext dbContext;
        private UserManager<User> _user;

        public MemberController(ApplicationDbContext context, UserManager<User> userManager )
        {
            dbContext = context;
            _user = userManager;
        }

        public IActionResult Profile()
        {
            return View(dbContext.Users.ToList());
        }

        public IActionResult Home()
        {
            return View();
        }

        public async Task<IActionResult> Post (string text)
        {
            var user = await _user.FindByNameAsync(User.Identity.Name);
            Post post = new Post()
            {
                BodyText = text,
                Date = DateTime.Now,
                UserId = user.Id,
            };
            await dbContext.Posts.AddAsync(post);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Home");
        }

        public IActionResult Search2(string name)
        {
            if (name == null)
            {
                return NotFound();
            }

            var member = from u in dbContext.Users
                         select u;

            var names = member.Where(m => m.UserName.Contains(name));

            if (names == null)
            {
                return NotFound();
            }

            return View(names);
        }

        public IActionResult Settings()
        {
            return View();
        }
    }
}