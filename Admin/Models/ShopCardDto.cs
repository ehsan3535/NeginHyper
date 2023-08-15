using Entities.ShopCards;
using WebFramework.Api;

namespace Client.Models.ShopCards
{
    public class ShopCardDto : BaseDto<ShopCardDto, ShopCard, Guid>
    {
        public Guid? AddressId { get; set; }
        //public string? Addressvahed { get; set; }
        //public string? AddressPostalCode { get; set; }
        //public string? AddressCityName { get; set; }
        //public string? Addresspelak { get; set; }
        //public string? AddressLocation { get; set; }
        public AddressDto Address { get; set; }
        public Guid? FreeTimeId { get; set; }
        public string? Day { get; set; }
        public DateTime? DateTime { get; set; }
        public string? FromHour { get; set; }
        public Guid OrderReportId { get; set; }
        public int OrderReportTax { get; set; }
        public int OrderReportPostCost { get; set; }
        public Guid UserId { get; set; }
        //this field must full by yourself
        public string UserUserName { get; set; }
        public string UserAddress { get; set; }
        public int TotalPrice { get; set; }
        public int PostPrice { get; set; }
        public int DiscountPercent { get; set; }
        public string DiscountCode { get; set; }
        public int UntilFreePost { get; set; }
        public int FinalTotalPrice { get; set; }
        public List<ShopCardDetailDto> ShopCardDetails { get; set; }
        public List<AddressDto> Addresses { get; set; }
        public List<FreeTimeDto> FreeTimes { get; set; }
        public List<ProvinceDto> Provinces { get; set; }
        public List<CityDto> Cities { get; set; }
        public string ClientId { get; set; }
        public bool Out { get; set; }
        public string FreeTimeTitle { get; set; }
        public bool DiscountCheckBox { get; set; }
        public bool FromArian { get; set; }


    }



    public class ShopCardDetailDto : BaseDto<ShopCardDetailDto, ShopCardDetail, Guid>
    {
        public int Price { get; set; }
        public int Count { get; set; }
        public int DisCount { get; set; }
        public Guid ProductsId { get; set; }
        public int ProductsPrice { get; set; }
        public int ProductsDiscount { get; set; }
        public string ProductsName { get; set; }
        public string ProductsImageLink { get; set; }
        public DateTime CreationDateTime { get; set; }




    }
}
