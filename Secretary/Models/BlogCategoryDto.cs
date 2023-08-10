using Entities;
using System;
using WebFramework.Api;

namespace Secretary.Models
{
    public class BlogCategoryDto : BaseDto<BlogCategoryDto, BlogCategory, Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
