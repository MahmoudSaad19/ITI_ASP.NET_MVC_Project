﻿using System;
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
        private UserManager<User> _user;

        public AdminController(ApplicationDbContext context, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            dbContext = context;
            _user = userManager;
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
            return View(dbContext.Users.ToList());
        }

        public IActionResult Block(int id = 0)
        {
            var user = dbContext.Users.Find(id);
            if (user != null)
            {
                user.Blocked = true;
                dbContext.SaveChanges();
            }
            return RedirectToAction("SearchUsers");
        }

        public IActionResult UnBlock(int id = 0)
        {
            var user = dbContext.Users.Find(id);
            if (user != null)
            {
                user.Blocked = false;
                dbContext.SaveChanges();
            }
            return RedirectToAction("SearchUsers");
        }

        public async Task<IActionResult> UpdateRole(string role_1, string role_2, string role_3, int id = 0, params string[] role)
        {
            var roles = _role.Roles.Select(r=>r.Name);
            var user = dbContext.Users.Find(id);
            if (user != null)
            {
                await _user.RemoveFromRolesAsync(user, roles);
                List<string> selectedRoles = new List<string> { role_1, role_2, role_3 };
                await _user.AddToRolesAsync(user, selectedRoles);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("SearchUsers");
        }

#warning remaining requirements 
        ///role checkboxes


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