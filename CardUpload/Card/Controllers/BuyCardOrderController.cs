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

namespace Card.CMS.Controllers
{
    public class BuyCardOrderController : Controller
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
        public BuyCardOrderController(ICardOrderService cardorderhervice, ITopupOrderService topupoderservice, IBuyCardService bservice, IAccountTokenService accounttokenservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
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
        [PermissionFilter(FunctionCode = FunctionCode.BuyCardList)]
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
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.Title = "Danh sách mua thẻ";
            ViewBag.UserList = lstUser;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.BuyCardList)]
        public ActionResult ListBuyCard(string username, int top, string telco,string cardseri,int status,long?id, string FromDate, string ToDate)
        {
            var data = new List<BuyCard>();
            if (username == "--Tất cả--")
                username = "";
            if (CurrentFullUser.Type == 1)
            {
                data = _bservice.GetList(top, username, "", telco, cardseri, status, id, FromDate, ToDate);
            }
            if (CurrentFullUser.Type == 2)
            {
                data = _bservice.GetList(top, CurrentFullUser.Username, username, telco, cardseri, status, id, FromDate, ToDate);
            }
            if (CurrentFullUser.Type == 3)
            {
                data = _bservice.GetList(top, "", CurrentFullUser.Username, telco, cardseri, status, id, FromDate, ToDate);
            }
            ViewBag.Type = CurrentFullUser.Type;
            ViewBag.Username = CurrentFullUser.Username;
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.BuyCardList)]
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
            ViewBag.Owner = "1";
            if (buycard.UserName != CurrentFullUser.Username)
            {
                if(CurrentFullUser.Type==1)
                {
                    ViewBag.Owner = "0";
                }
                else
                {
                    if (buycard.ParrentName == CurrentFullUser.Username)
                    {
                        ViewBag.Owner = "0";
                    }
                    else
                    {
                        return PartialView(null);
                    }
                }
                
            }
            var data = _cardorderhervice.GetByRefCode(Id);
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.BuyCardReport)]
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
        [PermissionFilter(FunctionCode = FunctionCode.BuyCardReport)]
        public ActionResult ListReport(string username, string FromDate, string ToDate, string Telco)
        {
            var data = new List<BuyCardReport>();
            if (username == "--Tất cả--")
                username = "";
            if (CurrentFullUser.Type == 1)
            {
                data = _bservice.GetReportDaily(username, "", Telco, FromDate, ToDate);
            }
            if (CurrentFullUser.Type == 2)
            {
                data = _bservice.GetReportDaily(CurrentFullUser.Username, username, Telco, FromDate, ToDate);
            }
            if (CurrentFullUser.Type == 3)
            {
                data = _bservice.GetReportDaily("", CurrentFullUser.Username, Telco, FromDate, ToDate);
            }
            ViewBag.Type = CurrentFullUser.Type;
            return PartialView(data);
        }
    }
}