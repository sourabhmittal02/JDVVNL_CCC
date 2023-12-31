using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using ComplaintTracker;
using ComplaintTracker.DAL;
using ComplaintTracker.ExternalAPI;
using ComplaintTracker.Models;
using static System.Net.WebRequestMethods;

namespace ComplaintTracker.Controllers
{
    public class LoginController : Controller
    {
        #region Data
        string message = string.Empty;
        #endregion

        #region AccountLogin
        [AllowAnonymous]
        public ActionResult AccountLogin()
        {
           
            ViewBag.Title = "JDVVNL Account login";
            if (TempData["loginmsg"] != null)
            {
                Session.RemoveAll();
                FormsAuthentication.SignOut();
            }
            return View();
        }
        #endregion
        public async Task<string> SendOTPSms(string MobileNO, string OTP)
        {
            COMPLAINT modelRemark = new COMPLAINT();
            ModelSmsAPI modelSmsAPI_FRT = new ModelSmsAPI();
            TextSmsAPI textSmsAPI1 = new TextSmsAPI();
            modelSmsAPI_FRT.To = MobileNO.ToString();
            modelSmsAPI_FRT.Smstemplete = "1307161561934810613";
            modelSmsAPI_FRT.Smstext = "Dear Consumer, One time password for login into Smart Meter portal is " + OTP + " .Please do not share with anyone. -JdVVNL";
            string response1 = await textSmsAPI1.RegisterComplaintSMS(modelSmsAPI_FRT);
            modelRemark.SMS = modelSmsAPI_FRT.Smstext;
            modelRemark.MOBILE_NO = modelSmsAPI_FRT.To;
            Repository.PUSH_SMS_DETAIL_Consumer(modelRemark, response1);
            return response1;
        }
        #region AccountLogin
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AccountLogin(ModelUser user)
        {
            //SendOTPSms("8769576997", "123456");
            SqlParameter[] param ={
                    new SqlParameter("@Username",user.LoginId.Trim()),
                    new SqlParameter("@Password",Utility.EncryptText(user.Password.Trim()) )
                                       };

            DataSet ds = SqlHelper.ExecuteDataset(HelperClass.Connection, CommandType.StoredProcedure, "Validate_User", param);

            if (ds.Tables.Count == 1)

            {
                message = "Username and/or password is incorrect.";
                ViewBag.Message = message;
                return View(user);
            }
            else
            {
                Session["UserName"] = ds.Tables[0].Rows[0]["name"].ToString();
                Session["UserID"] = ds.Tables[0].Rows[0]["USER_ID"].ToString();
                Session["User_Name"] = ds.Tables[0].Rows[0]["USER_Name"].ToString();
                Session["OFFICE_ID"] = ds.Tables[0].Rows[0]["OFFICE_ID"].ToString();
                Session["Roll_ID"] = ds.Tables[0].Rows[0]["ROLE_ID"].ToString();
                Session["Roll_Name"] = ds.Tables[0].Rows[0]["ROLE_NAME"].ToString();
                Session["LoginType"] = "Active";

                FormsAuthentication.SetAuthCookie(user.LoginId, false);
                if (!string.IsNullOrEmpty(Request.Form["ReturnUrl"]))
                {

                    return RedirectToAction(Request.Form["ReturnUrl"].Split('/')[2]);
                }
                else
                {
                    UserAPI userAPI = new UserAPI();
                    ModelUser modelUser = new ModelUser();
                    modelUser.PhoneLogin = ds.Tables[0].Rows[0]["PHONE_LOGIN"].ToString();
                    modelUser.PhonePassword = ds.Tables[0].Rows[0]["PHONE_PASS"].ToString();
                    modelUser.User_Name = ds.Tables[0].Rows[0]["USER_Name"].ToString();
                    modelUser.Password = user.Password.Trim();
                    modelUser.agent_type = ds.Tables[0].Rows[0]["ROLE_NAME"].ToString();



                    //userAPI.LoginAgentUser(modelUser);
                    //---in sp

                    Repository.AgentLogin(modelUser.User_Name, modelUser.agent_type, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "IN");
                    List<ModelMenu> lstMenu = new List<ModelMenu>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string submenuId = dr.ItemArray[9].ToString(); //SubMenuId
                        if (string.IsNullOrEmpty(submenuId))
                        {
                            //Main Menu
                            ModelMenu modelMenu = new ModelMenu();
                            modelMenu.MainMenuName = dr.ItemArray[8].ToString(); //MenuName
                            modelMenu.MainMenuViewURL = dr.ItemArray[11].ToString(); //Url
                            lstMenu.Add(modelMenu);
                        }
                        else
                        {
                            List<ModelSubMenu> lstsubMenu = new List<ModelSubMenu>();

                            if (lstMenu.Where(x => x.MainMenuName == dr.ItemArray[8].ToString()).Count() <= 0)
                            {

                                ModelMenu modelMenu = new ModelMenu();
                                modelMenu.MainMenuName = dr.ItemArray[8].ToString();

                                foreach (DataRow drsubMenu in ds.Tables[0].Rows)
                                {

                                    if (!string.IsNullOrEmpty(submenuId) && modelMenu.MainMenuName == drsubMenu.ItemArray[8].ToString())
                                    {
                                        ModelSubMenu modelSubMenu = new ModelSubMenu();
                                        modelSubMenu.SubMenuName = drsubMenu.ItemArray[10].ToString(); //SubMenuName
                                        modelSubMenu.SubMenuViewURL = drsubMenu.ItemArray[11].ToString(); //Url

                                        lstsubMenu.Add(modelSubMenu);
                                        modelMenu.ListSubMenu = lstsubMenu;

                                    }

                                }
                                lstMenu.Add(modelMenu);

                            }
                        }
                    }
                    TempData["loginmsg"] = "Login Successfull.";
                    Session["ModelMenu"] = lstMenu;
                    return RedirectToAction("Index", "Dashboard");
                }
            }


        }
        #endregion

