using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using EduLearn.Models;
using EduLearn.Services;

namespace EduLearn.Student
{
    public partial class LessonDetails : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Student"))
            {
                return;
            }

            ConfigureMasterControls();
            BindLesson();
        }

        protected void btnMarkComplete_Click(object sender, EventArgs e)
        {
            User student;
            Course course;
            Week week;
            Lesson lesson;
            if (!TryGetAccessibleLesson(
                out student,
                out course,
                out week,
                out lesson))
            {
                BindLesson();
                return;
            }

            bool updated = LessonProgressService.MarkLessonCompleted(
                student.UserId,
                lesson.LessonId);
            BindLesson();
            pnlActionMessage.Visible = true;
            litActionMessage.Text = updated
                ? "Đã ghi nhận bài giảng hoàn thành."
                : "Bài giảng này đã được hoàn thành trước đó.";
        }

        private void BindLesson()
        {
            pnlLessonNotFound.Visible = false;
            pnlLessonAccessDenied.Visible = false;
            pnlLessonView.Visible = false;

            Lesson lesson = LessonService.GetLessonById(
                Request.QueryString["lessonId"]);
            Week week = lesson == null
                ? null
                : WeekService.GetWeekById(lesson.WeekId);
            Course course = week == null
                ? null
                : CourseService.GetCourseById(week.CourseId);

            if (lesson == null || week == null || course == null)
            {
                pnlLessonNotFound.Visible = true;
                return;
            }

            string learningUrl = ResolveUrl(
                "~/Student/CourseLearning.aspx?courseId=" +
                Server.UrlEncode(course.CourseId));
            lnkAccessDestination.NavigateUrl = learningUrl;
            lnkCourseLearning.NavigateUrl = learningUrl;
            lnkBreadcrumbCourses.NavigateUrl = ResolveUrl(
                "~/Student/MyCourses.aspx");
            lnkBreadcrumbCourse.NavigateUrl = learningUrl;

            User student = AuthSession.GetCurrentUser();
            if (student == null ||
                !EnrollmentService.HasEnrolled(
                    student.UserId,
                    course.CourseId))
            {
                pnlLessonAccessDenied.Visible = true;
                litAccessHeading.Text = "Bạn chưa đăng ký môn học này";
                litAccessMessage.Text =
                    "Hãy xem thông tin môn học và trạng thái đăng ký để truy cập bài giảng.";
                lnkAccessDestination.NavigateUrl = ResolveUrl(
                    "~/Student/CourseDetails.aspx?courseId=" +
                    Server.UrlEncode(course.CourseId));
                lnkAccessDestination.Text = "Xem chi tiết môn học";
                return;
            }

            if (!IsWeekAccessible(week) ||
                !string.Equals(
                    lesson.Status,
                    "Published",
                    StringComparison.OrdinalIgnoreCase))
            {
                pnlLessonAccessDenied.Visible = true;
                litAccessHeading.Text = "Bài giảng chưa được mở";
                litAccessMessage.Text =
                    "Nội dung tuần này chưa được phát hành. Bạn vẫn có thể xem tiến độ môn học và các tuần đang mở.";
                lnkAccessDestination.NavigateUrl = learningUrl;
                lnkAccessDestination.Text = "Quay lại nội dung môn học";
                return;
            }

            pnlLessonView.Visible = true;
            SetActiveMenu("lnkMyCourses");
            BindLessonDetails(course, week, lesson, student);
        }

        private void BindLessonDetails(
            Course course,
            Week currentWeek,
            Lesson lesson,
            User student)
        {
            List<Week> weeks = WeekService.GetWeeksByCourse(course.CourseId)
                .OrderBy(x => x.DisplayOrder)
                .ToList();
            List<Lesson> courseLessons = LessonService.GetAllLessons()
                .Where(x => weeks.Any(week => string.Equals(
                    week.WeekId,
                    x.WeekId,
                    StringComparison.OrdinalIgnoreCase)))
                .ToList();
            List<LessonProgress> progressRecords =
                LessonProgressService.GetProgressByStudent(student.UserId);
            List<LessonResource> resources =
                LessonResourceService.GetAllResources()
                    .Where(x => string.Equals(
                        x.LessonId,
                        lesson.LessonId,
                        StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();
            User instructor = UserService.GetUserById(course.InstructorId);
            LessonProgress currentProgress = GetProgress(
                progressRecords,
                lesson.LessonId);
            int currentProgressPercent =
                GetLessonProgressPercent(currentProgress);
            bool isCompleted = IsCompleted(currentProgress);
            bool isInProgress = currentProgress != null &&
                string.Equals(
                    currentProgress.Status,
                    "InProgress",
                    StringComparison.OrdinalIgnoreCase);
            string courseName = string.IsNullOrWhiteSpace(course.CourseName)
                ? course.CourseCode
                : course.CourseName;
            string weekTitle = "Tuần " +
                currentWeek.WeekNumber.ToString(
                    CultureInfo.InvariantCulture) + ": " +
                currentWeek.WeekTitle;
            string displayTitle = "Bài " +
                lesson.DisplayOrder.ToString(
                    CultureInfo.InvariantCulture) + ": " +
                lesson.LessonTitle;

            litBreadcrumbCourse.Text = Encode(courseName);
            litBreadcrumbWeek.Text = Encode(weekTitle);
            litBreadcrumbLesson.Text = Encode(displayTitle);
            litWeekBadge.Text = Encode(weekTitle);
            litLessonStatus.Text = isCompleted
                ? "Đã hoàn thành"
                : isInProgress ? "Đang học" : "Chưa bắt đầu";
            litLessonCode.Text = Encode(string.IsNullOrWhiteSpace(
                lesson.LessonCode) ? lesson.LessonId : lesson.LessonCode);
            litLessonTitle.Text = Encode(displayTitle);
            litLessonDescription.Text = Encode(
                string.IsNullOrWhiteSpace(lesson.ShortDescription)
                    ? "Bài giảng chưa có mô tả."
                    : lesson.ShortDescription);
            litInstructorName.Text = Encode(instructor == null
                ? "Chưa cập nhật"
                : instructor.FullName);
            litDuration.Text = lesson.DurationMinutes > 0
                ? FormatDuration(lesson.DurationMinutes)
                : "Chưa cập nhật";
            litLessonType.Text = Encode(GetLessonTypeText(lesson.LessonType));
            litTerm.Text = Encode(FormatTerm(
                course.Semester,
                course.AcademicYear));
            litProgressPercent.Text = currentProgressPercent.ToString(
                CultureInfo.InvariantCulture);
            lessonProgressTrack.Attributes["aria-valuenow"] =
                currentProgressPercent.ToString(CultureInfo.InvariantCulture);
            lessonProgressTrack.Attributes["aria-label"] =
                "Tiến độ bài giảng";
            lessonProgressBar.Style["width"] =
                currentProgressPercent + "%";
            btnMarkComplete.Enabled = !isCompleted;
            lblCompletionState.Visible = isCompleted;
            lblCompletionState.Text = "Đã hoàn thành";
            pnlActionMessage.Visible = false;

            List<Lesson> accessibleLessons = courseLessons
                .Where(x => string.Equals(
                    x.Status,
                    "Published",
                    StringComparison.OrdinalIgnoreCase))
                .Where(x =>
                {
                    Week lessonWeek = weeks.FirstOrDefault(y =>
                        string.Equals(
                            y.WeekId,
                            x.WeekId,
                            StringComparison.OrdinalIgnoreCase));
                    return lessonWeek != null &&
                        IsWeekAccessible(lessonWeek);
                })
                .OrderBy(x => weeks.FindIndex(y => string.Equals(
                    y.WeekId,
                    x.WeekId,
                    StringComparison.OrdinalIgnoreCase)))
                .ThenBy(x => x.DisplayOrder)
                .ToList();
            int lessonIndex = accessibleLessons.FindIndex(x =>
                string.Equals(
                    x.LessonId,
                    lesson.LessonId,
                    StringComparison.OrdinalIgnoreCase));

            BindAdjacentLesson(
                lnkPreviousLesson,
                lessonIndex > 0
                    ? accessibleLessons[lessonIndex - 1]
                    : null);
            BindAdjacentLesson(
                lnkNextLesson,
                lessonIndex >= 0 &&
                lessonIndex + 1 < accessibleLessons.Count
                    ? accessibleLessons[lessonIndex + 1]
                    : null);

            List<LessonSectionItem> sections = lesson.ContentSections
                .OrderBy(x => x.DisplayOrder)
                .Select((section, index) => new LessonSectionItem
                {
                    Number = (index + 1).ToString(
                        "00",
                        CultureInfo.InvariantCulture),
                    Title = section.Title,
                    Body = section.Body,
                    IsCode = string.Equals(
                        section.SectionType,
                        "Code",
                        StringComparison.OrdinalIgnoreCase)
                })
                .ToList();
            pnlNoLessonSections.Visible = sections.Count == 0;
            litContentSectionCount.Text = sections.Count + " mục";
            rptLessonSections.DataSource = sections;
            rptLessonSections.DataBind();

            LessonResource videoResource = resources.FirstOrDefault(x =>
                string.Equals(
                    x.ContentItemType,
                    "Video",
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    x.FileFormat,
                    "MP4",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(
                    ResolveResourceUrl(x.ResourceUrl)));
            pnlVideoPlayer.Visible = videoResource != null;
            pnlVideoUnavailable.Visible = videoResource == null;
            if (videoResource != null)
            {
                lessonVideoSource.Attributes["src"] =
                    ResolveUrl(videoResource.ResourceUrl);
                litVideoPlaceholderTitle.Text = string.Empty;
                litVideoPlaceholderMessage.Text = string.Empty;
            }
            else
            {
                litVideoPlaceholderTitle.Text =
                    Encode(GetLessonTypeText(lesson.LessonType));
                litVideoPlaceholderMessage.Text = Encode(
                    string.Equals(
                        lesson.LessonType,
                        "Video",
                        StringComparison.OrdinalIgnoreCase)
                        ? "Tệp video của bài học chưa có trong thư mục Uploads."
                        : "Tài nguyên của bài học chưa được tải lên.");
            }

            LessonResource downloadResource = resources.FirstOrDefault(x =>
                x.IsDownloadable &&
                !string.IsNullOrWhiteSpace(
                    ResolveResourceUrl(x.ResourceUrl)));
            pnlLessonResources.Visible = downloadResource != null;
            if (downloadResource != null)
            {
                litResourceName.Text = Encode(downloadResource.ResourceName);
                litResourceMetadata.Text = Encode(
                    FormatFileSize(downloadResource.FileSizeBytes) +
                    (string.IsNullOrWhiteSpace(downloadResource.FileFormat)
                        ? string.Empty
                        : " · " + downloadResource.FileFormat));
                lnkDownloadResource.NavigateUrl =
                    ResolveUrl(downloadResource.ResourceUrl);
                lnkDownloadResource.Attributes["rel"] = "noopener";
            }

            int courseProgress = CalculateCourseProgress(
                courseLessons,
                progressRecords);
            int completedCount = courseLessons.Count(x =>
                IsCompleted(GetProgress(progressRecords, x.LessonId)));
            int weekLessonCount = courseLessons.Count(x =>
                string.Equals(
                    x.WeekId,
                    currentWeek.WeekId,
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    x.Status,
                    "Published",
                    StringComparison.OrdinalIgnoreCase));
            int weekCompletedCount = courseLessons.Count(x =>
                string.Equals(
                    x.WeekId,
                    currentWeek.WeekId,
                    StringComparison.OrdinalIgnoreCase) &&
                IsCompleted(GetProgress(progressRecords, x.LessonId)));
            double weekProgress = weekLessonCount == 0
                ? 0
                : Math.Round(
                    courseLessons
                        .Where(x => string.Equals(
                            x.WeekId,
                            currentWeek.WeekId,
                            StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(
                                x.Status,
                                "Published",
                                StringComparison.OrdinalIgnoreCase))
                        .Sum(x => GetLessonProgressPercent(
                            GetProgress(progressRecords, x.LessonId))) /
                    (double)weekLessonCount,
                    MidpointRounding.AwayFromZero);

            litCourseProgressPercent.Text = courseProgress.ToString(
                CultureInfo.InvariantCulture);
            litCourseCompletionCount.Text = completedCount + " / " +
                courseLessons.Count + " bài giảng";
            courseProgressBar.Style["width"] = courseProgress + "%";
            litCurrentWeekProgress.Text = "Tuần " +
                currentWeek.WeekNumber.ToString(
                    CultureInfo.InvariantCulture) + " (" +
                weekCompletedCount + "/" + weekLessonCount + ")";

            List<LessonNavigationWeekItem> weekItems = weeks
                .Select(week => BuildWeekItem(
                    week,
                    currentWeek,
                    courseLessons,
                    progressRecords,
                    lesson))
                .ToList();
            rptCourseWeeks.DataSource = weekItems;
            rptCourseWeeks.DataBind();

            if (instructor != null &&
                !string.IsNullOrWhiteSpace(instructor.Email))
            {
                lnkInstructorEmail.Visible = true;
                lnkInstructorEmail.NavigateUrl =
                    "mailto:" + instructor.Email;
            }

            litCurrentWeekProgress.Text += " · " +
                weekProgress.ToString(
                    "0",
                    CultureInfo.InvariantCulture) + "%";
        }

        private LessonNavigationWeekItem BuildWeekItem(
            Week week,
            Week currentWeek,
            List<Lesson> courseLessons,
            List<LessonProgress> progressRecords,
            Lesson currentLesson)
        {
            bool accessible = IsWeekAccessible(week);
            List<Lesson> visibleLessons = accessible
                ? courseLessons
                    .Where(x => string.Equals(
                        x.WeekId,
                        week.WeekId,
                        StringComparison.OrdinalIgnoreCase))
                    .Where(x => string.Equals(
                        x.Status,
                        "Published",
                        StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.DisplayOrder)
                    .ToList()
                : new List<Lesson>();
            List<LessonNavigationItem> lessonItems = visibleLessons
                .Select(lesson =>
                {
                    LessonProgress progress = GetProgress(
                        progressRecords,
                        lesson.LessonId);
                    bool completed = IsCompleted(progress);
                    bool inProgress = progress != null &&
                        string.Equals(
                            progress.Status,
                            "InProgress",
                            StringComparison.OrdinalIgnoreCase);
                    string displayTitle = "Bài " +
                        lesson.DisplayOrder.ToString(
                            CultureInfo.InvariantCulture) + ": " +
                        lesson.LessonTitle;
                    return new LessonNavigationItem
                    {
                        LessonId = lesson.LessonId,
                        DisplayTitle = displayTitle,
                        Url = ResolveUrl(
                            "~/Student/LessonDetails.aspx?lessonId=" +
                            Server.UrlEncode(lesson.LessonId)),
                        Meta = FormatDuration(lesson.DurationMinutes) +
                            " · " + GetLessonTypeText(lesson.LessonType),
                        ProgressText = completed
                            ? "✓"
                            : inProgress
                                ? GetLessonProgressPercent(progress) + "%"
                                : "›",
                        Icon = completed
                            ? "✓"
                            : GetLessonTypeIcon(lesson.LessonType),
                        CurrentCss = string.Equals(
                            lesson.LessonId,
                            currentLesson.LessonId,
                            StringComparison.OrdinalIgnoreCase)
                            ? "is-current"
                            : string.Empty,
                        SearchText = displayTitle + " " +
                            lesson.LessonCode + " " +
                            lesson.ShortDescription
                    };
                })
                .ToList();
            int weekProgress = visibleLessons.Count == 0
                ? 0
                : (int)Math.Round(
                    visibleLessons.Sum(x => GetLessonProgressPercent(
                        GetProgress(progressRecords, x.LessonId))) /
                    (double)visibleLessons.Count,
                    MidpointRounding.AwayFromZero);
            bool isCurrentWeek = string.Equals(
                currentWeek.WeekId,
                week.WeekId,
                StringComparison.OrdinalIgnoreCase);

            return new LessonNavigationWeekItem
            {
                WeekTitle = "TUẦN " +
                    week.WeekNumber.ToString(
                        CultureInfo.InvariantCulture) + ": " +
                    week.WeekTitle,
                StatusText = accessible ? "Đang học" : "Khóa",
                StatusCss = accessible ? "is-open" : "is-locked",
                ProgressSummary = visibleLessons.Count == 0
                    ? "Chưa có bài giảng"
                    : lessonItems.Count(x => x.ProgressText == "✓") +
                        "/" + visibleLessons.Count + " bài · " +
                        weekProgress + "%",
                CurrentCss = isCurrentWeek ? "is-current" : string.Empty,
                IsAccessible = accessible,
                HasLessons = lessonItems.Count > 0,
                Lessons = lessonItems
            };
        }

        private void BindAdjacentLesson(
            HyperLink link,
            Lesson lesson)
        {
            link.Visible = lesson != null;
            if (lesson == null)
            {
                return;
            }

            link.NavigateUrl = ResolveUrl(
                "~/Student/LessonDetails.aspx?lessonId=" +
                Server.UrlEncode(lesson.LessonId));
            link.Text = "Bài " +
                lesson.DisplayOrder.ToString(
                    CultureInfo.InvariantCulture) +
                (link.ID == "lnkPreviousLesson"
                    ? "  ← Bài trước"
                    : "  Bài tiếp theo →");
        }

        private bool TryGetAccessibleLesson(
            out User student,
            out Course course,
            out Week week,
            out Lesson lesson)
        {
            student = AuthSession.GetCurrentUser();
            lesson = LessonService.GetLessonById(
                Request.QueryString["lessonId"]);
            week = lesson == null
                ? null
                : WeekService.GetWeekById(lesson.WeekId);
            course = week == null
                ? null
                : CourseService.GetCourseById(week.CourseId);

            return student != null &&
                lesson != null &&
                week != null &&
                course != null &&
                EnrollmentService.HasEnrolled(
                    student.UserId,
                    course.CourseId) &&
                IsWeekAccessible(week) &&
                string.Equals(
                    lesson.Status,
                    "Published",
                    StringComparison.OrdinalIgnoreCase);
        }

        private void ConfigureMasterControls()
        {
            Literal heading = FindMasterControl("litPageHeading") as Literal;
            if (heading != null)
            {
                heading.Text = "Chi tiết bài giảng";
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
                explore.NavigateUrl =
                    ResolveUrl("~/Student/ExploreCourses.aspx");
            }

            SetActiveMenu("lnkMyCourses");

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
            return progress != null &&
                string.Equals(
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

        private string ResolveResourceUrl(string resourceUrl)
        {
            if (string.IsNullOrWhiteSpace(resourceUrl) ||
                !resourceUrl.StartsWith("~/", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            string physicalPath = Server.MapPath(resourceUrl);
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

            return resourceUrl;
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

        private static string FormatDuration(int minutes)
        {
            if (minutes <= 0)
            {
                return "Chưa cập nhật";
            }

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

        private static string FormatTerm(string semester, string academicYear)
        {
            if (string.IsNullOrWhiteSpace(semester) &&
                string.IsNullOrWhiteSpace(academicYear))
            {
                return string.Empty;
            }

            string term = string.IsNullOrWhiteSpace(semester)
                ? string.Empty
                : "Học kỳ " + semester.Trim();
            return string.IsNullOrWhiteSpace(academicYear)
                ? term
                : term + (term.Length == 0 ? string.Empty : " · ") +
                    academicYear.Trim();
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes <= 0)
            {
                return "Kích thước chưa cập nhật";
            }

            double megabytes = bytes / 1024d / 1024d;
            return megabytes < 1
                ? Math.Round(bytes / 1024d, 0).ToString(
                    CultureInfo.InvariantCulture) + " KB"
                : megabytes.ToString("0.#", CultureInfo.InvariantCulture) +
                    " MB";
        }

        private static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value ?? string.Empty);
        }
    }

    public class LessonNavigationWeekItem
    {
        public string WeekTitle { get; set; }
        public string StatusText { get; set; }
        public string StatusCss { get; set; }
        public string ProgressSummary { get; set; }
        public string CurrentCss { get; set; }
        public bool IsAccessible { get; set; }
        public bool HasLessons { get; set; }
        public List<LessonNavigationItem> Lessons { get; set; }
    }

    public class LessonNavigationItem
    {
        public string LessonId { get; set; }
        public string DisplayTitle { get; set; }
        public string Url { get; set; }
        public string Meta { get; set; }
        public string ProgressText { get; set; }
        public string Icon { get; set; }
        public string CurrentCss { get; set; }
        public string SearchText { get; set; }
    }

    public class LessonSectionItem
    {
        public string Number { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsCode { get; set; }
    }
}
