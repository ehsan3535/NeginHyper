using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using WebFramework.Api;

namespace Client.Models
{
    public class OurClientDto : BaseDto<OurClientDto, OurClient, Guid>
    {
        public string Name { get; set; }
        public string ImageLink { get; set; }
        public IFormFile File { get; set; }
    }
}
