using CMS.Data.DTO;
using CMS.Data.Factory;
using CMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        #region "Đăng nhập"
        public ActionResult Login(string act, string url)
        {
            if (!string.IsNullOrEmpty(act) && act == "out")
            {
                ///m_UserValidation.SignOut();
                Session.Abandon();
                Session.RemoveAll();
                Response.Redirect("~/", true);
            }
            ViewBag.url = url;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password, string Capchar)
        {
            try
            {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                    return Json(new { success = false, statusCode = -1, msg = "Dữ liệu không được bỏ trống" });

                //if (Capchar != Session["Captcha"].ToString())
                //{
                //    return Json(new { success = false, statusCode = -5, msg = "Mã xác thực không đúng" });

                //}
                var password = Encrypt.MD5(Password.Trim());

                int checkLogin = AbstractDAOFactory.Instance().UsersService().Authentication(Username.Trim(), password);

                if (checkLogin > 0)
                {
                    var m_Users = AbstractDAOFactory.Instance().UsersService().GetByUsername(Username);
                    if (m_Users != null && m_Users.UserID > 0)
                    {
                        // m_Users.IsActive = true;
                        var Log = new UsersLog();
                        Log.ClientIP = Config.GetIP();
                        Log.UserID = m_Users.UserID;
                        Log.UserName = m_Users.Username;
                        Log.LogType = 1;
                        Log.FunctionCode = "login";
                        Log.Description = "Tài khoản " + m_Users.Username + " Đăng nhập hệ thống";
                        var insertLog = AbstractDAOFactory.Instance().UsersLogService().InsertUsersLog(Log);

                        if (m_Users.Status)
                        {
                            if (m_Users.Type == 1)
                            {

                                Session[SessionsManager.SESSION_FUNCTIONS] = AbstractDAOFactory.Instance().FunctionsService().GetListFunctionBySystemID(0);
                                Session[SessionsManager.SESSION_USERFUNCTIONS] = new List<UserFunction>();
                            }
                            else
                            {
                                Session[SessionsManager.SESSION_FUNCTIONS] = AbstractDAOFactory.Instance().FunctionsService().GetListFunctionByUserID(m_Users.UserID);
                                Session[SessionsManager.SESSION_USERFUNCTIONS] = AbstractDAOFactory.Instance().UserRoleService().GroupFunction_GetByID(m_Users.Type);
                            }

                            Session[SessionsManager.SESSION_USERID] = m_Users.UserID;
                            Session[SessionsManager.SESSION_USERNAME] = m_Users.Username;
                            var userinfo = new UserSession
                            {
                                UserID = m_Users.UserID,
                                Username = m_Users.Username,
                                FullName = m_Users.FullName,
                                Email = m_Users.Email,
                                Type = m_Users.Type
                            };
                            Session[SessionsManager.SESSION_USER] = userinfo;

                            string SessionID = Session.SessionID;


                            return Json(new { success = true, statusCode = 1, msg = "Đăng Nhập Thành Công" });
                        }
                        return Json(new { success = false, statusCode = -102, msg = "Tài khoản của bạn đã bị khóa" });
                    }
                }
                return Json(new { success = false, statusCode = checkLogin, msg = "Username hoặc Password không đúng" });
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return Json(new { success = false, statusCode = -99, msg = "Hệ thống bận vui lòng quay lại sau" });
            }
        }
        #endregion
    }
}