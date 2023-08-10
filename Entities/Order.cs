using Entities.Constants;
using Entities.Product;
using System;
using System.Collections.Generic;

namespace Entities.Orders
{
    public class Order : BaseEntity
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Address.Address Address { get; set; }
        public Guid AddressId { get; set; }
        public FreeTime.FreeTime FreeTime { get; set; }
        public Guid FreeTimeId { get; set; }
        public DateTime DateTime { get; set; }
        public int FactorNumber { get; set; }
        public int TotalPrice { get; set; }
        public int DiscountPercent { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public class OrderDetail : BaseEntity
    {
        public int Price { get; set; }
        public int Count { get; set; }
        public Products Products { get; set; }
        public Guid ProductsId { get; set; }
        public Order Order { get; set; }
        public Guid OrderId { get; set; }
    }
}
