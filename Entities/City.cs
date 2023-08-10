using System;

namespace Entities.CityProvince
{
    public class City : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public Province Province { get; set; }
        public Guid ProvinceId { get; set; }
    }
}
