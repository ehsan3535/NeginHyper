using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Entities.CityProvince;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Secretary.Models;
using Secretary.Models.ShopCards;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ContactController : Controller
    {

        private readonly UserManager<User> userManager;
        private readonly IRepository<Contact> contactRepo;
        private readonly IRepository<Partnership> PartnerShipRepo;
        private readonly IRepository<OurClient> OurClientRepo;
        private readonly IRepository<PartnerShipProducts> PartnerShipProductRepo;
        private readonly IRepository<City> CityRepo;
        private readonly IRepository<newsletter> newsletterRepo;
        private readonly IRepository<BuyBulk> buybulkrepo;
        private readonly IMapper mapper;

        public ContactController(UserManager<User> userManager, IRepository<Contact> contactRepo, IRepository<Partnership> partnerShipRepo, IRepository<City> cityRepo, IMapper mapper, IRepository<PartnerShipProducts> partnerShipProductRepo, IRepository<newsletter> newsletterRepo, IRepository<BuyBulk> buybulkrepo, IRepository<OurClient> ourClientRepo)
        {
            this.userManager = userManager;
            this.contactRepo = contactRepo;
            PartnerShipRepo = partnerShipRepo;
            CityRepo = cityRepo;
            this.mapper = mapper;
            PartnerShipProductRepo = partnerShipProductRepo;
            this.newsletterRepo = newsletterRepo;
            this.buybulkrepo = buybulkrepo;
            OurClientRepo = ourClientRepo;
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

        public async Task<IActionResult> PartnerShipList(CancellationToken cancellationToken)
        {
            var model = await PartnerShipRepo.TableNoTracking.ProjectTo<partnershipDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            foreach (var item in model)
            {
                item.Province = CityRepo.TableNoTracking.ProjectTo<CityDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == item.CityId).Result.ProvinceName;
            }
            return View(model);
        }

        public async Task<IActionResult> ShowPartnerShip(Guid PartnershipId, CancellationToken cancellationToken)
        {
            var model = await PartnerShipRepo.TableNoTracking.Where(x => x.Id == PartnershipId).ProjectTo<partnershipDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            model.Province = CityRepo.TableNoTracking.ProjectTo<CityDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == model.CityId).Result.ProvinceName;
            model.PartnerShipProduct = PartnerShipProductRepo.TableNoTracking.Where(x => x.PartnershipId == model.Id).ProjectTo<PartnerShipProductDto>(mapper.ConfigurationProvider).ToList();
            return View(model);
        }
        public async Task<IActionResult> NewsletterList(CancellationToken cancellationToken)
        {
            var model = await newsletterRepo.TableNoTracking.ProjectTo<newsletterDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }

        public async Task<IActionResult> BuybulkList(CancellationToken cancellationToken)
        {
            var model = await buybulkrepo.TableNoTracking.ProjectTo<BuyBulkDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }

        #region Our Client

        public async Task<IActionResult> Add_Edit_OurClient(Guid? Id, CancellationToken cancellationToken)
        {
            var model = await OurClientRepo.TableNoTracking.Where(x => x.Id == Id).ProjectTo<OurClientDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_OurClient(OurClientDto Dto, CancellationToken cancellationToken)
        {
            if (Dto.Id == Guid.Empty)
            {
                var Client = Dto.ToEntity(mapper);
                if (Dto.File != null)
                {
                    Client.ImageLink = UploadImage.SaveImage(Dto.File, "OurClientImage");
                }

                await OurClientRepo.AddAsync(Client, cancellationToken);
            }
            else
            {
                var Client = await OurClientRepo.TableNoTracking.FirstOrDefaultAsync(cancellationToken);
                if (Dto.File == null && Client.ImageLink != null)
                {
                    Dto.ImageLink = Client.ImageLink;
                }

                Client = Dto.ToEntity(mapper, Client);

                if (Dto.File != null)
                {
                    Client.ImageLink = UploadImage.SaveImage(Dto.File, "OurClientImage");
                }
                await OurClientRepo.UpdateAsync(Client, cancellationToken);
            }
            return RedirectToAction("OurClientList", "Contact");
        }

        public async Task<IActionResult> OurClientList(CancellationToken cancellationToken)
        {
            var model = await OurClientRepo.TableNoTracking.ProjectTo<OurClientDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }
        public async Task<IActionResult> DeleteOurClient(Guid Id, CancellationToken cancellationToken)
        {
            var model = await OurClientRepo.TableNoTracking.Where(x => x.Id == Id).FirstOrDefaultAsync(cancellationToken);
            model.Active = false;
            OurClientRepo.Update(model);
            return RedirectToAction(nameof(OurClientList));
        }



        #endregion
    }
}
