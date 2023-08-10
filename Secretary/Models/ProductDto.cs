using Entities.Product;
using Microsoft.AspNetCore.Http;
using Secretary.Models.CommentsDto;
using System;
using System.Collections.Generic;
using WebFramework.Api;

namespace Secretary.Models.ProductDto
{
    public class ProductDto : BaseDto<ProductDto, Products, Guid>
    {
        public ProductDto()
        {
            File = new List<IFormFile>();
        }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Pureweight { get; set; }
        public string Allweight { get; set; }
        public string GoodForWhat { get; set; }
        public string KeepWay { get; set; }
        public string KeepTime { get; set; }
        public string Matterial { get; set; }
        public string ImageCoverUrl { get; set; }
        public DateTime CreationDateTime { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
        public int darage { get; set; }
        public int Percent { get; set; }
        public int Count { get; set; }
        public int Rate { get; set; }
        public int TotalRate { get; set; }
        public Guid SameProduct1Id { get; set; }
        public Guid SameProduct2Id { get; set; }
        public Guid SameProduct3Id { get; set; }
        public List<ProductCategoryDto> Categories { get; set; }
        public List<ProductCommentDto> Comments { get; set; }
        public Guid CommentsId { get; set; }
        public Guid CommentsText { get; set; }
        public Guid ShopCartDeailCount { get; set; }
        public Guid CategorysId { get; set; }
        public string CategorysName { get; set; }
        public string CategorysCode { get; set; }
        public List<IFormFile> File { get; set; }
        public IFormFile File2 { get; set; }
        public string Detail { get; set; }
        public bool DiscountCheckBox { get; set; }
        
        public List<ProductImagesDto> ProductImages { get; set; }

    }
}
