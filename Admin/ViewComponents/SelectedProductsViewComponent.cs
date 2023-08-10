using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models;
using Client.Models.ProductDto;
using Data.Repositories;
using Entities;
using Entities.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Client.ViewComponents
{
    public class SelectedProductsViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly IRepository<Products> productRepo;
        private readonly IRepository<ProductImage> productImageripo;
        private readonly IMapper mapper;

        public SelectedProductsViewComponent(IRepository<Products> productRepo, IMapper mapper, IRepository<ProductImage> productImageripo)
        {
            this.productRepo = productRepo;
            this.mapper = mapper;
            this.productImageripo = productImageripo;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid CategoryId)
        {
            var Products = await productRepo.TableNoTracking.Where(d => d.CategorysId == CategoryId).ProjectTo<ProductDto>(mapper.ConfigurationProvider).ToListAsync();
            foreach (var item in Products)
            {
                item.ProductImages = await productImageripo.AllTableNoTracking.Where(x => x.ProductId == item.Id).ProjectTo<ProductImagesDto>(mapper.ConfigurationProvider).ToListAsync();
            }
            return View("SelectedProducts", Products);
        }
    }
}
