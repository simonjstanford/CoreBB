using System;
using CoreBB.Web.Models;

namespace CoreBB.Web.Models
{
    public class UserEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsLocked { get; set; }
        public DateTime RegisterDateTime { get; set; }
        public DateTime LastLogInDateTime { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }

        internal static object FromUser(User user)
        {
            return new UserEditViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Description = user.Description,
                RegisterDateTime = user.RegisterDateTime,
                LastLogInDateTime = user.LastLogInDateTime,
                IsAdministrator = user.IsAdministrator,
                IsLocked = user.IsLocked,
            };
        }
    }
}