using Entities.Address;
using System;
using WebFramework.Api;

namespace Secretary.Models
{
    public class AddressDto : BaseDto<AddressDto, Address, Guid>
    {
		public Guid CityId { get; set; }
		public string CityName { get; set; }
        public Guid ClientId { get; set; }
		public string ClientPhoneNumber { get; set; }
		public string ClientFname { get; set; }
		public string ClientLname { get; set; }
		public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public string province { get; set; }
        public string City { get; set; }
        public string Location { get; set; }
		public string pelak { get; set; }
		public string vahed { get; set; }
        public bool FromArian { get; set; }

    }
}
