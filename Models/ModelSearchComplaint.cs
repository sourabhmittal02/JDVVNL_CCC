using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ComplaintTracker.JqueryDatatableParam;

namespace ComplaintTracker.Models
{
    public class ModelSearchComplaint : DataTableAjaxPostModel
    {
        public Int64 KNO { get; set; }
        public string NAME { get; set; }
        public string COMPLAINT_DATE { get; set; }
        public string DURATION { get; set; }
        public string OFFICE_ID { get; set; }
        public Int64 COMPLAINT_NO { get; set; }
        public string OFFICE_NAME { get; set; }
        public string REMARK { get; set; }
        public string ADDRESS { get; set; }
        public string COMPLAINT_TYPE { get; set; }
        public string SUB_COMPLAINT_TYPE { get; set; }
        public string SOURCE_NAME { get; set; }
        public string COMPLAINT_status { get; set; }
        public string COMPLAINT_SOURCE { get; set; }
        public string ASSIGNED_TO { get; set; }
        public string OUTAGE_TYPE { get; set; }
        public string RECTIFICATION { get; set; }
        public string CAUSE { get; set; }
        public string METER_NO { get; set; }
        public string USP_GETFRT { get; set; }
        public string METER_TYPE { get; set; }
        public string BEFORE_RECTIFICATION { get; set; }
        public string AFTER_RECTIFICATION { get; set; }
        public string ANY_ABNORMALITY { get; set; }
        public string FILE { get; set; }
        public string SIGNATURE { get; set; }
        public string UPLOAD { get; set; }
        public string CLOSED_BY { get; set; }
        public string CLOSED_SOURCE { get; set; }
        public string MOBILE_NO { get; set; }
        public string ALTERNATE_MOBILE_NO { get; set; }
        public string LANDLINE_NO { get; set; }
        public string REJECTION_CAUSE { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string assigned_status { get; set; }

    }
}