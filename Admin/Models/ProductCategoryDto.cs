using Entities;
using WebFramework.Api;

namespace Client.Models
{
    public class ProductCategoryDto : BaseDto<ProductCategoryDto, ProductCategory, Guid>
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string MenuImageUrl { get; set; }
    }
}
