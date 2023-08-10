using AutoMapper;
using Client.Models;
using Client.Models.CommentsDto;
using Data.Repositories;
using Entities;
using Entities.BlogComnent;
using Entities.Product;
using Entities.ProductComment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Client.Controllers
{

    public class CommentController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IRepository<BlogComment> blogCommentripo;
        private readonly IRepository<ProductComments> Commentripo;
        private readonly IRepository<Products> productripo;
        private readonly IRepository<Blog> blogrepo;

        public CommentController(IMapper mapper, IRepository<ProductComments> Commentripo, UserManager<User> userManager, IRepository<BlogComment> blogCommentripo, IRepository<Products> productripo, IRepository<Blog> blogrepo)
        {
            this.mapper = mapper;
            this.Commentripo = Commentripo;
            this.userManager = userManager;
            this.blogCommentripo = blogCommentripo;
            this.productripo = productripo;
            this.blogrepo = blogrepo;
        }
        public async Task<IActionResult> AddBlogComment()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddBlogComment(BlogCommentDto Dto, CancellationToken cancellationToken)
        {
            var Blog = await blogrepo.TableNoTracking.Where(x => x.Id == Dto.BlogId).FirstOrDefaultAsync();
            var BlogComment = await blogCommentripo.TableNoTracking.Where(x => x.BlogId == Dto.BlogId).ToListAsync();
            Blog.TotalRate += Dto.Rate;
            var Count = BlogComment.Count() + 1;
            Blog.Rate = Blog.TotalRate / Count;
            await blogrepo.UpdateAsync(Blog, cancellationToken);

            var model = Dto.ToEntity(mapper);
            model.BlogId = Dto.BlogId;
            model.CommentStatus = Entities.Constants.CommentStatus.InAccepted;
            model.CreationDateTime = DateTime.Now;
            await blogCommentripo.AddAsync(model, cancellationToken);
            return LocalRedirect($"/Blog/BlogDetail?BlogId={model.BlogId}");
        }
        public async Task<IActionResult> AddComment()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(ProductCommentDto Dto, CancellationToken cancellationToken)
        {
            var model = Dto.ToEntity(mapper);
            model.ProductId = Dto.ProductId;
            model.CommentStatus = Entities.Constants.CommentStatus.InAccepted;
            model.CreationDateTime = DateTime.Now;
            await Commentripo.AddAsync(model, cancellationToken);
            return LocalRedirect($"/Product/ProductDetail_User?Id={model.ProductId}");
        }
    }
}
