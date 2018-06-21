using CoreBB.Web.Interfaces;
using CoreBB.Web.Models;
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

        [HttpGet]
        public async Task<IActionResult> Create(int forumId)
        {
            var forum = await repository.GetForumAsync(forumId);

            if (forum.IsLocked)
                throw new Exception("Forum is locked");

            var topic = new Topic { ForumId = forumId };
            return View(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Topic topic)
        {
            if (!ModelState.IsValid)
                throw new Exception("Invalid topic information");

            var user = await repository.GetUserByNameAsync(User.Identity.Name);
            topic.OwnerId = user.Id;
            topic.PostDateTime = DateTime.Now;
            await repository.AddTopicAsync(topic);
            topic.RootTopicId = topic.Id;
            await repository.SaveTopicAsync(topic);

            return RedirectToAction(nameof(Index), new { forumId = topic.ForumId });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var rootTopic = await repository.GetTopicAsync(id);
            return View(rootTopic);
        }
    }
}
