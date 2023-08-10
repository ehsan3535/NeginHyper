using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Client.ViewComponents
{
    public class BlogsViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly IMapper mapper;
        private readonly IRepository<Blog> BlogRepo;
        public BlogsViewComponent(IMapper mapper, IRepository<Blog> blogRepo)
        {
            this.mapper = mapper;
            BlogRepo = blogRepo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var Blogs = new List<BlogDto>();
            if (BlogRepo.TableNoTracking.Count() >= 10)
            {
                Blogs = await BlogRepo.TableNoTracking.ProjectTo<BlogDto>(mapper.ConfigurationProvider).Take(10).ToListAsync();
            }
            else
            {
                Blogs = await BlogRepo.TableNoTracking.ProjectTo<BlogDto>(mapper.ConfigurationProvider).ToListAsync();
            }
            return View("Blogs", Blogs);
        }
    }
}
