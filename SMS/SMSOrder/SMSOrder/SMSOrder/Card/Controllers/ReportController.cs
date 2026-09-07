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
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.ReportLockNumber)]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteNumber(string number, string type)
        {
            var ReturnData = new ReturnData();
            try
            {


                var lstCmsPort = ServerProcess.GetNumberLockCMSCache();
                lstCmsPort = lstCmsPort.Where(x => x.number != number).ToList();
                RedisCaching.Add("NumberLockCacheCMS", JsonConvert.SerializeObject(lstCmsPort), 3600 * 12);
                //xóa 4 T
                if (type == "1")
                {
                    var lst4TPort = ServerProcess.GetNumberLock4TCache();
                    lst4TPort = lst4TPort.Where(x => x != number).ToList();
                    RedisCaching.Add("NumberLock4TCache", JsonConvert.SerializeObject(lst4TPort), 3600 * 6);
                }
                else
                {
                    var lstLockPort = ServerProcess.GetNumberLockCache();
                    lstLockPort = lstLockPort.Where(x => x != number).ToList();
                    RedisCaching.Add("NumberLockCache", JsonConvert.SerializeObject(lstLockPort), 3600 * 6);

                }

                ReturnData.Description = "Xóa Thành Công";
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
        [PermissionFilter(FunctionCode = FunctionCode.ReportLockNumber)]
        public ActionResult ReportLockNumber()
        {
            var lstdata = ServerProcess.GetNumberLockCMSCache();

            ViewBag.Title = "Thông báo khóa sim";
            return View(lstdata);
        }
        [PermissionFilter(FunctionCode = FunctionCode.AdminConfig)]
        public ActionResult AdminConfig()
        {
            ViewBag.TestPort = ServerProcess.GetTestPortStringCache();
            ViewBag.PriorityPort = ServerProcess.GetPriorityPortStringCache();
            ViewBag.VMSPort = ServerProcess.GetVMSPortStringCache();
            ViewBag.SystemStatus = ServerProcess.GetSystemStatusCache();
            ViewBag.VMSStatus = ServerProcess.GetVMSStatusCache();
            ViewBag.Title = "Cấu hình";
            return View();
        }
        [HttpPost]
        public JsonResult SaveConfig(string TestPort,string PriorityPort,string VMSPort,int Status,int VMSStatus)
        {
            var ReturnData = new ReturnData();

            try
            {
                ServerProcess.SetTestPortCache(TestPort);
                ServerProcess.SetPriorityPortCache(PriorityPort);
                //ServerProcess.SetVMSPortCache(VMSPort);
                System.Threading.Thread.Sleep(300);
                ServerProcess.SetSystemStatusCache(Status);
                ServerProcess.SetVMSStatusCache(VMSStatus);
                ReturnData.ResponseCode = 1;
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
        [PermissionFilter(FunctionCode = FunctionCode.ReportDaily)]
        public ActionResult Daily()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin || ViewBag.IsSupport)
            {
                lstUser = _userservice.GetAll().ToList();
                lstUser.Insert(0, new Users { UserID = 1, Username = "--Tất cả--" });
            }
            else
            {
                lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
            }

            ViewBag.UserList = lstUser;
            ViewBag.Title = "Báo cáo sản lượng ngày";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportDaily)]
        public ActionResult ListReportDaily(int userId, string FromDate, string ToDate)
        {
            if (!ViewBag.IsAdmin && !ViewBag.IsSupport)
            {
                userId = CurrentUser.UserID;
            }
            ViewBag.userId = userId;



            var usser = _userservice.SelectByUserID(userId);

            ViewBag.UserType = usser.Type;
            var data = _smsservice.GetReportDaily(usser, FromDate, ToDate);

            var dataLine = string.Empty;
            var dataLine2 = string.Empty;
            var dataLine3 = string.Empty;
            var datacategory = string.Empty;
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();
            var lstData = new List<SMSReport>();

            //lstCate.Reverse();
            foreach (var cate in lstCate)
            {
                datacategory += String.Format("'{0}',", cate);
                if (data.Exists(x => x.Data.Equals(cate) && x.Telco == 1))
                {
                    dataLine += String.Format("{0},", data.Where(x => x.Data == cate && x.Telco == 1).Sum(x => x.Total));
                }
                else
                {
                    dataLine += String.Format("{0},", 0);
                }
                if (data.Exists(x => x.Data.Equals(cate) && x.Telco == 2))
                {
                    dataLine2 += String.Format("{0},", data.Where(x => x.Data == cate && x.Telco == 2).Sum(x => x.Total));
                }
                else
                {
                    dataLine2 += String.Format("{0},", 0);
                }

                if (data.Exists(x => x.Data.Equals(cate) && x.Telco == 3))
                {
                    dataLine3 += String.Format("{0},", data.Where(x => x.Data == cate && x.Telco == 3).Sum(x => x.Total));
                }
                else
                {
                    dataLine3 += String.Format("{0},", 0);
                }
            }
            ViewBag.lstCate = lstCate;
            ViewBag.DataLine = String.Format("[{0}]", dataLine);
            ViewBag.DataLine2 = String.Format("[{0}]", dataLine2);
            ViewBag.DataLine3 = String.Format("[{0}]", dataLine3);
            ViewBag.datacategory = String.Format("[{0}]", datacategory);
            data.Reverse();
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportUser)]
        public ActionResult ReportUser()
        {
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;

            ViewBag.Title = "Báo cáo user";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportRevenue)]
        public ActionResult ListRevenue(int userId,  string FromDate, string ToDate)
        {

           
            if (!ViewBag.IsAdmin)
            {
                userId = CurrentUser.UserID;
            }
            ViewBag.userId = userId;
            var usser = _userservice.SelectByUserID(userId);

            ViewBag.UserType = usser.Type;
            var data = _smsservice.GetReporRevenue(FromDate, ToDate, usser);
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();
            var lstUserName = data.GroupBy(x => x.CreatedUser).Select(a => a.Key).OrderBy(x => x).ToList();

            ViewBag.lstUser = _userservice.GetAll().ToList();

            ViewBag.lstCate = lstCate;
            ViewBag.lstUserName = lstUserName;
            return PartialView(data);

        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportTopup)]
        public ActionResult ListTopup(string from)
        {

            IFormatProvider culture = new CultureInfo("en-US", true);
            var BeginTime = DateTime.ParseExact(from, "MM/yyyy", culture);
            var toDate = DateTime.Now;
            var fromDate = new DateTime(BeginTime.Year, BeginTime.Month, 1);
            if (BeginTime.Month == 12)
                toDate = new DateTime(BeginTime.Year, BeginTime.Month, 31);
            else
                toDate = new DateTime(BeginTime.Year, BeginTime.Month + 1, 1).AddMilliseconds(-1);

            var data = _smsservice.GetReporTopup(fromDate, toDate);
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();


            ViewBag.lstCate = lstCate;

            return PartialView(data);

        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportTopup)]
        public ActionResult ListLockSim(string from)
        {

            IFormatProvider culture = new CultureInfo("en-US", true);
            var BeginTime = DateTime.ParseExact(from, "MM/yyyy", culture);

            var fromDate = new DateTime(BeginTime.Year, BeginTime.Month, 1);
            var toDate = DateTime.Now;
            if (BeginTime.Month == 12)
                toDate = new DateTime(BeginTime.Year, BeginTime.Month, 31);
            else
                toDate = new DateTime(BeginTime.Year, BeginTime.Month + 1, 1).AddMilliseconds(-1);

            var data = _smsservice.GetReporLockSim(fromDate, toDate);
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();


            ViewBag.lstCate = lstCate;

            return PartialView(data);

        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportLockSim)]
        public ActionResult LockSim()
        {
            ViewBag.fromDate = DateTime.Now.AddDays(-5);
            ViewBag.Title = "Báo cáo sim khóa";

            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportTopup)]
        public ActionResult Topup()
        {
            ViewBag.fromDate = DateTime.Now.AddDays(-5);
            ViewBag.Title = "Báo cáo nạp tiền";

            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportRevenue)]
        public ActionResult Revenue()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.Title = "Báo cáo doanh thu";
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().ToList();
                lstUser.Insert(0, new Users { UserID = 1, Username = "--Tất cả--" });
            }
            else
            {
                lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
            }

            ViewBag.UserList = lstUser;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportSim)]
        public ActionResult Sim()
        {
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            
            ViewBag.Title = "Báo cáo sản lượng sim";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportHour)]
        public ActionResult Hour()
        {
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin || ViewBag.IsSupport)
            {
                lstUser = _userservice.GetAll().ToList();
                lstUser.Insert(0, new Users { UserID = 1, Username = "--Tất cả--" });
            }
            else
            {
                lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
            }

            ViewBag.UserList = lstUser;
            ViewBag.Title = "Báo cáo sản lượng giờ";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportGenerate)]
        public ActionResult Generate()
        {
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now.AddDays(-1);
            var toDate2 = DateTime.Now.AddDays(-5);
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.toDate2 = toDate2;
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().ToList();
                lstUser.Insert(0, new Users { UserID = 1, Username = "--Tất cả--" });
            }
            else
            {
                lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
            }

            ViewBag.UserList = lstUser;
            ViewBag.Title = "Báo cáo tổng hợp";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportSim)]
        public ActionResult ListReportSim(string FromDate,string Telco)
        {


            var data = _smsservice.GetReporSim(FromDate, "");

            var lstdata = ServerProcess.GetNumberLockCMSCache();
            if(lstdata.Count>0)
            {

                data = data.Where(x => x.Data.Length > 0 && !lstdata.Where(b=>b.type.GetValueOrDefault() == 0).Select(a => a.number).Contains(x.Data)).ToList();
            }
            var lstPort = ServerProcess.GetAllPortInfo(Config.sn, Config.url).ResponseContent;
            foreach(var item in data)
            {
                if(lstPort.Exists(x => x.Number == item.Data))
                {
                    item.CreatedUser = lstPort.FirstOrDefault(x => x.Number == item.Data).Position.Port.ToString();
                    item.Telco = StringUtils.GetTelCo(item.Data);
                }
               
            }
            if (!string.IsNullOrEmpty(Telco))
                data = data.Where(x => x.Telco == int.Parse(Telco)).ToList();
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportUser)]
        public ActionResult ListReportUser(string FromDate)
        {


            var data = _smsservice.GetReporUser(FromDate, "");

            var dataLine = string.Empty;
            //var dataLine2 = string.Empty;
            //var dataLine3 = string.Empty;
            var datacategory = string.Empty;
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();
            var lstUserName = data.GroupBy(x => x.CreatedUser).Select(a => a.Key).OrderBy(x => x).ToList();

            ViewBag.lstCate = lstCate;
            ViewBag.lstUserName = lstUserName;
            foreach (var item in lstUserName)
            {

                dataLine += "{" + $"name:'{item}', y: {data.Where(x => x.CreatedUser == item).Sum(x => x.Total)}" + "},";

            }

            ViewBag.DataLine = dataLine;
            data.Reverse();
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportHour)]
        public ActionResult ListReportHour(int userId, string FromDate)
        {
            if (!ViewBag.IsAdmin && !ViewBag.IsSupport)
            {
                userId = CurrentUser.UserID;
            }
            ViewBag.userId = userId;



            var usser = _userservice.SelectByUserID(userId);

            ViewBag.UserType = usser.Type;
            var data = _smsservice.GetReporHour(usser, FromDate, "");

            var dataLine = string.Empty;
            var dataLine2 = string.Empty;
            var dataLine3 = string.Empty;
            var datacategory = string.Empty;
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();
            var lstData = new List<SMSReport>();
            ViewBag.lstCate = lstCate;

            foreach (var cate in lstCate)
            {
                datacategory += String.Format("'{0}',", cate);
                if (data.Exists(x => x.Data.Equals(cate) && x.Telco == 1))
                {
                    dataLine += String.Format("{0},", data.Where(x => x.Data == cate && x.Telco == 1).Sum(x => x.Total));
                }
                else
                {
                    dataLine += String.Format("{0},", 0);
                }
                if (data.Exists(x => x.Data.Equals(cate) && x.Telco == 2))
                {
                    dataLine2 += String.Format("{0},", data.Where(x => x.Data == cate && x.Telco == 2).Sum(x => x.Total));
                }
                else
                {
                    dataLine2 += String.Format("{0},", 0);
                }

                if (data.Exists(x => x.Data.Equals(cate) && x.Telco == 3))
                {
                    dataLine3 += String.Format("{0},", data.Where(x => x.Data == cate && x.Telco == 3).Sum(x => x.Total));
                }
                else
                {
                    dataLine3 += String.Format("{0},", 0);
                }
            }
            lstCate.Reverse();
            ViewBag.DataLine = String.Format("[{0}]", dataLine);
            ViewBag.DataLine2 = String.Format("[{0}]", dataLine2);
            ViewBag.DataLine3 = String.Format("[{0}]", dataLine3);
            ViewBag.datacategory = String.Format("[{0}]", datacategory);
            data.Reverse();
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportGenerate)]
        public ActionResult ListReportGerenate(int userId, string FromDate, string Telco, string ToDate, string ToDate2)
        {
            if (!ViewBag.IsAdmin)
            {
                userId = CurrentUser.UserID;
            }
            ViewBag.userId = userId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ToDate2 = ToDate2;

            var usser = _userservice.SelectByUserID(userId);

            ViewBag.UserType = usser.Type;
            var data = _smsservice.GetReporHour(usser, FromDate, Telco);
            var data2 = _smsservice.GetReporHour(usser, ToDate, Telco);
            var data3 = _smsservice.GetReporHour(usser, ToDate2, Telco);
            var dataLine = string.Empty;
            var dataLine2 = string.Empty;
            var dataLine3 = string.Empty;
            var datacategory = string.Empty;
            var lstCate = data3.GroupBy(x => x.Data).Select(a => a.Key).ToList();
            var lstData = new List<SMSReport>();
            ViewBag.lstCate = lstCate;

            foreach (var cate in lstCate)
            {
                datacategory += String.Format("'{0}',", cate);
                if (data.Exists(x => x.Data.Equals(cate)))
                {
                    dataLine += String.Format("{0},", data.Where(x => x.Data == cate).Sum(x => x.Total));
                }
                else
                {
                    dataLine += String.Format("{0},", 0);
                }
                if (data2.Exists(x => x.Data.Equals(cate)))
                {
                    dataLine2 += String.Format("{0},", data2.Where(x => x.Data == cate).Sum(x => x.Total));
                }
                else
                {
                    dataLine2 += String.Format("{0},", 0);
                }

                if (data3.Exists(x => x.Data.Equals(cate)))
                {
                    dataLine3 += String.Format("{0},", data3.Where(x => x.Data == cate).Sum(x => x.Total));
                }
                else
                {
                    dataLine3 += String.Format("{0},", 0);
                }
            }
            lstCate.Reverse();
            ViewBag.DataLine = String.Format("[{0}]", dataLine);
            ViewBag.DataLine2 = String.Format("[{0}]", dataLine2);
            ViewBag.DataLine3 = String.Format("[{0}]", dataLine3);
            ViewBag.datacategory = String.Format("[{0}]", datacategory);
            //data.Reverse();
            return PartialView();
        }
    }
}