
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using EduLearn.Models;
using EduLearn.Services;

namespace EduLearn.Student
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User currentUser = AuthSession.GetCurrentUser();

            if (currentUser == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (currentUser.Role != "Student")
            {
                Response.Redirect(
                    AuthorizationService.GetDashboardUrl(currentUser.Role),
                    false);

                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            ConfigureMasterControls();

            if (!IsPostBack)
            {
                LoadDashboard(currentUser);
            }
        }

        // Nạp toàn bộ dữ liệu Dashboard.
        private void LoadDashboard(User user)
        {
            litWelcomeName.Text =
                Server.HtmlEncode(GetFirstName(user.FullName));

            LoadCourseStatistics(user.UserId);
            LoadMyCourses(user.UserId);
            LoadRecentLessons(user.UserId);
            LoadNotifications(user.UserId);
        }

        // Thống kê môn học và bài giảng.
        private void LoadCourseStatistics(string studentId)
        {
            List<Enrollment> enrollments =
                EnrollmentService.GetEnrollmentsByStudent(studentId);

            lblTotalCourses.Text = enrollments.Count.ToString();

            lblActiveCourses.Text = enrollments.Count(
                x => x.Status == "Active").ToString();

            lblCompletedCourses.Text = enrollments.Count(
                x => x.Status == "Completed").ToString();

            List<LessonProgress> progresses =
                LessonProgressService.GetProgressByStudent(studentId);

            lblCompletedLessons.Text = progresses.Count(
                x => x.Status == "Completed").ToString();
        }

        // Hiển thị danh sách môn học đã đăng ký.
        private void LoadMyCourses(string studentId)
        {
            List<Enrollment> enrollments =
                EnrollmentService.GetEnrollmentsByStudent(studentId);

            List<DashboardCourseItem> courses =
                new List<DashboardCourseItem>();

            List<LessonProgress> allProgress =
                LessonProgressService.GetProgressByStudent(studentId);

            List<User> users = UserService.GetAllUsers();
            List<Week> allWeeks = WeekService.GetAllWeeks();
            List<Lesson> allLessons = LessonService.GetAllLessons();

            foreach (Enrollment enrollment in enrollments)
            {
                Course course =
                    CourseService.GetCourseById(enrollment.CourseId);

                if (course == null)
                    continue;

                HashSet<string> weekIds = new HashSet<string>(
                    allWeeks
                        .Where(x => string.Equals(
                            x.CourseId,
                            course.CourseId,
                            StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.WeekId),
                    StringComparer.OrdinalIgnoreCase);

                List<Lesson> lessons = allLessons
                    .Where(x => weekIds.Contains(x.WeekId))
                    .ToList();

                HashSet<string> lessonIds = new HashSet<string>(
                    lessons.Select(x => x.LessonId),
                    StringComparer.OrdinalIgnoreCase);

                List<LessonProgress> courseProgress =
                    allProgress.Where(x =>
                        lessonIds.Contains(x.LessonId)).ToList();

                int totalLessons = lessons.Count;

                int completedLessons = courseProgress.Count(
                    x => x.Status == "Completed");

                int progressPercent = 0;

                if (totalLessons > 0)
                {
                    int progressSum = 0;

                    foreach (Lesson lesson in lessons)
                    {
                        LessonProgress lessonProgress =
                            courseProgress.FirstOrDefault(x =>
                                string.Equals(
                                    x.LessonId,
                                    lesson.LessonId,
                                    StringComparison.OrdinalIgnoreCase));

                        progressSum += GetLessonProgressPercent(lessonProgress);
                    }

                    progressPercent = (int)Math.Round(
                        progressSum / (double)totalLessons,
                        MidpointRounding.AwayFromZero);
                }

                Category category =
                    CategoryService.GetCategoryById(course.CategoryId);

                User instructor = users.FirstOrDefault(x =>
                    string.Equals(
                        x.UserId,
                        course.InstructorId,
                        StringComparison.OrdinalIgnoreCase));

                string thumbnailUrl = ResolveThumbnailUrl(course.Thumbnail);

                courses.Add(new DashboardCourseItem
                {
                    CourseId = course.CourseId,
                    CourseCode = course.CourseCode,
                    CourseName = course.CourseName,
                    InstructorName = instructor != null
                        ? instructor.FullName
                        : "Giảng viên",
                    ShortDescription = course.ShortDescription,
                    Thumbnail = thumbnailUrl,
                    ThumbnailUrl = thumbnailUrl,
                    HasThumbnail = thumbnailUrl.Length > 0,
                    CategoryName = category != null
                        ? category.CategoryName
                        : "Chưa phân loại",
                    TotalLessons = totalLessons,
                    CompletedLessons = completedLessons,
                    ProgressPercent = progressPercent,
                    ProgressText = totalLessons > 0
                        ? progressPercent + "%"
                        : "—",
                    ProgressSummary = totalLessons > 0
                        ? completedLessons + "/" + totalLessons + " bài hoàn thành"
                        : "Chưa có bài giảng"
                });
            }

            pnlNoCourses.Visible = courses.Count == 0;
            rptCourses.DataSource = courses;
            rptCourses.DataBind();
        }

        // Hiển thị những bài giảng được truy cập gần đây.
        private void LoadRecentLessons(string studentId)
        {
            List<LessonProgress> progresses =
                LessonProgressService.GetProgressByStudent(studentId);

            List<Lesson> allLessons =
                LessonService.GetAllLessons();

            List<Course> allCourses =
                CourseService.GetAllCourses();

            var recentLessons = progresses
                .Where(x => !string.IsNullOrWhiteSpace(x.LastAccessAt))
                .OrderByDescending(x => ParseDate(x.LastAccessAt))
                .Take(5)
                .Select(progress =>
                {
                    Lesson lesson = allLessons.FirstOrDefault(
                        x => x.LessonId == progress.LessonId);

                    if (lesson == null)
                        return null;

                    Week week = WeekService.GetWeekById(lesson.WeekId);

                    Course course = week == null
                        ? null
                        : allCourses.FirstOrDefault(
                            x => x.CourseId == week.CourseId);

                    return new
                    {
                        LessonTitle = lesson.LessonTitle,
                        CourseName = course != null
                            ? course.CourseName
                            : "Môn học",
                        StatusText = GetProgressStatus(progress.Status),
                        StatusCss = GetProgressStatusCss(progress.Status)
                    };
                })
                .Where(x => x != null)
                .ToList();

            pnlNoRecentLessons.Visible =
                recentLessons.Count == 0;

            rptRecentLessons.DataSource = recentLessons;
            rptRecentLessons.DataBind();
        }

        // Hiển thị thông báo của sinh viên.
        private void LoadNotifications(string studentId)
        {
            List<Notification> notifications =
                NotificationService.GetNotificationsByRecipient(studentId);

            var recentNotifications = notifications
                .OrderByDescending(x => ParseDate(x.CreatedAt))
                .Take(5)
                .Select(x => new
                {
                    Title = x.Title,
                    Content = x.Content,
                    CreatedAt = FormatDate(x.CreatedAt)
                })
                .ToList();

            pnlNoNotifications.Visible =
                recentNotifications.Count == 0;

            rptNotifications.DataSource = recentNotifications;
            rptNotifications.DataBind();
        }

        private void ConfigureMasterControls()
        {
            Literal heading = FindMasterControl("litPageHeading") as Literal;
            if (heading != null)
            {
                heading.Text = "Trang chủ";
            }

            HyperLink myCourses = FindMasterControl("lnkMyCourses") as HyperLink;
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

            HyperLink explore = FindMasterControl("lnkExplore") as HyperLink;
            if (explore != null)
            {
                explore.NavigateUrl =
                    ResolveUrl("~/Student/ExploreCourses.aspx");
                explore.Visible = true;
            }

            TextBox search = FindMasterControl("txtGlobalSearch") as TextBox;
            if (search != null)
            {
                search.ClientIDMode = ClientIDMode.Static;
                search.Attributes["aria-label"] =
                    "Tìm môn học, bài giảng hoặc thông báo trên trang chủ";
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

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthSession.Logout();
            FormsAuthentication.SignOut();

            Response.Redirect(ResolveUrl("~/Login.aspx"), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // Lấy tên gọi đầu tiên của người dùng.
        private string GetFirstName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "bạn";

            string[] words = fullName.Trim().Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            return words.Length > 0
                ? words[words.Length - 1]
                : "bạn";
        }

        // Điểm phần trăm của một bài: hoàn thành là 100, đang học theo ProgressPercent, còn lại là 0.
        private static int GetLessonProgressPercent(LessonProgress progress)
        {
            if (progress == null)
                return 0;

            if (string.Equals(
                progress.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
            {
                return 100;
            }

            if (string.Equals(
                progress.Status,
                "InProgress",
                StringComparison.OrdinalIgnoreCase))
            {
                if (progress.ProgressPercent < 0)
                    return 0;

                if (progress.ProgressPercent > 100)
                    return 100;

                return progress.ProgressPercent;
            }

            return 0;
        }

        // Chỉ trả về ~/Images/tên-file khi file thật sự có trong thư mục Images.
        private string ResolveThumbnailUrl(string thumbnail)
        {
            if (string.IsNullOrWhiteSpace(thumbnail))
                return string.Empty;

            string fileName = Path.GetFileName(
                thumbnail.Trim().Replace('\\', '/'));

            if (string.IsNullOrWhiteSpace(fileName) ||
                fileName == "." ||
                fileName == "..")
            {
                return string.Empty;
            }

            string virtualPath = "~/Images/" + fileName;
            string physicalPath = Server.MapPath(virtualPath);

            if (!File.Exists(physicalPath))
                return string.Empty;

            return virtualPath;
        }

        // Chuyển trạng thái tiến độ thành tiếng Việt.
        private string GetProgressStatus(string status)
        {
            switch (status)
            {
                case "Completed":
                    return "Đã hoàn thành";

                case "InProgress":
                    return "Đang học";

                default:
                    return "Chưa bắt đầu";
            }
        }

        private string GetProgressStatusCss(string status)
        {
            switch (status)
            {
                case "Completed":
                    return "is-completed";

                case "InProgress":
                    return "is-progress";

                default:
                    return "is-pending";
            }
        }

        // Chuyển chuỗi ngày tháng thành DateTime.
        private DateTime ParseDate(string value)
        {
            DateTime result;

            return DateTime.TryParse(value, out result)
                ? result
                : DateTime.MinValue;
        }

        // Hiển thị ngày tháng theo định dạng Việt Nam.
        private string FormatDate(string value)
        {
            DateTime date = ParseDate(value);

            return date == DateTime.MinValue
                ? value
                : date.ToString("dd/MM/yyyy HH:mm");
        }
    }

    // Đối tượng dữ liệu riêng dùng cho thẻ môn học Dashboard.
    public class DashboardCourseItem
    {
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string InstructorName { get; set; }
        public string ShortDescription { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool HasThumbnail { get; set; }
        public string CategoryName { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public int ProgressPercent { get; set; }
        public string ProgressText { get; set; }
        public string ProgressSummary { get; set; }
    }
}
