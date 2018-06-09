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

            var targetUser = await GetUser(model.Name);

            var passwordVerified = hasher.VerifyHash(targetUser, targetUser.PasswordHash, model.Password);     
            if (passwordVerified == false)
                throw new Exception("The password is wrong");

            await login.LogInUserAsync(targetUser, HttpContext);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private async Task<User> GetUser(string userName)
        {
            var user = await repository.GetUserByNameAsync(userName);
            if (user == null)
                throw new Exception("User does not exist");
            return user;
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
            var user = await GetUser(name);
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string name)
        {
            if (User.Identity.Name != name && !User.IsInRole(Roles.Administrator))
                throw new Exception("Operation denied");

            var user = await GetUser(name);
            var model = UserEditViewModel.FromUser(user);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
                throw new Exception("Invalid user information");

            User user = await GetUser(model.Name);
            ChangePassword(model, user);
            user.Description = model.Description;
            ChangeAdministratorSettings(model, user);
            await repository.SaveUserAsync(user);

            return RedirectToAction(nameof(Detail), new { name = user.Name });
        }

        private void ChangePassword(UserEditViewModel model, User user)
        {
            if (!string.IsNullOrEmpty(model.Password))
            {
                model.Password = model.Password.Trim();
                model.RepeatPassword = model.RepeatPassword.Trim();
                if (model.Password != model.RepeatPassword)
                    throw new Exception("Passwords are not identical");

                if (!User.IsInRole(Roles.Administrator))
                {
                    var hashVerified = hasher.VerifyHash(user, user.PasswordHash, model.CurrentPassword);
                    if (hashVerified == false)
                        throw new Exception("Incorrect user password");
                }

                user.PasswordHash = hasher.HashPassword(model.Password, user);
            }
        }

        private void ChangeAdministratorSettings(UserEditViewModel model, User user)
        {
            if (User.IsInRole(Roles.Administrator))
            {
                user.IsAdministrator = model.IsAdministrator;
                user.IsLocked = model.IsLocked;
            }
        }
    }
}