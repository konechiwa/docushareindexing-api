using System;

namespace DocuShareIndexingAPI.Entities
{

    /**
    * @notice properties variables.
    */
    public class EventQueue
    {
        public int TrxNo { get; set; }
        public int EventID { get; set; }
        public string EventKey { get; set; }
        public DateTime EventCreateDate { get; set; }
    }
}