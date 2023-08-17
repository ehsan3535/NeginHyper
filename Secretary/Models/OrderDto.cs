using Entities.Constants;
using Entities.Orders;
using System;
using System.Collections.Generic;
using WebFramework.Api;
namespace Secretary.Models.Orders
{
    public class OrderDto : BaseDto<OrderDto, Order, Guid>
    {
        public Guid FreeTimeId { get; set; }
        public string FreeTimeFromHour { get; set; }
        public string FreeTimeToHour { get; set; }
        public string FreeTimeDay { get; set; }
        public string FreeTimeTitle { get; set; }
        public Guid AddressId { get; set; }
        public string AddressLocation { get; set; }
        public Guid UserId { get; set; }
        public string UserUserName { get; set; }
        public string UserFName { get; set; }
        public string UserLName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserAddress { get; set; }
        public int TotalPrice { get; set; }
        public int FactorNumber { get; set; }

        public string DiscountCode { get; set; }
        public int DiscountPercent { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }

        public string PaymentImg { get; set; }

        //factor
        public List<AddressDto> Addresses { get; set; }
        public int PostPrice { get; set; }
        public string Day { get; set; }
        public string Hour { get; set; }
        public DateTime CreationDateTime { get; set; }

    }
    public class OrderDetailDto : BaseDto<OrderDetailDto, OrderDetail, Guid>
    {
        public int Price { get; set; }
        public int Number { get; set; }
        public int Count { get; set; }
        public Guid ProductsId { get; set; }
        public Guid OrderId { get; set; }
        public string ProductsName { get; set; }
        public string ProductsImageLink { get; set; }
        public DateTime CreationDateTime { get; set; }


    }

}
