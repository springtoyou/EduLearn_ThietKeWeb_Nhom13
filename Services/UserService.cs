
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class UserService
    {
        private static readonly object userWriteLock = new object();

        // Lấy toàn bộ danh sách người dùng.
        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            XmlDocument document =
                XmlDataAccess.LoadXml("Users.xml");

            XmlNodeList nodes =
                document.SelectNodes("/Users/User");

            foreach (XmlNode node in nodes)
            {
                users.Add(MapUser(node));
            }

            return users;
        }

        // Tìm người dùng theo UserId.
        public static User GetUserById(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            List<User> users = GetAllUsers();

            foreach (User user in users)
            {
                if (string.Equals(
                    user.UserId,
                    userId.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }

            return null;
        }

        // Tìm người dùng theo email.
        public static User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            List<User> users = GetAllUsers();

            foreach (User user in users)
            {
                if (string.Equals(
                    user.Email,
                    email.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }

            return null;
        }

        public static void UpdateStudentProfile(
            string userId,
            string fullName,
            string phone,
            string gender,
            string dateOfBirth)
        {
            string normalizedUserId = (userId ?? string.Empty).Trim();
            string normalizedName = (fullName ?? string.Empty).Trim();
            string normalizedPhone = (phone ?? string.Empty).Trim();
            string normalizedGender = (gender ?? string.Empty).Trim();
            string normalizedDateOfBirth =
                (dateOfBirth ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(normalizedUserId))
            {
                throw new ArgumentException(
                    "Mã tài khoản không được để trống.",
                    "userId");
            }

            if (string.IsNullOrWhiteSpace(normalizedName) ||
                normalizedName.Length > 100)
            {
                throw new ArgumentException(
                    "Họ và tên phải có từ 1 đến 100 ký tự.",
                    "fullName");
            }

            if (normalizedPhone.Length > 32)
            {
                throw new ArgumentException(
                    "Số điện thoại không được vượt quá 32 ký tự.",
                    "phone");
            }

            int phoneDigits = 0;
            foreach (char character in normalizedPhone)
            {
                if (char.IsDigit(character))
                {
                    phoneDigits++;
                }
                else if (!char.IsWhiteSpace(character) &&
                    "+()-.".IndexOf(character) < 0)
                {
                    throw new ArgumentException(
                        "Số điện thoại chứa ký tự không hợp lệ.",
                        "phone");
                }
            }

            if (normalizedPhone.Length > 0 && phoneDigits < 7)
            {
                throw new ArgumentException(
                    "Số điện thoại cần có ít nhất 7 chữ số.",
                    "phone");
            }

            if (normalizedGender.Length > 0 &&
                normalizedGender != "Nam" &&
                normalizedGender != "Nữ" &&
                normalizedGender != "Khác" &&
                normalizedGender != "Không muốn chia sẻ")
            {
                throw new ArgumentException(
                    "Giá trị giới tính không hợp lệ.",
                    "gender");
            }

            if (normalizedDateOfBirth.Length > 0)
            {
                DateTime parsedDateOfBirth;
                if (!DateTime.TryParseExact(
                        normalizedDateOfBirth,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out parsedDateOfBirth) ||
                    parsedDateOfBirth.Date > DateTime.Today)
                {
                    throw new ArgumentException(
                        "Ngày sinh không hợp lệ.",
                        "dateOfBirth");
                }
            }

            lock (userWriteLock)
            {
                XmlDocument document = XmlDataAccess.LoadXml("Users.xml");
                XmlNodeList users = document.SelectNodes("/Users/User");
                XmlNode targetUser = null;

                foreach (XmlNode user in users)
                {
                    if (string.Equals(
                            GetValue(user, "UserId"),
                            normalizedUserId,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        targetUser = user;
                        break;
                    }
                }

                if (targetUser == null)
                {
                    throw new InvalidOperationException(
                        "Không tìm thấy tài khoản cần cập nhật.");
                }

                if (!string.Equals(
                    GetValue(targetUser, "Role"),
                    "Student",
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Chỉ có thể cập nhật hồ sơ của sinh viên.");
                }

                SetValue(targetUser, "FullName", normalizedName);
                SetValue(targetUser, "Phone", normalizedPhone);
                SetValue(targetUser, "Gender", normalizedGender);
                SetValue(
                    targetUser,
                    "DateOfBirth",
                    normalizedDateOfBirth);
                SetValue(
                    targetUser,
                    "UpdatedAt",
                    DateTime.Now.ToString(
                        "yyyy-MM-dd'T'HH:mm:ss",
                        CultureInfo.InvariantCulture));

                XmlDataAccess.SaveXml("Users.xml", document);
            }
        }

        // Đếm tổng số người dùng.
        public static int CountAllUsers()
        {
            return GetAllUsers().Count;
        }

        // Đếm người dùng theo vai trò.
        public static int CountUsersByRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return 0;

            int count = 0;
            List<User> users = GetAllUsers();

            foreach (User user in users)
            {
                if (string.Equals(
                    user.Role,
                    role.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            return count;
        }

        // Đếm số tài khoản đang hoạt động.
        public static int CountActiveUsers()
        {
            int count = 0;
            List<User> users = GetAllUsers();

            foreach (User user in users)
            {
                if (string.Equals(
                    user.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            return count;
        }

        // Chuyển dữ liệu XML thành đối tượng User.
        private static User MapUser(XmlNode node)
        {
            return new User
            {
                UserId = GetValue(node, "UserId"),
                Username = GetValue(node, "Username"),
                FullName = GetValue(node, "FullName"),
                Email = GetValue(node, "Email"),

                // Không đưa mật khẩu băm vào danh sách quản lý.
                PasswordHash = null,

                Role = GetValue(node, "Role"),
                Phone = GetValue(node, "Phone"),
                Avatar = GetValue(node, "Avatar"),
                Gender = GetValue(node, "Gender"),
                DateOfBirth = GetValue(node, "DateOfBirth"),
                Status = GetValue(node, "Status"),
                CreatedAt = GetValue(node, "CreatedAt"),
                UpdatedAt = GetValue(node, "UpdatedAt"),
                LastLoginAt = GetValue(node, "LastLoginAt")
            };
        }

        // Đọc giá trị của một phần tử XML.
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
    }
}
