using Entities;
using WebFramework.Api;

namespace Client.Models
{
    public class ContactDto : BaseDto<ContactDto, Contact, Guid>
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }


}
