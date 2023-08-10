using Entities;
using WebFramework.Api;

namespace Client.Models
{
    public class DiscountDto : BaseDto<DiscountDto, Discount, Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int DiscountPercent { get; set; }

    }
}
