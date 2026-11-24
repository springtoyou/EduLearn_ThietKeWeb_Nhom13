using System;
using System.IO;
using System.Web;
using System.Xml;

namespace EduLearn.DataAccess
{
    public static class XmlDataAccess
    {
        private static readonly object fileLock = new object();

        // Danh sách 12 tệp XML được phép truy cập.
        private static readonly string[] allowedFiles =
        {
            "Users.xml",
            "InstructorProfiles.xml",
            "Categories.xml",
            "Courses.xml",
            "Enrollments.xml",
            "Weeks.xml",
            "Lessons.xml",
            "LessonResources.xml",
            "LessonProgress.xml",
            "Notifications.xml",
            "ActivityLogs.xml",
            "ReleaseSchedules.xml"
        };

        private static string GetAppDataPath()
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException(
                    "Không tìm thấy ngữ cảnh ứng dụng ASP.NET."
                );
            }

            string path = HttpContext.Current.Server.MapPath("~/App_Data/");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        private static string GetFilePath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException(
                    "Tên tệp XML không được để trống.",
                    "fileName"
                );
            }

            string requestedName = Path.GetFileName(fileName);

            if (!string.Equals(
                requestedName,
                fileName,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Tên tệp XML không hợp lệ.",
                    "fileName"
                );
            }

            foreach (string allowedFile in allowedFiles)
            {
                if (string.Equals(
                    allowedFile,
                    requestedName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Path.Combine(
                        GetAppDataPath(),
                        allowedFile
                    );
                }
            }

            throw new ArgumentException(
                "Tệp XML không nằm trong danh sách được phép truy cập.",
                "fileName"
            );
        }

        // Đọc một tệp XML.
        public static XmlDocument LoadXml(string fileName)
        {
            lock (fileLock)
            {
                string path = GetFilePath(fileName);

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(
                        "Không tìm thấy tệp XML: " + fileName,
                        path
                    );
                }

                XmlDocument document = new XmlDocument();
                document.XmlResolver = null;

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Prohibit;
                settings.XmlResolver = null;

                using (XmlReader reader = XmlReader.Create(path, settings))
                {
                    document.Load(reader);
                }

                if (document.DocumentElement == null)
                {
                    throw new XmlException(
                        "Tệp XML không có phần tử gốc: " + fileName
                    );
                }

                return document;
            }
        }

        // Ghi một XmlDocument xuống tệp XML.
        public static void SaveXml(
            string fileName,
            XmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            if (document.DocumentElement == null)
            {
                throw new XmlException(
                    "Không thể lưu XML không có phần tử gốc."
                );
            }

            lock (fileLock)
            {
                string path = GetFilePath(fileName);
                string tempPath = path + ".tmp";

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = new System.Text.UTF8Encoding(false);
                settings.Indent = true;
                settings.IndentChars = "    ";
                settings.NewLineChars = Environment.NewLine;
                settings.CloseOutput = true;

                try
                {
                    using (XmlWriter writer =
                        XmlWriter.Create(tempPath, settings))
                    {
                        document.Save(writer);
                    }

                    // Chỉ thay tệp chính sau khi ghi tệp tạm thành công.
                    File.Copy(tempPath, path, true);
                }
                finally
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
            }
        }

        // Kiểm tra tệp XML có tồn tại hay không.
        public static bool FileExists(string fileName)
        {
            string path = GetFilePath(fileName);
            return File.Exists(path);
        }
    }
}