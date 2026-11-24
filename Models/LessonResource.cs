
namespace EduLearn.Models
{
    public class LessonResource
    {
        public string ResourceId { get; set; }
        public string LessonId { get; set; }
        public string ResourceName { get; set; }
        public string ContentItemType { get; set; }
        public string FileFormat { get; set; }
        public string ResourceUrl { get; set; }
        public long FileSizeBytes { get; set; }
        public bool IsDownloadable { get; set; }
        public int DisplayOrder { get; set; }
        public string UploadedBy { get; set; }
        public string UploadedAt { get; set; }
    }
}
