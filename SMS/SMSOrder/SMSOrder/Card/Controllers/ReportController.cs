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
using System.Globalization;
using System.Web.Script.Serialization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using Newtonsoft.Json;

namespace SMS.CMS.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        private readonly IUsersService _userservice;
        private readonly IContactsService _contactservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        private readonly ITransactionsService _transervice;
        private readonly IGroupsService _groupservice;
        private readonly ICampaignsService _cservice;
        private readonly ISMSLogsService _smsservice;
        private readonly ISMSDictionaryService _dicservice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } }
        public ReportController(ISMSDictionaryService dicservice, ISMSLogsService smsservice, ICampaignsService cservice, IGroupsService groupservice, IContactsService contactservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _contactservice = contactservice;
            _groupservice = groupservice;
            _cservice = cservice;
            _smsservice = smsservice;
            _dicservice = dicservice;
        }
      
        public ActionResult AdminConfig()
        {

            ViewBag.SystemStatus = ServerProcess.GetSystemStatusCache();
            ViewBag.Title = "Cấu hình";
            return View();
        }
        [HttpPost]
        public JsonResult SaveConfig(string TestPort,string PriorityPort,string VMSPort,int Status,int VMSStatus)
        {
            var ReturnData = new ReturnData();

            try
            {
              
                ServerProcess.SetSystemStatusCache(Status);
               
                return Json(ReturnData);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                ReturnData.ResponseCode = -99;
                ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau";
                return Json(ReturnData);
            }
        }
        
    }
}