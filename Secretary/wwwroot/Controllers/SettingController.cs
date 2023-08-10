using Secretary.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{

    public class SettingController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<Setting> settingrepo;
        private readonly IMapper mapper;
        public SettingController(UserManager<User> userManager, IMapper mapper, IRepository<Setting> settingrepo)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.settingrepo = settingrepo;
        }
        public async Task<IActionResult> Setting(Guid? settingId, CancellationToken cancellationToken)
        {
            var model = await settingrepo.TableNoTracking.ProjectTo<SettingDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Setting(SettingDto Dto, CancellationToken cancellationToken)
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
    }
}
