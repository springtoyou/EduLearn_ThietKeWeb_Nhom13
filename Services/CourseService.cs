
using System;
using System.Collections.Generic;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class CourseService
    {
        // Lấy toàn bộ danh sách môn học.
        public static List<Course> GetAllCourses()
        {
            List<Course> courses = new List<Course>();

            XmlDocument document =
                XmlDataAccess.LoadXml("Courses.xml");

            XmlNodeList nodes =
                document.SelectNodes("/Courses/Course");

            foreach (XmlNode node in nodes)
            {
                courses.Add(MapCourse(node));
            }

            return courses;
        }

        // Tìm môn học theo mã.
        public static Course GetCourseById(string courseId)
        {
            if (string.IsNullOrWhiteSpace(courseId))
                return null;

            List<Course> courses = GetAllCourses();

            foreach (Course course in courses)
            {
                if (string.Equals(
                    course.CourseId,
                    courseId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return course;
                }
            }

            return null;
        }

        // Tìm môn học theo mã môn học.
        public static Course GetCourseByCode(string courseCode)
        {
            if (string.IsNullOrWhiteSpace(courseCode))
                return null;

            List<Course> courses = GetAllCourses();

            foreach (Course course in courses)
            {
                if (string.Equals(
                    course.CourseCode,
                    courseCode.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return course;
                }
            }

            return null;
        }

        // Lấy danh sách môn học theo giảng viên.
        public static List<Course> GetCoursesByInstructor(
            string instructorId)
        {
            List<Course> result = new List<Course>();

            if (string.IsNullOrWhiteSpace(instructorId))
                return result;

            List<Course> courses = GetAllCourses();

            foreach (Course course in courses)
            {
                if (string.Equals(
                    course.InstructorId,
                    instructorId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(course);
                }
            }

            return result;
        }

        // Lấy danh sách môn học theo danh mục.
        public static List<Course> GetCoursesByCategory(
            string categoryId)
        {
            List<Course> result = new List<Course>();

            if (string.IsNullOrWhiteSpace(categoryId))
                return result;

            List<Course> courses = GetAllCourses();

            foreach (Course course in courses)
            {
                if (string.Equals(
                    course.CategoryId,
                    categoryId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(course);
                }
            }

            return result;
        }

        // Lọc môn học theo trạng thái.
        public static List<Course> GetCoursesByStatus(string status)
        {
            List<Course> result = new List<Course>();

            if (string.IsNullOrWhiteSpace(status))
                return result;

            List<Course> courses = GetAllCourses();

            foreach (Course course in courses)
            {
                if (string.Equals(
                    course.Status,
                    status.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(course);
                }
            }

            return result;
        }

        // Đếm tổng số môn học.
        public static int CountAllCourses()
        {
            return GetAllCourses().Count;
        }

        // Đếm môn học theo trạng thái.
        public static int CountCoursesByStatus(string status)
        {
            return GetCoursesByStatus(status).Count;
        }

        // Đếm môn học do một giảng viên phụ trách.
        public static int CountCoursesByInstructor(
            string instructorId)
        {
            return GetCoursesByInstructor(instructorId).Count;
        }

        // Chuyển dữ liệu XML thành đối tượng Course.
        private static Course MapCourse(XmlNode node)
        {
            Course course = new Course
            {
                CourseId = GetValue(node, "CourseId"),
                CourseCode = GetValue(node, "CourseCode"),
                CourseName = GetValue(node, "CourseName"),
                CategoryId = GetValue(node, "CategoryId"),
                InstructorId = GetValue(node, "InstructorId"),
                ShortDescription = GetValue(node, "ShortDescription"),
                Description = GetValue(node, "Description"),
                Thumbnail = GetValue(node, "Thumbnail"),
                Credits = GetIntValue(node, "Credits"),
                TotalWeeks = GetIntValue(node, "TotalWeeks"),
                Level = GetValue(node, "Level"),
                Semester = GetValue(node, "Semester"),
                AcademicYear = GetValue(node, "AcademicYear"),
                EnrollmentOpen = GetBoolValue(node, "EnrollmentOpen"),
                Status = GetValue(node, "Status"),
                CreatedAt = GetValue(node, "CreatedAt"),
                UpdatedAt = GetValue(node, "UpdatedAt")
            };

            XmlNode objectivesNode = node["LearningObjectives"];

            if (objectivesNode != null)
            {
                foreach (XmlNode objective in objectivesNode.ChildNodes)
                {
                    if (objective.Name == "Objective")
                        course.LearningObjectives.Add(objective.InnerText);
                }
            }

            XmlNode prerequisitesNode = node["Prerequisites"];

            if (prerequisitesNode != null)
            {
                foreach (XmlNode prerequisite in prerequisitesNode.ChildNodes)
                {
                    if (prerequisite.Name == "Prerequisite")
                        course.Prerequisites.Add(prerequisite.InnerText);
                }
            }

            return course;
        }

        // Đọc nội dung XML dạng chuỗi.
        private static string GetValue(
            XmlNode parent,
            string elementName)
        {
            XmlNode node = parent[elementName];

            return node == null
                ? string.Empty
                : node.InnerText.Trim();
        }

        // Đọc số nguyên.
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
