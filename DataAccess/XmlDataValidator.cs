using System;
using System.Collections.Generic;
using System.Xml;

namespace EduLearn.DataAccess
{
    public static class XmlDataValidator
    {
        // Lấy giá trị của phần tử con.
        private static string GetValue(XmlNode node, string elementName)
        {
            if (node == null || node[elementName] == null)
            {
                return string.Empty;
            }

            return node[elementName].InnerText.Trim();
        }

        // Kiểm tra mã có bị trùng hay không.
        private static void CheckDuplicate(
            string id,
            string type,
            HashSet<string> ids,
            List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                errors.Add(type + ": Không được để trống mã.");
                return;
            }

            if (!ids.Add(id))
            {
                errors.Add(type + ": Mã " + id + " bị trùng.");
            }
        }

        // Kiểm tra phần tử gốc của XML.
        private static void CheckRoot(
            XmlDocument document,
            string expectedRoot,
            string fileName,
            List<string> errors)
        {
            if (document.DocumentElement == null ||
                document.DocumentElement.Name != expectedRoot)
            {
                errors.Add(
                    fileName + ": Phần tử gốc phải là <" +
                    expectedRoot + ">."
                );
            }
        }

        private static bool IsValidValue(
            string value,
            params string[] allowedValues)
        {
            foreach (string allowed in allowedValues)
            {
                if (string.Equals(
                    value,
                    allowed,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsId(
            HashSet<string> ids,
            string id)
        {
            return !string.IsNullOrWhiteSpace(id) && ids.Contains(id);
        }

        // Kiểm tra toàn bộ 12 tệp XML.
        public static List<string> ValidateAll()
        {
            List<string> errors = new List<string>();

            try
            {
                // Đọc toàn bộ XML.
                XmlDocument users =
                    XmlDataAccess.LoadXml("Users.xml");

                XmlDocument instructorProfiles =
                    XmlDataAccess.LoadXml("InstructorProfiles.xml");

                XmlDocument categories =
                    XmlDataAccess.LoadXml("Categories.xml");

                XmlDocument courses =
                    XmlDataAccess.LoadXml("Courses.xml");

                XmlDocument enrollments =
                    XmlDataAccess.LoadXml("Enrollments.xml");

                XmlDocument weeks =
                    XmlDataAccess.LoadXml("Weeks.xml");

                XmlDocument lessons =
                    XmlDataAccess.LoadXml("Lessons.xml");

                XmlDocument lessonResources =
                    XmlDataAccess.LoadXml("LessonResources.xml");

                XmlDocument lessonProgress =
                    XmlDataAccess.LoadXml("LessonProgress.xml");

                XmlDocument notifications =
                    XmlDataAccess.LoadXml("Notifications.xml");

                XmlDocument activityLogs =
                    XmlDataAccess.LoadXml("ActivityLogs.xml");

                XmlDocument releaseSchedules =
                    XmlDataAccess.LoadXml("ReleaseSchedules.xml");

                // Kiểm tra tên phần tử gốc.
                CheckRoot(users, "Users", "Users.xml", errors);
                CheckRoot(instructorProfiles, "InstructorProfiles",
                    "InstructorProfiles.xml", errors);
                CheckRoot(categories, "Categories", "Categories.xml", errors);
                CheckRoot(courses, "Courses", "Courses.xml", errors);
                CheckRoot(enrollments, "Enrollments", "Enrollments.xml", errors);
                CheckRoot(weeks, "Weeks", "Weeks.xml", errors);
                CheckRoot(lessons, "Lessons", "Lessons.xml", errors);
                CheckRoot(lessonResources, "LessonResources",
                    "LessonResources.xml", errors);
                CheckRoot(lessonProgress, "LessonProgresses",
                    "LessonProgress.xml", errors);
                CheckRoot(notifications, "Notifications",
                    "Notifications.xml", errors);
                CheckRoot(activityLogs, "ActivityLogs",
                    "ActivityLogs.xml", errors);
                CheckRoot(releaseSchedules, "ReleaseSchedules",
                    "ReleaseSchedules.xml", errors);

                // Các tập hợp mã định danh.
                HashSet<string> userIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> categoryIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> courseIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> enrollmentIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> weekIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> lessonIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> resourceIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> progressIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> notificationIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> activityIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> scheduleIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // Lưu vai trò của tài khoản.
                Dictionary<string, string> userRoles =
                    new Dictionary<string, string>(
                        StringComparer.OrdinalIgnoreCase);

                // Lưu môn học tương ứng với từng tuần.
                Dictionary<string, string> weekCourses =
                    new Dictionary<string, string>(
                        StringComparer.OrdinalIgnoreCase);

                // Lưu môn học tương ứng với từng bài giảng.
                Dictionary<string, string> lessonCourses =
                    new Dictionary<string, string>(
                        StringComparer.OrdinalIgnoreCase);

                // Lưu số tuần của từng môn học.
                Dictionary<string, int> courseWeekCounts =
                    new Dictionary<string, int>(
                        StringComparer.OrdinalIgnoreCase);

                // 1. USERS.XML
                foreach (XmlNode node in users.SelectNodes("/Users/User"))
                {
                    string id = GetValue(node, "UserId");
                    string role = GetValue(node, "Role");
                    string status = GetValue(node, "Status");

                    CheckDuplicate(id, "Tài khoản", userIds, errors);

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        continue;
                    }

                    if (!IsValidValue(role,
                        "Student", "Instructor", "Admin"))
                    {
                        errors.Add(
                            "Tài khoản " + id +
                            ": Vai trò không hợp lệ."
                        );
                    }
                    else
                    {
                        userRoles[id] = role;
                    }

                    if (string.IsNullOrWhiteSpace(
                        GetValue(node, "Username")))
                    {
                        errors.Add(
                            "Tài khoản " + id +
                            ": Username không được để trống."
                        );
                    }

                    if (string.IsNullOrWhiteSpace(
                        GetValue(node, "FullName")))
                    {
                        errors.Add(
                            "Tài khoản " + id +
                            ": Họ tên không được để trống."
                        );
                    }

                    if (string.IsNullOrWhiteSpace(
                        GetValue(node, "Email")))
                    {
                        errors.Add(
                            "Tài khoản " + id +
                            ": Email không được để trống."
                        );
                    }

                    if (!IsValidValue(status,
                        "Active", "Inactive", "Locked"))
                    {
                        errors.Add(
                            "Tài khoản " + id +
                            ": Trạng thái không hợp lệ."
                        );
                    }
                }

                // 2. INSTRUCTOR PROFILES.XML
                HashSet<string> instructorProfileIds =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                HashSet<string> instructorProfileUsers =
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (XmlNode node in instructorProfiles.SelectNodes(
                    "/InstructorProfiles/InstructorProfile"))
                {
                    string id = GetValue(node, "InstructorProfileId");
                    string userId = GetValue(node, "UserId");
                    CheckDuplicate(
                        id,
                        "Hồ sơ giảng viên",
                        instructorProfileIds,
                        errors
                    );

                    if (!ContainsId(userIds, userId))
                    {
                        errors.Add(
                            "Hồ sơ giảng viên " + id +
                            ": Không tìm thấy tài khoản " + userId + "."
                        );
                    }
                    else if (!userRoles.ContainsKey(userId) ||
                        userRoles[userId] != "Instructor")
                    {
                        errors.Add(
                            "Hồ sơ giảng viên " + id +
                            ": Tài khoản liên kết không có vai trò Instructor."
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(userId) &&
                        !instructorProfileUsers.Add(userId))
                    {
                        errors.Add(
                            "Tài khoản " + userId +
                            ": Bị trùng hồ sơ giảng viên."
                        );
                    }
                }

                // 3. CATEGORIES.XML
                foreach (XmlNode node in categories.SelectNodes(
                    "/Categories/Category"))
                {
                    string id = GetValue(node, "CategoryId");

                    CheckDuplicate(id, "Danh mục", categoryIds, errors);

                    if (!string.IsNullOrWhiteSpace(id) &&
                        string.IsNullOrWhiteSpace(
                            GetValue(node, "CategoryName")))
                    {
                        errors.Add(
                            "Danh mục " + id +
                            ": Tên danh mục không được để trống."
                        );
                    }
                }

                // 4. COURSES.XML
                foreach (XmlNode node in courses.SelectNodes(
                    "/Courses/Course"))
                {
                    string id = GetValue(node, "CourseId");
                    string categoryId = GetValue(node, "CategoryId");
                    string instructorId = GetValue(node, "InstructorId");

                    CheckDuplicate(id, "Môn học", courseIds, errors);

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        continue;
                    }

                    if (!ContainsId(categoryIds, categoryId))
                    {
                        errors.Add(
                            "Môn học " + id +
                            ": Không tìm thấy danh mục " + categoryId + "."
                        );
                    }

                    if (!userRoles.ContainsKey(instructorId))
                    {
                        errors.Add(
                            "Môn học " + id +
                            ": Không tìm thấy giảng viên " +
                            instructorId + "."
                        );
                    }
                    else if (userRoles[instructorId] != "Instructor")
                    {
                        errors.Add(
                            "Môn học " + id +
                            ": Tài khoản phụ trách không có vai trò Instructor."
                        );
                    }

                    if (string.IsNullOrWhiteSpace(
                        GetValue(node, "CourseName")))
                    {
                        errors.Add(
                            "Môn học " + id +
                            ": Tên môn học không được để trống."
                        );
                    }
                }

                // 5. ENROLLMENTS.XML
                foreach (XmlNode node in enrollments.SelectNodes(
                    "/Enrollments/Enrollment"))
                {
                    string id = GetValue(node, "EnrollmentId");
                    string studentId = GetValue(node, "StudentId");
                    string courseId = GetValue(node, "CourseId");
                    string status = GetValue(node, "Status");

                    CheckDuplicate(id, "Đăng ký môn học",
                        enrollmentIds, errors);

                    if (!userRoles.ContainsKey(studentId))
                    {
                        errors.Add(
                            "Đăng ký " + id +
                            ": Không tìm thấy tài khoản " + studentId + "."
                        );
                    }
                    else if (userRoles[studentId] != "Student")
                    {
                        errors.Add(
                            "Đăng ký " + id +
                            ": Người đăng ký không có vai trò Student."
                        );
                    }

                    if (!ContainsId(courseIds, courseId))
                    {
                        errors.Add(
                            "Đăng ký " + id +
                            ": Không tìm thấy môn học " + courseId + "."
                        );
                    }

                    if (!IsValidValue(status,
                        "Active", "Completed", "Cancelled"))
                    {
                        errors.Add(
                            "Đăng ký " + id +
                            ": Trạng thái không hợp lệ."
                        );
                    }
                }

                // 6. WEEKS.XML
                foreach (XmlNode node in weeks.SelectNodes("/Weeks/Week"))
                {
                    string id = GetValue(node, "WeekId");
                    string courseId = GetValue(node, "CourseId");
                    string status = GetValue(node, "Status");

                    CheckDuplicate(id, "Tuần học", weekIds, errors);

                    if (!ContainsId(courseIds, courseId))
                    {
                        errors.Add(
                            "Tuần học " + id +
                            ": Không tìm thấy môn học " + courseId + "."
                        );
                    }

                    if (!IsValidValue(status,
                        "Available", "Locked", "Completed"))
                    {
                        errors.Add(
                            "Tuần học " + id +
                            ": Trạng thái không hợp lệ."
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        weekCourses[id] = courseId;
                    }

                    if (!string.IsNullOrWhiteSpace(courseId))
                    {
                        if (!courseWeekCounts.ContainsKey(courseId))
                        {
                            courseWeekCounts[courseId] = 0;
                        }

                        courseWeekCounts[courseId]++;
                    }
                }

                // Kiểm tra số tuần thực tế so với TotalWeeks.
                foreach (XmlNode node in courses.SelectNodes(
                    "/Courses/Course"))
                {
                    string courseId = GetValue(node, "CourseId");
                    string totalWeeksText = GetValue(node, "TotalWeeks");

                    int totalWeeks;

                    if (int.TryParse(totalWeeksText, out totalWeeks) &&
                        totalWeeks > 0)
                    {
                        int actualWeeks = 0;

                        if (courseWeekCounts.ContainsKey(courseId))
                        {
                            actualWeeks = courseWeekCounts[courseId];
                        }

                        if (actualWeeks != totalWeeks)
                        {
                            errors.Add(
                                "Môn học " + courseId +
                                ": TotalWeeks là " + totalWeeks +
                                " nhưng dữ liệu có " + actualWeeks + " tuần."
                            );
                        }
                    }
                }

                // 7. LESSONS.XML
                foreach (XmlNode node in lessons.SelectNodes(
                    "/Lessons/Lesson"))
                {
                    string id = GetValue(node, "LessonId");
                    string weekId = GetValue(node, "WeekId");
                    string status = GetValue(node, "Status");
                    string lessonType = GetValue(node, "LessonType");
                    string createdBy = GetValue(node, "CreatedBy");
                    string updatedBy = GetValue(node, "UpdatedBy");

                    CheckDuplicate(id, "Bài giảng", lessonIds, errors);

                    string courseId = string.Empty;

                    if (!weekCourses.ContainsKey(weekId))
                    {
                        errors.Add(
                            "Bài giảng " + id +
                            ": Không tìm thấy tuần học " + weekId + "."
                        );
                    }
                    else
                    {
                        courseId = weekCourses[weekId];
                    }

                    if (!IsValidValue(status, "Draft", "Published"))
                    {
                        errors.Add(
                            "Bài giảng " + id +
                            ": Trạng thái không hợp lệ."
                        );
                    }

                    if (!IsValidValue(lessonType,
                        "Video", "Exercise", "Document"))
                    {
                        errors.Add(
                            "Bài giảng " + id +
                            ": Loại bài giảng không hợp lệ."
                        );
                    }

                    if (!userRoles.ContainsKey(createdBy) ||
                        userRoles[createdBy] != "Instructor")
                    {
                        errors.Add(
                            "Bài giảng " + id +
                            ": Người tạo không phải giảng viên hợp lệ."
                        );
                    }

                    if (!userRoles.ContainsKey(updatedBy) ||
                        userRoles[updatedBy] != "Instructor")
                    {
                        errors.Add(
                            "Bài giảng " + id +
                            ": Người cập nhật không phải giảng viên hợp lệ."
                        );
                    }

                    if (string.IsNullOrWhiteSpace(
                        GetValue(node, "LessonTitle")))
                    {
                        errors.Add(
                            "Bài giảng " + id +
                            ": Tiêu đề không được để trống."
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(id) &&
                        !string.IsNullOrWhiteSpace(courseId))
                    {
                        lessonCourses[id] = courseId;
                    }
                }

                // 8. LESSONRESOURCES.XML
                foreach (XmlNode node in lessonResources.SelectNodes(
                    "/LessonResources/Resource"))
                {
                    string id = GetValue(node, "ResourceId");
                    string lessonId = GetValue(node, "LessonId");
                    string type = GetValue(node, "ContentItemType");
                    string uploadedBy = GetValue(node, "UploadedBy");

                    CheckDuplicate(
                        id, "Tài nguyên bài giảng",
                        resourceIds, errors
                    );

                    if (!ContainsId(lessonIds, lessonId))
                    {
                        errors.Add(
                            "Tài nguyên " + id +
                            ": Không tìm thấy bài giảng " + lessonId + "."
                        );
                    }

                    if (!IsValidValue(type,
                        "Video", "Exercise", "Document"))
                    {
                        errors.Add(
                            "Tài nguyên " + id +
                            ": Loại nội dung không hợp lệ."
                        );
                    }

                    if (!userRoles.ContainsKey(uploadedBy) ||
                        userRoles[uploadedBy] != "Instructor")
                    {
                        errors.Add(
                            "Tài nguyên " + id +
                            ": Người tải lên không phải giảng viên hợp lệ."
                        );
                    }
                }

                // 9. LESSONPROGRESS.XML
                HashSet<string> studentLessonPairs =
                    new HashSet<string>(
                        StringComparer.OrdinalIgnoreCase);

                foreach (XmlNode node in lessonProgress.SelectNodes(
                    "/LessonProgresses/Progress"))
                {
                    string id = GetValue(node, "ProgressId");
                    string studentId = GetValue(node, "StudentId");
                    string lessonId = GetValue(node, "LessonId");
                    string status = GetValue(node, "Status");
                    string percentText = GetValue(node, "ProgressPercent");

                    CheckDuplicate(
                        id, "Tiến độ bài giảng",
                        progressIds, errors
                    );

                    if (!userRoles.ContainsKey(studentId))
                    {
                        errors.Add(
                            "Tiến độ " + id +
                            ": Không tìm thấy tài khoản " + studentId + "."
                        );
                    }
                    else if (userRoles[studentId] != "Student")
                    {
                        errors.Add(
                            "Tiến độ " + id +
                            ": Tài khoản liên kết không phải Student."
                        );
                    }

                    if (!ContainsId(lessonIds, lessonId))
                    {
                        errors.Add(
                            "Tiến độ " + id +
                            ": Không tìm thấy bài giảng " + lessonId + "."
                        );
                    }

                    if (!IsValidValue(status,
                        "NotStarted", "InProgress", "Completed"))
                    {
                        errors.Add(
                            "Tiến độ " + id +
                            ": Trạng thái không hợp lệ."
                        );
                    }

                    int percent;

                    if (!int.TryParse(percentText, out percent) ||
                        percent < 0 || percent > 100)
                    {
                        errors.Add(
                            "Tiến độ " + id +
                            ": ProgressPercent phải nằm trong khoảng 0–100."
                        );
                    }

                    string pairKey = studentId + "|" + lessonId;

                    if (!studentLessonPairs.Add(pairKey))
                    {
                        errors.Add(
                            "Tiến độ: Sinh viên " + studentId +
                            " bị trùng tiến độ của bài giảng " + lessonId + "."
                        );
                    }
                }

                // 10. NOTIFICATIONS.XML
                foreach (XmlNode node in notifications.SelectNodes(
                    "/Notifications/Notification"))
                {
                    string id = GetValue(node, "NotificationId");
                    string recipientId = GetValue(node, "RecipientId");
                    string senderId = GetValue(node, "SenderId");
                    string courseId = GetValue(node, "RelatedCourseId");
                    string lessonId = GetValue(node, "RelatedLessonId");

                    CheckDuplicate(
                        id, "Thông báo",
                        notificationIds, errors
                    );

                    if (!ContainsId(userIds, recipientId))
                    {
                        errors.Add(
                            "Thông báo " + id +
                            ": Không tìm thấy người nhận " + recipientId + "."
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(senderId) &&
                        senderId != "System" &&
                        !ContainsId(userIds, senderId))
                    {
                        errors.Add(
                            "Thông báo " + id +
                            ": Không tìm thấy người gửi " + senderId + "."
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(courseId) &&
                        !ContainsId(courseIds, courseId))
                    {
                        errors.Add(
                            "Thông báo " + id +
                            ": Không tìm thấy môn học liên quan " +
                            courseId + "."
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(lessonId))
                    {
                        if (!lessonCourses.ContainsKey(lessonId))
                        {
                            errors.Add(
                                "Thông báo " + id +
                                ": Không tìm thấy bài giảng liên quan " +
                                lessonId + "."
                            );
                        }
                        else if (!string.IsNullOrWhiteSpace(courseId) &&
                            lessonCourses[lessonId] != courseId)
                        {
                            errors.Add(
                                "Thông báo " + id +
                                ": Bài giảng không thuộc môn học liên quan."
                            );
                        }
                    }
                }

                // 11. ACTIVITYLOGS.XML
                foreach (XmlNode node in activityLogs.SelectNodes(
                    "/ActivityLogs/Activity"))
                {
                    string id = GetValue(node, "ActivityId");
                    string userId = GetValue(node, "UserId");

                    CheckDuplicate(
                        id, "Nhật ký hoạt động",
                        activityIds, errors
                    );

                    if (!ContainsId(userIds, userId))
                    {
                        errors.Add(
                            "Nhật ký " + id +
                            ": Không tìm thấy tài khoản " + userId + "."
                        );
                    }
                }

                // 12. RELEASESCHEDULES.XML
                foreach (XmlNode node in releaseSchedules.SelectNodes(
                    "/ReleaseSchedules/Schedule"))
                {
                    string id = GetValue(node, "ScheduleId");
                    string courseId = GetValue(node, "CourseId");
                    string weekId = GetValue(node, "WeekId");
                    string lessonId = GetValue(node, "LessonId");
                    string scheduleType = GetValue(node, "ScheduleType");
                    string status = GetValue(node, "Status");
                    string createdBy = GetValue(node, "CreatedBy");

                    CheckDuplicate(
                        id, "Lịch phát hành",
                        scheduleIds, errors
                    );

                    bool hasWeek = !string.IsNullOrWhiteSpace(weekId);
                    bool hasLesson = !string.IsNullOrWhiteSpace(lessonId);

                    if (hasWeek == hasLesson)
                    {
                        errors.Add(
                            "Lịch phát hành " + id +
                            ": Phải chọn đúng một trong hai mục WeekId hoặc LessonId."
                        );
                    }

                    if (!ContainsId(courseIds, courseId))
                    {
                        errors.Add(
                            "Lịch phát hành " + id +
                            ": Không tìm thấy môn học " + courseId + "."
                        );
                    }

                    if (hasWeek)
                    {
                        if (!weekCourses.ContainsKey(weekId))
                        {
                            errors.Add(
                                "Lịch phát hành " + id +
                                ": Không tìm thấy tuần học " + weekId + "."
                            );
                        }
                        else if (weekCourses[weekId] != courseId)
                        {
                            errors.Add(
                                "Lịch phát hành " + id +
                                ": Tuần học không thuộc môn học đã chọn."
                            );
                        }
                    }

                    if (hasLesson)
                    {
                        if (!lessonCourses.ContainsKey(lessonId))
                        {
                            errors.Add(
                                "Lịch phát hành " + id +
                                ": Không tìm thấy bài giảng " + lessonId + "."
                            );
                        }
                        else if (lessonCourses[lessonId] != courseId)
                        {
                            errors.Add(
                                "Lịch phát hành " + id +
                                ": Bài giảng không thuộc môn học đã chọn."
                            );
                        }
                    }

                    if (!IsValidValue(scheduleType,
                        "WholeWeek", "SingleLesson"))
                    {
                        errors.Add(
                            "Lịch phát hành " + id +
                            ": ScheduleType không hợp lệ."
                        );
                    }

                    if (hasWeek && scheduleType != "WholeWeek")
                    {
                        errors.Add(
                            "Lịch phát hành " + id +
                            ": Có WeekId thì ScheduleType phải là WholeWeek."
                        );
                    }

                    if (hasLesson && scheduleType != "SingleLesson")
                    {
                        errors.Add(
                            "Lịch phát hành " + id +
                            ": Có LessonId thì ScheduleType phải là SingleLesson."
                        );
                    }

                    if (!IsValidValue(status,
                        "Pending", "Executed", "Cancelled", "Failed"))
                    {
                        errors.Add(
                            "Lịch phát hành " + id +
                            ": Trạng thái không hợp lệ."
                        );
                    }

                    if (!userRoles.ContainsKey(createdBy) ||
                        userRoles[createdBy] != "Instructor")
                    {
                        errors.Add(
                            "Lịch phát hành " + id +
                            ": Người tạo không phải giảng viên hợp lệ."
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add(
                    "Lỗi đọc hoặc kiểm tra XML: " + ex.Message
                );
            }

            return errors;
        }
    }
}