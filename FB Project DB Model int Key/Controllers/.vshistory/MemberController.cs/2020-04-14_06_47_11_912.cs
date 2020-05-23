using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FB_Project_DB_Model_int_Key.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB_Project_DB_Model_int_Key.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private ApplicationDbContext dbContext;

        public MemberController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Profile()
        {
            return View(dbContext.Users.ToList());
        }

        public IActionResult Home()
        {
            return View();
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