using Entities;
using System;
using WebFramework.Api;

namespace Secretary.Models
{
    public class ContactDto : BaseDto<ContactDto, Contact, Guid>
    {
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

    }


}
