using CoreBB.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CoreBB.Web.Helpers
{
    public interface ILogin
    {
        Task LogInUserAsync(User user, HttpContext httpContext);
    }
}