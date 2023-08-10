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
    public class ChipestProductViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly IMapper mapper;
        private readonly IRepository<Products> productRepo;
        private readonly IRepository<ProductImage> productImageripo;

        public ChipestProductViewComponent(IRepository<Products> productRepo, IRepository<ProductImage> productImageripo, IMapper mapper)
        {
            this.productRepo = productRepo;
            this.productImageripo = productImageripo;
            this.mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ChipestProducts = new List<ProductDto>();

            if (productRepo.TableNoTracking.Count() >= 10)
            {
                ChipestProducts = await productRepo.TableNoTracking.Where(x=> x.Count != 0).ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderBy(x => x.Discount).Take(10).ToListAsync();
            }
            else
            {
                ChipestProducts = await productRepo.TableNoTracking.Where(x => x.Count != 0).ProjectTo<ProductDto>(mapper.ConfigurationProvider).OrderBy(x => x.Discount).ToListAsync();
            }
            return View("ChipestProduct", ChipestProducts);
        }
    }
}
