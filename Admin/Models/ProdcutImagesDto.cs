using Entities;
using WebFramework.Api;

namespace Client.Models
{
    public class ProductImagesDto : BaseDto<ProductImagesDto, ProductImage, Guid>
    {
        public Guid ProductId { get; set; }
        public string ImageLink { get; set; }
    }
}
