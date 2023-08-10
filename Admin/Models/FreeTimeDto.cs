using Entities.FreeTime;
using WebFramework.Api;

namespace Client.Models
{
    public class FreeTimeDto : BaseDto<FreeTimeDto, FreeTime, Guid>
    {
        public string FromHour { get; set; }
        public string ToHour { get; set; }
        public string Day { get; set; }
        public DateTime DateTime { get; set; }
        public bool Free { get; set; }
        public string Title { get; set; }
        public bool Out { get; set; }
    }
}
