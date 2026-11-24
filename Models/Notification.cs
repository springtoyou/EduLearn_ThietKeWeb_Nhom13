
using System;

namespace EduLearn.Models
{
    public class Notification
    {
        public string NotificationId { get; set; }
        public string RecipientId { get; set; }
        public string SenderId { get; set; }

        public string NotificationType { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string RelatedCourseId { get; set; }
        public string RelatedLessonId { get; set; }

        public string CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string ReadAt { get; set; }
    }
}
