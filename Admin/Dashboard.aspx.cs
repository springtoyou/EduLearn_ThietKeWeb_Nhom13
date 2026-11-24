using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using EduLearn.DataAccess;
using EduLearn.Models;
using EduLearn.Services;

namespace EduLearn.Admin
{
    public partial class Dashboard : Page
    {
        private static readonly object xmlWriteLock = new object();
        private const string ActivityLogXmlFile = "ActivityLogs.xml";
        private const string ActivityLogRootPath = "/ActivityLogs";
        private const string ActivityLogItemName = "ActivityLog";

        private const string ActionCreateUser = "CreateUser";
        private const string ActionCreateCourse = "CreateCourse";
        private const string ActionDeactivateUser = "DeactivateUser";

        private string pendingToast = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Admin"))
            {
                return;
            }

            if (!IsPostBack)
            {
                BindDashboard();
                BindAddCourseInstructors();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthSession.Logout();

            Response.Redirect(
                ResolveUrl("~/Login.aspx"),
                false
            );

            Context.ApplicationInstance.CompleteRequest();
        }

        protected void rptUsers_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (!string.Equals(
                e.CommandName,
                "ToggleUserStatus",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            User admin = AuthSession.GetCurrentUser();
            string userId = Convert.ToString(
                e.CommandArgument,
                CultureInfo.InvariantCulture);

            if (admin != null &&
                string.Equals(
                    admin.UserId,
                    userId,
                    StringComparison.OrdinalIgnoreCase))
            {
                ShowFeedback(
                    "Không thể tự khóa tài khoản quản trị đang đăng nhập.",
                    true);
                BindDashboard();
                return;
            }

            User targetUser = UserService.GetAllUsers().FirstOrDefault(u =>
                string.Equals(
                    u.UserId,
                    userId,
                    StringComparison.OrdinalIgnoreCase));

            string newStatus = ToggleXmlActiveStatus(
                "Users.xml",
                "/Users/User",
                "UserId",
                userId);

            if (newStatus != null)
            {
                ShowFeedback(
                    "Đã cập nhật trạng thái hoạt động của tài khoản " +
                    userId + ".",
                    false);

                if (string.Equals(
                    newStatus,
                    "Inactive",
                    StringComparison.OrdinalIgnoreCase))
                {
                    string targetName = targetUser != null &&
                        !string.IsNullOrWhiteSpace(targetUser.FullName)
                        ? targetUser.FullName
                        : userId;

                    LogAdminActivity(
                        ActionDeactivateUser,
                        "Admin ngừng hoạt động tài khoản " +
                        targetName + " (" + userId + ").",
                        "User",
                        userId);
                }
            }

            BindDashboard();
        }

        protected void rptCourses_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (!string.Equals(
                e.CommandName,
                "ToggleCourseStatus",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string courseId = Convert.ToString(
                e.CommandArgument,
                CultureInfo.InvariantCulture);

            if (ToggleXmlActiveStatus(
                "Courses.xml",
                "/Courses/Course",
                "CourseId",
                courseId) != null)
            {
                ShowFeedback(
                    "Đã cập nhật trạng thái hoạt động của môn học " +
                    courseId + ".",
                    false);
            }

            BindDashboard();
        }

        protected void btnCreateUser_Click(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Admin"))
            {
                return;
            }

            TextBox txtUsername = FindControlRecursive(
                this,
                "txtNewUsername") as TextBox;
            TextBox txtFullName = FindControlRecursive(
                this,
                "txtNewFullName") as TextBox;
            TextBox txtEmail = FindControlRecursive(
                this,
                "txtNewEmail") as TextBox;
            TextBox txtPassword = FindControlRecursive(
                this,
                "txtNewPassword") as TextBox;
            TextBox txtPhone = FindControlRecursive(
                this,
                "txtNewPhone") as TextBox;
            DropDownList ddlRole = FindControlRecursive(
                this,
                "ddlNewRole") as DropDownList;
            DropDownList ddlGender = FindControlRecursive(
                this,
                "ddlNewGender") as DropDownList;
            TextBox txtDob = FindControlRecursive(
                this,
                "txtNewDateOfBirth") as TextBox;
            DropDownList ddlStatus = FindControlRecursive(
                this,
                "ddlNewUserStatus") as DropDownList;

            if (txtUsername == null ||
                txtFullName == null ||
                txtEmail == null ||
                txtPassword == null ||
                ddlRole == null)
            {
                return;
            }

            string username = (txtUsername.Text ?? string.Empty).Trim();
            string fullName = (txtFullName.Text ?? string.Empty).Trim();
            string email = (txtEmail.Text ?? string.Empty).Trim();
            string password = txtPassword.Text ?? string.Empty;
            string phone = txtPhone != null
                ? (txtPhone.Text ?? string.Empty).Trim()
                : string.Empty;
            string role = ddlRole.SelectedValue;
            string gender = ddlGender != null
                ? ddlGender.SelectedValue
                : string.Empty;
            string dateOfBirth = txtDob != null
                ? (txtDob.Text ?? string.Empty).Trim()
                : string.Empty;
            string status = ddlStatus != null
                ? ddlStatus.SelectedValue
                : "Active";

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ShowFeedback(
                    "Vui lòng nhập đầy đủ Tên đăng nhập, Họ tên, Email và Mật khẩu.",
                    true);
                BindDashboard();
                return;
            }

            if (password.Length < 6)
            {
                ShowFeedback("Mật khẩu phải có ít nhất 6 ký tự.", true);
                BindDashboard();
                return;
            }

            try
            {
                string newUserId = CreateUserXml(
                    username,
                    fullName,
                    email,
                    password,
                    role,
                    phone,
                    gender,
                    dateOfBirth,
                    status);

                txtUsername.Text = string.Empty;
                txtFullName.Text = string.Empty;
                txtEmail.Text = string.Empty;
                txtPassword.Text = string.Empty;
                if (txtPhone != null) txtPhone.Text = string.Empty;
                if (txtDob != null) txtDob.Text = string.Empty;

                ShowFeedback(
                    "Đã thêm tài khoản mới " + fullName +
                    " (" + newUserId + ").",
                    false);

                // Đẩy thông báo lên tab Nhật ký hoạt động.
                LogAdminActivity(
                    ActionCreateUser,
                    "Admin tạo tài khoản " +
                    GetRoleLabel(role).ToLowerInvariant() +
                    " mới: " + fullName + " (" + newUserId + ").",
                    "User",
                    newUserId);
            }
            catch (InvalidOperationException ex)
            {
                ShowFeedback(ex.Message, true);
            }
            catch (Exception ex)
            {
                ShowFeedback("Không thể thêm tài khoản: " + ex.Message, true);
            }

            BindDashboard();
            BindAddCourseInstructors();
        }

        protected void btnCreateCourse_Click(object sender, EventArgs e)
        {
            if (!AuthorizationService.RequireRole("Admin"))
            {
                return;
            }

            TextBox txtCourseCode = FindControlRecursive(
                this,
                "txtNewCourseCode") as TextBox;
            TextBox txtCourseName = FindControlRecursive(
                this,
                "txtNewCourseName") as TextBox;
            DropDownList ddlInstructor = FindControlRecursive(
                this,
                "ddlNewCourseInstructor") as DropDownList;
            DropDownList ddlCategoryId = FindControlRecursive(
                this,
                "ddlNewCourseCategoryId") as DropDownList;
            TextBox txtCredits = FindControlRecursive(
                this,
                "txtNewCourseCredits") as TextBox;
            TextBox txtTotalWeeks = FindControlRecursive(
                this,
                "txtNewCourseTotalWeeks") as TextBox;
            DropDownList ddlLevel = FindControlRecursive(
                this,
                "ddlNewCourseLevel") as DropDownList;
            DropDownList ddlStatus = FindControlRecursive(
                this,
                "ddlNewCourseStatus") as DropDownList;
            TextBox txtSemester = FindControlRecursive(
                this,
                "txtNewCourseSemester") as TextBox;
            TextBox txtAcademicYear = FindControlRecursive(
                this,
                "txtNewCourseAcademicYear") as TextBox;
            TextBox txtThumbnail = FindControlRecursive(
                this,
                "txtNewCourseThumbnail") as TextBox;
            CheckBox chkEnrollmentOpen = FindControlRecursive(
                this,
                "chkNewCourseEnrollmentOpen") as CheckBox;
            TextBox txtShortDesc = FindControlRecursive(
                this,
                "txtNewCourseShortDesc") as TextBox;
            TextBox txtDescription = FindControlRecursive(
                this,
                "txtNewCourseDescription") as TextBox;
            TextBox txtObjectives = FindControlRecursive(
                this,
                "txtNewCourseObjectives") as TextBox;
            TextBox txtPrerequisites = FindControlRecursive(
                this,
                "txtNewCoursePrerequisites") as TextBox;

            if (txtCourseCode == null ||
                txtCourseName == null ||
                ddlInstructor == null ||
                txtCredits == null ||
                txtTotalWeeks == null ||
                txtShortDesc == null)
            {
                return;
            }

            string courseCode = (txtCourseCode.Text ?? string.Empty).Trim().ToUpperInvariant();
            string courseName = (txtCourseName.Text ?? string.Empty).Trim();
            string instructorId = ddlInstructor.SelectedValue ?? string.Empty;
            string categoryId = ddlCategoryId != null
                ? (ddlCategoryId.SelectedValue ?? string.Empty).Trim()
                : "CAT001";
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                categoryId = "CAT001";
            }

            int credits;
            if (!int.TryParse(
                (txtCredits.Text ?? string.Empty).Trim(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out credits) || credits < 1 || credits > 20)
            {
                ShowFeedback(
                    "Số tín chỉ phải là số nguyên từ 1 đến 20.",
                    true);
                BindDashboard();
                return;
            }

            int totalWeeks;
            if (!int.TryParse(
                (txtTotalWeeks.Text ?? string.Empty).Trim(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out totalWeeks) || totalWeeks < 1 || totalWeeks > 52)
            {
                ShowFeedback(
                    "Số tuần học phải là số nguyên từ 1 đến 52.",
                    true);
                BindDashboard();
                return;
            }

            string level = ddlLevel != null ? ddlLevel.SelectedValue : "Cơ bản";
            string status = ddlStatus != null ? ddlStatus.SelectedValue : "Active";
            string semester = txtSemester != null && !string.IsNullOrWhiteSpace(txtSemester.Text)
                ? txtSemester.Text.Trim()
                : "1";
            string academicYear = txtAcademicYear != null && !string.IsNullOrWhiteSpace(txtAcademicYear.Text)
                ? txtAcademicYear.Text.Trim()
                : "2026-2027";
            string thumbnail = txtThumbnail != null
                ? (txtThumbnail.Text ?? string.Empty).Trim()
                : string.Empty;
            bool enrollmentOpen = chkEnrollmentOpen == null || chkEnrollmentOpen.Checked;
            string shortDescription = (txtShortDesc.Text ?? string.Empty).Trim();
            string description = txtDescription != null && !string.IsNullOrWhiteSpace(txtDescription.Text)
                ? txtDescription.Text.Trim()
                : shortDescription;

            List<string> objectives = SplitLines(txtObjectives != null ? txtObjectives.Text : string.Empty);
            List<string> prerequisites = SplitLines(txtPrerequisites != null ? txtPrerequisites.Text : string.Empty);

            if (string.IsNullOrWhiteSpace(courseCode) ||
                string.IsNullOrWhiteSpace(courseName) ||
                string.IsNullOrWhiteSpace(shortDescription))
            {
                ShowFeedback(
                    "Vui lòng nhập đầy đủ Mã môn học, Tên môn học và Mô tả ngắn.",
                    true);
                BindDashboard();
                return;
            }

            try
            {
                string newCourseId = CreateCourseXml(
                    courseCode,
                    courseName,
                    categoryId,
                    instructorId,
                    shortDescription,
                    description,
                    thumbnail,
                    credits,
                    totalWeeks,
                    objectives,
                    prerequisites,
                    level,
                    semester,
                    academicYear,
                    enrollmentOpen,
                    status);

                txtCourseCode.Text = string.Empty;
                txtCourseName.Text = string.Empty;
                txtShortDesc.Text = string.Empty;
                if (txtDescription != null) txtDescription.Text = string.Empty;
                if (txtThumbnail != null) txtThumbnail.Text = string.Empty;
                if (txtObjectives != null) txtObjectives.Text = string.Empty;
                if (txtPrerequisites != null) txtPrerequisites.Text = string.Empty;

                ShowFeedback(
                    "Đã thêm môn học mới \"" + courseName +
                    "\" (" + newCourseId + " · " + courseCode + ").",
                    false);

                // Đẩy thông báo lên tab Nhật ký hoạt động.
                LogAdminActivity(
                    ActionCreateCourse,
                    "Admin tạo môn học mới: " + courseName +
                    " (" + courseCode + " · " + newCourseId + ").",
                    "Course",
                    newCourseId);
            }
            catch (InvalidOperationException ex)
            {
                ShowFeedback(ex.Message, true);
            }
            catch (Exception ex)
            {
                ShowFeedback("Không thể thêm môn học: " + ex.Message, true);
            }

            BindDashboard();
        }

        private void BindDashboard()
        {
            User admin = AuthSession.GetCurrentUser();
            if (admin == null)
            {
                return;
            }

            List<User> users = UserService.GetAllUsers()
                .OrderBy(x => GetRoleOrder(x.Role))
                .ThenBy(x => x.FullName)
                .ToList();
            List<Course> courses = CourseService.GetAllCourses()
                .OrderBy(x => x.CourseName)
                .ToList();
            List<Lesson> lessons = LessonService.GetAllLessons();
            List<Week> weeks = WeekService.GetAllWeeks();
            List<Enrollment> enrollments =
                EnrollmentService.GetAllEnrollments();
            List<ActivityLog> allActivities =
                ActivityLogService.GetAllActivities().ToList();
            List<ActivityLog> activities = allActivities
                .OrderByDescending(x => ParseDate(x.CreatedAt))
                .Take(6)
                .ToList();
            Dictionary<string, User> usersById = users
                .Where(x => !string.IsNullOrWhiteSpace(x.UserId))
                .GroupBy(x => x.UserId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    x => x.Key,
                    x => x.First(),
                    StringComparer.OrdinalIgnoreCase);

            litAdminName.Text = Encode(admin.FullName);
            litWelcomeName.Text = Encode(admin.FullName);
            litAdminInitials.Text = Encode(GetInitials(admin.FullName));
            string[] terms = courses
                .Select(x => FormatTerm(x.Semester, x.AcademicYear))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            string termSummary = terms.Length == 0
                ? "Chưa cập nhật học kỳ"
                : string.Join(", ", terms);
            litToday.Text = "Hôm nay: " +
                DateTime.Now.ToString(
                    "dddd, dd/MM/yyyy",
                    new CultureInfo("vi-VN")) +
                " · " + termSummary;
            litSystemStatus.Text = "Dữ liệu XML đã tải";

            int activeEnrollmentsTotal = enrollments.Count(x =>
                !string.Equals(
                    x.Status,
                    "Cancelled",
                    StringComparison.OrdinalIgnoreCase));

            litTotalUsers.Text = users.Count.ToString(
                CultureInfo.InvariantCulture);
            litActiveUsers.Text = users.Count(x =>
                string.Equals(
                    x.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase)).ToString(
                    CultureInfo.InvariantCulture);
            litStudentCount.Text = users.Count(x =>
                string.Equals(
                    x.Role,
                    "Student",
                    StringComparison.OrdinalIgnoreCase)).ToString(
                    CultureInfo.InvariantCulture);
            litInstructorCount.Text = users.Count(x =>
                string.Equals(
                    x.Role,
                    "Instructor",
                    StringComparison.OrdinalIgnoreCase)).ToString(
                    CultureInfo.InvariantCulture);
            litCourseCount.Text = courses.Count.ToString(
                CultureInfo.InvariantCulture);
            litEnrollmentCount.Text = activeEnrollmentsTotal.ToString(
                CultureInfo.InvariantCulture);
            litUserCount.Text = users.Count.ToString(
                CultureInfo.InvariantCulture);
            litUserCountFooter.Text = users.Count.ToString(
                CultureInfo.InvariantCulture);
            litCourseCountText.Text = courses.Count.ToString(
                CultureInfo.InvariantCulture);

            List<AdminUserRow> userRows = users
                .Select((user, index) => new AdminUserRow
                {
                    Number = (index + 1).ToString(
                        CultureInfo.InvariantCulture),
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Initials = GetInitials(user.FullName),
                    Email = user.Email,
                    RoleKey = GetRoleKey(user.Role),
                    RoleLabel = GetRoleLabel(user.Role),
                    RoleCss = GetRoleCss(user.Role),
                    StatusKey = string.Equals(
                        user.Status,
                        "Active",
                        StringComparison.OrdinalIgnoreCase)
                        ? "active"
                        : "inactive",
                    StatusLabel = GetStatusLabel(user.Status),
                    StatusCss = GetStatusCss(user.Status),
                    SearchText = string.Join(
                        " ",
                        new[]
                        {
                            user.UserId,
                            user.Username,
                            user.FullName,
                            user.Email,
                            user.Role
                        })
                })
                .ToList();
            rptUsers.DataSource = userRows;
            rptUsers.DataBind();
            pnlNoUsers.Visible = userRows.Count == 0;

            List<AdminCourseRow> courseRows = courses
                .Select(course =>
                {
                    User instructor;
                    usersById.TryGetValue(course.InstructorId, out instructor);
                    List<Week> courseWeeks = weeks.Where(x =>
                        string.Equals(
                            x.CourseId,
                            course.CourseId,
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    int lessonCount = lessons.Count(lesson =>
                        courseWeeks.Any(week => string.Equals(
                            week.WeekId,
                            lesson.WeekId,
                            StringComparison.OrdinalIgnoreCase)));
                    int activeCourseEnrollments = enrollments.Count(x =>
                        string.Equals(
                            x.CourseId,
                            course.CourseId,
                            StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(
                            x.Status,
                            "Cancelled",
                            StringComparison.OrdinalIgnoreCase));
                    return new AdminCourseRow
                    {
                        CourseId = course.CourseId,
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName,
                        Credits = course.Credits.ToString(
                            CultureInfo.InvariantCulture),
                        InstructorName = instructor == null
                            ? "Chưa phân công"
                            : instructor.FullName,
                        WeekCount = courseWeeks.Count.ToString(
                            CultureInfo.InvariantCulture),
                        LessonCount = lessonCount.ToString(
                            CultureInfo.InvariantCulture),
                        EnrollmentCount = activeCourseEnrollments
                            .ToString(CultureInfo.InvariantCulture),
                        StatusLabel = GetCourseStatusLabel(course.Status),
                        StatusCss = string.Equals(
                            course.Status,
                            "Active",
                            StringComparison.OrdinalIgnoreCase)
                            ? "is-active"
                            : "is-neutral",
                        SearchText = string.Join(
                            " ",
                            new[]
                            {
                                course.CourseId,
                                course.CourseCode,
                                course.CourseName,
                                instructor == null
                                    ? string.Empty
                                    : instructor.FullName
                            })
                    };
                })
                .ToList();
            rptCourses.DataSource = courseRows;
            rptCourses.DataBind();
            pnlNoCourses.Visible = courseRows.Count == 0;

            List<AdminActivityRow> activityRows = activities
                .Select(activity =>
                {
                    User actor;
                    usersById.TryGetValue(activity.UserId, out actor);
                    return new AdminActivityRow
                    {
                        UserName = actor == null
                            ? "Tài khoản " + activity.UserId
                            : actor.FullName,
                        Description = activity.Description,
                        CreatedAt = FormatDate(activity.CreatedAt),
                        Icon = GetActivityIcon(activity.ActionType),
                        IconCss = GetActivityCss(activity.ActionType),
                        SortKey = ToSortKey(activity.CreatedAt).ToString(
                            CultureInfo.InvariantCulture),
                        IsAlert = IsAlertAction(activity.ActionType)
                            ? "true"
                            : "false"
                    };
                })
                .ToList();

            List<AdminRoleRow> roleRows = new[]
            {
                new AdminRoleRow
                {
                    Role = "Admin",
                    RoleLabel = "Quản trị viên",
                    Description = "Quản lý hệ thống"
                },
                new AdminRoleRow
                {
                    Role = "Instructor",
                    RoleLabel = "Giảng viên",
                    Description = "Phụ trách nội dung môn học"
                },
                new AdminRoleRow
                {
                    Role = "Student",
                    RoleLabel = "Sinh viên",
                    Description = "Học tập các môn đã đăng ký"
                }
            }
            .Select(role => new AdminRoleRow
            {
                Role = role.Role,
                RoleLabel = role.RoleLabel,
                Description = role.Description,
                Count = users.Count(user => string.Equals(
                    user.Role,
                    role.Role,
                    StringComparison.OrdinalIgnoreCase)).ToString(
                    CultureInfo.InvariantCulture),
                RoleCss = GetRoleCss(role.Role)
            })
            .ToList();
            rptRoleCounts.DataSource = roleRows;
            rptRoleCounts.DataBind();

            BindPopupJsonData(
                users,
                courses,
                enrollments,
                usersById,
                admin,
                allActivities);
        }

        private void BindAddCourseInstructors()
        {
            DropDownList ddlInstructor = FindControlRecursive(
                this,
                "ddlNewCourseInstructor") as DropDownList;
            if (ddlInstructor == null)
            {
                return;
            }

            List<User> instructors = UserService.GetAllUsers()
                .Where(u => string.Equals(
                    u.Role,
                    "Instructor",
                    StringComparison.OrdinalIgnoreCase))
                .OrderBy(u => u.FullName)
                .ToList();

            ddlInstructor.Items.Clear();
            foreach (User inst in instructors)
            {
                string label = string.IsNullOrWhiteSpace(inst.UserId)
                    ? inst.FullName
                    : inst.UserId + " - " + inst.FullName;
                ddlInstructor.Items.Add(new ListItem(label, inst.UserId));
            }
        }

        private void BindPopupJsonData(
            List<User> users,
            List<Course> courses,
            List<Enrollment> enrollments,
            Dictionary<string, User> usersById,
            User admin,
            List<ActivityLog> allActivities)
        {
            Literal litPopupsJson = FindControlRecursive(
                this,
                "litAdminPopupsJson") as Literal;
            if (litPopupsJson == null)
            {
                return;
            }

            var accountDtos = users.Select(u => new
            {
                UserId = u.UserId ?? string.Empty,
                FullName = u.FullName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                Role = u.Role ?? string.Empty,
                IsActive = string.Equals(
                    u.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase),
                StatusText = GetStatusLabel(u.Status)
            }).ToList();

            var courseDtos = courses.Select(c =>
            {
                User instructor = null;
                if (!string.IsNullOrWhiteSpace(c.InstructorId))
                {
                    usersById.TryGetValue(c.InstructorId, out instructor);
                }

                List<Enrollment> courseEnrollments = enrollments
                    .Where(en =>
                        string.Equals(
                            en.CourseId,
                            c.CourseId,
                            StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(
                            en.Status,
                            "Cancelled",
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var studentDtos = courseEnrollments.Select(en =>
                {
                    User st = null;
                    if (!string.IsNullOrWhiteSpace(en.StudentId))
                    {
                        usersById.TryGetValue(en.StudentId, out st);
                    }

                    return new
                    {
                        StudentId = en.StudentId ?? string.Empty,
                        FullName = st != null &&
                            !string.IsNullOrWhiteSpace(st.FullName)
                            ? st.FullName
                            : (en.StudentId ?? string.Empty),
                        Email = st != null ? (st.Email ?? string.Empty) : string.Empty,
                        EnrollmentDate = FormatDate(en.EnrollmentDate),
                        Status = string.Equals(
                            en.Status,
                            "Completed",
                            StringComparison.OrdinalIgnoreCase)
                            ? "Đã hoàn thành"
                            : "Đang học"
                    };
                }).ToList();

                return new
                {
                    CourseId = c.CourseId ?? string.Empty,
                    CourseCode = c.CourseCode ?? string.Empty,
                    CourseName = c.CourseName ?? string.Empty,
                    InstructorName = instructor != null &&
                        !string.IsNullOrWhiteSpace(instructor.FullName)
                        ? instructor.FullName
                        : "Chưa phân công",
                    EnrolledCount = studentDtos.Count,
                    Students = studentDtos
                };
            }).ToList();

            long[] alertKeys = allActivities
                .Where(a => IsAlertAction(a.ActionType))
                .Select(a => ToSortKey(a.CreatedAt))
                .Where(k => k > 0)
                .ToArray();

            var serializer = new JavaScriptSerializer();
            string payload = serializer.Serialize(new
            {
                accounts = accountDtos,
                courses = courseDtos,
                Activity = new
                {
                    AdminId = admin != null
                        ? (admin.UserId ?? string.Empty)
                        : string.Empty,
                    AlertKeys = alertKeys,
                    Toast = pendingToast ?? string.Empty
                }
            });

            payload = payload
                .Replace("<", "\\u003c")
                .Replace(">", "\\u003e");

            litPopupsJson.Text =
                "<script type=\"text/javascript\">window.EDU_ADMIN_POPUPS = " +
                payload + ";</script>";
        }

        private static string ToggleXmlActiveStatus(
            string xmlFileName,
            string itemXPath,
            string idTagName,
            string idValue)
        {
            if (string.IsNullOrWhiteSpace(idValue))
            {
                return null;
            }

            lock (xmlWriteLock)
            {
                XmlDocument doc = XmlDataAccess.LoadXml(xmlFileName);
                XmlNodeList nodes = doc.SelectNodes(itemXPath);
                if (nodes == null)
                {
                    return null;
                }

                string now = DateTime.Now.ToString(
                    "yyyy-MM-dd'T'HH:mm:ss",
                    CultureInfo.InvariantCulture);

                foreach (XmlNode node in nodes)
                {
                    XmlNode idNode = node[idTagName];
                    if (idNode != null &&
                        string.Equals(
                            idNode.InnerText.Trim(),
                            idValue.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        XmlNode statusNode = node["Status"];
                        if (statusNode == null)
                        {
                            statusNode = doc.CreateElement("Status");
                            node.AppendChild(statusNode);
                        }

                        bool isActive = string.Equals(
                            statusNode.InnerText.Trim(),
                            "Active",
                            StringComparison.OrdinalIgnoreCase);
                        string newStatus = isActive ? "Inactive" : "Active";
                        statusNode.InnerText = newStatus;

                        XmlNode updatedAtNode = node["UpdatedAt"];
                        if (updatedAtNode != null)
                        {
                            updatedAtNode.InnerText = now;
                        }

                        XmlDataAccess.SaveXml(xmlFileName, doc);
                        return newStatus;
                    }
                }
            }

            return null;
        }

        private static string CreateUserXml(
            string username,
            string fullName,
            string email,
            string password,
            string role,
            string phone,
            string gender,
            string dateOfBirth,
            string status)
        {
            lock (xmlWriteLock)
            {
                XmlDocument doc = XmlDataAccess.LoadXml("Users.xml");
                XmlNode root = doc.SelectSingleNode("/Users");
                if (root == null)
                {
                    throw new XmlException(
                        "Tệp Users.xml không có phần tử gốc /Users.");
                }

                List<User> existingUsers = UserService.GetAllUsers();
                bool isDuplicate = existingUsers.Any(u =>
                    string.Equals(
                        u.Username,
                        username,
                        StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(
                        u.Email,
                        email,
                        StringComparison.OrdinalIgnoreCase));

                if (isDuplicate)
                {
                    throw new InvalidOperationException(
                        "Tên đăng nhập hoặc email đã tồn tại.");
                }

                int nextSeq = GetNextIdSequence(
                    existingUsers.Select(u => u.UserId),
                    "U") + 1;
                string newUserId = "U" + nextSeq.ToString(
                    "D3",
                    CultureInfo.InvariantCulture);
                string now = DateTime.Now.ToString(
                    "yyyy-MM-dd'T'HH:mm:ss",
                    CultureInfo.InvariantCulture);

                XmlElement userEl = doc.CreateElement("User");
                AppendXmlElement(doc, userEl, "UserId", newUserId);
                AppendXmlElement(doc, userEl, "Username", username);
                AppendXmlElement(doc, userEl, "FullName", fullName);
                AppendXmlElement(doc, userEl, "Email", email);
                AppendXmlElement(
                    doc,
                    userEl,
                    "PasswordHash",
                    HashPasswordPbkdf2(password));
                AppendXmlElement(doc, userEl, "Role", role);
                AppendXmlElement(doc, userEl, "Phone", phone);
                AppendXmlElement(doc, userEl, "Avatar", string.Empty);
                AppendXmlElement(doc, userEl, "Gender", gender);
                AppendXmlElement(doc, userEl, "DateOfBirth", dateOfBirth);
                AppendXmlElement(doc, userEl, "Status", status);
                AppendXmlElement(doc, userEl, "CreatedAt", now);
                AppendXmlElement(doc, userEl, "UpdatedAt", now);
                AppendXmlElement(doc, userEl, "LastLoginAt", string.Empty);

                root.AppendChild(userEl);
                XmlDataAccess.SaveXml("Users.xml", doc);
                return newUserId;
            }
        }

        private static string CreateCourseXml(
            string courseCode,
            string courseName,
            string categoryId,
            string instructorId,
            string shortDescription,
            string description,
            string thumbnail,
            int credits,
            int totalWeeks,
            List<string> objectives,
            List<string> prerequisites,
            string level,
            string semester,
            string academicYear,
            bool enrollmentOpen,
            string status)
        {
            lock (xmlWriteLock)
            {
                XmlDocument doc = XmlDataAccess.LoadXml("Courses.xml");
                XmlNode root = doc.SelectSingleNode("/Courses");
                if (root == null)
                {
                    throw new XmlException(
                        "Tệp Courses.xml không có phần tử gốc /Courses.");
                }

                List<Course> existingCourses = CourseService.GetAllCourses();
                bool codeExists = existingCourses.Any(c =>
                    string.Equals(
                        c.CourseCode,
                        courseCode,
                        StringComparison.OrdinalIgnoreCase));

                if (codeExists)
                {
                    throw new InvalidOperationException(
                        "Mã môn học \"" + courseCode + "\" đã tồn tại.");
                }

                int nextSeq = GetNextIdSequence(
                    existingCourses.Select(c => c.CourseId),
                    "C") + 1;
                string newCourseId = "C" + nextSeq.ToString(
                    "D3",
                    CultureInfo.InvariantCulture);
                string now = DateTime.Now.ToString(
                    "yyyy-MM-dd'T'HH:mm:ss",
                    CultureInfo.InvariantCulture);

                XmlElement courseEl = doc.CreateElement("Course");
                AppendXmlElement(doc, courseEl, "CourseId", newCourseId);
                AppendXmlElement(doc, courseEl, "CourseCode", courseCode);
                AppendXmlElement(doc, courseEl, "CourseName", courseName);
                AppendXmlElement(doc, courseEl, "CategoryId", categoryId);
                AppendXmlElement(doc, courseEl, "InstructorId", instructorId);
                AppendXmlElement(doc, courseEl, "ShortDescription", shortDescription);
                AppendXmlElement(doc, courseEl, "Description", description);
                AppendXmlElement(doc, courseEl, "Thumbnail", thumbnail);
                AppendXmlElement(
                    doc,
                    courseEl,
                    "Credits",
                    credits.ToString(CultureInfo.InvariantCulture));
                AppendXmlElement(
                    doc,
                    courseEl,
                    "TotalWeeks",
                    totalWeeks.ToString(CultureInfo.InvariantCulture));

                XmlElement objectivesEl = doc.CreateElement("LearningObjectives");
                foreach (string obj in objectives)
                {
                    AppendXmlElement(doc, objectivesEl, "Objective", obj);
                }
                courseEl.AppendChild(objectivesEl);

                XmlElement prereqEl = doc.CreateElement("Prerequisites");
                foreach (string pre in prerequisites)
                {
                    AppendXmlElement(doc, prereqEl, "Prerequisite", pre);
                }
                courseEl.AppendChild(prereqEl);

                AppendXmlElement(doc, courseEl, "Level", level);
                AppendXmlElement(doc, courseEl, "Semester", semester);
                AppendXmlElement(doc, courseEl, "AcademicYear", academicYear);
                AppendXmlElement(
                    doc,
                    courseEl,
                    "EnrollmentOpen",
                    enrollmentOpen ? "true" : "false");
                AppendXmlElement(doc, courseEl, "Status", status);
                AppendXmlElement(doc, courseEl, "CreatedAt", now);
                AppendXmlElement(doc, courseEl, "UpdatedAt", now);

                root.AppendChild(courseEl);
                XmlDataAccess.SaveXml("Courses.xml", doc);
                return newCourseId;
            }
        }

        private void LogAdminActivity(
            string actionType,
            string description,
            string targetType,
            string targetId)
        {
            User admin = AuthSession.GetCurrentUser();
            string actorId = admin != null
                ? (admin.UserId ?? string.Empty)
                : string.Empty;

            if (WriteActivityLog(
                actorId,
                actionType,
                description,
                targetType,
                targetId))
            {
                pendingToast = description;
            }
        }

        private static bool WriteActivityLog(
            string actorUserId,
            string actionType,
            string description,
            string targetType,
            string targetId)
        {
            try
            {
                lock (xmlWriteLock)
                {
                    XmlDocument doc = XmlDataAccess.LoadXml(ActivityLogXmlFile);
                    XmlNode root = doc.SelectSingleNode(ActivityLogRootPath);
                    if (root == null)
                    {
                        return false;
                    }

                    XmlNodeList items = root.SelectNodes(ActivityLogItemName);

                    List<string> fieldNames = new List<string>();
                    if (items != null && items.Count > 0)
                    {
                        XmlNode template = items[items.Count - 1];
                        foreach (XmlNode child in template.ChildNodes)
                        {
                            if (child.NodeType == XmlNodeType.Element)
                            {
                                fieldNames.Add(child.Name);
                            }
                        }
                    }

                    if (fieldNames.Count == 0)
                    {
                        fieldNames.AddRange(new[]
                        {
                            "ActivityId",
                            "UserId",
                            "ActionType",
                            "Description",
                            "CreatedAt"
                        });
                    }

                    string idField = null;
                    string firstField = fieldNames[0];
                    if (firstField.EndsWith("Id", StringComparison.Ordinal) &&
                        !string.Equals(
                            firstField,
                            "UserId",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        idField = firstField;
                    }

                    string newId = string.Empty;
                    if (idField != null)
                    {
                        List<string> existingIds = new List<string>();
                        if (items != null)
                        {
                            foreach (XmlNode item in items)
                            {
                                XmlNode idNode = item[idField];
                                if (idNode != null)
                                {
                                    existingIds.Add(idNode.InnerText.Trim());
                                }
                            }
                        }

                        newId = GetNextFormattedId(existingIds, "A", 3);
                    }

                    string now = DateTime.Now.ToString(
                        "yyyy-MM-dd'T'HH:mm:ss",
                        CultureInfo.InvariantCulture);

                    XmlElement logEl = doc.CreateElement(ActivityLogItemName);
                    foreach (string fieldName in fieldNames)
                    {
                        AppendXmlElement(
                            doc,
                            logEl,
                            fieldName,
                            GetActivityFieldValue(
                                fieldName,
                                idField,
                                newId,
                                actorUserId,
                                actionType,
                                description,
                                targetType,
                                targetId,
                                now));
                    }

                    root.AppendChild(logEl);
                    XmlDataAccess.SaveXml(ActivityLogXmlFile, doc);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetActivityFieldValue(
            string fieldName,
            string idField,
            string newId,
            string actorUserId,
            string actionType,
            string description,
            string targetType,
            string targetId,
            string now)
        {
            if (idField != null &&
                string.Equals(fieldName, idField, StringComparison.Ordinal))
            {
                return newId;
            }

            switch (fieldName.ToLowerInvariant())
            {
                case "userid":
                    return actorUserId;
                case "actiontype":
                    return actionType;
                case "description":
                    return description;
                case "createdat":
                    return now;
                case "targettype":
                case "entitytype":
                case "relatedtype":
                case "objecttype":
                    return targetType;
                case "targetid":
                case "entityid":
                case "relatedid":
                case "objectid":
                    return targetId;
                default:
                    return string.Empty;
            }
        }

        private static string GetNextFormattedId(
            IEnumerable<string> ids,
            string fallbackPrefix,
            int fallbackWidth)
        {
            string prefix = fallbackPrefix;
            int width = fallbackWidth;
            int max = 0;
            bool found = false;

            foreach (string id in ids)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    continue;
                }

                Match match = Regex.Match(id.Trim(), @"^(\D*)(\d+)$");
                if (!match.Success)
                {
                    continue;
                }

                int number;
                if (!int.TryParse(
                    match.Groups[2].Value,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out number))
                {
                    continue;
                }

                if (!found || number > max)
                {
                    max = number;
                    prefix = match.Groups[1].Value;
                    width = match.Groups[2].Value.Length;
                    found = true;
                }
            }

            return prefix + (max + 1).ToString(
                "D" + width.ToString(CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture);
        }

        private static bool IsAlertAction(string action)
        {
            return string.Equals(
                    action,
                    ActionCreateUser,
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    action,
                    ActionCreateCourse,
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    action,
                    ActionDeactivateUser,
                    StringComparison.OrdinalIgnoreCase);
        }

        private static long ToSortKey(string value)
        {
            DateTime date = ParseDate(value);
            if (date == DateTime.MinValue)
            {
                return 0;
            }

            return long.Parse(
                date.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture);
        }

        private static List<string> SplitLines(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return new List<string>();
            }

            return raw.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();
        }

        private static string HashPasswordPbkdf2(string password)
        {
            const int iterations = 100000;
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return iterations.ToString(CultureInfo.InvariantCulture) +
                    "." + Convert.ToBase64String(salt) +
                    "." + Convert.ToBase64String(hash);
            }
        }

        private static int GetNextIdSequence(
            IEnumerable<string> ids,
            string prefix)
        {
            int max = 0;
            foreach (string id in ids)
            {
                if (!string.IsNullOrWhiteSpace(id) &&
                    id.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    int num;
                    if (int.TryParse(
                        id.Substring(prefix.Length),
                        NumberStyles.None,
                        CultureInfo.InvariantCulture,
                        out num))
                    {
                        max = Math.Max(max, num);
                    }
                }
            }
            return max;
        }

        private static void AppendXmlElement(
            XmlDocument doc,
            XmlElement parent,
            string tagName,
            string value)
        {
            XmlElement el = doc.CreateElement(tagName);
            el.InnerText = value ?? string.Empty;
            parent.AppendChild(el);
        }

        private void ShowFeedback(string message, bool isError)
        {
            Panel pnl = FindControlRecursive(
                this,
                "pnlAdminFeedback") as Panel;
            Literal lit = FindControlRecursive(
                this,
                "litAdminFeedback") as Literal;

            if (lit != null)
            {
                lit.Text = Encode(message);
            }
            if (pnl != null)
            {
                pnl.CssClass = isError
                    ? "admin-feedback-banner is-error"
                    : "admin-feedback-banner is-success";
                pnl.Visible = true;
            }
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
                Control found = FindControlRecursive(child, id);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static int GetRoleOrder(string role)
        {
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            if (string.Equals(
                role,
                "Instructor",
                StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }

            if (string.Equals(role, "Student", StringComparison.OrdinalIgnoreCase))
            {
                return 2;
            }

            return 3;
        }

        private static string GetRoleKey(string role)
        {
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return "admin";
            }

            if (string.Equals(
                role,
                "Instructor",
                StringComparison.OrdinalIgnoreCase))
            {
                return "instructor";
            }

            if (string.Equals(role, "Student", StringComparison.OrdinalIgnoreCase))
            {
                return "student";
            }

            return "other";
        }

        private static string GetRoleLabel(string role)
        {
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return "Quản trị viên";
            }

            if (string.Equals(
                role,
                "Instructor",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Giảng viên";
            }

            if (string.Equals(role, "Student", StringComparison.OrdinalIgnoreCase))
            {
                return "Sinh viên";
            }

            return string.IsNullOrWhiteSpace(role) ? "Chưa phân quyền" : role;
        }

        private static string GetRoleCss(string role)
        {
            string key = GetRoleKey(role);
            return key == "admin"
                ? "role-admin"
                : key == "instructor"
                    ? "role-instructor"
                    : key == "student"
                        ? "role-student"
                        : "role-other";
        }

        private static string GetStatusLabel(string status)
        {
            if (string.Equals(
                status,
                "Active",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Đang hoạt động";
            }

            if (string.Equals(
                status,
                "Inactive",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Ngừng hoạt động";
            }

            return string.IsNullOrWhiteSpace(status)
                ? "Chưa cập nhật"
                : status;
        }

        private static string GetStatusCss(string status)
        {
            return string.Equals(
                status,
                "Active",
                StringComparison.OrdinalIgnoreCase)
                ? "is-active"
                : "is-neutral";
        }

        private static string GetCourseStatusLabel(string status)
        {
            if (string.Equals(
                status,
                "Active",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Đang hoạt động";
            }

            if (string.Equals(
                status,
                "Inactive",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Ngừng hoạt động";
            }

            return string.IsNullOrWhiteSpace(status)
                ? "Chưa cập nhật"
                : status;
        }

        private static string GetActivityIcon(string action)
        {
            if (string.Equals(action, "Login", StringComparison.OrdinalIgnoreCase))
            {
                return "↪";
            }

            if (string.Equals(
                action,
                "Publish",
                StringComparison.OrdinalIgnoreCase))
            {
                return "↑";
            }

            if (string.Equals(action, "Enroll", StringComparison.OrdinalIgnoreCase))
            {
                return "+";
            }

            if (string.Equals(
                action,
                ActionCreateUser,
                StringComparison.OrdinalIgnoreCase))
            {
                return "+";
            }

            if (string.Equals(
                action,
                ActionCreateCourse,
                StringComparison.OrdinalIgnoreCase))
            {
                return "▤";
            }

            if (string.Equals(
                action,
                ActionDeactivateUser,
                StringComparison.OrdinalIgnoreCase))
            {
                return "!";
            }

            return "•";
        }

        private static string GetActivityCss(string action)
        {
            if (string.Equals(
                action,
                "Publish",
                StringComparison.OrdinalIgnoreCase))
            {
                return "activity-publish";
            }

            if (string.Equals(
                action,
                "Enroll",
                StringComparison.OrdinalIgnoreCase))
            {
                return "activity-enroll";
            }

            if (string.Equals(
                    action,
                    ActionCreateUser,
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    action,
                    ActionCreateCourse,
                    StringComparison.OrdinalIgnoreCase))
            {
                return "activity-create";
            }

            if (string.Equals(
                action,
                ActionDeactivateUser,
                StringComparison.OrdinalIgnoreCase))
            {
                return "activity-deactivate";
            }

            return "activity-default";
        }

        private static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "?";
            }

            string[] parts = fullName.Split(
                new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 1
                ? parts[0].Substring(0, 1).ToUpperInvariant()
                : (parts[0].Substring(0, 1) +
                    parts[parts.Length - 1].Substring(0, 1))
                    .ToUpperInvariant();
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

        private static string FormatTerm(string semester, string academicYear)
        {
            string term = string.IsNullOrWhiteSpace(semester)
                ? string.Empty
                : "Học kỳ " + semester.Trim();
            return string.IsNullOrWhiteSpace(academicYear)
                ? term
                : term + (term.Length == 0 ? string.Empty : " ") +
                    academicYear.Trim();
        }

        private static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value ?? string.Empty);
        }
    }

    public class AdminUserRow
    {
        public string Number { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Initials { get; set; }
        public string Email { get; set; }
        public string RoleKey { get; set; }
        public string RoleLabel { get; set; }
        public string RoleCss { get; set; }
        public string StatusKey { get; set; }
        public string StatusLabel { get; set; }
        public string StatusCss { get; set; }
        public string SearchText { get; set; }
    }

    public class AdminCourseRow
    {
        public string CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Credits { get; set; }
        public string InstructorName { get; set; }
        public string WeekCount { get; set; }
        public string LessonCount { get; set; }
        public string EnrollmentCount { get; set; }
        public string StatusLabel { get; set; }
        public string StatusCss { get; set; }
        public string SearchText { get; set; }
    }

    public class AdminActivityRow
    {
        public string UserName { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }
        public string Icon { get; set; }
        public string IconCss { get; set; }
        public string SortKey { get; set; }
        public string IsAlert { get; set; }
    }

    public class AdminRoleRow
    {
        public string Role { get; set; }
        public string RoleLabel { get; set; }
        public string Description { get; set; }
        public string Count { get; set; }
        public string RoleCss { get; set; }
    }
}