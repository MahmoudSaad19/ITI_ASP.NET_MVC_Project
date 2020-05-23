#region usings ( imports )
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using FB_Project_DB_Model_int_Key.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging; 
#endregion

namespace FB_Project_DB_Model_int_Key.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<User> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<User> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// ui bind model class
        /// </summary>
        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        /// <summary>
        /// checks for current login if there is a login it will prevent to go to login page
        /// </summary>
        /// <param name="returnUrl"> url will return to it</param>
        /// <returns>task!!??</returns>
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// try login with string written in the ui
        /// using user manager with user name at first if failed 
        /// then try to login with email and on success
        /// it checks if they is blocked or not before
        /// it checks for current user role to redirect them to thier home page 
        /// on failure it redirect to the same page
        /// </summary>
        /// <param name="returnUrl"> url will return to it (home page)</param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Member/Home");

            #region Regex For Email and UseName validation in case we need it 

            //if (input.Email.IndexOf('@') > -1)
            //{
            //    //Validate email format
            //    string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
            //                           @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
            //                              @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            //    Regex re = new Regex(emailRegex);
            //    if (!re.IsMatch(input.Email))
            //    {
            //        ModelState.AddModelError("Email", "Email is not valid");
            //    }
            //}
            //else
            //{
            //    //validate Username format
            //    string emailRegex = @"^[a-zA-Z0-9]*$";
            //    Regex re = new Regex(emailRegex);
            //    if (!re.IsMatch(input.Email))
            //    {
            //        ModelState.AddModelError("Email", "Username is not valid");
            //    }
            //}

            #endregion

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                #region Login process with username or email

                var user2 = await _userManager.FindByNameAsync(Input.UserName);
                if (user2.Blocked)
                {
                    returnUrl = Url.Content("~/Identity/Account/Unused/AccessDenied");
                    return LocalRedirect(returnUrl);
                }

                ///first try with username
                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(Input.UserName);
                    if (user.Blocked)
                    {
                        returnUrl = Url.Content("~/Identity/Account/Unused/AccessDenied");
                        return LocalRedirect(returnUrl);
                    }
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Any(r => r == "Admin"))
                        returnUrl = Url.Content("~/Admin/SearchUsers");
                    else
                        returnUrl = Url.Content("~/Member/Home");
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                ///2nd try with email
                else
                {
                    var user = await _userManager.FindByEmailAsync(Input.UserName);
                    Input.UserName = user?.UserName ?? Input.UserName;
                    result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (user.Blocked)
                        {
                            returnUrl = Url.Content("~/Identity/Account/Unused/AccessDenied");
                            return LocalRedirect(returnUrl);
                        }
                        if (roles.Any(r => r == "Admin"))
                            returnUrl = Url.Content("~/Admin/SearchUsers");
                        else
                            returnUrl = Url.Content("~/Member/Home");
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(returnUrl);
                    }
                }

                #endregion
                
                #region if login process failed

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                } 

                #endregion

            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
