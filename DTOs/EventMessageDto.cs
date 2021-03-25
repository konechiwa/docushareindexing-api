using DocuShareIndexingAPI.Entities;

namespace DocuShareIndexingAPI.DTOs
{
    public class EventMessageDto
    {
        public string EventType { get; set; }
        public string EventKey { get; set; }
        public string UserID { get; set; }
        public string EventStatus { get; set; }
        public string EventDescription { get; set; }
    }
}