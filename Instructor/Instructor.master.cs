using System;
using System.Web.UI.HtmlControls;
using EduLearn.Instructor.Data;
using EduLearn.Services;

namespace EduLearn.Instructor
{
    public partial class InstructorMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HighlightMenu();

            string key = InstructorStore.CurrentKey();
            GvProfile profile = InstructorStore.GetProfile(key, InstructorStore.CurrentFullName());

            string name = string.IsNullOrWhiteSpace(profile.FullName) ? "Giảng viên" : profile.FullName.Trim();
            string[] parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string initial = parts.Length > 0 ? parts[parts.Length - 1].Substring(0, 1).ToUpperInvariant() : "G";

            litTopName.Text = name;
            litSideName.Text = name;
            litTopInitial.Text = initial;
            litSideInitial.Text = initial;

            // Chấm đỏ khi còn thông báo chưa đọc
            bool hasUnread = InstructorStore.UnreadCount(key) > 0;
            bellDot.Visible = hasUnread;
            sideDot.Visible = hasUnread;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthSession.Logout();

            Response.Redirect(ResolveUrl("~/Login.aspx"), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // Tô sáng mục menu của trang đang mở
        private void HighlightMenu()
        {
            string path = (Request.AppRelativeCurrentExecutionFilePath ?? string.Empty).ToLowerInvariant();

            SetActive(navDashboard, path.EndsWith("/dashboard.aspx"));
            SetActive(navCourses, path.EndsWith("/courses.aspx") || path.EndsWith("/coursemanage.aspx"));
            SetActive(navLectures, path.EndsWith("/lectures.aspx") || path.EndsWith("/lectureedit.aspx"));
            SetActive(navNotifications, path.EndsWith("/notifications.aspx"));
            SetActive(navProfile, path.EndsWith("/profile.aspx"));
        }

        private static void SetActive(HtmlAnchor link, bool active)
        {
            link.Attributes["class"] = active ? "active" : string.Empty;
        }
    }
}
