using Entities;
using WebFramework.Api;

namespace Client.Models
{
    public class BlogDto : BaseDto<BlogDto, Blog, Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string ImageLink { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreationDateTime { get; set; }
        public int Rate { get; set; }
        public int TotalRate { get; set; }
        public int SiteView { get; set; }

        public List<BlogCommentDto> Comments { get; set; }
        public List<BlogCategoryDto> Categories { get; set; }


    }
}
