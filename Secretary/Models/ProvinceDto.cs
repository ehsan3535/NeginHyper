using Entities.CityProvince;
using System;
using System.Collections.Generic;
using WebFramework.Api;

namespace Secretary.Models.ShopCards
{
	public class ProvinceDto : BaseDto<ProvinceDto,Province , Guid>
	{
		public string Name { get; set; }
		public ICollection<City> Cities { get; set; }
	}
}