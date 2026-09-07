using SMS.Data.DTO;
using SMS.Data.Service;
using SMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.CMS.Filter;
using SMS.CMS.Models;
using SMS.Data.Api;
using System.Text.RegularExpressions;
using System.Text;

namespace SMS.CMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUsersService _userservice;
        private readonly IAccountTokenService _accounttokenservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        private readonly ITransactionsService _transervice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } }
        public HomeController(IAccountTokenService accounttokenservice,IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _accounttokenservice = accounttokenservice;
        }

        public ActionResult Index()
        {
            //string Content = "{xin chao|hello|hi|} Cuong {cam on|thanks}";
            //Content = StringUtils.FomatSMSContent(Content);
            //NLogLogger.DebugMessage(Content);
            //ServerProcess.GetProfile(11000);


            // Get the number of messages in the mailbox.
            //ServerProcess.SendSMSAPI("0904514902", "123456");

            //var str = "M\u00e3 th\u1ebb n\u00e0y \u0111\u00e3 c\u00f3 tr\u00ean h\u1ec7 th\u1ed1ng & \u0111ang x\u1eed l\u00fd. Vui l\u00f2ng ch\u1edd & kh\u00f4ng g\u1eedi l\u1ea1i l\u1ea7n 2 cho c\u00f9ng 1 m\u00e3 th\u1ebb";
            //byte[] utf8Bytes = Encoding.UTF8.GetBytes(str);
            //NLogLogger.DebugMessage( Encoding.UTF8.GetString(utf8Bytes));

            if (CurrentUser == null)
                return RedirectToAction("Login", "Account");
            return RedirectToAction("Hour", "Report");
            //var user = _userservice.SelectByUserID(CurrentUser.UserID);
            //ViewBag.Order = user.StatusOrder;
            //Session[SessionsManager.SESSION_USER_FULL] = user;
            //return View(user);
        }


        public ActionResult ErrorPermission()
        {


            return View();
        }
        public ActionResult ErrorNotPage()
        {


            return View();
        }


        public ActionResult Header()
        {
           // var userinfo = (UserSession)Session[SessionsManager.SESSION_USER];
            var user = _userservice.SelectByUserID(CurrentUser.UserID);
            return PartialView(user);
        }
        public ActionResult Balance(Users user)
        {
            
            ViewBag.Balance = user.Balance+ user.BalanceHold;
            ViewBag.BalanceHold = user.BalanceHold>0? user.BalanceHold:0;
            return PartialView();
        }
        public ActionResult StatusBar(Users user = null)
        {
            if (user != null)
            {

                return PartialView(user);
            }
            var userinfo = (UserSession)Session[SessionsManager.SESSION_USER];
            user = _userservice.SelectByUserID(userinfo.UserID);
            ViewBag.Balance = user.Balance;
            return PartialView(user);
        }
        public ActionResult MetaData()
        {

            var userinfo = (UserSession)Session[SessionsManager.SESSION_USER];
            if (userinfo == null)
            {
                return PartialView(null);
            }
            var data = new MetaDataUser();
            var keycache = string.Format("GetUserMeta-{0}", userinfo.Username);
            int Balance = 0;
            int NumberContact = 0;
            int NumberSMSSend = 0;
            int NumberSMSFinish = 0;
            _userservice.GetMetaData(userinfo.Username, ref Balance, ref NumberContact, ref NumberSMSSend, ref NumberSMSFinish);
            data = new MetaDataUser
            {
                Balance = Balance,
                NumberContact = NumberContact,
                NumberSMSSend = NumberSMSSend,
                NumberSMSFinish = NumberSMSFinish
            };

            return PartialView(data);
        }
        public ActionResult Menu()
        {
            var userinfo = (UserSession)Session[SessionsManager.SESSION_USER];

            var functions = (List<Functions>)Session[SessionsManager.SESSION_FUNCTIONS];
            if (userinfo == null)
            {
                return PartialView(null);
            }
            if (functions == null)
            {
                if (userinfo.Type == 1)
                {
                    Session[SessionsManager.SESSION_FUNCTIONS] = _functionservice.GetListFunctionBySystemID(0);
                    Session[SessionsManager.SESSION_USERFUNCTIONS] = new List<UserFunction>();
                }
                else
                {
                    /*bo quyen theo user*/
                    //functions= _functionservice.GetListFunctionByUserID(userinfo.UserID); 
                    //Session[SessionsManager.SESSION_USERFUNCTIONS] = _userroleservice.UserFunction_GetByUserID(userinfo.UserID);

                    functions = _userroleservice.GetListFunctionByID(userinfo.Type);
                    Session[SessionsManager.SESSION_USERFUNCTIONS] = _userroleservice.GroupFunction_GetByID(userinfo.Type);
                }
            }
            Session[SessionsManager.SESSION_FUNCTIONS] = functions;
            return PartialView(functions);
        }
      
    }
}