
using Secretary.Models;
using Secretary.Models.NotificationDto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Entities;
using Entities.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers.Notification1
{
    public class NotificationController : Controller
    {
        private readonly IMapper mapper;
        private readonly IRepository<Notification> notificationrepo;
        private readonly IToastNotification notification;
        private readonly UserManager<User> userManager;
        public NotificationController(IMapper mapper, IRepository<Notification> notificationrepo, UserManager<User> userManager, IToastNotification notification)
        {
            this.mapper = mapper;
            this.notificationrepo = notificationrepo;
            this.userManager = userManager;
            this.notification = notification;
        }
        public async Task<IActionResult> Add_Edit_Notification(Guid NotificationId, CancellationToken cancellationToken)
        {
            var model = await notificationrepo.TableNoTracking.Where(x => x.Id == NotificationId).ProjectTo<NotificationDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            if (model == null)
            {
                model = new();
            }
            model.Users = userManager.Users.ProjectTo<UserDto>(mapper.ConfigurationProvider).ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_Notification(NotificationDto Dto, CancellationToken cancellationToken)
        {
            if (Dto.Id == Guid.Empty)
            {
                if (Dto.UserId == Guid.Empty)
                {
                    notification.AddErrorToastMessage("کاربر انتخاب نشده");
                    return RedirectToAction("NotificationList");
                }
                var model = Dto.ToEntity(mapper);
                await notificationrepo.AddAsync(model, cancellationToken);
            }
            else
            {
                var model = notificationrepo.GetById(Dto.Id);
                var User = userManager.Users.FirstOrDefault(x => x.Id == Dto.UserId);
                model = Dto.ToEntity(mapper, model);
                model.UserId = User.Id;
                model.DateTimes = DateTime.Now;
                await notificationrepo.UpdateAsync(model, cancellationToken);
            }
            return RedirectToAction("NotificationList");
        }
        public async Task<IActionResult> NotificationList()
        {
            var model = notificationrepo.AllTableNoTracking.ProjectTo<NotificationDto>(mapper.ConfigurationProvider).ToList();
            if (model != null)
            {
                return View(model);
            }
            return RedirectToAction("index", "Home");
        }
        public async Task<IActionResult> DeleteNotification(Guid NotificationId, CancellationToken cancellationToken)
        {
            var model = notificationrepo.GetById(NotificationId);
            await notificationrepo.DeleteAsync(model, cancellationToken);
            return RedirectToAction("NotificationList");
        }
        public async Task<IActionResult> NotificationDetail(Guid NotificationId, CancellationToken cancellationToken)
        {
            var model = await notificationrepo.TableNoTracking.Where(x => x.Id == NotificationId).ProjectTo<NotificationDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            return View(model);
        }
    }
}
