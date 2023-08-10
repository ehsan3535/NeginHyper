using System;

namespace Entities
{
    public class Setting : BaseEntity
    {
        public string MerchantId { get; set; }
        public string JoinCost { get; set; }
        public string VerifyLink { get; set; }
        public int PostPrice { get; set; }
        public int FreePost { get; set; }

        public string PHoneNumber1 { get; set; }
        public string PHoneNumber2 { get; set; }

        public string TopImageLink1 { get; set; }
        public string TopImageLink2 { get; set; }
        public string TopImageLink3 { get; set; }
        public string BottemRightImageLink { get; set; }
        public string BottemLeftImageLink { get; set; }

        public string TopImageUrl1 { get; set; }
        public string TopImageUrl2 { get; set; }
        public string TopImageUrl3 { get; set; }
        public string BottemRightImageUrl { get; set; }
        public string BottemLeftImageUrl { get; set; }

        public bool SeggestActivation { get; set; }
        public DateTime SeggestDate { get; set; }
        public string SeggestHour { get; set; }
    }
}
