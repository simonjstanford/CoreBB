using CoreBB.Web.Interfaces;
using CoreBB.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBB.Web.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private IRepository repository;
        
        public MessageController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User user = await repository.GetUserByNameAsync(User.Identity.Name);
            IEnumerable<Message> messages = await repository.GetMessagesAsync(user.Id);
            return View(messages);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string toUserName)
        {
            User toUser = await repository.GetUserByNameAsync(toUserName);
            Message message = new Message() { ToUserId = toUser.Id, ToUser = toUser };
            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Message message)
        {
            if (ModelState.IsValid == false)
                throw new Exception("Invalid message information");

            User fromUser = await repository.GetUserByNameAsync(User.Identity.Name);
            message.FromUserId = fromUser.Id;
            message.SendDateTime = DateTime.Now;
            await repository.AddMessageAsync(message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Message message = await repository.GetMessageAsync(id);
            User user = await repository.GetUserByNameAsync(User.Identity.Name);

            if (message.ToUserId != user.Id && message.FromUserId != user.Id)
                throw new Exception("Message access denied.");

            if (message.ToUserId == user.Id)
                message.IsRead = true;

            await repository.SaveMessageAsync(message);
            return View(message);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Message message = await repository.GetMessageAsync(id);
            User user = await repository.GetUserByNameAsync(User.Identity.Name);

            if (message.ToUserId != user.Id && message.FromUserId != user.Id)
                throw new Exception("Message access denied.");

            await repository.DeleteMessageAsync(message);
            return RedirectToAction(nameof(Index));
        }
    }
}
