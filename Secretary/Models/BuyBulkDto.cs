using Entities;
using Secretary.Models.ShopCards;
using System;
using System.Collections.Generic;
using WebFramework.Api;

namespace Secretary.Models
{
    public class BuyBulkDto : BaseDto<BuyBulkDto, BuyBulk, Guid>
    {
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
    }
}
