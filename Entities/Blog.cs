using Entities.BlogComnent;
using System;
using System.Collections.Generic;

namespace Entities
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageLink { get; set; }
        public int TotalRate { get; set; }
        public int Rate { get; set; }
        public int SiteView { get; set; }
        public BlogCategory Category { get; set; }
        public Guid CategoryId { get; set; }
        public ICollection<BlogCategory> Categories { get; set; }
        public ICollection<BlogComment> Comments { get; set; }
    }
}
