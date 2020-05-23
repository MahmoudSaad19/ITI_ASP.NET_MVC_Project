#region usings ( imports )
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using FB_Project_DB_Model_int_Key.Models;
using Microsoft.AspNetCore.Mvc.Rendering; 
#endregion

namespace FB_Project_DB_Model_int_Key.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class AdminCreateUserModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<Role> _roleManager;

        public AdminCreateUserModel(
            UserManager<User> userManager,
            ILogger<RegisterModel> logger,
            RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        /// <summary>
        /// Bind class for ui to seperate database model from client ui
        /// </summary>
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
#warning  change pass length 
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            public string UserName { get; set; }
            public char Gender { get; set; }
            public DateTime BirthDate { get; set; }
            public string RoleName { set; get; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ViewData["Roles"] = new SelectList(_roleManager.Roles);
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Admin/Searchusers");
            if (ModelState.IsValid)
            {
                var user = new User { Email = Input.Email, UserName = Input.UserName, Gender = Input.Gender, BirthDate= Input.BirthDate };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, Input.RoleName);

                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
