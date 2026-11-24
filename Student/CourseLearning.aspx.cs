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
    public partial class CourseLearning : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Student"))
            {
                return;
            }

            User student = AuthSession.GetCurrentUser();
            Course course = CourseService.GetCourseById(
                Request.QueryString["courseId"]);

            ConfigureMasterControls();

            if (course == null)
            {
                pnlCourseNotFound.Visible = true;
                pnlNotEnrolled.Visible = false;
                pnlCourseLearning.Visible = false;
                return;
            }

            pnlCourseNotFound.Visible = false;
            string courseDetailsUrl = ResolveUrl(
                "~/Student/CourseDetails.aspx?courseId=" +
                Server.UrlEncode(course.CourseId));
            lnkCourseDetailsNotEnrolled.NavigateUrl = courseDetailsUrl;
            lnkCourseDetailsHero.NavigateUrl = courseDetailsUrl;

            bool isEnrolled = student != null &&
                EnrollmentService.HasEnrolled(
                    student.UserId,
                    course.CourseId);
            pnlNotEnrolled.Visible = !isEnrolled;
            pnlCourseLearning.Visible = isEnrolled;

            if (!isEnrolled)
            {
                SetActiveMenu("lnkExplore");
                return;
            }

            SetActiveMenu("lnkMyCourses");
            BindCourse(course, student);
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
            List<LessonProgress> progressRecords =
                LessonProgressService.GetProgressByStudent(student.UserId);
            List<LessonResource> allResources =
                LessonResourceService.GetAllResources();
            List<ReleaseSchedule> schedules =
                ReleaseScheduleService.GetSchedulesByCourse(course.CourseId);

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

            List<Lesson> publishedLessons = courseLessons
                .Where(x => string.Equals(
                    x.Status,
                    "Published",
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
            int completedLessons = courseLessons.Count(lesson =>
                IsCompleted(GetProgress(progressRecords, lesson.LessonId)));
            int progressPercent = CalculateCourseProgress(
                courseLessons,
                progressRecords);
            int totalMinutes = courseLessons.Sum(
                x => Math.Max(0, x.DurationMinutes));
            int completedWeeks = weeks.Count(week =>
            {
                List<Lesson> weekLessons = courseLessons.Where(x =>
                    string.Equals(
                        x.WeekId,
                        week.WeekId,
                        StringComparison.OrdinalIgnoreCase)).ToList();
                return string.Equals(
                        week.Status,
                        "Completed",
                        StringComparison.OrdinalIgnoreCase) ||
                    (weekLessons.Count > 0 && weekLessons.All(lesson =>
                        IsCompleted(GetProgress(
                            progressRecords,
                            lesson.LessonId))));
            });

            string courseName = string.IsNullOrWhiteSpace(course.CourseName)
                ? course.CourseCode
                : course.CourseName;
            string term = FormatTerm(
                course.Semester,
                course.AcademicYear);
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
            litDepartment.Text = Encode(
                instructorProfile != null &&
                !string.IsNullOrWhiteSpace(instructorProfile.Department)
                    ? instructorProfile.Department
                    : "Chưa cập nhật");
            litCourseCode.Text = Encode(
                string.IsNullOrWhiteSpace(course.CourseCode)
                    ? course.CourseId
                    : course.CourseCode);
            litEnrollmentStatus.Text = string.Equals(
                EnrollmentService.GetEnrollmentsByStudent(student.UserId)
                    .FirstOrDefault(x => string.Equals(
                        x.CourseId,
                        course.CourseId,
                        StringComparison.OrdinalIgnoreCase))?.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase)
                ? "Đã hoàn thành học phần"
                : "Đã đăng ký học phần";

            phTermBadge.Visible = !string.IsNullOrWhiteSpace(term);
            litTerm.Text = Encode(term);
            litTotalWeeks.Text = weeks.Count.ToString(
                "00",
                CultureInfo.InvariantCulture);
            litTotalDuration.Text = totalMinutes > 0
                ? "Thời lượng: " + FormatDuration(totalMinutes)
                : "Chưa có thời lượng bài giảng";
            litTotalLessons.Text = courseLessons.Count.ToString(
                CultureInfo.InvariantCulture);
            litLessonCompletion.Text = completedLessons + " / " +
                courseLessons.Count + " bài hoàn thành";
            litProgressPercent.Text = progressPercent.ToString(
                CultureInfo.InvariantCulture);
            litProgressSummary.Text = completedLessons + " / " +
                courseLessons.Count + " bài";
            courseProgressTrack.Attributes["aria-valuenow"] =
                progressPercent.ToString(CultureInfo.InvariantCulture);
            courseProgressBar.Style["width"] = progressPercent + "%";

            List<CourseUpdateItem> updates = NotificationService
                .GetNotificationsByRecipient(student.UserId)
                .Where(x => string.Equals(
                    x.RelatedCourseId,
                    course.CourseId,
                    StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => ParseDate(x.CreatedAt))
                .Take(3)
                .Select(x => new CourseUpdateItem
                {
                    Title = x.Title,
                    Content = x.Content,
                    CreatedAtText = FormatDate(x.CreatedAt)
                })
                .ToList();
            pnlUpdates.Visible = updates.Count > 0;
            rptUpdates.DataSource = updates;
            rptUpdates.DataBind();

            Lesson currentLesson = FindCurrentLesson(
                weeks,
                courseLessons,
                progressRecords);
            Week currentWeek = currentLesson == null
                ? FindCurrentWeek(weeks, courseLessons, progressRecords)
                : weeks.FirstOrDefault(x => string.Equals(
                    x.WeekId,
                    currentLesson.WeekId,
                    StringComparison.OrdinalIgnoreCase));
            List<CourseLearningWeekItem> weekItems = weeks
                .Select(week => BuildWeekItem(
                    week,
                    currentWeek,
                    courseLessons,
                    progressRecords,
                    allResources,
                    schedules))
                .ToList();

            litWeeklySummary.Text = completedWeeks + "/" + weeks.Count +
                " tuần đã hoàn thành · " + completedLessons + "/" +
                courseLessons.Count + " bài giảng hoàn thành";
            pnlNoWeeks.Visible = weeks.Count == 0;
            rptWeeks.DataSource = weekItems;
            rptWeeks.DataBind();

            lnkContinueLearning.Text = currentLesson != null
                ? "Tiếp tục bài giảng"
                : currentWeek == null
                    ? "Xem nội dung môn học"
                    : "Tiếp tục học Tuần " +
                    currentWeek.WeekNumber.ToString(
                        CultureInfo.InvariantCulture);
            lnkContinueLearning.NavigateUrl = currentLesson != null
                ? ResolveUrl(
                    "~/Student/LessonDetails.aspx?lessonId=" +
                    Server.UrlEncode(currentLesson.LessonId))
                : currentWeek == null
                    ? "#learning-program"
                    : "#week-" + currentWeek.WeekId;
        }

        private CourseLearningWeekItem BuildWeekItem(
            Week week,
            Week currentWeek,
            List<Lesson> courseLessons,
            List<LessonProgress> progressRecords,
            List<LessonResource> allResources,
            List<ReleaseSchedule> schedules)
        {
            List<Lesson> weekLessons = courseLessons
                .Where(x => string.Equals(
                    x.WeekId,
                    week.WeekId,
                    StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.DisplayOrder)
                .ToList();
            bool accessible = IsWeekAccessible(week);
            List<Lesson> visibleLessons = accessible
                ? weekLessons.Where(x => string.Equals(
                    x.Status,
                    "Published",
                    StringComparison.OrdinalIgnoreCase)).ToList()
                : new List<Lesson>();
            List<CourseLearningLessonItem> lessonItems = visibleLessons
                .Select(lesson => BuildLessonItem(
                    lesson,
                    progressRecords,
                    allResources))
                .ToList();
            int lessonProgressSum = weekLessons.Sum(lesson =>
                GetLessonProgressPercent(
                    GetProgress(progressRecords, lesson.LessonId)));
            int weekProgress = weekLessons.Count == 0
                ? (string.Equals(
                    week.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase) ? 100 : 0)
                : (int)Math.Round(
                    lessonProgressSum / (double)weekLessons.Count,
                    MidpointRounding.AwayFromZero);

            string weekStatusText = GetWeekStatus(week, schedules);
            string weekStatusCss = GetWeekStatusCss(week.Status);
            bool completed = string.Equals(
                week.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase) ||
                (weekLessons.Count > 0 && weekLessons.All(lesson =>
                    IsCompleted(GetProgress(
                        progressRecords,
                        lesson.LessonId))));

            return new CourseLearningWeekItem
            {
                WeekId = week.WeekId,
                WeekTitle = "Tuần " +
                    week.WeekNumber.ToString(
                        CultureInfo.InvariantCulture) + ": " +
                    week.WeekTitle,
                LessonSummary = lessonItems.Count == 1
                    ? "1 bài giảng · " + FormatDuration(
                        SumDuration(weekLessons))
                    : lessonItems.Count + " bài giảng · " +
                        FormatDuration(SumDuration(weekLessons)),
                ProgressSummary = weekProgress + "% · " +
                    lessonItems.Count(x => x.ProgressPercent == 100) +
                    "/" + lessonItems.Count + " bài",
                ProgressPercent = weekProgress,
                StatusText = completed
                    ? "Đã hoàn thành"
                    : weekStatusText,
                StatusCss = completed
                    ? "is-complete"
                    : weekStatusCss,
                WeekIcon = completed ? "✓" : accessible ? "▷" : "▣",
                WeekIconCss = completed
                    ? "is-complete"
                    : accessible ? "is-open" : "is-locked",
                CurrentCss = currentWeek != null &&
                    string.Equals(
                        currentWeek.WeekId,
                        week.WeekId,
                        StringComparison.OrdinalIgnoreCase)
                    ? "is-current"
                    : string.Empty,
                IsCurrent = currentWeek != null &&
                    string.Equals(
                        currentWeek.WeekId,
                        week.WeekId,
                        StringComparison.OrdinalIgnoreCase),
                IsAccessible = accessible,
                HasLessons = lessonItems.Count > 0,
                Lessons = lessonItems
            };
        }

        private CourseLearningLessonItem BuildLessonItem(
            Lesson lesson,
            List<LessonProgress> progressRecords,
            List<LessonResource> allResources)
        {
            LessonProgress progress = GetProgress(
                progressRecords,
                lesson.LessonId);
            List<CourseLearningSectionItem> sections =
                lesson.ContentSections
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new CourseLearningSectionItem
                    {
                        Title = x.Title,
                        Body = x.Body,
                        IsCode = string.Equals(
                            x.SectionType,
                            "Code",
                            StringComparison.OrdinalIgnoreCase)
                    })
                    .ToList();

            List<CourseLearningResourceItem> resources = allResources
                .Where(x => string.Equals(
                    x.LessonId,
                    lesson.LessonId,
                    StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.DisplayOrder)
                .Select(x =>
                {
                    string url = ResolveResourceUrl(x.ResourceUrl);
                    return string.IsNullOrWhiteSpace(url)
                        ? null
                        : new CourseLearningResourceItem
                        {
                            Name = x.ResourceName,
                            Url = url
                        };
                })
                .Where(x => x != null)
                .ToList();

            int progressPercent = GetLessonProgressPercent(progress);
            bool completed = IsCompleted(progress);
            bool inProgress = progress != null &&
                string.Equals(
                    progress.Status,
                    "InProgress",
                    StringComparison.OrdinalIgnoreCase);

            return new CourseLearningLessonItem
            {
                LessonId = lesson.LessonId,
                LessonTitle = lesson.LessonTitle,
                ShortDescription = string.IsNullOrWhiteSpace(
                    lesson.ShortDescription)
                    ? "Bài giảng chưa có mô tả."
                    : lesson.ShortDescription,
                LessonTypeText = GetLessonTypeText(lesson.LessonType),
                DurationText = lesson.DurationMinutes > 0
                    ? FormatDuration(lesson.DurationMinutes)
                    : "Thời lượng chưa cập nhật",
                ProgressPercent = progressPercent,
                ProgressText = completed
                    ? "Đã hoàn thành"
                    : inProgress
                        ? "Đang học · " + progressPercent + "%"
                        : "Chưa bắt đầu",
                ProgressCss = completed
                    ? string.Empty
                    : inProgress ? "is-progress" : "is-not-started",
                ProgressIconCss = completed ? "is-complete" : string.Empty,
                LessonIcon = completed
                    ? "✓"
                    : GetLessonTypeIcon(lesson.LessonType),
                IsRequired = lesson.IsRequired,
                HasSections = sections.Count > 0,
                HasResources = resources.Count > 0,
                Sections = sections,
                Resources = resources
            };
        }

        private Week FindCurrentWeek(
            List<Week> weeks,
            List<Lesson> courseLessons,
            List<LessonProgress> progressRecords)
        {
            foreach (Week week in weeks)
            {
                bool hasInProgressLesson = courseLessons
                    .Where(x => string.Equals(
                        x.WeekId,
                        week.WeekId,
                        StringComparison.OrdinalIgnoreCase))
                    .Any(lesson =>
                    {
                        LessonProgress progress = GetProgress(
                            progressRecords,
                            lesson.LessonId);
                        return progress != null &&
                            string.Equals(
                                progress.Status,
                                "InProgress",
                                StringComparison.OrdinalIgnoreCase);
                    });

                if (hasInProgressLesson && IsWeekAccessible(week))
                {
                    return week;
                }
            }

            return weeks.FirstOrDefault(IsWeekAccessible);
        }

        private Lesson FindCurrentLesson(
            List<Week> weeks,
            List<Lesson> courseLessons,
            List<LessonProgress> progressRecords)
        {
            foreach (Week week in weeks)
            {
                if (!IsWeekAccessible(week))
                {
                    continue;
                }

                Lesson inProgress = courseLessons
                    .Where(x => string.Equals(
                        x.WeekId,
                        week.WeekId,
                        StringComparison.OrdinalIgnoreCase))
                    .Where(x => string.Equals(
                        x.Status,
                        "Published",
                        StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.DisplayOrder)
                    .FirstOrDefault(lesson =>
                    {
                        LessonProgress progress = GetProgress(
                            progressRecords,
                            lesson.LessonId);
                        return progress != null &&
                            string.Equals(
                                progress.Status,
                                "InProgress",
                                StringComparison.OrdinalIgnoreCase);
                    });

                if (inProgress != null)
                {
                    return inProgress;
                }
            }

            return null;
        }

        private string ResolveResourceUrl(string resourceUrl)
        {
            if (string.IsNullOrWhiteSpace(resourceUrl) ||
                !resourceUrl.StartsWith("~/", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            string virtualPath = resourceUrl.Trim();
            string physicalPath = Server.MapPath(virtualPath);
            string applicationPath = Path.GetFullPath(
                Server.MapPath("~/")).TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar) +
                Path.DirectorySeparatorChar;
            string fullPath = Path.GetFullPath(physicalPath);

            if (!fullPath.StartsWith(
                    applicationPath,
                    StringComparison.OrdinalIgnoreCase) ||
                !File.Exists(fullPath))
            {
                return string.Empty;
            }

            return virtualPath;
        }

        private void ConfigureMasterControls()
        {
            Literal heading = FindMasterControl("litPageHeading") as Literal;
            if (heading != null)
            {
                heading.Text = "Bài giảng môn học";
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

            HyperLink explore = FindMasterControl("lnkExplore") as HyperLink;
            if (explore != null)
            {
                explore.NavigateUrl = ResolveUrl("~/Student/ExploreCourses.aspx");
                explore.Visible = true;
            }

            HyperLink notifications =
                FindMasterControl("lnkNotifications") as HyperLink;
            if (notifications != null)
            {
                notifications.NavigateUrl =
                    ResolveUrl("~/Student/Dashboard.aspx#notifications");
            }

            TextBox search =
                FindMasterControl("txtGlobalSearch") as TextBox;
            if (search != null)
            {
                search.ClientIDMode = ClientIDMode.Static;
                search.Attributes["aria-label"] =
                    "Tìm bài giảng trong môn học hiện tại";
            }
        }

        private void SetActiveMenu(string controlId)
        {
            HyperLink dashboard =
                FindMasterControl("lnkDashboard") as HyperLink;
            HyperLink myCourses =
                FindMasterControl("lnkMyCourses") as HyperLink;
            HyperLink explore =
                FindMasterControl("lnkExplore") as HyperLink;

            if (dashboard != null)
            {
                dashboard.CssClass = controlId == "lnkDashboard"
                    ? "nav-link active"
                    : "nav-link";
            }

            if (myCourses != null)
            {
                myCourses.CssClass = controlId == "lnkMyCourses"
                    ? "nav-link active"
                    : "nav-link";
            }

            if (explore != null)
            {
                explore.CssClass = controlId == "lnkExplore"
                    ? "nav-link active"
                    : "nav-link";
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

        private static LessonProgress GetProgress(
            List<LessonProgress> progressRecords,
            string lessonId)
        {
            return progressRecords.FirstOrDefault(x =>
                string.Equals(
                    x.LessonId,
                    lessonId,
                    StringComparison.OrdinalIgnoreCase));
        }

        private static int CalculateCourseProgress(
            List<Lesson> lessons,
            List<LessonProgress> progressRecords)
        {
            if (lessons.Count == 0)
            {
                return 0;
            }

            int progressSum = lessons.Sum(lesson =>
                GetLessonProgressPercent(
                    GetProgress(progressRecords, lesson.LessonId)));

            return (int)Math.Round(
                progressSum / (double)lessons.Count,
                MidpointRounding.AwayFromZero);
        }

        private static int GetLessonProgressPercent(
            LessonProgress progress)
        {
            if (progress == null)
            {
                return 0;
            }

            if (IsCompleted(progress))
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

        private static bool IsCompleted(LessonProgress progress)
        {
            return progress != null && string.Equals(
                progress.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsWeekAccessible(Week week)
        {
            return string.Equals(
                    week.Status,
                    "Available",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    week.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase);
        }

        private static string GetWeekStatus(
            Week week,
            List<ReleaseSchedule> schedules)
        {
            if (string.Equals(
                week.Status,
                "Available",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Đang học";
            }

            if (string.Equals(
                week.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Đã hoàn thành";
            }

            bool hasPendingRelease = schedules.Any(x =>
                string.Equals(
                    x.WeekId,
                    week.WeekId,
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    x.Status,
                    "Pending",
                    StringComparison.OrdinalIgnoreCase));

            return hasPendingRelease ? "Chờ phát hành" : "Chưa mở";
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

            return string.Empty;
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

            return academicYear ?? string.Empty;
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

        private static int SumDuration(List<Lesson> lessons)
        {
            return lessons.Sum(x => Math.Max(0, x.DurationMinutes));
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

        private static string FormatDate(string value)
        {
            DateTime date = ParseDate(value);
            return date == DateTime.MinValue
                ? "Thời gian chưa cập nhật"
                : date.ToString(
                    "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture);
        }

        private static string Encode(string value)
        {
            return System.Web.HttpUtility.HtmlEncode(value ?? string.Empty);
        }
    }

    public class CourseLearningWeekItem
    {
        public string WeekId { get; set; }
        public string WeekTitle { get; set; }
        public string LessonSummary { get; set; }
        public string ProgressSummary { get; set; }
        public int ProgressPercent { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string WeekIcon { get; set; }
        public string WeekIconCss { get; set; }
        public string CurrentCss { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsAccessible { get; set; }
        public bool HasLessons { get; set; }
        public List<CourseLearningLessonItem> Lessons { get; set; }
    }

    public class CourseLearningLessonItem
    {
        public string LessonId { get; set; }
        public string LessonTitle { get; set; }
        public string ShortDescription { get; set; }
        public string LessonTypeText { get; set; }
        public string DurationText { get; set; }
        public int ProgressPercent { get; set; }
        public string ProgressText { get; set; }
        public string ProgressCss { get; set; }
        public string ProgressIconCss { get; set; }
        public string LessonIcon { get; set; }
        public bool IsRequired { get; set; }
        public bool HasSections { get; set; }
        public bool HasResources { get; set; }
        public List<CourseLearningSectionItem> Sections { get; set; }
        public List<CourseLearningResourceItem> Resources { get; set; }
    }

    public class CourseLearningSectionItem
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsCode { get; set; }
    }

    public class CourseLearningResourceItem
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class CourseUpdateItem
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedAtText { get; set; }
    }
}
