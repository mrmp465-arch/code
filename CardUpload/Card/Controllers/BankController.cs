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
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using OfficeOpenXml;

namespace Card.CMS.Controllers
{
    public class BankController : Controller
    {
        private readonly IUsersService _userservice;
        private readonly IAccountTokenService _accounttokenservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        private readonly IOrderTempsService _oservice;
        // private readonly IOrderReportsService _orderservice;
        private readonly ITransactionsService _transervice;
        //private readonly IBidHistoryService _bidHistoryservice;

        private readonly IBankGateService _bankgateservice;
        private readonly IBankCashService _bankcashervice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } set { } }
        public BankController(IBankCashService bankcashervice, IBankGateService bankgateservice, IOrderTempsService oservice, IAccountTokenService accounttokenservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _accounttokenservice = accounttokenservice;
            _oservice = oservice;
            _bankcashervice = bankcashervice;
            _bankgateservice = bankgateservice;
        }
        [PermissionFilter(FunctionCode = FunctionCode.BankLog)]
        public ActionResult Index()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddMinutes(-1);

            var PartList = new List<Users>();
            if (CurrentUser.Type == 1)
            {
                PartList = _userservice.GetAll().Where(x => x.Type == 2).ToList();

            }
            PartList.Insert(0, new Users { UserID = -1, Username = "--Tài khoản-" });
            //lstBankCode.Insert(0, new BankGateAPI { BankCode = "--BankCode--"});
            //ViewBag.lstBankCode = lstBankCode;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.PartList = PartList;


            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.BankLog)]
        public ActionResult ListBankLog(string FromDate, string ToDate, int? status, int? userId,string orderNo)
        {
            DateTime fromdate = DateTime.ParseExact(FromDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(ToDate, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            string newFromDate = fromdate.ToString("MM/dd/yyyy HH:mm:ss");
            string newToDate = todate.ToString("MM/dd/yyyy HH:mm:ss");

            int UserId = userId.GetValueOrDefault();
            if (CurrentUser.Type != 1 )
            {
                UserId = CurrentFullUser.UserID;
            }
            ViewBag.UserType = CurrentUser.Type;
            BankGateAPI _CardAPILog = new BankGateAPI();
            int Status = status == null ? 1 : (int)status;
            var data = _bankgateservice.GetFilter(UserId, 100, orderNo, -1, Status, FromDate,ToDate);
            return PartialView(data);
        }

        [PermissionFilter(FunctionCode = FunctionCode.BankReport)]
        public ActionResult Report()
        {
            var PartList = new List<Users>();
            if (CurrentUser.Type == 1)
            {
                PartList = _userservice.GetAll().Where(x => x.Type == 2).ToList();

            }
            PartList.Insert(0, new Users { UserID = -1, Username = "--Tài khoản-" });


            ViewBag.PartList = PartList;
           
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.BankReport)]
        public ActionResult ListBankReport(int Year, int Month, int Day, int? userId)
        {

            int UserId = userId.GetValueOrDefault();
            if (CurrentUser.Type != 1)
            {
                UserId = CurrentFullUser.UserID;
            }
            ViewBag.UserType = CurrentUser.Type;
            int totalTransaction = 0;
            long totalAmount = 0;
            var Data = _bankgateservice.Report(UserId, Year, Month, Day, ref totalTransaction, ref totalAmount);
            return PartialView(Data);
        }

        [PermissionFilter(FunctionCode = FunctionCode.BankDS)]
        public ActionResult ReportDS()
        {

            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            var PartList = new List<Users>();
            if (CurrentUser.Type == 1)
            {
                PartList = _userservice.GetAll().Where(x => x.Type == 2).ToList();

            }
            PartList.Insert(0, new Users { UserID = -1, Username = "--Tài khoản-" });


            ViewBag.PartList = PartList;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.BankDS)]
        public ActionResult ListBankLogDSExcel(string FromDate, string ToDate, int? userId)
        {
            DateTime fromdate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1);


            int UserId = userId.GetValueOrDefault();
            if (CurrentUser.Type != 1)
            {
                UserId = CurrentFullUser.UserID;
            }
            var Data = _bankgateservice.ListReportDoiSoat(UserId,  fromdate, todate, 1);
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Bank");
                        worksheet.Column(1).Width = 14;
                        worksheet.Column(2).Width = 19;
                        worksheet.Column(3).Width = 19;
                        worksheet.Column(4).Width = 14;
                        worksheet.Column(5).Width = 19;
                        worksheet.Column(6).Width = 19;
                        worksheet.Column(7).Width = 19;
                        worksheet.Column(8).Width = 19;
                        worksheet.Column(9).Width = 19;
                        worksheet.Cells["A1"].Value = "STT";
                       
                        worksheet.Cells["B1"].Value = "TransactionID";
                        worksheet.Cells["C1"].Value = "BankCode";
                        worksheet.Cells["D1"].Value = "Nội dung";
                        worksheet.Cells["E1"].Value = "Số tiền tạo lệnh";
                        worksheet.Cells["F1"].Value = "Số tiền";
                        worksheet.Cells["G1"].Value = "Thời gian tạo";
                        worksheet.Cells["H1"].Value = "Thời gian thực hiện";
                        //worksheet.Cells["H1"].Value = "Thời gian tạo";
                        //worksheet.Cells["I1"].Value = "Thời gian thực hiện";
                        worksheet.Row(1).Style.Font.Bold = true;

                        int rowTotal = 1;
                        foreach (var item in Data)
                        {
                            rowTotal++;
                            worksheet.Cells["A" + rowTotal].Value = rowTotal-1;
                            worksheet.Cells["B" + rowTotal].Value = item.TransactionID;
                            worksheet.Cells["C" + rowTotal].Value = item.BankCode;
                            
                            worksheet.Cells["D" + rowTotal].Value = item.OrderNo;
                            worksheet.Cells["E" + rowTotal].Value = item.Amount;
                            worksheet.Cells["F" + rowTotal].Value = item.TotalAmount;
                            //worksheet.Cells["F" + rowTotal].Value = item.BankAccountNumber;
                            //worksheet.Cells["G" + rowTotal].Value = item.BankAccountName;
                            worksheet.Cells["G" + rowTotal].Value = item.CreatedTime.ToString("dd/MM/yyyy HH:mm:ss");
                            worksheet.Cells["H" + rowTotal].Value = item.LastTime.ToString("dd/MM/yyyy HH:mm:ss");

                            
                        }
                        rowTotal++;
                        worksheet.Row(rowTotal).Style.Font.Bold = true;
                        worksheet.Cells["A" + rowTotal].Value = "Tổng";
                        worksheet.Cells["F" + rowTotal].Formula = $"sum(F2:F{ rowTotal - 1})";
                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                Response.AddHeader("Content-disposition", "attachment; filename=Bank.xlsx");
                Response.ContentType = "text/xls";
                Response.BinaryWrite(bytes);
                Response.End();
            }
            catch (Exception ex)
            {

                NLogLogger.PublishException(ex);
                return RedirectToAction("ReportDS");
            }
            return RedirectToAction("ReportDS");
        }
        [PermissionFilter(FunctionCode = FunctionCode.BankDS)]
        public ActionResult ListBankLogDS(string FromDate, string ToDate, int? userId)
        {
            DateTime fromdate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1);


            int UserId = userId.GetValueOrDefault();
            if (CurrentUser.Type != 1)
            {
                UserId = CurrentFullUser.UserID;
            }
           
            //int Amount = 0;
            var Data = _bankgateservice.ReportDoiSoat(UserId, fromdate, todate, 1);
            ViewBag.lstPartner = Data.GroupBy(x => x.UserName).Select(a => a.Key).OrderBy(x => x).ToList();

            return PartialView(Data);

        }

    }
}