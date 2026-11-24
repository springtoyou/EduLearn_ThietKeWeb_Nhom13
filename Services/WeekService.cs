
using System;
using System.Collections.Generic;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class WeekService
    {
        // Lấy toàn bộ danh sách tuần học.
        public static List<Week> GetAllWeeks()
        {
            List<Week> weeks = new List<Week>();

            XmlDocument document =
                XmlDataAccess.LoadXml("Weeks.xml");

            XmlNodeList nodes =
                document.SelectNodes("/Weeks/Week");

            foreach (XmlNode node in nodes)
            {
                weeks.Add(MapWeek(node));
            }

            weeks.Sort((a, b) =>
                a.DisplayOrder.CompareTo(b.DisplayOrder));

            return weeks;
        }

        // Tìm tuần học theo mã WeekId.
        public static Week GetWeekById(string weekId)
        {
            if (string.IsNullOrWhiteSpace(weekId))
                return null;

            foreach (Week week in GetAllWeeks())
            {
                if (string.Equals(
                    week.WeekId,
                    weekId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return week;
                }
            }

            return null;
        }

        // Lấy danh sách tuần học của một môn học.
        public static List<Week> GetWeeksByCourse(string courseId)
        {
            List<Week> result = new List<Week>();

            if (string.IsNullOrWhiteSpace(courseId))
                return result;

            foreach (Week week in GetAllWeeks())
            {
                if (string.Equals(
                    week.CourseId,
                    courseId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(week);
                }
            }

            result.Sort((a, b) =>
                a.WeekNumber.CompareTo(b.WeekNumber));

            return result;
        }

        // Lấy một tuần cụ thể trong môn học.
        public static Week GetWeekByCourseAndNumber(
            string courseId,
            int weekNumber)
        {
            if (string.IsNullOrWhiteSpace(courseId) ||
                weekNumber <= 0)
            {
                return null;
            }

            foreach (Week week in GetWeeksByCourse(courseId))
            {
                if (week.WeekNumber == weekNumber)
                    return week;
            }

            return null;
        }

        // Lấy danh sách tuần theo trạng thái.
        public static List<Week> GetWeeksByStatus(string status)
        {
            List<Week> result = new List<Week>();

            if (string.IsNullOrWhiteSpace(status))
                return result;

            foreach (Week week in GetAllWeeks())
            {
                if (string.Equals(
                    week.Status,
                    status.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(week);
                }
            }

            return result;
        }

        // Đếm tổng số tuần học.
        public static int CountAllWeeks()
        {
            return GetAllWeeks().Count;
        }

        // Đếm số tuần của một môn học.
        public static int CountWeeksByCourse(string courseId)
        {
            return GetWeeksByCourse(courseId).Count;
        }

        // Đếm tuần theo trạng thái.
        public static int CountWeeksByStatus(string status)
        {
            return GetWeeksByStatus(status).Count;
        }

        // Đếm số tuần đã hoàn thành của một môn học.
        public static int CountCompletedWeeks(string courseId)
        {
            int count = 0;

            foreach (Week week in GetWeeksByCourse(courseId))
            {
                if (string.Equals(
                    week.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            return count;
        }

        // Đếm số tuần đang khả dụng của một môn học.
        public static int CountAvailableWeeks(string courseId)
        {
            int count = 0;

            foreach (Week week in GetWeeksByCourse(courseId))
            {
                if (string.Equals(
                    week.Status,
                    "Available",
                    StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            return count;
        }

        // Chuyển dữ liệu XML thành đối tượng Week.
        private static Week MapWeek(XmlNode node)
        {
            Week week = new Week
            {
                WeekId = GetValue(node, "WeekId"),
                CourseId = GetValue(node, "CourseId"),
                WeekNumber = GetIntValue(node, "WeekNumber"),
                WeekTitle = GetValue(node, "WeekTitle"),
                Description = GetValue(node, "Description"),
                UnlockAt = GetValue(node, "UnlockAt"),
                Status = GetValue(node, "Status"),
                DisplayOrder = GetIntValue(node, "DisplayOrder")
            };

            XmlNode objectivesNode = node["LearningObjectives"];

            if (objectivesNode != null)
            {
                foreach (XmlNode objective in objectivesNode.ChildNodes)
                {
                    if (objective.Name == "Objective")
                    {
                        week.LearningObjectives.Add(
                            objective.InnerText.Trim());
                    }
                }
            }

            return week;
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
    }
}
