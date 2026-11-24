
using System;
using System.Collections.Generic;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class InstructorProfileService
    {
        // Lấy toàn bộ hồ sơ giảng viên.
        public static List<InstructorProfile> GetAllProfiles()
        {
            List<InstructorProfile> profiles =
                new List<InstructorProfile>();

            XmlDocument document =
                XmlDataAccess.LoadXml("InstructorProfiles.xml");

            XmlNodeList nodes =
                document.SelectNodes(
                    "/InstructorProfiles/InstructorProfile");

            foreach (XmlNode node in nodes)
            {
                profiles.Add(MapProfile(node));
            }

            return profiles;
        }

        // Tìm hồ sơ theo mã hồ sơ.
        public static InstructorProfile GetProfileById(
            string instructorProfileId)
        {
            if (string.IsNullOrWhiteSpace(instructorProfileId))
                return null;

            foreach (InstructorProfile profile in GetAllProfiles())
            {
                if (string.Equals(
                    profile.InstructorProfileId,
                    instructorProfileId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return profile;
                }
            }

            return null;
        }

        // Tìm hồ sơ theo mã tài khoản giảng viên.
        public static InstructorProfile GetProfileByUserId(
            string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            foreach (InstructorProfile profile in GetAllProfiles())
            {
                if (string.Equals(
                    profile.UserId,
                    userId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return profile;
                }
            }

            return null;
        }

        // Kiểm tra giảng viên đã có hồ sơ hay chưa.
        public static bool HasProfile(string userId)
        {
            return GetProfileByUserId(userId) != null;
        }

        // Đếm tổng số hồ sơ giảng viên.
        public static int CountAllProfiles()
        {
            return GetAllProfiles().Count;
        }

        // Chuyển dữ liệu XML thành đối tượng InstructorProfile.
        private static InstructorProfile MapProfile(XmlNode node)
        {
            return new InstructorProfile
            {
                InstructorProfileId =
                    GetValue(node, "InstructorProfileId"),

                UserId =
                    GetValue(node, "UserId"),

                Department =
                    GetValue(node, "Department"),

                Specialization =
                    GetValue(node, "Specialization"),

                AcademicTitle =
                    GetValue(node, "AcademicTitle"),

                ExperienceYears =
                    GetIntValue(node, "ExperienceYears"),

                Biography =
                    GetValue(node, "Biography"),

                Office =
                    GetValue(node, "Office"),

                UpdatedAt =
                    GetValue(node, "UpdatedAt")
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

        // Đọc giá trị XML dạng số nguyên.
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