using Entities.Notifications;
using System;
using System.Collections.Generic;
using WebFramework.Api;

namespace Secretary.Models
{
    public class NotificationDto : BaseDto<NotificationDto, Notification, Guid>
    {
        public NotificationDto()
        {
            DateTimes = DateTime.Now;
        }

        public string Topic { get; set; }
        public string Text { get; set; }
        public DateTime? DateTimes { get; set; }
        public Guid UserId { get; set; }
        public string UserFname { get; set; }
        public string UserLname { get; set; }
        public string UserUserName { get; set; }
        public List<UserDto> Users { get; set; }

    }
}
