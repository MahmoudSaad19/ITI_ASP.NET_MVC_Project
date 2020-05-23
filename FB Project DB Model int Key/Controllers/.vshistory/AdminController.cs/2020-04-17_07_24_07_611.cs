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
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext dbContext;
        private RoleManager<Role> _role;

        public AdminController(ApplicationDbContext context, RoleManager<Role> roleManager)
        {
            dbContext = context;
            _role = roleManager;
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName, string roleDesc)
        {
            var result = await _role.CreateAsync(new Role(roleName, roleDesc)); 
            return View(result);
        }

        public IActionResult SearchUsers()
        {
            return View();
        }

        public IActionResult AjaxSearch()
        {
            return View(dbContext.Users.ToList());
        }

        public JsonResult GetSearchingData(string SearchBy, string SearchValue)
        {
            List<User> userList = new List<User>();
            if (SearchBy == "UserID")
            {
                int id = Convert.ToInt32(SearchValue);
                userList = dbContext.Users.Where(u => u.Id == id || SearchValue == null).ToList();
                return Json(userList);
            }
            else
            {
                userList = dbContext.Users.Where(n => n.UserName.Contains(SearchValue) || SearchValue == null).ToList();
                return Json(userList);
            }
        }
    }
}