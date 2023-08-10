using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models.ProductDto;
using Data.Repositories;
using Entities;
using Entities.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Client.ViewComponents
{
    public class LatestProductViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly IMapper mapper;
        private readonly IRepository<Products> productRepo;
        private readonly IRepository<ProductImage> productImageripo;

        public LatestProductViewComponent(IRepository<Products> productRepo, IRepository<ProductImage> productImageripo, IMapper mapper)
        {
            this.productRepo = productRepo;
            this.productImageripo = productImageripo;
            this.mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var LatestProducts = new List<ProductDto>();
            if (productRepo.TableNoTracking.Count() >= 10)
            {
                LatestProducts = await productRepo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).Take(10).ToListAsync();
            }
            else
            {
                LatestProducts = await productRepo.TableNoTracking.ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).ToListAsync();
            }
            return View("LatestProduct", LatestProducts);
        }
    }
}
