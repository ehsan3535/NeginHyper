using Entities;
using Microsoft.AspNetCore.Http;
using System;
using WebFramework.Api;

namespace Secretary.Models
{
    public class ProductCategoryDto : BaseDto<ProductCategoryDto, ProductCategory, Guid>
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public IFormFile File { get; set; }
        public IFormFile MenuImageFile { get; set; }
        public string ImageUrl { get; set; }
        public string MenuImageUrl { get; set; }
        public string Description { get; set; }
    }
}
