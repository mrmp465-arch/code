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

namespace SMS.CMS.Controllers
{
    public class ContactController : Controller
    {
        private readonly IUsersService _userservice;
        private readonly IContactsService _contactservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IUserRoleService _userroleservice;
        private readonly ITransactionsService _transervice;
        private readonly IGroupsService _groupservice;
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private Users CurrentFullUser { get { return ((Users)Session[SessionsManager.SESSION_USER_FULL]); } }
        public ContactController(IGroupsService groupservice, IContactsService contactservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _transervice = transervice;
            _contactservice = contactservice;
            _groupservice = groupservice;
        }
        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
        public ActionResult Index()
        {
            var lstGroup = _groupservice.GetList(2, CurrentUser.Username);
            lstGroup.Insert(0, new Groups { GroupID = -1, Name = "--Nhóm--" });

            ViewBag.GroupList = lstGroup;
            return View();
        }

        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
        public ActionResult ListContact(int? group, string email, int? currentPage, int? pageSize)
        {
            var data = new List<Contacts>();

            string Email = string.IsNullOrEmpty(email) ? string.Empty : email;
            int Group = group == null ? -1 : (int)group;
            int TotalRecord = 0;
            int CurrPage = currentPage == null ? 1 : (int)currentPage;
            int RecordPerPage = pageSize == null ? 20 : (int)pageSize;
            data = _contactservice.GetFilter(Group, Email, CurrentUser.Username, CurrPage, RecordPerPage, ref TotalRecord);

            ViewBag.TotalRecord = TotalRecord;
            ViewBag.PageSize = RecordPerPage;
            ViewBag.CurrentPage = CurrPage;

            var lstGroup = _groupservice.GetList(2, CurrentUser.Username);

            ViewBag.GroupList = lstGroup;
            return PartialView(data);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
        public ActionResult ReportExcel(int? group, string email)
        {
            var data = new List<Contacts>();

            string Email = string.IsNullOrEmpty(email) ? string.Empty : email;
            int Group = group == null ? -1 : (int)group;
            int TotalRecord = 0;
           
           
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Data");
                        worksheet.Column(1).Width = 30;
                        worksheet.Column(2).Width = 30;
                        worksheet.Column(3).Width = 15;
                        worksheet.Column(4).Width = 15;
                        worksheet.Column(5).Width = 15;
                        //worksheet.Column(1).Style.WrapText = true;
                        //worksheet.Column(2).Style.WrapText = true;
                        //worksheet.Column(3).Style.WrapText = true;
                        //worksheet.Column(4).Style.WrapText = true;
                        //worksheet.Column(5).Style.WrapText = true;
                        var properties = new[]
                        {
                            "Mobile",
                            "Name",
                            "Tùy chọn ",
                            "Tùy chọn 2",
                            "Tùy chọn 3"
                        };
                        for (int i = 0; i < properties.Length; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = properties[i];
                            worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                            //worksheet.Cells[1, i + 1].AutoFitColumns();
                        }
                        var lstdata  = _contactservice.GetFilter(Group, Email, CurrentUser.Username, 1, 100000, ref TotalRecord);
                        int row = 2;
                        foreach (var item in lstdata)
                        {
                           
                           
                            int col = 1;

                            worksheet.Cells[row, col].Value =item.Number;
                            col++;

                            worksheet.Cells[row, col].Value = item.Name;
                            col++;

                            worksheet.Cells[row, col].Value = item.Option1;
                            col++;

                            worksheet.Cells[row, col].Value = item.Option2;
                            col++;

                            worksheet.Cells[row, col].Value = item.Option3;
                            col++;

                            row++;
                        }
                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "Contact.xlsx");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
        public ActionResult Upload()
        {
            var lstGroup = _groupservice.GetList(2, CurrentUser.Username);
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            ViewBag.JsonGroup = jsonSerializer.Serialize(lstGroup.Select(x => x.Name).Distinct().ToList()).ToString();
            return PartialView();
        }
        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult SaveMulti(List<ContactsForm> data)
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
                var group = _groupservice.GetByName(GroupName, CurrentUser.Username);
                if (group != null)
                {
                    GroupId = group.GroupID;

                }
                else
                {
                    group = new Groups { IsActive = true, Name = GroupName, Alias = CurrentUser.Username, Type = 2 };
                    GroupId = _groupservice.InsertUpdate(group);
                    ReturnData.Extended = "add";
                }
                IFormatProvider culture = new CultureInfo("en-US", true);
                foreach (var item in data)
                {
                    var contact = new Contacts
                    {
                        Id = 0,
                        CreatedUser = CurrentUser.Username,
                        Group = GroupId,
                        Gender = item.Gender,
                        Email = StringUtils.ReplaceVietnameseChar(item.Email),
                        Name = StringUtils.ReplaceVietnameseChar(item.Name),
                        Number = StringUtils.FormatTelCo(item.Number),
                        Birthday = String.IsNullOrEmpty(item.Birthday) ? (DateTime?)null : DateTime.ParseExact(item.Birthday, "dd/MM/yyyy", culture),
                        Option1 = StringUtils.ReplaceVietnameseChar(item.Option1),
                        Option2 = StringUtils.ReplaceVietnameseChar(item.Option2),
                        Option3 = StringUtils.ReplaceVietnameseChar(item.Option3),
                        Option4 = StringUtils.ReplaceVietnameseChar(item.Option4),
                        Option5 = StringUtils.ReplaceVietnameseChar(item.Option5),

                    };
                    contact.Telco = StringUtils.GetTelCo(contact.Number);
                    if (contact.Telco > 0)
                    {
                        _contactservice.InsertUpdate(contact);
                    }
                    
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
        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string id)
        {
            var ReturnData = new ReturnData();
            try
            {
                int Id = int.Parse(Encrypt.Base64Decode(id));
                if (Id > 0)
                {
                    var result = _contactservice.Delete(Id);
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
        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
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
                    var result = _contactservice.DeleteDynamic(where);
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
                    var result = _contactservice.DeleteDynamic(where);
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
        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
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
                    var result = _contactservice.DeleteDynamic(where);
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
        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
        public ActionResult AddContactInfo()
        {

            var obj = new Contacts
            {

                Group = 1,


            };
            var lstGroup = _groupservice.GetList(2, CurrentUser.Username);
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            ViewBag.JsonGroup = jsonSerializer.Serialize(lstGroup.Select(x => x.Name).Distinct().ToList()).ToString();
            ViewBag.Title = "Thêm mới danh bạ";
            ViewBag.GroupList = lstGroup;
            return PartialView("GetContactInfo", obj);
        }
        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
        public ActionResult GetContactInfo(int? id)
        {
            int Id = id == null ? 0 : (int)id;
            var obj = new Contacts();

            if (Id > 0)
            {
                obj = _contactservice.Get(Id);
            }

            var lstGroup = _groupservice.GetList(2, CurrentUser.Username);
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            ViewBag.JsonGroup = jsonSerializer.Serialize(lstGroup.Select(x => x.Name).Distinct().ToList()).ToString();
            ViewBag.GroupList = lstGroup;
            ViewBag.Title = "Cập nhật danh bạ";
            return PartialView(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilter(FunctionCode = FunctionCode.Contact)]
        public JsonResult SaveData(ContactsForm item)
        {
            var ReturnData = new ReturnData();

            try
            {
                IFormatProvider culture = new CultureInfo("en-US", true);
                var contact = new Contacts
                {
                    Id = item.Id,
                    CreatedUser = CurrentUser.Username,
                    Gender = 1,
                    Email = StringUtils.ReplaceVietnameseChar(item.Email),
                    Name = StringUtils.ReplaceVietnameseChar(item.Name),
                    Number = StringUtils.FormatTelCo(item.Number),
                    Birthday = String.IsNullOrEmpty(item.Birthday) ? (DateTime?)null : DateTime.ParseExact(item.Birthday, "dd/MM/yyyy", culture),
                    Option1 = StringUtils.ReplaceVietnameseChar(item.Option1),
                    Option2 = StringUtils.ReplaceVietnameseChar(item.Option2),
                    Option3 = StringUtils.ReplaceVietnameseChar(item.Option3),
                    Option4 = StringUtils.ReplaceVietnameseChar(item.Option4),
                    Option5 = StringUtils.ReplaceVietnameseChar(item.Option5),

                };

                contact.Telco = StringUtils.GetTelCo(contact.Number);
                if(contact.Telco<=0)
                {
                    ReturnData.ResponseCode = -101;
                    ReturnData.Description = "Số điện thoại không đúng định dạng";
                    return Json(ReturnData);
                }
                var group = _groupservice.GetByName(item.GroupName, CurrentUser.Username);
                if (group != null)
                {
                    contact.Group = group.GroupID;

                }
                else
                {
                    group = new Groups { IsActive = true, Name = item.GroupName, Alias = CurrentUser.Username, Type = 2 };
                    contact.Group = _groupservice.InsertUpdate(group);
                }
                var result = _contactservice.InsertUpdate(contact);
                ReturnData.ResponseCode = result;
                if (result >= 0)
                {
                    if (contact.Id > 0)
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
    }
}