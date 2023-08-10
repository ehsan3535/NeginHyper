using AutoMapper;
using AutoMapper.QueryableExtensions;
using Client.Models;
using Data.Repositories;
using Entities;
using Entities.BlogComnent;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Client.Controllers
{

    public class BlogController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<BlogCategory> categoryrepo;
        private readonly IRepository<Blog> Blogrepo;
        private readonly IRepository<BlogComment> BlogCommentrepo;
        private readonly IMapper mapper;
        private readonly IToastNotification notification;
        public BlogController(UserManager<User> userManager, IMapper mapper, IRepository<BlogCategory> categoryrepo, IRepository<Blog> Blogrepo, IRepository<BlogComment> BlogCommentrepo, IToastNotification notification)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.categoryrepo = categoryrepo;
            this.Blogrepo = Blogrepo;
            this.BlogCommentrepo = BlogCommentrepo;
            this.notification = notification;
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
            if (BlogId != Guid.Empty)
            {
                var model = await Blogrepo.TableNoTracking.Where(x => x.Id == BlogId).ProjectTo<BlogDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
                model.Comments = BlogCommentrepo.TableNoTracking.Where(x => x.CommentStatus == Entities.Constants.CommentStatus.Accepted && x.BlogId == BlogId).ProjectTo<BlogCommentDto>(mapper.ConfigurationProvider).ToList();
                model.SiteView += 1;
                var BlogEntity = mapper.Map<Blog>(model);
                Blogrepo.Update(BlogEntity);
                return View(model);
            }
            else
            {
                return View();
            }
        }
    }
}
