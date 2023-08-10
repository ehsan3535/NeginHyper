using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Secretary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{
    [Authorize(Roles = "Admin")]

    public class BlogController : Controller
    {
        public class uploadsuccess
        {
            public int Uploaded { get; set; }
            public string FileName { get; set; }
            public string Url { get; set; }
        }
        private readonly UserManager<User> userManager;
        private readonly IRepository<BlogCategory> categoryrepo;
        private readonly IRepository<Blog> Blogrepo;
        private readonly IMapper mapper;
        private readonly IToastNotification notification;

        public BlogController(UserManager<User> userManager, IMapper mapper, IRepository<BlogCategory> categoryrepo, IRepository<Blog> Blogrepo, IToastNotification notification)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.categoryrepo = categoryrepo;
            this.Blogrepo = Blogrepo;
            this.notification = notification;
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
            model.Categories = await categoryrepo.TableNoTracking.ProjectTo<BlogCategoryDto>(mapper.ConfigurationProvider).ToListAsync();
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
                var model = Blogrepo.GetById(Dto.Id);
                Dto.CreationDateTime = model.CreationDateTime;
                var Blog = Dto.ToEntity(mapper, model);
                if (Dto.Image != null)
                {
                    Blog.ImageLink = UploadImage.SaveImage(Dto.Image, "BlogImage");
                }
                await Blogrepo.UpdateAsync(Blog, cancellationToken);
            }
            return RedirectToAction("BlogList");
        }
        public async Task<IActionResult> BlogList(string? Searched, CancellationToken cancellationToken)
        {
            var model2 = await Blogrepo.TableNoTracking.ProjectTo<BlogDto>(mapper.ConfigurationProvider).OrderByDescending(x => x.CreationDateTime).ToListAsync(cancellationToken);
            var model = new List<BlogDto>();
            if (Searched != null)
            {
                foreach (var item in model2)
                {
                    if (item.Title.Contains(Searched))
                    {
                        model.Add(item);
                    }
                    model.OrderByDescending(x => x.CreationDateTime);
                }
                //اگر سرچ پیدا نکرد کل محصولات رو بفرست با نوتیفیکیشن
                if (!model.Any())
                {
                    notification.AddErrorToastMessage("محصول جست و جو شده یافت نشد", new NotyOptions
                    {
                        Timeout = 1000,
                        ProgressBar = true,
                        Modal = true,
                        Layout = "topCenter",
                        Theme = "metroui",
                    });
                }
            }
            if (!model.Any())
            {
                model = model2;
            }
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
