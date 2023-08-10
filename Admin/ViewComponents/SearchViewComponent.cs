using AutoMapper;
using Data.Repositories;
using Entities;
using Entities.Product;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Client.ViewComponents
{
    public class SearchViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<Products> productRepo;
        private readonly IMapper mapper;

        public SearchViewComponent(UserManager<User> userManager, IRepository<Products> productRepo, IMapper mapper)
        {
            this.userManager = userManager;
            this.productRepo = productRepo;
            this.mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("Search");
        }
    }
}
