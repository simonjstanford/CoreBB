using CoreBB.Web.Interfaces;
using CoreBB.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreBB.Web.Helpers
{
    public class LoginHelper : ILogin
    {
        private IRepository repository;

        public LoginHelper(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task LogInUserAsync(User user, HttpContext httpContext)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            if (user.IsAdministrator)
                claims.Add(new Claim(ClaimTypes.Role, Roles.Administrator));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            await repository.SetLastLoginTime(user, DateTime.Now);
        }
    }
}
