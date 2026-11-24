
namespace EduLearn.Models
{
    public class LessonProgress
    {
        public string ProgressId { get; set; }
        public string StudentId { get; set; }
        public string LessonId { get; set; }
        public string Status { get; set; }
        public int ProgressPercent { get; set; }
        public int WatchedSeconds { get; set; }
        public int LastPositionSeconds { get; set; }
        public string FirstAccessAt { get; set; }
        public string LastAccessAt { get; set; }
        public string CompletedAt { get; set; }
    }
}
