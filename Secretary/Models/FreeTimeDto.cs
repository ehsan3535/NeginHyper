using Entities.FreeTime;
using System;
using System.Collections.Generic;
using WebFramework.Api;

namespace Secretary.Models
{
    public class FreeTimeDto : BaseDto<FreeTimeDto, FreeTime, Guid>
    {
        public string FromHour { get; set; }
        public string ToHour { get; set; }
        public string Day { get; set; }
        public DateTime DateTime { get; set; }
        public bool Free { get; set; }

        public List<FreeTimeDto> AllTimes { get; set; }
    }
}
