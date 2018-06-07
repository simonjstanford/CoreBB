using CoreBB.Web.Models;
using System.Threading.Tasks;

namespace CoreBB.Web.Interfaces
{
    public interface IRegister
    {
        Task<User> RegisterUser(RegisterViewModel model);
    }
}