using Entities;
using System;
using WebFramework.Api;

namespace Secretary.Models.ShopCards
{
	public class PartnerShipProductDto : BaseDto<PartnerShipProductDto, PartnerShipProducts, Guid>
	{
		public string Name { get; set; }
		public string Count { get; set; }
	}
}