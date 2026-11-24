
using System;
using System.Security.Cryptography;
using EduLearn.DataAccess;
using EduLearn.Models;
using System.Xml;

namespace EduLearn.Services
{
    public static class AuthService
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        // Tạo chuỗi băm mật khẩu theo PBKDF2.
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Mật khẩu không được để trống.");

            byte[] salt = new byte[SaltSize];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                return Iterations + "." +
                    Convert.ToBase64String(salt) + "." +
                    Convert.ToBase64String(hash);
            }
        }

        // Kiểm tra mật khẩu với chuỗi băm đã lưu.
        public static bool VerifyPassword(
            string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(storedHash))
                return false;

            try
            {
                string[] parts = storedHash.Split('.');

                if (parts.Length != 3)
                    return false;

                int iterations = int.Parse(parts[0]);

                if (iterations < 10000 || iterations > 1000000)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] expectedHash = Convert.FromBase64String(parts[2]);

                if (salt.Length != SaltSize ||
                    expectedHash.Length != HashSize)
                    return false;

                using (var pbkdf2 = new Rfc2898DeriveBytes(
                    password, salt, iterations))
                {
                    byte[] actualHash = pbkdf2.GetBytes(HashSize);

                    int difference = 0;

                    for (int i = 0; i < HashSize; i++)
                    {
                        difference |= actualHash[i] ^ expectedHash[i];
                    }

                    return difference == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // Tìm tài khoản bằng email và xác thực mật khẩu.
        public static User Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
                return null;

            XmlDocument document =
                XmlDataAccess.LoadXml("Users.xml");

            XmlNodeList users =
                document.SelectNodes("/Users/User");

            foreach (XmlNode node in users)
            {
                string savedEmail =
                    node["Email"]?.InnerText.Trim();

                if (!string.Equals(
                    savedEmail,
                    email.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                    continue;

                string status =
                    node["Status"]?.InnerText.Trim();

                if (status != "Active")
                    return null;

                string storedHash =
                    node["PasswordHash"]?.InnerText.Trim();

                if (!VerifyPassword(password, storedHash))
                    return null;

                return new User
                {
                    UserId = node["UserId"]?.InnerText,
                    Username = node["Username"]?.InnerText,
                    FullName = node["FullName"]?.InnerText,
                    Email = savedEmail,
                    PasswordHash = storedHash,
                    Role = node["Role"]?.InnerText,
                    Phone = node["Phone"]?.InnerText,
                    Avatar = node["Avatar"]?.InnerText,
                    Gender = node["Gender"]?.InnerText,
                    DateOfBirth = node["DateOfBirth"]?.InnerText,
                    Status = status,
                    CreatedAt = node["CreatedAt"]?.InnerText,
                    UpdatedAt = node["UpdatedAt"]?.InnerText,
                    LastLoginAt = node["LastLoginAt"]?.InnerText
                };
            }

            // Không tìm thấy tài khoản phù hợp.
            return null;
        }
    }
}
