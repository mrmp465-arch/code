using Card.Data.Api;
using Card.Data.DTO;
using Card.Data.Service;
using Card.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Card.CMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsersService _userservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        public AccountController(IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
        }

        #region "Đăng nhập"
        public ActionResult Login(string act, string url)
        {
            if (!string.IsNullOrEmpty(act) && act == "out")
            {
                ///m_UserValidation.SignOut();
                Session.Abandon();
                Session.RemoveAll();
                ExpireAllCookies();
                Response.Redirect("~/", true);
            }
            ViewBag.url = url;
          
            return View();
        }
        private void ExpireAllCookies()
        {
            if (HttpContext != null)
            {
                int cookieCount = Request.Cookies.Count;
                for (var i = 0; i < cookieCount; i++)
                {
                    var cookie = Request.Cookies[i];
                    if (cookie != null)
                    {
                        var expiredCookie = new HttpCookie(cookie.Name)
                        {
                            Expires = DateTime.Now.AddDays(-1),
                            Domain = cookie.Domain
                        };
                        Response.Cookies.Add(expiredCookie); // overwrite it
                    }
                }

                // clear cookies server side
                Request.Cookies.Clear();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password, string Capchar)
        {
            try
            {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                    return Json(new { success = false, statusCode = -1, msg = "Dữ liệu không được bỏ trống" });

                if (Capchar != Session["Captcha"].ToString())
                {
                    return Json(new { success = false, statusCode = -5, msg = "Mã xác thực không đúng" });

                }
                var password = Encrypt.MD5(Password.Trim() + Config.GetAppsetting("Salt"));

                int checkLogin = _userservice.Authentication(Username.Trim(), password);

                if (checkLogin > 0)
                {
                    var m_Users = _userservice.GetByUsername(Username);
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
                        var insertLog = _userlogservice.InsertUsersLog(Log);

                        if (m_Users.Status)
                        {
                            if (m_Users.Type == 1)
                            {

                                Session[SessionsManager.SESSION_FUNCTIONS] = _functionservice.GetListFunctionBySystemID(0);
                                Session[SessionsManager.SESSION_USERFUNCTIONS] = new List<UserFunction>();
                            }
                            else
                            {
                                //tk admin tổng
                                //if (m_Users.Type == 2)
                                //{
                                //    if (m_Users.Balance < 500000000)
                                //    {
                                //        var result = _userservice.Topup(m_Users.UserID, "admin", 1000000000, "Cộng tiền tự động");
                                //        if (result > 0)
                                //            m_Users.Balance += 1000000000;
                                //    }
                                //}
                                /*bo quyen theo user*/
                                //Session[SessionsManager.SESSION_FUNCTIONS] = _functionservice.GetListFunctionByUserID(m_Users.UserID);
                                //Session[SessionsManager.SESSION_USERFUNCTIONS] = _userroleservice.UserFunction_GetByUserID(m_Users.UserID);
                                Session[SessionsManager.SESSION_FUNCTIONS] = _userroleservice.GetListFunctionByID(m_Users.Type);
                                Session[SessionsManager.SESSION_USERFUNCTIONS] = _userroleservice.GroupFunction_GetByID(m_Users.Type);
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
                            //if (m_Users.Type > 3)
                            //{
                            //    var parrent = _userservice.GetByUsername(m_Users.CreatedUser);
                            //    m_Users.StatusGarena = m_Users.StatusGarena && parrent.StatusGarena;
                            //    m_Users.StatusZing = m_Users.StatusZing && parrent.StatusZing;
                            //    m_Users.StatusVTC = m_Users.StatusVTC && parrent.StatusVTC;
                            //    m_Users.StatusVTT = m_Users.StatusVTT && parrent.StatusVTT;
                            //    m_Users.StatusVMS = m_Users.StatusVMS && parrent.StatusVMS;
                            //    m_Users.StatusVNP = m_Users.StatusVNP && parrent.StatusVNP;
                            //}
                            Session[SessionsManager.SESSION_USER] = userinfo;
                            Session[SessionsManager.SESSION_USER_FULL] = m_Users;
                            //Session[SessionsManager.SESSION_TOKEN] = ServerProcess.GetUserTokenCache(m_Users.UserAPI, m_Users.PasswordAPI);
                            string SessionID = Session.SessionID;


                            return Json(new { success = true, statusCode = 1, msg = "Đăng Nhập Thành Công" });
                        }
                        return Json(new { success = false, statusCode = -102, msg = "Tài khoản của bạn đã bị khóa" });
                    }
                }
                return Json(new { success = false, statusCode = -1, msg = "Username hoặc Password không đúng" });
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