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
            return View();
        }

        public IActionResult Post(string textPost)
        {
            var user = _user.FindByNameAsync(User.Identity.Name);
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

        public IActionResult Search()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }
    }
}