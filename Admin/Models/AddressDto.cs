using Client.Models.ShopCards;
using Entities.Address;
using WebFramework.Api;

namespace Client.Models
{
    public class AddressDto : BaseDto<AddressDto, Address, Guid>
    {
        public Guid ClientId { get; set; }
        public Guid CityId { get; set; }
        public string CityName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ClientFname { get; set; }
        public string ClientLname { get; set; }
        public string? ReturnUrl { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public string Location { get; set; }
        public string pelak { get; set; }
        public string vahed { get; set; }
        public bool FromArian { get; set; }

        public List<ProvinceDto> provinces { get; set; }
        public List<AddressDto> Addresses { get; set; }

    }
}
