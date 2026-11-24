using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Xml.Linq;
using EduLearn.Models;
using EduLearn.Services;

namespace EduLearn.Instructor.Data
{
    public class GvProfile
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string Degree { get; set; }
        public string Bio { get; set; }
    }

    public class CourseItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Status { get; set; }
        public int StudentCount { get; set; }
        public int Weeks { get; set; }
        public int LectureCount { get; set; }
        public int PublishedCount { get; set; }
        public int Progress { get; set; }
    }

    public class LectureItem
    {
        public string Id { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public int Week { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }       // Video | Tài liệu | Bài tập
        public string Duration { get; set; }
        public string VideoUrl { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }     // Đã xuất bản | Bản nháp
        public DateTime UpdatedAt { get; set; }
    }

    public class NotificationItem
    {
        public string Id { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class ActivityItem
    {
        public string Type { get; set; }       // created | updated | published | assigned
        public string Title { get; set; }
        public string Subject { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; }
        public string Meta { get; set; }
    }

    public class StudentEnrolledItem
    {
        public string StudentCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int CompletedPercent { get; set; }
        public string Status { get; set; }
    }

    public static class InstructorStore
    {
        private const string XML_PATH = "~/App_Data/InstructorData.xml";
        private const string USERS_XML_PATH = "~/App_Data/Users.xml";
        private const string THUMB_FOLDER = "~/anhimg/";
        private static readonly string[] THUMB_EXT = { ".png", ".jpg", ".jpeg", ".webp" };

        public static string CurrentKey()
        {
            User user = AuthSession.GetCurrentUser();
            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(user.Username)) return user.Username.Trim();
                if (!string.IsNullOrWhiteSpace(user.Email)) return user.Email.Trim();
            }
            string idName = HttpContext.Current != null && HttpContext.Current.User != null
                ? HttpContext.Current.User.Identity.Name : string.Empty;
            return string.IsNullOrWhiteSpace(idName) ? "an.instructor" : idName.Trim();
        }

        public static string CurrentFullName()
        {
            User user = AuthSession.GetCurrentUser();
            return (user != null && !string.IsNullOrWhiteSpace(user.FullName)) ? user.FullName.Trim() : "Giảng viên";
        }

        // Tự động tạo file InstructorData.xml mẫu nếu chưa tồn tại
        private static XDocument EnsureDocument()
        {
            string path = HttpContext.Current.Server.MapPath(XML_PATH);
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!File.Exists(path))
            {
                XDocument seed = BuildSeedDocument("an.instructor", "Trần Quốc An", "an.instructor@edulearn.vn");
                seed.Save(path);
                return seed;
            }

            return XDocument.Load(path);
        }

        private static void SaveDocument(XDocument doc)
        {
            string path = HttpContext.Current.Server.MapPath(XML_PATH);
            doc.Save(path);
        }

        private static XDocument BuildSeedDocument(string owner, string fullName, string email)
        {
            DateTime now = DateTime.Now;
            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("InstructorData",
                    new XElement("Profiles",
                        new XElement("Profile",
                            new XElement("InstructorUsername", owner),
                            new XElement("FullName", fullName),
                            new XElement("Email", email),
                            new XElement("Phone", "0912 345 678"),
                            new XElement("Department", "Khoa Công nghệ thông tin"),
                            new XElement("Degree", "Thạc sĩ Khoa học Máy tính"),
                            new XElement("Bio", "Giảng viên phụ trách các học phần Phát triển Web, Cơ sở dữ liệu và Lập trình hướng đối tượng.")
                        )
                    ),
                    new XElement("Courses",
                        new XElement("Course",
                            new XElement("Id", "LTW"),
                            new XElement("InstructorUsername", owner),
                            new XElement("Name", "Lập trình Web"),
                            new XElement("Category", "Công nghệ thông tin"),
                            new XElement("Description", "Phát triển ứng dụng Web toàn diện với ASP.NET và HTML/CSS."),
                            new XElement("Thumbnail", "LTW"),
                            new XElement("Status", "Đang diễn ra"),
                            new XElement("StudentCount", 48),
                            new XElement("Weeks", 8),
                            new XElement("Progress", 83)
                        ),
                        new XElement("Course",
                            new XElement("Id", "CSDL"),
                            new XElement("InstructorUsername", owner),
                            new XElement("Name", "Cơ sở dữ liệu"),
                            new XElement("Category", "Hệ thống thông tin"),
                            new XElement("Description", "Mô hình hóa quan hệ E-R, đại số quan hệ và chuẩn hóa dữ liệu."),
                            new XElement("Thumbnail", "CSDL"),
                            new XElement("Status", "Đang diễn ra"),
                            new XElement("StudentCount", 45),
                            new XElement("Weeks", 10),
                            new XElement("Progress", 85)
                        ),
                        new XElement("Course",
                            new XElement("Id", "JAVA"),
                            new XElement("InstructorUsername", owner),
                            new XElement("Name", "Lập trình hướng đối tượng (OOP)"),
                            new XElement("Category", "Khoa học máy tính"),
                            new XElement("Description", "Nguyên lý thiết kế phần mềm hướng đối tượng với Java và C#."),
                            new XElement("Thumbnail", "javanangcao"),
                            new XElement("Status", "Đang diễn ra"),
                            new XElement("StudentCount", 35),
                            new XElement("Weeks", 9),
                            new XElement("Progress", 80)
                        ),
                        new XElement("Course",
                            new XElement("Id", "NET"),
                            new XElement("InstructorUsername", owner),
                            new XElement("Name", "Mạng máy tính & IoT căn bản"),
                            new XElement("Category", "Công nghệ thông tin"),
                            new XElement("Description", "Kiến thức nền tảng về giao thức mạng TCP/IP và hệ thống IoT."),
                            new XElement("Thumbnail", "mangmaytinh"),
                            new XElement("Status", "Sắp diễn ra"),
                            new XElement("StudentCount", 0),
                            new XElement("Weeks", 8),
                            new XElement("Progress", 0)
                        )
                    ),
                    new XElement("Lectures",
                        new XElement("Lecture",
                            new XElement("Id", "LEC001"),
                            new XElement("CourseId", "LTW"),
                            new XElement("InstructorUsername", owner),
                            new XElement("Week", 1),
                            new XElement("Title", "Tổng quan kiến trúc Web & ASP.NET Web Forms"),
                            new XElement("Type", "Video"),
                            new XElement("Duration", "45 phút"),
                            new XElement("VideoUrl", "https://www.youtube.com/watch?v=demo1"),
                            new XElement("Content", "Giới thiệu mô hình Client-Server, vòng đời trang ASP.NET và cấu trúc dự án."),
                            new XElement("Status", "Đã xuất bản"),
                            new XElement("UpdatedAt", now.AddDays(-10).ToString("s"))
                        ),
                        new XElement("Lecture",
                            new XElement("Id", "LEC002"),
                            new XElement("CourseId", "LTW"),
                            new XElement("InstructorUsername", owner),
                            new XElement("Week", 4),
                            new XElement("Title", "Tuần 4: Thiết kế Responsive với CSS Grid & Flexbox"),
                            new XElement("Type", "Video"),
                            new XElement("Duration", "55 phút"),
                            new XElement("VideoUrl", "https://www.youtube.com/watch?v=demo2"),
                            new XElement("Content", "Xây dựng bố cục đa thiết bị với Flexbox, CSS Grid và Media Queries."),
                            new XElement("Status", "Bản nháp"),
                            new XElement("UpdatedAt", now.AddMinutes(-15).ToString("s"))
                        ),
                        new XElement("Lecture",
                            new XElement("Id", "LEC003"),
                            new XElement("CourseId", "CSDL"),
                            new XElement("InstructorUsername", owner),
                            new XElement("Week", 2),
                            new XElement("Title", "Tuần 2: Chuẩn hóa dữ liệu 3NF và BCNF"),
                            new XElement("Type", "Tài liệu"),
                            new XElement("Duration", "40 phút"),
                            new XElement("VideoUrl", ""),
                            new XElement("Content", "Phụ thuộc hàm, khóa ứng viên và các bước tách lược đồ đạt chuẩn 3NF/BCNF."),
                            new XElement("Status", "Đã xuất bản"),
                            new XElement("UpdatedAt", now.AddHours(-2).ToString("s"))
                        ),
                        new XElement("Lecture",
                            new XElement("Id", "LEC004"),
                            new XElement("CourseId", "JAVA"),
                            new XElement("InstructorUsername", owner),
                            new XElement("Week", 3),
                            new XElement("Title", "Tuần 3: Kế thừa và Đa hình trong Java"),
                            new XElement("Type", "Bài tập"),
                            new XElement("Duration", "60 phút"),
                            new XElement("VideoUrl", ""),
                            new XElement("Content", "Thực hành ghi đè phương thức, lớp trừu tượng và giao diện Interface."),
                            new XElement("Status", "Đã xuất bản"),
                            new XElement("UpdatedAt", now.AddDays(-1).ToString("s"))
                        )
                    ),
                    new XElement("Notifications",
                        new XElement("Notification",
                            new XElement("Id", "NOTI01"),
                            new XElement("InstructorUsername", owner),
                            new XElement("CourseId", "LTW"),
                            new XElement("Title", "Lịch nộp bài thực hành Tuần 4 - Lập trình Web"),
                            new XElement("Message", "Các nhóm hoàn thành giao diện Responsive và nộp mã nguồn lên hệ thống trước 23:59 Chủ nhật."),
                            new XElement("CreatedAt", now.AddHours(-3).ToString("s")),
                            new XElement("IsRead", "false")
                        ),
                        new XElement("Notification",
                            new XElement("Id", "NOTI02"),
                            new XElement("InstructorUsername", owner),
                            new XElement("CourseId", "SYSTEM"),
                            new XElement("Title", "Phân công giảng dạy Học kỳ 1 năm học 2026 - 2027"),
                            new XElement("Message", "Phòng Đào tạo đã cập nhật danh sách lớp học phần chính thức của giảng viên."),
                            new XElement("CreatedAt", now.AddDays(-3).ToString("s")),
                            new XElement("IsRead", "true")
                        )
                    ),
                    new XElement("Activities",
                        new XElement("Activity",
                            new XElement("InstructorUsername", owner),
                            new XElement("Type", "created"),
                            new XElement("Title", "Bài giảng mới được tạo: “Tuần 4: Thiết kế Responsive với CSS Grid & Flexbox”"),
                            new XElement("Subject", "Môn: Lập trình Web"),
                            new XElement("Time", now.AddMinutes(-15).ToString("s")),
                            new XElement("Status", "Bản nháp")
                        ),
                        new XElement("Activity",
                            new XElement("InstructorUsername", owner),
                            new XElement("Type", "updated"),
                            new XElement("Title", "Bài giảng vừa được cập nhật: “Tuần 2: Chuẩn hóa dữ liệu 3NF và BCNF”"),
                            new XElement("Subject", "Môn: Cơ sở dữ liệu"),
                            new XElement("Time", now.AddHours(-2).ToString("s")),
                            new XElement("Status", "Đã lưu")
                        ),
                        new XElement("Activity",
                            new XElement("InstructorUsername", owner),
                            new XElement("Type", "published"),
                            new XElement("Title", "Bài giảng vừa được xuất bản: “Tuần 3: Kế thừa và Đa hình trong Java”"),
                            new XElement("Subject", "Môn: Lập trình hướng đối tượng (OOP)"),
                            new XElement("Time", now.AddDays(-1).ToString("s")),
                            new XElement("Status", "Đã công khai")
                        ),
                        new XElement("Activity",
                            new XElement("InstructorUsername", owner),
                            new XElement("Type", "assigned"),
                            new XElement("Title", "Môn học vừa được phân công: Phân công giảng dạy học kỳ 1 năm học 2026 - 2027"),
                            new XElement("Subject", "Khoa Công nghệ thông tin"),
                            new XElement("Time", now.AddDays(-3).ToString("s")),
                            new XElement("Status", "Chính thức")
                        )
                    )
                )
            );
        }

        // ====================================================================
        //  1. HỒ SƠ GIẢNG VIÊN & ĐỔI MẬT KHẨU
        // ====================================================================
        public static GvProfile GetProfile(string key, string fallbackFullName)
        {
            User sessionUser = AuthSession.GetCurrentUser();
            var profile = new GvProfile
            {
                Username = key,
                FullName = !string.IsNullOrWhiteSpace(fallbackFullName) ? fallbackFullName.Trim() : "Giảng viên",
                Email = sessionUser != null ? (sessionUser.Email ?? string.Empty) : string.Empty,
                Phone = sessionUser != null ? (sessionUser.Phone ?? string.Empty) : string.Empty,
                Department = "Khoa Công nghệ thông tin",
                Degree = "Thạc sĩ",
                Bio = string.Empty
            };

            XDocument doc = EnsureDocument();
            XElement profilesNode = doc.Root.Element("Profiles");
            if (profilesNode == null) return profile;

            XElement node = profilesNode.Elements("Profile").FirstOrDefault(p =>
                string.Equals(Str(p, "InstructorUsername"), key, StringComparison.OrdinalIgnoreCase));

            if (node != null)
            {
                string xmlName = Str(node, "FullName");
                if (!string.IsNullOrEmpty(xmlName)) profile.FullName = xmlName;
                string xmlEmail = Str(node, "Email");
                if (!string.IsNullOrEmpty(xmlEmail)) profile.Email = xmlEmail;
                profile.Phone = Str(node, "Phone");
                profile.Department = string.IsNullOrEmpty(Str(node, "Department")) ? profile.Department : Str(node, "Department");
                profile.Degree = string.IsNullOrEmpty(Str(node, "Degree")) ? profile.Degree : Str(node, "Degree");
                profile.Bio = Str(node, "Bio");
            }
            return profile;
        }

        public static void SaveProfile(string key, GvProfile updated)
        {
            XDocument doc = EnsureDocument();
            XElement profilesNode = doc.Root.Element("Profiles");
            if (profilesNode == null)
            {
                profilesNode = new XElement("Profiles");
                doc.Root.Add(profilesNode);
            }

            XElement node = profilesNode.Elements("Profile").FirstOrDefault(p =>
                string.Equals(Str(p, "InstructorUsername"), key, StringComparison.OrdinalIgnoreCase));

            if (node == null)
            {
                node = new XElement("Profile", new XElement("InstructorUsername", key));
                profilesNode.Add(node);
            }

            SetVal(node, "FullName", updated.FullName);
            SetVal(node, "Email", updated.Email);
            SetVal(node, "Phone", updated.Phone);
            SetVal(node, "Department", updated.Department);
            SetVal(node, "Degree", updated.Degree);
            SetVal(node, "Bio", updated.Bio);
            SaveDocument(doc);

            // Đồng bộ sang Users.xml và AuthSession
            try
            {
                string usersPath = HttpContext.Current.Server.MapPath(USERS_XML_PATH);
                if (File.Exists(usersPath))
                {
                    XDocument uDoc = XDocument.Load(usersPath);
                    XElement uNode = uDoc.Root.Elements("User").FirstOrDefault(u =>
                        string.Equals(Str(u, "Username"), key, StringComparison.OrdinalIgnoreCase));
                    if (uNode != null)
                    {
                        SetVal(uNode, "FullName", updated.FullName);
                        SetVal(uNode, "Phone", updated.Phone);
                        SetVal(uNode, "UpdatedAt", DateTime.Now.ToString("s"));
                        uDoc.Save(usersPath);
                    }
                }

                User current = AuthSession.GetCurrentUser();
                if (current != null)
                {
                    current.FullName = updated.FullName;
                    current.Phone = updated.Phone;
                    AuthSession.SetCurrentUser(current);
                }
            }
            catch { }
        }

        public static bool ChangePassword(string key, string oldPassword, string newPassword, out string errorMessage)
        {
            errorMessage = string.Empty;
            string usersPath = HttpContext.Current.Server.MapPath(USERS_XML_PATH);
            if (!File.Exists(usersPath))
            {
                errorMessage = "Không tìm thấy dữ liệu tài khoản (Users.xml).";
                return false;
            }

            XDocument doc = XDocument.Load(usersPath);
            XElement userNode = doc.Root.Elements("User").FirstOrDefault(u =>
                string.Equals(Str(u, "Username"), key, StringComparison.OrdinalIgnoreCase));

            if (userNode == null)
            {
                errorMessage = "Không tìm thấy thông tin tài khoản.";
                return false;
            }

            string storedHash = Str(userNode, "PasswordHash");
            if (!VerifyPbkdf2(oldPassword, storedHash))
            {
                errorMessage = "Mật khẩu hiện tại không chính xác.";
                return false;
            }

            SetVal(userNode, "PasswordHash", HashPbkdf2(newPassword));
            SetVal(userNode, "UpdatedAt", DateTime.Now.ToString("s"));
            doc.Save(usersPath);
            return true;
        }

        // ====================================================================
        //  2. MÔN HỌC
        // ====================================================================
        public static List<CourseItem> GetCourses(string key)
        {
            XDocument doc = EnsureDocument();
            XElement coursesRoot = doc.Root.Element("Courses");
            XElement lecturesRoot = doc.Root.Element("Lectures");
            var list = new List<CourseItem>();
            if (coursesRoot == null) return list;

            var allLectures = lecturesRoot == null
                ? new List<XElement>()
                : lecturesRoot.Elements("Lecture").Where(l => IsOwner(l, key)).ToList();

            foreach (XElement c in coursesRoot.Elements("Course").Where(x => IsOwner(x, key)))
            {
                string id = Str(c, "Id");
                var courseLecs = allLectures.Where(l => string.Equals(Str(l, "CourseId"), id, StringComparison.OrdinalIgnoreCase)).ToList();

                list.Add(new CourseItem
                {
                    Id = id,
                    Name = Str(c, "Name"),
                    Category = Str(c, "Category"),
                    Description = Str(c, "Description"),
                    Thumbnail = Str(c, "Thumbnail"),
                    ThumbnailUrl = ResolveThumb(Str(c, "Thumbnail")),
                    Status = Str(c, "Status"),
                    StudentCount = Int(c, "StudentCount"),
                    Weeks = Int(c, "Weeks"),
                    LectureCount = courseLecs.Count,
                    PublishedCount = courseLecs.Count(l => string.Equals(Str(l, "Status"), "Đã xuất bản", StringComparison.OrdinalIgnoreCase)),
                    Progress = Math.Max(0, Math.Min(100, Int(c, "Progress")))
                });
            }
            return list;
        }

        public static CourseItem GetCourseById(string key, string courseId)
        {
            return GetCourses(key).FirstOrDefault(c => string.Equals(c.Id, courseId, StringComparison.OrdinalIgnoreCase));
        }

        public static void UpdateCourse(string key, string courseId, string description, string status, int progress)
        {
            XDocument doc = EnsureDocument();
            XElement coursesRoot = doc.Root.Element("Courses");
            if (coursesRoot == null) return;

            XElement node = coursesRoot.Elements("Course").FirstOrDefault(c =>
                IsOwner(c, key) && string.Equals(Str(c, "Id"), courseId, StringComparison.OrdinalIgnoreCase));
            if (node == null) return;

            SetVal(node, "Description", description);
            SetVal(node, "Status", status);
            SetVal(node, "Progress", Math.Max(0, Math.Min(100, progress)).ToString(CultureInfo.InvariantCulture));
            SaveDocument(doc);

            AddActivity(key, "updated", "Cập nhật thông tin học phần: “" + Str(node, "Name") + "” (Tiến độ " + progress + "%)", "Môn: " + Str(node, "Name"), "Đã lưu");
        }

        // ====================================================================
        //  3. BÀI GIẢNG (CRUD + XUẤT BẢN)
        // ====================================================================
        public static List<LectureItem> GetLectures(string key, string courseIdFilter = "")
        {
            XDocument doc = EnsureDocument();
            XElement lecturesRoot = doc.Root.Element("Lectures");
            var courses = GetCourses(key).ToDictionary(c => c.Id, c => c.Name, StringComparer.OrdinalIgnoreCase);
            var list = new List<LectureItem>();
            if (lecturesRoot == null) return list;

            foreach (XElement l in lecturesRoot.Elements("Lecture").Where(x => IsOwner(x, key)))
            {
                string cid = Str(l, "CourseId");
                if (!string.IsNullOrWhiteSpace(courseIdFilter) && !string.Equals(cid, courseIdFilter, StringComparison.OrdinalIgnoreCase))
                    continue;

                DateTime dt;
                if (!DateTime.TryParse(Str(l, "UpdatedAt"), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    dt = DateTime.Now;

                list.Add(new LectureItem
                {
                    Id = Str(l, "Id"),
                    CourseId = cid,
                    CourseName = courses.ContainsKey(cid) ? courses[cid] : cid,
                    Week = Int(l, "Week"),
                    Title = Str(l, "Title"),
                    Type = Str(l, "Type"),
                    Duration = Str(l, "Duration"),
                    VideoUrl = Str(l, "VideoUrl"),
                    Content = Str(l, "Content"),
                    Status = Str(l, "Status"),
                    UpdatedAt = dt
                });
            }

            return list.OrderBy(x => x.CourseName).ThenBy(x => x.Week).ThenByDescending(x => x.UpdatedAt).ToList();
        }

        public static LectureItem GetLectureById(string key, string lectureId)
        {
            return GetLectures(key).FirstOrDefault(l => string.Equals(l.Id, lectureId, StringComparison.OrdinalIgnoreCase));
        }

        public static void SaveLecture(string key, LectureItem item)
        {
            XDocument doc = EnsureDocument();
            XElement lecturesRoot = doc.Root.Element("Lectures");
            if (lecturesRoot == null)
            {
                lecturesRoot = new XElement("Lectures");
                doc.Root.Add(lecturesRoot);
            }

            CourseItem course = GetCourseById(key, item.CourseId);
            string courseName = course != null ? course.Name : item.CourseId;

            bool isNew = string.IsNullOrWhiteSpace(item.Id);
            XElement node = null;
            if (!isNew)
            {
                node = lecturesRoot.Elements("Lecture").FirstOrDefault(l =>
                    IsOwner(l, key) && string.Equals(Str(l, "Id"), item.Id, StringComparison.OrdinalIgnoreCase));
            }

            if (node == null)
            {
                isNew = true;
                item.Id = "LEC" + DateTime.Now.ToString("yyMMddHHmmss");
                node = new XElement("Lecture",
                    new XElement("Id", item.Id),
                    new XElement("InstructorUsername", key)
                );
                lecturesRoot.Add(node);
            }

            SetVal(node, "CourseId", item.CourseId);
            SetVal(node, "Week", item.Week.ToString(CultureInfo.InvariantCulture));
            SetVal(node, "Title", item.Title);
            SetVal(node, "Type", item.Type);
            SetVal(node, "Duration", item.Duration);
            SetVal(node, "VideoUrl", item.VideoUrl);
            SetVal(node, "Content", item.Content);
            SetVal(node, "Status", item.Status);
            SetVal(node, "UpdatedAt", DateTime.Now.ToString("s"));
            SaveDocument(doc);

            bool isPublished = string.Equals(item.Status, "Đã xuất bản", StringComparison.OrdinalIgnoreCase);
            string actType = isPublished ? "published" : (isNew ? "created" : "updated");
            string actPrefix = isPublished ? "Bài giảng vừa được xuất bản: " : (isNew ? "Bài giảng mới được tạo: " : "Bài giảng vừa được cập nhật: ");
            string chip = isPublished ? "Đã công khai" : (isNew ? "Bản nháp" : "Đã lưu");

            AddActivity(key, actType, actPrefix + "“" + item.Title + "”", "Môn: " + courseName, chip);
        }

        public static void ToggleLecturePublish(string key, string lectureId)
        {
            XDocument doc = EnsureDocument();
            XElement lecturesRoot = doc.Root.Element("Lectures");
            if (lecturesRoot == null) return;

            XElement node = lecturesRoot.Elements("Lecture").FirstOrDefault(l =>
                IsOwner(l, key) && string.Equals(Str(l, "Id"), lectureId, StringComparison.OrdinalIgnoreCase));
            if (node == null) return;

            bool currentlyPublished = string.Equals(Str(node, "Status"), "Đã xuất bản", StringComparison.OrdinalIgnoreCase);
            string nextStatus = currentlyPublished ? "Bản nháp" : "Đã xuất bản";
            SetVal(node, "Status", nextStatus);
            SetVal(node, "UpdatedAt", DateTime.Now.ToString("s"));
            SaveDocument(doc);

            CourseItem course = GetCourseById(key, Str(node, "CourseId"));
            string courseName = course != null ? course.Name : Str(node, "CourseId");

            if (!currentlyPublished)
                AddActivity(key, "published", "Bài giảng vừa được xuất bản: “" + Str(node, "Title") + "”", "Môn: " + courseName, "Đã công khai");
            else
                AddActivity(key, "updated", "Chuyển bài giảng về bản nháp: “" + Str(node, "Title") + "”", "Môn: " + courseName, "Bản nháp");
        }

        public static void DeleteLecture(string key, string lectureId)
        {
            XDocument doc = EnsureDocument();
            XElement lecturesRoot = doc.Root.Element("Lectures");
            if (lecturesRoot == null) return;

            XElement node = lecturesRoot.Elements("Lecture").FirstOrDefault(l =>
                IsOwner(l, key) && string.Equals(Str(l, "Id"), lectureId, StringComparison.OrdinalIgnoreCase));
            if (node == null) return;

            node.Remove();
            SaveDocument(doc);
        }

        // ====================================================================
        //  4. THÔNG BÁO & HOẠT ĐỘNG GẦN ĐÂY
        // ====================================================================
        public static List<NotificationItem> GetNotifications(string key)
        {
            XDocument doc = EnsureDocument();
            XElement notiRoot = doc.Root.Element("Notifications");
            var courses = GetCourses(key).ToDictionary(c => c.Id, c => c.Name, StringComparer.OrdinalIgnoreCase);
            var list = new List<NotificationItem>();
            if (notiRoot == null) return list;

            foreach (XElement n in notiRoot.Elements("Notification").Where(x => IsOwner(x, key)))
            {
                string cid = Str(n, "CourseId");
                DateTime dt;
                if (!DateTime.TryParse(Str(n, "CreatedAt"), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    dt = DateTime.Now;

                list.Add(new NotificationItem
                {
                    Id = Str(n, "Id"),
                    CourseId = cid,
                    CourseName = cid == "SYSTEM" ? "Hệ thống đào tạo" : (courses.ContainsKey(cid) ? courses[cid] : "Tất cả môn học"),
                    Title = Str(n, "Title"),
                    Message = Str(n, "Message"),
                    CreatedAt = dt,
                    IsRead = string.Equals(Str(n, "IsRead"), "true", StringComparison.OrdinalIgnoreCase)
                });
            }
            return list.OrderByDescending(x => x.CreatedAt).ToList();
        }

        public static int UnreadCount(string key)
        {
            return GetNotifications(key).Count(n => !n.IsRead);
        }

        public static void CreateNotification(string key, string courseId, string title, string message)
        {
            XDocument doc = EnsureDocument();
            XElement notiRoot = doc.Root.Element("Notifications");
            if (notiRoot == null)
            {
                notiRoot = new XElement("Notifications");
                doc.Root.Add(notiRoot);
            }

            notiRoot.Add(new XElement("Notification",
                new XElement("Id", "NOTI" + DateTime.Now.ToString("yyMMddHHmmss")),
                new XElement("InstructorUsername", key),
                new XElement("CourseId", courseId),
                new XElement("Title", title),
                new XElement("Message", message),
                new XElement("CreatedAt", DateTime.Now.ToString("s")),
                new XElement("IsRead", "true")
            ));
            SaveDocument(doc);

            CourseItem c = GetCourseById(key, courseId);
            string targetName = c != null ? ("Môn: " + c.Name) : "Tất cả lớp phụ trách";
            AddActivity(key, "assigned", "Gửi thông báo mới: “" + title + "”", targetName, "Chính thức");
        }

        public static void MarkNotificationRead(string key, string id)
        {
            XDocument doc = EnsureDocument();
            XElement notiRoot = doc.Root.Element("Notifications");
            if (notiRoot == null) return;

            XElement node = notiRoot.Elements("Notification").FirstOrDefault(n =>
                IsOwner(n, key) && string.Equals(Str(n, "Id"), id, StringComparison.OrdinalIgnoreCase));
            if (node != null)
            {
                SetVal(node, "IsRead", "true");
                SaveDocument(doc);
            }
        }

        public static void MarkAllNotificationsRead(string key)
        {
            XDocument doc = EnsureDocument();
            XElement notiRoot = doc.Root.Element("Notifications");
            if (notiRoot == null) return;

            foreach (XElement n in notiRoot.Elements("Notification").Where(x => IsOwner(x, key)))
            {
                SetVal(n, "IsRead", "true");
            }
            SaveDocument(doc);
        }

        public static void DeleteNotification(string key, string id)
        {
            XDocument doc = EnsureDocument();
            XElement notiRoot = doc.Root.Element("Notifications");
            if (notiRoot == null) return;

            XElement node = notiRoot.Elements("Notification").FirstOrDefault(n =>
                IsOwner(n, key) && string.Equals(Str(n, "Id"), id, StringComparison.OrdinalIgnoreCase));
            if (node != null)
            {
                node.Remove();
                SaveDocument(doc);
            }
        }

        public static List<ActivityItem> GetRecentActivities(string key, int maxCount)
        {
            XDocument doc = EnsureDocument();
            XElement actRoot = doc.Root.Element("Activities");
            var list = new List<ActivityItem>();
            if (actRoot == null) return list;

            foreach (XElement a in actRoot.Elements("Activity").Where(x => IsOwner(x, key)))
            {
                DateTime dt;
                if (!DateTime.TryParse(Str(a, "Time"), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    dt = DateTime.MinValue;

                var item = new ActivityItem
                {
                    Type = Str(a, "Type"),
                    Title = Str(a, "Title"),
                    Subject = Str(a, "Subject"),
                    Time = dt,
                    Status = Str(a, "Status")
                };
                item.Meta = BuildMeta(item);
                list.Add(item);
            }
            return list.OrderByDescending(x => x.Time).Take(maxCount).ToList();
        }

        public static void AddActivity(string key, string type, string title, string subject, string status)
        {
            XDocument doc = EnsureDocument();
            XElement actRoot = doc.Root.Element("Activities");
            if (actRoot == null)
            {
                actRoot = new XElement("Activities");
                doc.Root.Add(actRoot);
            }

            actRoot.AddFirst(new XElement("Activity",
                new XElement("InstructorUsername", key),
                new XElement("Type", type),
                new XElement("Title", title),
                new XElement("Subject", subject),
                new XElement("Time", DateTime.Now.ToString("s")),
                new XElement("Status", status)
            ));
            SaveDocument(doc);
        }

        public static List<StudentEnrolledItem> GetSampleStudentsForCourse(string courseId)
        {
            return new List<StudentEnrolledItem>
            {
                new StudentEnrolledItem { StudentCode = "SV2026001", FullName = "Nguyễn Minh Anh", Email = "minhanh@student.edu.vn", CompletedPercent = 92, Status = "Đang học" },
                new StudentEnrolledItem { StudentCode = "SV2026014", FullName = "Lê Hoàng Nam", Email = "hoangnam@student.edu.vn", CompletedPercent = 85, Status = "Đang học" },
                new StudentEnrolledItem { StudentCode = "SV2026027", FullName = "Phạm Thu Hà", Email = "thuha@student.edu.vn", CompletedPercent = 78, Status = "Đang học" },
                new StudentEnrolledItem { StudentCode = "SV2026039", FullName = "Đặng Quốc Bảo", Email = "quocbao@student.edu.vn", CompletedPercent = 64, Status = "Cần nhắc nhở" }
            };
        }

        // ====================================================================
        //  HÀM HỖ TRỢ NỘI BỘ
        // ====================================================================
        public static string ResolveThumb(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            value = value.Trim();

            if (value.StartsWith("~") || value.StartsWith("/") ||
                value.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                Path.HasExtension(value))
            {
                return value;
            }

            foreach (string ext in THUMB_EXT)
            {
                if (File.Exists(HttpContext.Current.Server.MapPath(THUMB_FOLDER + value + ext)))
                    return THUMB_FOLDER + value + ext;
            }
            return string.Empty;
        }

        private static string BuildMeta(ActivityItem a)
        {
            string when = a.Time == DateTime.MinValue
                ? string.Empty
                : RelativeTime(a.Time) + " (" + a.Time.ToString("dd/MM/yyyy, HH:mm") + ")";
            if (string.IsNullOrEmpty(a.Subject)) return when;
            if (string.IsNullOrEmpty(when)) return a.Subject;
            return a.Subject + "  •  " + when;
        }

        public static string RelativeTime(DateTime t)
        {
            TimeSpan d = DateTime.Now - t;
            if (d.TotalMinutes < 1) return "Vừa xong";
            if (d.TotalMinutes < 60) return (int)d.TotalMinutes + " phút trước";
            if (d.TotalHours < 24) return (int)d.TotalHours + " giờ trước";
            if (d.TotalDays < 2) return "Hôm qua";
            return (int)d.TotalDays + " ngày trước";
        }

        private static bool IsOwner(XElement e, string username)
        {
            return string.Equals(Str(e, "InstructorUsername"), username, StringComparison.OrdinalIgnoreCase);
        }

        private static string Str(XElement e, string name)
        {
            XElement x = e.Element(name);
            return x == null ? string.Empty : (x.Value ?? string.Empty).Trim();
        }

        private static int Int(XElement e, string name)
        {
            int v;
            return int.TryParse(Str(e, name), out v) ? v : 0;
        }

        private static void SetVal(XElement parent, string name, string val)
        {
            XElement el = parent.Element(name);
            if (el != null) el.Value = val ?? string.Empty;
            else parent.Add(new XElement(name, val ?? string.Empty));
        }

        private static string HashPbkdf2(string password)
        {
            const int iterations = 100000;
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return iterations + "." + Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
            }
        }

        private static bool VerifyPbkdf2(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash)) return false;
            string[] parts = storedHash.Split('.');
            if (parts.Length != 3) return false;

            int iterations;
            if (!int.TryParse(parts[0], out iterations)) return false;

            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expectedHash = Convert.FromBase64String(parts[2]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] actualHash = pbkdf2.GetBytes(expectedHash.Length);
                return actualHash.SequenceEqual(expectedHash);
            }
        }
    }
}