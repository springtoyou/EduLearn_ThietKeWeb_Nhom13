
using System;
using System.Web;
using EduLearn.Models;

namespace EduLearn.Services
{
    public static class AuthorizationService
    {
        // Xác định trang chính theo vai trò
        public static string GetDashboardUrl(string role)
        {
            if (string.Equals(
                role,
                "Student",
                StringComparison.OrdinalIgnoreCase))
            {
                return "~/Student/Dashboard.aspx";
            }

            if (string.Equals(
                role,
                "Instructor",
                StringComparison.OrdinalIgnoreCase))
            {
                return "~/Instructor/Dashboard.aspx";
            }

            if (string.Equals(
                role,
                "Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                return "~/Admin/Dashboard.aspx";
            }

            return null;
        }

        // Kiểm tra người dùng có được phép vào trang không
        public static bool RequireRole(params string[] allowedRoles)
        {
            HttpContext context = HttpContext.Current;

            if (context == null)
            {
                return false;
            }

            User user = AuthSession.GetCurrentUser();

            // Chưa đăng nhập
            if (user == null)
            {
                RedirectTo("~/Login.aspx");
                return false;
            }

            // Kiểm tra vai trò được phép
            if (allowedRoles != null)
            {
                foreach (string role in allowedRoles)
                {
                    if (string.Equals(
                        user.Role,
                        role,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            // Đăng nhập rồi nhưng không có quyền:
            // chuyển về khu vực đúng với vai trò của mình.
            string dashboardUrl = GetDashboardUrl(user.Role);

            if (dashboardUrl == null)
            {
                AuthSession.Logout();
                RedirectTo("~/Login.aspx");
                return false;
            }

            RedirectTo(dashboardUrl);
            return false;
        }

        private static void RedirectTo(string virtualPath)
        {
            HttpContext context = HttpContext.Current;

            string url = VirtualPathUtility.ToAbsolute(virtualPath);

            context.Response.Redirect(url, false);
            context.ApplicationInstance.CompleteRequest();
        }
    }
}
