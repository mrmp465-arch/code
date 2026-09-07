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
using Google.Authenticator;

namespace Card.CMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUsersService _userservice;
        private readonly IAccountTokenService _accounttokenservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        private readonly IBuyCardService _bservice;
        // private readonly IOrderReportsService _orderservice;
        private readonly ITransactionsService _transervice;
        //private readonly IBidHistoryService _bidHistoryservice;

        private readonly ITopupOrderService _topupoderservice;
        private readonly ICardOrderService _cardorderhervice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } set { } }
        public HomeController(ICardOrderService cardorderhervice, ITopupOrderService topupoderservice, IBuyCardService bservice, IAccountTokenService accounttokenservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _accounttokenservice = accounttokenservice;
            _bservice = bservice;
            _cardorderhervice = cardorderhervice;
            _topupoderservice = topupoderservice;
        }
        public ActionResult Index()
        {

            if (CurrentUser == null)
                return RedirectToAction("Login", "Account");
            var user = _userservice.GetByUsername(CurrentUser.Username);
            ViewBag.Type = user.Type;
            Session[SessionsManager.SESSION_USER_FULL] = user;

            //ViewBag.Type = CurrentFullUser.Type;
            return View();

        }



        public ActionResult ErrorPermission()
        {


            return View();
        }
        public ActionResult ErrorNotPage()
        {


            return View();
        }
        public ActionResult F2A()
        {
            string googleAuthKey = "cardstore123";
            string UserUniqueKey = ("admin" + googleAuthKey);
            //Two Factor Authentication Setup
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            var setupInfo = TwoFacAuth.GenerateSetupCode("cardstore.com", "admin", Helper.HtmlHelpers.ConvertSecretToBytes(UserUniqueKey, false), 300);
            //Session["UserUniqueKey"] = UserUniqueKey;
            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            ViewBag.SetupCode = setupInfo.ManualEntryKey;
            return View();
        }
        public ActionResult TopupAdmin()
        {
            var user = _userservice.GetByUsername("admin");
                if (user.Balance < 1000000000)
                _transervice.Topup("admin", 1000000000, "cộng tiền cho admin", "", 1);
            return View();
        }

        public ActionResult Header()
        {
            // var userinfo = (UserSession)Session[SessionsManager.SESSION_USER];
            var user = _userservice.GetByUsername(CurrentUser.Username);
            return PartialView(user);
        }
        public ActionResult Balance(Users user)
        {

            ViewBag.Balance = user.Balance;
            ViewBag.BalanceHold = user.BalanceHold > 0 ? user.BalanceHold : 0;
            return PartialView();
        }



        public ActionResult Menu()
        {
            var userinfo = (UserSession)Session[SessionsManager.SESSION_USER];
            if (CurrentFullUser == null)
            {
                var m_Users = _userservice.GetByUsername(CurrentUser.Username);
                Session[SessionsManager.SESSION_USER_FULL] = m_Users;
                CurrentFullUser = m_Users;
            }
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
        public ActionResult BankOrder()
        {
            var BankList = BankAPI.GetBankCache();


            return PartialView(BankList);
        }




        public ActionResult CardOrder()
        {
            ViewBag.PercentVTT = CurrentFullUser.PercentVTT;
            ViewBag.PercentVNP = CurrentFullUser.PercentVNP;
            ViewBag.PercentVMS = CurrentFullUser.PercentVMS;
            ViewBag.PercentGarena = CurrentFullUser.PercentGarena;
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
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
                if (CurrentFullUser.Type > 1)
                {

                    ReturnData.ResponseCode = -101;
                    ReturnData.Description = "Bạn không có quyền sử dụng chức năng này";
                    return Json(ReturnData);

                }
                var PreOrderNo = lstorder[0].OrderNo;
                var userinfo = _userservice.GetByUsername(CurrentFullUser.Username);
                //var parrentUser = _userservice.GetByUsername(CurrentFullUser.CreatedUser);

                foreach (var order in lstorder)
                {
                    var OrderNo = String.Format("{0}_{1}", order.Telco.ToUpper(), DateTime.Now.ToString("ddMMyy"));
                    if (!string.IsNullOrEmpty(PreOrderNo))
                        OrderNo = OrderNo + "_" + PreOrderNo;
                    //tạo order
                    var fee = 0;
                    //if (order.Telco.ToUpper() == "VINA")
                    //{
                    //    fee = userinfo.PercentVNP;
                    //}
                    //if (order.Telco.ToUpper() == "MOBI")
                    //{
                    //    fee = userinfo.PercentVMS;
                    //}
                    //if (order.Telco.ToUpper() == "GARENA")
                    //{
                    //    fee = userinfo.PercentGarena;
                    //}
                    var orderObj = new CardOrder
                    {
                        OrderNo = OrderNo,
                        AmountSuccess = 0,
                        CardCode = order.CardCode,
                        CardSerial = order.CardSerial,
                        Amount = order.Amount,
                        Telco = order.Telco,
                        UserName = CurrentFullUser.Username,
                        ParrentName = CurrentFullUser.Username,
                        Fee = fee,
                        Reward = 0,
                    };

                    //cấp 2
                    //if (CurrentFullUser.Type > 2)
                    //{
                    //    orderObj.Reward = userinfo.PercentVTT - parrentUser.PercentVTT;
                    //    orderObj.ParrentName = parrentUser.Username;
                    //}

                    if (orderObj.Amount > 0 && orderObj.CardCode.Length > 5 && orderObj.CardSerial.Length > 5)
                        _cardorderhervice.Add(orderObj);
                }
                _userlogservice.InsertUsersLog(new UsersLog
                {
                    FunctionCode = "Orders",
                    Description = String.Format("Tạo đơn thẻ mã kho {0}", PreOrderNo),
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

        public ActionResult ListCardOrder()
        {
            if (CurrentFullUser == null)
            {

                return PartialView(null);

            }
            if (CurrentFullUser.Type > 1)
            {

                return PartialView(null);

            }
            var data = new List<CardOrder>();
            data = _cardorderhervice.GetList(10, "", CurrentFullUser.Username, "", "", "");


            return PartialView(data);
        }

        public ActionResult TopupOrder()
        {
            ViewBag.PercentVTTTT = CurrentFullUser.PercentVTTTT;
            ViewBag.PercentVTTTS = CurrentFullUser.PercentVTTTS;
            ViewBag.PercentVTTMY = CurrentFullUser.PercentVTTMY;
            ViewBag.PercentVNPTT = CurrentFullUser.PercentVNPTT;
            ViewBag.PercentVNPTS = CurrentFullUser.PercentVNPTS;
            ViewBag.PercentVNPMY = CurrentFullUser.PercentVNPMY;
            ViewBag.PercentVMSMY = CurrentFullUser.PercentVMSMY;
            ViewBag.PercentVMSNH = CurrentFullUser.PercentVMSNH;
            ViewBag.PercentVNMNH = CurrentFullUser.PercentVNMNH;
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
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
                var PreOrderNo = lstorder[0].OrderNo;
                var userinfo = _userservice.GetByUsername(CurrentFullUser.Username);
                var parrentUser = _userservice.GetByUsername(CurrentFullUser.CreatedUser);
                var OrderNo = String.Format("{0}_{1}", CurrentFullUser.Config, DateTime.Now.ToString("ddMMyy"));
                if (CurrentFullUser.Type > 2)
                {

                    OrderNo = String.Format("{0}_{1}_{2}", parrentUser.Config, CurrentFullUser.Config, DateTime.Now.ToString("ddMM"));

                }
                if (!string.IsNullOrEmpty(PreOrderNo))
                    OrderNo = OrderNo + "_" + PreOrderNo;
                var totalAmountSuccess = 0;
                var lstOrderData = new List<TopupOrder>();
                foreach (var order in lstorder)
                {
                    //tạo order
                    double fee = 0;
                    if (order.Telco.ToUpper() == "VIETTEL")
                    {
                        if (order.TopupType.ToUpper() == "TT")
                            fee = userinfo.PercentVTTTT;
                        if (order.TopupType.ToUpper() == "TS")
                            fee = userinfo.PercentVTTTS;
                        if (order.TopupType.ToUpper() == "MY")
                            fee = userinfo.PercentVTTMY;
                    }
                    if (order.Telco.ToUpper() == "VINA")
                    {
                        if (order.TopupType.ToUpper() == "TT")
                            fee = userinfo.PercentVNPTT;
                        if (order.TopupType.ToUpper() == "TS")
                            fee = userinfo.PercentVNPTS;
                        if (order.TopupType.ToUpper() == "MY")
                            fee = userinfo.PercentVNPMY;
                    }
                    if (order.Telco.ToUpper() == "MOBI")
                    {
                        if (order.TopupType.ToUpper() == "NH")
                            fee = userinfo.PercentVMSNH;
                        if (order.TopupType.ToUpper() == "MY")
                            fee = userinfo.PercentVMSMY;
                    }
                    if (order.Telco.ToUpper() == "VNMOBILE")
                    {
                        fee = userinfo.PercentVNMNH;
                    }
                    var orderObj = new TopupOrder
                    {

                        OrderNo = OrderNo,
                        AmountSuccess = 0,
                        CardValue = order.CardValue,
                        Account = order.Account,
                        Priority = order.Priority,
                        Password = order.Password,
                        TopupType = order.TopupType,
                        Amount = order.Amount,
                        Telco = order.Telco,
                        UserName = CurrentFullUser.Username,
                        ParrentName = CurrentFullUser.Username,
                        Fee = fee,
                        Reward = 0,
                        RefCode = ""
                    };
                    if (order.Amount < 5000)
                        continue;
                    //cấp 2
                    if (CurrentFullUser.Type > 2)
                    {
                        orderObj.Reward = parrentUser.PercentVTTTT - userinfo.PercentVTTTT;
                        orderObj.ParrentName = parrentUser.Username;
                    }
                    if (orderObj.Priority < 0)
                        orderObj.Priority = 0;

                    var amount = order.Amount;
                    amount = (int)(amount / 100 * (100 - orderObj.Fee + orderObj.Priority));
                    totalAmountSuccess += amount;
                    lstOrderData.Add(orderObj);
                }
                var resultDeduct = _transervice.DeductHold(CurrentFullUser.Username, totalAmountSuccess, "Trừ tạm tiền đơn topup mã đơn:  " + lstOrderData[0].OrderNo);
                if (resultDeduct < 0)
                {
                    switch (resultDeduct)
                    {
                        case -98: ReturnData.Description = "Số dư tài khoản không đủ để thực hiện giao dịch"; break;
                        default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                    }
                    ReturnData.ResponseCode = -1;
                    return Json(ReturnData);

                }
                foreach (var order in lstOrderData)
                {
                    _topupoderservice.Add(order);
                }

                _userlogservice.InsertUsersLog(new UsersLog
                {
                    FunctionCode = "Orders",
                    Description = String.Format("Tạo đơn topup mã đơn {0}", OrderNo),
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

        public ActionResult ListTopupOrder()
        {
            var data = new List<TopupOrder>();
            data = _topupoderservice.GetList(10, "", CurrentFullUser.Username, "", "", "");


            return PartialView(data);
        }
        public ActionResult BuyCard()
        {
            ViewBag.PercentVTT = CurrentFullUser.PercentVTT;
            ViewBag.PercentVNP = CurrentFullUser.PercentVNP;
            ViewBag.PercentVMS = CurrentFullUser.PercentVMS;
            ViewBag.PercentGarena = CurrentFullUser.PercentGarena;
            return PartialView();
        }
        public ActionResult ListBuyCard()
        {
            if (CurrentFullUser == null)
            {

                return PartialView(null);

            }
            //if (CurrentFullUser.Type > 1)
            //{

            //    return PartialView(null);

            //}
            // var data = new List<BuyCard>();
            var data = _bservice.GetList(10, "", CurrentFullUser.Username, "", "", -1000, null, "", "");


            return PartialView(data);
        }
        public ActionResult ListCardInfo(long Id)
        {
            if (CurrentFullUser == null)
            {

                return PartialView(null);

            }
            var buycard = _bservice.GetDetail(Id);
            if (buycard == null)
            {

                return PartialView(null);

            }
            if (buycard.UserName != CurrentFullUser.Username)
            {
                return PartialView(null);
            }
            var data = _cardorderhervice.GetByRefCode(Id);
            return PartialView(data);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult BuyCard(int cardNumber, int cardValue, string telco)
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
                var totalAmount = cardNumber * cardValue;
                var userinfo = _userservice.GetByUsername(CurrentFullUser.Username);
                double fee = userinfo.PercentVTT;
                if (telco.ToUpper() == "VINA")
                {
                    fee = userinfo.PercentVNP;
                }
                if (telco.ToUpper() == "MOBI")
                {
                    fee = userinfo.PercentVMS;
                }
                if (telco.ToUpper() == "GARENA")
                {
                    fee = userinfo.PercentGarena;
                }
                double reward = 0;
                var parrentUser = _userservice.GetByUsername(CurrentFullUser.CreatedUser);
                if (CurrentFullUser.Type > 2)
                {

                    reward = parrentUser.PercentVTT - userinfo.PercentVTT;

                }
                var money = (int)(totalAmount / 100 * (100 - fee));
                var moneyReward = (int)(totalAmount / 100 * reward);

                if (userinfo.Balance < money)
                {

                    ReturnData.ResponseCode = -3;
                    ReturnData.Description = "Số dư không đủ để thực hiện giao dịch";
                }

                var orderObj = new BuyCard
                {
                    MoneyReward = moneyReward,
                    Money = money,
                    Fee = fee,
                    Reward = reward,
                    CardNumber = cardNumber,
                    CardValue = cardValue,
                    Amount = totalAmount,
                    Telco = telco,
                    UserName = CurrentFullUser.Username,
                    ParrentName = CurrentFullUser.Username,
                    Type = 1,
                    RefCode = ""

                };

                //cấp 2
                if (CurrentFullUser.Type > 2)
                {
                    //orderObj.Reward = userinfo.PercentVTT - parrentUser.PercentVTT;
                    orderObj.ParrentName = parrentUser.Username;
                }


                var id = _bservice.Add(orderObj);
                if (id > 0)
                {
                    var result = _transervice.BuyCard(orderObj.UserName, orderObj.ParrentName, money, moneyReward, orderObj.Telco, orderObj.CardValue, orderObj.CardNumber, id);
                    if (result > 0)
                    {
                        ReturnData.ResponseCode = 1;
                        ReturnData.Description = "Mua thẻ thành công. Mã giao dịch " + id;
                    }
                    else
                    {
                        ReturnData.ResponseCode = result;
                        if (result == -99)
                            ReturnData.Description = "Có lỗi trong quá trình xử lý";
                        if (result == -2)
                            ReturnData.Description = "Kho thẻ không đủ, vui lòng liên hệ admin";
                        if (result == -3)
                            ReturnData.Description = "Số dư không đủ để thực hiện giao dịch";
                    }
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