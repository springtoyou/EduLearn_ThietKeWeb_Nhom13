
using System;
using System.Collections.Generic;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class ActivityLogService
    {
        // Lấy toàn bộ nhật ký hoạt động.
        public static List<ActivityLog> GetAllActivities()
        {
            List<ActivityLog> activities =
                new List<ActivityLog>();

            XmlDocument document =
                XmlDataAccess.LoadXml("ActivityLogs.xml");

            XmlNodeList nodes =
                document.SelectNodes("/ActivityLogs/Activity");

            foreach (XmlNode node in nodes)
            {
                activities.Add(MapActivity(node));
            }

            return activities;
        }

        // Tìm nhật ký theo mã hoạt động.
        public static ActivityLog GetActivityById(string activityId)
        {
            if (string.IsNullOrWhiteSpace(activityId))
                return null;

            foreach (ActivityLog activity in GetAllActivities())
            {
                if (string.Equals(
                    activity.ActivityId,
                    activityId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return activity;
                }
            }

            return null;
        }

        // Lấy nhật ký của một người dùng.
        public static List<ActivityLog> GetActivitiesByUser(string userId)
        {
            List<ActivityLog> result = new List<ActivityLog>();

            if (string.IsNullOrWhiteSpace(userId))
                return result;

            foreach (ActivityLog activity in GetAllActivities())
            {
                if (string.Equals(
                    activity.UserId,
                    userId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(activity);
                }
            }

            return result;
        }

        // Lấy nhật ký theo loại hành động.
        public static List<ActivityLog> GetActivitiesByActionType(
            string actionType)
        {
            List<ActivityLog> result = new List<ActivityLog>();

            if (string.IsNullOrWhiteSpace(actionType))
                return result;

            foreach (ActivityLog activity in GetAllActivities())
            {
                if (string.Equals(
                    activity.ActionType,
                    actionType.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(activity);
                }
            }

            return result;
        }

        // Lấy nhật ký theo loại đối tượng.
        public static List<ActivityLog> GetActivitiesByEntityType(
            string entityType)
        {
            List<ActivityLog> result = new List<ActivityLog>();

            if (string.IsNullOrWhiteSpace(entityType))
                return result;

            foreach (ActivityLog activity in GetAllActivities())
            {
                if (string.Equals(
                    activity.EntityType,
                    entityType.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(activity);
                }
            }

            return result;
        }

        // Đếm tổng số hoạt động.
        public static int CountAllActivities()
        {
            return GetAllActivities().Count;
        }

        // Đếm hoạt động của một người dùng.
        public static int CountActivitiesByUser(string userId)
        {
            return GetActivitiesByUser(userId).Count;
        }

        // Chuyển dữ liệu XML thành đối tượng ActivityLog.
        private static ActivityLog MapActivity(XmlNode node)
        {
            return new ActivityLog
            {
                ActivityId = GetValue(node, "ActivityId"),
                UserId = GetValue(node, "UserId"),
                ActionType = GetValue(node, "ActionType"),
                EntityType = GetValue(node, "EntityType"),
                EntityId = GetValue(node, "EntityId"),
                Description = GetValue(node, "Description"),
                CreatedAt = GetValue(node, "CreatedAt"),
                IpAddress = GetValue(node, "IpAddress")
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
    }
}
