
using System;
using EduLearn.Models;
using EduLearn.Services;

namespace EduLearn
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User currentUser = AuthSession.GetCurrentUser();

            // Nếu chưa đăng nhập, chuyển về trang đăng nhập.
            if (currentUser == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadUserInformation(currentUser);
                SetActiveMenu();
            }
        }

        // Hiển thị thông tin người dùng.
        private void LoadUserInformation(User user)
        {
            string fullName = string.IsNullOrWhiteSpace(user.FullName)
                ? "Người dùng"
                : user.FullName.Trim();

            string role = user.Role == "Student"
                ? "Sinh viên"
                : user.Role == "Instructor"
                    ? "Giảng viên"
                    : user.Role == "Admin"
                        ? "Quản trị viên"
                        : user.Role;

            lblSidebarName.Text = Server.HtmlEncode(fullName);
            lblHeaderName.Text = Server.HtmlEncode(fullName);
            lblSidebarRole.Text = Server.HtmlEncode(role);

            string initials = GetInitials(fullName);

            lblSidebarInitials.Text = Server.HtmlEncode(initials);
            lblHeaderInitials.Text = Server.HtmlEncode(initials);

            // Hiển thị ảnh đại diện nếu có.
            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                string avatarPath = user.Avatar.Trim();

                if (avatarPath.StartsWith("~/"))
                {
                    avatarPath = ResolveUrl(avatarPath);
                }
                else
                {
                    avatarPath = ResolveUrl(
                        "~/" + avatarPath.TrimStart('/'));
                }

                imgSidebarAvatar.ImageUrl = avatarPath;
                imgHeaderAvatar.ImageUrl = avatarPath;

                imgSidebarAvatar.Visible = true;
                imgHeaderAvatar.Visible = true;

                lblSidebarInitials.Visible = false;
                lblHeaderInitials.Visible = false;
            }
        }

        // Lấy chữ cái đầu của tên người dùng.
        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "U";

            string[] words = fullName.Trim().Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 1)
                return words[0].Substring(0, 1).ToUpper();

            return (
                words[0].Substring(0, 1) +
                words[words.Length - 1].Substring(0, 1)
            ).ToUpper();
        }

        // Đánh dấu menu tương ứng với trang đang mở.
        private void SetActiveMenu()
        {
            string currentPage =
                Request.Url.AbsolutePath.ToLowerInvariant();

            lnkDashboard.CssClass = "nav-link";
            lnkMyCourses.CssClass = "nav-link";
            lnkExplore.CssClass = "nav-link";
            lnkNotifications.CssClass = "nav-link";
            lnkProfile.CssClass = "nav-link";

            if (currentPage.EndsWith("/dashboard.aspx"))
            {
                lnkDashboard.CssClass = "nav-link active";
                litPageHeading.Text = "Tổng quan";
            }
            else if (currentPage.EndsWith("/mycourses.aspx"))
            {
                lnkMyCourses.CssClass = "nav-link active";
                litPageHeading.Text = "Môn học của tôi";
            }
            else if (currentPage.EndsWith("/explorecourses.aspx"))
            {
                lnkExplore.CssClass = "nav-link active";
                litPageHeading.Text = "Khám phá môn học";
            }
            else if (currentPage.EndsWith("/profile.aspx"))
            {
                lnkProfile.CssClass = "nav-link active";
                litPageHeading.Text = "Hồ sơ cá nhân";
            }
        }
    }
}
