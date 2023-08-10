using Entities.CityProvince;
using WebFramework.Api;

namespace Client.Models.ShopCards
{
    public class CityDto : BaseDto<CityDto, City, Guid>
    {
        public string Name { get; set; }
        public Guid ProvinceId { get; set; }
    }
}