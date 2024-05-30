using DirectComplaintRegister.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using DirectComplaintRegister.DAL;
using System.IO;
using System.Text;
using static ComplaintTracker.JqueryDatatableParam;
using ComplaintTracker.DAL;
using ComplaintTracker.Handler;
//using ComplaintTracker.Models;

namespace ComplaintTracker.Areas.DirectComplaintRegister.Controllers
{
    public class ComplaintCloseController : Controller
    {

        // GET: Complaint
        static readonly Serilog.ILogger log = EventLogger._log;
  
        public ActionResult closeSearch()
        {
            ComplaintTracker.Models.COMPLAINT objComplaint = new ComplaintTracker.Models.COMPLAINT();
            ViewBag.fromDate = DateTime.Now;
            ViewBag.toDate = DateTime.Now.AddDays(1);
            ViewBag.RoleID = Session["Roll_ID"];
            objComplaint.ComplaintTypeCollection = Repository.GetComplaintTypeList("0");
            return View(objComplaint);
        }

        [HttpPost]
        public JsonResult GetComplaintSearch(DataTableAjaxPostModel model) //It will be fired from Jquery ajax call
        {
        
            ModelSearchComplaint dataObject = new ModelSearchComplaint();
            List<ModelSearchComplaint> data = new List<ModelSearchComplaint>();
            if (ModelState.IsValid)
            {
                try
                {

                    dataObject.draw = model.draw;
                    dataObject.start = model.start;
                    dataObject.length = model.length;

                    // Initialization.   
                    dataObject.COMPLAINT_NO = 0;
                    dataObject.KNO = 0;
                    dataObject.MOBILE_NO = "0";
                    dataObject.COMPLAINT_TYPE = Convert.ToString(Request.Form.GetValues("COMPLAINT_TYPE")[0]);
                    dataObject.OFFICE_ID = "0";
                    dataObject.COMPLAINT_status = "1";
                    dataObject.COMPLAINT_SOURCE = "0";
                    dataObject.fromdate = Convert.ToString(Request.Form.GetValues("fromdate")[0]);
                    dataObject.todate = Convert.ToString(Request.Form.GetValues("todate")[0]);
                    dataObject.assigned_status = "0";
                   
                    data = RepositoryArea.GetComplaintDetailsForClose(dataObject);
                    int count = data.Count() > 0 ? data[0].Total : 0;
                    return Json(new { draw = dataObject.draw, recordsFiltered = count, recordsTotal = count, data = data }, JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    return Json(new { draw = dataObject.draw, recordsFiltered = 0, recordsTotal = 0, data = data }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Close(string closeIds)
        {
            ComplaintTracker.Models.Response data = RepositoryArea.ComplaintClose(closeIds);

            if (data.status == "-1")
            {
                data.message = "Some error occured in close Complaint No. " + closeIds ;
            }
            else
            {
                data.message = "Complaint No. " + closeIds + " closed Successfully ...!";
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}
