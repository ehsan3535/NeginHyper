using Entities.Product;
using System;
using System.Collections.Generic;

namespace Entities.ShopCards
{
    public class ShopCard : BaseEntity
    {
        public Guid CookieId { get; set; }
        public User User { get; set; }
        public Guid? UserId { get; set; }
        public int TotalPrice { get; set; }
        public int DiscountPercent { get; set; }
        public string DiscountCode { get; set; }
        public int FinalTotalPrice { get; set; }

        public ICollection<ShopCardDetail> ShopCardDetails { get; set; }

    }

    public class ShopCardDetail : BaseEntity
    {
        public int Price { get; set; }
        public int Count { get; set; }
        public int DisCount { get; set; }
        public ShopCard ShopCard { get; set; }
        public Guid ShopCardId { get; set; }
        public Products Products { get; set; }
        public Guid ProductsId { get; set; }
        public DateTime CreationDateTime { get; set; }


    }
}
