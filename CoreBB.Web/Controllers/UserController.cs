using System;
using System.Threading.Tasks;
using CoreBB.Web.Helpers;
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
    }
}