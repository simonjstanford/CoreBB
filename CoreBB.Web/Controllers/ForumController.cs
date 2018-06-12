using System;
using System.Collections.Generic;
using System.Linq;
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
            ViewBag.TopicCount = repository.TopicCount;
            ViewBag.ForumCount = repository.ForumCount;
            return View(forums);
        }
    }
}