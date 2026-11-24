
using System;
using System.Collections.Generic;

namespace EduLearn.Models
{
    public class Lesson
    {
        public string LessonId { get; set; }
        public string WeekId { get; set; }

        public string LessonCode { get; set; }
        public string LessonTitle { get; set; }
        public string ShortDescription { get; set; }

        public List<LessonContentSection> ContentSections { get; set; }

        public string LessonType { get; set; }
        public int DurationMinutes { get; set; }

        public string ReleaseAt { get; set; }
        public string Status { get; set; }

        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedAt { get; set; }

        // Các thuộc tính tương thích với cấu trúc cũ.
        // Không phải trường dữ liệu trong Lessons.xml mới.
        public string CourseId { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string DocumentUrl { get; set; }
        public DateTime ReleaseDate { get; set; }

        public Lesson()
        {
            ContentSections = new List<LessonContentSection>();
        }
    }

    public class LessonContentSection
    {
        public string SectionId { get; set; }
        public string Title { get; set; }
        public string SectionType { get; set; }
        public string Body { get; set; }
        public int DisplayOrder { get; set; }
        public string Language { get; set; }

        public LessonContentSection()
        {
        }
    }
}
