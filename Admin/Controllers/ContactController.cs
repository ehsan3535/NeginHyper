using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models;
using Client.Models.ShopCards;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Entities.CityProvince;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Client.Controllers
{

    public class ContactController : Controller
    {

        private readonly UserManager<User> userManager;
        private readonly IRepository<Contact> contactRepo;
        private readonly IRepository<Partnership> partnershiprepo;
        private readonly IRepository<PartnerShipProducts> PartnerShipProductsRepo;
        private readonly IRepository<Province> provineRepo;
        private readonly IRepository<BuyBulk> buybulkRepo;
        private readonly IMapper mapper;
        private readonly IToastNotification notification;

        public ContactController(UserManager<User> userManager, IRepository<Contact> contactRepo, IMapper mapper, IToastNotification notification, IRepository<Partnership> partnershiprepo, IRepository<Province> provineRepo, IRepository<PartnerShipProducts> partnerShipProductsRepo, IRepository<BuyBulk> buybulkRepo)
        {
            this.userManager = userManager;
            this.contactRepo = contactRepo;
            this.mapper = mapper;
            this.notification = notification;
            this.partnershiprepo = partnershiprepo;
            this.provineRepo = provineRepo;
            PartnerShipProductsRepo = partnerShipProductsRepo;
            this.buybulkRepo = buybulkRepo;
        }

        public async Task<IActionResult> AboutUs(CancellationToken cancellationToken)
        {
            return View();
        }
        public async Task<IActionResult> BuyBulk(CancellationToken cancellationToken)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BuyBulk(BuyBulkDto Dto, CancellationToken cancellationToken)
        {
            var BuyBulk = Dto.ToEntity(mapper);
            await buybulkRepo.AddAsync(BuyBulk, cancellationToken);
            notification.AddInfoToastMessage("درخواست شما با موفقیت ارسال شد.");
            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> ContactUs(CancellationToken cancellationToken)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactDto Dto, CancellationToken cancellationToken)
        {
            var Contact = Dto.ToEntity(mapper);
            await contactRepo.AddAsync(Contact, cancellationToken);
            notification.AddInfoToastMessage("درخواست شما با موفقیت ارسال شد.");
            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> Partnership(CancellationToken cancellationToken)
        {
            var model = new partnershipDto();
            model.Provinces = await provineRepo.TableNoTracking.Where(x=>x.Id == Guid.Parse("1fc718be-0810-ee11-a9d7-8ca6b29b3f38")).ProjectTo<ProvinceDto>(mapper.ConfigurationProvider).ToListAsync();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Partnership(partnershipDto Dto, CancellationToken cancellationToken)
        {
            var Partnership = Dto.ToEntity(mapper);
            await partnershiprepo.AddAsync(Partnership, cancellationToken);
            int g = 0;
            foreach (var item in Dto.CountList.Trim().Split(","))
            {
                if (!item.HasValue())
                {
                    continue;
                }
                PartnerShipProductsRepo.Add(new PartnerShipProducts
                {
                    Name = Dto.ProductList.Trim().Split(",")[g],
                    Count = Dto.CountList.Trim().Split(",")[g],
                    PartnershipId = Partnership.Id,
                });
                g++;
            }
            
            //int i = 0;
            //int j = 0;
            //foreach (var item in Dto.ProductList.Trim().Split(","))
            //{
            //    if (!item.HasValue())
            //    {
            //        continue;
            //    }
            //    j = 0;
            //    foreach (var item2 in Dto.CountList.Trim().Split(","))
            //    {
            //        if (!item2.HasValue())
            //        {
            //            continue;
            //        }
            //        if (i == j)
            //        {
            //            PartnerShipProductsRepo.Add(new PartnerShipProducts
            //            {
            //                Name = item,
            //                Count = item2,
            //                PartnershipId = Partnership.Id,
            //            });
            //        }
            //        j++;
            //    }
            //    i++;
            //}
            notification.AddInfoToastMessage("درخواست شما با موفقیت ارسال شد.");
            return RedirectToAction("index", "home");
        }
    }
}
