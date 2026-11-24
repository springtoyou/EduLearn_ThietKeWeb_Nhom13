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
    public partial class ExploreCourses : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Student"))
            {
                return;
            }

            ConfigureMasterControls();
            BindCourses(AuthSession.GetCurrentUser());
        }

        protected void rptCourses_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (!string.Equals(
                e.CommandName,
                "Enroll",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            User student = AuthSession.GetCurrentUser();
            if (student == null ||
                !string.Equals(
                    student.Role,
                    "Student",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string courseId = Convert.ToString(
                e.CommandArgument,
                CultureInfo.InvariantCulture);

            try
            {
                EnrollmentService.EnrollStudent(student.UserId, courseId);
                Course course = CourseService.GetCourseById(courseId);
                ShowActionMessage(
                    "success",
                    "Đăng ký môn học " +
                    Server.HtmlEncode(course.CourseName) +
                    " thành công.");
            }
            catch (InvalidOperationException exception)
            {
                ShowActionMessage(
                    "error",
                    Server.HtmlEncode(exception.Message));
            }

            BindCourses(student);
        }

        private void BindCourses(User student)
        {
            if (student == null)
            {
                return;
            }

            List<Course> courses = CourseService.GetAllCourses()
                .Where(x => string.Equals(
                    x.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
            List<Category> categories = CategoryService.GetAllCategories();
            List<User> users = UserService.GetAllUsers();
            List<Enrollment> enrollments =
                EnrollmentService.GetEnrollmentsByStudent(student.UserId);
            List<Week> weeks = WeekService.GetAllWeeks();
            List<Lesson> lessons = LessonService.GetAllLessons();

            List<ExploreCourseItem> items = new List<ExploreCourseItem>();

            foreach (Course course in courses)
            {
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
                bool isEnrolled = enrollments.Any(x =>
                    string.Equals(
                        x.CourseId,
                        course.CourseId,
                        StringComparison.OrdinalIgnoreCase));
                List<Week> courseWeeks = weeks.Where(x =>
                    string.Equals(
                        x.CourseId,
                        course.CourseId,
                        StringComparison.OrdinalIgnoreCase)).ToList();
                HashSet<string> weekIds = new HashSet<string>(
                    courseWeeks.Select(x => x.WeekId),
                    StringComparer.OrdinalIgnoreCase);
                int lessonCount = lessons.Count(x => weekIds.Contains(x.WeekId));
                string thumbnailUrl = ResolveThumbnailUrl(course);
                DateTime createdAt = ParseDate(course.CreatedAt);

                items.Add(new ExploreCourseItem
                {
                    CourseId = course.CourseId,
                    CourseCode = string.IsNullOrWhiteSpace(course.CourseCode)
                        ? course.CourseId
                        : course.CourseCode,
                    CourseName = course.CourseName,
                    CategoryId = course.CategoryId,
                    CategoryName = category != null
                        ? category.CategoryName
                        : "Chưa phân loại",
                    InstructorName = instructor != null &&
                        !string.IsNullOrWhiteSpace(instructor.FullName)
                        ? instructor.FullName
                        : "Chưa cập nhật",
                    ShortDescription =
                        !string.IsNullOrWhiteSpace(course.ShortDescription)
                            ? course.ShortDescription
                            : course.Description,
                    Description = !string.IsNullOrWhiteSpace(course.Description)
                        ? course.Description
                        : course.ShortDescription,
                    CreditsText = course.Credits > 0
                        ? course.Credits.ToString(CultureInfo.InvariantCulture)
                        : "Chưa cập nhật",
                    WeeksText = course.TotalWeeks > 0
                        ? course.TotalWeeks + " tuần"
                        : courseWeeks.Count + " tuần",
                    LevelText = string.IsNullOrWhiteSpace(course.Level)
                        ? "Chưa cập nhật"
                        : course.Level,
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
                    LessonCountText = lessonCount == 1
                        ? "1 bài giảng"
                        : lessonCount + " bài giảng",
                    CreatedAtSort = createdAt.ToString(
                        "yyyy-MM-dd'T'HH:mm:ss",
                        CultureInfo.InvariantCulture),
                    ThumbnailUrl = thumbnailUrl,
                    HasThumbnail = !string.IsNullOrWhiteSpace(thumbnailUrl),
                    IsEnrolled = isEnrolled,
                    CanEnroll = !isEnrolled && course.EnrollmentOpen
                });
            }

            List<ExploreCategoryItem> categoryItems = categories
                .Where(x =>
                    string.Equals(
                        x.Status,
                        "Active",
                        StringComparison.OrdinalIgnoreCase) &&
                    items.Any(item => string.Equals(
                        item.CategoryId,
                        x.CategoryId,
                        StringComparison.OrdinalIgnoreCase)))
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new ExploreCategoryItem
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName
                })
                .ToList();

            items = items
                .OrderByDescending(x => x.CreatedAtSort)
                .ToList();

            litCourseCount.Text = items.Count.ToString(
                CultureInfo.InvariantCulture);
            pnlNoCourses.Visible = items.Count == 0;
            rptCategories.DataSource = categoryItems;
            rptCategories.DataBind();
            rptCourses.DataSource = items;
            rptCourses.DataBind();
        }

        private void ConfigureMasterControls()
        {
            HyperLink explore = FindMasterControl("lnkExplore") as HyperLink;
            if (explore != null)
            {
                explore.NavigateUrl = ResolveUrl("~/Student/ExploreCourses.aspx");
                explore.CssClass = "nav-link active";
                explore.Visible = true;
            }

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
            }

            HyperLink notifications =
                FindMasterControl("lnkNotifications") as HyperLink;
            if (notifications != null)
            {
                notifications.NavigateUrl =
                    ResolveUrl("~/Student/Dashboard.aspx#notifications");
            }

            TextBox globalSearch =
                FindMasterControl("txtGlobalSearch") as TextBox;
            if (globalSearch != null)
            {
                globalSearch.ClientIDMode = ClientIDMode.Static;
                globalSearch.Attributes["aria-label"] =
                    "Tìm môn học trong danh sách khám phá";
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

        private void ShowActionMessage(string type, string message)
        {
            pnlActionMessage.CssClass = "course-alert is-" + type;
            litActionMessage.Text = message;
            pnlActionMessage.Visible = true;
        }
    }

    public class ExploreCourseItem
    {
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string InstructorName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string CreditsText { get; set; }
        public string WeeksText { get; set; }
        public string LevelText { get; set; }
        public string ObjectivesText { get; set; }
        public string PrerequisitesText { get; set; }
        public string LessonCountText { get; set; }
        public string CreatedAtSort { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool HasThumbnail { get; set; }
        public bool IsEnrolled { get; set; }
        public bool CanEnroll { get; set; }
    }

    public class ExploreCategoryItem
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
