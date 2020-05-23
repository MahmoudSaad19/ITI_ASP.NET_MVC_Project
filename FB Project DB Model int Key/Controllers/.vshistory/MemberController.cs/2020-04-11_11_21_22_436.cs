using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FB_Project_DB_Model_int_Key.Data;
using Microsoft.AspNetCore.Mvc;

namespace FB_Project_DB_Model_int_Key.Controllers
{
    
    public class MemberController : Controller
    {
        private ApplicationDbContext dbContext;

        public MemberController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Home()
        {
            return View();
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