        #region ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            ModelUser obj = new ModelUser();
            obj = Repository.EditUser(Convert.ToInt32(Session["UserID"].ToString()));

            return View(obj);
        }
        #endregion

        #region ChangePassword
        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ModelUser User)
        {
            try
            {
                // TODO: Add insert logic here
                User.User_id = Convert.ToInt32(Session["UserID"].ToString());
                String Status = Repository.ChangePassword(User);

                TempData["AlertMessage"] = Status;
                return RedirectToAction("ChangePassword", "Login");
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region Logout
        [HttpGet]
        [Authorize]
        public ActionResult Logout()
        {

            //out sp
            UserAPI userAPI = new UserAPI();
            ModelUser modelUser = new ModelUser();
            modelUser.User_Name = Session["User_Name"].ToString();

            //userAPI.LoginAgentUser(modelUser);
            if (userAPI.LogOutAgentUser(modelUser))
            {
                Repository.AgentLogin(Session["User_Name"].ToString(), Session["Roll_Name"].ToString(), DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "OUT");
            }
 
            message = "Logout Successfully! " + modelUser.User_Name;
            TempData["loginmsg"] = message;
            return RedirectToAction("AccountLogin");
        }
        #endregion

        #region Break
        [HttpGet]
        public ActionResult Break()
        {
            UserAPI userAPI = new UserAPI();
            ModelUser modelUser = new ModelUser();
            modelUser.User_Name = Session["User_Name"].ToString();

            //userAPI.LoginAgentUser(modelUser);
            if (userAPI.BreakResumeAgentUser(modelUser, "PAUSE"))
            {
                Repository.AgentLogin(Session["User_Name"].ToString(), Session["Roll_Name"].ToString(), DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "BR");
            }

            Session["LoginType"] = "Resume";
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        #region Resume
        [HttpGet]
        public ActionResult Resume()
        {
            UserAPI userAPI = new UserAPI();
            ModelUser modelUser = new ModelUser();
            modelUser.User_Name = Session["User_Name"].ToString();

            //userAPI.LoginAgentUser(modelUser);
            if (userAPI.BreakResumeAgentUser(modelUser, "RESUME"))
            {
                Repository.AgentLogin(Session["User_Name"].ToString(), Session["Roll_Name"].ToString(), DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "BRR");
            }
            Session["LoginType"] = "Active";
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion

        [HttpGet]
        public async Task<ActionResult> ShowLoginOTP(ModelUser user) //It will be fired from Jquery ajax call
        {
            SqlParameter[] param ={
                    new SqlParameter("@Username",user.LoginId.Trim()),
                    new SqlParameter("@Password",Utility.EncryptText(user.Password.Trim()) )
                                       };

            DataSet ds = SqlHelper.ExecuteDataset(HelperClass.Connection, CommandType.StoredProcedure, "Validate_User", param);

            if (ds.Tables.Count == 1)

            {
                message = "Username and/or password is incorrect.";
                ViewBag.Message = message;
                return View(user);
            }
            else
            {
                Session["UserName"] = ds.Tables[0].Rows[0]["name"].ToString();
                Session["UserID"] = ds.Tables[0].Rows[0]["USER_ID"].ToString();
                Session["User_Name"] = ds.Tables[0].Rows[0]["USER_Name"].ToString();
                Session["OFFICE_ID"] = ds.Tables[0].Rows[0]["OFFICE_ID"].ToString();
                Session["Roll_ID"] = ds.Tables[0].Rows[0]["ROLE_ID"].ToString();
                Session["Roll_ID"] = "5";
                Session["Roll_Name"] = ds.Tables[0].Rows[0]["ROLE_NAME"].ToString();
                Session["Mobile_No"] = ds.Tables[0].Rows[0]["MOBILE_NO"].ToString();
                Session["LoginType"] = "Active";

                FormsAuthentication.SetAuthCookie(user.LoginId, false);
                if (!string.IsNullOrEmpty(Request.Form["ReturnUrl"]))
                {

                    return RedirectToAction(Request.Form["ReturnUrl"].Split('/')[2]);
                }
                else
                {

                    if (Convert.ToString(Session["Roll_ID"]).Contains("5") || Convert.ToString(Session["Roll_ID"]).Contains("6") || Convert.ToString(Session["Roll_ID"]).Contains("7"))
                    {
                        #region generateOTP
                        LoginOTP obj = new LoginOTP();
                        obj.LoginId = Convert.ToString(Session["User_Name"]);

                        string otpforuser = Repository.GenerateOtp(Convert.ToString(@Session["Mobile_No"]));
                        obj.otpforLogin = await SendOTPSms(Convert.ToString(@Session["Mobile_No"]), otpforuser);
                        return PartialView("_otpPopup", obj);
                        #endregion
                    }
                    else
                    {

                        UserAPI userAPI = new UserAPI();
                        ModelUser modelUser = new ModelUser();
                        modelUser.PhoneLogin = ds.Tables[0].Rows[0]["PHONE_LOGIN"].ToString();
                        modelUser.PhonePassword = ds.Tables[0].Rows[0]["PHONE_PASS"].ToString();
                        modelUser.User_Name = ds.Tables[0].Rows[0]["USER_Name"].ToString();
                        modelUser.Password = user.Password.Trim();
                        modelUser.agent_type = ds.Tables[0].Rows[0]["ROLE_NAME"].ToString();



                        //userAPI.LoginAgentUser(modelUser);
                        //---in sp

                        Repository.AgentLogin(modelUser.User_Name, modelUser.agent_type, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, "IN");
                        List<ModelMenu> lstMenu = new List<ModelMenu>();
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string submenuId = dr.ItemArray[9].ToString(); //SubMenuId
                            if (string.IsNullOrEmpty(submenuId))
                            {
                                //Main Menu
                                ModelMenu modelMenu = new ModelMenu();
                                modelMenu.MainMenuName = dr.ItemArray[8].ToString(); //MenuName
                                modelMenu.MainMenuViewURL = dr.ItemArray[11].ToString(); //Url
                                lstMenu.Add(modelMenu);
                            }
                            else
                            {
                                List<ModelSubMenu> lstsubMenu = new List<ModelSubMenu>();

                                if (lstMenu.Where(x => x.MainMenuName == dr.ItemArray[8].ToString()).Count() <= 0)
                                {

                                    ModelMenu modelMenu = new ModelMenu();
                                    modelMenu.MainMenuName = dr.ItemArray[8].ToString();

                                    foreach (DataRow drsubMenu in ds.Tables[0].Rows)
                                    {

                                        if (!string.IsNullOrEmpty(submenuId) && modelMenu.MainMenuName == drsubMenu.ItemArray[8].ToString())
                                        {
                                            ModelSubMenu modelSubMenu = new ModelSubMenu();
                                            modelSubMenu.SubMenuName = drsubMenu.ItemArray[10].ToString(); //SubMenuName
                                            modelSubMenu.SubMenuViewURL = drsubMenu.ItemArray[11].ToString(); //Url

                                            lstsubMenu.Add(modelSubMenu);
                                            modelMenu.ListSubMenu = lstsubMenu;

                                        }

                                    }
                                    lstMenu.Add(modelMenu);

                                }
                            }
                        }
                        TempData["loginmsg"] = "Login Successfull.";
                        Session["ModelMenu"] = lstMenu;
                        var urlBuilder = new UrlHelper(Request.RequestContext);
                        var url = urlBuilder.Action("Index", "Dashboard");
                        return Json(new { status = "success", redirectUrl = url }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

        }

        [HttpGet]
        public JsonResult ValidateOTP(LoginOTP  loginOTP)
        {
            string response = Repository.ValidateOTP(Convert.ToString(Session["Mobile_No"]), loginOTP.otpforLogin);

            if (response=="1")
            {
                SqlParameter[] param ={
                    new SqlParameter("@Username","Outbound"),
                    new SqlParameter("@Password",Utility.EncryptText("a"))
                                       };

                DataSet ds = SqlHelper.ExecuteDataset(HelperClass.Connection, CommandType.StoredProcedure, "Validate_User", param);

            

                List<ModelMenu> lstMenu = new List<ModelMenu>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string submenuId = dr.ItemArray[9].ToString(); //SubMenuId
                    if (string.IsNullOrEmpty(submenuId))
                    {
                        //Main Menu
                        ModelMenu modelMenu = new ModelMenu();
                        modelMenu.MainMenuName = dr.ItemArray[8].ToString(); //MenuName
                        modelMenu.MainMenuViewURL = dr.ItemArray[11].ToString(); //Url
                        lstMenu.Add(modelMenu);
                    }
                    else
                    {
                        List<ModelSubMenu> lstsubMenu = new List<ModelSubMenu>();

                        if (lstMenu.Where(x => x.MainMenuName == dr.ItemArray[8].ToString()).Count() <= 0)
                        {

                            ModelMenu modelMenu = new ModelMenu();
                            modelMenu.MainMenuName = dr.ItemArray[8].ToString();

                            foreach (DataRow drsubMenu in ds.Tables[0].Rows)
                            {

                                if (!string.IsNullOrEmpty(submenuId) && modelMenu.MainMenuName == drsubMenu.ItemArray[8].ToString())
                                {
                                    ModelSubMenu modelSubMenu = new ModelSubMenu();
                                    modelSubMenu.SubMenuName = drsubMenu.ItemArray[10].ToString(); //SubMenuName
                                    modelSubMenu.SubMenuViewURL = drsubMenu.ItemArray[11].ToString(); //Url

                                    lstsubMenu.Add(modelSubMenu);
                                    modelMenu.ListSubMenu = lstsubMenu;

                                }

                            }
                            lstMenu.Add(modelMenu);

                        }
                    }
                }
                TempData["loginmsg"] = "Login Successfull.";
                Session["ModelMenu"] = lstMenu;



                var urlBuilder = new UrlHelper(Request.RequestContext);
                var url = urlBuilder.Action("Index", "Dashboard");
                return Json(new { status = "success", redirectUrl = url }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { status = "failure", redirectUrl = string.Empty }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}