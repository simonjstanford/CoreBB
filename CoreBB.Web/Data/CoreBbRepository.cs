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

        public async Task<List<ForumIndexViewModel>> GetAllForumsAsync()
        {
            var result = new List<ForumIndexViewModel>();
            var forums = await context.Forum.Include(x => x.Owner).ToListAsync();

            foreach (var forum in forums)
            {
                var TopicCount = await context.Topic.Where(x => x.ForumId == forum.Id && x.ReplyToTopicId == null).CountAsync();
                result.Add(new ForumIndexViewModel { Forum = forum, TopicCount = TopicCount });
            }

            return result;
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

        public async Task<Forum> GetForumAsync(int forumId)
        {
            var forum = await context.Forum.Include(x => x.Owner).SingleOrDefaultAsync(x => x.Id == forumId);
            if (forum == null)
                throw new Exception("Forum does not exist");
            return forum;
        }

        public async Task DeleteForumAsync(Forum forumToDelete)
        {
            context.Forum.Remove(forumToDelete);
            await context.SaveChangesAsync();
        }

        public async Task SaveForumAsync(Forum forum)
        {
            context.Forum.Update(forum);
            await context.SaveChangesAsync();
        }

        public async Task<ICollection<Topic>> GetTopicsAsync(int forumId)
        {
            return await context.Topic.Include(t => t.Owner).Where(t => t.ForumId == forumId && t.ReplyToTopicId == null).ToListAsync();
        }

        public async Task AddTopicAsync(Topic topic)
        {
            context.Topic.Add(topic);
            await context.SaveChangesAsync();
        }

        public async Task SaveTopicAsync(Topic topic)
        {
            context.Topic.Update(topic);
            await context.SaveChangesAsync();
        }

        public async Task<Topic> GetTopicAsync(int topicId)
        {
            var topic = await context.Topic.Include(x => x.Owner).Include(x => x.ModifiedByUser).FirstOrDefaultAsync(t => t.Id == topicId);
            if (topic == null)
                throw new Exception("Topic does not exist");

            topic.InverseReplyToTopic = await context.Topic.Include(x => x.Owner).Include(x => x.ModifiedByUser).Where(t => t.RootTopicId == topicId && t.ReplyToTopicId != null).ToListAsync();

            return topic;
        }
    }
}
