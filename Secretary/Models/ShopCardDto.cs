using Entities.ShopCards;
using System;
using System.Collections.Generic;
using WebFramework.Api;

namespace Secretary.Models.ShopCards
{
    public class ShopCardDto : BaseDto<ShopCardDto, ShopCard, Guid>
    {
        public Guid OrderReportId { get; set; }
        public int OrderReportTax { get; set; }
        public int OrderReportPostCost { get; set; }

        public Guid UserId { get; set; }
        public string UserUserName { get; set; }
        public string UserAddress { get; set; }
        public int TotalPrice { get; set; }
        public int FinalTotalPrice { get; set; }
        public List<ShopCardDetailDto> ShopCardDetails { get; set; }

    }



    public class ShopCardDetailDto : BaseDto<ShopCardDetailDto, ShopCardDetail, Guid>
    {
        public int Price { get; set; }
        public int Count { get; set; }
        public int DisCount { get; set; }
        public Guid ProductId { get; set; }
        public string ProductPrice { get; set; }
        public string ProductName { get; set; }
        public string ProductImageLink { get; set; }
        public DateTime CreationDateTime { get; set; }

    }
}
