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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace SMS.CMS.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUsersService _userservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        private readonly ITransactionsService _transervice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } }
        public OrderController(IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult TransactionOrder(string OrderNo)
        {
            ViewBag.OrderNo = OrderNo;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult ListTransactionOrder(int status, int confirm, int top, string orderNo, string telco)
        {

            int? Status = null;
            if (status > -1000)
            {
                Status = status;
            }
            int? Confirm = null;
            if (confirm > -1000)
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
            //if (CurrentUser.Type == 2)
            //{
            //    ViewBag.Config = "";
            //}
            var data = ServerProcess.ListTransaction(token, top, telco, orderNo, Status, Confirm);
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult ReportTransactionOrder(string orderNo)
        {
            if(CurrentFullUser.Type>2)
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
            ViewBag.OrderNo = orderNo;
           
            var data = ServerProcess.ListTransaction(token, 1000, "", orderNo, null, null);
            var lsdata = new List<OrderOutputExcel>();
            foreach(var item in data)
            {
                lsdata.Add(
                    new OrderOutputExcel
                    {
                        Amount=item.Amount,
                        AmountMin=item.AmountMin,
                        AmountMinAll=item.AmountMinAll,
                        AmountPending=item.AmountPending,
                        AmountTopupSuccess=item.AmountTopupSuccess,
                        CreatedTime=item.CreatedTime,
                        //FullName=item.FullName,
                        IsConfirm=item.IsConfirm,
                        LastTime=item.LastTime,
                        LogContent=item.LogContent,
                        Mobile=item.Mobile,
                        OrderNo=item.OrderNo,
                        Priority=item.Priority,
                        RequestNo=item.RequestNo,
                        Status=item.Status,
                        Telco=item.Telco,
                        TopupType=item.TopupType,
                        TransactionID=item.TransactionID,
                  
                    }
                    );
            }
            Response.Clear();
            Response.Buffer = true;
            //Response.Charset = "UTF-8"; 
            Response.AppendHeader("Content-Disposition", "attachment;filename="+ orderNo + ".xls");
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Response.ContentType = "application/ms-excel";
          
            var myCItrad = new CultureInfo("VI-VN", true);
            var oStringWriter = new StringWriter(myCItrad);
            var oHtmlTextWriter = new HtmlTextWriter(oStringWriter);



            var grid = new DataGrid { DataSource = lsdata };
            grid.DataBind();
            grid.GridLines = GridLines.None;
            grid.BorderWidth = 0;

            grid.HeaderStyle.BackColor = Color.FromArgb(79, 129, 189);
            grid.HeaderStyle.ForeColor = Color.White;
            grid.BackColor = Color.Transparent;

            grid.RenderControl(oHtmlTextWriter);

            string style = @"<style> td{ mso-number-format:\@; } </style>";
            Response.Write(style);
            Response.ContentType = "application/text";
            Response.Write(oStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("OrderActive");
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
        public ActionResult ReportTransactionHistory(int transId,string orderNo)
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
            Response.Clear();
            Response.Buffer = true;
            //Response.Charset = "UTF-8"; 
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + orderNo+ transId.ToString() + ".xls");
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Response.ContentType = "application/ms-excel";

            var myCItrad = new CultureInfo("VI-VN", true);
            var oStringWriter = new StringWriter(myCItrad);
            var oHtmlTextWriter = new HtmlTextWriter(oStringWriter);

            var grid = new DataGrid { DataSource = data };
            grid.DataBind();
            grid.GridLines = GridLines.None;
            grid.BorderWidth = 0;
          
            grid.HeaderStyle.BackColor = Color.FromArgb(79, 129, 189);
            grid.HeaderStyle.ForeColor = Color.White;
            grid.BackColor = Color.Transparent;
          
            grid.RenderControl(oHtmlTextWriter);

            string style = @"<style> td{ mso-number-format:\@; } </style>";
            Response.Write(style);
            Response.ContentType = "application/text";
            Response.Write(oStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("OrderActive");
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult OrderActive()
        {
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().ToList();
                lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
            }
            else
            {
                if (CurrentUser.Type == 2|| CurrentUser.Type == 3)
                {
                    lstUser = _userservice.GetAll().Where(x => x.CreatedUser == CurrentUser.Username || x.UserID == CurrentUser.UserID).ToList();
                    lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                }
                else
                {
                    lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
                }
            }



            ViewBag.UserList = lstUser;
            ViewBag.Title = "Danh sách đơn active";
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult ResetTran(OrderOutput item)
        {
            var ReturnData = new ReturnData();
            if (CurrentFullUser.Type >= 2)
            {
                if (!item.OrderNo.Contains("_" + CurrentFullUser.Config + "_"))
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

            item.Status = 1;
            var result = ServerProcess.OrderUpdate(item, token);
            if (result >= 0)
            {
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
            if (CurrentFullUser.Type>=2)
            {
                if (!item.OrderNo.Contains("_" + CurrentFullUser.Config + "_"))
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

            item.Status = (int)Enums.Status.Lock;
            var result = ServerProcess.OrderUpdate(item, token);
            if (result >= 0)
            {
                ReturnData.Description = "Khóa Thành Công";
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
            if (!ViewBag.IsAdmin)
            {
                if (!item.OrderNo.Contains("_" + CurrentFullUser.Config + "_"))
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

            //item.Status = (int)Enums.Status.Lock;
            var result = ServerProcess.OrderUpdate(item, token);
            if (result >= 0)
            {
                ReturnData.Description = "Cập nhật Thành Công";
                ReturnData.ResponseCode = 1;
               
                return Json(ReturnData);
            }
            ReturnData.Description = "Cập nhật thất bại";
            ReturnData.ResponseCode = -99;
            return Json(ReturnData);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult ConfirmTran(OrderOutput item)
        {
            var m_Users = _userservice.SelectByUserID(CurrentUser.UserID);
            Session[SessionsManager.SESSION_USER_FULL] = m_Users;

            var ReturnData = new ReturnData();
            if (!item.OrderNo.Contains("_" + CurrentFullUser.Config + "_"))
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

            item.IsConfirm = 1;
            if(item.Status!= (int)Enums.Status.Success)
            {
                item.Status = (int)Enums.Status.Lock;
            }
           
            var result = ServerProcess.OrderConfirm(item, token);
            if (result >= 0)
            {
                //chốt đơn: tính lại tiền+ ghi log
                var lstdata = ServerProcess.ListTransaction(token, 10, "", item.OrderNo, null, null);
                var currentitem = lstdata.FirstOrDefault(x => x.TransactionID == item.TransactionID);
                var parrentUser = _userservice.GetByUsername(CurrentFullUser.CreatedUser);
                var pecent = 0;
                var pecentParrent = 0;
                switch (currentitem.Telco)
                {
                    case "vnp":
                        pecent = CurrentFullUser.PercentVNP;
                        pecentParrent = parrentUser.PercentVNP;
                        break;
                    case "vtt":
                        pecent = CurrentFullUser.PercentVTT;
                        pecentParrent = parrentUser.PercentVTT;
                        break;
                    default:
                        pecent = CurrentFullUser.PercentVMS;
                        pecentParrent = parrentUser.PercentVMS;
                        break;
                }
                // P1 thì trừ tiếp 2%
                //if (CurrentFullUser.Type >= 3 && CurrentFullUser.Piority == 1)
                //{
                //    pecent -= int.Parse(Config.GetAppsetting("P1Percent"));
                //}
                
                var amountParrent = currentitem.AmountTopupSuccess * ( pecent-pecentParrent)/100;
                var amountSuccess = currentitem.AmountTopupSuccess * pecent/100;
                var amountTopupHold = currentitem.Amount * pecent/100;
                //tk cha là cấp 1 thì ko hoa hồng
                if (parrentUser.Type <=2)
                {
                    amountParrent = 0;
                }
                var resultConfirm = _transervice.Confrim(CurrentFullUser.Username, parrentUser.Username, amountParrent, amountSuccess, amountTopupHold, currentitem.OrderNo + "-" + currentitem.TransactionID);

                _userlogservice.InsertUsersLog(new UsersLog
                {
                    FunctionCode = "Orders",
                    Description = String.Format("Chốt đơn {0}-{1}", item.OrderNo, item.TransactionID),
                    UserID = CurrentFullUser.UserID,
                    UserName = CurrentFullUser.Username,
                    ClientIP = Config.GetIP()
                });
                ReturnData.Description = "Chốt Thành Công";
                ReturnData.ResponseCode = 1;
                return Json(ReturnData);
            }
            ReturnData.Description = "Chốt thất bại";
            ReturnData.ResponseCode = -99;

            return Json(ReturnData);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public JsonResult LockOrder(string order)
        {
            var ReturnData = new ReturnData();
            if (CurrentFullUser.Type >= 2)
            {
                if (!order.Contains("_" + CurrentFullUser.Config + "_"))
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

            var data = ServerProcess.ListTransaction(token, 1000, "", order, null, null);
            foreach (var item in data)
            {
                if (item.Status != (int)Enums.Status.Lock)
                {
                    item.Status = (int)Enums.Status.Lock;
                    ServerProcess.OrderUpdate(item, token);
                }

            }
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
            if (!order.Contains("_" + CurrentFullUser.Config + "_"))
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

            var data = ServerProcess.ListTransaction(token, 1000, "", order, null, null);
            var totalAmount = 0;
            var totalAmountSucccess = 0;
            var telco = "";
            foreach (var item in data)
            {
                if (item.IsConfirm != 1)
                {
                    item.IsConfirm = 1;
                    if (item.Status != (int)Enums.Status.Success)
                    {
                        item.Status = (int)Enums.Status.Lock;
                    }
                    var resultConfirm = ServerProcess.OrderConfirm(item, token);
                    if (resultConfirm >= 0)
                    {
                        totalAmount += item.Amount;
                        totalAmountSucccess += item.AmountTopupSuccess;
                        telco = item.Telco;
                    }

                }

            }
            if(totalAmount>0)
            {
                var parrentUser = _userservice.GetByUsername(CurrentFullUser.CreatedUser);
                var pecent = 0;
                var pecentParrent = 0;
                switch (telco)
                {
                    case "vnp":
                        pecent = CurrentFullUser.PercentVNP;
                        pecentParrent = parrentUser.PercentVNP;
                        break;
                    case "vtt":
                        pecent = CurrentFullUser.PercentVTT;
                        pecentParrent = parrentUser.PercentVTT;
                        break;
                    default:
                        pecent = CurrentFullUser.PercentVMS;
                        pecentParrent = parrentUser.PercentVMS;
                        break;
                }
                // P1 thì trừ tiếp 2%
                //if (CurrentFullUser.Type >= 3 && CurrentFullUser.Piority == 1)
                //{
                //    pecent -= int.Parse(Config.GetAppsetting("P1Percent"));
                //}
                var amountParrent = totalAmountSucccess * (pecent - pecentParrent) / 100;
                var amountSuccess = totalAmountSucccess * pecent / 100;
                var amountTopupHold = totalAmount * pecent / 100;

                //tk cha cấp 1 thì ko phải tính phế
                if (parrentUser.Type <= 2)
                {
                    amountParrent = 0;
                }
                var resultConfirm = _transervice.Confrim(CurrentFullUser.Username, parrentUser.Username, amountParrent, amountSuccess, amountTopupHold, order);

                _userlogservice.InsertUsersLog(new UsersLog
                {
                    FunctionCode = "Orders",
                    Description = String.Format("Chốt đơn {0}", order),
                    UserID = CurrentFullUser.UserID,
                    UserName = CurrentFullUser.Username,
                    ClientIP = Config.GetIP()
                });
            }
           
            ReturnData.Description = "Cập nhật Thành Công";
            ReturnData.ResponseCode = 1;
            return Json(ReturnData);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrderActive)]
        public ActionResult ListOrderActive(int userId, int top, string keyword, string telco)
        {
            var data = new List<OrderGroup>();
            var orderNo = "";
            var token = "";
            if (ViewBag.IsAdmin)
            {
                if (userId != -1)
                {
                    var user = _userservice.SelectByUserID(userId);
                    token = ServerProcess.GetUserTokenCache(user.UserAPI, user.PasswordAPI);
                    orderNo = String.Format("_{0}_", user.Config);
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
                if (CurrentUser.Type == 2)
                {
                    if (userId == -1)
                    {
                        //lấy tất con của nó
                        orderNo ="";
                    }
                    else
                    {
                        var user = _userservice.SelectByUserID(userId);
                        orderNo = String.Format("_{0}_", user.Config);
                    }
                }
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
                            var user = _userservice.SelectByUserID(userId);
                            orderNo = String.Format("_{0}_", user.Config);
                        }
                    }
                    else
                    {
                        orderNo = String.Format("_{0}_", CurrentFullUser.Config);

                    }
                    
                }
            }
            data = ServerProcess.ListGroupActive(token, top, telco, orderNo);
            if (!string.IsNullOrEmpty(keyword))
            {
                if (data != null)
                    data = data.Where(x => x.OrderNo.Contains(keyword)).ToList();
            }
            ViewBag.Config = String.Format("_{0}_", CurrentFullUser.Config);
            //if(CurrentUser.Type==2)
            //{
            //    ViewBag.Config = "";
            //}
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.OrdeConfirm)]
        public ActionResult OrderConfirm()
        {
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().ToList();
                lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
            }
            else
            {
                if (CurrentUser.Type == 2 || CurrentUser.Type == 3)
                {
                    lstUser = _userservice.GetAll().Where(x => x.CreatedUser == CurrentUser.Username || x.UserID == CurrentUser.UserID).ToList();
                    lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
                }
                else
                {
                    lstUser.Insert(0, new Users { UserID = CurrentUser.UserID, Username = CurrentUser.Username });
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
            if (ViewBag.IsAdmin)
            {
                if (userId != -1)
                {
                    var user = _userservice.SelectByUserID(userId);
                    token = ServerProcess.GetUserTokenCache(user.UserAPI, user.PasswordAPI);
                    orderNo = String.Format("_{0}_", user.Config);
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
                if (CurrentUser.Type == 2)
                {
                    if (userId == -1)
                    {
                        //lấy tất con của nó
                        orderNo = "";
                    }
                    else
                    {
                        var user = _userservice.SelectByUserID(userId);
                        orderNo = String.Format("_{0}_", user.Config);
                    }
                }
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
                            var user = _userservice.SelectByUserID(userId);
                            orderNo = String.Format("_{0}_", user.Config);
                        }
                    }
                    else
                    {
                        orderNo = String.Format("_{0}_", CurrentFullUser.Config);

                    }

                }
            }
            data = ServerProcess.ListGroupFinish(token, top, telco, orderNo);
            if (!string.IsNullOrEmpty(keyword))
            {
                if (data != null)
                    data = data.Where(x => x.OrderNo.Contains(keyword)).ToList();
            }
            return PartialView(data);
        }
    }
}