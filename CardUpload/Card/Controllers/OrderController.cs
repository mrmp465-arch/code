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
using Card.CMS.Helper;
using SMS.Data.Factory;
using System.Configuration;

namespace Card.CMS.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUsersService _userservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        private readonly ITransactionsService _transervice;
        private readonly IBidHistoryService _bidHistoryservice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } set { } }
        public OrderController(IBidHistoryService bidHistoryservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _bidHistoryservice = bidHistoryservice;
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderTranSearch)]
        public ActionResult TransactionSearch(string OrderNo, int? confirm)
        {
            ViewBag.OrderNo = OrderNo;
            ViewBag.confirm = -1;
            if (confirm != null)
            {
                ViewBag.confirm = confirm.GetValueOrDefault();
            }
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().OrderBy(x => x.Type).ToList();
                lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
            }
            else
            {
                if (CurrentFullUser.Type == 2)
                {
                    lstUser = _userservice.GetByEmail(CurrentFullUser.UserAPI);
                    lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                }
                else
                {
                    if (CurrentUser.Type == 3)
                    {
                        lstUser = _userservice.GetAll().Where(x => x.C1User == CurrentUser.Username).ToList();

                        lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                        lstUser.Insert(1, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                    }
                    else
                    {
                        if (CurrentUser.Type == 4)
                        {
                            lstUser = _userservice.GetAll().Where(x => x.CreatedUser == CurrentUser.Username).ToList();

                            lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                            lstUser.Insert(1, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                        }
                        else
                        {
                            lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                        }

                    }
                }
            }
            ViewBag.UserList = lstUser;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderTranSearch)]
        public ActionResult ListTransactionSearch(int userId, int status, int confirm, int top, string orderNo, string telco, string mobile,int ussd)
        {
            var subUser = "";
            var token = "";
            var user = new Users();
            if (ViewBag.IsAdmin)
            {
                if (userId != -1)
                {
                    user = _userservice.SelectByUserID(userId);
                    token = ServerProcess.GetUserTokenCache(user.UserAPI, user.PasswordAPI);
                    Session[SessionsManager.SESSION_TOKEN] = token;

                    orderNo = String.Format("_{0}_", user.Config);
                    if (user.Type == 2)
                    {
                        orderNo = "";
                    }


                }
                else
                {
                    token = Session[SessionsManager.SESSION_TOKEN].ToString();
                    if (string.IsNullOrEmpty(token))
                    {
                        token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                        Session[SessionsManager.SESSION_TOKEN] = token;
                    }
                }

            }
            else
            {
                token = Session[SessionsManager.SESSION_TOKEN].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                    Session[SessionsManager.SESSION_TOKEN] = token;
                }

                //cấp super admin
                if (CurrentUser.Type == 2)
                {
                    if (userId == -1)
                    {
                        //lấy tất con của nó
                        orderNo = "";
                    }
                    else
                    {
                        user = _userservice.SelectByUserID(userId);
                        subUser = "";
                        orderNo = String.Format("_{0}_", user.Config);
                        //orderNo = String.Format("{0}_", user.Config);
                    }
                }
                //cấp 1
                else
                {
                    if (CurrentUser.Type <= 4)
                    {
                        if (userId == -1)
                        {
                            //lấy tất con của nó
                            orderNo = String.Format("_{0}_", CurrentFullUser.Config);
                        }
                        else
                        {
                            user = _userservice.SelectByUserID(userId);
                            subUser = "";
                            orderNo = String.Format("_{0}_", user.Config);
                            //orderNo = String.Format("{0}_", user.Config);
                        }
                    }
                    //cấp 3 type=5
                    else
                    {
                        user = CurrentFullUser;
                        subUser = user.Username;
                        orderNo = "";
                        //orderNo = String.Format("{0}_", CurrentFullUser.Config);

                    }

                }
            }

            int? Status = null;
            if (status > -1000)
            {
                Status = status;
            }
            int? Confirm = null;
            if (confirm > -1)
            {
                Confirm = confirm;
            }

            ViewBag.OrderNo = orderNo;
            ViewBag.Config = String.Format("_{0}_", CurrentFullUser.Config);

            ViewBag.UserType = CurrentUser.Type;
            ViewBag.Username = CurrentUser.Username;
            ViewBag.UpdateP = CurrentFullUser.Username.Contains("_admin");
            var data = ServerProcess.ListTransaction(token, top, telco, orderNo, Status, Confirm, subUser, mobile,ussd);


            ViewBag.IsBid = true;
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult TransactionOrder(string OrderNo, int? confirm)
        {
            ViewBag.OrderNo = OrderNo;
            ViewBag.confirm = -1;
            if (confirm != null)
            {
                ViewBag.confirm = confirm.GetValueOrDefault();
            }
            return View();
        }

        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        [ValidateInput(false)]
        public ActionResult ListTransactionOrder(int status, int confirm, int top, string orderNo, string telco)
        {

            int? Status = null;
            if (status > -1000)
            {
                Status = status;
            }
            int? Confirm = null;
            if (confirm > -1)
            {
                Confirm = confirm;
            }
            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }
            ViewBag.OrderNo = orderNo;
            ViewBag.Config = String.Format("_{0}_", CurrentFullUser.Config);

            ViewBag.UserType = CurrentUser.Type;
            ViewBag.Username = CurrentUser.Username;
            ViewBag.UpdateP = CurrentFullUser.Username.Contains("_admin");
            var data = ServerProcess.ListTransaction(token, top, telco, orderNo, Status, Confirm, "", "");
            if (data.Count() > 0)
                data = data.Where(x => x.OrderNo.Equals(orderNo)).ToList();

            ViewBag.IsBid = true;
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult ReportTransactionOrder(string orderNo, int confirm)
        {
            if (CurrentFullUser == null)
            {
                var m_Users = _userservice.GetByUsername(CurrentUser.Username);
                Session[SessionsManager.SESSION_USER_FULL] = m_Users;
                CurrentFullUser = m_Users;
            }
            if (CurrentFullUser.Type > 2)
            {
                if (!orderNo.Contains(CurrentFullUser.Config))
                {
                    return RedirectToAction("OrderActive");
                }
            }

            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }
            ViewBag.OrderNo = orderNo;

            var data = ServerProcess.ListTransaction(token, 1000, "", orderNo, null, confirm);
            if (data.Count() > 0)
                data = data.Where(x => x.OrderNo.Equals(orderNo)).ToList();
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Data");
                        worksheet.Column(1).Width = 15;
                        worksheet.Column(2).Width = 30;
                        worksheet.Column(3).Width = 20;
                        worksheet.Column(4).Width = 12;
                        worksheet.Column(5).Width = 15;
                        worksheet.Column(6).Width = 15;
                        worksheet.Column(7).Width = 15;
                        worksheet.Column(8).Width = 15;
                        worksheet.Column(9).Width = 12;
                        worksheet.Column(10).Width = 12;
                        worksheet.Column(11).Width = 10;
                        worksheet.Column(12).Width = 7;
                        worksheet.Column(13).Width = 7;
                        worksheet.Column(14).Width = 7;
                        worksheet.Column(15).Width = 18;
                        worksheet.Column(16).Width = 18;
                        worksheet.Column(17).Width = 18;
                        //worksheet.Column(1).Style.WrapText = true;
                        //worksheet.Column(2).Style.WrapText = true;
                        //worksheet.Column(3).Style.WrapText = true;
                        //worksheet.Column(4).Style.WrapText = true;
                        //worksheet.Column(5).Style.WrapText = true;
                        var properties = new[]
                        {
                            "TransactionID",
                            "OrderNo",
                            //"RequestNo",
                            "Mobile",
                            "Telco",
                             "AmountTopupSuccess",
                            "Amount",
                            "Phí đua",
                            "State",
                            "Status",

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


                            int col = 1;

                            worksheet.Cells[row, col].Value = item.TransactionID;
                            col++;

                            worksheet.Cells[row, col].Value = item.OrderNo;
                            col++;

                            //worksheet.Cells[row, col].Value = item.RequestNo;
                            //col++;

                            worksheet.Cells[row, col].Value = item.Mobile;
                            col++;

                            worksheet.Cells[row, col].Value = item.Telco.ToUpper();
                            col++;

                            //worksheet.Cells[row, col].Value = item.TopupType;
                            //col++;


                            if (item.AmountTopupSuccess > item.Amount)
                                item.AmountTopupSuccess = item.Amount;
                            worksheet.Cells[row, col].Value = item.AmountTopupSuccess;
                            col++;

                            worksheet.Cells[row, col].Value = item.Amount;
                            col++;

                            worksheet.Cells[row, col].Value = item.BidFee;
                            col++;
                            if (item.AmountTopupSuccess == 0)
                            {
                                worksheet.Cells[row, col].Value = "Chưa nạp";
                                col++;
                            }
                            else
                            {
                                if (item.AmountTopupSuccess == item.Amount)
                                {
                                    worksheet.Cells[row, col].Value = "Đủ";
                                    col++;
                                }
                                else
                                {
                                    worksheet.Cells[row, col].Value = "Chưa đủ";
                                    col++;
                                }
                            }


                            worksheet.Cells[row, col].Value = HtmlHelpers.GetTranStatusNormal(item.Status);
                            col++;

                            //worksheet.Cells[row, col].Value = item.IsConfirm;
                            //col++;


                            row++;
                        }
                        worksheet.Row(row).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        worksheet.Row(row).Style.Font.Size = 14;
                        worksheet.Row(row).Style.Font.Name = "Times News Roman";
                        worksheet.Row(row).Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));



                        worksheet.Cells[row, 4].Value = "TỔNG";
                        worksheet.Cells[row, 4].Style.Font.Bold = true;
                        worksheet.Cells[row, 4].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        //Sum
                        worksheet.Cells[row, 5].Formula = "sum(E2:E" + (row - 1) + ")";
                        worksheet.Cells[row, 5].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        worksheet.Cells[row, 6].Formula = "sum(F2:F" + (row - 1) + ")";
                        worksheet.Cells[row, 6].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));
                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", orderNo + ".xlsx");
            }
            catch (Exception ex)

            {
                NLogLogger.PublishException(ex);
                return RedirectToAction("OrderActive");
            }

        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult ListTransactionHistory(int transId)
        {


            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }

            var data = ServerProcess.ListTransactionHistory(token, transId, null);
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult ReportOrderConfirm(int userId, int top, string keyword, string telco)
        {
            var data = new List<OrderGroup>();
            var orderNo = "";
            var token = "";
            var user = new Users();
            var subUser = "";
            if (ViewBag.IsAdmin)
            {
                if (userId != -1)
                {
                    user = _userservice.SelectByUserID(userId);
                    token = ServerProcess.GetUserTokenCache(user.UserAPI, user.PasswordAPI);
                    //orderNo = String.Format("{0}_", user.Config);
                    orderNo = "";
                    subUser = user.Username;
                }
                else
                {
                    token = Session[SessionsManager.SESSION_TOKEN].ToString();
                    if (string.IsNullOrEmpty(token))
                    {
                        token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                        Session[SessionsManager.SESSION_TOKEN] = token;
                    }
                }

            }
            else
            {
                token = Session[SessionsManager.SESSION_TOKEN].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                    Session[SessionsManager.SESSION_TOKEN] = token;
                }

                //cấp super admin
                if (CurrentUser.Type == 2)
                {
                    if (userId == -1)
                    {
                        //lấy tất con của nó
                        orderNo = "";
                    }
                    else
                    {
                        user = _userservice.SelectByUserID(userId);
                        subUser = user.Username;
                        orderNo = "";
                        //orderNo = String.Format("{0}_", user.Config);
                    }
                }
                //cấp 1
                else
                {
                    if (CurrentUser.Type == 3)
                    {
                        if (userId == -1)
                        {
                            //lấy tất con của nó
                            orderNo = String.Format("{0}_", CurrentFullUser.Config);
                        }
                        else
                        {
                            user = _userservice.SelectByUserID(userId);
                            subUser = user.Username;
                            orderNo = "";
                            //orderNo = String.Format("{0}_", user.Config);
                        }
                    }
                    //cấp 2 type=4
                    else
                    {
                        //orderNo = String.Format("{0}_", CurrentFullUser.Config);
                        user = CurrentFullUser;
                        subUser = user.Username;
                        orderNo = "";
                    }

                }
            }
            data = ServerProcess.ListGroupFinish(token, top, telco, orderNo, subUser);
            if (userId > 0)
            {
                //tk cấp 1 chỉ nhìn đc nó
                if (user.Type <= 3)
                {
                    data = data.Where(x => x.OrderNo.ToCharArray().Count(c => c == '_') == 2).ToList();

                }
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                if (data != null)
                    data = data.Where(x => x.OrderNo.Contains(keyword)).ToList();
            }
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Data");
                        worksheet.Column(1).Width = 30;
                        worksheet.Column(2).Width = 20;
                        worksheet.Column(3).Width = 15;
                        worksheet.Column(4).Width = 15;
                        worksheet.Column(5).Width = 20;
                        worksheet.Column(6).Width = 20;
                        worksheet.Column(7).Width = 20;

                        //worksheet.Column(1).Style.WrapText = true;
                        //worksheet.Column(2).Style.WrapText = true;
                        //worksheet.Column(3).Style.WrapText = true;
                        //worksheet.Column(4).Style.WrapText = true;
                        //worksheet.Column(5).Style.WrapText = true;
                        var properties = new[]
                        {

                            "OrderNo",
                            "Telco",
                             "TotalSuccess",
                            "TotalTrans",
                            "AmountSuccess",
                            "Amount",


                            "CreatedTime",

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


                            int col = 1;


                            worksheet.Cells[row, col].Value = item.OrderNo;
                            col++;



                            worksheet.Cells[row, col].Value = item.Telco.ToUpper();
                            col++;


                            worksheet.Cells[row, col].Value = item.TotalTranSuccess;
                            col++;

                            worksheet.Cells[row, col].Value = item.TotalTrans;
                            col++;

                            if (item.TotalAmountSuccess > item.TotalAmount)
                                item.TotalAmountSuccess = item.TotalAmount;

                            worksheet.Cells[row, col].Value = item.TotalAmountSuccess;
                            col++;

                            worksheet.Cells[row, col].Value = item.TotalAmount;
                            col++;


                            worksheet.Cells[row, col].Value = item.CreatedTime.ToString("dd/MM/yyy");
                            col++;


                            row++;
                        }
                        worksheet.Row(row).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        worksheet.Row(row).Style.Font.Size = 14;
                        worksheet.Row(row).Style.Font.Name = "Times News Roman";
                        worksheet.Row(row).Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));



                        worksheet.Cells[row, 2].Value = "TỔNG";
                        worksheet.Cells[row, 2].Style.Font.Bold = true;
                        worksheet.Cells[row, 2].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        //Sum
                        worksheet.Cells[row, 3].Formula = "sum(C2:C" + (row - 1) + ")";
                        worksheet.Cells[row, 3].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        worksheet.Cells[row, 4].Formula = "sum(D2:D" + (row - 1) + ")";
                        worksheet.Cells[row, 4].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        worksheet.Cells[row, 5].Formula = "sum(E2:E" + (row - 1) + ")";
                        worksheet.Cells[row, 5].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));


                        worksheet.Cells[row, 6].Formula = "sum(F2:F" + (row - 1) + ")";
                        worksheet.Cells[row, 6].Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));

                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Order.xlsx");
            }
            catch (Exception)
            {
                return RedirectToAction("OrderActive");
            }
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult ReportTransactionHistory(int transId, string orderNo)
        {
            if (CurrentFullUser.Type > 2)
            {
                if (!orderNo.Contains("_" + CurrentFullUser.Config))
                {
                    return RedirectToAction("OrderActive");
                }
            }

            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }

            var data = ServerProcess.ListTransactionHistory(token, transId, null);

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Data");
                        worksheet.Column(1).Width = 15;
                        worksheet.Column(2).Width = 30;
                        worksheet.Column(3).Width = 20;
                        worksheet.Column(4).Width = 12;
                        worksheet.Column(5).Width = 8;
                        worksheet.Column(6).Width = 10;
                        worksheet.Column(7).Width = 12;
                        //worksheet.Column(8).Width = 12;
                        worksheet.Column(8).Width = 18;
                        worksheet.Column(9).Width = 18;
                        worksheet.Column(10).Width = 8;
                        worksheet.Column(11).Width = 18;
                        worksheet.Column(12).Width = 18;
                        //worksheet.Column(1).Style.WrapText = true;
                        //worksheet.Column(2).Style.WrapText = true;
                        //worksheet.Column(3).Style.WrapText = true;
                        //worksheet.Column(4).Style.WrapText = true;
                        //worksheet.Column(5).Style.WrapText = true;
                        var properties = new[]
                        {
                            "TransactionID",
                            "OrderNo",
                            "FullName",
                            "Mobile",
                            "Telco",
                            "TopupType",
                            "Amount",
                            //"AmountUser",
                            "CardSerial",
                            "CardCode",


                            "Status",

                            "CreatedTime",
                            "LastTime"
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


                            int col = 1;

                            worksheet.Cells[row, col].Value = item.TransactionID;
                            col++;

                            worksheet.Cells[row, col].Value = item.OrderNo;
                            col++;

                            worksheet.Cells[row, col].Value = item.FullName;
                            col++;

                            worksheet.Cells[row, col].Value = item.Mobile;
                            col++;

                            worksheet.Cells[row, col].Value = item.Telco.ToUpper();
                            col++;

                            worksheet.Cells[row, col].Value = item.TopupType;
                            col++;

                            worksheet.Cells[row, col].Value = item.Amount;
                            col++;

                            //worksheet.Cells[row, col].Value = item.AmountUser;
                            //col++;

                            worksheet.Cells[row, col].Value = item.CardSerial;
                            col++;

                            worksheet.Cells[row, col].Value = item.CardCode;
                            col++;



                            worksheet.Cells[row, col].Value = item.Status;
                            col++;



                            worksheet.Cells[row, col].Value = item.CreateTime.ToString("dd/MM/yyyy HH:mm");
                            col++;

                            worksheet.Cells[row, col].Value = item.LastTime.ToString("dd/MM/yyyy HH:mm");
                            col++;
                            row++;
                        }
                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", orderNo + "-" + transId.ToString() + ".xlsx");
            }
            catch (Exception)
            {
                return RedirectToAction("OrderActive");
            }
            //Response.Clear();
            //Response.Buffer = true;
            ////Response.Charset = "UTF-8"; 
            //Response.AppendHeader("Content-Disposition", "attachment;filename=" + orderNo + transId.ToString() + ".xls");
            //Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            //Response.ContentType = "application/ms-excel";

            //var myCItrad = new CultureInfo("VI-VN", true);
            //var oStringWriter = new StringWriter(myCItrad);
            //var oHtmlTextWriter = new HtmlTextWriter(oStringWriter);

            //var grid = new DataGrid { DataSource = data };
            //grid.DataBind();
            //grid.GridLines = GridLines.None;
            //grid.BorderWidth = 0;

            //grid.HeaderStyle.BackColor = Color.FromArgb(79, 129, 189);
            //grid.HeaderStyle.ForeColor = Color.White;
            //grid.BackColor = Color.Transparent;

            //grid.RenderControl(oHtmlTextWriter);

            //string style = @"<style> td{ mso-number-format:\@; } </style>";
            //Response.Write(style);
            //Response.ContentType = "application/text";
            //Response.Write(oStringWriter.ToString());
            //Response.Flush();
            //Response.End();
            //return RedirectToAction("OrderActive");
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult OrderActive()
        {
            try
            {
                var lstUser = new List<Users>();
                if (ViewBag.IsAdmin)
                {
                    lstUser = _userservice.GetAll().OrderBy(x => x.Type).ToList();
                    lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                }
                else
                {
                    if (CurrentFullUser.Type == 2)
                    {
                        lstUser = _userservice.GetByEmail(CurrentFullUser.UserAPI);
                        lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                    }
                    else
                    {
                        if (CurrentUser.Type == 3)
                        {
                            lstUser = _userservice.GetAll().Where(x => x.C1User == CurrentUser.Username).ToList();

                            lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                            lstUser.Insert(1, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                        }
                        else
                        {
                            if (CurrentUser.Type == 4)
                            {
                                lstUser = _userservice.GetAll().Where(x => x.CreatedUser == CurrentUser.Username).ToList();

                                lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                                lstUser.Insert(1, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                            }   
                            else
                            {
                                lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                            }    
                                
                        }
                    }

                }
                ViewBag.UserList = lstUser;
                ViewBag.Title = "Danh sách đơn active";
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult GetDescription(long id)
        {
            var order = AbstractDAOFactory.Instance().OrderReportsService().Get(id);
            return PartialView(order);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult Edit(long id)
        {
            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }
            var order = ServerProcess.GetOrder(token, id);
            //if (!order.OrderNo.Contains(CurrentFullUser.Config ))
            //{
            //    return RedirectToAction("Home", "Index");
            //}
            if (CurrentUser.Username != order.SubUser && CurrentUser.Type>2)
                return null;
              return PartialView(order);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult ResetTran(OrderOutput item)
        {
            var ReturnData = new ReturnData();
            if (CurrentFullUser.Type > 2)
            {
                if (!item.OrderNo.Contains("_" + CurrentFullUser.Config + "_") && CurrentFullUser.Type > 2)
                {
                    ReturnData.Description = "Bạn không có quyền";
                    ReturnData.ResponseCode = -1;
                    return Json(ReturnData);
                }

            }

            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }
            var currentitem = ServerProcess.GetOrder(token, item.TransactionID);
            if (currentitem == null)
            {
                ReturnData.Description = "Có lỗi trong quá trình xử lý, vui lòng quay lại sau";
                ReturnData.ResponseCode = -99;
                return Json(ReturnData);
            }
            currentitem.Status = 1;
            var result = ServerProcess.OrderUpdate(currentitem, token);
            if (result >= 0)
            {

                AbstractDAOFactory.Instance().OrderReportsService().UpdateStatus(item.TransactionID, 1);
                ReturnData.Description = "Chuyển trạng thái Thành Công";
                ReturnData.ResponseCode = 1;

                return Json(ReturnData);
            }
            ReturnData.Description = "Chuyển trạng thái thất bại";
            ReturnData.ResponseCode = -99;
            return Json(ReturnData);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult LockTran(OrderOutput item)
        {
            var ReturnData = new ReturnData();
            if (CurrentFullUser.Type > 2)
            {
                if (!item.OrderNo.Contains("_" + CurrentFullUser.Config + "_") && CurrentFullUser.Type > 2)
                {
                    ReturnData.Description = "Bạn không có quyền";
                    ReturnData.ResponseCode = -1;
                    return Json(ReturnData);
                }

            }

            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }
            var currentitem = ServerProcess.GetOrder(token, item.TransactionID);
            if (currentitem == null)
            {
                ReturnData.Description = "Có lỗi trong quá trình xử lý, vui lòng quay lại sau";
                ReturnData.ResponseCode = -99;
                return Json(ReturnData);
            }
            if (currentitem.Status==2)
            {
                ReturnData.Description = "Đơn trong quá trình xử lý, vui lòng quay lại sau";
                ReturnData.ResponseCode = -99;
                return Json(ReturnData);
            }
            currentitem.Status = -3;
            var result = ServerProcess.OrderUpdate(currentitem, token);
            if (result >= 0)
            {

                AbstractDAOFactory.Instance().OrderReportsService().UpdateStatus(item.TransactionID, 0);
                ReturnData.Description = "Dừng nạp Thành Công";
                ReturnData.ResponseCode = 1;
                _userlogservice.InsertUsersLog(new UsersLog
                {
                    FunctionCode = "Orders",
                    Description = String.Format("Khóa  đơn {0}-{1}", item.OrderNo, item.TransactionID),
                    UserID = CurrentFullUser.UserID,
                    UserName = CurrentFullUser.Username,
                    ClientIP = Config.GetIP()
                });
                return Json(ReturnData);
            }
            ReturnData.Description = "Khóa thất bại";
            ReturnData.ResponseCode = -99;
            return Json(ReturnData);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult UpdateAmountTran(OrderOutput item)
        {
            var ReturnData = new ReturnData();
            //if (!ViewBag.IsAdmin)
            //{
            //    if (!item.OrderNo.Contains(CurrentFullUser.Config + "_"))
            //    {
            //        ReturnData.Description = "Bạn không có quyền";
            //        ReturnData.ResponseCode = -1;
            //        return Json(ReturnData);
            //    }
            //}

            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }
            var currentitem = ServerProcess.GetOrder(token, item.TransactionID);
            if (currentitem == null)
            {
                ReturnData.Description = "Có lỗi trong quá trình xử lý, vui lòng quay lại sau";
                ReturnData.ResponseCode = -99;
                return Json(ReturnData);
            }
            currentitem.AmountMinAll = item.AmountMinAll;
            currentitem.AmountMin = item.AmountMin;
            //item.Status = (int)Enums.Status.Lock;
            var result = ServerProcess.OrderUpdate(currentitem, token);
            if (result >= 0)
            {
                ReturnData.Description = "Cập nhật Thành Công";
                ReturnData.ResponseCode = 1;
                AbstractDAOFactory.Instance().OrderReportsService().UpdateAmount(item.TransactionID, item.AmountMinAll);
                return Json(ReturnData);
            }
            ReturnData.Description = "Cập nhật thất bại";
            ReturnData.ResponseCode = -99;
            return Json(ReturnData);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult Update(OrderOutput item)
        {
            var ReturnData = new ReturnData();


            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }

            var order = ServerProcess.GetOrder(token, item.TransactionID);
            if((order.SubUser==CurrentUser.Username|| CurrentUser.Type<=2) && order.Status!=2)
            {
                //order.Priority = item.Priority;
                //order.AmountMin = item.AmountMin;
                //order.AmountMinAll = item.AmountMinAll;
                order.Password = item.Password;
                // order.Ussd = item.Ussd;
                if (item.Status > 0)
                    order.Status = 1;
                
                var result = ServerProcess.OrderUpdateFull(order, token);
                if (result >= 0)
                {
                    ReturnData.Description = "Cập nhật Thành Công";
                    ReturnData.ResponseCode = 1;

                    return Json(ReturnData);
                }
            }    
           
            ReturnData.Description = "Cập nhật thất bại";
            ReturnData.ResponseCode = -99;
            return Json(ReturnData);
        }
        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //[PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        //public JsonResult UpdatePriority(string OrderNo, int Priority)
        //{
        //    var ReturnData = new ReturnData();

        //    if (OrderNo.Contains("Zing") || OrderNo.Contains("Garena"))
        //    {
        //        var token = Session[SessionsManager.SESSION_TOKEN].ToString();
        //        if (string.IsNullOrEmpty(token))
        //        {
        //            token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
        //            Session[SessionsManager.SESSION_TOKEN] = token;
        //        }

        //        //item.Status = (int)Enums.Status.Lock;
        //        var result = ServerProcess.UpdatePriority(OrderNo, Priority, token);
        //        if (result >= 0)
        //        {
        //            ReturnData.Description = "Cập nhật Thành Công";
        //            ReturnData.ResponseCode = 1;

        //            return Json(ReturnData);
        //        }
        //    }

        //    ReturnData.Description = "Cập nhật thất bại";
        //    ReturnData.ResponseCode = -99;
        //    return Json(ReturnData);
        //}

        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //[PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        //public JsonResult UpdatePriorityTran(OrderOutput item)
        //{
        //    var ReturnData = new ReturnData();


        //    var token = Session[SessionsManager.SESSION_TOKEN].ToString();
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
        //        Session[SessionsManager.SESSION_TOKEN] = token;
        //    }

        //    var order = ServerProcess.GetOrder(token, item.TransactionID);
        //    order.Priority = item.Priority;
        //    //item.Status = (int)Enums.Status.Lock;
        //    var result = ServerProcess.OrderUpdate(order, token);
        //    if (result >= 0)
        //    {
        //        ReturnData.Description = "Cập nhật Thành Công";
        //        ReturnData.ResponseCode = 1;

        //        return Json(ReturnData);
        //    }
        //    ReturnData.Description = "Cập nhật thất bại";
        //    ReturnData.ResponseCode = -99;
        //    return Json(ReturnData);
        //}
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult ConfirmTran(OrderOutput item)
        {
            var m_Users = _userservice.SelectByUserID(CurrentUser.UserID);
            Session[SessionsManager.SESSION_USER_FULL] = m_Users;

            var ReturnData = new ReturnData();
            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }
            var currentitem = ServerProcess.GetOrder(token, item.TransactionID);
            if (currentitem == null)
            {
                ReturnData.Description = "Có lỗi trong quá trình xử lý, vui lòng quay lại sau";
                ReturnData.ResponseCode = -99;
                return Json(ReturnData);
            }
            if (currentitem.IsConfirm==1)
            {
                ReturnData.Description = "Có lỗi trong quá trình xử lý, vui lòng quay lại sau";
                ReturnData.ResponseCode = -99;
                return Json(ReturnData);
            }
            if (currentitem.Status == 2)
            {
                ReturnData.Description = "Giao dịch trong quá trình xử lý, vui lòng quay lại sau";
                ReturnData.ResponseCode = -99;
                return Json(ReturnData);
            }
            if (!item.OrderNo.Contains("_" + CurrentFullUser.Config + "_") && CurrentFullUser.Type > 2)
            {
                ReturnData.Description = "Bạn không có quyền";
                ReturnData.ResponseCode = -1;
                return Json(ReturnData);
            }
            var _orderUser = _userservice.GetByUsername(currentitem.SubUser);

            //lấy current tran
            //var lstdata = ServerProcess.ListTransaction(token, 10000, "", item.OrderNo, null, 0);
            //var currentitem = lstdata.FirstOrDefault(x => x.TransactionID == item.TransactionID);

            currentitem.IsConfirm = 1;
            //if (currentitem.Status != (int)Enums.Status.Success)
            //{
            //    currentitem.Status = (int)Enums.Status.Success;
            //}

            var result = ServerProcess.OrderConfirm(currentitem, token);
            if (result >= 0)
            {
                AbstractDAOFactory.Instance().OrderReportsService().Confirm(item.TransactionID);
                //chốt đơn: tính lại tiền+ ghi log

                var pecent = 0;
                var pecentParrent = 0;

                //lấy chiết khấu trong log
                var orderReport = AbstractDAOFactory.Instance().OrderReportsService().Get(item.TransactionID);
                if (orderReport != null && orderReport.OrderId > 0)
                {
                    pecent = orderReport.Percent;
                    pecentParrent = orderReport.PercentParrent;

                    //hoàn lại tiền đua giá
                    if (orderReport.BidFeeHold > 0)
                    {
                        _transervice.TopupHold(_orderUser.Username, orderReport.BidFeeHold, String.Format("Hoàn tiền đua giá, đơn {0}", currentitem.OrderNo + "-" + currentitem.TransactionID), orderReport.OrderId.ToString());
                        //ghi log đua giá
                        var logBid = new BidHistory
                        {
                            OrderId = orderReport.OrderId,
                            OrderNo = orderReport.OrderNo,
                            UserID = _orderUser.UserID,
                            Description = String.Format("Hoàn tiền đua giá {0}", orderReport.BidFeeHold)
                        };
                        _bidHistoryservice.InsertBidHistory(logBid);
                    }
                }


                if (currentitem.AmountTopupSuccess > currentitem.Amount)
                    currentitem.AmountTopupSuccess = currentitem.Amount;
                var amountSuccess = currentitem.AmountTopupSuccess * pecent / 100;
                var amountTopupHold = currentitem.Amount * pecent / 100;
                //hoàn lại tiền
                if (currentitem.AmountTopupSuccess < currentitem.Amount)
                {
                    _transervice.TopupHold(_orderUser.Username, amountTopupHold - amountSuccess, String.Format("Hoàn tiền chốt đơn, đơn {0}", currentitem.OrderNo + "-" + currentitem.TransactionID), orderReport.OrderId.ToString());
                }
                //var amountParrent = currentitem.AmountTopupSuccess * (pecent - pecentParrent) / 100;

                ////tk cha là cấp 1 thì ko hoa hồng
                //if (parrentUser.Type <= 2)
                //{
                //    amountParrent = 0;
                //}
                //var resultConfirm = _transervice.Confrim(CurrentFullUser.Username, parrentUser.Username, amountParrent, amountSuccess, amountTopupHold, currentitem.OrderNo + "-" + currentitem.TransactionID);

                _userlogservice.InsertUsersLog(new UsersLog
                {
                    FunctionCode = "Orders",
                    Description = String.Format("Chốt đơn {0}-{1} Số tiền {2}/{3} Chiết khấu {4}", item.OrderNo, item.TransactionID, amountSuccess, amountTopupHold, 100 - pecent),
                    UserID = CurrentFullUser.UserID,
                    UserName = CurrentFullUser.Username,
                    ClientIP = Config.GetIP()
                });



                //hoàn lại hạn mức ngày
                //if (item.OrderNo.Contains(DateTime.Now.ToString("MMddyy")))
                //{
                //    if (amountSuccess < amountTopupHold)
                //    {
                //        _userservice.SetDay(_orderUser.UserID, _orderUser.CreatedUser, amountSuccess - amountTopupHold);
                //    }
                //}
                ReturnData.Description = "Chốt Thành Công";
                ReturnData.ResponseCode = 1;
                return Json(ReturnData);
            }
            ReturnData.Description = "Chốt thất bại do có giao dịch nghi vấn. Vui lòng liên hệ với quản trị !";
            ReturnData.ResponseCode = -99;

            return Json(ReturnData);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult ResetOrderAdmin(string order)
        {
            var ReturnData = new ReturnData();
           

            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }

            var data = ServerProcess.ListTransaction(token, 10000, "", order, null, 0);
            foreach (var item in data)
            {
                if (item.Status == -2)
                {
                    item.Status = 1;
                    ServerProcess.OrderUpdate(item, token);

                    AbstractDAOFactory.Instance().OrderReportsService().UpdateStatus(item.TransactionID, 1);
                }

            }
            _userlogservice.InsertUsersLog(new UsersLog
            {
                FunctionCode = "Orders",
                Description = String.Format("Mở Khóa đơn {0}", order),
                UserID = CurrentFullUser.UserID,
                UserName = CurrentFullUser.Username,
                ClientIP = Config.GetIP()
            });
            ReturnData.Description = "Mở Khóa Thành Công";
            ReturnData.ResponseCode = 1;
            return Json(ReturnData);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult ResetOrder(string order)
        {
            var ReturnData = new ReturnData();
            if (CurrentFullUser.Type > 2)
            {
                if (!order.Contains("_" + CurrentFullUser.Config + "_") && CurrentFullUser.Type > 2)
                {
                    ReturnData.Description = "Bạn không có quyền";
                    ReturnData.ResponseCode = -1;
                    return Json(ReturnData);
                }

            }

            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }

            var data = ServerProcess.ListTransaction(token, 10000, "", order, null, 0);
            foreach (var item in data)
            {
                if (item.Status == -3)
                {
                    item.Status = 1;
                    ServerProcess.OrderUpdate(item, token);

                    AbstractDAOFactory.Instance().OrderReportsService().UpdateStatus(item.TransactionID, 1);
                }

            }
            _userlogservice.InsertUsersLog(new UsersLog
            {
                FunctionCode = "Orders",
                Description = String.Format("Mở Khóa đơn {0}", order),
                UserID = CurrentFullUser.UserID,
                UserName = CurrentFullUser.Username,
                ClientIP = Config.GetIP()
            });
            ReturnData.Description = "Mở Khóa Thành Công";
            ReturnData.ResponseCode = 1;
            return Json(ReturnData);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult LockOrder(string order)
        {
            var ReturnData = new ReturnData();
            if (CurrentFullUser.Type > 2)
            {
                if (!order.Contains("_" + CurrentFullUser.Config + "_") && CurrentFullUser.Type > 2)
                {
                    ReturnData.Description = "Bạn không có quyền";
                    ReturnData.ResponseCode = -1;
                    return Json(ReturnData);
                }

            }

            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }

            //var data = ServerProcess.ListTransaction(token, 1000, "", order, null, null);
            //foreach (var item in data)
            //{
            //    if (item.Status != (int)Enums.Status.Lock)
            //    {
            //        item.Status = (int)Enums.Status.Lock;
            //        ServerProcess.OrderUpdate(item, token);
            //    }

            //}
            ServerProcess.OrderLock(order, token);
            AbstractDAOFactory.Instance().OrderReportsService().UpdateStatusOrder(order, 0);
            _userlogservice.InsertUsersLog(new UsersLog
            {
                FunctionCode = "Orders",
                Description = String.Format("Khóa đơn {0}", order),
                UserID = CurrentFullUser.UserID,
                UserName = CurrentFullUser.Username,
                ClientIP = Config.GetIP()
            });
            ReturnData.Description = "Khóa Thành Công";
            ReturnData.ResponseCode = 1;
            return Json(ReturnData);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult ConfirmOrder(string order)
        {
            var m_Users = _userservice.SelectByUserID(CurrentUser.UserID);
            Session[SessionsManager.SESSION_USER_FULL] = m_Users;
            var ReturnData = new ReturnData();

            if (!order.Contains("_" + CurrentFullUser.Config + "_") && CurrentFullUser.Type > 2)
            {
                ReturnData.Description = "Bạn không có quyền";
                ReturnData.ResponseCode = -1;
                return Json(ReturnData);
            }

            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }

            var data = ServerProcess.ListTransaction(token, 10000, "", order, null, 0);
            if (data.Count() > 0)
                data = data.Where(x => x.OrderNo.Equals(order) && x.Status != 2).ToList();
            var _orderUser = _userservice.GetByUsername(data.FirstOrDefault().SubUser);
            var lstTran = "";
            var lstPecent = "";
            var totalAmount = 0;
            var amountSuccess = 0;
            var TotalamountSuccess = 0;
            var TotalamountTopupHold = 0;
            var amountTopupHold = 0;
            var totalTran = 0;
            var pecent = 0;
            var pecentParrent = 0;

            //confirm Order
            var resultConfirm = ServerProcess.OrderConfirm(order, token);
            if (resultConfirm < 0)
            {
                ReturnData.Description = "Chốt thất bại do có giao dịch nghi vấn. Vui lòng liên hệ với quản trị !";
                ReturnData.ResponseCode = -1;
                return Json(ReturnData);
            }
            //var parrentUser = _userservice.GetByUsername(CurrentFullUser.CreatedUser);
            foreach (var item in data)
            {

                if (item.IsConfirm != 1)
                {
                    //item.IsConfirm = 1;
                    //if (item.Status != (int)Enums.Status.Success)
                    //{
                    //    item.Status = (int)Enums.Status.Lock;
                    //}
                    //var resultConfirm = ServerProcess.OrderConfirm(item, token);
                    //if (resultConfirm >= 0)
                    //{
                    if (item.AmountTopupSuccess > item.Amount)
                        item.AmountTopupSuccess = item.Amount;

                    totalAmount += item.Amount;
                    //totalAmountSucccess += item.AmountTopupSuccess;
                    totalTran++;
                    lstTran = lstTran + item.TransactionID + ", ";


                    //lấy chiết khấu trong log
                    var orderReport = AbstractDAOFactory.Instance().OrderReportsService().Get(item.TransactionID);
                    if (orderReport != null && orderReport.OrderId > 0)
                    {
                        pecent = orderReport.Percent;
                        pecentParrent = orderReport.PercentParrent;

                        //hoàn lại tiền đua giá
                        if (orderReport.BidFeeHold > 0)
                        {
                            _transervice.TopupHold(_orderUser.Username, orderReport.BidFeeHold, String.Format("Hoàn tiền đua giá, đơn {0}", item.OrderNo + "-" + item.TransactionID), orderReport.OrderId.ToString());
                            //ghi log đua giá
                            var logBid = new BidHistory
                            {
                                OrderId = orderReport.OrderId,
                                OrderNo = orderReport.OrderNo,
                                UserID = _orderUser.UserID,

                                Description = String.Format("Hoàn tiền đua giá {0}", orderReport.BidFeeHold)
                            };
                            _bidHistoryservice.InsertBidHistory(logBid);
                        }

                    }
                    AbstractDAOFactory.Instance().OrderReportsService().Confirm(item.TransactionID);
                    lstPecent = lstPecent + pecent + ", ";
                    amountSuccess = item.AmountTopupSuccess / 100 * pecent;
                    amountTopupHold = item.Amount / 100 * pecent;

                    TotalamountSuccess += amountSuccess;
                    TotalamountTopupHold += amountTopupHold;
                    //amountParrent += item.AmountTopupSuccess / 100 * (pecent - pecentParrent);
                    //hoàn lại tiền đơn
                    if (item.AmountTopupSuccess < item.Amount)
                    {
                        _transervice.TopupHold(_orderUser.Username, amountTopupHold - amountSuccess, String.Format("Hoàn tiền chốt đơn, đơn {0}", item.OrderNo + "-" + item.TransactionID), orderReport.OrderId.ToString());
                    }
                }

                //}

            }
            if (totalAmount > 0)
            {

                _userlogservice.InsertUsersLog(new UsersLog
                {
                    FunctionCode = "Orders",
                    Description = String.Format("Chốt đơn {0} Số tiền {1}/{2} Số giao dịch:{3}, Giao dịch {4} Chiết khấu {5}", order, TotalamountSuccess, TotalamountTopupHold, totalTran, lstTran, lstPecent),
                    UserID = CurrentFullUser.UserID,
                    UserName = CurrentFullUser.Username,
                    ClientIP = Config.GetIP()
                });

                //hoàn lại hạn mức ngày
                //if (order.Contains(DateTime.Now.ToString("MMddyy")))
                //{
                //    if (amountSuccess < amountTopupHold)
                //    {
                //        _userservice.SetDay(_orderUser.UserID, CurrentFullUser.CreatedUser, amountSuccess - amountTopupHold);
                //    }
                //}

            }

            ReturnData.Description = "Cập nhật Thành Công";
            ReturnData.ResponseCode = 1;
            return Json(ReturnData);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult ListOrderActive(int userId, int top, int ussd, string keyword, string telco)
        {
            var data = new List<OrderGroup>();
            var orderNo = "";
            var token = "";
            var user = new Users();
            var subUser = "";
            if (ViewBag.IsAdmin)
            {
                if (userId != -1)
                {
                    user = _userservice.SelectByUserID(userId);
                    token = ServerProcess.GetUserTokenCache(user.UserAPI, user.PasswordAPI);
                    Session[SessionsManager.SESSION_TOKEN] = token;

                    orderNo = String.Format("_{0}_", user.Config);
                    if (user.Type == 2)
                    {
                        orderNo = "";
                    }
                   
                       
                }
                else
                {
                    token = Session[SessionsManager.SESSION_TOKEN].ToString();
                    if (string.IsNullOrEmpty(token))
                    {
                        token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                        Session[SessionsManager.SESSION_TOKEN] = token;
                    }
                }

            }
            else
            {
                token = Session[SessionsManager.SESSION_TOKEN].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                    Session[SessionsManager.SESSION_TOKEN] = token;
                }

                //cấp super admin
                if (CurrentUser.Type == 2)
                {
                    if (userId == -1)
                    {
                        //lấy tất con của nó
                        orderNo = "";
                    }
                    else
                    {
                        user = _userservice.SelectByUserID(userId);
                        subUser = "";
                        orderNo = String.Format("_{0}_", user.Config);
                        //orderNo = String.Format("{0}_", user.Config);
                    }
                }
                //cấp 1
                else
                {
                    if (CurrentUser.Type <= 4)
                    {
                        if (userId == -1)
                        {
                            //lấy tất con của nó
                            orderNo = String.Format("_{0}_", CurrentFullUser.Config);
                        }
                        else
                        {
                            user = _userservice.SelectByUserID(userId);
                            subUser = user.Username;
                            orderNo = String.Format("_{0}_", user.Config);
                            //orderNo = String.Format("{0}_", user.Config);
                        }
                    }
                    //cấp 3 type=5
                    else
                    {
                        user = CurrentFullUser;
                        subUser = user.Username;
                        orderNo = "";
                        //orderNo = String.Format("{0}_", CurrentFullUser.Config);

                    }

                }
            }
            data = ServerProcess.ListGroupActive(token, top,ussd, telco, orderNo, subUser);
            //foreach(var itemorder in data)
            //{
            //    var lstData = ServerProcess.ListTransaction(token, 1000, "", itemorder.OrderNo, null, 0);
            //    foreach(var item in lstData)
            //    {
            //        AbstractDAOFactory.Instance().OrderReportsService().UnConfirm(item.TransactionID);
            //    }

            //}
            //if (userId > 0)
            //{
            //    //tk cấp 1 chỉ nhìn đc nó
            //    if (user.Type <= 3)
            //    {
            //        data = data.Where(x => x.OrderNo.ToCharArray().Count(c => c == '_') == 2).ToList();

            //    }
            //}
            //if (!string.IsNullOrEmpty(keyword))
            //{
            //    if (data != null)
            //        data = data.Where(x => x.OrderNo.Contains(keyword)).ToList();
            //}
            ViewBag.UserType = CurrentUser.Type;
            ViewBag.Username = CurrentUser.Username;
            ViewBag.Config = String.Format("_{0}_", CurrentFullUser.Config);
            //if(CurrentUser.Type==2)
            //{
            //    ViewBag.Config = "";
            //}
            ViewBag.IsBid = ConfigurationManager.AppSettings["UsersBid"].ToString().Contains("," + CurrentFullUser.Username + ",") || ConfigurationManager.AppSettings["UsersAPIBid"].ToString().Contains("," + CurrentFullUser.UserAPI + ",");
            if (data == null)
                return PartialView(new List<OrderInput>());
            return PartialView(data.OrderByDescending(x => x.LastTime).ToList());
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrdeConfirm)]
        public ActionResult OrderConfirm()
        {
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().OrderBy(x => x.Type).ToList();
                lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
            }
            else
            {
                if (CurrentFullUser.Type == 2)
                {
                    lstUser = _userservice.GetByEmail(CurrentFullUser.UserAPI);
                    lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                }
                else
                {
                    if (CurrentUser.Type == 3)
                    {
                        lstUser = _userservice.GetAll().Where(x => x.C1User == CurrentUser.Username).ToList();

                        lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                        lstUser.Insert(1, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                    }
                    else
                    {
                        if (CurrentUser.Type == 4)
                        {
                            lstUser = _userservice.GetAll().Where(x => x.CreatedUser == CurrentUser.Username).ToList();

                            lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                            lstUser.Insert(1, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                        }
                        else
                        {
                            lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                        }

                    }
                }
            }

            ViewBag.UserList = lstUser;
            ViewBag.Title = "Danh sách đơn đã chốt";
            return View();

        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult ListOrderConfirm(int userId, int top, string keyword, string telco)
        {
            var data = new List<OrderGroup>();
            var orderNo = "";
            var token = "";
            var user = new Users();
            var subUser = "";
            if (ViewBag.IsAdmin)
            {
                if (userId != -1)
                {
                    user = _userservice.SelectByUserID(userId);
                    token = ServerProcess.GetUserTokenCache(user.UserAPI, user.PasswordAPI);
                    Session[SessionsManager.SESSION_TOKEN] = token;

                    orderNo = String.Format("_{0}_", user.Config);
                    if (user.Type == 2)
                    {
                        orderNo = "";
                    }


                }
                else
                {
                    token = Session[SessionsManager.SESSION_TOKEN].ToString();
                    if (string.IsNullOrEmpty(token))
                    {
                        token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                        Session[SessionsManager.SESSION_TOKEN] = token;
                    }
                }

            }
            else
            {
                token = Session[SessionsManager.SESSION_TOKEN].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                    Session[SessionsManager.SESSION_TOKEN] = token;
                }

                //cấp super admin
                if (CurrentUser.Type == 2)
                {
                    if (userId == -1)
                    {
                        //lấy tất con của nó
                        orderNo = "";
                    }
                    else
                    {
                        user = _userservice.SelectByUserID(userId);
                        subUser = "";
                        orderNo = String.Format("_{0}_", user.Config);
                        //orderNo = String.Format("{0}_", user.Config);
                    }
                }
                //cấp 1
                else
                {
                    if (CurrentUser.Type <= 4)
                    {
                        if (userId == -1)
                        {
                            //lấy tất con của nó
                            orderNo = String.Format("_{0}_", CurrentFullUser.Config);
                        }
                        else
                        {
                            user = _userservice.SelectByUserID(userId);
                            subUser = user.Username;
                            orderNo = String.Format("_{0}_", user.Config);
                            //orderNo = String.Format("{0}_", user.Config);
                        }
                    }
                    //cấp 3 type=5
                    else
                    {
                        user = CurrentFullUser;
                        subUser = user.Username;
                        orderNo = "";
                        //orderNo = String.Format("{0}_", CurrentFullUser.Config);

                    }

                }
            }
            data = ServerProcess.ListGroupFinish(token, top, telco, orderNo, subUser);
            //if (userId > 0)
            //{
            //    //tk cấp 1 chỉ nhìn đc nó
            //    if (user.Type <= 3)
            //    {
            //        data = data.Where(x => x.OrderNo.ToCharArray().Count(c => c == '_') == 2).ToList();

            //    }
            //}
            //if (!string.IsNullOrEmpty(keyword))
            //{
            //    if (data != null)
            //        data = data.Where(x => x.OrderNo.Contains(keyword)).ToList();
            //}
            //NLogLogger.Info(user.Type.ToString());
            //NLogLogger.Info(data.Count.ToString());
            return PartialView(data);
        }
    }
}