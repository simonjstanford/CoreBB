using CoreBB.Web.Models;

namespace CoreBB.Web.Interfaces
{
    public interface IHasher
    {
        string HashPassword(string password, User targetUser);
        bool VerifyHash(User targetUser, string hash, string password);
    }
}
