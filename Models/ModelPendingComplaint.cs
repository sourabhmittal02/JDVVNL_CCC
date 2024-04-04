using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComplaintTracker.Models
{
    public class ModelPendingComplaint
    {
           public string SlaType { get; set; }

            public string OfficeCode { get; set; }

            public string Source { get; set; }
            public string Complainttype { get; set; }

            public string OfficeName { get; set; }
            public string TotalComplaintReceived { get; set; }
            public string TodayPending { get; set; }
            public string PreviousDayPending { get; set; }
        public string TodayResolved { get; set; }
        public string OverAllPending { get; set; }
        public string OverAllResolved { get; set; }

    }

    public class ModelQueryBuilderReport
    {
        public string KNO { get; set; }

        public string Consumer_name { get; set; }

        public string ComplaintDate { get; set; }
        public string Duration { get; set; }

        public string Complaint_no { get; set; }
        public string office_name { get; set; }
        public string Address { get; set; }
        public string Complaint_Type { get; set; }
        public string Sub_Complaint_Type { get; set; }
        public string Complaint_Source { get; set; }
        public string Complaint_Status { get; set; }

    }
}