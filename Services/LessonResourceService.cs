
using System;
using System.Collections.Generic;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class LessonResourceService
    {
        // Lấy toàn bộ tài nguyên bài giảng.
        public static List<LessonResource> GetAllResources()
        {
            List<LessonResource> resources =
                new List<LessonResource>();

            XmlDocument document =
                XmlDataAccess.LoadXml("LessonResources.xml");

            XmlNodeList nodes =
                document.SelectNodes("/LessonResources/Resource");

            foreach (XmlNode node in nodes)
            {
                resources.Add(MapResource(node));
            }

            resources.Sort((a, b) =>
                a.DisplayOrder.CompareTo(b.DisplayOrder));

            return resources;
        }

        // Tìm tài nguyên theo mã ResourceId.
        public static LessonResource GetResourceById(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                return null;

            foreach (LessonResource resource in GetAllResources())
            {
                if (string.Equals(
                    resource.ResourceId,
                    resourceId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return resource;
                }
            }

            return null;
        }

        // Lấy tài nguyên theo bài giảng.
        public static List<LessonResource> GetResourcesByLesson(
            string lessonId)
        {
            List<LessonResource> result =
                new List<LessonResource>();

            if (string.IsNullOrWhiteSpace(lessonId))
                return result;

            foreach (LessonResource resource in GetAllResources())
            {
                if (string.Equals(
                    resource.LessonId,
                    lessonId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(resource);
                }
            }

            result.Sort((a, b) =>
                a.DisplayOrder.CompareTo(b.DisplayOrder));

            return result;
        }

        // Lọc tài nguyên theo loại.
        // Ví dụ: Video, Exercise, Document.
        public static List<LessonResource> GetResourcesByType(
            string contentItemType)
        {
            List<LessonResource> result =
                new List<LessonResource>();

            if (string.IsNullOrWhiteSpace(contentItemType))
                return result;

            foreach (LessonResource resource in GetAllResources())
            {
                if (string.Equals(
                    resource.ContentItemType,
                    contentItemType.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(resource);
                }
            }

            return result;
        }

        // Lấy tài nguyên theo bài giảng và loại.
        public static List<LessonResource> GetResourcesByLessonAndType(
            string lessonId,
            string contentItemType)
        {
            List<LessonResource> result =
                new List<LessonResource>();

            if (string.IsNullOrWhiteSpace(lessonId) ||
                string.IsNullOrWhiteSpace(contentItemType))
            {
                return result;
            }

            foreach (LessonResource resource in
                GetResourcesByLesson(lessonId))
            {
                if (string.Equals(
                    resource.ContentItemType,
                    contentItemType.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(resource);
                }
            }

            return result;
        }

        // Lấy các tài nguyên được phép tải xuống.
        public static List<LessonResource> GetDownloadableResources()
        {
            List<LessonResource> result =
                new List<LessonResource>();

            foreach (LessonResource resource in GetAllResources())
            {
                if (resource.IsDownloadable)
                    result.Add(resource);
            }

            return result;
        }

        // Đếm tổng số tài nguyên.
        public static int CountAllResources()
        {
            return GetAllResources().Count;
        }

        // Đếm tài nguyên theo loại.
        public static int CountResourcesByType(string contentItemType)
        {
            return GetResourcesByType(contentItemType).Count;
        }

        // Đếm tài nguyên của một bài giảng.
        public static int CountResourcesByLesson(string lessonId)
        {
            return GetResourcesByLesson(lessonId).Count;
        }

        // Kiểm tra tài nguyên có được tải xuống không.
        public static bool IsResourceDownloadable(string resourceId)
        {
            LessonResource resource = GetResourceById(resourceId);

            return resource != null && resource.IsDownloadable;
        }

        // Chuyển dữ liệu XML thành đối tượng LessonResource.
        private static LessonResource MapResource(XmlNode node)
        {
            return new LessonResource
            {
                ResourceId = GetValue(node, "ResourceId"),
                LessonId = GetValue(node, "LessonId"),
                ResourceName = GetValue(node, "ResourceName"),
                ContentItemType = GetValue(node, "ContentItemType"),
                FileFormat = GetValue(node, "FileFormat"),
                ResourceUrl = GetValue(node, "ResourceUrl"),
                FileSizeBytes = GetLongValue(node, "FileSizeBytes"),
                IsDownloadable = GetBoolValue(node, "IsDownloadable"),
                DisplayOrder = GetIntValue(node, "DisplayOrder"),
                UploadedBy = GetValue(node, "UploadedBy"),
                UploadedAt = GetValue(node, "UploadedAt")
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

        // Đọc số nguyên dài.
        private static long GetLongValue(
            XmlNode parent,
            string elementName)
        {
            long value;

            return long.TryParse(
                GetValue(parent, elementName),
                out value) ? value : 0L;
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
