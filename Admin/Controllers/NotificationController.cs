
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models.NotificationDto;
using Data.Repositories;
using Entities;
using Entities.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Client.Controllers
{

    [Authorize(Roles = "Client")]
    public class NotificationController : Controller
    {
        private readonly IMapper mapper;
        private readonly IRepository<Notification> notificationrepo;
        private readonly UserManager<User> userManager;
        public NotificationController(IMapper mapper, IRepository<Notification> notificationrepo, UserManager<User> userManager)
        {
            this.mapper = mapper;
            this.notificationrepo = notificationrepo;
            this.userManager = userManager;
        }


        public async Task<IActionResult> NotificationList(CancellationToken cancellationToken)
        {
            var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var model = await notificationrepo.TableNoTracking.Where(x => x.UserId == User.Id).ProjectTo<NotificationDto>(mapper.ConfigurationProvider).ToListAsync();
            TempData["Name"] = User.Fname + " " + User.Lname;
            TempData["PhoneNumber"] = User.PhoneNumber;
            return View(model);
        }
    }
}
