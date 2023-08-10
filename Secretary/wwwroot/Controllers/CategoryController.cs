using Secretary.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{
    public class CategoryController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<Category> categoryrepo;
        private readonly IRepository<Blog> Blogrepo;
        private readonly IMapper mapper;
        public CategoryController(UserManager<User> userManager, IMapper mapper, IRepository<Category> categoryrepo)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.categoryrepo = categoryrepo;
        }
        public async Task<IActionResult> Add_Edit_Category(Guid? CategoryId)
        {
            if (CategoryId != Guid.Empty)
            {
                var model = categoryrepo.TableNoTracking.Where(x => x.Id == CategoryId).ProjectTo<CategoryDto>(mapper.ConfigurationProvider).FirstOrDefault();
                return View(model);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add_Edit_Category(CategoryDto dto, CancellationToken cancellationToken)
        {
            if (dto.Id != Guid.Empty)
            {
                var model = await categoryrepo.TableNoTracking.Where(x => x.Id == dto.Id).FirstOrDefaultAsync(cancellationToken);
                var Category = dto.ToEntity(mapper, model);
                await categoryrepo.UpdateAsync(Category, cancellationToken);
            }
            else
            {
                var model = dto.ToEntity(mapper);
                await categoryrepo.AddAsync(model, cancellationToken);
            }
            return RedirectToAction("CategoryList");
        }
        public async Task<IActionResult> CategoryDetail(Guid id, CancellationToken cancellationToken)
        {
            var model = categoryrepo.TableNoTracking.Where(x => x.Id == id).ProjectTo<CategoryDto>(mapper.ConfigurationProvider).FirstOrDefault();
            if (model != null)
            {
                return View(model);
            }
            return View();
        }
        public async Task<IActionResult> CategoryList(CancellationToken cancellationToken)
        {
            var model = categoryrepo.TableNoTracking.ProjectTo<CategoryDto>(mapper.ConfigurationProvider).ToList();
            return View(model);
        }
        public async Task<IActionResult> DeleteCategory(Guid CategoryId, CancellationToken cancellationToken)
        {
            var model = categoryrepo.GetById(CategoryId);
            if (model != null)
            {
                model.Active = false;
                await categoryrepo.UpdateAsync(model, cancellationToken);
                return RedirectToAction("CategoryList");
            }
            return View();
        }
    }
}
