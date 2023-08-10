using Entities.CityProvince;
using System;

namespace Entities
{
    public class Partnership : BaseEntity
    {
        public City City { get; set; }
        public Guid CityId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Age { get; set; }
        public string HowToMeetGilHyper { get; set; }

    }
}
