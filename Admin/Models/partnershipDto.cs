using Client.Models.ShopCards;
using Entities;
using WebFramework.Api;

namespace Client.Models
{
    public class partnershipDto : BaseDto<partnershipDto, Partnership, Guid>
    {
        public Guid CityId { get; set; }
        public string CityName { get; set; }
        public string ProductList { get; set; }
        public string CountList { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Age { get; set; }
        public string HowToMeetGilHyper { get; set; }
        public List<ProvinceDto> Provinces { get; set; }
    }
}
