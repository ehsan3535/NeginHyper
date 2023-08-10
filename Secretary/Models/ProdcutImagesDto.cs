using Entities;
using System;
using WebFramework.Api;

namespace Secretary.Models
{
    public class ProductImagesDto : BaseDto<ProductImagesDto, ProductImage, Guid>
    {
        public Guid ProductId { get; set; }
        public string ImageLink { get; set; }
    }
}
