
using System;
using System.Collections.Generic;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class NotificationService
    {
        // Lấy toàn bộ thông báo.
        public static List<Notification> GetAllNotifications()
        {
            List<Notification> notifications =
                new List<Notification>();

            XmlDocument document =
                XmlDataAccess.LoadXml("Notifications.xml");

            XmlNodeList nodes =
                document.SelectNodes("/Notifications/Notification");

            foreach (XmlNode node in nodes)
            {
                notifications.Add(MapNotification(node));
            }

            return notifications;
        }

        // Tìm thông báo theo mã.
        public static Notification GetNotificationById(
            string notificationId)
        {
            if (string.IsNullOrWhiteSpace(notificationId))
                return null;

            foreach (Notification notification in GetAllNotifications())
            {
                if (string.Equals(
                    notification.NotificationId,
                    notificationId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return notification;
                }
            }

            return null;
        }

        // Lấy thông báo của một người nhận.
        public static List<Notification> GetNotificationsByRecipient(
            string recipientId)
        {
            List<Notification> result = new List<Notification>();

            if (string.IsNullOrWhiteSpace(recipientId))
                return result;

            foreach (Notification notification in GetAllNotifications())
            {
                if (string.Equals(
                    notification.RecipientId,
                    recipientId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(notification);
                }
            }

            return result;
        }

        // Lấy thông báo chưa đọc của một người nhận.
        public static List<Notification> GetUnreadNotifications(
            string recipientId)
        {
            List<Notification> result = new List<Notification>();

            foreach (Notification notification
                in GetNotificationsByRecipient(recipientId))
            {
                if (!notification.IsRead)
                {
                    result.Add(notification);
                }
            }

            return result;
        }

        // Lấy thông báo đã đọc của một người nhận.
        public static List<Notification> GetReadNotifications(
            string recipientId)
        {
            List<Notification> result = new List<Notification>();

            foreach (Notification notification
                in GetNotificationsByRecipient(recipientId))
            {
                if (notification.IsRead)
                {
                    result.Add(notification);
                }
            }

            return result;
        }

        // Lọc thông báo theo loại.
        public static List<Notification> GetNotificationsByType(
            string recipientId,
            string notificationType)
        {
            List<Notification> result = new List<Notification>();

            if (string.IsNullOrWhiteSpace(notificationType))
                return result;

            foreach (Notification notification
                in GetNotificationsByRecipient(recipientId))
            {
                if (string.Equals(
                    notification.NotificationType,
                    notificationType.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(notification);
                }
            }

            return result;
        }

        // Đếm tổng số thông báo của người nhận.
        public static int CountNotifications(string recipientId)
        {
            return GetNotificationsByRecipient(recipientId).Count;
        }

        // Đếm số thông báo chưa đọc.
        public static int CountUnreadNotifications(string recipientId)
        {
            return GetUnreadNotifications(recipientId).Count;
        }

        // Đếm số thông báo đã đọc.
        public static int CountReadNotifications(string recipientId)
        {
            return GetReadNotifications(recipientId).Count;
        }

        // Chuyển dữ liệu XML thành đối tượng Notification.
        private static Notification MapNotification(XmlNode node)
        {
            return new Notification
            {
                NotificationId = GetValue(node, "NotificationId"),
                RecipientId = GetValue(node, "RecipientId"),
                SenderId = GetValue(node, "SenderId"),
                NotificationType = GetValue(node, "NotificationType"),
                Title = GetValue(node, "Title"),
                Content = GetValue(node, "Content"),
                RelatedCourseId = GetValue(node, "RelatedCourseId"),
                RelatedLessonId = GetValue(node, "RelatedLessonId"),
                CreatedAt = GetValue(node, "CreatedAt"),
                IsRead = GetBoolValue(node, "IsRead"),
                ReadAt = GetValue(node, "ReadAt")
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
