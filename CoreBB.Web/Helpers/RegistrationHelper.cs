using CoreBB.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBB.Web.Helpers
{
    public class RegistrationHelper : IRegister
    {
        private CoreBBContext coreBBContext;

        public RegistrationHelper(CoreBBContext coreBBContext)
        {
            this.coreBBContext = coreBBContext;
        }

        public async Task<User> RegisterUser(RegisterViewModel model)
        {
            TrimUserData(model);
            ValidateUserNotAlreadyRegistered(model);
            ValidatePassword(model);
            var user = await CreateUser(model);
            return user;
        }

        private static void TrimUserData(RegisterViewModel model)
        {
            model.Name = model.Name.Trim();
            model.Password = model.Password.Trim();
            model.RepeatPassword = model.RepeatPassword.Trim();
        }

        private static void ValidatePassword(RegisterViewModel model)
        {
            if (!model.Password.Equals(model.RepeatPassword))
                throw new Exception("Passwords are not identical");
        }

        private void ValidateUserNotAlreadyRegistered(RegisterViewModel model)
        {
            var targetUser = coreBBContext.User.SingleOrDefault(u => u.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));

            if (targetUser != null)
                throw new Exception("User name already exists");
        }

        private async Task<User> CreateUser(RegisterViewModel model)
        {
            var targetUser = CreateUserInstance(model);
            HashPassword(model, targetUser);

            if (coreBBContext.User.Count() == 0)
                targetUser.IsAdministrator = true;

            await coreBBContext.User.AddAsync(targetUser);
            await coreBBContext.SaveChangesAsync();
            return targetUser;
        }

        private static User CreateUserInstance(RegisterViewModel model)
        {
            return new User
            {
                Name = model.Name,
                RegisterDateTime = DateTime.Now,
                Description = model.Description
            };
        }

        private static void HashPassword(RegisterViewModel model, User targetUser)
        {
            var hasher = new PasswordHasher<User>();
            targetUser.PasswordHash = hasher.HashPassword(targetUser, model.Password);
        }
    }
}
