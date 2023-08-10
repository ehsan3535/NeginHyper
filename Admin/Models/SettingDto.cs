using Entities;
using Microsoft.AspNetCore.Http;
using System;
using WebFramework.Api;

namespace Client.Models
{
    public class SettingDto : BaseDto<SettingDto, Setting, Guid>
    {
        public string MerchantId { get; set; }
        public string JoinCost { get; set; }
        public string VerifyLink { get; set; }
        public int PostPrice { get; set; }
        public int FreePost { get; set; }
        public string PHoneNumber1 { get; set; }
        public string PHoneNumber2 { get; set; }

        public string TopImageUrl1 { get; set; }
        public string TopImageUrl2 { get; set; }
        public string TopImageUrl3 { get; set; }
        public string BottemRightImageUrl { get; set; }
        public string BottemLeftImageUrl { get; set; }

        public string TopImageLink1 { get; set; }
        public string TopImageLink2 { get; set; }
        public string TopImageLink3 { get; set; }
        public string BottemRightImageLink { get; set; }
        public string BottemLeftImageLink { get; set; } 
        
        public IFormFile TopImageFile1 { get; set; }
        public IFormFile TopImageFile2 { get; set; }
        public IFormFile TopImageFile3 { get; set; }
        public IFormFile BottemRightImageFile { get; set; }
        public IFormFile BottemLeftImageFile { get; set; }

        public bool SeggestActivation { get; set; }
        public DateTime SeggestDate { get; set; }
        public string SeggestHour { get; set; }
        public int FinalTimer { get; set; }
    }
}
