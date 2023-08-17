using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Repositories;
using DNTPersianUtils.Core;
using Entities;
using Entities.FreeTime;
using Entities.Notifications;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Secretary.Models;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{
    [Authorize(Roles = "Admin")]

    public class SettingController : Controller
    {
        private readonly UserManager<Entities.User> userManager;
        private readonly IRepository<Setting> settingrepo;
        private readonly IRepository<FreeTime> freeTimeRepo;
        private readonly IToastNotification notification;
        private readonly IMapper mapper;
        public SettingController(UserManager<Entities.User> userManager, IMapper mapper, IRepository<Setting> settingrepo, IRepository<FreeTime> freeTimeRepo, IToastNotification notification)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.settingrepo = settingrepo;
            this.freeTimeRepo = freeTimeRepo;
            this.notification = notification;
        }
        public async Task<IActionResult> Setting(Guid? settingId, CancellationToken cancellationToken)
        {
            var model = await settingrepo.TableNoTracking.ProjectTo<SettingDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            if (model.SeggestDate == DateTime.MinValue)
            {
                //for eror
                model.SeggestDate = DateTime.Now;
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Setting(SettingDto Dto, CancellationToken cancellationToken)
        {
            if (Dto.Id == Guid.Empty)
            {
                var Setting = Dto.ToEntity(mapper);
                if (Dto.TopImageFile1 != null)
                {
                    Setting.TopImageLink1 = UploadImage.SaveImage(Dto.TopImageFile1, "TopImageFile1");
                }
                if (Dto.TopImageFile2 != null)
                {
                    Setting.TopImageLink2 = UploadImage.SaveImage(Dto.TopImageFile2, "TopImageFile2");
                }
                if (Dto.TopImageFile3 != null)
                {
                    Setting.TopImageLink3 = UploadImage.SaveImage(Dto.TopImageFile3, "TopImageFile3");
                }
                if (Dto.BottemRightImageFile != null)
                {
                    Setting.BottemRightImageLink = UploadImage.SaveImage(Dto.BottemRightImageFile, "BottemRightImageFile");
                }
                if (Dto.BottemLeftImageFile != null)
                {
                    Setting.BottemLeftImageLink = UploadImage.SaveImage(Dto.BottemLeftImageFile, "BottemLeftImageFile");
                }
                await settingrepo.AddAsync(Setting, cancellationToken);
            }
            else
            {
                var Setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync(cancellationToken);
                if (Dto.TopImageFile1 == null && Setting.TopImageLink1 != null)
                {
                    Dto.TopImageLink1 = Setting.TopImageLink1;
                }
                if (Dto.TopImageFile2 == null && Setting.TopImageLink2 != null)
                {
                    Dto.TopImageLink2 = Setting.TopImageLink2;
                }
                if (Dto.TopImageFile3 == null && Setting.TopImageLink3 != null)
                {
                    Dto.TopImageLink3 = Setting.TopImageLink3;
                }
                if (Dto.BottemRightImageFile == null && Setting.BottemRightImageLink != null)
                {
                    Dto.BottemRightImageLink = Setting.BottemRightImageLink;
                }
                if (Dto.BottemLeftImageFile == null && Setting.BottemLeftImageLink != null)
                {
                    Dto.BottemLeftImageLink = Setting.BottemLeftImageLink;
                }

                {
                    if (Dto.SeggestDate2.Split(" ").Count() >1)
                    {
                        Dto.SeggestDate = Setting.SeggestDate;
                    }
                    else
                    {
                        Dto.SeggestDate = $"{Dto.SeggestDate2}".ToGregorianDateTime() ?? Setting.SeggestDate;
                    }
                    Setting = Dto.ToEntity(mapper, Setting);
                }
                if (Dto.TopImageFile1 != null)
                {
                    Setting.TopImageLink1 = UploadImage.SaveImage(Dto.TopImageFile1, "TopImageFile1");
                }
                if (Dto.TopImageFile2 != null)
                {
                    Setting.TopImageLink2 = UploadImage.SaveImage(Dto.TopImageFile2, "TopImageFile2");
                }
                if (Dto.TopImageFile3 != null)
                {
                    Setting.TopImageLink3 = UploadImage.SaveImage(Dto.TopImageFile3, "TopImageFile3");
                }
                if (Dto.BottemRightImageFile != null)
                {
                    Setting.BottemRightImageLink = UploadImage.SaveImage(Dto.BottemRightImageFile, "BottemRightImageFile");
                }
                if (Dto.BottemLeftImageFile != null)
                {
                    Setting.BottemLeftImageLink = UploadImage.SaveImage(Dto.BottemLeftImageFile, "BottemLeftImageFile");
                }
                //{
                //    var Year = Dto.SeggestDate.Year;
                //    var month = Dto.SeggestDate.Month;
                //    var day = Dto.SeggestDate.Day;
                //    Setting.SeggestDate = $"{Year}/{month}/{day}".ToGregorianDateTime() ?? DateTime.Now;
                //}
                //Setting.SeggestDate = DNTPersianUtils.Core.PersianDateTimeUtils.ToGregorianDateTime($"{Year}/{month}/{day}") ?? DateTime.Now;

                await settingrepo.UpdateAsync(Setting, cancellationToken);
            }
            return RedirectToAction("Dashboard", "Dashboard");
        }
        #region FreeTime
        public async Task<IActionResult> FreeTimes(DateTime Date, CancellationToken cancellationToken)
        {
            var model = new List<FreeTimeDto>();
            if (Date != DateTime.MinValue)
            {
                model = await freeTimeRepo.TableNoTracking.Where(x => x.DateTime.Date == Date.Date).ProjectTo<FreeTimeDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            }
            else
            {
                model = await freeTimeRepo.TableNoTracking.Where(x => x.DateTime.Date == DateTime.Now.Date).ProjectTo<FreeTimeDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            }
            model.First().AllTimes = await freeTimeRepo.TableNoTracking.Where(x => x.Out == false).ProjectTo<FreeTimeDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            model.First().AllTimes = model.First().AllTimes.DistinctBy(x => x.Day).ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> FreeTimes(SettingDto Dto, CancellationToken cancellationToken)
        {
            if (Dto.Id == Guid.Empty)
            {
                var Setting = Dto.ToEntity(mapper);
                await settingrepo.AddAsync(Setting, cancellationToken);
            }
            else
            {
                var Setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync(cancellationToken);
                Setting = Dto.ToEntity(mapper, Setting);
                await settingrepo.UpdateAsync(Setting, cancellationToken);
            }
            return RedirectToAction("Dashboard", "Dashboard");
        }

        public async Task<IActionResult> ChangeFreeStatus(Guid id, bool Free, CancellationToken cancellationToken)
        {
            var model = await freeTimeRepo.TableNoTracking.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (Free)
                model.Free = true;
            else
            {
                model.Free = false;
            }
            freeTimeRepo.Update(model);
            return RedirectToAction(nameof(FreeTimes), new { Date = model.DateTime });
        }
        #endregion

        #region Change Admin Password
        public async Task<IActionResult> ChangePass(Guid UserId)
        {
            TempData["userid"] = UserId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePass(ChangePssDto Dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                TempData["userid"] = Dto.UserId;
                //ModelState.AddModelError("Pssword","")
                return View(Dto );
            }
            var User = await userManager.FindByIdAsync(Dto.UserId.ToString());

            string Token = await userManager.GeneratePasswordResetTokenAsync(User);

            var status = await userManager.ResetPasswordAsync(User, Token, Dto.Pssword);
            if (status.Succeeded)
            {
                notification.AddSuccessToastMessage("تغییر رمز با موفقیت انجام شد");
            }
            else
            {
                notification.AddErrorToastMessage("رمز نامعتبر(رمز ورود باید بیش از 6 کارکتر باشد)");
                TempData["userid"] = Dto.UserId;

                return View(Dto);
            }
            return RedirectToAction("DashBoard", "DashBoard");
        }
        #endregion
    }
}
