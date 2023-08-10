using Secretary.Models;
using Secretary.Models.CommentsDto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Entities;
using Entities.Comment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Secretary.Controllers.Comment
{
    public class CommentController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IRepository<Comments> Commentripo;
        private readonly IRepository<BlogComment> BlogCommentripo;

        public CommentController(IMapper mapper, IRepository<Comments> Commentripo, UserManager<User> userManager, IRepository<BlogComment> BlogCommentripo)
        {
            this.mapper = mapper;
            this.Commentripo = Commentripo;
            this.userManager = userManager;
            this.BlogCommentripo = BlogCommentripo;
        }

        public async Task<IActionResult> CourseCommentList()
        {
            var model = Commentripo.TableNoTracking.ProjectTo<CommentDto>(mapper.ConfigurationProvider).ToList();
            return View(model);
        }
        public async Task<IActionResult> BlogCommentList()
        {
            var model = BlogCommentripo.TableNoTracking.ProjectTo<BlogCommentDto>(mapper.ConfigurationProvider).ToList();
            return View(model);
        }
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(Guid BlogCommentId, Guid CourseCommentId, CancellationToken cancellationToken)
        {
            if (CourseCommentId != Guid.Empty)
            {
                var model = Commentripo.GetById(CourseCommentId);
                model.Active = false;
                await Commentripo.UpdateAsync(model, cancellationToken);
                return RedirectToAction("CourseCommentList");
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
        public async Task<IActionResult> Accept_Reject_Comment(Guid BlogCommentId, Guid CourseCommentId, bool Accept, CancellationToken cancellationToken)
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
            else if (CourseCommentId != Guid.Empty)
            {
                var Comment = Commentripo.GetById(CourseCommentId);
                if (Accept == true)
                {
                    Comment.CommentStatus = Entities.Constants.CommentStatus.Accepted;
                }
                else
                {
                    Comment.CommentStatus = Entities.Constants.CommentStatus.InAccepted;
                }
                await Commentripo.UpdateAsync(Comment, cancellationToken);
            }
            return RedirectToAction(nameof(CourseCommentList));
        }
    }
}
