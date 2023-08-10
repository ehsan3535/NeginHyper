using Secretary.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{
    public class InternShipController : Controller
    {

        private readonly UserManager<User> userManager;
        private readonly IRepository<InternShip> internshiprepo;
        private readonly IRepository<Lesson> lessonrepo;
        private readonly IMapper mapper;
        public InternShipController(UserManager<User> userManager, IMapper mapper, IRepository<InternShip> internshiprepo, IRepository<Lesson> lessonrepo)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.internshiprepo = internshiprepo;
            this.lessonrepo = lessonrepo;
        }



        public async Task<IActionResult> InternShipList(CancellationToken cancellationToken)
        {
            var model = await internshiprepo.TableNoTracking.ProjectTo<InternShipDto>(mapper.ConfigurationProvider).ToListAsync();
            return View(model);
        }
        public async Task<IActionResult> ShowInternShip(Guid InternShipId, CancellationToken cancellationToken)
        {
            var model = await internshiprepo.TableNoTracking.Where(x => x.Id == InternShipId).ProjectTo<InternShipDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }
    }
}
