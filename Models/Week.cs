
using System.Collections.Generic;

namespace EduLearn.Models
{
    public class Week
    {
        public string WeekId { get; set; }
        public string CourseId { get; set; }
        public int WeekNumber { get; set; }
        public string WeekTitle { get; set; }
        public string Description { get; set; }

        public List<string> LearningObjectives { get; set; }

        public string UnlockAt { get; set; }
        public string Status { get; set; }
        public int DisplayOrder { get; set; }

        public Week()
        {
            LearningObjectives = new List<string>();
        }
    }
}
