using System;

namespace DocuShareIndexingAPI.Entities
{
    /**
    * @notice properties variables.
    */
    public class WorkerJob
    {
        public int TrxNo {get;set;}
        public string WorkerID {get;set;}
        public string FromPath {get;set;}
        public string ToPath {get;set;}
        public string WorkerStatus {get;set;}
        public DateTime CreateDateTime {get;set;}
        public DateTime UpdateDateTime {get;set;}
    }
}