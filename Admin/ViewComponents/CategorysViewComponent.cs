using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Client.ViewComponents
{
    public class CategorysViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<ProductCategory> productCategoryRepo;
        private readonly IMapper mapper;
        public CategorysViewComponent(UserManager<User> userManager, IMapper mapper, IRepository<ProductCategory> productCategoryRepo)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.productCategoryRepo = productCategoryRepo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var Categorys = await productCategoryRepo.TableNoTracking.ProjectTo<ProductCategoryDto>(mapper.ConfigurationProvider).Where(d => d.Code == "1001").ToListAsync();
            return View("Categorys", Categorys);
        }
    }
}
