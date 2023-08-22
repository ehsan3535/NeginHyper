using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models;
using Client.Models.CommentsDto;
using Client.Models.ProductDto;
using Client.Models.ShopCards;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Entities.Product;
using Entities.ProductComment;
using Entities.ShopCards;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Client.Controllers
{
    public class ProductController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly IRepository<ShopCard> shopcardrepo;
        private readonly IRepository<ShopCardDetail> shopcardDetailrepo;
        private readonly IRepository<Setting> settingrepo;
        private readonly IRepository<ProductImage> ProductImageRepo;
        private readonly IRepository<Products> productripo;
        private readonly IRepository<ProductImage> productImageripo;
        private readonly IRepository<ProductCategory> categoryripo;
        private readonly IToastNotification notification;
        private readonly IRepository<Entities.ProductComment.ProductComments> productCommentrepo;

        public ProductController(UserManager<User> userManager, IMapper mapper, IRepository<ShopCard> shopcardrepo, IRepository<ShopCardDetail> shopcardDetailrepo, IRepository<Products> productripo, IRepository<ProductCategory> categoryripo, IRepository<ProductComments> productCommentrepo, IToastNotification notification, IRepository<ProductImage> productImageripo, IRepository<ProductImage> productImageRepo, IRepository<Setting> settingrepo)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.shopcardrepo = shopcardrepo;
            this.shopcardDetailrepo = shopcardDetailrepo;
            this.productripo = productripo;
            this.categoryripo = categoryripo;
            this.productCommentrepo = productCommentrepo;
            this.notification = notification;
            this.productImageripo = productImageripo;
            ProductImageRepo = productImageRepo;
            this.settingrepo = settingrepo;
        }

        public async Task<IActionResult> ProductDetail_User(Guid id, string ClientId, CancellationToken cancellationToken)
        {
            var Product = await productripo.TableNoTracking.Where(x => x.Id == id).ProjectTo<ProductDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            Product.Comments = await productCommentrepo.TableNoTracking.Where(x => x.ProductId == id).ProjectTo<ProductCommentDto>(mapper.ConfigurationProvider).ToListAsync();
            Product.ProductImages = await productImageripo.AllTableNoTracking.Where(x => x.ProductId == id).ProjectTo<ProductImagesDto>(mapper.ConfigurationProvider).ToListAsync();
            if (!ClientId.HasValue())
            {
                ClientId = Guid.Empty.ToString();
            }
            Guid CookieId = Guid.Parse(ClientId);
            var ShopCard = new ShopCardDto();
            if (User.Identity.IsAuthenticated)
            {
                var FindUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                ShopCard = await shopcardrepo.TableNoTracking.Where(x => x.CookieId == CookieId).ProjectTo<ShopCardDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            }
            else
            {
                if (CookieId != Guid.Empty)
                {
                    ShopCard = await shopcardrepo.TableNoTracking.Where(x => x.CookieId == CookieId).ProjectTo<ShopCardDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
                }
            }
            if (ShopCard != null)
            {
                #region For Modal
                var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
                Product.ShopCard = ShopCard;
                Product.ShopCard.ShopCardDetails = await shopcardDetailrepo.TableNoTracking.Where(x => x.ShopCardId == ShopCard.Id).ProjectTo<ShopCardDetailDto>(mapper.ConfigurationProvider).ToListAsync();
                Product.ShopCard.PostPrice = setting.PostPrice;
                Product.ShopCard.FinalTotalPrice = Product.ShopCard.TotalPrice + setting.PostPrice;
                if (Product.ShopCard.TotalPrice >= 500000)
                {
                    Product.ShopCard.FinalTotalPrice = Product.ShopCard.TotalPrice;
                }
                else
                {
                    Product.ShopCard.FinalTotalPrice = Product.ShopCard.TotalPrice + Product.ShopCard.PostPrice;
                }
                foreach (var item in Product.ShopCard.ShopCardDetails)
                {
                    item.ProductsImageLink = await ProductImageRepo.TableNoTracking.Where(x => x.ProductId == item.ProductsId).Select(t => t.ImageLink).FirstOrDefaultAsync(cancellationToken);
                }
                #endregion

                var ShopCardDetail = await shopcardDetailrepo.TableNoTracking.Where(x => x.ProductsId == id && x.ShopCardId == ShopCard.Id).FirstOrDefaultAsync(cancellationToken);
                if (ShopCardDetail != null)
                {
                    Product.ShopCartDeailCount = ShopCardDetail.Count;
                }
                else
                {
                    Product.ShopCartDeailCount = 0;
                }
            }
            else
            {
                Product.ShopCartDeailCount = 0;
            }

            return View(Product);
        }
        public async Task<IActionResult> Shop(Guid? CategoryId, string? Searched, int? Type, int page, CancellationToken cancellationToken)
        {
            #region Pageination
            if (page == 0)
                page = 1;
            TempData["Page"] = page;

            int skip = (page - 1) * 9;
            int Take = 0;

            Take = productripo.TableNoTracking.Count();
            int Count = Take;
            if (Take > 9)
            {
                Take = 9;
            }
            var baghimande = Count % 9;
            if (Take < 9)
            {
                TempData["PageCount"] = 1;
            }
            else if (baghimande == 0)
            {
                TempData["PageCount"] = Count / 9;
            }
            else if (baghimande != 0)
            {
                TempData["PageCount"] = (Count / 9) + 1;
            }
            #endregion
            var model = new List<ProductDto>();
            //Search
            if (Searched != null)
            {
                model = productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).Where(x => x.Name.Trim().ToLower().Contains(Searched.Trim().ToLower().FixPersianChars())).OrderByDescending(x => x.CreationDateTime).Skip(skip).Take(Take).ToList();
                //foreach (var item in model2)
                //{
                //    if (item.Name.Contains(Searched))
                //    {
                //        model.Add(item);
                //    }
                //    model.OrderByDescending(x => x.CreationDateTime);
                //}
                if (!model.Any())
                {
                    model = productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).Skip(skip).Take(Take).ToList();
                    notification.AddErrorToastMessage("محصول جست و جو شده یافت نشد", new NotyOptions
                    {
                        Timeout = 1000,
                        ProgressBar = true,
                        Modal = true,
                        Layout = "topCenter",
                        Theme = "metroui",
                    });
                }
            }
            else if (Type != null)
            {
                //مرتب سازی بر اساس آخرین
                if (Type == 1)
                {
                    model = await productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).Skip(skip).Take(Take).ToListAsync();
                }
                if (Type == 2)
                {
                    model = await productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.Rate).Skip(skip).Take(Take).ToListAsync();
                }
                if (Type == 3)
                {
                    model = await productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.Discount).Skip(skip).Take(Take).ToListAsync();
                }
                if (Type == 4)
                {
                    model = await productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderBy(x => x.Discount).Skip(skip).Take(Take).ToListAsync();
                }
                if (Type == 5)
                {
                    model = await productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.Percent).Skip(skip).Take(Take).ToListAsync();
                }
            }
            //ProductList With CategoryId
            else
            {
                if (CategoryId != null)
                {
                    model = productripo.TableNoTracking.Where(x => x.CategorysId == CategoryId).ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).Skip(skip).Take(Take).ToList();
                    if (!model.Any())
                    {
                        model = productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).Skip(skip).Take(Take).ToList();
                        notification.AddErrorToastMessage("محصولی از این دسته بندی یافت نشد");
                    }
                    else
                    {
                        TempData["CategoryId"] = CategoryId;
                    }
                }
                else
                {
                    model = productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).Skip(skip).Take(Take).ToList();
                }
            }
            //CategoryList
            if (!model.Any())
            {
                model = new();
            }
            model.First().Categories = categoryripo.TableNoTracking.ProjectTo<ProductCategoryDto>(mapper.ConfigurationProvider).ToList();

            /* //Topest Products
             if (productripo.TableNoTracking.Where(x => x.Rate >= 4).Count() > 6)
             {
                 model.First().Products = productripo.TableNoTracking.Where(x => x.Rate >= 4).ProjectTo<ProductDto>(mapper.ConfigurationProvider).Take(6).OrderByDescending(x => x.Rate).ToList();
             }
             else
             {
                 model.First().Products = productripo.TableNoTracking.Where(x => x.Rate >= 4).ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.Rate).ToList();
             }
             foreach (var item in model)
             {
                 item.ProductImages = await productImageripo.AllTableNoTracking.Where(x => x.ProductId == item.Id).ProjectTo<ProductImagesDto>(mapper.ConfigurationProvider).ToListAsync();
             }*/
            #region For Modal
            if (User.Identity.IsAuthenticated && TempData["Added"] != null)
            {
                //عکس محصول رو از خود مودل میتونم پیدا کنم برای مودال و نیازی به پر کردنش نیست 
                var setting = await settingrepo.TableNoTracking.FirstOrDefaultAsync();
                var FindUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                model.First().ShopCard = await shopcardrepo.TableNoTracking.Where(x => x.UserId == FindUser.Id).ProjectTo<ShopCardDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
                model.First().ShopCard.ShopCardDetails = await shopcardDetailrepo.TableNoTracking.ProjectTo<ShopCardDetailDto>(mapper.ConfigurationProvider).ToListAsync();
                model.First().ShopCard.PostPrice = setting.PostPrice;
                model.First().ShopCard.FinalTotalPrice = model.First().ShopCard.TotalPrice + setting.PostPrice;
                if (model.First().ShopCard.TotalPrice >= 500000)
                {
                    model.First().ShopCard.FinalTotalPrice = model.First().ShopCard.TotalPrice;
                }
                else
                {
                    model.First().ShopCard.FinalTotalPrice = model.First().ShopCard.TotalPrice + setting.PostPrice;
                }
            }
            #endregion

            return View(model);
        }
        public async Task<IActionResult> ReplaceForLocal(CancellationToken cancellationToken)
        {
            var productList = productripo.TableNoTracking.ToList();
            foreach (var item in productList)
            {
                 item.ImageCoverUrl = item.ImageCoverUrl.Replace("http://admin.NeginHyper.Com", "https://localhost:44382");
                productripo.Update(item);
            }

            return RedirectToAction("shop");
        }
        public async Task<IActionResult> ReplaceForServer(CancellationToken cancellationToken)
        {
            var productList = productripo.TableNoTracking.ToList();
            foreach (var item in productList)
            {
                item.ImageCoverUrl = item.ImageCoverUrl.Replace("https://localhost:44382", "http://admin.NeginHyper.Com");
                productripo.Update(item);
            }

            return RedirectToAction("shop");
        }

    }
}
