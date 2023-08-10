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
    public class MobileMenueViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<ProductCategory> productCategoryRepo;
        private readonly IMapper mapper;
        public MobileMenueViewComponent(UserManager<User> userManager, IMapper mapper, IRepository<ProductCategory> productCategoryRepo)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.productCategoryRepo = productCategoryRepo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var Categorys = await productCategoryRepo.TableNoTracking.ProjectTo<ProductCategoryDto>(mapper.ConfigurationProvider).ToListAsync();
            return View("MobileMenue", Categorys);
        }
    }
}
