
using System.Web;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class AuthSession
    {
        private const string CurrentUserKey = "EduLearn_CurrentUser";

        // Lưu người dùng vào Session
        public static void SetCurrentUser(User user)
        {
            if (HttpContext.Current == null || user == null)
            {
                return;
            }

            HttpContext.Current.Session[CurrentUserKey] = user;
        }

        // Lấy thông tin người đang đăng nhập
        public static User GetCurrentUser()
        {
            if (HttpContext.Current == null ||
                HttpContext.Current.Session == null)
            {
                return null;
            }

            return HttpContext.Current.Session[CurrentUserKey] as User;
        }

        // Kiểm tra đã đăng nhập chưa
        public static bool IsLoggedIn()
        {
            return GetCurrentUser() != null;
        }

        // Kiểm tra vai trò tài khoản
        public static bool HasRole(string role)
        {
            User user = GetCurrentUser();

            return user != null &&
                   !string.IsNullOrWhiteSpace(role) &&
                   string.Equals(
                       user.Role,
                       role,
                       System.StringComparison.OrdinalIgnoreCase
                   );
        }

        // Đăng xuất
        public static void Logout()
        {
            if (HttpContext.Current == null ||
                HttpContext.Current.Session == null)
            {
                return;
            }

            HttpContext.Current.Session.Remove(CurrentUserKey);
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}
