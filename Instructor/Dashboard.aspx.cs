using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EduLearn.Instructor.Data;
using EduLearn.Services;

namespace EduLearn.Instructor
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Instructor")) return;

            if (!IsPostBack)
            {
                string key = InstructorStore.CurrentKey();
                GvProfile profile = InstructorStore.GetProfile(key, InstructorStore.CurrentFullName());

                litName.Text = profile.FullName;

                DateTime now = DateTime.Now;
                string day = now.ToString("dddd, dd/MM/yyyy", new CultureInfo("vi-VN"));
                day = char.ToUpper(day[0]) + day.Substring(1);
                litDateChip.Text = day + " • Học kỳ 1 (" + now.Year + " - " + (now.Year + 1) + ")";

                List<CourseItem> courses = InstructorStore.GetCourses(key);
                int totalLectures = courses.Sum(c => c.LectureCount);
                int published = courses.Sum(c => c.PublishedCount);
                double rate = totalLectures == 0 ? 0 : published * 100.0 / totalLectures;

                litTotalCourses.Text = courses.Count.ToString();
                litTotalStudents.Text = courses.Sum(c => c.StudentCount).ToString();
                litClassCount.Text = courses.Count(c => c.StudentCount > 0).ToString();
                litTotalLectures.Text = totalLectures.ToString();
                litPublished.Text = published.ToString();
                litRate.Text = rate.ToString("0.#", CultureInfo.InvariantCulture);

                List<CourseItem> shown = courses.Take(3).ToList();
                litCourseBadge.Text = courses.Count + " môn học đang phụ trách";
                litCourseShown.Text = "(Hiển thị " + shown.Count + " / " + courses.Count + " môn học)";
                rptCourses.DataSource = shown;
                rptCourses.DataBind();
                pnlNoCourse.Visible = courses.Count == 0;

                List<ActivityItem> recent = InstructorStore.GetRecentActivities(key, 4);
                rptActivities.DataSource = recent;
                rptActivities.DataBind();
                pnlNoActivity.Visible = recent.Count == 0;
            }
        }

        protected string GetCourseBadgeClass(object status)
        {
            string s = Convert.ToString(status) ?? string.Empty;
            return s.Equals("Đang diễn ra", StringComparison.OrdinalIgnoreCase) ? "tag tag--live" : "tag tag--muted";
        }

        protected string GetStatusClass(object status)
        {
            switch ((Convert.ToString(status) ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "bản nháp": return "chip chip--draft";
                case "đã lưu": return "chip chip--saved";
                case "đã công khai": return "chip chip--public";
                case "chính thức": return "chip chip--official";
                default: return "chip chip--draft";
            }
        }

        protected string GetActivityIconClass(object type)
        {
            switch ((Convert.ToString(type) ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "created": return "activity__icon ai-created";
                case "updated": return "activity__icon ai-updated";
                case "published": return "activity__icon ai-published";
                case "assigned": return "activity__icon ai-assigned";
                default: return "activity__icon";
            }
        }

        protected string GetActivityIcon(object type)
        {
            const string open = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\">";
            const string close = "</svg>";
            switch ((Convert.ToString(type) ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "created": return open + "<rect x=\"2\" y=\"6\" width=\"14\" height=\"12\" rx=\"2\"/><path d=\"M22 8l-6 4 6 4z\"/>" + close;
                case "updated": return open + "<polyline points=\"23 4 23 10 17 10\"/><polyline points=\"1 20 1 14 7 14\"/><path d=\"M3.5 9a9 9 0 0 1 14.9-3.4L23 10M1 14l4.6 4.4A9 9 0 0 0 20.5 15\"/>" + close;
                case "published": return open + "<circle cx=\"12\" cy=\"12\" r=\"10\"/><line x1=\"2\" y1=\"12\" x2=\"22\" y2=\"12\"/><path d=\"M12 2a15 15 0 0 1 0 20 15 15 0 0 1 0-20z\"/>" + close;
                default: return open + "<rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\"/><polyline points=\"9 12 11 14 15 10\"/>" + close;
            }
        }
    }
}