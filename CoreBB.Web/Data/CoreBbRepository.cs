using CoreBB.Web.Interfaces;
using CoreBB.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBB.Web.Data
{
    public class CoreBBRepository : IRepository
    {
        CoreBBContext context;

        public CoreBBRepository(CoreBBContext context)
        {
            this.context = context;
        }

        public int UserCount => context.User.Count();
        public int TopicCount => context.Topic.Count();
        public int ForumCount => context.Forum.Count();

        public async Task AddUser(User targetUser)
        {
            await context.User.AddAsync(targetUser);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await context.User.ToListAsync();
        }

        public async Task<User> GetUserByNameAsync(string name)
        {
            return await context.User.SingleOrDefaultAsync(u => u.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task SaveUserAsync(User user)
        {
            context.User.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task SetLastLoginTime(User user, DateTime now)
        {
            user.LastLogInDateTime = DateTime.Now;
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Forum>> GetAllForumsAsync()
        {
            var forums = await context.Forum.Include(x => x.Owner).ToListAsync();

            foreach (var forum in forums)
            {
                var TopicCount = await context.Topic.Where(x => x.ForumId == forum.Id && x.ReplyToTopicId == null).CountAsync();
            }

            return forums;
        }

        public async Task<int> GetUserId(string name)
        {
            var user = await context.User.SingleOrDefaultAsync(x => x.Name == name);
            return user.Id;
        }

        public async Task AddForum(Forum forum)
        {
            await context.Forum.AddAsync(forum);
            await context.SaveChangesAsync();
        }
    }
}
