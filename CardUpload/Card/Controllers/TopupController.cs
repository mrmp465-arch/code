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
using OfficeOpenXml.Style;
using System.Drawing;
namespace Card.CMS.Controllers
{
    public class TopupController : Controller
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

        private readonly ITopupOrderService _topupoderservice;

        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } set { } }
        public TopupController(ITopupOrderService topupoderservice, IOrderTempsService oservice, IAccountTokenService accounttokenservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _accounttokenservice = accounttokenservice;
            _oservice = oservice;

            _topupoderservice = topupoderservice;
        }
        [PermissionFilter(FunctionCode = FunctionCode.TopupList)]
        public ActionResult Index()
        {
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().Where(x => x.Type == 2).ToList();
                lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
            }
            else
            {
                if (CurrentFullUser.Type == 2)
                {
                    lstUser = _userservice.GetAll().Where(x => x.CreatedUser == CurrentUser.Username).ToList();

                    lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                    lstUser.Insert(1, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                }
                else
                {
                    if (CurrentUser.Type == 3)
                    {

                        lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                        //lstUser.Insert(1, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                    }

                }
            }
            ViewBag.Title = "Danh sách đơn topup";
            ViewBag.UserList = lstUser;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.TopupList)]
        public ActionResult ListOrder(string username, int top, string orderNo)
        {
            var data = new List<TopupOrderGroup>();
            if (username == "--Tất cả--")
                username = "";
            if (CurrentFullUser.Type == 1)
            {
                data = _topupoderservice.GetListGroup(top, username, "", orderNo);
            }
            if (CurrentFullUser.Type == 2)
            {
                data = _topupoderservice.GetListGroup(top, CurrentFullUser.Username, username, orderNo);
            }
            if (CurrentFullUser.Type == 3)
            {
                data = _topupoderservice.GetListGroup(top, "", CurrentFullUser.Username, orderNo);
            }
            ViewBag.Type = CurrentFullUser.Type;
            ViewBag.Username = CurrentFullUser.Username;
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.TopupList)]
        public ActionResult Detail(string OrderNo)
        {
            var obj = _topupoderservice.GetByOrderNo(OrderNo);
            obj = obj.OrderBy(a => a.Id).ToList();
            ViewBag.OrderNo = OrderNo;
            if (CurrentFullUser.Type == 2)
            {
                if (obj[0].ParrentName != CurrentFullUser.Username)
                    return RedirectToAction("Index", "Home");
            }
            if (CurrentFullUser.Type == 3)
            {
                if (obj[0].UserName != CurrentFullUser.Username)
                    return RedirectToAction("Index", "Home");

            }
            ViewBag.Confirm = obj.Count(x => x.IsConfirm == 0);
            ViewBag.Title = "Chi tiết đơn topup";

            return View(obj);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.TopupList, FunctionType = (int)Enums.FunctionType.IsFullControl)]
        public JsonResult ConfirmOrder(string order)
        {
            var ReturnData = new ReturnData();
            var lstdata = _topupoderservice.GetByOrderNo(order);
            var where = " [OrderNo]  ='" + order + "' "; ;
            var update = "IsConfirm=1";
            update += ",UpdateDate= getdate()";

            _topupoderservice.UpdateDynamic(where, update);

            _userlogservice.InsertUsersLog(new UsersLog
            {
                FunctionCode = "Orders",
                Description = String.Format("Chốt đơn topup mã đơn {0}", order),
                UserID = CurrentFullUser.UserID,
                UserName = CurrentFullUser.Username,
                LogType = 1,
                ClientIP = Config.GetIP()
            });

            var TotaMoney = lstdata.Sum(x => x.Money);
            var TotaMoneyReward = lstdata.Sum(x => x.MoneyReward);
            var TotaMoneyPriority = lstdata.Sum(x => x.MoneyPriority);
            var TotalTrutam = (int)lstdata.Sum(x => x.Amount / 100 * (100 - x.Fee + x.Priority));

            //cộng tiền cho thằng tạo đơn

            var user = _userservice.GetByUsername(lstdata[0].UserName);
            //hoàn tiền đã trừ tạm
            _transervice.TopupHold(user.Username, TotalTrutam, String.Format("Hoàn tiền trừ tạm đơn {0}", order));
            if (TotaMoneyPriority + TotaMoney > 0)
                _transervice.Deduct(user.Username, TotaMoneyPriority + TotaMoney, String.Format("Trừ tiền thực tế đơn {0}", order),"");

            //trừ tiền thực tế
            //_userservice.Topup(user.UserID, CurrentUser.Username, TotaMoneyReward, $"Cộng tiền hoa hồng cho đơn topup {order}");

            //cộng tiền cho thằng đại lý
            if (TotaMoneyReward > 0)
            {
                var puser = _userservice.GetByUsername(lstdata[0].ParrentName);
                _userservice.Topup(puser.UserID, CurrentUser.Username, TotaMoneyReward, $"Cộng tiền hoa hồng cho đơn topup {order}");
            }
            ReturnData.Description = "1";
            ReturnData.ResponseCode = 1;
            return Json(ReturnData);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.TopupList)]
        public JsonResult ResetOrder(string order)
        {
            var ReturnData = new ReturnData();
            //cehck chính chủ


            //update status=-2
            var lstdata = _topupoderservice.GetByOrderNo(order);
            lstdata = lstdata.Where(x => x.Status == 0 && x.IsConfirm == 0).ToList();
            if (!order.Contains("_" + CurrentFullUser.Config + "_") && CurrentFullUser.Type >= 2)
            {
                ReturnData.Description = "Bạn không có quyền";
                ReturnData.ResponseCode = -1;
                return Json(ReturnData);
            }
            var where = " [OrderNo]  ='" + order + "' And IsConfirm=0 And Status=0";
            var update = "Status=-2,IsConfirm=1";
            update += ",UpdateDate= getdate()";

            _topupoderservice.UpdateDynamic(where, update);
            var money = (int)lstdata.Sum(x => x.Amount / 100 * (100 - x.Fee + x.Priority));
            //hoàn tiền
            if (money > 0)
                _transervice.TopupHold(CurrentFullUser.Username, money, String.Format("Hoàn tiền hủy đơn mã {0}", order));
            _userlogservice.InsertUsersLog(new UsersLog
            {
                FunctionCode = "Orders",
                Description = String.Format("Hủy đơn {0}", order),
                UserID = CurrentFullUser.UserID,
                UserName = CurrentFullUser.Username,
                ClientIP = Config.GetIP()
            });
            ReturnData.Description = "Thao tác Thành Công";
            ReturnData.ResponseCode = 1;
            return Json(ReturnData);
        }
        //xử lý đơn
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.TopupList, FunctionType = (int)Enums.FunctionType.IsFullControl)]
        public JsonResult LockOrder(string order)
        {
            var ReturnData = new ReturnData();


            //var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            //if (string.IsNullOrEmpty(token))
            //{
            //    token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
            //    Session[SessionsManager.SESSION_TOKEN] = token;
            //}
            //var lstdata = _topupoderservice.GetByOrderNo(order);
            var where = " [OrderNo]  ='" + order + "' And IsConfirm=0";
            var update = "Status=3";
            update += ",UpdateDate= getdate()";

            _topupoderservice.UpdateDynamic(where, update);
            //update status=-3
            _userlogservice.InsertUsersLog(new UsersLog
            {
                FunctionCode = "Orders",
                Description = String.Format("Xử lý  đơn {0}", order),
                UserID = CurrentFullUser.UserID,
                UserName = CurrentFullUser.Username,
                ClientIP = Config.GetIP()
            });
            ReturnData.Description = "Thao tác Thành Công";
            ReturnData.ResponseCode = 1;
            return Json(ReturnData);
        }
        [PermissionFilter(FunctionCode = FunctionCode.TopupList, FunctionType = (int)Enums.FunctionType.IsFullControl)]

        public ActionResult TopupOrder(List<TopupOrder> lstorder)
        {
            var ReturnData = new ReturnData();
            try
            {
                if (CurrentFullUser == null)
                {

                    ReturnData.ResponseCode = -101;
                    ReturnData.Description = "Bạn không có quyền sử dụng chức năng này";
                    return Json(ReturnData);

                }
                var OrderNo = lstorder[0].OrderNo;
                var lstdata = _topupoderservice.GetByOrderNo(OrderNo);
                lstdata = lstdata.Where(x => x.IsConfirm == 0).ToList();
                foreach (var order in lstorder)
                {

                    if (lstdata.Exists(x => x.Id == order.Id))
                    {
                        var orderObj = lstdata.FirstOrDefault(x => x.Id == order.Id);
                        orderObj.Status = order.Status;
                        orderObj.AmountSuccess = order.AmountSuccess;
                        if (order.Status == -1)
                            order.AmountSuccess = 0;
                        orderObj.Money = (int) (orderObj.AmountSuccess / 100 * (100 - orderObj.Fee));
                        orderObj.MoneyReward = (int) (orderObj.AmountSuccess / 100 * (orderObj.Reward));
                        orderObj.MoneyPriority = orderObj.AmountSuccess / 100 * (orderObj.Priority);
                        //_topupoderservice.Add(orderObj);
                        var where = " [Id]  =" + order.Id;
                        var update = "Status=" + orderObj.Status;
                        update += ",AmountSuccess=" + orderObj.AmountSuccess;
                        update += ",Money=" + orderObj.Money;
                        update += ",MoneyReward=" + orderObj.MoneyReward;
                        update += ",MoneyPriority=" + orderObj.MoneyPriority;
                        update += ",UpdateDate= getdate()";
                        _topupoderservice.UpdateDynamic(where, update);

                    }


                }
                _userlogservice.InsertUsersLog(new UsersLog
                {
                    FunctionCode = "Orders",
                    Description = String.Format("Cập nhật đơn topup mã đơn {0}", OrderNo),
                    UserID = CurrentFullUser.UserID,
                    UserName = CurrentFullUser.Username,
                    LogType = 1,
                    ClientIP = Config.GetIP()
                });
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                ReturnData.ResponseCode = -99;
                ReturnData.Description = "Có lỗi trong quá trình xử lý";
            }
            return Json(ReturnData);

        }
        [PermissionFilter(FunctionCode = FunctionCode.TopupList, FunctionType = (int)Enums.FunctionType.IsFullControl)]
        public ActionResult Download(string OrderNo)
        {
            var
            data = _topupoderservice.GetByOrderNo(OrderNo);
            data = data.OrderBy(a => a.Id).Where(x => x.Status != -2).ToList();
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Data");
                        worksheet.Column(1).Width = 10;
                        worksheet.Column(2).Width = 20;
                        worksheet.Column(3).Width = 15;
                        worksheet.Column(4).Width = 10;
                        worksheet.Column(5).Width = 30;
                        worksheet.Column(6).Width = 20;
                        worksheet.Column(7).Width = 20;
                        worksheet.Column(8).Width = 20;
                        worksheet.Column(9).Width = 20;
                        //worksheet.Column(1).Style.WrapText = true;
                        //worksheet.Column(2).Style.WrapText = true;
                        //worksheet.Column(3).Style.WrapText = true;
                        //worksheet.Column(4).Style.WrapText = true;
                        //worksheet.Column(5).Style.WrapText = true;
                        var properties = new[]
                        {

                            "TransId",

                             "Nhà mạng",
                              "SDT",
                            "Số tiền",
                           "Mã loại đơn",
                            "Mênh giá nạp",
                             "Mức ưu tiên",
                              "Mật khẩu",
                             "Số tiền thành công",
                            "Kết quả",



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


                            worksheet.Cells[row, col].Value = item.Id;
                            col++;



                            worksheet.Cells[row, col].Value = item.Telco.ToUpper();
                            col++;

                            worksheet.Cells[row, col].Value = item.Account;
                            col++;
                            worksheet.Cells[row, col].Value = item.Amount;
                            col++;
                            worksheet.Cells[row, col].Value = item.TopupType;
                            col++;
                            worksheet.Cells[row, col].Value = item.CardValue;
                            col++;

                            worksheet.Cells[row, col].Value = item.Priority;
                            col++;

                            worksheet.Cells[row, col].Value = item.Password;
                            col++;

                            worksheet.Cells[row, col].Value = item.AmountSuccess;
                            col++;
                            row++;
                        }
                        worksheet.Row(row).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        worksheet.Row(row).Style.Font.Size = 14;
                        worksheet.Row(row).Style.Font.Name = "Calibri";
                        worksheet.Row(row).Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));





                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", OrderNo + ".xlsx");
            }
            catch (Exception)
            {
                return RedirectToAction("ListOrder");
            }
        }

        [PermissionFilter(FunctionCode = FunctionCode.TopupReport)]
        public ActionResult Report()
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
                if (CurrentFullUser.Type == 2)
                {
                    lstUser = _userservice.GetAll().Where(x => x.CreatedUser == CurrentUser.Username).ToList();
                    lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                    lstUser.Insert(1, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                }
                else
                {
                    if (CurrentUser.Type == 3)
                    {


                        lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                    }

                }

            }
            ViewBag.UserList = lstUser;
            ViewBag.Title = "Báo cáo doanh thu ngày";
            return View();


        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderReport)]
        public ActionResult ListReport(string username, string FromDate, string ToDate, string OrderNo)
        {
            var data = new List<TopupOrderReport>();
            if (username == "--Tất cả--")
                username = "";
            if (CurrentFullUser.Type == 1)
            {
                data = _topupoderservice.GetReportDaily(username, "", OrderNo, FromDate, ToDate);
            }
            if (CurrentFullUser.Type == 2)
            {
                data = _topupoderservice.GetReportDaily(CurrentFullUser.Username, username, OrderNo, FromDate, ToDate);
            }
            if (CurrentFullUser.Type == 3)
            {
                data = _topupoderservice.GetReportDaily("", CurrentFullUser.Username, OrderNo, FromDate, ToDate);
            }
            ViewBag.Type = CurrentFullUser.Type;
            return PartialView(data);
        }
    }
}