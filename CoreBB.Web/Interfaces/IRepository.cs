using CoreBB.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBB.Web.Interfaces
{
    public interface IRepository
    {
        int UserCount { get; }
        int TopicCount { get; }
        int ForumCount { get; }

        Task<User> GetUserByNameAsync(string name);
        Task AddUser(User targetUser);
        Task SetLastLoginTime(User user, DateTime now);
        Task SaveUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<Forum>> GetAllForumsAsync();
    }
}
