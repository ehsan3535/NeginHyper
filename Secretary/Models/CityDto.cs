using Entities.CityProvince;
using System;
using WebFramework.Api;

namespace Secretary.Models.ShopCards
{
	public class CityDto : BaseDto<CityDto,City,Guid>
	{
		public string Name { get; set; }
		public Guid ProvinceId { get; set; }
		public string ProvinceName { get; set; }

	}
}