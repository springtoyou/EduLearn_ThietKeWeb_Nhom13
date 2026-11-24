
namespace EduLearn.Models
{
    public class Enrollment
    {
        public string EnrollmentId { get; set; }
        public string StudentId { get; set; }
        public string CourseId { get; set; }
        public string EnrollmentDate { get; set; }
        public string Status { get; set; }
        public string CompletedAt { get; set; }
        public string LastAccessAt { get; set; }

        public Enrollment()
        {
        }
    }
}
