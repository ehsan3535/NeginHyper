using Entities.CityProvince;
using WebFramework.Api;

namespace Client.Models.ShopCards
{
    public class ProvinceDto : BaseDto<ProvinceDto, Province, Guid>
    {
        public string Name { get; set; }
        public ICollection<City> Cities { get; set; }
    }
}