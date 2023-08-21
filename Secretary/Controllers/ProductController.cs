using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Entities.Product;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Secretary.Models;
using Secretary.Models.ProductDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ProductController : Controller
    {
        private readonly IMapper mapper;
        private readonly IRepository<Products> productripo;
        private readonly IRepository<ProductImage> productImageripo;
        private readonly IRepository<ProductCategory> categoryripo;
        private readonly IToastNotification notification;

        public ProductController(IMapper mapper, IRepository<Products> productripo, IRepository<ProductCategory> categoryripo, IToastNotification notification, IRepository<ProductImage> productImageripo)
        {
            this.mapper = mapper;
            this.productripo = productripo;
            this.categoryripo = categoryripo;
            this.notification = notification;
            this.productImageripo = productImageripo;
        }
        #region Add Product
        public async Task<IActionResult> Add_Edit_Product(Guid? id, int Page)
        {
            TempData["Page"] = Page;
            if (id != null)
            {
                var model = await productripo.TableNoTracking.Where(x => x.Id == id).ProjectTo<ProductDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
                model.ProductImages = await productImageripo.AllTableNoTracking.Where(x => x.ProductId == id).ProjectTo<ProductImagesDto>(mapper.ConfigurationProvider).ToListAsync();
                model.Categories = categoryripo.TableNoTracking.ProjectTo<ProductCategoryDto>(mapper.ConfigurationProvider).ToList();
                if (model.Discount == model.Price)
                {
                    model.DiscountCheckBox = false;
                }
                else
                {
                    model.DiscountCheckBox = true;
                }
                return View(model);
            }
            else
            {
                var model = new ProductDto();
                model.Categories = categoryripo.TableNoTracking.ProjectTo<ProductCategoryDto>(mapper.ConfigurationProvider).ToList();
                return View(model);
            }
           
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_Product(ProductDto dto, CancellationToken cancellationToken)
        {

            if (dto.DiscountCheckBox == false)
            {
                dto.Discount = dto.Price;
            }
            else
            {
                double x = ((double)dto.Discount / (double)dto.Price);
                x = x * 100;
                dto.Percent = 100 - (int)x;
            }
            if (dto.Id != Guid.Empty)
            {
                var model = productripo.GetById(dto.Id);
                if (model != null)
                {
                    if (dto.File2 == null && model.ImageCoverUrl != null)
                    {
                        dto.ImageCoverUrl = model.ImageCoverUrl;
                    }
                    model = dto.ToEntity(mapper, model);
                    if (dto.File != null)
                    {
                        if (dto.File.Any())
                        {
                            var ListProductImages = new List<ProductImage>();
                            foreach (var item in dto.File)
                            {
                                var ProductImage = new ProductImage()
                                {
                                    ImageLink = UploadImage.SaveImage(item, "ProductImage"),
                                    ProductId = model.Id,
                                };
                                ListProductImages.Add(ProductImage);
                            }
                            await productImageripo.AddRangeAsync(ListProductImages, cancellationToken);
                        }
                    }
                    if (dto.File2 != null)
                    {
                        model.ImageCoverUrl = UploadImage.SaveImage(dto.File2, "ProductTestImage");
                    }
                   
                    await productripo.UpdateAsync(model, cancellationToken);
                    int Page = (int)TempData["Page"];
                    return RedirectToAction(nameof(ListProduct), new { page = Page });
                }
                return View();
            }
            else
            {

                var model = dto.ToEntity(mapper);
                /* if (model.CategorysId == null)
                 {
                    return RedirectToAction("AddCategory", "Category");
                 }*/
                model.CategorysId = dto.CategorysId;
                if (dto.File2 != null)
                {
                    model.ImageCoverUrl = UploadImage.SaveImage(dto.File2, "ProductTestImage");
                }

                await productripo.AddAsync(model, cancellationToken);
                if (dto.File != null)
                {
                    if (dto.File.Any())
                    {
                        var ListProductImages = new List<ProductImage>();
                        foreach (var item in dto.File)
                        {

                            var ProductImage = new ProductImage()
                            {
                                ImageLink = UploadImage.SaveImage(item, "ProductImage"),
                                ProductId = model.Id,
                            };
                            ListProductImages.Add(ProductImage);
                        }
                        await productImageripo.AddRangeAsync(ListProductImages, cancellationToken);
                    }
                }
                await productripo.UpdateAsync(model, cancellationToken);
            }
            int Pagee = (int)TempData["Page"];
            return RedirectToAction(nameof(ListProduct), new { page = Pagee });
        }
        #endregion

        /// <summary>
        /// this is just for Admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IActionResult> ProductDetail(Guid id)
        {
            var model = await productripo.AllTableNoTracking.Where(x => x.Id == id).ProjectTo<ProductDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();
            model.ProductImages = await productImageripo.AllTableNoTracking.Where(x => x.ProductId == id).ProjectTo<ProductImagesDto>(mapper.ConfigurationProvider).ToListAsync();
            if (model != null)
            {
                return View(model);
            }
            return View();
        }

        public async Task<IActionResult> DeleteProductImages(Guid? ProductImageId, string? ImageLink2)
        {
            if (ProductImageId != null)
            {
                var Image = await productImageripo.AllTableNoTracking.Where(x => x.Id == ProductImageId).FirstOrDefaultAsync();
                productImageripo.Delete(Image);
                return RedirectToAction(nameof(Add_Edit_Product), new { id = Image.ProductId });
            }
            else if (ImageLink2 != null)
            {
                var product = await productripo.AllTableNoTracking.Where(x => x.ImageCoverUrl == ImageLink2).FirstOrDefaultAsync();
                product.ImageCoverUrl = null;
                productripo.Update(product);
                return RedirectToAction(nameof(Add_Edit_Product), new { id = product.Id });
            }
            return RedirectToAction("listProduct");
        }

        public async Task<IActionResult> ListProduct(int page, string? Searched, CancellationToken cancellationToken)
        {
            #region Pageination
            if (page == 0)
                page = 1;
            TempData["Page"] = page;

            int skip = (page - 1) * 9;

            int Take = productripo.TableNoTracking.Count();
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

            var model2 = productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).ToList();
            var model = new List<ProductDto>();
            //if (CategoryId != null)
            //{
            //    model = productripo.TableNoTracking.Where(x => x.CategorysId == CategoryId).ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).ToList();
            //    if (!model.Any())
            //    {
            //        model = productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).ToList();
            //        notification.AddErrorToastMessage("محصولی از این دسته بندی یافت نشد");
            //    }
            //    else
            //    {
            //        TempData["CategoryId"] = CategoryId;
            //    }
            //}
            //else
            //{
            //    model = productripo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).ToList();
            //}
            if (Searched != null)
            {
                foreach (var item in model2)
                {
                    if (item.Name.Contains(Searched))
                    {
                        model.Add(item);
                    }
                    model.OrderByDescending(x => x.CreationDateTime);
                }
                if (model.Any())
                {
                    Take = model.Count();
                    Count = Take;
                    if (Take > 9)
                    {
                        Take = 9;
                    }
                    baghimande = Count % 9;
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
                }
                //اگر سرچ پیدا نکرد کل محصولات رو بفرست با نوتیفیکیشن
                else if (!model.Any())
                {
                    model = model2;
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
            if (!model.Any())
            {
                model = model2;
            }
            model = model.Skip(skip).Take(Take).ToList();
            return View(model);
        }
        /// <summary>
        /// to inactive the product 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
        {
            var model = productripo.GetById(id);
            if (model != null)
            {
                model.Active = false;
                await productripo.UpdateAsync(model, cancellationToken);
                return RedirectToAction("listProduct");
            }
            return RedirectToAction("listProduct");
        }
    }
}
