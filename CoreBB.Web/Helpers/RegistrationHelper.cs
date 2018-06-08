using CoreBB.Web.Interfaces;
using CoreBB.Web.Models;
using System;
using System.Threading.Tasks;

namespace CoreBB.Web.Helpers
{
    public class RegistrationHelper : IRegister
    {
        private IRepository repository;
        private IHasher hasher;

        public RegistrationHelper(IRepository repository, IHasher hasher)
        {
            this.repository = repository;
            this.hasher = hasher;
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
            var targetUser = repository.GetUserByNameAsync(model.Name);

            if (targetUser != null)
                throw new Exception("User name already exists");
        }

        private async Task<User> CreateUser(RegisterViewModel model)
        {
            var targetUser = CreateUserInstance(model);
            targetUser.PasswordHash = hasher.HashPassword(model.Password, targetUser);

            if (repository.UserCount == 0)
                targetUser.IsAdministrator = true;

            await repository.AddUser(targetUser);
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

        
    }
}
