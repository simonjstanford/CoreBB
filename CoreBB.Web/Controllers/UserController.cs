using System;
using System.Threading.Tasks;
using CoreBB.Web.Interfaces;
using CoreBB.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBB.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private IRepository repository;
        private IRegister register;
        private ILogin login;
        private IHasher hasher;

        public UserController(IRepository repository, IRegister register, ILogin login, IHasher hasher)
        {
            this.repository = repository;
            this.register = register;
            this.login = login;
            this.hasher = hasher;
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

            var targetUser = await repository.GetUserByNameAsync(model.Name);
            if (targetUser == null)
                throw new Exception("User does not exist");

            var passwordVerified = hasher.VerifyHash(targetUser, targetUser.PasswordHash, model.Password);     
            if (passwordVerified == false)
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

        [HttpGet]
        public async Task<IActionResult> Detail(string name)
        {
            var user = await repository.GetUserByNameAsync(name);

            if (user == null)
                throw new Exception("User does not exist");

            return View(user);
        }
    }
}