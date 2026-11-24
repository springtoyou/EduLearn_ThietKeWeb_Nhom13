
namespace EduLearn.Models
{
    public class ReleaseSchedule
    {
        public string ScheduleId { get; set; }
        public string CourseId { get; set; }
        public string WeekId { get; set; }
        public string LessonId { get; set; }
        public string ScheduleType { get; set; }
        public string ReleaseAt { get; set; }
        public bool AutoPublish { get; set; }
        public bool SendNotification { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string ExecutedAt { get; set; }
        public string LastError { get; set; }
    }
}
