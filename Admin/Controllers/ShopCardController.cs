using AutoMapper;
using AutoMapper.QueryableExtensions;
using client.Models;
using Client.Models;
using Client.Models.ShopCards;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Entities.Address;
using Entities.CityProvince;
using Entities.FreeTime;
using Entities.Product;
using Entities.ShopCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Client.Controllers
{
    public class ShopCardController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly IRepository<ShopCard> shopcardrepo;
        private readonly IRepository<ShopCardDetail> shopcardDetailrepo;
        private readonly IRepository<Products> productrepo;
        private readonly IRepository<ProductImage> ProductImageRepo;
        private readonly IRepository<City> CityRepo;
        private readonly IRepository<FreeTime> freeTimeRepo;
        private readonly IRepository<Province> provineRepo;
        private readonly IRepository<Entities.Address.Address> AddressRepo;
        private readonly IRepository<Setting> settingrepo;
        private readonly IRepository<Discount> discountrepo;
        private readonly IToastNotification notification;

        public ShopCardController(IRepository<ShopCardDetail> shopcardDetailrepo, IRepository<ShopCard> shopcardrepo, IMapper mapper, UserManager<User> userManager, IRepository<Products> productrepo, IToastNotification notification, IRepository<Setting> settingrepo, IRepository<ProductImage> productImageRepo, IRepository<Entities.Address.Address> addressRepo, IRepository<FreeTime> freeTimeRepo, IRepository<Discount> discountrepo, IRepository<Province> provineRepo, IRepository<City> cityRepo)
        {
            this.shopcardDetailrepo = shopcardDetailrepo;
            this.shopcardrepo = shopcardrepo;
            this.mapper = mapper;
            this.userManager = userManager;
            this.productrepo = productrepo;
            this.notification = notification;
            this.settingrepo = settingrepo;
            ProductImageRepo = productImageRepo;
            AddressRepo = addressRepo;
            this.freeTimeRepo = freeTimeRepo;
            this.discountrepo = discountrepo;
            this.provineRepo = provineRepo;
            CityRepo = cityRepo;
        }
        [AllowAnonymous]
        public async Task<IActionResult> ShopCard_Detail(string ClientId, CancellationToken cancellationToken)
        {
            if (!ClientId.HasValue())
            {
                return RedirectToAction("index", "home");
            }
            Guid CookieId = Guid.Parse(ClientId);
            if (CookieId == Guid.Empty)
            {
                return RedirectToAction("index", "home");
            }
            var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
            var model = new ShopCardDto();
            var ShopCard = new ShopCard();

            ShopCard = await shopcardrepo.TableNoTracking.Where(x => x.CookieId == CookieId).FirstOrDefaultAsync(cancellationToken);
            if (ShopCard == null)
            {
                ShopCard = new ShopCard()
                {
                    CookieId = CookieId,
                    TotalPrice = 0,
                };
                await shopcardrepo.AddAsync(ShopCard, cancellationToken);
            }
            /*    if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var FindUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    var OldShopCard = await shopcardrepo.TableNoTracking.Where(x => x.UserId == FindUser.Id && x.Id != ShopCard.Id).ToListAsync(cancellationToken);
                    if (OldShopCard.Any())
                    {
                        var OldShopCardDetails = await shopcardrepo.TableNoTracking.Where(x => x.UserId == FindUser.Id && x.Id != ShopCard.Id).SelectMany(x => x.ShopCardDetails).ToListAsync(cancellationToken);
                        if (OldShopCardDetails.Any())
                        {
                            await shopcardDetailrepo.DeleteRangeAsync(OldShopCardDetails, cancellationToken);
                        }
                        await shopcardrepo.DeleteRangeAsync(OldShopCard, cancellationToken);
                    }
                    ShopCard.UserId = FindUser.Id;
                    await shopcardrepo.UpdateAsync(ShopCard, cancellationToken);
                }*/
            model = mapper.Map<ShopCardDto>(ShopCard);
            model.ShopCardDetails = await shopcardDetailrepo.TableNoTracking.Where(x => x.ShopCardId == model.Id).ProjectTo<ShopCardDetailDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            //this if is for full imagelink with first image of productimage entity.
            if (model.ShopCardDetails.Any())
            {
                foreach (var item in model.ShopCardDetails)
                {
                    item.ProductsImageLink = await ProductImageRepo.TableNoTracking.Where(x => x.ProductId == item.ProductsId).Select(t => t.ImageLink).FirstOrDefaultAsync(cancellationToken);
                }
            }
            model.PostPrice = setting.PostPrice;
            if (model.TotalPrice >= 500000)
            {
                model.FinalTotalPrice = model.TotalPrice;
            }
            else
            {
                model.FinalTotalPrice = model.TotalPrice + model.PostPrice;
            }
            model.ClientId = ClientId;
            return View(model);
        }
        public async Task<IActionResult> ShopCard_Detail2(string? ClientId, CancellationToken cancellationToken)
        {
            var FindUser = new User();
            var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
            var model = new ShopCardDto();
            if (ClientId != null)
            {
                Guid CookieId = Guid.Parse(ClientId);
                var ShopCard = new ShopCard();

                ShopCard = await shopcardrepo.TableNoTracking.Where(x => x.CookieId == CookieId).FirstOrDefaultAsync(cancellationToken);
                if (ShopCard == null)
                {
                    ShopCard = new ShopCard()
                    {
                        CookieId = CookieId,
                        TotalPrice = 0,
                    };
                    await shopcardrepo.AddAsync(ShopCard, cancellationToken);
                }
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    FindUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    var OldShopCard = await shopcardrepo.TableNoTracking.Where(x => x.UserId == FindUser.Id && x.Id != ShopCard.Id).ToListAsync(cancellationToken);
                    if (OldShopCard.Any())
                    {
                        var OldShopCardDetails = await shopcardrepo.TableNoTracking.Where(x => x.UserId == FindUser.Id && x.Id != ShopCard.Id).SelectMany(x => x.ShopCardDetails).ToListAsync(cancellationToken);
                        if (OldShopCardDetails.Any())
                        {
                            await shopcardDetailrepo.DeleteRangeAsync(OldShopCardDetails, cancellationToken);
                        }
                        await shopcardrepo.DeleteRangeAsync(OldShopCard, cancellationToken);
                    }
                    ShopCard.UserId = FindUser.Id;
                    await shopcardrepo.UpdateAsync(ShopCard, cancellationToken);
                }
                model = mapper.Map<ShopCardDto>(ShopCard);
            }
            else
            {
                FindUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                model = await shopcardrepo.TableNoTracking.Where(x => x.UserId == FindUser.Id).ProjectTo<ShopCardDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            }

            model.PostPrice = setting.PostPrice;
            if (model.TotalPrice >= 500000)
            {
                model.FinalTotalPrice = model.TotalPrice;
            }
            else
            {
                model.FinalTotalPrice = model.TotalPrice + model.PostPrice;
            }
            model.Addresses = await AddressRepo.TableNoTracking.Where(x => x.ClientId == FindUser.Id).ProjectTo<AddressDto>(mapper.ConfigurationProvider).ToListAsync();
            model.Provinces = await provineRepo.TableNoTracking.ProjectTo<ProvinceDto>(mapper.ConfigurationProvider).ToListAsync();
            //دراپ داون شهر با ای جکس پر میشه
            return View(model);
        }
        public async Task<IActionResult> ShopCard_Detail3(Guid? AddressId, CancellationToken cancellationToken)
        {
            //If addressId is null
            if (AddressId == null)
            {
                notification.AddErrorToastMessage("ابتدا یک آدرس انتخاب کنید");
                return RedirectToAction("ShopCard_Detail2", "ShopCard");
            }
            TempData["AddressId"] = AddressId;
            var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
            var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var model = await shopcardrepo.TableNoTracking.Where(x => x.UserId == User.Id).ProjectTo<ShopCardDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            model.PostPrice = setting.PostPrice;
            var Address = AddressRepo.TableNoTracking.Where(x => x.Id == AddressId).FirstOrDefault();
            var AddressCity = await AddressRepo.TableNoTracking.Where(x => x.Id == AddressId).Select(x => x.City).FirstOrDefaultAsync();
            Guid ShirazId = Guid.Parse("df5d4546-7137-ee11-81b3-f0761c623f70");
            Guid MarvdashtId = Guid.Parse("dd5d4546-7137-ee11-81b3-f0761c623f70");

            if (AddressCity.Id == ShirazId && Address.FromArian)
            {
                model.FreeTimes = freeTimeRepo.TableNoTracking.Where(x => x.Out).ProjectTo<FreeTimeDto>(mapper.ConfigurationProvider).ToList();
            }
            else if (AddressCity.Id == ShirazId || AddressCity.Id == MarvdashtId)
            {
                model.FreeTimes = freeTimeRepo.TableNoTracking.Where(x => x.Out).ProjectTo<FreeTimeDto>(mapper.ConfigurationProvider).ToList();
            }
            else
            {
                model.FreeTimes = freeTimeRepo.TableNoTracking.Where(x => x.Out && x.Title != "ساکن آرین (رایگان)" && x.Title != "ارسال با اسنپ باکس").ProjectTo<FreeTimeDto>(mapper.ConfigurationProvider).ToList();
            }

            if (model.TotalPrice >= 500000 || AddressCity.Id == Guid.Parse("d25d4546-7137-ee11-81b3-f0761c623f70"))
            {
                model.FinalTotalPrice = model.TotalPrice;
            }
            else
            {
                model.FinalTotalPrice = model.TotalPrice + model.PostPrice;
            }
            //var shopcard = model.ToEntity(mapper);
            //shopcardrepo.Update(shopcard);
            model.AddressId = AddressId;
            return View(model);
        }
        public async Task<IActionResult> ShopCard_Detail4(Guid? FreeTimeId, Guid? AddressId, CancellationToken cancellationToken)
        {
            //TempData["AddressId"] = TempData["AddressId"].ToString();
            var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();

            var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var model = await shopcardrepo.TableNoTracking.Where(x => x.UserId == User.Id)
              .ProjectTo<ShopCardDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            model.AddressId = AddressId;
            if (FreeTimeId == null)
            {
                notification.AddErrorToastMessage("ابتدا یک روش برای ارسال انتخاب کنید:");
                return RedirectToAction("ShopCard_Detail3", "ShopCard", new { AddressId = model.AddressId });
            }


            model.Address = await AddressRepo.TableNoTracking.Where(t => t.Id == model.AddressId)
               .ProjectTo<AddressDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);

            var freeTime = freeTimeRepo.GetById(FreeTimeId);
            model.PostPrice = setting.PostPrice;
            var Shopcard = await shopcardrepo.TableNoTracking.Where(x => x.UserId == User.Id).FirstOrDefaultAsync(cancellationToken);
            if (freeTime.Out)
            {
                model.FinalTotalPrice = model.TotalPrice;
                Shopcard.FinalTotalPrice = model.TotalPrice;
                shopcardrepo.Update(Shopcard);
            }
            else if (model.TotalPrice >= 500000)
            {
                model.FinalTotalPrice = model.TotalPrice;
                Shopcard.FinalTotalPrice = model.TotalPrice;
                shopcardrepo.Update(Shopcard);
            }
            else
            {
                model.FinalTotalPrice = model.TotalPrice + model.PostPrice;
                Shopcard.FinalTotalPrice = model.TotalPrice + model.PostPrice;
                shopcardrepo.Update(Shopcard);
            }
            model.Addresses = await AddressRepo.TableNoTracking.Where(x => x.ClientId == User.Id).ProjectTo<AddressDto>(mapper.ConfigurationProvider).ToListAsync();
            model.Address.Id = model.AddressId ?? Guid.Empty;
            model.FreeTimeId = FreeTimeId;
            model.DateTime = freeTime.DateTime;
            model.FromHour = freeTime.FromHour;
            model.Out = freeTime.Out;
            model.FreeTimeTitle = freeTime.Title;
            return View(model);
        }
        public async Task<IActionResult> CheckDiscount(CheckDiscountDto Dto, CancellationToken cancellationToken)
        {
            TempData["AddressId"] = Dto.AddressId;
            var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
            var shopcard = shopcardrepo.TableNoTracking.FirstOrDefault(x => x.Id == Dto.shopcardId);
            var DiscountEntity = discountrepo.TableNoTracking.FirstOrDefault(x => x.Code == Dto.DiscountCode);
            if (DiscountEntity != null)
            {
                //یه رشته از استرینگ از کدهای تخفیفی که تو این شاپ کارت ثبت شده داریم و باید مطابقت بدیم که کد قبلا وارد نشده باشه
                if (shopcard.DiscountCode != null)
                {
                    foreach (var item in shopcard.DiscountCode.Split(","))
                    {
                        if (item != DiscountEntity.Code)
                        {
                            shopcard.DiscountPercent = DiscountEntity.DiscountPercent;
                            shopcard.DiscountCode += DiscountEntity.Code + ",";
                            shopcard.TotalPrice = (shopcard.TotalPrice * (100 - DiscountEntity.DiscountPercent)) / 100;
                            shopcardrepo.Update(shopcard);
                            notification.AddSuccessToastMessage("کد تخفیف اعمال شد.");
                        }
                        else
                        {
                            notification.AddErrorToastMessage("کد تخفیف قبلا استفاده شده .");
                            break;
                        }
                    }
                }
                else
                {
                    shopcard.DiscountPercent = DiscountEntity.DiscountPercent;
                    shopcard.DiscountCode += DiscountEntity.Code + ",";
                    shopcard.TotalPrice = (shopcard.TotalPrice * (100 - DiscountEntity.DiscountPercent)) / 100;
                    shopcardrepo.Update(shopcard);
                    notification.AddSuccessToastMessage("کد تخفیف اعمال شد است.");
                }
            }
            else
            {
                notification.AddErrorToastMessage("کد تخفیف اشتباه است.");
            }
            return RedirectToAction("ShopCard_Detail4", new { FreeTimeId = Dto.FreeTimeId });
        }
        [AllowAnonymous]
        public async Task<string> AddOrRemove(string ClientId, Guid ProductId, int Count, bool AddOrRemove, CancellationToken cancellationToken)
        {
            if (!ClientId.HasValue())
            {
                return "Bad";
            }
            Guid CookieId = Guid.Parse(ClientId);
            if (CookieId == Guid.Empty)
            {
                return "Bad";
            }

            var ShopCard = new ShopCard();
            var FindProduct = await productrepo.TableNoTracking.FirstOrDefaultAsync(x => x.Id == ProductId);

            if (Count <= 1)
            {
                if (FindProduct.Count == 0)
                {
                    notification.AddErrorToastMessage("موجودی این محصول تمام شده است");
                    return $"/Product/ProductDetail_User?id={ProductId}&ClientId={ClientId}";
                }
            }
            else
            {
                if (FindProduct.Count < Count)
                {
                    notification.AddErrorToastMessage("موجودی این محصول از تعداد درخواستی شما کمتر است");
                    return $"/Product/ProductDetail_User?id={ProductId}&ClientId={ClientId}";
                }
            }
            ShopCard = await shopcardrepo.TableNoTracking.Where(x => x.CookieId == CookieId).FirstOrDefaultAsync(cancellationToken);
            if (ShopCard == null)
            {
                ShopCard = new ShopCard()
                {
                    CookieId = CookieId,
                    TotalPrice = 0,
                };
                await shopcardrepo.AddAsync(ShopCard, cancellationToken);
            }
            if (HttpContext.User.Identity.IsAuthenticated && (ShopCard.UserId == null || ShopCard.UserId == Guid.Empty))
            {
                var FindUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                ShopCard.UserId = FindUser.Id;
                await shopcardrepo.UpdateAsync(ShopCard, cancellationToken);
            }

            var ShopCardDetail = await shopcardDetailrepo.TableNoTracking.Where(x => x.ProductsId == FindProduct.Id && x.ShopCardId == ShopCard.Id).FirstOrDefaultAsync(cancellationToken);
            if (ShopCardDetail == null)
            {
                ShopCardDetail = new ShopCardDetail()
                {
                    ShopCardId = ShopCard.Id,
                    ProductsId = ProductId,
                    Price = 0,
                    Count = 0,
                    DisCount = 0,
                    CreationDateTime = DateTime.Now,
                };
                await shopcardDetailrepo.AddAsync(ShopCardDetail, cancellationToken);
            }
            //اگر شاپ کارت نال نباشه به این قسمت ها میرسه 
            if (AddOrRemove == true)
            {
                if (Count > 0)
                {
                    int Difference = Count - ShopCardDetail.Count;
                    ShopCardDetail.Count = Count;
                    ShopCardDetail.Price = FindProduct.Discount * Count;
                    if (Difference > 0)
                    {
                        ShopCard.TotalPrice += FindProduct.Discount * Difference;
                        ShopCard.FinalTotalPrice += FindProduct.Discount * Difference;
                    }
                    else
                    {
                        Difference = -Difference;
                        ShopCard.TotalPrice -= FindProduct.Discount * Difference;
                        ShopCard.FinalTotalPrice -= FindProduct.Discount * Difference;
                    }
                }
                else
                {
                    ShopCardDetail.Count++;
                    ShopCard.TotalPrice += FindProduct.Discount;
                    ShopCard.FinalTotalPrice += FindProduct.Discount;
                    ShopCardDetail.Price += FindProduct.Discount;
                }
                if (FindProduct.Count < ShopCardDetail.Count)
                {
                    return "Bad";
                }
                await shopcardDetailrepo.UpdateAsync(ShopCardDetail, cancellationToken);
                await shopcardrepo.UpdateAsync(ShopCard, cancellationToken);
            }
            else if (AddOrRemove == false)
            {
                if (Count > 0)
                {
                    ShopCardDetail.Count = Count;
                    ShopCard.TotalPrice -= FindProduct.Discount * Count;
                    ShopCard.FinalTotalPrice -= FindProduct.Discount * Count;
                    ShopCardDetail.Price = FindProduct.Discount * Count;
                }
                else
                {
                    ShopCardDetail.Count--;
                }
                if (ShopCardDetail.Count == 0)
                {
                    await shopcardDetailrepo.DeleteAsync(ShopCardDetail, cancellationToken);
                }
                else
                {
                    await shopcardDetailrepo.UpdateAsync(ShopCardDetail, cancellationToken);
                }
                ShopCard.TotalPrice -= FindProduct.Discount;
                ShopCard.FinalTotalPrice -= FindProduct.Discount;
                await shopcardrepo.UpdateAsync(ShopCard, cancellationToken);
            }
            TempData["Added"] = "Added";
            return $"/Product/ProductDetail_User?id={ProductId}&ClientId={ClientId}";
        }
        [AllowAnonymous]
        public async Task<ShopCardDto> ShopCardModal(string ClientId, CancellationToken cancellationToken)
        {
            if (!ClientId.HasValue())
            {
                ClientId = Guid.Empty.ToString();
            }
            Guid CookieId = Guid.Parse(ClientId);

            var ShopCard = new ShopCardDto();

            if (User.Identity.IsAuthenticated)
            {
                var FindUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                ShopCard = await shopcardrepo.TableNoTracking.Where(x => x.UserId == FindUser.Id).ProjectTo<ShopCardDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            }
            else
            {
                if (CookieId == Guid.Empty)
                {
                    return ShopCard;
                }
                ShopCard = await shopcardrepo.TableNoTracking.Where(x => x.CookieId == CookieId).ProjectTo<ShopCardDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            }
            if (ShopCard != null)
            {
                #region For Modal
                var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
                ShopCard.ShopCardDetails = await shopcardDetailrepo.TableNoTracking.Where(x => x.ShopCardId == ShopCard.Id).ProjectTo<ShopCardDetailDto>(mapper.ConfigurationProvider).ToListAsync();
                ShopCard.PostPrice = setting.PostPrice;
                ShopCard.FinalTotalPrice = ShopCard.TotalPrice + setting.PostPrice;
                if (ShopCard.TotalPrice >= 500000)
                {
                    ShopCard.FinalTotalPrice = ShopCard.TotalPrice;
                }
                else
                {
                    ShopCard.FinalTotalPrice = ShopCard.TotalPrice + ShopCard.PostPrice;
                }
                foreach (var item in ShopCard.ShopCardDetails)
                {
                    item.ProductsImageLink = await ProductImageRepo.TableNoTracking.Where(x => x.ProductId == item.ProductsId).Select(t => t.ImageLink).FirstOrDefaultAsync(cancellationToken);
                }
                #endregion
                return ShopCard;
            }
            else
            {
                return ShopCard;
            }
        }
        public async Task<int> ShopCardCount(string ClientId, CancellationToken cancellationToken)
        {
            if (!ClientId.HasValue())
            {
                return 0;
            }
            Guid CookieId = Guid.Parse(ClientId);
            if (CookieId == Guid.Empty)
            {
                return 0;
            }
            int ShopCardCount = 0;
            var ShopCard = new ShopCard();
            ShopCard = await shopcardrepo.TableNoTracking.FirstOrDefaultAsync(x => x.CookieId == CookieId);
            if (ShopCard == null)
            {
                return 0;
            }
            if (HttpContext.User.Identity.IsAuthenticated && (ShopCard.UserId == null || ShopCard.UserId == Guid.Empty))
            {
                var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                ShopCard.UserId = User.Id;
                shopcardrepo.Update(ShopCard);
            }
            if (ShopCard != null)
            {
                var ShopCardDetail = await shopcardDetailrepo.TableNoTracking.Where(x => x.ShopCardId == ShopCard.Id).ToListAsync();
                ShopCardCount = ShopCardDetail.Count();
            }
            return ShopCardCount;
        }
        public async Task<IActionResult> DeleteShopCardDetail(Guid ProductId, string ClientId, CancellationToken cancellationToken)
        {
            var ShopCard = new ShopCard();
            if (ClientId != null)
            {
                Guid CookieId = Guid.Parse(ClientId);
                ShopCard = await shopcardrepo.TableNoTracking.Where(x => x.CookieId == CookieId).FirstOrDefaultAsync(cancellationToken);
            }
            var model = await shopcardDetailrepo.TableNoTracking.FirstOrDefaultAsync(x => x.ShopCardId == ShopCard.Id && x.ProductsId == ProductId);

            ShopCard.TotalPrice -= model.Price;
            ShopCard.FinalTotalPrice -= model.Price;
            shopcardrepo.Update(ShopCard);
            shopcardDetailrepo.Delete(model);

            TempData["Added"] = "full";
            return RedirectToAction("ProductDetail_User", "Product", new { id = ProductId, ClientId = ClientId });
        }
        #region Address
        [HttpPost]
        public async Task<IActionResult> AddAddress(AddressDto dto, CancellationToken cancellationToken)
        {
            var User = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (User.Fname == null)
            {
                User.Fname = dto.Name;
                User.PostalCode = dto.PostalCode;
            }
            await userManager.UpdateAsync(User);
            var model = dto.ToEntity(mapper);
            model.ClientId = User.Id;
            await AddressRepo.AddAsync(model, cancellationToken);
            notification.AddSuccessToastMessage("آدرس با موفقیت ثبت شد");
            if (dto.ReturnUrl == "AddressProfile")
            {
                return RedirectToAction("AddressProfile", "Home");
            }
            return RedirectToAction("ShopCard_Detail2", "Shopcard");
        }
        public async Task<IActionResult> DeleteAddress(Guid Id, string? ReturnUrl, CancellationToken cancellationToken)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("~/home/addressProfile");
            var model = await AddressRepo.TableNoTracking.FirstOrDefaultAsync(x => x.Id == Id);
            model.Active = false;
            await AddressRepo.UpdateAsync(model, cancellationToken);
            return Redirect(ReturnUrl);
        }
        public List<CityDto> Send_Cities(Guid ProvinceId)
        {
            var Cities = CityRepo.TableNoTracking.Where(x => x.ProvinceId == ProvinceId).ProjectTo<CityDto>(mapper.ConfigurationProvider).ToList();
            return Cities;
        }
        #endregion
    }
}
