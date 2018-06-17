using CoreBB.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBB.Web.Controllers
{
    [Authorize]
    public class TopicController : Controller
    {
        private IRepository repository;

        public TopicController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int forumId)
        {
            var forum = await repository.GetForumAsync(forumId);
            forum.Topic = await repository.GetTopicsAsync(forumId);
            return View(forum);
        }
    }
}
