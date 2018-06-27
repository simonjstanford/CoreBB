using CoreBB.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBB.Web.Interfaces
{
    public interface IRepository
    {
        int UserCount { get; }

        Task<User> GetUserByNameAsync(string name);
        Task AddUser(User targetUser);
        Task SetLastLoginTime(User user, DateTime now);
        Task SaveUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<List<ForumIndexViewModel>> GetAllForumsAsync();
        Task<int> GetUserId(string name);
        Task AddForum(Forum forum);
        Task<Forum> GetForumAsync(int forumId);
        Task<ICollection<Topic>> GetTopicsAsync(int forumId);
        Task<IEnumerable<Message>> GetMessagesAsync(int messageId);
        Task DeleteForumAsync(Forum forumToDelete);
        Task SaveForumAsync(Forum forum);
        Task AddTopicAsync(Topic topic);
        Task SaveTopicAsync(Topic topic);
        Task<Topic> GetTopicAsync(int topicId);
        Task DeleteTopicAsync(Topic topicToDelete);
        Task AddMessageAsync(Message message);
        Task<Message> GetMessageAsync(int id);
        Task SaveMessageAsync(Message message);
        Task DeleteMessageAsync(Message message);
    }
}
