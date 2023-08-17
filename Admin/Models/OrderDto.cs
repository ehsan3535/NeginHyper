using Entities.Constants;
using Entities.Orders;
using WebFramework.Api;
namespace Client.Models.Orders
{
    public class OrderDto : BaseDto<OrderDto, Order, Guid>
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserUserName { get; set; }
        public string UserFName { get; set; }
        public string UserLName { get; set; }
        public string UserAddress { get; set; }
        public int TotalPrice { get; set; }
        public int FactorNumber { get; set; }
        public int DiscountPercent { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string PaymentImg { get; set; }
        public IFormFile File { get; set; }
        public int PostPrice { get; set; }
        public Guid? AddressId { get; set; }
        public Guid? FreeTimeId { get; set; }
        public string Day { get; set; }
        public string Hour { get; set; }
        public string AddressLocation { get; set; }
        public List<AddressDto> Addresses { get; set; }
    }
    public class OrderDetailDto : BaseDto<OrderDetailDto, OrderDetail, Guid>
    {
        public int Price { get; set; }
        public int Number { get; set; }
        public int Count { get; set; }
        public Guid ProductsId { get; set; }
        public Guid OrderId { get; set; }
        public string ProductsName { get; set; }
        public int ProductsDiscount { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string ProductsImageLink { get; set; }


    }

}
