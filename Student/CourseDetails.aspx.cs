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
    public partial class CourseDetails : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Student"))
            {
                return;
            }

            ConfigureMasterControls();
            pnlRegistrationMessage.Visible = false;

            Course course = CourseService.GetCourseById(
                Request.QueryString["courseId"]);
            if (course == null)
            {
                pnlCourseNotFound.Visible = true;
                pnlCourseDetails.Visible = false;
                return;
            }

            pnlCourseNotFound.Visible = false;
            pnlCourseDetails.Visible = true;
            BindCourse(course, AuthSession.GetCurrentUser());
        }

        protected void btnEnroll_Click(object sender, EventArgs e)
        {
            User student = AuthSession.GetCurrentUser();
            string courseId = Request.QueryString["courseId"];

            if (student == null ||
                !string.Equals(
                    student.Role,
                    "Student",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                EnrollmentService.EnrollStudent(student.UserId, courseId);
                string destination = ResolveUrl(
                    "~/Student/CourseLearning.aspx?courseId=" +
                    Server.UrlEncode(courseId));
                Response.Redirect(destination, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (InvalidOperationException exception)
            {
                litRegistrationMessage.Text =
                    Server.HtmlEncode(exception.Message);
                pnlRegistrationMessage.Visible = true;

                Course course = CourseService.GetCourseById(courseId);
                if (course != null)
                {
                    BindCourse(course, student);
                }
            }
        }

        private void BindCourse(Course course, User student)
        {
            List<Week> weeks = WeekService.GetWeeksByCourse(course.CourseId);
            List<Lesson> allLessons = LessonService.GetAllLessons();
            List<Lesson> courseLessons = allLessons
                .Where(x => weeks.Any(week =>
                    string.Equals(
                        week.WeekId,
                        x.WeekId,
                        StringComparison.OrdinalIgnoreCase)))
                .ToList();
            Category category = CategoryService.GetCategoryById(
                course.CategoryId);
            User instructor = UserService.GetAllUsers().FirstOrDefault(x =>
                string.Equals(
                    x.UserId,
                    course.InstructorId,
                    StringComparison.OrdinalIgnoreCase));
            InstructorProfile instructorProfile =
                InstructorProfileService.GetProfileByUserId(
                    course.InstructorId);
            bool isEnrolled = EnrollmentService.HasEnrolled(
                student.UserId,
                course.CourseId);
            bool canEnroll = !isEnrolled &&
                course.EnrollmentOpen &&
                string.Equals(
                    course.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase);

            string courseName = string.IsNullOrWhiteSpace(course.CourseName)
                ? course.CourseCode
                : course.CourseName;
            string term = FormatTerm(
                course.Semester,
                course.AcademicYear);
            int totalLessonMinutes = courseLessons.Sum(
                x => Math.Max(0, x.DurationMinutes));

            litBreadcrumbCourse.Text = Encode(courseName);
            litCategory.Text = Encode(category != null
                ? category.CategoryName
                : "Chưa phân loại");
            litCourseName.Text = Encode(courseName);
            litShortDescription.Text = Encode(
                !string.IsNullOrWhiteSpace(course.ShortDescription)
                    ? course.ShortDescription
                    : course.Description);
            litInstructorName.Text = Encode(instructor != null
                ? instructor.FullName
                : "Chưa cập nhật");
            litWeekCount.Text = FormatCount(weeks.Count, "tuần");
            litLessonCount.Text = FormatCount(courseLessons.Count, "bài giảng");
            litCourseCode.Text = Encode(
                string.IsNullOrWhiteSpace(course.CourseCode)
                    ? course.CourseId
                    : course.CourseCode);
            litLevel.Text = Encode(string.IsNullOrWhiteSpace(course.Level)
                ? "Trình độ chưa cập nhật"
                : course.Level);
            litDescription.Text = Encode(
                !string.IsNullOrWhiteSpace(course.Description)
                    ? course.Description
                    : course.ShortDescription);
            litCurriculumSummary.Text =
                FormatCount(weeks.Count, "tuần học") + " · " +
                FormatCount(courseLessons.Count, "bài giảng") +
                (totalLessonMinutes > 0
                    ? " · " + FormatDuration(totalLessonMinutes)
                    : string.Empty);

            phTermBadge.Visible = !string.IsNullOrWhiteSpace(term);
            litTermBadge.Text = Encode(term);

            string thumbnailPath = ResolveThumbnailUrl(course);
            imgCourseCover.Visible = !string.IsNullOrWhiteSpace(thumbnailPath);
            if (imgCourseCover.Visible)
            {
                imgCourseCover.ImageUrl = ResolveUrl(thumbnailPath);
                imgCourseCover.AlternateText = courseName;
            }

            List<string> objectives = course.LearningObjectives
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList();
            pnlNoObjectives.Visible = objectives.Count == 0;
            rptObjectives.DataSource = objectives;
            rptObjectives.DataBind();

            List<string> prerequisites = course.Prerequisites
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList();
            pnlNoPrerequisites.Visible = prerequisites.Count == 0;
            rptPrerequisites.DataSource = prerequisites;
            rptPrerequisites.DataBind();

            List<CoursePreviewWeekItem> weekItems = weeks
                .Select((week, index) =>
                {
                    List<CoursePreviewLessonItem> lessonItems =
                        allLessons
                            .Where(x => string.Equals(
                                x.WeekId,
                                week.WeekId,
                                StringComparison.OrdinalIgnoreCase))
                            .OrderBy(x => x.DisplayOrder)
                            .Select(x => new CoursePreviewLessonItem
                            {
                                LessonTitle = x.LessonTitle,
                                TypeText = GetLessonTypeText(x.LessonType),
                                TypeIcon = GetLessonTypeIcon(x.LessonType),
                                DurationText = x.DurationMinutes > 0
                                    ? FormatDuration(x.DurationMinutes)
                                    : "Thời lượng chưa cập nhật"
                            })
                            .ToList();

                    string weekStatus = GetWeekStatus(week.Status);
                    return new CoursePreviewWeekItem
                    {
                        WeekNumberText = week.WeekNumber.ToString(
                            "00",
                            CultureInfo.InvariantCulture),
                        WeekTitle = week.WeekTitle,
                        Description = week.Description,
                        StatusText = weekStatus,
                        StatusCss = GetWeekStatusCss(week.Status),
                        LessonSummary = lessonItems.Count == 1
                            ? "1 bài giảng"
                            : lessonItems.Count + " bài giảng",
                        Lessons = lessonItems,
                        HasLessons = lessonItems.Count > 0,
                        IsCurrent = index == 0 &&
                            string.Equals(
                                week.Status,
                                "Available",
                                StringComparison.OrdinalIgnoreCase)
                    };
                })
                .ToList();

            pnlNoWeeks.Visible = weekItems.Count == 0;
            rptWeeks.DataSource = weekItems;
            rptWeeks.DataBind();

            bool enrollmentOpen =
                course.EnrollmentOpen &&
                string.Equals(
                    course.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase);
            pnlCanEnroll.Visible = canEnroll;
            pnlAlreadyEnrolled.Visible = isEnrolled;
            pnlRegistrationClosed.Visible = !isEnrolled && !enrollmentOpen;
            lblEnrollmentStatus.Text = isEnrolled
                ? "Đã đăng ký"
                : enrollmentOpen
                    ? "Chưa đăng ký"
                    : "Đã đóng";
            lblEnrollmentStatus.CssClass = isEnrolled
                ? "course-status-badge is-enrolled"
                : enrollmentOpen
                    ? "course-status-badge is-open"
                    : "course-status-badge is-closed";

            if (isEnrolled)
            {
                lnkCourseLearning.NavigateUrl = ResolveUrl(
                    "~/Student/CourseLearning.aspx?courseId=" +
                    Server.UrlEncode(course.CourseId));
            }

            litAsideWeekCount.Text = FormatCount(weeks.Count, "tuần");
            litAsideLessonCount.Text =
                FormatCount(courseLessons.Count, "bài");
            litCredits.Text = course.Credits > 0
                ? course.Credits.ToString(CultureInfo.InvariantCulture)
                : "Chưa cập nhật";
            litAsideLevel.Text = Encode(string.IsNullOrWhiteSpace(course.Level)
                ? "Chưa cập nhật"
                : course.Level);

            string instructorName = instructor != null &&
                !string.IsNullOrWhiteSpace(instructor.FullName)
                ? instructor.FullName
                : "Giảng viên";
            litInstructorFullName.Text = Encode(
                FormatInstructorName(instructorProfile) + instructorName);
            litInstructorInitials.Text = Encode(GetInitials(instructorName));
            litDepartment.Text = Encode(
                instructorProfile != null &&
                !string.IsNullOrWhiteSpace(instructorProfile.Department)
                    ? instructorProfile.Department
                    : "Giảng viên EduLearn");
            litInstructorBiography.Text = Encode(
                instructorProfile != null &&
                !string.IsNullOrWhiteSpace(instructorProfile.Biography)
                    ? instructorProfile.Biography
                    : "Thông tin giới thiệu chưa được cập nhật.");

            bool hasEmail = instructor != null &&
                !string.IsNullOrWhiteSpace(instructor.Email);
            lnkInstructorEmail.Visible = hasEmail;
            if (hasEmail)
            {
                lnkInstructorEmail.NavigateUrl =
                    "mailto:" + instructor.Email.Trim();
                lnkInstructorEmail.Text =
                    "✉ " + instructor.Email.Trim();
            }
        }

        private void ConfigureMasterControls()
        {
            Literal heading = FindMasterControl("litPageHeading") as Literal;
            if (heading != null)
            {
                heading.Text = "Khám phá môn học";
            }

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

            string fallbackFile = GetFallbackImage(course.CourseId);
            if (fallbackFile == null)
            {
                return string.Empty;
            }

            string fallbackPath = "~/anhimg/" + fallbackFile;
            return File.Exists(Server.MapPath(fallbackPath))
                ? fallbackPath
                : string.Empty;
        }

        private static string GetFallbackImage(string courseId)
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
                ? string.Empty
                : academicYear.Trim();
        }

        private static string FormatInstructorName(
            InstructorProfile profile)
        {
            if (profile == null ||
                string.IsNullOrWhiteSpace(profile.AcademicTitle))
            {
                return string.Empty;
            }

            return profile.AcademicTitle.Trim() + " ";
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "GV";
            }

            string[] words = name.Trim().Split(
                new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 1)
            {
                return words[0].Substring(0, 1).ToUpperInvariant();
            }

            return (
                words[0].Substring(0, 1) +
                words[words.Length - 1].Substring(0, 1)
            ).ToUpperInvariant();
        }

        private static string GetWeekStatus(string status)
        {
            if (string.Equals(
                status,
                "Available",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Đang mở";
            }

            if (string.Equals(
                status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Đã hoàn thành";
            }

            return "Chưa mở";
        }

        private static string GetWeekStatusCss(string status)
        {
            if (string.Equals(
                status,
                "Available",
                StringComparison.OrdinalIgnoreCase))
            {
                return "is-open";
            }

            if (string.Equals(
                status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
            {
                return "is-complete";
            }

            return "is-locked";
        }

        private static string GetLessonTypeText(string type)
        {
            if (string.Equals(
                type,
                "Exercise",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Bài tập thực hành";
            }

            if (string.Equals(
                type,
                "Document",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Tài liệu";
            }

            return "Video bài giảng";
        }

        private static string GetLessonTypeIcon(string type)
        {
            if (string.Equals(
                type,
                "Exercise",
                StringComparison.OrdinalIgnoreCase))
            {
                return "□";
            }

            if (string.Equals(
                type,
                "Document",
                StringComparison.OrdinalIgnoreCase))
            {
                return "▤";
            }

            return "▷";
        }

        private static string FormatCount(int count, string label)
        {
            return count.ToString(CultureInfo.InvariantCulture) + " " + label;
        }

        private static string FormatDuration(int minutes)
        {
            if (minutes < 60)
            {
                return minutes + " phút";
            }

            int hours = minutes / 60;
            int remainingMinutes = minutes % 60;
            return remainingMinutes == 0
                ? hours + " giờ"
                : hours + " giờ " + remainingMinutes + " phút";
        }

        private static string Encode(string value)
        {
            return System.Web.HttpUtility.HtmlEncode(value ?? string.Empty);
        }
    }

    public class CoursePreviewWeekItem
    {
        public string WeekNumberText { get; set; }
        public string WeekTitle { get; set; }
        public string Description { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string LessonSummary { get; set; }
        public List<CoursePreviewLessonItem> Lessons { get; set; }
        public bool HasLessons { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class CoursePreviewLessonItem
    {
        public string LessonTitle { get; set; }
        public string TypeText { get; set; }
        public string TypeIcon { get; set; }
        public string DurationText { get; set; }
    }
}
