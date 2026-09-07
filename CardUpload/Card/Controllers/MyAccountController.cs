using Card.Data.DTO;
using Card.Data.Service;
using Card.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Card.CMS.Filter;
using Card.CMS.Models;
using Card.Data.Api;
using System.Configuration;

namespace Card.CMS.Controllers
{
    public class MyAccountController : Controller
    {
        private readonly IUsersService _userservice;
        private readonly IAccountTokenService _accounttokenservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        private readonly IOrderTempsService _oservice;
        private readonly IOrderReportsService _orderservice;
        private readonly ITransactionsService _transervice;
        private readonly IBidHistoryService _bidHistoryservice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } set { } }
        public MyAccountController(IBidHistoryService bidHistoryservice, IOrderReportsService orderservice, IOrderTempsService oservice, IAccountTokenService accounttokenservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _accounttokenservice = accounttokenservice;
            _oservice = oservice;
            _orderservice = orderservice;
            _bidHistoryservice = bidHistoryservice;
        }
        [PermissionFilter(FunctionCode = FunctionCode.MyAccountVTT)]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadVTT(List<VTTAccount> lstorder)
        {
            //tạm bỏ nạp hộ ussd
            //lstorder.RemoveAll(order => order.Telco == "VTT" && order.Ussd > 0 && order.TopupType == "1");

            var ReturnData = new ReturnData();
            try
            {
                var token = Session[SessionsManager.SESSION_TOKEN].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                    Session[SessionsManager.SESSION_TOKEN] = token;
                }
                if (ServerProcess.AddVTTAccount(lstorder, token) > 0)
                {
                    ReturnData.ResponseCode = 1;
                    ReturnData.Description = "Thành công";
                }
                else
                {
                    ReturnData.ResponseCode = -99;
                    ReturnData.Description = "Có lỗi trong quá trình xử lý";
                }
               
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                ReturnData.ResponseCode = -99;
                ReturnData.Description = "Có lỗi trong quá trình xử lý";
            }

            return Json(ReturnData);

        }
    }
}