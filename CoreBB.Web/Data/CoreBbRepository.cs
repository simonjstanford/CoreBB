using CoreBB.Web.Interfaces;
using CoreBB.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<User> GetUserByNameAsync(string name)
        {
            return await context.User.SingleOrDefaultAsync(u => u.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task SetLastLoginTime(User user, DateTime now)
        {
            user.LastLogInDateTime = DateTime.Now;
            await context.SaveChangesAsync();
        }
    }
}
