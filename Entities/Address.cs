using Entities.CityProvince;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Entities.Address
{
    public class Address : BaseEntity
    {
        public User Client { get; set; }
        public Guid ClientId { get; set; }
        public City City { get; set; }
        public Guid CityId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public string Location { get; set; }
        public string pelak { get; set; }
        public string vahed { get; set; }
        public bool FromArian { get; set; }
		public ICollection<City> Cities { get; set; }
	}
}
