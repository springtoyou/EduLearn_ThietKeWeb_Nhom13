
using System;
using System.Collections.Generic;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class ReleaseScheduleService
    {
        // Lấy toàn bộ lịch phát hành.
        public static List<ReleaseSchedule> GetAllSchedules()
        {
            List<ReleaseSchedule> schedules =
                new List<ReleaseSchedule>();

            XmlDocument document =
                XmlDataAccess.LoadXml("ReleaseSchedules.xml");

            XmlNodeList nodes =
                document.SelectNodes("/ReleaseSchedules/Schedule");

            foreach (XmlNode node in nodes)
            {
                schedules.Add(MapSchedule(node));
            }

            return schedules;
        }

        // Tìm lịch phát hành theo mã.
        public static ReleaseSchedule GetScheduleById(string scheduleId)
        {
            if (string.IsNullOrWhiteSpace(scheduleId))
                return null;

            foreach (ReleaseSchedule schedule in GetAllSchedules())
            {
                if (string.Equals(
                    schedule.ScheduleId,
                    scheduleId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return schedule;
                }
            }

            return null;
        }

        // Lấy lịch phát hành của một khóa học.
        public static List<ReleaseSchedule> GetSchedulesByCourse(
            string courseId)
        {
            List<ReleaseSchedule> result =
                new List<ReleaseSchedule>();

            if (string.IsNullOrWhiteSpace(courseId))
                return result;

            foreach (ReleaseSchedule schedule in GetAllSchedules())
            {
                if (string.Equals(
                    schedule.CourseId,
                    courseId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(schedule);
                }
            }

            return result;
        }

        // Lấy lịch phát hành theo trạng thái.
        public static List<ReleaseSchedule> GetSchedulesByStatus(
            string status)
        {
            List<ReleaseSchedule> result =
                new List<ReleaseSchedule>();

            if (string.IsNullOrWhiteSpace(status))
                return result;

            foreach (ReleaseSchedule schedule in GetAllSchedules())
            {
                if (string.Equals(
                    schedule.Status,
                    status.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(schedule);
                }
            }

            return result;
        }

        // Lấy các lịch đang chờ thực hiện.
        public static List<ReleaseSchedule> GetPendingSchedules()
        {
            return GetSchedulesByStatus("Pending");
        }

        // Lấy lịch phát hành theo tuần.
        public static List<ReleaseSchedule> GetSchedulesByWeek(
            string weekId)
        {
            List<ReleaseSchedule> result =
                new List<ReleaseSchedule>();

            if (string.IsNullOrWhiteSpace(weekId))
                return result;

            foreach (ReleaseSchedule schedule in GetAllSchedules())
            {
                if (string.Equals(
                    schedule.WeekId,
                    weekId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(schedule);
                }
            }

            return result;
        }

        // Lấy lịch phát hành theo bài học.
        public static List<ReleaseSchedule> GetSchedulesByLesson(
            string lessonId)
        {
            List<ReleaseSchedule> result =
                new List<ReleaseSchedule>();

            if (string.IsNullOrWhiteSpace(lessonId))
                return result;

            foreach (ReleaseSchedule schedule in GetAllSchedules())
            {
                if (string.Equals(
                    schedule.LessonId,
                    lessonId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(schedule);
                }
            }

            return result;
        }

        // Đếm tổng số lịch phát hành.
        public static int CountAllSchedules()
        {
            return GetAllSchedules().Count;
        }

        // Đếm lịch theo trạng thái.
        public static int CountSchedulesByStatus(string status)
        {
            return GetSchedulesByStatus(status).Count;
        }

        // Đếm lịch đang chờ thực hiện.
        public static int CountPendingSchedules()
        {
            return GetPendingSchedules().Count;
        }

        // Chuyển dữ liệu XML thành đối tượng ReleaseSchedule.
        private static ReleaseSchedule MapSchedule(XmlNode node)
        {
            return new ReleaseSchedule
            {
                ScheduleId = GetValue(node, "ScheduleId"),
                CourseId = GetValue(node, "CourseId"),
                WeekId = GetValue(node, "WeekId"),
                LessonId = GetValue(node, "LessonId"),
                ScheduleType = GetValue(node, "ScheduleType"),
                ReleaseAt = GetValue(node, "ReleaseAt"),
                AutoPublish = GetBoolValue(node, "AutoPublish"),
                SendNotification = GetBoolValue(node, "SendNotification"),
                Status = GetValue(node, "Status"),
                CreatedBy = GetValue(node, "CreatedBy"),
                CreatedAt = GetValue(node, "CreatedAt"),
                ExecutedAt = GetValue(node, "ExecutedAt"),
                LastError = GetValue(node, "LastError")
            };
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

        // Đọc giá trị XML dạng bool.
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
