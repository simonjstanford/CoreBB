using System;
using System.Linq;
using System.Threading.Tasks;
using CoreBB.Web.Helpers;
using CoreBB.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreBB.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private CoreBBContext coreBBContext;
        private IRegister register;
        private ILogin login;

        public UserController(CoreBBContext coreBBContext, IRegister register, ILogin login)
        {
            this.coreBBContext = coreBBContext;
            this.register = register;
            this.login = login;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> Register()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                throw new Exception("Invalid registration information.");

            var user = await register.RegisterUser(model);
            await login.LogInUserAsync(user, HttpContext);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> Login(LogInViewModel model)
        {
            if (!ModelState.IsValid)
                throw new Exception("Invalid user information");

            var targetUser = coreBBContext.User.SingleOrDefault(u => u.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));

            if (targetUser == null)
                throw new Exception("User does not exist");

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(targetUser, targetUser.PasswordHash, model.Password);

            if (result != PasswordVerificationResult.Success)
                throw new Exception("The password is wrong");

            await login.LogInUserAsync(targetUser, HttpContext);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public async Task<IActionResult> LogOUt()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}