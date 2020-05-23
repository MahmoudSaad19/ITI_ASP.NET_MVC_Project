using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FB_Project_DB_Model_int_Key.Data;
using Microsoft.AspNetCore.Mvc;

namespace FB_Project_DB_Model_int_Key.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext dbContext;

        public AdminController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult SearchUsers()
        {
            return View();
        }
    }
}