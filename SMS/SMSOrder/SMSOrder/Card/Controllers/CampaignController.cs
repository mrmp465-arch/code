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
namespace SMS.CMS.Controllers
{
    public class CampaignController : Controller
    {
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
        public CampaignController(ISMSDictionaryService dicservice, ISMSLogsService smsservice, ICampaignsService cservice, IGroupsService groupservice, IContactsService contactservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
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
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        public ActionResult Index()
        {
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        public ActionResult ListCampaign(string email, int? currentPage, int? pageSize)
        {
            var data = new List<Campaigns>();

            string Email = string.IsNullOrEmpty(email) ? string.Empty : email;

            int TotalRecord = 0;
            int CurrPage = currentPage == null ? 1 : (int)currentPage;
            int RecordPerPage = pageSize == null ? 20 : (int)pageSize;
            data = _cservice.GetFilter(Email, CurrentUser.Username, CurrPage, RecordPerPage, ref TotalRecord);

            ViewBag.TotalRecord = TotalRecord;
            ViewBag.PageSize = RecordPerPage;
            ViewBag.CurrentPage = CurrPage;

            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        public ActionResult AddCampaignInfo()
        {
            var obj = new Campaigns
            {
                StartTime = DateTime.Now.AddHours(1),
                Group = "",
                Name = $"[{DateTime.Now.ToString("dd/MM/yyyy")}]",
                Contents= "{xin chao|hello|hi|hey}"

            };

            var createdUser = ViewBag.IsAdmin ? "" : CurrentUser.Username;
            ViewBag.Dictionary = _dicservice.GetAll(createdUser);
            var lstGroup = _groupservice.GetList(2, CurrentUser.Username);
            ViewBag.GroupList = lstGroup;
            return View("GetCampaignInfo", obj);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        public ActionResult GetCampaignInfo(int? id)
        {
            int Id = id == null ? 0 : (int)id;
            var obj = new Campaigns();
            var lstGroup = _groupservice.GetList(2, CurrentUser.Username);
            ViewBag.GroupList = lstGroup;
            if (Id > 0)
            {
                obj = _cservice.Get(Id);
                obj.Group += ",";
            }
            if (obj.Status != 0)
            {
                return View("Index");
            }
            var createdUser = ViewBag.IsAdmin ? "" : CurrentUser.Username;
            ViewBag.Dictionary = _dicservice.GetAll(createdUser);
            ViewBag.Title = "Cập nhật Campaign";
            return View(obj);
        }

        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Delete(string id)
        {
            var ReturnData = new ReturnData();
            try
            {
                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 0)
                {
                    var result = _cservice.Delete(Id, CurrentUser.Username);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        ReturnData.Description = "Xóa thành Công";
                        return Json(ReturnData);
                    }
                    else switch (result)
                        {
                            case -600: ReturnData.Description = "Không thể xóa dữ liệu"; break;
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định dữ liệu cần xóa";
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
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Send(string id)
        {
            var ReturnData = new ReturnData();
            try
            {
                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 0)
                {
                    var result = _cservice.Send(Id, CurrentUser.Username);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        //Ghi log
                        _userlogservice.InsertUsersLog(new UsersLog
                        {
                            FunctionCode = "Deduct",
                            Description = "Chốt gửi tin nhắn CampainId: " + Id + ", trừ tạm tiền " + result,
                            UserID = CurrentUser.UserID,
                            UserName = CurrentUser.Username,
                            ClientIP = Config.GetIP()
                        });
                        ReturnData.Description = "Cập nhật thành Công";

                        return Json(ReturnData);
                    }
                    else switch (result)
                        {
                            case -600: ReturnData.Description = "Không thể cập nhật dữ liệu"; break;
                            case -98: ReturnData.Description = "Số dư tài khoản không đủ để gửi sms"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định dữ liệu cần cập nhật";
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
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Lock(string id)
        {
            var ReturnData = new ReturnData();
            try
            {
                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 0)
                {
                    var result = _cservice.Lock(Id, CurrentUser.Username);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        ReturnData.Description = "Khóa thành Công";
                        return Json(ReturnData);
                    }
                    else switch (result)
                        {
                            case -600: ReturnData.Description = "Không thể cập nhật dữ liệu"; break;
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định dữ liệu cần cập nhật";
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
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult UnLock(string id)
        {
            var ReturnData = new ReturnData();
            try
            {
                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 0)
                {
                    var result = _cservice.UnLock(Id, CurrentUser.Username);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        ReturnData.Description = "Mở Khóa thành Công";
                        return Json(ReturnData);
                    }
                    else switch (result)
                        {
                            case -600: ReturnData.Description = "Không thể cập nhật dữ liệu"; break;
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định dữ liệu cần cập nhật";
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
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Preview(string group,string content)
        {
            var ReturnData = new ReturnData();
            try
            {

                ReturnData.ResponseCode = 1;
                ReturnData.Description = _cservice.Preview(group,content);
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
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Confirm(string id)
        {
            var ReturnData = new ReturnData();
            try
            {
                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 0)
                {
                    var result = _cservice.Confirm(Id, CurrentUser.Username);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        //Ghi log
                        _userlogservice.InsertUsersLog(new UsersLog
                        {
                            FunctionCode = "Deduct",
                            Description = "Chốt CampainId: " + Id + ", trừ  tiền " + result,
                            UserID = CurrentUser.UserID,
                            UserName = CurrentUser.Username,
                            ClientIP = Config.GetIP()
                        });
                        ReturnData.Description = "Chốt thành Công";
                        return Json(ReturnData);
                    }
                    else switch (result)
                        {
                            case -600: ReturnData.Description = "Không thể cập nhật dữ liệu"; break;
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định dữ liệu cần cập nhật";
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
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        public JsonResult SaveData(CampaignsForm item)
        {
            var ReturnData = new ReturnData();

            try
            {
                IFormatProvider culture = new CultureInfo("en-US", true);
                var contact = new Campaigns
                {
                    Id = item.Id,
                    CreatedUser = CurrentUser.Username,
                    Contents = StringUtils.ReplaceVietnameseChar(item.Contents),
                    Name = item.Name,
                    StartTime = DateTime.ParseExact(item.StartTime, "dd/MM/yyyy HH:ss", culture),
                    Status = 0,
                    Confirm = 0,
                    Type = 1,
                    Group = item.Group,


                };
                //update
                if (contact.Id > 0)
                {
                    var oldobj = _cservice.Get(contact.Id);
                    var result = _cservice.InsertUpdate(contact);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        ReturnData.Description = "Thêm mới Thành Công";
                        if (oldobj.Group != contact.Group || oldobj.Contents != contact.Contents)
                        {
                            _cservice.SetData(contact.Id, 1, CurrentUser.Username, CurrentFullUser.Piority);
                        }
                        else
                        if (oldobj.StartTime != contact.StartTime)
                        {

                            _smsservice.UpdateTime(contact.Id, contact.StartTime);
                        }

                    }
                    else switch (result)
                        {
                            case -51: ReturnData.Description = "Tài khoản đã tồn tại"; break;
                            case -52: ReturnData.Description = "Email đã tồn tại"; break;
                            case -600: ReturnData.Description = "Tham số truyền vào không hợp lệ"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                }
                //tạo mới
                else
                {
                    var result = _cservice.InsertUpdate(contact);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        _cservice.SetData(result, 0, CurrentUser.Username, CurrentFullUser.Piority);
                        ReturnData.Description = "Thêm mới Thành Công";


                    }
                    else switch (result)
                        {
                            case -51: ReturnData.Description = "Tài khoản đã tồn tại"; break;
                            case -52: ReturnData.Description = "Email đã tồn tại"; break;
                            case -600: ReturnData.Description = "Tham số truyền vào không hợp lệ"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
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
        [PermissionFilter(FunctionCode = FunctionCode.Campaign)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Copy(string id)
        {
            var ReturnData = new ReturnData();
            try
            {
                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 0)
                {

                    var oldObj = _cservice.Get(Id);
                    var obj = new Campaigns
                    {
                        StartTime = DateTime.Now.AddHours(1),
                        Group = oldObj.Group,
                        Status = 0,
                        CreatedUser = CurrentUser.Username,
                        Contents = oldObj.Contents,
                        Confirm = 0,
                        Type = 1,
                        Name = "[Copy] " + oldObj.Name

                    };
                    var result = _cservice.InsertUpdate(obj);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        _cservice.SetData(result, 0, CurrentUser.Username, CurrentFullUser.Piority);
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
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định dữ liệu cần copy";
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
        [PermissionFilter(FunctionCode = FunctionCode.Campaign, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult SMSLog(string id)
        {
            ViewBag.id = id;


            return PartialView();
        }


        [PermissionFilter(FunctionCode = FunctionCode.Campaign, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult ListSMSLog(string id, string keyword,string telco,string port,int status, int? currentPage, int? PageSize)
        {
            ViewBag.id = id;
            var userId = 1;
            if (!ViewBag.IsAdmin)
            {
                userId = CurrentUser.UserID;
            }
            ViewBag.userId = userId;



            var usser = _userservice.SelectByUserID(userId);
            int Id = int.Parse(Encrypt.Base64Decode(id));
            if (Id > 0)
            {
                int TotalRecord = 0;
                int CurrentPage = currentPage ?? 1;
                int pageSize = PageSize ?? 20;
                var data = _smsservice.GetFilter(usser,Id, keyword,port,telco,status, CurrentPage, pageSize, ref TotalRecord);
                ViewBag.TotalRecord = TotalRecord;
                ViewBag.CurrentPage = CurrentPage;

                ViewBag.PageSize = pageSize;
                return PartialView(data);

            }
            
            return PartialView(null);
        }
        [PermissionFilter(FunctionCode = FunctionCode.SMSSearch)]
        public ActionResult SMS()
        {
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
            return View();
        }


        [PermissionFilter(FunctionCode = FunctionCode.SMSSearch)]
        public ActionResult ListSMS(int userId, string keyword, string telco, string port, int status, int? currentPage, int? PageSize)
        {
            if (!ViewBag.IsAdmin && !ViewBag.IsSupport)
            {
                userId = CurrentUser.UserID;
            }
            ViewBag.userId = userId;

          

                var usser = _userservice.SelectByUserID(userId);
            int TotalRecord = 0;
            int CurrentPage = currentPage ?? 1;
            int pageSize = PageSize ?? 50;
            var data = _smsservice.GetFilter(usser ,- 1, keyword, port, telco, status, CurrentPage, pageSize, ref TotalRecord);
            ViewBag.TotalRecord = TotalRecord;
            ViewBag.CurrentPage = CurrentPage;

            ViewBag.PageSize = pageSize;
            return PartialView(data);
        }
    }
}