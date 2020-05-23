#region Usings ( imports )

using System.Diagnostics;
using System.Threading.Tasks;
using FB_Project_DB_Model_int_Key.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 

#endregion

namespace FB_Project_DB_Model_int_Key.Controllers
{
    /// <summary>
    ///URL http://localhost:port/Home 
    /// this is the entry point for our website 
    /// where we have 2 pages : 
    /// 1.welcome page for login and registeration ( index )
    /// 2.our team Names Page ( privacy )
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private RoleManager<Role> _role;

        public HomeController(ILogger<HomeController> logger, RoleManager<Role> roleManager)
        {
            _logger = logger;
            /// document the entire project 
            /// work on doaa task 
            /// work on mostafa task
            /// adjust ui 
            /// learn github on vs
            /// try to update project with git 
            _role = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName, string roleDesc)
        {
            await _role.CreateAsync(new Role(roleName, roleDesc));
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
