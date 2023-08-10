using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Entities;
using Entities.BlogComnent;
using Entities.Product;
using Entities.ProductComment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Secretary.Models;
using Secretary.Models.CommentsDto;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers
{
    [Authorize(Roles = "Admin")]

    public class CommentController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IRepository<ProductComments> Commentripo;
        private readonly IRepository<BlogComment> BlogCommentripo;
        private readonly IRepository<Products> productripo;

        public CommentController(IMapper mapper, IRepository<ProductComments> Commentripo, UserManager<User> userManager, IRepository<BlogComment> BlogCommentripo, IRepository<Products> productripo)
        {
            this.mapper = mapper;
            this.Commentripo = Commentripo;
            this.userManager = userManager;
            this.BlogCommentripo = BlogCommentripo;
            this.productripo = productripo;
        }
        public async Task<IActionResult> ProductCommentList()
        {
            var model = Commentripo.TableNoTracking.ProjectTo<ProductCommentDto>(mapper.ConfigurationProvider).ToList();
            return View(model);
        }
        public async Task<IActionResult> BlogCommentList()
        {
            var model = BlogCommentripo.TableNoTracking.ProjectTo<BlogCommentDto>(mapper.ConfigurationProvider).ToList();
            return View(model);
        }
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(Guid BlogCommentId, Guid ProductCommentId, CancellationToken cancellationToken)
        {
            if (ProductCommentId != Guid.Empty)
            {
                var Comment = Commentripo.GetById(ProductCommentId);
                var ProductComment = await Commentripo.TableNoTracking.Where(x => x.Id == ProductCommentId && x.CommentStatus == Entities.Constants.CommentStatus.Accepted).ToListAsync();
                var Product = await productripo.TableNoTracking.Where(x => x.Id == Comment.ProductId).FirstOrDefaultAsync();
                var model = Commentripo.GetById(ProductCommentId);
                model.Active = false;
                await Commentripo.UpdateAsync(model, cancellationToken);
                Product.TotalRate -= Comment.Rate;
                var Count = ProductComment.Count() - 1;
                if (Product.TotalRate == 0)
                {
                    Product.Rate = 0;
                }
                else
                {
                    Product.Rate = Product.TotalRate / Count;
                }
                await productripo.UpdateAsync(Product, cancellationToken);
                return RedirectToAction("ProductCommentList");
            }
            else if (BlogCommentId != Guid.Empty)
            {
                var model = BlogCommentripo.GetById(BlogCommentId);
                model.Active = false;
                await BlogCommentripo.UpdateAsync(model, cancellationToken);
            }
            return RedirectToAction("BlogCommentList");
            //[Authorize(Roles = "Admin")]
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Accept_Reject_Comment(Guid BlogCommentId, Guid ProductCommentId, bool Accept, CancellationToken cancellationToken)
        {
            if (BlogCommentId != Guid.Empty)
            {
                var Comment = BlogCommentripo.GetById(BlogCommentId);
                if (Accept == true)
                {
                    Comment.CommentStatus = Entities.Constants.CommentStatus.Accepted;
                }
                else
                {
                    Comment.CommentStatus = Entities.Constants.CommentStatus.InAccepted;
                }
                await BlogCommentripo.UpdateAsync(Comment, cancellationToken);
                return RedirectToAction(nameof(BlogCommentList));
            }
            else if (ProductCommentId != Guid.Empty)
            {
                var Comment = Commentripo.GetById(ProductCommentId);
                var Product = await productripo.TableNoTracking.Where(x => x.Id == Comment.ProductId).FirstOrDefaultAsync();
                var ProductComment = await Commentripo.TableNoTracking.Where(x => x.ProductId == Comment.ProductId && x.CommentStatus == Entities.Constants.CommentStatus.Accepted).ToListAsync();

                if (Accept == true)
                {
                    Comment.CommentStatus = Entities.Constants.CommentStatus.Accepted;
                    Product.TotalRate += Comment.Rate;
                    var Count = ProductComment.Count() + 1;
                    Product.Rate = Product.TotalRate / Count;
                }
                else
                {
                    Comment.CommentStatus = Entities.Constants.CommentStatus.InAccepted;
                    Product.TotalRate -= Comment.Rate;
                    var Count = ProductComment.Count() - 1;
                    if (Product.TotalRate == 0)
                    {
                        Product.Rate = 0;
                    }
                    else
                    {
                        Product.Rate = Product.TotalRate / Count;
                    }
                }
                await productripo.UpdateAsync(Product, cancellationToken);
                await Commentripo.UpdateAsync(Comment, cancellationToken);
            }
            return RedirectToAction(nameof(ProductCommentList));
        }
    }
}
