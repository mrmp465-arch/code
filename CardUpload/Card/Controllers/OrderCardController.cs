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
    public class OrderCardController : Controller
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
        private readonly ICardOrderService _cardorderhervice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } set { } }
        public OrderCardController(ICardOrderService cardorderhervice, IBankGateService bankgateservice, IOrderTempsService oservice, IAccountTokenService accounttokenservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _accounttokenservice = accounttokenservice;
            _oservice = oservice;
            _cardorderhervice = cardorderhervice;
            _bankgateservice = bankgateservice;
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderList)]
        public ActionResult Search()
        {
            ViewBag.Title = "Tra cứu thẻ";
            return View();
        }
            [PermissionFilter(FunctionCode = FunctionCode.OrderList)]
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
            ViewBag.Title = "Danh sách mã kho thẻ";
            ViewBag.UserList = lstUser;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderList)]
        public ActionResult ListCard(string OrderNo)
        {
            var obj = _cardorderhervice.GetByOrderNo(OrderNo);
            obj = obj.OrderBy(a => a.Id).ToList();
            return PartialView(obj);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderList)]
        public ActionResult ListCardOrder( string orderNo,string telco,string seri,int amount)
        {
            var obj = _cardorderhervice.Search(orderNo, seri,telco, amount);
            //obj = obj.OrderBy(a => a.Id).ToList();
            return PartialView(obj);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderList)]
        public ActionResult ListOrder(string telco, int top, string orderNo)
        {
            var data = new List<CardOrderGroup>();
            //if (username == "--Tất cả--")
            //    username = "";
            if (CurrentFullUser.Type == 1)
            {
                data = _cardorderhervice.GetListGroup(top, "", "", orderNo, telco);
            }
            //if (CurrentFullUser.Type == 2)
            //{
            //    data = _cardorderhervice.GetListGroup(top, CurrentFullUser.Username, username, orderNo);
            //}
            //if (CurrentFullUser.Type == 3)
            //{
            //    data = _cardorderhervice.GetListGroup(top, "", CurrentFullUser.Username, orderNo);
            //}
            ViewBag.Type = CurrentFullUser.Type;
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderList)]
        public ActionResult Detail(string OrderNo)
        {
            var obj = _cardorderhervice.GetByOrderNo(OrderNo);
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
            ViewBag.Title = "Chi tiết đơn thẻ";

            return View(obj);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderList, FunctionType = (int)Enums.FunctionType.IsFullControl)]
        public JsonResult ConfirmOrder(string order)
        {
            var ReturnData = new ReturnData();
            var lstdata = _cardorderhervice.GetByOrderNo(order);
            var where = " [OrderNo]  ='" + order + "' "; ;
            var update = "IsConfirm=1" ;
            update += ",UpdateDate= getdate()";
            
            _cardorderhervice.UpdateDynamic(where, update);

            _userlogservice.InsertUsersLog(new UsersLog
            {
                FunctionCode = "Orders",
                Description = String.Format("Chốt đơn thẻ mã đơn {0}", order),
                UserID = CurrentFullUser.UserID,
                UserName = CurrentFullUser.Username,
                LogType = 1,
                ClientIP = Config.GetIP()
            });

            var TotaMoney = lstdata.Sum(x => x.Money);
            var TotaMoneyReward = lstdata.Sum(x => x.MoneyReward);
            //cộng tiền cho thằng tạo đơn
            if (TotaMoney > 0) {
                var user = _userservice.GetByUsername(lstdata[0].UserName);
                _userservice.Topup(user.UserID, CurrentUser.Username, TotaMoney, $"Cộng tiền cho đơn thẻ {order}" );
            }
            //cộng tiền cho thằng tạo đơn
            if (TotaMoneyReward > 0)
            {
                var user = _userservice.GetByUsername(lstdata[0].ParrentName);
                _userservice.Topup(user.UserID, CurrentUser.Username, TotaMoneyReward, $"Cộng tiền hoa hồng cho đơn thẻ {order}" );
            }
            ReturnData.Description = "1";
            ReturnData.ResponseCode = 1;
            return Json(ReturnData);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderList, FunctionType = (int)Enums.FunctionType.IsFullControl)]

        public ActionResult CardOrder(List<CardOrder> lstorder)
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
                var lstdata = _cardorderhervice.GetByOrderNo(OrderNo);
                lstdata = lstdata.Where(x => x.IsConfirm == 0).ToList();
                foreach (var order in lstorder)
                {

                    if (lstdata.Exists(x => x.CardCode == order.CardCode))
                    {
                        var orderObj = lstdata.FirstOrDefault(x => x.CardCode == order.CardCode);
                        orderObj.Status = order.Status;
                        orderObj.AmountSuccess = order.AmountSuccess;
                        if (orderObj.AmountSuccess != orderObj.Amount && orderObj.AmountSuccess > 0)
                            orderObj.Status = 2;
                        if (order.Status == -1)
                            order.AmountSuccess = 0;
                        orderObj.Money = (int) (orderObj.AmountSuccess / 100 * (100 - orderObj.Fee));
                        orderObj.MoneyReward = (int) (orderObj.AmountSuccess / 100 * (orderObj.Reward));
                        //_cardorderhervice.Add(orderObj);
                        var where = " [CardCode]  ='" + order.CardCode + "' "; ;
                        var update = "Status=" + orderObj.Status;
                        update += ",AmountSuccess=" + orderObj.AmountSuccess;
                        update += ",Money=" + orderObj.Money;
                        update += ",MoneyReward=" + orderObj.MoneyReward;
                        update += ",UpdateDate= getdate()";
                        _cardorderhervice.UpdateDynamic(where, update);

                    }


                }
                _userlogservice.InsertUsersLog(new UsersLog
                {
                    FunctionCode = "Orders",
                    Description = String.Format("Cập nhật đơn thẻ mã đơn {0}", OrderNo),
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
        [PermissionFilter(FunctionCode = FunctionCode.OrderList, FunctionType = (int)Enums.FunctionType.IsFullControl)]
        public ActionResult Download(string OrderNo)
        {
            var
            data = _cardorderhervice.GetByOrderNo(OrderNo);
            data = data.OrderBy(a => a.Id).ToList() ;
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

                            "Mã thẻ",
                            "Số seri",
                             "Nhà mạng",
                            "Mệnh giá",
                            //"Mệnh giá thực",
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


                            worksheet.Cells[row, col].Value = item.CardCode;
                            col++;



                            worksheet.Cells[row, col].Value = item.CardSerial.ToUpper();
                            col++;


                            worksheet.Cells[row, col].Value = item.Telco;
                            col++;

                            worksheet.Cells[row, col].Value = item.Amount;
                            col++;

                            if(item.Status==0)
                            {
                                worksheet.Cells[row, col].Value = "Chưa dùng";
                            }
                            if (item.Status == 1)
                            {
                                worksheet.Cells[row, col].Value = "Đã dùng";
                            }
                            if (item.Status == -1)
                            {
                                worksheet.Cells[row, col].Value = "Thất bại";
                            }
                            if (item.Status == -7)
                            {
                                worksheet.Cells[row, col].Value = "Trùng mã";
                            }
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

        [PermissionFilter(FunctionCode = FunctionCode.OrderReport)]
        public ActionResult Report()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            //var lstUser = new List<Users>();
            //if (ViewBag.IsAdmin)
            //{
            //    lstUser = _userservice.GetAll().Where(x => x.Type == 2).ToList();
            //    lstUser.Insert(0, new Users { UserID = 1, Username = "--Tất cả--" });
            //}
            //else
            //{
            //    if (CurrentFullUser.Type == 2)
            //    {
            //        lstUser = _userservice.GetAll().Where(x => x.CreatedUser == CurrentUser.Username).ToList();
            //        lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
            //        lstUser.Insert(1, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
            //    }
            //    else
            //    {
            //        if (CurrentUser.Type == 3)
            //        {


            //            lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
            //        }

            //    }

            //}
            //ViewBag.UserList = lstUser;
            ViewBag.Title = "Báo cáo kho thẻ";
            return View();


        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderReport)]
        public ActionResult ListReport()
        {
            var data = new List<CardOrderReport2>();
            
            if (CurrentFullUser.Type == 1)
            {
                data = _cardorderhervice.GetReport();
            }
            var listCardAmount = data.GroupBy(x => x.Amount).Select(a => a.Key).OrderBy(x => x).ToList();
            var lstTelco = data.GroupBy(x => x.Telco).Select(a => a.Key).ToList();
            //if (CurrentFullUser.Type == 2)
            //{
            //    data = _cardorderhervice.GetReportDaily( CurrentFullUser.Username, username, OrderNo, FromDate, ToDate);
            //}
            //if (CurrentFullUser.Type == 3)
            //{
            //    data = _cardorderhervice.GetReportDaily( "", CurrentFullUser.Username, OrderNo, FromDate, ToDate);
            //}
            ViewBag.listCardAmount = listCardAmount;
            ViewBag.lstTelco = lstTelco;
            return PartialView(data);
        }
    }
}