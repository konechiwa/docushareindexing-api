using System;

namespace DocuShareIndexingAPI.Entities
{
    public class EventMessage
    {
        /**
        * @notice properties variables.
        */
        public int EventID { get; set; }
        public DateTime EventTimestamp { get; set; }
        public string EventType { get; set; }
        public string EventKey { get; set; }
        public string UserID { get; set; }
        public string EventStatus { get; set; }
        public string EventDescription { get; set; }

    }
}