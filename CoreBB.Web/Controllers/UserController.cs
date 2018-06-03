using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public UserController(CoreBBContext coreBBContext)
        {
            this.coreBBContext = coreBBContext;
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
    }
}