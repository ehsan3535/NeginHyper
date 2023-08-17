using Entities.CityProvince;
using WebFramework.Api;

namespace Client.Models
{
    public class SubmitOrderDto 
    {
        public IFormFile File { get; set; }
        public string PaymentType { get; set; }
        public Guid ShopCardId { get; set; }
        public Guid AddressId { get; set; }
        public Guid FreeTimeId { get; set; }
    }
}