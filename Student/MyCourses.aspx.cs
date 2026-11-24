using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using EduLearn.Models;
using EduLearn.Services;

namespace EduLearn.Student
{
    public partial class MyCourses : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Student"))
            {
                return;
            }

            ConfigureMasterControls();

            if (!IsPostBack)
            {
                BindCourses(AuthSession.GetCurrentUser());
            }
        }

        private void BindCourses(User student)
        {
            List<Enrollment> enrollments =
                EnrollmentService.GetEnrollmentsByStudent(student.UserId);
            List<Course> allCourses = CourseService.GetAllCourses();
            List<Category> categories = CategoryService.GetAllCategories();
            List<User> users = UserService.GetAllUsers();
            List<Week> allWeeks = WeekService.GetAllWeeks();
            List<Lesson> allLessons = LessonService.GetAllLessons();
            List<LessonProgress> allProgress =
                LessonProgressService.GetProgressByStudent(student.UserId);

            List<MyCourseItem> items = new List<MyCourseItem>();

            foreach (Enrollment enrollment in enrollments)
            {
                Course course = allCourses.FirstOrDefault(x =>
                    string.Equals(
                        x.CourseId,
                        enrollment.CourseId,
                        StringComparison.OrdinalIgnoreCase));

                if (course == null)
                {
                    continue;
                }

                List<Week> courseWeeks = allWeeks
                    .Where(x => string.Equals(
                        x.CourseId,
                        course.CourseId,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                HashSet<string> weekIds = new HashSet<string>(
                    courseWeeks.Select(x => x.WeekId),
                    StringComparer.OrdinalIgnoreCase);

                List<Lesson> courseLessons = allLessons
                    .Where(x => weekIds.Contains(x.WeekId))
                    .ToList();

                HashSet<string> lessonIds = new HashSet<string>(
                    courseLessons.Select(x => x.LessonId),
                    StringComparer.OrdinalIgnoreCase);

                List<LessonProgress> courseProgress = allProgress
                    .Where(x => lessonIds.Contains(x.LessonId))
                    .ToList();

                int completedLessons = courseProgress.Count(x =>
                    string.Equals(
                        x.Status,
                        "Completed",
                        StringComparison.OrdinalIgnoreCase));

                int progressPercent = CalculateProgress(
                    courseLessons,
                    courseProgress);

                Category category = categories.FirstOrDefault(x =>
                    string.Equals(
                        x.CategoryId,
                        course.CategoryId,
                        StringComparison.OrdinalIgnoreCase));

                User instructor = users.FirstOrDefault(x =>
                    string.Equals(
                        x.UserId,
                        course.InstructorId,
                        StringComparison.OrdinalIgnoreCase));

                string statusKey = GetStatusKey(enrollment.Status);
                DateTime enrollmentDate = ParseDate(enrollment.EnrollmentDate);
                string thumbnailUrl = ResolveThumbnailUrl(course);
                string courseCode = string.IsNullOrWhiteSpace(course.CourseCode)
                    ? course.CourseId
                    : course.CourseCode;

                items.Add(new MyCourseItem
                {
                    CourseId = course.CourseId,
                    CourseCode = courseCode,
                    CourseName = course.CourseName,
                    CategoryName = category != null
                        ? category.CategoryName
                        : "Chưa phân loại",
                    InstructorName = instructor != null &&
                        !string.IsNullOrWhiteSpace(instructor.FullName)
                        ? instructor.FullName
                        : "Giảng viên",
                    Description = !string.IsNullOrWhiteSpace(course.Description)
                        ? course.Description
                        : course.ShortDescription,
                    CreditsText = course.Credits > 0
                        ? course.Credits.ToString(CultureInfo.InvariantCulture)
                        : "—",
                    WeeksText = course.TotalWeeks > 0
                        ? course.TotalWeeks + " tuần"
                        : courseWeeks.Count + " tuần",
                    LevelText = string.IsNullOrWhiteSpace(course.Level)
                        ? "Chưa cập nhật"
                        : course.Level,
                    TermText = FormatTerm(course.Semester, course.AcademicYear),
                    ObjectivesText = string.Join(
                        " • ",
                        course.LearningObjectives
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Select(x => x.Trim())),
                    PrerequisitesText = string.Join(
                        " • ",
                        course.Prerequisites
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Select(x => x.Trim())),
                    StatusKey = statusKey,
                    StatusText = GetStatusText(enrollment.Status),
                    StatusCss = GetStatusCss(statusKey),
                    ProgressPercent = progressPercent,
                    ProgressText = courseLessons.Count > 0
                        ? progressPercent + "%"
                        : "—",
                    LessonSummary = courseLessons.Count > 0
                        ? completedLessons + " / " + courseLessons.Count
                        : "Chưa có bài giảng",
                    EnrollmentDateSort = enrollmentDate.ToString(
                        "yyyy-MM-dd'T'HH:mm:ss",
                        CultureInfo.InvariantCulture),
                    ThumbnailUrl = thumbnailUrl,
                    HasThumbnail = !string.IsNullOrWhiteSpace(thumbnailUrl)
                });
            }

            items = items
                .OrderByDescending(x => x.EnrollmentDateSort)
                .ToList();

            litTotalCourses.Text = items.Count.ToString(
                CultureInfo.InvariantCulture);
            litActiveCourses.Text = items.Count(x => x.StatusKey == "Active")
                .ToString(CultureInfo.InvariantCulture);
            litCompletedCourses.Text = items.Count(
                x => x.StatusKey == "Completed")
                .ToString(CultureInfo.InvariantCulture);
            litCourseCount.Text = items.Count.ToString(
                CultureInfo.InvariantCulture);

            pnlNoCourses.Visible = items.Count == 0;
            rptCourses.DataSource = items;
            rptCourses.DataBind();
        }

        private void ConfigureMasterControls()
        {
            HyperLink dashboard =
                FindMasterControl("lnkDashboard") as HyperLink;
            if (dashboard != null)
            {
                dashboard.NavigateUrl =
                    ResolveUrl("~/Student/Dashboard.aspx");
            }

            HyperLink myCourses =
                FindMasterControl("lnkMyCourses") as HyperLink;
            if (myCourses != null)
            {
                myCourses.NavigateUrl =
                    ResolveUrl("~/Student/MyCourses.aspx");
                myCourses.CssClass = "nav-link active";
            }

            HyperLink notifications =
                FindMasterControl("lnkNotifications") as HyperLink;
            if (notifications != null)
            {
                notifications.NavigateUrl =
                    ResolveUrl("~/Student/Dashboard.aspx#notifications");
            }

            HyperLink explore = FindMasterControl("lnkExplore") as HyperLink;
            if (explore != null)
            {
                explore.NavigateUrl =
                    ResolveUrl("~/Student/ExploreCourses.aspx");
                explore.Visible = true;
            }

            TextBox globalSearch =
                FindMasterControl("txtGlobalSearch") as TextBox;
            if (globalSearch != null)
            {
                globalSearch.ClientIDMode = ClientIDMode.Static;
                globalSearch.Attributes["aria-label"] =
                    "Tìm môn học trong danh sách môn học của bạn";
            }
        }

        private Control FindMasterControl(string id)
        {
            return FindControlRecursive(Master, id);
        }

        private static Control FindControlRecursive(Control root, string id)
        {
            if (root == null)
            {
                return null;
            }

            if (root.ID == id)
            {
                return root;
            }

            foreach (Control child in root.Controls)
            {
                Control match = FindControlRecursive(child, id);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static int CalculateProgress(
            List<Lesson> lessons,
            List<LessonProgress> progressRecords)
        {
            if (lessons.Count == 0)
            {
                return 0;
            }

            int totalPercent = 0;

            foreach (Lesson lesson in lessons)
            {
                LessonProgress progress = progressRecords.FirstOrDefault(x =>
                    string.Equals(
                        x.LessonId,
                        lesson.LessonId,
                        StringComparison.OrdinalIgnoreCase));

                totalPercent += GetLessonProgressPercent(progress);
            }

            return (int)Math.Round(
                totalPercent / (double)lessons.Count,
                MidpointRounding.AwayFromZero);
        }

        private static int GetLessonProgressPercent(LessonProgress progress)
        {
            if (progress == null)
            {
                return 0;
            }

            if (string.Equals(
                progress.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
            {
                return 100;
            }

            if (!string.Equals(
                progress.Status,
                "InProgress",
                StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            return Math.Max(0, Math.Min(100, progress.ProgressPercent));
        }

        private string ResolveThumbnailUrl(Course course)
        {
            string fileName = Path.GetFileName(
                (course.Thumbnail ?? string.Empty).Replace('\\', '/'));

            if (!string.IsNullOrWhiteSpace(fileName) &&
                fileName != "." &&
                fileName != "..")
            {
                string imagesPath = "~/Images/" + fileName;
                if (File.Exists(Server.MapPath(imagesPath)))
                {
                    return imagesPath;
                }

                string assetsPath = "~/anhimg/" + fileName;
                if (File.Exists(Server.MapPath(assetsPath)))
                {
                    return assetsPath;
                }
            }

            string fallbackFile = GetExistingCourseImage(course.CourseId);
            if (fallbackFile == null)
            {
                return string.Empty;
            }

            string fallbackPath = "~/anhimg/" + fallbackFile;
            return File.Exists(Server.MapPath(fallbackPath))
                ? fallbackPath
                : string.Empty;
        }

        private static string GetExistingCourseImage(string courseId)
        {
            switch ((courseId ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "C001":
                    return "LTW.png";
                case "C002":
                    return "CSDL.png";
                case "C003":
                    return "javanangcao.png";
                default:
                    return null;
            }
        }

        private static string GetStatusKey(string status)
        {
            if (string.Equals(
                status,
                "Active",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Active";
            }

            if (string.Equals(
                status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Completed";
            }

            return "Other";
        }

        private static string GetStatusText(string status)
        {
            string key = GetStatusKey(status);

            if (key == "Active")
            {
                return "Đang học";
            }

            if (key == "Completed")
            {
                return "Đã hoàn thành";
            }

            return string.IsNullOrWhiteSpace(status)
                ? "Chưa cập nhật"
                : status.Trim();
        }

        private static string GetStatusCss(string statusKey)
        {
            if (statusKey == "Active")
            {
                return "is-active";
            }

            if (statusKey == "Completed")
            {
                return "is-completed";
            }

            return "is-other";
        }

        private static string FormatTerm(string semester, string academicYear)
        {
            if (!string.IsNullOrWhiteSpace(semester) &&
                !string.IsNullOrWhiteSpace(academicYear))
            {
                return "Học kỳ " + semester.Trim() + " · " +
                    academicYear.Trim();
            }

            if (!string.IsNullOrWhiteSpace(semester))
            {
                return "Học kỳ " + semester.Trim();
            }

            return string.IsNullOrWhiteSpace(academicYear)
                ? "Chưa cập nhật"
                : academicYear.Trim();
        }

        private static DateTime ParseDate(string value)
        {
            DateTime result;
            return DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result)
                ? result
                : DateTime.MinValue;
        }
    }

    public class MyCourseItem
    {
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CategoryName { get; set; }
        public string InstructorName { get; set; }
        public string Description { get; set; }
        public string CreditsText { get; set; }
        public string WeeksText { get; set; }
        public string LevelText { get; set; }
        public string TermText { get; set; }
        public string ObjectivesText { get; set; }
        public string PrerequisitesText { get; set; }
        public string StatusKey { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public int ProgressPercent { get; set; }
        public string ProgressText { get; set; }
        public string LessonSummary { get; set; }
        public string EnrollmentDateSort { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool HasThumbnail { get; set; }
    }
}
