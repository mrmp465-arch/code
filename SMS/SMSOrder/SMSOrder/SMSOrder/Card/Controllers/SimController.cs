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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SMS.Data.Factory;

namespace SMS.CMS.Controllers
{
    public class SimController : Controller
    {
        private readonly IUsersService _userservice;
        private readonly ISimsService _Simservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        private readonly ITransactionSimsService _transervice;
        private readonly IGroupsService _groupservice;
        private readonly IContentsService _contentservice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } }
        public SimController(IContentsService contentservice, IGroupsService groupservice, ISimsService Simservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionSimsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _contentservice = contentservice;
            _Simservice = Simservice;
            _groupservice = groupservice;
        }
        [PermissionFilter(FunctionCode = FunctionCode.USSD)]
        public ActionResult USSD()
        {
           
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.SMSInbox)]
        public ActionResult SMSInbox()
        {
            ViewBag.Number = "";
            if (!ViewBag.IsAdmin)
                ViewBag.Number = "170";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.SMSInbox)]
        public ActionResult ListSMSInbox(string port,  string number)
        {
            int Port = -1;
            if (!string.IsNullOrEmpty(port))
                Port = int.Parse(port);
            var data = ServerProcess.GetSMSInbox(Port, number, Config.sn);



            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.SMSOutbox)]
        public ActionResult SMSOutbox()
        {
            ViewBag.Number = "";
            if (!ViewBag.IsAdmin)
                ViewBag.Number = "170";
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.SMSOutbox)]
        public ActionResult ListSMSOutbox(string port, string number)
        {
            int Port = -1;
            if (!string.IsNullOrEmpty(port))
                Port = int.Parse(port);
            var data = AbstractDAOFactory.Instance().SMSOutboxService().GetFilter(Port, number);



            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.RegisterVTT)]
        public ActionResult RegisterVTT()
        {
            var data = _contentservice.Get(3);
            return View(data);
        }
        [ValidateInput(false)]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.RegisterVTT)]
        public JsonResult SaveNote(string Description)
        {
            var ReturnData = new ReturnData();
            try
            {


                if (!string.IsNullOrEmpty(Description))
                {
                    var data = _contentservice.Get(3);
                    data.Description = Description;
                    var result = _contentservice.InsertUpdate(data);
                    if (result >= 0)
                    {
                        ReturnData.ResponseCode = 1;
                        ReturnData.Description = "Cập nhật trạng thái Thành Công";
                        return Json(ReturnData);
                    }
                    else switch (result)
                        {
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định ";
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
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        public ActionResult Index()
        {
            var lstGroup = _groupservice.GetList(3, "");
            lstGroup.Insert(0, new Groups { GroupID = -1, Name = "--Nhóm--" });

            ViewBag.GroupList = lstGroup;
            return View();
        }

        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        public ActionResult ListSim(int? group, string keyword, string telco, int? status, int? currentPage, int? pageSize)
        {
            var data = new List<Sims>();

            string Email = string.IsNullOrEmpty(keyword) ? string.Empty : keyword;
            int Group = group == null ? -1 : (int)group;
            int TotalRecord = 0;
            int CurrPage = currentPage == null ? 1 : (int)currentPage;
            int Status = status == null ? -1 : (int)status;
            int RecordPerPage = pageSize == null ? 20 : (int)pageSize;
            data = _Simservice.GetFilter(Group, Email, telco, Status, CurrPage, RecordPerPage, ref TotalRecord);

            ViewBag.TotalRecord = TotalRecord;
            ViewBag.PageSize = RecordPerPage;
            ViewBag.CurrentPage = CurrPage;

            var lstGroup = _groupservice.GetList(3, "");

            ViewBag.GroupList = lstGroup;
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        public ActionResult UploadTran()
        {
           
           
            return PartialView();
        }
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        public ActionResult Upload()
        {
            var lstGroup = _groupservice.GetList(3, "");
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            ViewBag.JsonGroup = jsonSerializer.Serialize(lstGroup.Select(x => x.Name).Distinct().ToList()).ToString();
            return PartialView();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Sim, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult UpdateActive(long Id)
        {
            var ReturnData = new ReturnData();
            try
            {


                if (Id > 0)
                {
                    var result = _Simservice.UpdateActive(Id);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        ReturnData.Description = "Cập nhật trạng thái Thành Công";
                    }
                    else switch (result)
                        {
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định sim cần active";
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
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult SaveTranMulti(List<SimsForm> data)
        {
            var ReturnData = new ReturnData();

            try
            {

                if (data == null || data.Count == 0)
                {
                    ReturnData.ResponseCode = -7001;
                    ReturnData.Description = "Bạn chưa chọn quyền cho user";
                    return Json(ReturnData);
                }
                
                foreach (var item in data)
                {
                    _transervice.Topup(item.Number, item.Balance, item.GroupName);

                }
                ReturnData.ResponseCode = 1;
                ReturnData.Description = "";
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
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult SaveMulti(List<SimsForm> data)
        {
            var ReturnData = new ReturnData();

            try
            {

                if (data == null || data.Count == 0)
                {
                    ReturnData.ResponseCode = -7001;
                    ReturnData.Description = "Bạn chưa chọn quyền cho user";
                    return Json(ReturnData);
                }
                string GroupName = data[0].GroupName;
                int GroupId = 0;
                var group = _groupservice.GetByName(GroupName, "");
                if (group != null)
                {
                    GroupId = group.GroupID;

                }
                else
                {
                    group = new Groups { IsActive = true, Name = GroupName, Alias = CurrentUser.Username, Type = 3 };
                    GroupId = _groupservice.InsertUpdate(group);
                    ReturnData.Extended = "add";
                }
                IFormatProvider culture = new CultureInfo("en-US", true);
                foreach (var item in data)
                {
                    var Sim = new Sims
                    {
                        Id = 0,
                        Group = GroupId,
                        Telco = item.Telco,
                        Status = 1,
                        ExpriteDate = DateTime.Now,
                        Password = "1234567a",
                        Number = item.Number,
                        Balance = item.Balance,
                        RealBalance = 0,
                        Note = ""


                    };
                    Sim.Telco = StringUtils.GetTelCoName(StringUtils.GetTelCo(Sim.Number));
                    _Simservice.InsertUpdate(Sim);

                }
                ReturnData.ResponseCode = GroupId;
                ReturnData.Description = GroupName;
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
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(long Id)
        {
            var ReturnData = new ReturnData();
            try
            {
                //int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 0)
                {
                    var result = _Simservice.Delete(Id);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        ReturnData.Description = "Xóa thành Công";
                        return Json(ReturnData);
                    }
                    else switch (result)
                        {
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định user cần xóa";
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
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteByGroup(int Id)
        {
            var ReturnData = new ReturnData();
            try
            {

                if (Id > 0)
                {
                    var group = _groupservice.Get(Id);
                    {
                        if (group.Alias != CurrentUser.Username)
                        {
                            ReturnData.ResponseCode = -100;
                            ReturnData.Description = "Không có quyền xóa dữ liệu";
                        }
                    }
                    var where = " [Group]=" + Id;
                    var result = _Simservice.DeleteDynamic(where);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        ReturnData.Description = "Xóa thành Công";
                        return Json(ReturnData);
                    }
                    else switch (result)
                        {
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                else
                {
                    var where = " [CreatedUser]=" + CurrentUser.Username;
                    var result = _Simservice.DeleteDynamic(where);
                    if (result >= 0)
                    {

                        ReturnData.Description = "Xóa thành Công";
                        return Json(ReturnData);
                    }
                    else switch (result)
                        {
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                ReturnData.ResponseCode = -99;
                ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau";
                return Json(ReturnData);
            }
        }
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteMuti(List<string> arrId)
        {
            var ReturnData = new ReturnData();
            try
            {
                string joinId = string.Empty;
                foreach (var id in arrId)
                {
                    joinId += "," + Encrypt.Base64Decode(id);
                }
                joinId = joinId.TrimStart(',');

                if (!string.IsNullOrEmpty(joinId))
                {
                    var where = " [Id] IN(" + joinId + ")";
                    var result = _Simservice.DeleteDynamic(where);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        ReturnData.Description = "Xóa thành Công";
                        return Json(ReturnData);
                    }
                    else switch (result)
                        {
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định user cần xóa";
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
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        public ActionResult AddSimInfo()
        {

            var obj = new Sims
            {

                Group = 1,


            };
            var lstGroup = _groupservice.GetList(3, "");
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            ViewBag.JsonGroup = jsonSerializer.Serialize(lstGroup.Select(x => x.Name).Distinct().ToList()).ToString();
            ViewBag.Title = "Thêm mới sim";
            ViewBag.GroupList = lstGroup;
            return PartialView("GetSimInfo", obj);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        public ActionResult GetSimInfo(int? id)
        {
            int Id = id == null ? 0 : (int)id;
            var obj = new Sims { Balance = 10000 };

            if (Id > 0)
            {
                obj = _Simservice.Get(Id);
            }

            var lstGroup = _groupservice.GetList(3, "");
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            ViewBag.JsonGroup = jsonSerializer.Serialize(lstGroup.Select(x => x.Name).Distinct().ToList()).ToString();
            ViewBag.GroupList = lstGroup;
            ViewBag.Title = "Cập nhật Sim";
            return PartialView(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilter(FunctionCode = FunctionCode.Sim)]
        public JsonResult SaveData(SimsForm item)
        {
            var ReturnData = new ReturnData();

            try
            {
                IFormatProvider culture = new CultureInfo("en-US", true);
                var Sim = new Sims
                {
                    Id = item.Id,

                    Status = 1,
                    ExpriteDate = DateTime.Now,
                    Password = "1234567a",
                    Number = item.Number,
                    Balance = item.Balance,
                    RealBalance = 0,
                    Note = ""

                };
                Sim.Telco = StringUtils.GetTelCoName(StringUtils.GetTelCo(Sim.Number));


                var group = _groupservice.GetByName(item.GroupName, CurrentUser.Username);
                if (group != null)
                {
                    Sim.Group = group.GroupID;

                }
                else
                {
                    group = new Groups { IsActive = true, Name = item.GroupName, Alias = CurrentUser.Username, Type = 3 };
                    Sim.Group = _groupservice.InsertUpdate(group);
                }
                var result = _Simservice.InsertUpdate(Sim);
                ReturnData.ResponseCode = result;
                if (result >= 0)
                {
                    if (Sim.Id > 0)
                        ReturnData.Description = "Cập nhật Thành Công";
                    else
                        ReturnData.Description = "Thêm mới Thành Công";
                }
                else switch (result)
                    {
                        case -51: ReturnData.Description = "Tài khoản đã tồn tại"; break;
                        case -52: ReturnData.Description = "Email đã tồn tại"; break;
                        case -600: ReturnData.Description = "Tham số truyền vào không hợp lệ"; break;
                        default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                    }
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
        [PermissionFilter(FunctionCode = FunctionCode.Sim, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult History(string number)
        {
            ViewBag.number = number;

            int total = 0;
            var data = _transervice.GetList(number, -1, 1, 100, ref total);

            return PartialView(data);

            
        }
        [PermissionFilter(FunctionCode = FunctionCode.Sim, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult Topup(string number)
        {
            ViewBag.number = number;
            return PartialView();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Sim, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult Topup(string number, int amount, string note)
        {
            var ReturnData = new ReturnData();
            try
            {
                var result = _transervice.Topup(number, amount, note);
                ReturnData.ResponseCode = result;
                if (result >= 0)
                {
                    Session[SessionsManager.SESSION_USER_FULL] = null;
                    ReturnData.Description = "Giao dịch Thành Công";
                    return Json(ReturnData);
                }
                else switch (result)
                    {
                        case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                        case -98: ReturnData.Description = "Số dư không đủ để thực hiện giao dịch"; break;
                        case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        case -101: ReturnData.Description = "Không có quyền thực hiện chức năng này"; break;
                        default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                    }
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