using System;

namespace DocuShareIndexingAPI.Entities
{
    public class DeclarationMessage
    {
        /**
        * @notice properties variables.
        */
        public string METAPath { get; set;}
        public string Title { get; set;}
        public string ShipmentType { get; set;}
        public string BranchCode { get; set;}
        public string RefNo { get; set;} 
        public string DecNo {get;set;}
        public string CommercialInvoices { get; set;}
        public string CmpTaxNo { get; set;}
        public string CmpBranch { get; set;}
        public string CmpName { get; set;}
        public string VesselName { get; set;}
        public string VoyNumber { get; set;}
        public string MasterBL { get; set; }
        public string HouseBL {get; set; }
        public string DestCountry { get; set;}
        public string DeptCountry { get; set; }
        public DateTime ETA { get; set; }
        public DateTime ETD { get; set; }
        public DateTime UDateDeclare { get; set; }
        public DateTime UDateRelease { get; set; }
        public string MaterialType { get; set; }
        public string DatabaseSeq { get; set; }
        public string BranchSeq { get; set; }
        public string Period { get; set; }

    }
}