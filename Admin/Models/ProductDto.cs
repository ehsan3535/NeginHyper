using Client.Models.CommentsDto;
using Client.Models.ShopCards;
using Entities;
using Entities.Product;
using WebFramework.Api;

namespace Client.Models.ProductDto
{
    public class ProductDto : BaseDto<ProductDto, Products, Guid>
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Pureweight { get; set; }
        public string Allweight { get; set; }
        public string GoodForWhat { get; set; }
        public string KeepWay { get; set; }
        public string KeepTime { get; set; }
        public string Matterial { get; set; }
        public string ImageCoverUrl { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
        public int Percent { get; set; }
        public int Count { get; set; }
        public int darage { get; set; }

        public int Rate { get; set; }
        public int TotalRate { get; set; }
        public Guid SameProduct1Id { get; set; }
        public Guid SameProduct2Id { get; set; }
        public Guid SameProduct3Id { get; set; }
        public List<ProductCategoryDto> Categories { get; set; }
        public List<ProductCommentDto> Comments { get; set; }
        public List<ProductDto> Products { get; set; }
        public List<ProductDto> Products2 { get; set; }
        public List<ProductImagesDto> ProductImages { get; set; }
        public List<OurClientDto> OurClient { get; set; }
        public SettingDto SettingDto { get; set; }
        public ShopCardDto ShopCard { get; set; }

        public Guid CommentsId { get; set; }
        public Guid CommentsText { get; set; }
        public Guid CategorysId { get; set; }
        public int ShopCartDeailCount { get; set; }
        public string CategorysName { get; set; }

        public string CategorysMenuImageUrl { get; set; }
        public string CategorysDescription { get; set; }
        public string CategorysCode { get; set; }
        public IFormFile File { get; set; }
        public IFormFile File2 { get; set; }
        public string Detail { get; set; }
        public DateTime CreationDateTime { get; set; }

    }
}
