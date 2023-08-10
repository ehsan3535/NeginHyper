using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Secretary.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{
    [Authorize(Roles = "Admin")]

    public class CategoryController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<BlogCategory> blogcategoryrepo;
        private readonly IRepository<ProductCategory> Productcategoryrepo;
        private readonly IRepository<Blog> Blogrepo;
        private readonly IMapper mapper;

        public CategoryController(UserManager<User> userManager, IRepository<BlogCategory> blogcategoryrepo, IRepository<ProductCategory> productcategoryrepo, IRepository<Blog> blogrepo, IMapper mapper)
        {
            this.userManager = userManager;
            this.blogcategoryrepo = blogcategoryrepo;
            Productcategoryrepo = productcategoryrepo;
            Blogrepo = blogrepo;
            this.mapper = mapper;
        }

        #region BlogCategory
        public async Task<IActionResult> Add_Edit_Blog_Category(Guid? CategoryId)
        {
            if (CategoryId != Guid.Empty)
            {
                var model = blogcategoryrepo.TableNoTracking.Where(x => x.Id == CategoryId).ProjectTo<BlogCategoryDto>(mapper.ConfigurationProvider).FirstOrDefault();
                return View(model);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_Blog_Category(BlogCategoryDto dto, CancellationToken cancellationToken)
        {
            if (dto.Id != Guid.Empty)
            {
                var model = await blogcategoryrepo.TableNoTracking.Where(x => x.Id == dto.Id).FirstOrDefaultAsync(cancellationToken);
                var Category = dto.ToEntity(mapper, model);
                await blogcategoryrepo.UpdateAsync(Category, cancellationToken);
            }
            else
            {
                var model = dto.ToEntity(mapper);
                await blogcategoryrepo.AddAsync(model, cancellationToken);
            }
            return RedirectToAction("BlogCategoryList");
        }

        public async Task<IActionResult> BlogCategoryList(CancellationToken cancellationToken)
        {
            var model = blogcategoryrepo.TableNoTracking.ProjectTo<BlogCategoryDto>(mapper.ConfigurationProvider).ToList();
            return View(model);
        }
        public async Task<IActionResult> DeleteBlogCategory(Guid CategoryId, CancellationToken cancellationToken)
        {
            var model = blogcategoryrepo.GetById(CategoryId);
            if (model != null)
            {
                model.Active = false;
                await blogcategoryrepo.UpdateAsync(model, cancellationToken);
                return RedirectToAction("BlogCategoryList");
            }
            return View();
        }
        #endregion


        #region ProductCategory
        public async Task<IActionResult> Add_Edit_Product_Category(Guid? CategoryId)
        {
            if (CategoryId != Guid.Empty)
            {
                var model = Productcategoryrepo.TableNoTracking.Where(x => x.Id == CategoryId).ProjectTo<ProductCategoryDto>(mapper.ConfigurationProvider).FirstOrDefault();
                return View(model);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_Product_Category(ProductCategoryDto dto, CancellationToken cancellationToken)
        {
            if (dto.Id != Guid.Empty)
            {
                var model = await Productcategoryrepo.TableNoTracking.Where(x => x.Id == dto.Id).FirstOrDefaultAsync(cancellationToken);
                var Category = dto.ToEntity(mapper, model);
                if (dto.File != null)
                {
                    Category.ImageUrl = UploadImage.SaveImage(dto.File, "Product_Category_Image");
                }
                if (dto.MenuImageFile != null)
                {
                    Category.MenuImageUrl = UploadImage.SaveImage(dto.MenuImageFile, "Product_Category_Image");
                }
                await Productcategoryrepo.UpdateAsync(Category, cancellationToken);
            }
            else
            {
                var model = dto.ToEntity(mapper);
                if (dto.File != null)
                {
                    model.ImageUrl = UploadImage.SaveImage(dto.File, "Product_Category_Image");
                }
                if (dto.MenuImageFile != null)
                {
                    model.MenuImageUrl = UploadImage.SaveImage(dto.MenuImageFile, "Product_Category_Image");
                }
                await Productcategoryrepo.AddAsync(model, cancellationToken);
            }
            return RedirectToAction("ProductCategoryList");
        }
        public async Task<IActionResult> ProductCategoryList(CancellationToken cancellationToken)
        {
            var model = Productcategoryrepo.TableNoTracking.ProjectTo<ProductCategoryDto>(mapper.ConfigurationProvider).ToList();
            return View(model);
        }
        public async Task<IActionResult> DeleteProductCategory(Guid CategoryId, CancellationToken cancellationToken)
        {
            var model = Productcategoryrepo.GetById(CategoryId);
            if (model != null)
            {
                model.Active = false;
                await Productcategoryrepo.UpdateAsync(model, cancellationToken);
                return RedirectToAction("ProductCategoryList");
            }
            return View();
        }
        #endregion
    }
}
