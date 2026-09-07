using Card.Data.DTO;
using Card.Data.Service;
using Card.CMS.Filter;
using Card.CMS.Models;

using Card.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Card.CMS.Controllers
{
    public class AdminContentController : Controller
    {
        public UserFunction Permission { get { return ((UserFunction)Session[SessionsManager.SESSION_PERMISSION]); } }
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } set { } }
        private readonly IContentsService _contentservice;
        private readonly IUsersService _userservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IGroupsService _groupservice;
        private readonly IUserRoleService _userroleservice;
        private readonly ITransactionsService _transervice;
        public AdminContentController(IContentsService contentservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IGroupsService groupservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _groupservice = groupservice;
            _transervice = transervice;
            _contentservice = contentservice;
        }
        [PermissionFilter(FunctionCode = FunctionCode.Content)]
        public ActionResult Index()
        {
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.Content)]
        public ActionResult ListContent(int? isActive, string email, int? currentPage, int? pageSize)
        {
            var data = new List<Contents>();

            string Email = string.IsNullOrEmpty(email) ? string.Empty : email;
            int IsActive = isActive == null ? -1 : (int)isActive;
            int TotalRecord = 0;
            int CurrPage = currentPage == null ? 1 : (int)currentPage;
            int RecordPerPage = pageSize == null ? 10000 : (int)pageSize;
            data = _contentservice.GetFilter(IsActive, Email, CurrentFullUser.UserAPI);
            if (data.Count > 0)
                ViewBag.TotalRecord = TotalRecord;
            else
                ViewBag.TotalRecord = 0;
            ViewBag.PageSize = RecordPerPage;
            ViewBag.CurrentPage = CurrPage;
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Content)]
        public ActionResult AddContentInfo()
        {

            var obj = new Contents
            {

                Status = 1,
                PublishDate = DateTime.Now,

            };


            ViewBag.Title = "Thêm mới bài viết";
            return View("GetContentInfo", obj);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Content)]
        public ActionResult GetContentInfo(int? id)
        {
            int Id = id == null ? 0 : (int)id;
            var obj = new Contents();

            if (Id > 0)
            {
                obj = _contentservice.Get(Id);
            }



            ViewBag.Title = "Cập nhập bài viết";
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [PermissionFilter(FunctionCode = FunctionCode.Content)]
        public JsonResult SaveData(Contents obj, string SPublishDate)
        {
            var ReturnData = new ReturnData();

            try
            {
                IFormatProvider culture = new CultureInfo("en-US", true);
                obj.PublishDate = DateTime.ParseExact(SPublishDate, "dd/MM/yyyy HH:mm", culture);

                obj.CreatedUser = CurrentFullUser.UserAPI;
                var result = _contentservice.InsertUpdate(obj);
                ReturnData.ResponseCode = result;
                if (result >= 0)
                {
                    if (obj.Id > 0)
                        ReturnData.Description = "Cập nhật Thành Công";
                    else
                        ReturnData.Description = "Thêm mới Thành Công";

                    if (obj.IsHot == 1)
                    {
                        var where = "Id !=" + result;
                        var update = "IsHot=0";
                        var resultupdate = _contentservice.UpdateUserDynamic(where, update);
                    }


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
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Content)]
        public JsonResult UpdateStatus(int Id, int status)
        {
            var ReturnData = new ReturnData();
            try
            {


                if (Id > 0)
                {


                    var where = " [Id] = " + Id;
                    var update = "Status=" + status;
                    var result = _contentservice.UpdateUserDynamic(where, update);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        //Ghi log

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
                ReturnData.Description = "Không xác định user cần active";
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Content)]
        public JsonResult SetHot(int Id, int status)
        {
            var ReturnData = new ReturnData();
            try
            {


                if (Id > 0)
                {
                    if (status == 1)
                    {
                        var where1 = " [Id] != " + Id;
                        var update1 = "IsHot=0";
                        var result1 = _contentservice.UpdateUserDynamic(where1, update1);

                        var where = " [Id] = " + Id;
                        var update = "IsHot=1";
                        var result = _contentservice.UpdateUserDynamic(where, update);
                        ReturnData.ResponseCode = result;
                    }
                    else
                    {


                        var where = " [Id] = " + Id;
                        var update = "IsHot=0";
                        var result = _contentservice.UpdateUserDynamic(where, update);
                        ReturnData.ResponseCode = result;
                    }

                    if (ReturnData.ResponseCode >= 0)
                    {
                        ReturnData.Description = "Cập nhật trạng thái Thành Công";
                        return Json(ReturnData);


                    }
                    else switch (ReturnData.ResponseCode)
                        {
                            case -50: ReturnData.Description = "Tài Khoản không tồn tại"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định user cần active";
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
        [PermissionFilter(FunctionCode = FunctionCode.Content)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int Id)
        {
            var ReturnData = new ReturnData();
            try
            {

                if (Id > 0)
                {
                    var result = _contentservice.Delete(Id);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        ReturnData.Description = "Xóa bài viết thành Công";
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
        public ActionResult TopContent()
        {
            var data = new List<Contents>();


            data = _contentservice.GetTop(10, CurrentFullUser.UserAPI);
            if (data.Count > 0)
                ViewBag.TotalRecord = data.Count;
            else
                ViewBag.TotalRecord = 0;
            return PartialView(data);
        }
        //[OutputCache(Duration = 600, VaryByParam = "*", VaryByCustom = "browser")]
        public ActionResult GetHotContent()
        {
            var data = _contentservice.GetTop(5, CurrentFullUser.UserAPI);


            return PartialView(data);
        }
        public ActionResult Detail(int Id, string Title)
        {
            var newsobj =  _contentservice.Get(Id);
            return PartialView(newsobj);
        }
    }
}