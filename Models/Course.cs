
using System.Collections.Generic;

namespace EduLearn.Models
{
    public class Course
    {
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CategoryId { get; set; }
        public string InstructorId { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public int Credits { get; set; }
        public int TotalWeeks { get; set; }

        public List<string> LearningObjectives { get; set; }
        public List<string> Prerequisites { get; set; }

        public string Level { get; set; }
        public string Semester { get; set; }
        public string AcademicYear { get; set; }
        public bool EnrollmentOpen { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

        public Course()
        {
            LearningObjectives = new List<string>();
            Prerequisites = new List<string>();
        }
    }
}
