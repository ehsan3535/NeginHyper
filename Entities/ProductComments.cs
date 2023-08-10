using Entities.Constants;
using System;

namespace Entities.ProductComment
{
    public class ProductComments : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public int Rate { get; set; }
        public User User { get; set; }
        public Guid? UserId { get; set; }
        public Product.Products Product { get; set; }
        public Guid ProductId { get; set; }
        public CommentStatus CommentStatus { get; set; }

    }
}
