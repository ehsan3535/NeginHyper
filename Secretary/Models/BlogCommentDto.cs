using Entities.BlogComnent;
using Entities.Constants;
using System;
using WebFramework.Api;

namespace Secretary.Models
{
    public class BlogCommentDto : BaseDto<BlogCommentDto, BlogComment, Guid>
    {
        public string Text { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Rate { get; set; }
        public Guid? UserId { get; set; }
        public Guid BlogId { get; set; }
        public string BlogTitle { get; set; }
        public CommentStatus CommentStatus { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
