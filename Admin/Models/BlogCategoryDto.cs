using Entities;
using WebFramework.Api;

namespace Client.Models
{
    public class BlogCategoryDto : BaseDto<BlogCategoryDto, BlogCategory, Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
