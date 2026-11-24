
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class EnrollmentService
    {
        private static readonly object enrollmentWriteLock = new object();

        // Lấy toàn bộ danh sách đăng ký.
        public static List<Enrollment> GetAllEnrollments()
        {
            List<Enrollment> enrollments =
                new List<Enrollment>();

            XmlDocument document =
                XmlDataAccess.LoadXml("Enrollments.xml");

            XmlNodeList nodes =
                document.SelectNodes("/Enrollments/Enrollment");

            foreach (XmlNode node in nodes)
            {
                enrollments.Add(MapEnrollment(node));
            }

            return enrollments;
        }

        // Tìm đăng ký theo mã EnrollmentId.
        public static Enrollment GetEnrollmentById(
            string enrollmentId)
        {
            if (string.IsNullOrWhiteSpace(enrollmentId))
                return null;

            foreach (Enrollment enrollment in GetAllEnrollments())
            {
                if (string.Equals(
                    enrollment.EnrollmentId,
                    enrollmentId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return enrollment;
                }
            }

            return null;
        }

        // Lấy danh sách môn học đã đăng ký của sinh viên.
        public static List<Enrollment> GetEnrollmentsByStudent(
            string studentId)
        {
            List<Enrollment> result = new List<Enrollment>();

            if (string.IsNullOrWhiteSpace(studentId))
                return result;

            foreach (Enrollment enrollment in GetAllEnrollments())
            {
                if (string.Equals(
                    enrollment.StudentId,
                    studentId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(enrollment);
                }
            }

            return result;
        }

        // Lấy danh sách sinh viên đăng ký một môn học.
        public static List<Enrollment> GetEnrollmentsByCourse(
            string courseId)
        {
            List<Enrollment> result = new List<Enrollment>();

            if (string.IsNullOrWhiteSpace(courseId))
                return result;

            foreach (Enrollment enrollment in GetAllEnrollments())
            {
                if (string.Equals(
                    enrollment.CourseId,
                    courseId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(enrollment);
                }
            }

            return result;
        }

        // Đếm tổng số lượt đăng ký.
        public static int CountAllEnrollments()
        {
            return GetAllEnrollments().Count;
        }

        // Đếm lượt đăng ký của một sinh viên.
        public static int CountEnrollmentsByStudent(
            string studentId)
        {
            return GetEnrollmentsByStudent(studentId).Count;
        }

        // Đếm lượt đăng ký của một môn học.
        public static int CountEnrollmentsByCourse(
            string courseId)
        {
            return GetEnrollmentsByCourse(courseId).Count;
        }

        // Đếm đăng ký theo trạng thái.
        public static int CountEnrollmentsByStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return 0;

            int count = 0;

            foreach (Enrollment enrollment in GetAllEnrollments())
            {
                if (string.Equals(
                    enrollment.Status,
                    status.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            return count;
        }

        // Kiểm tra sinh viên đã đăng ký môn học hay chưa.
        public static bool HasEnrolled(
            string studentId,
            string courseId)
        {
            if (string.IsNullOrWhiteSpace(studentId) ||
                string.IsNullOrWhiteSpace(courseId))
            {
                return false;
            }

            foreach (Enrollment enrollment in GetAllEnrollments())
            {
                if (string.Equals(
                        enrollment.StudentId,
                        studentId.Trim(),
                        StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(
                        enrollment.CourseId,
                        courseId.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static void EnrollStudent(string studentId, string courseId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                throw new ArgumentException(
                    "Mã sinh viên không được để trống.",
                    "studentId");
            }

            if (string.IsNullOrWhiteSpace(courseId))
            {
                throw new ArgumentException(
                    "Mã môn học không được để trống.",
                    "courseId");
            }

            Course course = CourseService.GetCourseById(courseId);
            if (course == null ||
                !string.Equals(
                    course.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase) ||
                !course.EnrollmentOpen)
            {
                throw new InvalidOperationException(
                    "Môn học hiện không mở đăng ký.");
            }

            lock (enrollmentWriteLock)
            {
                XmlDocument document = XmlDataAccess.LoadXml("Enrollments.xml");
                XmlNode root = document.SelectSingleNode("/Enrollments");
                if (root == null)
                {
                    throw new XmlException(
                        "Tệp đăng ký không có phần tử Enrollments.");
                }

                XmlNodeList enrollments =
                    document.SelectNodes("/Enrollments/Enrollment");
                int highestId = 0;

                foreach (XmlNode enrollment in enrollments)
                {
                    string existingStudentId = GetValue(enrollment, "StudentId");
                    string existingCourseId = GetValue(enrollment, "CourseId");

                    if (string.Equals(
                            existingStudentId,
                            studentId.Trim(),
                            StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(
                            existingCourseId,
                            courseId.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "Bạn đã đăng ký môn học này.");
                    }

                    string enrollmentId = GetValue(enrollment, "EnrollmentId");
                    int sequence;
                    if (enrollmentId.StartsWith(
                            "EN",
                            StringComparison.OrdinalIgnoreCase) &&
                        int.TryParse(
                            enrollmentId.Substring(2),
                            NumberStyles.None,
                            CultureInfo.InvariantCulture,
                            out sequence))
                    {
                        highestId = Math.Max(highestId, sequence);
                    }
                }

                XmlElement newEnrollment = document.CreateElement("Enrollment");
                AppendElement(
                    document,
                    newEnrollment,
                    "EnrollmentId",
                    "EN" + (highestId + 1).ToString(
                        "D3",
                        CultureInfo.InvariantCulture));
                AppendElement(
                    document,
                    newEnrollment,
                    "StudentId",
                    studentId.Trim());
                AppendElement(
                    document,
                    newEnrollment,
                    "CourseId",
                    courseId.Trim());

                string now = DateTime.Now.ToString(
                    "yyyy-MM-dd'T'HH:mm:ss",
                    CultureInfo.InvariantCulture);
                AppendElement(document, newEnrollment, "EnrollmentDate", now);
                AppendElement(document, newEnrollment, "Status", "Active");
                AppendElement(document, newEnrollment, "CompletedAt", string.Empty);
                AppendElement(document, newEnrollment, "LastAccessAt", string.Empty);

                root.AppendChild(newEnrollment);
                XmlDataAccess.SaveXml("Enrollments.xml", document);
            }
        }

        // Chuyển dữ liệu XML thành đối tượng Enrollment.
        private static Enrollment MapEnrollment(XmlNode node)
        {
            return new Enrollment
            {
                EnrollmentId = GetValue(node, "EnrollmentId"),
                StudentId = GetValue(node, "StudentId"),
                CourseId = GetValue(node, "CourseId"),
                EnrollmentDate = GetValue(node, "EnrollmentDate"),
                Status = GetValue(node, "Status"),
                CompletedAt = GetValue(node, "CompletedAt"),
                LastAccessAt = GetValue(node, "LastAccessAt")
            };
        }

        // Đọc giá trị phần tử XML.
        private static string GetValue(
            XmlNode parent,
            string elementName)
        {
            XmlNode node = parent[elementName];

            return node == null
                ? string.Empty
                : node.InnerText.Trim();
        }

        private static void AppendElement(
            XmlDocument document,
            XmlElement parent,
            string name,
            string value)
        {
            XmlElement element = document.CreateElement(name);
            element.InnerText = value ?? string.Empty;
            parent.AppendChild(element);
        }
    }
}
