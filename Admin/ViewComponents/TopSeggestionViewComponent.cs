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
    public class TopSeggestionViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly IMapper mapper;
        private readonly IRepository<Products> productRepo;
        private readonly IRepository<ProductImage> productImageripo;

        public TopSeggestionViewComponent(IRepository<Products> productRepo, IRepository<ProductImage> productImageripo, IMapper mapper)
        {
            this.productRepo = productRepo;
            this.productImageripo = productImageripo;
            this.mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var TopProducts = new List<ProductDto>();
            if (productRepo.TableNoTracking.Where(x => x.Rate == 5).Count() >= 10)
            {
                TopProducts = await productRepo.TableNoTracking.Where(x => x.Rate == 5).ProjectTo<ProductDto>(mapper.ConfigurationProvider).Take(10).ToListAsync();
            }
            else
            {
                TopProducts = await productRepo.TableNoTracking.Where(x => x.Rate == 5).ProjectTo<ProductDto>(mapper.ConfigurationProvider).ToListAsync();
            }
            return View("TopSeggestion", TopProducts);
        }
    }
}
