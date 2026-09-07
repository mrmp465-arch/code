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
using Card.Data.Api;
using System.Drawing;
using OfficeOpenXml.Style;
using System.IO;
using OfficeOpenXml;

namespace Card.CMS.Controllers
{
    public class SystemController : Controller
    {
        // GET: System
        public UserFunction Permission { get { return ((UserFunction)Session[SessionsManager.SESSION_PERMISSION]); } }
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } set { } }
        private readonly IUsersService _userservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IGroupsService _groupservice;
        private readonly IUserRoleService _userroleservice;
        private readonly ITransactionsService _transervice;
        public SystemController(IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IGroupsService groupservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _groupservice = groupservice;
            _transervice = transervice;

        }
        #region Quản trị nhóm
        [PermissionFilter(FunctionCode = FunctionCode.Group)]
        public ActionResult ManageGroup()
        {

            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.Group)]
        public ActionResult ListGroup()
        {
            var data = new List<Groups>();

            data = _groupservice.GetList().Where(x => x.Type == 2).ToList();
            if (CurrentFullUser.Type > 2)
            {
                data = data.Where(x => x.Alias == "2").ToList();
            }
            else
            {
                data = data.Where(x => x.Alias == "1").ToList();
            }
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Group, FunctionType = (int)Enums.FunctionType.IsFullControl)]
        public ActionResult GroupPermisstion()
        {
            var data = new List<Groups>();

            data = _groupservice.GetList();

            return View(data.Where(x => x.Type == 1).ToList());
        }
        [PermissionFilter(FunctionCode = FunctionCode.Group, FunctionType = (int)Enums.FunctionType.IsInsert)]
        public ActionResult AddGroup()
        {
            var result = new Groups();

            ViewBag.Title = "Thêm mới nhóm";
            return View("GetGroupInfo", result);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Group, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult GetGroupInfo(int? id)
        {
            var result = new Groups();
            int Id = id == null ? 0 : (int)id;
            result = _groupservice.Get(Id);
            ViewBag.Title = "Cập nhập nhóm";
            return View(result);
        }

        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Group, FunctionType = (int)Enums.FunctionType.IsInsert)]
        [ValidateAntiForgeryToken]
        public JsonResult SaveDataGroup(Groups function)
        {
            var ReturnData = new ReturnData();


            try
            {
                if (CurrentFullUser.Type > 2)
                {
                    function.Alias = "2";
                }
                else
                {
                    function.Alias = "1";
                }
                var result = _groupservice.InsertUpdate(function);
                ReturnData.ResponseCode = result;
                if (result >= 0)
                {
                    string Description = "";
                    if (function.GroupID > 0)
                    {
                        ReturnData.Description = "Cập nhật Thành Công";
                        Description = ReturnData.Description + " nhóm = " + function.Name;
                    }
                    else
                    {
                        ReturnData.Description = "Thêm mới Thành Công";
                        Description = ReturnData.Description + " nhóm = " + function.Name;
                    }
                    //Ghi log

                    _userlogservice.InsertUsersLog(new UsersLog
                    {
                        FunctionCode = ViewBag.FunctionCode,
                        Description = Description,
                        UserID = CurrentUser.UserID,
                        UserName = CurrentUser.Username,
                        ClientIP = Config.GetIP()
                    });
                }
                else switch (result)
                    {
                        case -70: ReturnData.Description = "Tên chức năng đã tồn tại"; break;
                        case -71: ReturnData.Description = "Đường dẫn đã tồn tại"; break;
                        case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
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
        [PermissionFilter(FunctionCode = FunctionCode.Group, FunctionType = (int)Enums.FunctionType.IsFullControl)]
        public ActionResult GetGrantGroup(int id)
        {

            var functionModel = new UserFunctionModel();
            var userfunction = _userroleservice.GroupFunction_GetByID(id);
            var functionlist = _functionservice.SelectAllFunctionID(-1, string.Empty, 1, -1);
            functionModel.ListFunction = functionlist;
            functionModel.UserFunction = userfunction;
            ViewBag.GroupId = id;
            ViewBag.Title = "Thông Tin Phân Quyền";
            return View(functionModel);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Group, FunctionType = (int)Enums.FunctionType.IsFullControl)]
        [HttpPost]

        public JsonResult SaveGrantGroup(List<UserFunction> listUserFunction)
        {
            var ReturnData = new ReturnData();

            try
            {

                //if (Permission == null || Permission.FunctionID != (int)Enums.FunctionId.GrantUser || (!Permission.IsInsert || !Permission.IsUpdate || !Permission.IsDelete))
                //{
                //    ReturnData.ResponseCode = -101;
                //    ReturnData.Description = "Bạn không có quyền sử dụng chức năng này";
                //    return Json(ReturnData);
                //}
                if (listUserFunction == null || listUserFunction.Count == 0)
                {
                    ReturnData.ResponseCode = -7001;
                    ReturnData.Description = "Bạn chưa chọn quyền cho user";
                    return Json(ReturnData);
                }
                string ListRole = string.Empty;
                foreach (var userfunction in listUserFunction)
                {
                    ListRole += userfunction.FunctionID + "," + userfunction.IsInsert + "," + userfunction.IsUpdate + "," + userfunction.IsDelete + "," + userfunction.IsFullControl + "," + userfunction.FunctionCode + ";";
                }
                var Result = _userroleservice.GroupFunctionInsertList(listUserFunction[0].UserID, ListRole);
                if (Result >= 0)
                {
                    ReturnData.Description = "Phân quyền Thành Công";
                    //Ghi log
                    _userlogservice.InsertUsersLog(new UsersLog
                    {
                        FunctionCode = ViewBag.FunctionCode,
                        Description = "Phân quyền Group =  " + listUserFunction[0].UserID,
                        UserID = CurrentUser.UserID,
                        UserName = CurrentUser.Username,
                        ClientIP = Config.GetIP()
                    });
                }
                else if (Result == -50)
                    ReturnData.Description = "Tài khoản không tồn tại";
                else if (Result == -600)
                    ReturnData.Description = "Tham số truyền vào không hợp lệ";
                else
                {
                    ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau";
                }
                ReturnData.ResponseCode = Result;

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

        #endregion
        #region Quản trị chức năng hệ thống
        [PermissionFilter(FunctionCode = FunctionCode.Function)]
        public ActionResult ManagerFunction()
        {

            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.Function)]
        public ActionResult ListFunction(string functionName, int? isActive, int? currentPage, int? PageSize)
        {
            var data = new List<Functions>();
            string FunctionName = string.IsNullOrEmpty(functionName) ? string.Empty : functionName;
            int TotalRecord = 0;
            int IsActive = isActive ?? -1;
            int CurrentPage = currentPage ?? 1;
            int pageSize = PageSize ?? 1000;
            data = _functionservice.GetListFunctions(FunctionName, IsActive, CurrentPage, pageSize, ref TotalRecord);
            ViewBag.TotalRecord = TotalRecord;
            ViewBag.CurrentPage = CurrentPage;
            ViewBag.PageSize = pageSize;
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Function, FunctionType = (int)Enums.FunctionType.IsInsert)]
        public ActionResult AddFunction()
        {
            var result = new ModelFunctionDetail();
            result.FunctionDetail = new Functions();

            result.ListFunction = _functionservice.GetListFunctionBySystemID(0);

            ViewBag.Title = "Thêm mới chức năng";
            return View("GetFunctionInfo", result);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Function, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult GetFunctionInfo(int? id)
        {
            var result = new ModelFunctionDetail();
            result.FunctionDetail = new Functions();

            int Id = id == null ? 0 : (int)id;
            result.ListFunction = _functionservice.GetListFunctionBySystemID(0);
            result.FunctionDetail = _functionservice.GetFunctionByFunctionID(Id);
            if (result.FunctionDetail == null)
                result.FunctionDetail = new Functions();
            ViewBag.Title = "Cập nhập chức năng";
            return View(result);
        }

        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Function, FunctionType = (int)Enums.FunctionType.IsInsert)]
        [ValidateAntiForgeryToken]
        public JsonResult SaveDataFunction(Functions function)
        {
            var ReturnData = new ReturnData();

            //if (Permission == null || (Permission.FunctionID != (int)Enums.FunctionId.Function) || (function.FunctionID == 0 && !Permission.IsInsert) || function.FunctionID > 0 && !Permission.IsUpdate)
            //{
            //    ReturnData.ResponseCode = -101;
            //    ReturnData.Description = "Bạn không có quyền sử dụng chức năng này";
            //    return Json(ReturnData);
            //}
            //if (string.IsNullOrEmpty(function.FunctionName))
            //{
            //    ReturnData.ResponseCode = -100;
            //    ReturnData.Description = "Bạn chưa nhập tên chức năng";
            //    return Json(ReturnData);
            //}
            function.UrlDisplay = string.IsNullOrEmpty(function.UrlDisplay) ? string.Empty : function.UrlDisplay;
            function.Url = string.IsNullOrEmpty(function.Url) ? string.Empty : function.Url;
            try
            {
                var result = _functionservice.InsertUpdateFunction(function);
                ReturnData.ResponseCode = result;
                if (result >= 0)
                {
                    string Description = "";
                    if (function.FunctionID > 0)
                    {
                        ReturnData.Description = "Cập nhật Thành Công";
                        Description = ReturnData.Description + " chức năng FunctionCode = " + function.FunctionCode + " , FunctionName = " + function.FunctionName;
                    }
                    else
                    {
                        ReturnData.Description = "Thêm mới Thành Công";
                        Description = ReturnData.Description + " chức năng FunctionCode = " + function.FunctionCode + " , FunctionName = " + function.FunctionName;
                    }
                    //Ghi log

                    _userlogservice.InsertUsersLog(new UsersLog
                    {
                        FunctionCode = ViewBag.FunctionCode,
                        Description = Description,
                        UserID = CurrentUser.UserID,
                        UserName = CurrentUser.Username,
                        ClientIP = Config.GetIP()
                    });
                }
                else switch (result)
                    {
                        case -70: ReturnData.Description = "Tên chức năng đã tồn tại"; break;
                        case -71: ReturnData.Description = "Đường dẫn đã tồn tại"; break;
                        case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
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

        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Function, FunctionType = (int)Enums.FunctionType.IsDelete)]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteFunction(int id)
        {
            var ReturnData = new ReturnData();
            try
            {

                //if (Permission == null || !Permission.IsDelete || (Permission.FunctionID != (int)Enums.FunctionId.Function))
                //{
                //    ReturnData.ResponseCode = -101;
                //    ReturnData.Description = "Bạn không có quyền sử dụng chức năng này";
                //    return Json(ReturnData);
                //}
                if (id > 0)
                {
                    var Function = _functionservice.GetFunctionByFunctionID(id);
                    if (Function == null)
                    {
                        ReturnData.ResponseCode = -102;
                        ReturnData.Description = "chức năng xóa không tồn tại !";
                        return Json(ReturnData);
                    }
                    var result = _functionservice.DelleteFunction(id);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        ReturnData.Description = "Xóa chức năng Thành Công";
                        //Ghi log
                        _userlogservice.InsertUsersLog(new UsersLog
                        {
                            FunctionCode = ViewBag.FunctionCode,
                            Description = "Xóa chức năng " + " FunctionCode = " + Function.FunctionCode + ", FunctionName = " + Function.FunctionName,
                            UserID = CurrentUser.UserID,
                            UserName = CurrentUser.Username,
                            ClientIP = Config.GetIP()
                        });
                    }
                    else switch (result)
                        {
                            case -24: ReturnData.Description = "Chức năng đang trong trạng thái hoạt động. Hãy tắt trạng thái hoạt động của hệ thống trước khi xóa !"; break;
                            case -25: ReturnData.Description = "Chức năng không tồn tại trong hệ thống"; break;
                            case -99: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                            default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                        }
                    return Json(ReturnData);
                }
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "Không xác định chức năng cần xóa";
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

        public ActionResult FunctionOrder()
        {

            var data = _functionservice.SelectAllFunctionID(-1, string.Empty, -1, -1);

            return PartialView(data);
        }

        public static string GetChildFunction(int FatherID, List<Functions> ListFunction)
        {
            var function = ListFunction.Find(f => f.FunctionID == FatherID);
            var listChirl = ListFunction.FindAll(f => f.FatherID == FatherID);
            listChirl.Sort((f1, f2) => f1.Order.CompareTo(f2.Order));

            var script = "<li class=\"dd-item\" data-id=\"" + FatherID + "\"><div class=\"dd-handle\"><i class=\"" + "fa fa-fw mid_" + function.IconId + "\" style=\"margin-right:7px\"></i>" + function.FunctionName + "</div>";

            if (listChirl.Count <= 0)
            {
                script += "</li>";
                return script;
            }
            script += "<ol class=\"dd-list\">";
            foreach (var t in listChirl)
            {
                script += GetChildFunction(t.FunctionID, ListFunction);
            }
            script += "</ol>";
            script += "</li>";
            return script;
        }

        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Function, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        [ValidateAntiForgeryToken]
        public JsonResult SaveOrderFunction(List<FunctionOrder> listOrder)
        {
            var ReturnData = new ReturnData();

            if (listOrder.Count <= 0)
            {
                ReturnData.ResponseCode = -100;
                ReturnData.Description = "không tồn tại danh sách chức năng sắp xếp mới";
                return Json(ReturnData);
            }

            try
            {
                foreach (var t in listOrder)
                {
                    _functionservice.UpdateOrder(t.Id, t.FatherID, t.Order);
                }

                ReturnData.ResponseCode = 1;
                ReturnData.Description = "Sắp xếp Thành Công Chức Năng";
                //Ghi log

                //_userlogservice.InsertUsersLog(new UsersLog
                //{
                //    FunctionCode = ViewBag.FunctionCode,
                //    Description = Description,
                //    UserID = CurrentUser.UserID,
                //    ClientIP = Config.GetIP()
                //});
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                ReturnData.ResponseCode = -99;
                ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau";
            }
            return Json(ReturnData);
        }
        #endregion

        #region Quản trị người dùng
        [PermissionFilter(FunctionCode = FunctionCode.Users)]
        public ActionResult ManagerUser()
        {
            var lstUser = new List<Users>();
            if (CurrentFullUser == null)
            {
                var m_Users = _userservice.GetByUsername(CurrentUser.Username);
                Session[SessionsManager.SESSION_USER_FULL] = m_Users;
                CurrentFullUser = m_Users;
            }

            if (CurrentUser.Type ==1)
            {
                lstUser = _userservice.GetAll().Where(x => x.Type ==2).ToList();
                //lstUser.Insert(0, new Users { UserID = CurrentFullUser.UserID, Username = CurrentFullUser.Username });
            }
            //else
            //{
            //    if (CurrentUser.Type == 4)
            //    {
            //        lstUser = lstUser.Where(x => x.CreatedUser == CurrentUser.Username).ToList();
            //    }

            //}
            lstUser.Insert(0, new Users { UserID = 0, Username = "--Người tạo--" });
            ViewBag.UserList = lstUser;

            var lstGroup = _groupservice.GetList().Where(x => x.Type == 2).ToList();
            if (CurrentFullUser.Type > 2)
            {
                lstGroup = lstGroup.Where(x => x.Alias == "2").ToList();
            }
            else
            {
                lstGroup = lstGroup.Where(x => x.Alias == "1").ToList();
            }
            lstGroup.Insert(0, new Groups { GroupID = -1, Name = "--Nhóm--" });

            ViewBag.GroupList = lstGroup;

            ViewBag.CurrentUser = CurrentFullUser;
            return View();
        }
        [PermissionFilter(FunctionCode = FunctionCode.Users)]
        public ActionResult ListUsers(int? isActive, string email, int? currentPage, int? pageSize, string createdUser, int? group)
        {
            var lstGroup = _groupservice.GetList();
            ViewBag.GroupList = lstGroup.Where(x => x.Type == 2).ToList();
            var data = new List<Users>();

            string Email = string.IsNullOrEmpty(email) ? string.Empty : email;
            int IsActive = isActive == null ? -1 : (int)isActive;
            int Group = group == null ? -1 : (int)group;
            int TotalRecord = 0;
            int CurrPage = currentPage == null ? 1 : (int)currentPage;
            int RecordPerPage = pageSize == null ? 10000 : (int)pageSize;


            if (createdUser == "--Người tạo--")
            {
                createdUser = "";

            }
            if (string.IsNullOrEmpty(createdUser))
            {
                if (!ViewBag.IsAdmin)
                {

                    createdUser = CurrentUser.Username;
                   
                }
                else
                {
                    createdUser = "";
                }
            }

            ViewBag.UserName = CurrentUser.Username;
            ViewBag.Type = CurrentUser.Type;
            data = _userservice.GetListUsers(Email, createdUser, IsActive, -1, CurrPage, RecordPerPage, ref TotalRecord);

            if (data.Count > 0)
                ViewBag.TotalRecord = TotalRecord;
            else
                ViewBag.TotalRecord = 0;
            ViewBag.PageSize = RecordPerPage;
            ViewBag.CurrentPage = CurrPage;

            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Users)]
        public ActionResult GetUserBalance()
        {

            var data = new List<Users>();


            int TotalRecord = 0;
            int CurrPage = 1;
            int RecordPerPage = 10000;
            var createdUser = "";

            if (!ViewBag.IsAdmin)
            {

                createdUser = CurrentUser.Username;
                if (CurrentFullUser.Type == 2)
                {
                    createdUser = CurrentFullUser.UserAPI;
                }
            }
            else
            {
                createdUser = "";
            }
            //ViewBag.UserName = CurrentUser.Username;
            data = _userservice.GetListUsers("", createdUser, -1, -1, CurrPage, RecordPerPage, ref TotalRecord);
            data = data.Where(x => x.Username != CurrentUser.Username && x.UserID != 1).ToList();
            ViewBag.Total = data.Sum(x => long.Parse(x.Balance.ToString())).ToString();
            ViewBag.TotalHold = data.Sum(x => long.Parse(x.BalanceHold.ToString())).ToString();

            return PartialView();
        }
        [PermissionFilter(FunctionCode = FunctionCode.Users)]
        public ActionResult AddUserInfo()
        {
            var lstGroup = _groupservice.GetList();
            if (CurrentFullUser.Type > 2)
            {
                lstGroup = lstGroup.Where(x => x.Alias == "2").ToList();
            }
            else
            {
                lstGroup = lstGroup.Where(x => x.Alias == "1").ToList();
            }
            ViewBag.GroupList = lstGroup.Where(x => x.Type == 2).ToList();
            var User = new Users
            {
                MaxDay = 0,
                Status = true,
                //StatusVMS = true,
                //StatusVNP = true,
                //StatusVTT = true,
                PercentVMS = 16,
                PercentVNP = 16,
                PercentVTT = 16,
                PercentGarena=16,
                Piority = 2,
                Type = 2,//tk cấp 1
                NumberUser = 100,
                StatusOrder = true,

            };
            User.PercentVNMNH = 16;
            User.PercentVMS = 16;
            User.PercentVMSMY = 16;
            User.PercentVMSNH = 16;
            User.PercentVNP = 16;
            User.PercentVNPMY = 16;
            User.PercentVNPTT = 16;
            User.PercentVNPTS = 16;
            User.PercentVTT = 16;
            User.PercentVTTMY = 16;
            User.PercentVTTTS = 16;
            User.PercentVTTTT = 16;
            if (CurrentUser.Type == 2)
            {
                User.Type = 3;//tk cấp 2
                User.PercentVTT = 2;
                User.PercentVTTTT = 2;
            }
            //if (CurrentUser.Type == 3)
            //{
            //    User.Type = 4;//tk cấp 2
            //    User.StatusOrder = true;
            //}
            //if (CurrentUser.Type == 4)
            //{
            //    User.Type = 5;//tk cấp 3
            //    User.StatusOrder = true;
            //}
            //if (User.Type > 3)
            //{
            //    User.PercentVMS = 0;
            //    User.PercentVTT = 0;
            //    User.PercentVNP = 0;
            //    User.PercentZing = 0;
            //    User.PercentGarena = 0;
            //    User.PercentVTC = 0;
            //}
            ViewBag.CurrentUser = CurrentFullUser;
            ViewBag.Title = "Thêm mới user";

            return View("GetUserInfo", User);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Users)]
        public ActionResult GetUserInfo(int? id)
        {
            int UserId = id == null ? 0 : (int)id;
            var User = new Users();

            if (UserId > 0)
            {
                User = _userservice.SelectByUserID(UserId);
            }
            if (!ViewBag.IsAdmin)
            {
                if (UserId <= 0)
                {
                    User.PercentVTT = 2;
                    User.PercentVTTTT = 2;
                }
               

            }
            else
            {
                if (UserId <= 0)
                {
                    User.PercentVNMNH = 16;
                    User.PercentVMS = 16;
                    User.PercentGarena = 16;
                    User.PercentVMSMY = 16;
                    User.PercentVNMNH = 16;
                    User.PercentVNP = 16;
                    User.PercentVNPMY = 16;
                    User.PercentVNPTT = 16;
                    User.PercentVNPTS = 16;
                    User.PercentVTT = 16;
                    User.PercentVTTMY = 16;
                    User.PercentVTTTS = 16;
                    User.PercentVTTTT = 16;
                }


            }
            var lstGroup = _groupservice.GetList();
            if (!ViewBag.IsAdmin)
            {
                //if(CurrentUser.Type==3)
                //{
                //    if (User.CreatedUser != CurrentUser.Username)
                //        return RedirectToAction("ManagerUser");
                //}
                //else
                //{
                //    if (User.CreatedUser != CurrentUser.Username)
                //    {
                //        var parrentUser = _userservice.GetByUsername(User.CreatedUser);
                //        if (parrentUser.CreatedUser != CurrentUser.Username )
                //            return RedirectToAction("ManagerUser");
                //    }


                //}
                if (User.CreatedUser != CurrentUser.Username)
                    return RedirectToAction("ManagerUser");
                if (CurrentFullUser.Type > 2)
                {
                    lstGroup = lstGroup.Where(x => x.Alias == "2").ToList();
                }
                else
                {
                    lstGroup = lstGroup.Where(x => x.Alias == "1").ToList();
                }
            }



            ViewBag.GroupList = lstGroup.Where(x => x.Type == 2).ToList();

            ViewBag.Title = "Cập nhập user";

            ViewBag.CurrentUser = CurrentFullUser;
            return View(User);
        }

        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsFullControl)]
        public ActionResult GetGrantUser(int userid)
        {

            var functionModel = new UserFunctionModel();
            var userfunction = _userroleservice.UserFunction_GetByUserID(userid);
            var functionlist = _functionservice.SelectAllFunctionID(-1, string.Empty, 1, -1);
            functionModel.ListFunction = functionlist;
            functionModel.UserFunction = userfunction;
            ViewBag.UserId = userid;
            ViewBag.Title = "Thông Tin Phân Quyền";
            return View(functionModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilter(FunctionCode = FunctionCode.Users)]
        public JsonResult SaveDataUser(Users users)
        {
            var ReturnData = new ReturnData();

            try
            {

                //if (Permission == null || Permission.FunctionID != (int)Enums.FunctionId.User || (users.UserID == 0 && !Permission.IsInsert) || users.UserID > 0 && !Permission.IsUpdate)
                //{
                //    ReturnData.ResponseCode = -101;
                //    ReturnData.Description = "Bạn không có quyền sử dụng chức năng này";
                //    return Json(ReturnData);
                //}
                if (string.IsNullOrEmpty(users.Username))
                {
                    ReturnData.ResponseCode = -6001;
                    ReturnData.Description = "Bạn chưa nhập tên tài khoản";
                    return Json(ReturnData);
                }
                users.Config = users.Config.Replace("_", "");
                users.Config = StringUtils.FirstCharToUpper(users.Config);
                users.CreatedUser = CurrentUser.Username;
                users.C1User = CurrentUser.Username;
                //if (string.IsNullOrEmpty(users.Email) || !users.Email.Contains("@"))
                //{
                //    ReturnData.ResponseCode = -6002;
                //    ReturnData.Description = "Email chưa nhập hoặc sai định dạng";
                //    return Json(ReturnData);
                //}
                //if (string.IsNullOrEmpty(users.Password))
                //{
                //    ReturnData.ResponseCode = -6003;
                //    ReturnData.Description = "Password chưa nhập hoặc sai định dạng";
                //    return Json(ReturnData);
                //}
                users.FullName = string.IsNullOrEmpty(users.FullName) ? string.Empty : users.FullName;
                //var userinfo = (UserSession)Session[SessionsManager.SESSION_USER];
                if (CurrentUser.Type == 2)
                {
                    users.Type = 3;//tk cấp 1
                }
                if (CurrentUser.Type == 3)
                {
                    users.Type = 4;//tk cấp 2
                }
                if (CurrentUser.Type == 4)
                {
                    users.Type = 5;//tk cấp 3
                }
                if (users.UserID == 0)
                {
                    if (string.IsNullOrEmpty(users.Password))
                    {
                        users.Password = Encrypt.MD5(Config.GetAppsetting("DefaultPassword") + Config.GetAppsetting("Salt"));
                    }
                    else
                    {
                        users.Password = Encrypt.MD5(users.Password + Config.GetAppsetting("Salt"));
                    }
                    if (string.IsNullOrEmpty(users.Password2))
                    {
                        users.Password2 = Encrypt.MD5(Config.GetAppsetting("DefaultPassword") + Config.GetAppsetting("Salt"));
                    }
                    else
                    {
                        users.Password2 = Encrypt.MD5(users.Password2 + Config.GetAppsetting("Salt"));
                    }
                    users.UserAPI = Encrypt.MD5(users.UserID + "buycard");

                    //if (users.Type >= 3)
                    //{
                    //    var currentuser = _userservice.SelectByUserID(CurrentUser.UserID);
                    //    users.UserAPI = currentuser.UserAPI;
                    //    users.PasswordAPI = currentuser.PasswordAPI;

                    //    if (users.Type >= 5)
                    //    {
                    //        users.C1User = currentuser.CreatedUser;
                    //    }
                    //}
                }
                else
                {
                    //if (users.Type == 2)
                    //{

                    //    var currentuser = _userservice.SelectByUserID(users.UserID);
                    //    //đổi mật khẩu api
                    //    if (!currentuser.UserAPI.Equals(users.UserAPI) || !currentuser.PasswordAPI.Equals(users.PasswordAPI))
                    //    {
                    //        //NLogLogger.DebugMessage("update pass api");
                    //        var where = " [UserID] =" + currentuser.UserID + "OR CreatedUser='" + currentuser.Username + "'";
                    //        var update = "UserAPI= '" + users.UserAPI + "', PasswordAPI='" + users.PasswordAPI + "'";
                    //        var resultupdate = _userservice.UpdateUserDynamic(where, update);
                    //        //đổi mk cho cả thằng cấp 3
                    //        var lstuser = _userservice.GetAll().Where(x => x.CreatedUser == currentuser.Username);
                    //        if (lstuser != null)
                    //        {
                    //            foreach (var item in lstuser)
                    //            {
                    //                where = " CreatedUser='" + item.Username + "'";
                    //                update = "UserAPI= '" + users.UserAPI + "', PasswordAPI='" + users.PasswordAPI + "'";
                    //                _userservice.UpdateUserDynamic(where, update);
                    //            }
                    //        }
                    //    }
                    //}
                }


                var result = _userservice.UpdateUsers(users);
                ReturnData.ResponseCode = result;
                if (result >= 0)
                {
                    if (users.UserID > 0)
                        ReturnData.Description = "Cập nhật Thành Công";
                    else
                        ReturnData.Description = "Thêm mới Thành Công";

                    //Ghi log
                    _userlogservice.InsertUsersLog(new UsersLog
                    {
                        FunctionCode = ViewBag.FunctionCode,
                        Description = ReturnData.Description + " UserID =  " + result,
                        UserID = CurrentUser.UserID,
                        UserName = CurrentUser.Username,
                        ClientIP = Config.GetIP()
                    });
                }
                else switch (result)
                    {
                        case -51: ReturnData.Description = "Tài khoản đã tồn tại"; break;
                        case -52: ReturnData.Description = "Email đã tồn tại"; break;
                        case -90: ReturnData.Description = "Số tài khoản vượt quá giới hạn"; break;
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

        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsFullControl)]
        [HttpPost]

        public JsonResult SaveGrantUser(List<UserFunction> listUserFunction)
        {
            var ReturnData = new ReturnData();

            try
            {

                //if (Permission == null || Permission.FunctionID != (int)Enums.FunctionId.GrantUser || (!Permission.IsInsert || !Permission.IsUpdate || !Permission.IsDelete))
                //{
                //    ReturnData.ResponseCode = -101;
                //    ReturnData.Description = "Bạn không có quyền sử dụng chức năng này";
                //    return Json(ReturnData);
                //}
                if (listUserFunction == null || listUserFunction.Count == 0)
                {
                    ReturnData.ResponseCode = -7001;
                    ReturnData.Description = "Bạn chưa chọn quyền cho user";
                    return Json(ReturnData);
                }
                string ListRole = string.Empty;
                foreach (var userfunction in listUserFunction)
                {
                    ListRole += userfunction.FunctionID + "," + userfunction.IsInsert + "," + userfunction.IsUpdate + "," + userfunction.IsDelete + "," + userfunction.IsFullControl + "," + userfunction.FunctionCode + ";";
                }
                var Result = _userroleservice.UserFunctionInsertList(listUserFunction[0].UserID, ListRole);
                if (Result >= 0)
                {
                    ReturnData.Description = "Phân quyền Thành Công";
                    //Ghi log
                    _userlogservice.InsertUsersLog(new UsersLog
                    {
                        FunctionCode = ViewBag.FunctionCode,
                        Description = "Phân quyền UserID =  " + listUserFunction[0].UserID,
                        UserID = CurrentUser.UserID,
                        UserName = CurrentUser.Username,
                        ClientIP = Config.GetIP()
                    });
                }
                else if (Result == -50)
                    ReturnData.Description = "Tài khoản không tồn tại";
                else if (Result == -600)
                    ReturnData.Description = "Tham số truyền vào không hợp lệ";
                else
                {
                    ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau";
                }
                ReturnData.ResponseCode = Result;

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
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult UpdateStatusCK(string id, string function, int status)
        {
            var ReturnData = new ReturnData();
            try
            {
                int Id = int.Parse(Encrypt.Base64Decode(id));

                if (Id >= 1)
                {
                    var where = " [UserID] = " + Id;
                    var update = function + "=" + status;
                    var result = _userservice.UpdateUserDynamic(where, update);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        //Ghi log
                        _userlogservice.InsertUsersLog(new UsersLog
                        {
                            FunctionCode = ViewBag.FunctionCode,
                            Description = "Kích hoạt " + function + " User ID:  " + Id,
                            UserID = CurrentUser.UserID,
                            UserName = CurrentUser.Username,
                            ClientIP = Config.GetIP()
                        });
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
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult UpdateStatusCKMulti(List<string> arrId, string Telco, int status)
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

                    var where = " [UserID] IN(" + joinId + ")";
                    var update = "Status" + Telco + "=" + status;
                    var result = _userservice.UpdateUserDynamic(where, update);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        //Ghi log

                        if (status == 0)
                        {
                            _userlogservice.InsertUsersLog(new UsersLog
                            {
                                FunctionCode = ViewBag.FunctionCode,
                                Description = "Khóa nạp " + Telco + " User ID:  " + joinId,
                                UserID = CurrentUser.UserID,
                                UserName = CurrentUser.Username,
                                ClientIP = Config.GetIP()
                            });
                        }
                        else
                        {
                            _userlogservice.InsertUsersLog(new UsersLog
                            {
                                FunctionCode = ViewBag.FunctionCode,
                                Description = "Mở khóa nạp " + Telco + " User ID:  " + joinId,
                                UserID = CurrentUser.UserID,
                                UserName = CurrentUser.Username,
                                ClientIP = Config.GetIP()
                            });
                        }
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
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult UpdateCKMulti(List<string> arrId, string Telco, int status)
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

                    var where = " [UserID] IN(" + joinId + ")";
                    var update = "Percent" + Telco + "=" + status;
                    var result = _userservice.UpdateUserDynamic(where, update);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        //Ghi log
                        _userlogservice.InsertUsersLog(new UsersLog
                        {
                            FunctionCode = ViewBag.FunctionCode,
                            Description = "Cập nhật chiết khấu " + Telco + " User ID:  " + joinId,
                            UserID = CurrentUser.UserID,
                            UserName = CurrentUser.Username,
                            ClientIP = Config.GetIP()
                        });
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
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteUser(string id)
        {
            var ReturnData = new ReturnData();
            try
            {

                //if (Permission == null || Permission.FunctionID != (int)Enums.FunctionId.User || !Permission.IsDelete)
                //{
                //    ReturnData.ResponseCode = -101;
                //    ReturnData.Description = "Bạn không có quyền sử dụng chức năng này";
                //    return Json(ReturnData);
                //}
                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 0)
                {
                    var result = _userservice.DeleteUsers(Id);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        //Ghi log
                        _userlogservice.InsertUsersLog(new UsersLog
                        {
                            FunctionCode = ViewBag.FunctionCode,
                            Description = "Xóa User ID:  " + Id,
                            UserID = CurrentUser.UserID,
                            UserName = CurrentUser.Username,
                            ClientIP = Config.GetIP()
                        });
                        ReturnData.Description = "Xóa user Thành Công";
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult UpdateActiveUser(string id)
        {
            var ReturnData = new ReturnData();
            try
            {

                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 1)
                {
                    var result = _userservice.UpdateActiveUser(Id);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        //Ghi log
                        _userlogservice.InsertUsersLog(new UsersLog
                        {
                            FunctionCode = ViewBag.FunctionCode,
                            Description = "Cập nhật trạng thái User ID:  " + Id,
                            UserID = CurrentUser.UserID,
                            UserName = CurrentUser.Username,
                            ClientIP = Config.GetIP()
                        });
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
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult ResetPassword(string id)
        {
            var ReturnData = new ReturnData();
            try
            {

                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 1)
                {
                    var user = _userservice.SelectByUserID(Id);
                    if (!ViewBag.IsAdmin)
                    {
                        if (user.CreatedUser != CurrentUser.Username)
                        {
                            ReturnData.ResponseCode = -101;
                            ReturnData.Description = "Không có quyền thực hiện chức năng này";
                            return Json(ReturnData);
                        }
                    }
                    var result = _userservice.ResetPassword(user.UserID, user.Username, Encrypt.MD5(Config.GetAppsetting("DefaultPassword") + Config.GetAppsetting("Salt")));
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        //Ghi log
                        _userlogservice.InsertUsersLog(new UsersLog
                        {
                            FunctionCode = ViewBag.FunctionCode,
                            Description = "Reset mật khẩu User :  " + user.Username,
                            UserID = CurrentUser.UserID,
                            UserName = CurrentUser.Username,
                            ClientIP = Config.GetIP()
                        });
                        ReturnData.Description = "Reset mật khẩu Thành Công";
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
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult ResetPassword2(string id)
        {
            var ReturnData = new ReturnData();
            try
            {

                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 1)
                {
                    var user = _userservice.SelectByUserID(Id);
                    if (!ViewBag.IsAdmin)
                    {
                        if (user.CreatedUser != CurrentUser.Username)
                        {
                            ReturnData.ResponseCode = -101;
                            ReturnData.Description = "Không có quyền thực hiện chức năn này";
                            return Json(ReturnData);
                        }
                    }
                    var result = _userservice.ResetPassword2(user.UserID, user.Username, Encrypt.MD5(Config.GetAppsetting("DefaultPassword") + Config.GetAppsetting("Salt")));
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {

                        //Ghi log
                        _userlogservice.InsertUsersLog(new UsersLog
                        {
                            FunctionCode = ViewBag.FunctionCode,
                            Description = "Reset mật khẩu bán hàng User :  " + user.Username,
                            UserID = CurrentUser.UserID,
                            UserName = CurrentUser.Username,
                            ClientIP = Config.GetIP()
                        });
                        ReturnData.Description = "Reset mật khẩu Thành Công";
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

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ChangePass(string PasswordOld, string PasswordNew)
        {
            var ReturnData = new ReturnData();

            try
            {

                if (string.IsNullOrEmpty(PasswordOld) || string.IsNullOrEmpty(PasswordNew))
                {
                    ReturnData.ResponseCode = -7002;
                    ReturnData.Description = "Mật khẩu cũ và mật khẩu mới không được phép bỏ trống";
                    return Json(ReturnData);
                }
                if (string.Compare(PasswordOld, PasswordNew) == 0)
                {
                    ReturnData.ResponseCode = -7003;
                    ReturnData.Description = "Mật khẩu cũ và mật khẩu mới không được phép trùng nhau";
                    return Json(ReturnData);
                }
                PasswordOld = Encrypt.MD5(PasswordOld + Config.GetAppsetting("Salt"));
                PasswordNew = Encrypt.MD5(PasswordNew + Config.GetAppsetting("Salt"));
                var Result = _userservice.ChangePassword(CurrentUser.Username, PasswordOld, PasswordNew);
                ReturnData.ResponseCode = Result;
                if (Result >= 0)
                {
                    ReturnData.Description = "Đổi mật khẩu thành công";
                }
                else switch (Result)
                    {
                        case -1: ReturnData.Description = "Tài khoản không tồn tại"; break;
                        case -2: ReturnData.Description = "Tài khoản bị block"; break;
                        case -3: ReturnData.Description = "Mật khẩu cũ không đúng"; break;
                        case -600: ReturnData.Description = "Tham số truyền vào không hợp lệ"; break;
                        default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                    }

                ReturnData.ResponseCode = Result;

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
        public ActionResult ChangePassword2()
        {
            return View();
        }

        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult TopupUser(string id)
        {
            ViewBag.id = id;
            int Id = int.Parse(Encrypt.Base64Decode(id));
            if (Id > 1)
            {
                var user = _userservice.SelectByUserID(Id);
                ViewBag.UserName = user.Username;
                //if (user.CreatedUser != CurrentUser.Username &&)
                //{

                //    return new EmptyResult();
                //}
            }

            return PartialView();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult Topup(string id, int amount, string note)
        {
            var ReturnData = new ReturnData();
            try
            {

                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 1)
                {
                    var result = _userservice.Topup(Id, CurrentUser.Username, amount, note);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        //Session[SessionsManager.SESSION_USER_FULL] = null;
                        Session[SessionsManager.SESSION_USER_FULL] = _userservice.GetByUsername(CurrentUser.Username);
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
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult DeductUser(string id)
        {
            ViewBag.id = id;
            int Id = int.Parse(Encrypt.Base64Decode(id));
            if (Id > 1)
            {
                var user = _userservice.SelectByUserID(Id);
                ViewBag.UserName = user.Username;
                if (user.CreatedUser != CurrentUser.Username)
                {

                    return new EmptyResult();
                }
            }

            return PartialView();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult Deduct(string id, int amount, string note)
        {
            var ReturnData = new ReturnData();
            try
            {

                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 1)
                {

                    var result = _userservice.Deduct(Id, CurrentUser.Username, amount, note);
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        Session[SessionsManager.SESSION_USER_FULL] = _userservice.GetByUsername(CurrentUser.Username);
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
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult HistoryUser(string id)
        {
            ViewBag.id = id;
            int Id = int.Parse(Encrypt.Base64Decode(id));
            if (Id > 1)
            {
                var user = _userservice.SelectByUserID(Id);
                ViewBag.UserName = user.Username;

            }
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            return PartialView();
        }
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult HistoryHoldUser(string id)
        {
            ViewBag.id = id;
            int Id = int.Parse(Encrypt.Base64Decode(id));
            if (Id > 1)
            {
                var user = _userservice.SelectByUserID(Id);
                ViewBag.UserName = user.Username;

            }

            return PartialView();
        }
        public ActionResult MyHistory()
        {
            if (CurrentUser == null)
                return RedirectToAction("Login", "Account");
            ViewBag.UserName = CurrentUser.Username;
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = DateTime.Now;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            return View();
        }
        public ActionResult MyHistoryHold()
        {
            if (CurrentUser == null)
                return RedirectToAction("Login", "Account");
            ViewBag.UserName = CurrentUser.Username;

            return View();
        }
        public ActionResult ListTransaction(string username, int? currentPage, int? PageSize, int? ActionType, string keyword, string FromDate, string ToDate)
        {
            var usser = _userservice.GetByUsername(username);

            if (CurrentUser.Username == username || CurrentUser.Type <= 2 || usser.CreatedUser == CurrentUser.Username )
            {
                int TotalRecord = 0;
                long TotalDeduct = 0;
                long TotalTopup = 0;
                int CurrentPage = currentPage ?? 1;
                int pageSize = PageSize ?? 15;
                int actionType = ActionType ?? -1;
                var data = _transervice.GetList(username, -1, CurrentPage, pageSize, ref TotalRecord, ref TotalDeduct, ref TotalTopup, actionType, "", keyword, FromDate, ToDate);

                ViewBag.TotalRecord = TotalRecord;
                ViewBag.TotalDeduct = TotalDeduct;
                ViewBag.TotalTopup = TotalTopup;
                ViewBag.CurrentPage = CurrentPage;
                ViewBag.username = username;
                ViewBag.PageSize = pageSize;
                return PartialView(data);
            }

            return PartialView(null);
        }
        public ActionResult ReportTransaction(string username, int? ActionType, string keyword, string FromDate, string ToDate)
        {
            var usser = _userservice.GetByUsername(username);

            if (CurrentUser.Username == username || CurrentUser.Type <= 2 || usser.CreatedUser == CurrentUser.Username | usser.C1User == CurrentUser.Username)
            {
                int TotalRecord = 0;
                long TotalRecord1 = 0;
                long TotalRecord2 = 0;
                int actionType = ActionType ?? -1;
                var data = _transervice.GetList(username, -1, 1, 10000, ref TotalRecord, ref TotalRecord1, ref TotalRecord2, actionType, "", keyword, FromDate, ToDate);

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
                            worksheet.Column(3).Width = 75;
                            worksheet.Column(4).Width = 25;
                            worksheet.Column(5).Width = 25;
                            worksheet.Column(6).Width = 25;
                            worksheet.Column(7).Width = 25;
                            worksheet.Column(8).Width = 25;
                            var properties = new[]
                            {

                            "Số tiền",
                            "Số dư sau giao dịch",
                            "Mô tả",
                            "Thời gian",


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
                                worksheet.Cells[row, col].Value = StringUtils.RemoveAllHtmlTags(StringUtils.InsertCommaMark(item.Amount.ToString(), item.Type));
                                col++;

                                worksheet.Cells[row, col].Value = StringUtils.InsertCommaNoStyle(item.Balance.ToString()) + "(" + StringUtils.InsertCommaNoStyle(item.BalanceHold.ToString()) + ")";
                                col++;

                                worksheet.Cells[row, col].Value = item.Description;
                                col++;

                                //worksheet.Cells[row, col].Value = item.TotalVNP;
                                //col++;

                                //worksheet.Cells[row, col].Value = item.TotalVMS;
                                //col++;

                                //worksheet.Cells[row, col].Value = item.TotalZing;
                                //col++;

                                //worksheet.Cells[row, col].Value = item.TotalGarena;
                                //col++;

                                worksheet.Cells[row, col].Value = string.Format("{0:dd/MM/yyyy HH:mm:ss}", item.Time);
                                row++;
                            }
                            worksheet.Row(row).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            worksheet.Row(row).Style.Font.Size = 14;
                            worksheet.Row(row).Style.Font.Name = "Times News Roman";
                            worksheet.Row(row).Style.Font.Color.SetColor(Color.FromArgb(0, 0, 117));





                            xlPackage.Save();
                        }
                        bytes = stream.ToArray();
                    }
                    return File(bytes, "text/xls", "History" + usser.Username + ".xlsx");
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }


            }

            return RedirectToAction("Index", "Home");
        }



        public ActionResult ListTransactionHold(string username, int? currentPage, int? PageSize)
        {

            int TotalRecord = 0;
            int CurrentPage = currentPage ?? 1;
            int pageSize = PageSize ?? 15;
            var data = _transervice.GetListHold(username, -1, CurrentPage, pageSize, ref TotalRecord);
            ViewBag.TotalRecord = TotalRecord;
            ViewBag.CurrentPage = CurrentPage;
            ViewBag.username = username;
            ViewBag.PageSize = pageSize;
            return PartialView(data);
        }
        [HttpPost]
        public JsonResult ChangePass2(string PasswordOld, string PasswordNew)
        {
            var ReturnData = new ReturnData();

            try
            {

                if (string.IsNullOrEmpty(PasswordOld) || string.IsNullOrEmpty(PasswordNew))
                {
                    ReturnData.ResponseCode = -7002;
                    ReturnData.Description = "Mật khẩu cũ và mật khẩu mới không được phép bỏ trống";
                    return Json(ReturnData);
                }
                if (string.Compare(PasswordOld, PasswordNew) == 0)
                {
                    ReturnData.ResponseCode = -7003;
                    ReturnData.Description = "Mật khẩu cũ và mật khẩu mới không được phép trùng nhau";
                    return Json(ReturnData);
                }
                PasswordOld = Encrypt.MD5(PasswordOld + Config.GetAppsetting("Salt"));
                PasswordNew = Encrypt.MD5(PasswordNew + Config.GetAppsetting("Salt"));
                var Result = _userservice.ChangePassword2(CurrentUser.Username, PasswordOld, PasswordNew);
                if (Result >= 0)
                {
                    ReturnData.Description = "Đổi mật khẩu thành công";
                }
                else switch (Result)
                    {
                        case -1: ReturnData.Description = "Tài khoản không tồn tại"; break;
                        case -2: ReturnData.Description = "Tài khoản bị block"; break;
                        case -3: ReturnData.Description = "Mật khẩu cũ không đúng"; break;
                        case -600: ReturnData.Description = "Tham số truyền vào không hợp lệ"; break;
                        default: ReturnData.Description = "Hệ thống đang bận. Vui lòng quay lại sau"; break;
                    }

                ReturnData.ResponseCode = Result;

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
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public ActionResult TopupUserHold(string id)
        {
            ViewBag.id = id;
            int Id = int.Parse(Encrypt.Base64Decode(id));
            if (Id > 1)
            {
                var user = _userservice.SelectByUserID(Id);
                ViewBag.UserName = user.Username;
                //if (user.CreatedUser != CurrentUser.Username &&)
                //{

                //    return new EmptyResult();
                //}
            }

            return PartialView();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [PermissionFilter(FunctionCode = FunctionCode.Users, FunctionType = (int)Enums.FunctionType.IsUpdate)]
        public JsonResult TopupHold(string id, int amount, string note)
        {
            var ReturnData = new ReturnData();
            try
            {

                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 1)
                {
                    var user = _userservice.SelectByUserID(Id);
                    var result = _transervice.TopupHold(user.Username, amount, note, "");
                    ReturnData.ResponseCode = result;
                    if (result >= 0)
                    {
                        //Session[SessionsManager.SESSION_USER_FULL] = null;
                        Session[SessionsManager.SESSION_USER_FULL] = _userservice.GetByUsername(CurrentUser.Username);
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
        #endregion


        #region Quản trị Log người dùng
        [PermissionFilter(FunctionCode = FunctionCode.SystemConfig)]
        public ActionResult GetConfig()
        {
            var token = Session[SessionsManager.SESSION_TOKEN].ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                Session[SessionsManager.SESSION_TOKEN] = token;
            }
            var lockstatus = ServerProcess.GetUserLockStatusCache(CurrentFullUser.UserAPI);
            ViewBag.lockstatus = lockstatus;
            return View();

        }
        [PermissionFilter(FunctionCode = FunctionCode.SystemConfig)]
        [HttpPost]
        public JsonResult SaveSystemConfig(string status)
        {
            var ReturnData = new ReturnData();
            try
            {
                var token = Session[SessionsManager.SESSION_TOKEN].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    token = ServerProcess.GetUserTokenCache(CurrentFullUser.UserAPI, CurrentFullUser.PasswordAPI);
                    Session[SessionsManager.SESSION_TOKEN] = token;
                }
                ServerProcess.StartPause(status, token, CurrentFullUser.UserAPI);

                ReturnData.ResponseCode = 1;
                ReturnData.Description = "Thành công";
                return Json(ReturnData);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                ReturnData.ResponseCode = -99;
                ReturnData.Description = "Hệ thông đang bận !";
                return Json(ReturnData);
            }

        }
        [PermissionFilter(FunctionCode = FunctionCode.UserLog)]
        public ActionResult ManagerUserLog()
        {
            var lstUser = new List<Users>();
            if (ViewBag.IsAdmin)
            {
                lstUser = _userservice.GetAll().ToList();
                lstUser.Insert(0, new Users { UserID = -1, Username = "--Tất cả--" });
            }
            else
            {
                //lấy thằng con của nó
                if (CurrentUser.Type == 2)
                {
                    lstUser = _userservice.GetByEmail(CurrentFullUser.UserAPI);
                }
                else
                {
                    if (CurrentUser.Type == 3)
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
            var fromDate = DateTime.Now.AddMonths(-1);
            var toDate = DateTime.Now;

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.UserList = lstUser;
            ViewBag.Title = "Quản trị log người dùng";
            return View(Permission);
        }
        [PermissionFilter(FunctionCode = FunctionCode.UserLog)]
        public ActionResult ListUserLog(string FromDate, string ToDate, int UserId, int? currentPage, int? pageSize, string keyword)
        {
            int CurrPage = currentPage == null ? 1 : (int)currentPage;
            int RecordPerPage = pageSize == null ? 20 : (int)pageSize;
            ViewBag.TotalRecord = 0;
            List<UsersLog> data = new List<UsersLog>();
            int TotalRecord = 0;
            if (!ViewBag.IsAdmin)
            {
                if (CurrentUser.Type == 2 || CurrentUser.Type == 3)
                {
                    if (UserId == -1)
                    {
                        UserId = CurrentUser.UserID;

                    }
                }
                else
                {
                    UserId = CurrentUser.UserID;
                }
            }

            if (FromDate != null && ToDate != null)
            {
                DateTime fromdate = DateTime.ParseExact(FromDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime todate = DateTime.ParseExact(ToDate, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                string newFromDate = fromdate.ToString("MM/dd/yyyy HH:mm:ss");
                string newToDate = todate.ToString("MM/dd/yyyy HH:mm:ss");
                data = _userlogservice.GetListUsersLog(newFromDate, newToDate, UserId, "", keyword, CurrPage, RecordPerPage, ref TotalRecord);
                if (data.Count > 0)
                    ViewBag.TotalRecord = TotalRecord;
            }
            else
            {
                data = _userlogservice.GetListUsersLog(FromDate, ToDate, UserId, "", keyword, CurrPage, RecordPerPage, ref TotalRecord);
                if (data.Count > 0)
                    ViewBag.TotalRecord = TotalRecord;
            }

            ViewBag.PageSize = RecordPerPage;
            ViewBag.CurrentPage = CurrPage;
            return PartialView(data);

        }
        [HttpPost]
        public JsonResult DelDataUserLog(string fromdate, string todate)
        {
            var ReturnData = new ReturnData();
            try
            {

                if (string.IsNullOrEmpty(fromdate) || string.IsNullOrEmpty(todate))
                {
                    ReturnData.ResponseCode = -600;
                    ReturnData.Description = "dữ liệu đầu vào không hợp lệ";
                    return Json(ReturnData);
                }

                DateTime FromDate = DateTime.ParseExact(fromdate, "d/M/yyyy", CultureInfo.InvariantCulture);
                DateTime ToDate = DateTime.ParseExact(todate, "d/M/yyyy", CultureInfo.InvariantCulture);

                if (DateTime.Compare(FromDate, ToDate) > 0)
                {
                    ReturnData.ResponseCode = -600;
                    ReturnData.Description = "dữ liệu đầu vào không hợp lệ";
                    return Json(ReturnData);
                }
                string newFromDate = FromDate.ToString("MM/dd/yyyy");
                string newToDate = ToDate.ToString("MM/dd/yyyy");
                //if (Permission.IsDelete && Permission.FunctionID == (int)Enums.FunctionId.UserLog)
                //{
                //    var status = _userlogservice.DeleteUsersLog(newFromDate, newToDate, -1, -1);
                //    if (status >= 0)
                //    {
                //        //Ghi log
                //        _userlogservice.InsertUsersLog(new UsersLog
                //        {
                //            FunctionID = Permission.FunctionID,
                //            Description = "Delete UserLog :  " + fromdate + "-" + todate,
                //            UserID = CurrentUser.UserID,
                //             ClientIP = Config.GetIP()
                //        });
                //        ReturnData.ResponseCode = 1;
                //        ReturnData.Description = "Xóa thành công";
                //        return Json(ReturnData);
                //    }
                //    else
                //    {
                //        ReturnData.ResponseCode = -2;
                //        ReturnData.Description = "Đã có lỗi xảy ra trong quá trình xóa dữ liệu !";
                //        return Json(ReturnData);
                //    }
                //}
                ReturnData.ResponseCode = -3;
                ReturnData.Description = "Không có quyền xóa";
                return Json(ReturnData);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                ReturnData.ResponseCode = -99;
                ReturnData.Description = "Hệ thông đang bận !";
                return Json(ReturnData);
            }

        }
        #endregion
        [PermissionFilter(FunctionCode = FunctionCode.Device)]
        public ActionResult Device()
        {
            ViewBag.UserAPI = CurrentFullUser.UserAPI;
            return View(model: CurrentFullUser.UserAPI);
        }

    }
}