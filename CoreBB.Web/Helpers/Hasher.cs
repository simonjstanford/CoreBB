using CoreBB.Web.Interfaces;
using CoreBB.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace CoreBB.Web.Helpers
{
    public class Hasher : IHasher
    {
        private PasswordHasher<User> hasher = new PasswordHasher<User>();

        public string HashPassword(string password, User targetUser)
        {
            return hasher.HashPassword(targetUser, password);
        }

        public bool VerifyHash(User targetUser, string hash, string password)
        {
            return hasher.VerifyHashedPassword(targetUser, targetUser.PasswordHash, password) == PasswordVerificationResult.Success;
        }
    }
}
