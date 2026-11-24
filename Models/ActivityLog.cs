
namespace EduLearn.Models
{
    public class ActivityLog
    {
        public string ActivityId { get; set; }
        public string UserId { get; set; }
        public string ActionType { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }
        public string IpAddress { get; set; }
    }
}
