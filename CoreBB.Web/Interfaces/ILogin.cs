using CoreBB.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CoreBB.Web.Interfaces
{
    public interface ILogin
    {
        Task LogInUserAsync(User user, HttpContext httpContext);
    }
}