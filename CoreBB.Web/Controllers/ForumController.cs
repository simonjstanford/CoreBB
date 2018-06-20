using System;
using System.Threading.Tasks;
using CoreBB.Web.Interfaces;
using CoreBB.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBB.Web.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private IRepository repository;
        
        public ForumController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var forums = await repository.GetAllForumsAsync();
            return View(forums);
        }

        [HttpGet, Authorize(Roles = Roles.Administrator)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Create(Forum forum)
        {
            if (ModelState.IsValid == false)
                throw new Exception("Invalid forum information");

            forum.OwnerId = await repository.GetUserId(User.Identity.Name);
            forum.CreateDateTime = DateTime.Now;
            await repository.AddForum(forum);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int forumId)
        {
            var forum = await repository.GetForumAsync(forumId);
            return View(forum);
        }

        [HttpGet, Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Edit(int forumId)
        {
            var forum = await repository.GetForumAsync(forumId);
            return View(forum);
        }

        [HttpPost, Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Edit(Forum forum)
        {
            if (ModelState.IsValid == false)
                throw new Exception("Invalid forum information");

            await repository.SaveForumAsync(forum);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet, Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Delete(int forumId)
        {
            var forum = await repository.GetForumAsync(forumId);
            return View(forum);
        }

        [HttpPost, Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Delete(Forum forum)
        {
            var forumToDelete = await repository.GetForumAsync(forum.Id);
            await repository.DeleteForumAsync(forumToDelete);
            return RedirectToAction(nameof(Index));
        }
    }
}