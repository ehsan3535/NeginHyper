using System;

namespace Entities.FreeTime
{
    public class FreeTime : BaseEntity
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
