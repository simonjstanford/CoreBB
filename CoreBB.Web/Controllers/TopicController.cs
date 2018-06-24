using CoreBB.Web.Interfaces;
using CoreBB.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<IActionResult> Index(int id)
        {
            var forum = await repository.GetForumAsync(id);
            forum.Topic = await repository.GetTopicsAsync(id);
            return View(forum);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var forum = await repository.GetForumAsync(id);

            if (forum.IsLocked)
                throw new Exception("Forum is locked");

            var topic = new Topic { ForumId = id };
            return View(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind(include: "ForumId,Title,Content,IsLocked")] Topic topic)
        {
            if (!ModelState.IsValid)
                throw new Exception("Invalid topic information");

            var user = await repository.GetUserByNameAsync(User.Identity.Name);
            topic.OwnerId = user.Id;
            topic.PostDateTime = DateTime.Now;
            await repository.AddTopicAsync(topic);
            topic.RootTopicId = topic.Id;
            await repository.SaveTopicAsync(topic);

            return RedirectToAction(nameof(Index), new { id = topic.ForumId });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var rootTopic = await repository.GetTopicAsync(id);
            return View(rootTopic);
        }

        [HttpGet]
        public async Task<IActionResult> Reply(int toid)
        {
            var toTopic = await repository.GetTopicAsync(toid);

            if (toTopic.IsLocked)
                throw new Exception("The topic is locked");

            var topic = new Topic
            {
                ReplyToTopicId = toTopic.Id,
                RootTopicId = toTopic.RootTopicId,
                ForumId = toTopic.ForumId,
                ReplyToTopic = toTopic,
            };

            return View(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Reply(Topic topic)
        {
            if (ModelState.IsValid == false)
                throw new Exception("Invalid topic information");

            var user = await repository.GetUserByNameAsync(User.Identity.Name);
            topic.OwnerId = user.Id;
            topic.PostDateTime = DateTime.Now;
            await repository.SaveTopicAsync(topic);
            return RedirectToAction(nameof(Detail), new { id = topic.RootTopicId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Topic topic = await GetTopicWithAccessVerification(id);
            return View(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Topic topic)
        {
            if (ModelState.IsValid == false)
                throw new Exception("Invalid topic information");

            var user = await repository.GetUserByNameAsync(User.Identity.Name);
            topic.ModifiedByUserId = user.Id;
            topic.ModifyDateTime = DateTime.Now;
            await repository.SaveTopicAsync(topic);
            return RedirectToAction(nameof(Detail), new { id = topic.RootTopicId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Topic topic = await GetTopicWithAccessVerification(id);
            return View(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Topic topic)
        {
            if (ModelState.IsValid == false)
                throw new Exception("Invalid topic information");

            Topic topicToDelete = await GetTopicWithAccessVerification(topic.Id);
            await repository.DeleteTopicAsync(topicToDelete);
            return RedirectToAction(nameof(Index), new { id = topicToDelete.ForumId });
        }

        private async Task<Topic> GetTopicWithAccessVerification(int topicId)
        {
            Task<Topic> topicTask = repository.GetTopicAsync(topicId);
            Task<User> userTask = repository.GetUserByNameAsync(User.Identity.Name);
            await Task.WhenAll(new Task[] { topicTask, userTask });
            Topic topic = topicTask.Result;
            User user = userTask.Result;
            if ((topic.OwnerId == user.Id) == false || User.IsInRole(Roles.Administrator) == false)
                throw new Exception("Access denied");
            return topic;
        }
    }
}
