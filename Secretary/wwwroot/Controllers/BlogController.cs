using Secretary.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{
    public class BlogController : Controller
    {
        public class uploadsuccess
        {
            public int Uploaded { get; set; }
            public string FileName { get; set; }
            public string Url { get; set; }
        }
        private readonly UserManager<User> userManager;
        private readonly IRepository<Category> categoryrepo;
        private readonly IRepository<Blog> Blogrepo;
        private readonly IMapper mapper;
        public BlogController(UserManager<User> userManager, IMapper mapper, IRepository<Category> categoryrepo, IRepository<Blog> Blogrepo)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.categoryrepo = categoryrepo;
            this.Blogrepo = Blogrepo;
        }

        [HttpPost]
        public async Task<JsonResult> SaveImage([FromForm] IFormFile upload)
        {
            if (upload.Length <= 0) return null;
            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();
            var url = UploadImage.SaveImageCkEditor(upload, fileName);
            var success = new uploadsuccess
            {
                Uploaded = 1,
                FileName = fileName,
                Url = url
            };
            var Request = new JsonResult(success);
            Request.StatusCode = 200;
            Request.ContentType = "application/json";
            return Request;
        }

        public async Task<IActionResult> Add_Edit_Blog(Guid? BlogId, CancellationToken cancellationToken)
        {
            var model = await Blogrepo.TableNoTracking.Where(x => x.Id == BlogId).ProjectTo<BlogDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            if (model == null)
            {
                model = new();
            }
            model.Categories = await categoryrepo.TableNoTracking.ProjectTo<CategoryDto>(mapper.ConfigurationProvider).ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add_Edit_Blog(BlogDto Dto, CancellationToken cancellationToken)
        {
            if (Dto.Id == Guid.Empty)
            {
                var Blog = Dto.ToEntity(mapper);
                Blog.ImageLink = UploadImage.SaveImage(Dto.Image, "BlogImage");
                await Blogrepo.AddAsync(Blog, cancellationToken);
            }
            else
            {
                var model = await Blogrepo.TableNoTracking.Where(x => x.Id == Dto.Id).FirstOrDefaultAsync(cancellationToken);
                var Blog = Dto.ToEntity(mapper, model);
                Blog.ImageLink = UploadImage.SaveImage(Dto.Image, "BlogImage");
                await Blogrepo.UpdateAsync(Blog, cancellationToken);
            }


            return RedirectToAction("BlogList");
        }

        public async Task<IActionResult> BlogList(CancellationToken cancellationToken)
        {
            var model = await Blogrepo.TableNoTracking.ProjectTo<BlogDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return View(model);
        }

        public async Task<IActionResult> BlogDetail(Guid BlogId, CancellationToken cancellationToken)
        {
            var model = await Blogrepo.TableNoTracking.Where(x => x.Id == BlogId).ProjectTo<BlogDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
            return View(model);
        }

        public async Task<IActionResult> DeleteBlog(Guid BlogId, CancellationToken cancellationToken)
        {
            var Blog = await Blogrepo.TableNoTracking.Where(x => x.Id == BlogId).FirstOrDefaultAsync();
            Blog.Active = false;
            await Blogrepo.UpdateAsync(Blog, cancellationToken);
            return RedirectToAction("BlogList");
        }
    }
}
