using System;
using System.Collections.Generic;

namespace Entities.Notifications
{
    public class Notification : BaseEntity
    {
        public string Topic { get; set; }
        public string Text { get; set; }
        public DateTime DateTimes { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public ICollection<User> Users { get; set; }

    }
}
