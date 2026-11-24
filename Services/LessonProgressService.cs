
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class LessonProgressService
    {
        private static readonly object progressWriteLock = new object();

        // Lấy toàn bộ tiến độ học tập.
        public static List<LessonProgress> GetAllProgress()
        {
            List<LessonProgress> result =
                new List<LessonProgress>();

            XmlDocument document =
                XmlDataAccess.LoadXml("LessonProgress.xml");

            XmlNodeList nodes =
                document.SelectNodes("/LessonProgresses/Progress");

            foreach (XmlNode node in nodes)
            {
                result.Add(MapProgress(node));
            }

            return result;
        }

        // Tìm tiến độ theo mã ProgressId.
        public static LessonProgress GetProgressById(string progressId)
        {
            if (string.IsNullOrWhiteSpace(progressId))
                return null;

            foreach (LessonProgress progress in GetAllProgress())
            {
                if (string.Equals(
                    progress.ProgressId,
                    progressId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return progress;
                }
            }

            return null;
        }

        // Lấy tiến độ học tập của một sinh viên.
        public static List<LessonProgress> GetProgressByStudent(
            string studentId)
        {
            List<LessonProgress> result =
                new List<LessonProgress>();

            if (string.IsNullOrWhiteSpace(studentId))
                return result;

            foreach (LessonProgress progress in GetAllProgress())
            {
                if (string.Equals(
                    progress.StudentId,
                    studentId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(progress);
                }
            }

            return result;
        }

        // Lấy tiến độ của một bài giảng.
        public static List<LessonProgress> GetProgressByLesson(
            string lessonId)
        {
            List<LessonProgress> result =
                new List<LessonProgress>();

            if (string.IsNullOrWhiteSpace(lessonId))
                return result;

            foreach (LessonProgress progress in GetAllProgress())
            {
                if (string.Equals(
                    progress.LessonId,
                    lessonId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(progress);
                }
            }

            return result;
        }

        // Lấy tiến độ của sinh viên trong một bài giảng.
        public static LessonProgress GetStudentLessonProgress(
            string studentId,
            string lessonId)
        {
            if (string.IsNullOrWhiteSpace(studentId) ||
                string.IsNullOrWhiteSpace(lessonId))
            {
                return null;
            }

            foreach (LessonProgress progress in
                GetProgressByStudent(studentId))
            {
                if (string.Equals(
                    progress.LessonId,
                    lessonId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return progress;
                }
            }

            return null;
        }

        // Kiểm tra sinh viên đã hoàn thành bài giảng chưa.
        public static bool IsLessonCompleted(
            string studentId,
            string lessonId)
        {
            LessonProgress progress =
                GetStudentLessonProgress(studentId, lessonId);

            return progress != null &&
                   string.Equals(
                       progress.Status,
                       "Completed",
                       StringComparison.OrdinalIgnoreCase);
        }

        public static bool MarkLessonCompleted(
            string studentId,
            string lessonId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                throw new ArgumentException(
                    "Mã sinh viên không được để trống.",
                    "studentId");
            }

            if (string.IsNullOrWhiteSpace(lessonId))
            {
                throw new ArgumentException(
                    "Mã bài giảng không được để trống.",
                    "lessonId");
            }

            lock (progressWriteLock)
            {
                XmlDocument document =
                    XmlDataAccess.LoadXml("LessonProgress.xml");
                XmlNode root =
                    document.SelectSingleNode("/LessonProgresses");
                if (root == null)
                {
                    throw new XmlException(
                        "Tệp tiến độ không có phần tử LessonProgresses.");
                }

                XmlNodeList progressNodes =
                    document.SelectNodes("/LessonProgresses/Progress");
                XmlNode existing = null;
                int highestId = 0;

                foreach (XmlNode node in progressNodes)
                {
                    if (string.Equals(
                            GetValue(node, "StudentId"),
                            studentId.Trim(),
                            StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(
                            GetValue(node, "LessonId"),
                            lessonId.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        existing = node;
                    }

                    string progressId = GetValue(node, "ProgressId");
                    int sequence;
                    if (progressId.StartsWith(
                            "PR",
                            StringComparison.OrdinalIgnoreCase) &&
                        int.TryParse(
                            progressId.Substring(2),
                            NumberStyles.None,
                            CultureInfo.InvariantCulture,
                            out sequence))
                    {
                        highestId = Math.Max(highestId, sequence);
                    }
                }

                if (existing != null &&
                    string.Equals(
                        GetValue(existing, "Status"),
                        "Completed",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                string now = DateTime.Now.ToString(
                    "yyyy-MM-dd'T'HH:mm:ss",
                    CultureInfo.InvariantCulture);

                if (existing == null)
                {
                    existing = document.CreateElement("Progress");
                    AppendElement(
                        document,
                        existing,
                        "ProgressId",
                        "PR" + (highestId + 1).ToString(
                            "D3",
                            CultureInfo.InvariantCulture));
                    AppendElement(
                        document,
                        existing,
                        "StudentId",
                        studentId.Trim());
                    AppendElement(
                        document,
                        existing,
                        "LessonId",
                        lessonId.Trim());
                    AppendElement(document, existing, "WatchedSeconds", "0");
                    AppendElement(
                        document,
                        existing,
                        "LastPositionSeconds",
                        "0");
                    AppendElement(
                        document,
                        existing,
                        "FirstAccessAt",
                        now);
                    AppendElement(
                        document,
                        existing,
                        "CompletedAt",
                        string.Empty);
                    root.AppendChild(existing);
                }

                SetValue(existing, "Status", "Completed");
                SetValue(existing, "ProgressPercent", "100");
                SetValue(existing, "LastAccessAt", now);
                SetValue(existing, "CompletedAt", now);
                XmlDataAccess.SaveXml("LessonProgress.xml", document);
                return true;
            }
        }

        // Tính tỷ lệ hoàn thành trung bình của sinh viên.
        public static double GetAverageProgressByStudent(
            string studentId)
        {
            List<LessonProgress> progresses =
                GetProgressByStudent(studentId);

            if (progresses.Count == 0)
                return 0;

            double total = 0;

            foreach (LessonProgress progress in progresses)
            {
                total += progress.ProgressPercent;
            }

            return Math.Round(total / progresses.Count, 2);
        }

        // Đếm tiến độ theo trạng thái của sinh viên.
        public static int CountProgressByStudentAndStatus(
            string studentId,
            string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return 0;

            int count = 0;

            foreach (LessonProgress progress in
                GetProgressByStudent(studentId))
            {
                if (string.Equals(
                    progress.Status,
                    status.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            return count;
        }

        // Đếm số bài đã hoàn thành của sinh viên.
        public static int CountCompletedLessons(string studentId)
        {
            return CountProgressByStudentAndStatus(
                studentId, "Completed");
        }

        // Đếm số bài đang học của sinh viên.
        public static int CountInProgressLessons(string studentId)
        {
            return CountProgressByStudentAndStatus(
                studentId, "InProgress");
        }

        // Đếm số bài chưa bắt đầu của sinh viên.
        public static int CountNotStartedLessons(string studentId)
        {
            return CountProgressByStudentAndStatus(
                studentId, "NotStarted");
        }

        // Tính tổng thời gian đã xem của sinh viên (giây).
        public static long GetTotalWatchedSeconds(string studentId)
        {
            long total = 0;

            foreach (LessonProgress progress in
                GetProgressByStudent(studentId))
            {
                total += progress.WatchedSeconds;
            }

            return total;
        }

        // Chuyển dữ liệu XML thành đối tượng LessonProgress.
        private static LessonProgress MapProgress(XmlNode node)
        {
            return new LessonProgress
            {
                ProgressId = GetValue(node, "ProgressId"),
                StudentId = GetValue(node, "StudentId"),
                LessonId = GetValue(node, "LessonId"),
                Status = GetValue(node, "Status"),
                ProgressPercent = GetIntValue(node, "ProgressPercent"),
                WatchedSeconds = GetIntValue(node, "WatchedSeconds"),
                LastPositionSeconds = GetIntValue(node, "LastPositionSeconds"),
                FirstAccessAt = GetValue(node, "FirstAccessAt"),
                LastAccessAt = GetValue(node, "LastAccessAt"),
                CompletedAt = GetValue(node, "CompletedAt")
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

        private static void SetValue(
            XmlNode parent,
            string elementName,
            string value)
        {
            XmlNode node = parent[elementName];
            if (node == null)
            {
                node = parent.OwnerDocument.CreateElement(elementName);
                parent.AppendChild(node);
            }

            node.InnerText = value ?? string.Empty;
        }

        private static void AppendElement(
            XmlDocument document,
            XmlNode parent,
            string name,
            string value)
        {
            XmlElement element = document.CreateElement(name);
            element.InnerText = value ?? string.Empty;
            parent.AppendChild(element);
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
