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
    public class ContactController : Controller
    {

        private readonly UserManager<User> userManager;
        private readonly IRepository<Contact> contactRepo;
        private readonly IRepository<Counselor_SignUp> Counselor_SignUpRepo;
        private readonly IRepository<Lesson> lessonrepo;
        private readonly IMapper mapper;
        public ContactController(UserManager<User> userManager, IMapper mapper, IRepository<Contact> contactRepo, IRepository<Lesson> lessonrepo, IRepository<Counselor_SignUp> counselor_SignUpRepo)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.contactRepo = contactRepo;
            this.lessonrepo = lessonrepo;
            Counselor_SignUpRepo = counselor_SignUpRepo;
        }
        public async Task<IActionResult> ContactList(CancellationToken cancellationToken)
        {
            var model = await contactRepo.TableNoTracking.ProjectTo<ContactDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }

        public async Task<IActionResult> ShowContactUs(Guid ContactId, CancellationToken cancellationToken)
        {
            var model = await contactRepo.TableNoTracking.Where(x => x.Id == ContactId).ProjectTo<ContactDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }

        public async Task<IActionResult> CounselorSignup_List(CancellationToken cancellationToken)
        {
            var model = await Counselor_SignUpRepo.TableNoTracking.ProjectTo<Counselor_SignUpDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }
    }
}
