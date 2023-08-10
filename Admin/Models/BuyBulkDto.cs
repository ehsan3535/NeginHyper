using Entities;
using WebFramework.Api;

namespace Client.Models
{
    public class BuyBulkDto : BaseDto<BuyBulkDto, BuyBulk, Guid>
    {
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
    }
}
