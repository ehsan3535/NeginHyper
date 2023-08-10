using Entities.Constants;
using System;

namespace Entities.BlogComnent
{
    public class BlogComment : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public int Rate { get; set; }
        public User User { get; set; }
        public Guid? UserId { get; set; }
        public Blog Blog { get; set; }
        public Guid BlogId { get; set; }
        public CommentStatus CommentStatus { get; set; }
    }
}
