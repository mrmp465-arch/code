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
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Card.CMS.Controllers
{
    public class ReportController : Controller
    {
        private readonly IUsersService _userservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        private readonly ITransactionsService _transervice;
        private readonly IOrderReportsService _orderservice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } }
        public ReportController(IOrderReportsService orderservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _orderservice = orderservice;

        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportBid)]
        public ActionResult ReportBid()
        {
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;



            ViewBag.Title = "Báo cáo chiết khấu đua";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportDS)]
        public ActionResult ListReportDS(int userId, string FromDate, string ToDate)
        {
            ViewBag.userId = userId;


            var usser = _userservice.SelectByUserID(userId);
            ViewBag.UserType = usser.Type;
            var data = _orderservice.GetReportDS(usser, FromDate, ToDate);

            var dataLine = string.Empty;
            //var dataLine2 = string.Empty;
            //var dataLine3 = string.Empty;
            var datacategory = string.Empty;
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();
            var lstData = new List<ReportDaily>();
            //lstCate.Reverse();

            return PartialView(data);
        }

        [PermissionFilter(FunctionCode = FunctionCode.ReportDS)]
        public ActionResult ReportDS()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().Where(x => x.Type == 2).ToList();
                lstUser.Insert(0, new Users { UserID = 1, Username = "--Tất cả--" });
            }
            else
            {
                lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
            }

            ViewBag.UserList = lstUser;
            ViewBag.Title = "Đối soát";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportDaily)]
        public ActionResult Index()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().OrderBy(x => x.Type).ToList();
                lstUser.Insert(0, new Users { UserID = 1, Username = "--Tất cả--" });
            }
            else
            {
                if (CurrentFullUser.Type == 2)
                {
                    lstUser = _userservice.GetByEmail(CurrentFullUser.UserAPI);
                    lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                }
                else
                {
                    if (CurrentUser.Type == 3)
                    {
                        lstUser = _userservice.GetAll().Where(x => x.C1User == CurrentUser.Username).ToList();

                        lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                    }
                    else
                    {
                        if (CurrentUser.Type == 4)
                        {
                            lstUser = _userservice.GetAll().Where(x => x.CreatedUser == CurrentUser.Username).ToList();

                            lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                        }
                        else
                        {
                            lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                        }

                    }
                }

            }
            ViewBag.UserList = lstUser;
            ViewBag.Title = "Báo cáo doanh thu ngày";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportDaily)]
        public ActionResult ExportDS(int userId, string FromDate, string ToDate)
        {
            ViewBag.userId = userId;
            IFormatProvider culture = new CultureInfo("en-US", true);
            var _fromDate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", culture);
            var _toDate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", culture).AddDays(1).AddMilliseconds(-1);
            var usser = _userservice.SelectByUserID(userId);

            var data = _orderservice.GetReportDS(usser, FromDate, ToDate);
            //data.Reverse();

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Data");
                        worksheet.Column(1).Width = 20;
                        worksheet.Column(2).Width = 25;
                        worksheet.Column(3).Width = 25;
                        worksheet.Column(4).Width = 25;
                        worksheet.Column(5).Width = 25;
                        worksheet.Column(6).Width = 25;
                        worksheet.Column(7).Width = 25;
                        worksheet.Column(8).Width = 25;
                        var properties = new[]
                        {

                            "Ngày",
                            "VTT(không my)",
                            "MYVTT-FTTH",
                            "VNP",
                            "VMS",
                            "Zing",
                            "Garena",
                            "Đua giá",

                        };

                        for (int i = 0; i < properties.Length; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = properties[i];
                            worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                            //worksheet.Cells[1, i + 1].AutoFitColumns();
                        }

                        int row = 2;
                        foreach (var item in data)
                        {
                            worksheet.Row(row).Style.Numberformat.Format = "#,##0";


                            int col = 1;
                            worksheet.Cells[row, col].Value = item.Data;
                            col++;

                            worksheet.Cells[row, col].Value = item.TotalVTT;
                            col++;

                            worksheet.Cells[row, col].Value = item.TotalMyVTT;
                            col++;

                            worksheet.Cells[row, col].Value = item.TotalVNP;
                            col++;

                            worksheet.Cells[row, col].Value = item.TotalVMS;
                            col++;

                            worksheet.Cells[row, col].Value = item.TotalZing;
                            col++;

                            worksheet.Cells[row, col].Value = item.TotalGarena;
                            col++;

                            worksheet.Cells[row, col].Value = item.TotalBidFee;
                            row++;
                        }
                        worksheet.Row(row).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        worksheet.Row(row).Style.Font.Size = 14;
                        worksheet.Row(row).Style.Font.Name = "Times News Roman";
                        worksheet.Row(row).Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));



                        worksheet.Cells[row, 1].Value = "TỔNG";
                        worksheet.Cells[row, 1].Style.Font.Bold = true;
                        worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        //Sum
                        worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row, 7].Style.Numberformat.Format = "#,##0";
                        worksheet.Cells[row, 8].Style.Numberformat.Format = "#,##0";

                        worksheet.Cells[row, 2].Formula = "sum(B2:B" + (row - 1) + ")";
                        worksheet.Cells[row, 2].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        worksheet.Cells[row, 3].Formula = "sum(C2:C" + (row - 1) + ")";
                        worksheet.Cells[row, 3].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        worksheet.Cells[row, 4].Formula = "sum(D2:D" + (row - 1) + ")";
                        worksheet.Cells[row, 4].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        worksheet.Cells[row, 5].Formula = "sum(E2:E" + (row - 1) + ")";
                        worksheet.Cells[row, 5].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        worksheet.Cells[row, 6].Formula = "sum(F2:F" + (row - 1) + ")";
                        worksheet.Cells[row, 6].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        worksheet.Cells[row, 7].Formula = "sum(G2:G" + (row - 1) + ")";
                        worksheet.Cells[row, 7].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        worksheet.Cells[row, 8].Formula = "sum(H2:H" + (row - 1) + ")";
                        worksheet.Cells[row, 8].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "ReportDS" + usser.Username + ".xlsx");
            }
            catch (Exception)
            {
                return RedirectToAction("OrderActive");
            }
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportDaily)]
        public ActionResult ReportDaily(int userId, string FromDate, string ToDate, string telco, int type, string OrderNo)
        {
            ViewBag.userId = userId;
            IFormatProvider culture = new CultureInfo("en-US", true);
            var _fromDate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", culture);
            var _toDate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", culture).AddDays(1).AddMilliseconds(-1);
            var usser = _userservice.SelectByUserID(userId);

            var data = _orderservice.GetReportDaily(usser, FromDate, ToDate, telco, type, OrderNo);
            //data.Reverse();

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Data");
                        worksheet.Column(1).Width = 20;
                        worksheet.Column(2).Width = 20;
                        worksheet.Column(3).Width = 25;
                        worksheet.Column(4).Width = 25;
                        var properties = new[]
                        {

                            "Ngày",
                            "TotalAmount",
                            "Doanh thu sau chiết khấu",
                            "Doanh thu đua giá",

                        };
                        if (usser.Type <= 2)
                        {
                            properties = new[]
                            {

                            "Ngày",
                            "TotalAmount",
                            "Doanh thu đua giá",
                            };
                        }
                        for (int i = 0; i < properties.Length; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = properties[i];
                            worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                            //worksheet.Cells[1, i + 1].AutoFitColumns();
                        }

                        int row = 2;
                        foreach (var item in data)
                        {


                            int col = 1;
                            worksheet.Cells[row, col].Value = item.Data;
                            col++;

                            worksheet.Cells[row, col].Value = item.TotalAmount;
                            col++;

                            if (usser.Type > 2)
                            {
                                worksheet.Cells[row, col].Value = item.TotalRevenue;
                                col++;
                            }

                            worksheet.Cells[row, col].Value = item.TotalBidFee;
                            row++;
                        }
                        worksheet.Row(row).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        worksheet.Row(row).Style.Font.Size = 14;
                        worksheet.Row(row).Style.Font.Name = "Times News Roman";
                        worksheet.Row(row).Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));



                        worksheet.Cells[row, 1].Value = "TỔNG";
                        worksheet.Cells[row, 1].Style.Font.Bold = true;
                        worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        //Sum
                        worksheet.Cells[row, 2].Formula = "sum(B2:B" + (row - 1) + ")";
                        worksheet.Cells[row, 2].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));
                        if (usser.Type > 2)
                        {
                            worksheet.Cells[row, 3].Formula = "sum(C2:C" + (row - 1) + ")";
                            worksheet.Cells[row, 3].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                            worksheet.Cells[row, 4].Formula = "sum(D2:D" + (row - 1) + ")";
                            worksheet.Cells[row, 4].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));
                        }
                        else
                        {
                            worksheet.Cells[row, 3].Formula = "sum(C2:C" + (row - 1) + ")";
                            worksheet.Cells[row, 3].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                            //worksheet.Cells[row, 4].Formula = "sum(D2:D" + (row - 1) + ")";
                            //worksheet.Cells[row, 4].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));
                        }



                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "ReportDaily" + usser.Username + ".xlsx");
            }
            catch (Exception)
            {
                return RedirectToAction("OrderActive");
            }
        }

        [PermissionFilter(FunctionCode = FunctionCode.ReportBid)]
        public ActionResult ListReportBid(string FromDate, string TelCo)
        {

            var data = _orderservice.GetGroupMaxBid(FromDate, TelCo);
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();
            ViewBag.lstCate = lstCate;
            var lstBidRate = data.GroupBy(x => x.BidRate.GetValueOrDefault()).Select(a => a.Key).OrderBy(x => x).ToList();
            lstBidRate.Reverse();
            lstCate.Reverse();
            ViewBag.lstBidRate = lstBidRate;
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportDaily)]
        public ActionResult ListReport(int userId, string FromDate, string ToDate, string telco, int type, string OrderNo)
        {
            ViewBag.userId = userId;



            var usser = _userservice.SelectByUserID(userId);
            ViewBag.UserType = usser.Type;
            var data = _orderservice.GetReportDaily(usser, FromDate, ToDate, telco, type, OrderNo);
            var lstData = new List<ReportDaily>();


            var dataLine = string.Empty;
            //var dataLine2 = string.Empty;
            //var dataLine3 = string.Empty;
            var datacategory = string.Empty;
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();

            //lstCate.Reverse();
            foreach (var cate in lstCate)
            {
                datacategory += String.Format("'{0}',", StringUtils.FormatDay(cate));
                if (data.Exists(x => x.Data.Equals(cate)))
                {
                    dataLine += String.Format("{0},", data.Where(x => x.Data == cate).Sum(x => x.TotalAmount));
                }
                else
                {
                    dataLine += String.Format("{0},", 0);
                }

            }
            ViewBag.DataLine = String.Format("[{0}]", dataLine);
            //ViewBag.DataLine2 = String.Format("[{0}]", dataLine2);
            //ViewBag.DataLine3 = String.Format("[{0}]", dataLine3);
            ViewBag.datacategory = String.Format("[{0}]", datacategory);
            //data.Reverse();
            return PartialView(data);
        }

        [PermissionFilter(FunctionCode = FunctionCode.ReportOur)]
        public ActionResult ReportOur()
        {
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().OrderBy(x => x.Type).ToList();
                lstUser.Insert(1, new Users { UserID = 1, Username = "--Tất cả--" });
            }
            else
            {
                if (CurrentFullUser.Type == 2)
                {
                    lstUser = _userservice.GetByEmail(CurrentFullUser.UserAPI);
                    lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                }
                else
                {
                    if (CurrentUser.Type == 3)
                    {
                        lstUser = _userservice.GetAll().Where(x => x.C1User == CurrentUser.Username).ToList();

                        lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                    }
                    else
                    {
                        if (CurrentUser.Type == 4)
                        {
                            lstUser = _userservice.GetAll().Where(x => x.CreatedUser == CurrentUser.Username).ToList();

                            lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                        }
                        else
                        {
                            lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                        }

                    }
                }

            }

            ViewBag.UserList = lstUser;
            ViewBag.Title = "Báo cáo doanh thu giờ";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReportOur)]
        public ActionResult ListReportOur(int userId, string FromDate, string telco, int type = -1)
        {
            ViewBag.userId = userId;



            var usser = _userservice.SelectByUserID(userId);
            ViewBag.UserType = usser.Type;
            var data = _orderservice.GetReportOur(usser, FromDate, telco, type);

            var dataLine = string.Empty;
            //var dataLine2 = string.Empty;
            //var dataLine3 = string.Empty;
            var datacategory = string.Empty;
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();
            var lstData = new List<ReportDaily>();
            //lstCate.Reverse();
            foreach (var cate in lstCate)
            {
                datacategory += String.Format("'{0}',", cate);
                if (data.Exists(x => x.Data.Equals(cate)))
                {
                    dataLine += String.Format("{0},", data.Where(x => x.Data == cate).Sum(x => x.TotalAmount));
                }
                else
                {
                    dataLine += String.Format("{0},", 0);
                }

            }
            ViewBag.DataLine = String.Format("[{0}]", dataLine);
            //ViewBag.DataLine2 = String.Format("[{0}]", dataLine2);
            //ViewBag.DataLine3 = String.Format("[{0}]", dataLine3);
            ViewBag.datacategory = String.Format("[{0}]", datacategory);
            //data.Reverse();
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReporUser)]
        public ActionResult ReportUser()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;

            var lstUser = new List<Users>();
            lstUser = _userservice.GetByEmail(CurrentFullUser.UserAPI).Where(x => x.Type == 3).ToList();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().Where(x => x.Type == 2).ToList();
            }
            lstUser.Insert(0, new Users { UserID = 1, Username = "--Tất cả--" });
            ViewBag.UserList = lstUser;
            ViewBag.Title = "Báo cáo doanh thu đại lý";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.ReporUser)]
        public ActionResult ListReportUser(string FromDate, string ToDate, string telco, int type = -1, string userName = "")
        {

            var data = new List<OrderReportHistoryItem>();
            if (ViewBag.IsAdmin)
            {
                data = _orderservice.GetReportUserAdmin(FromDate, ToDate, telco, type);
            }
            else
            {
                data = _orderservice.GetReportUser(CurrentFullUser.UserAPI, FromDate, ToDate, telco, type);
            }



            var dataLine = string.Empty;
            //var dataLine2 = string.Empty;
            //var dataLine3 = string.Empty;
            var datacategory = string.Empty;
            var lstCate = data.GroupBy(x => x.Data).Select(a => a.Key).ToList();
            var lstUserName = data.GroupBy(x => x.UserName).Select(a => a.Key).OrderBy(x => x).ToList();

            ViewBag.lstCate = lstCate;
            ViewBag.lstUserName = lstUserName;
            foreach (var item in lstUserName)
            {

                dataLine += "{" + $"name:'{item}', y: {data.Where(x => x.UserName == item).Sum(x => x.TotalAmount)}" + "},";

            }

            ViewBag.DataLine = dataLine;
            //data.Reverse();
            if (!String.IsNullOrEmpty(userName) && userName != "--Tất cả--")
            {
                data = data.Where(x => x.UserName == userName).ToList();
            }
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderSearch)]
        public ActionResult Search()
        {
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderSearch)]
        public ActionResult ListOrder(string keyword)
        {

            var data = _orderservice.OrderSearch(keyword);

            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderSearch)]
        public ActionResult ListTransactionHistory(int transId)
        {

            var data = _orderservice.OrderHistory(transId);
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.UserWarning)]
        public ActionResult UserWarning()
        {
            ViewBag.Title = "Tổng đơn chờ";
            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }
            var data = ServerProcess.CountWaitingCache(token);
            return View(data);
        }

        

    }
}