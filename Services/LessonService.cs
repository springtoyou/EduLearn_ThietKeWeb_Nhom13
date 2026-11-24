
using System;
using System.Collections.Generic;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class LessonService
    {
        // Lấy toàn bộ danh sách bài giảng.
        public static List<Lesson> GetAllLessons()
        {
            List<Lesson> lessons = new List<Lesson>();

            XmlDocument document =
                XmlDataAccess.LoadXml("Lessons.xml");

            XmlNodeList nodes =
                document.SelectNodes("/Lessons/Lesson");

            foreach (XmlNode node in nodes)
            {
                lessons.Add(MapLesson(node));
            }

            lessons.Sort((a, b) =>
                a.DisplayOrder.CompareTo(b.DisplayOrder));

            return lessons;
        }

        // Tìm bài giảng theo mã LessonId.
        public static Lesson GetLessonById(string lessonId)
        {
            if (string.IsNullOrWhiteSpace(lessonId))
                return null;

            foreach (Lesson lesson in GetAllLessons())
            {
                if (string.Equals(
                    lesson.LessonId,
                    lessonId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return lesson;
                }
            }

            return null;
        }

        // Lấy danh sách bài giảng theo tuần.
        public static List<Lesson> GetLessonsByWeek(string weekId)
        {
            List<Lesson> result = new List<Lesson>();

            if (string.IsNullOrWhiteSpace(weekId))
                return result;

            foreach (Lesson lesson in GetAllLessons())
            {
                if (string.Equals(
                    lesson.WeekId,
                    weekId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(lesson);
                }
            }

            result.Sort((a, b) =>
                a.DisplayOrder.CompareTo(b.DisplayOrder));

            return result;
        }

        // Lấy danh sách bài giảng theo trạng thái.
        public static List<Lesson> GetLessonsByStatus(string status)
        {
            List<Lesson> result = new List<Lesson>();

            if (string.IsNullOrWhiteSpace(status))
                return result;

            foreach (Lesson lesson in GetAllLessons())
            {
                if (string.Equals(
                    lesson.Status,
                    status.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(lesson);
                }
            }

            return result;
        }

        // Lấy danh sách bài giảng theo loại.
        public static List<Lesson> GetLessonsByType(string lessonType)
        {
            List<Lesson> result = new List<Lesson>();

            if (string.IsNullOrWhiteSpace(lessonType))
                return result;

            foreach (Lesson lesson in GetAllLessons())
            {
                if (string.Equals(
                    lesson.LessonType,
                    lessonType.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(lesson);
                }
            }

            return result;
        }

        // Lấy các bài giảng đã xuất bản trong một tuần.
        public static List<Lesson> GetPublishedLessonsByWeek(
            string weekId)
        {
            List<Lesson> result = new List<Lesson>();

            foreach (Lesson lesson in GetLessonsByWeek(weekId))
            {
                if (string.Equals(
                    lesson.Status,
                    "Published",
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(lesson);
                }
            }

            return result;
        }

        // Đếm tổng số bài giảng.
        public static int CountAllLessons()
        {
            return GetAllLessons().Count;
        }

        // Đếm bài giảng theo tuần.
        public static int CountLessonsByWeek(string weekId)
        {
            return GetLessonsByWeek(weekId).Count;
        }

        // Đếm bài giảng theo trạng thái.
        public static int CountLessonsByStatus(string status)
        {
            return GetLessonsByStatus(status).Count;
        }

        // Đếm bài giảng theo loại.
        public static int CountLessonsByType(string lessonType)
        {
            return GetLessonsByType(lessonType).Count;
        }

        // Đếm bài giảng bắt buộc trong một tuần.
        public static int CountRequiredLessonsByWeek(string weekId)
        {
            int count = 0;

            foreach (Lesson lesson in GetLessonsByWeek(weekId))
            {
                if (lesson.IsRequired)
                    count++;
            }

            return count;
        }

        // Chuyển dữ liệu XML thành đối tượng Lesson.
        private static Lesson MapLesson(XmlNode node)
        {
            Lesson lesson = new Lesson
            {
                LessonId = GetValue(node, "LessonId"),
                WeekId = GetValue(node, "WeekId"),
                LessonCode = GetValue(node, "LessonCode"),
                LessonTitle = GetValue(node, "LessonTitle"),
                ShortDescription = GetValue(node, "ShortDescription"),
                LessonType = GetValue(node, "LessonType"),
                DurationMinutes = GetIntValue(node, "DurationMinutes"),
                ReleaseAt = GetValue(node, "ReleaseAt"),
                Status = GetValue(node, "Status"),
                IsRequired = GetBoolValue(node, "IsRequired"),
                DisplayOrder = GetIntValue(node, "DisplayOrder"),
                CreatedBy = GetValue(node, "CreatedBy"),
                CreatedAt = GetValue(node, "CreatedAt"),
                UpdatedBy = GetValue(node, "UpdatedBy"),
                UpdatedAt = GetValue(node, "UpdatedAt"),
                ContentSections = new List<LessonContentSection>()
            };

            XmlNode sectionsNode = node["ContentSections"];

            if (sectionsNode != null)
            {
                foreach (XmlNode sectionNode in sectionsNode.ChildNodes)
                {
                    if (sectionNode.Name != "Section")
                        continue;

                    lesson.ContentSections.Add(
                        new LessonContentSection
                        {
                            SectionId = GetValue(sectionNode, "SectionId"),
                            Title = GetValue(sectionNode, "Title"),
                            SectionType = GetValue(sectionNode, "SectionType"),
                            Body = GetValue(sectionNode, "Body"),
                            DisplayOrder = GetIntValue(sectionNode, "DisplayOrder"),
                            Language = GetValue(sectionNode, "Language")
                        });
                }
            }

            return lesson;
        }

        // Đọc giá trị XML dạng chuỗi.
        private static string GetValue(
            XmlNode parent,
            string elementName)
        {
            XmlNode node = parent[elementName];

            return node == null
                ? string.Empty
                : node.InnerText.Trim();
        }

        // Đọc giá trị số nguyên.
        private static int GetIntValue(
            XmlNode parent,
            string elementName)
        {
            int value;

            return int.TryParse(
                GetValue(parent, elementName),
                out value) ? value : 0;
        }

        // Đọc giá trị true/false.
        private static bool GetBoolValue(
            XmlNode parent,
            string elementName)
        {
            bool value;

            return bool.TryParse(
                GetValue(parent, elementName),
                out value) && value;
        }
    }
}
