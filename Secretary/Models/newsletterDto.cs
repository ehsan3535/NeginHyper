using Entities;
using Secretary.Models.ShopCards;
using System;
using System.Collections.Generic;
using WebFramework.Api;

namespace Secretary.Models
{
    public class newsletterDto : BaseDto<newsletterDto, newsletter, Guid>
    {
        public string PhoneNumber { get; set; }

    }
}
