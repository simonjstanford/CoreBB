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
        private CoreBBContext coreBBContext;

        public LoginHelper(CoreBBContext coreBBContext)
        {
            this.coreBBContext = coreBBContext;
        }

        public async Task LogInUserAsync(User user, HttpContext httpContext)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            if (user.IsAdministrator)
                claims.Add(new Claim(ClaimTypes.Role, Roles.Adminstrator));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            user.LastLogInDateTime = DateTime.Now;
            await coreBBContext.SaveChangesAsync();
        }
    }
}
