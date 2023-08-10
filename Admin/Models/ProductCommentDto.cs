using Entities.Constants;
using Entities.ProductComment;
using WebFramework.Api;

namespace Client.Models.CommentsDto
{
    public class ProductCommentDto : BaseDto<ProductCommentDto, ProductComments, Guid>
    {
        public string Text { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Rate { get; set; }

        public Guid? UserId { get; set; }
        public Guid ProductId { get; set; }
        public CommentStatus CommentStatus { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
