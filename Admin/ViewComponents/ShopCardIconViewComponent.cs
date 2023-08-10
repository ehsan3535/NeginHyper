using AutoMapper;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Entities.ShopCards;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Client.ViewComponents
{
    public class ShopCardIconViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<ShopCard> shopcardrepo;
        private readonly IRepository<ShopCardDetail> shopCardDetailRepo;
        private readonly IMapper mapper;

        public ShopCardIconViewComponent(UserManager<User> userManager, IRepository<ShopCard> shopcardrepo, IMapper mapper, IRepository<ShopCardDetail> shopCardDetailRepo)
        {
            this.userManager = userManager;
            this.shopcardrepo = shopcardrepo;
            this.mapper = mapper;
            this.shopCardDetailRepo = shopCardDetailRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ClientId)
        {
            int ShopCardCount = 0;
            var ShopCard = new ShopCard();
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                ShopCard = await shopcardrepo.TableNoTracking.FirstOrDefaultAsync(x => x.UserId == User.Id);
            }
            else
            {

                if (!ClientId.HasValue())
                {
                    ClientId = Guid.Empty.ToString();
                }
                Guid CookieId = Guid.Parse(ClientId);

                ShopCard = await shopcardrepo.TableNoTracking.FirstOrDefaultAsync(x => x.CookieId == CookieId);
            }
            if (ShopCard != null)
            {
                var ShopCardDetail = await shopCardDetailRepo.TableNoTracking.Where(x => x.ShopCardId == ShopCard.Id).ToListAsync();
                ShopCardCount = ShopCardDetail.Count();
            }


            return View("ShopCardIcon", ShopCardCount);
        }
    }
}
