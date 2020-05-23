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

        public AdminController(ApplicationDbContext context, RoleManager<Role> roleManager)
        {
            dbContext = context;
            _role = roleManager;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string roleName, string roleDesc)
        {
            _role.CreateAsync(new Role(roleName, roleDesc));
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