using Entities;
using Secretary.Models.ShopCards;
using System;
using System.Collections.Generic;
using WebFramework.Api;

namespace Secretary.Models
{
    public class partnershipDto : BaseDto<partnershipDto, Partnership, Guid>
    {
        public Guid CityId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Age { get; set; }
        public string CityName { get; set; }
        public string Province { get; set; }
        public string ProductName { get; set; }
        public string ProductionValume { get; set; }
        public string HowToMeetGilHyper { get; set; }
        public List<PartnerShipProductDto> PartnerShipProduct { get; set; }

    }
}
