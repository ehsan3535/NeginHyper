using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using WebFramework.Api;

namespace Secretary.Models
{
    public class BlogDto : BaseDto<BlogDto, Blog, Guid>
    {

        public string Title { get; set; }
        public int SiteView { get; set; }

        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string ImageLink { get; set; }
        public DateTime CreationDateTime { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<BlogCommentDto> BlogComments { get; set; }
        public List<BlogCategoryDto> Categories { get; set; }
    }
}
