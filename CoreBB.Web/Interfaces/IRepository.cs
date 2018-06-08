﻿using CoreBB.Web.Models;
using System;
using System.Threading.Tasks;

namespace CoreBB.Web.Interfaces
{
    public interface IRepository
    {
        int UserCount { get; }
        User GetUserByName(string name);
        Task AddUser(User targetUser);
        Task SetLastLoginTime(User user, DateTime now);
    }
}
