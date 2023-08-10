using Entities.Notifications;
using WebFramework.Api;

namespace Client.Models.NotificationDto
{
    public class NotificationDto : BaseDto<NotificationDto, Notification, Guid>
    {
        public string Topic { get; set; }
        public string Text { get; set; }
        public DateTime DateTimes { get; set; }
        public Guid UserId { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserFName { get; set; }
        public string UserLName { get; set; }
    }
}
