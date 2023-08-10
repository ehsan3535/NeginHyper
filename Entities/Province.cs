using System;
using System.Collections.Generic;

namespace Entities.CityProvince
{
    public class Province : BaseEntity<Guid>
    {
        public Province()
        {
            Cities = new HashSet<City>();
        }
        public string Name { get; set; }
        public ICollection<City> Cities { get; set; }
    }
}
