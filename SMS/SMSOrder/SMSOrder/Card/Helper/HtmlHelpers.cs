using SMS.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.CMS.Helper
{
    public static class HtmlHelpers
    {
        public static string GetGroupyName(int? id, List<Groups> lstdata)
        {
            try
            {
                var obj = lstdata.Where(x => x.GroupID == id.GetValueOrDefault()).FirstOrDefault();
                if (obj == null)
                    return "";
                return obj.Name;
            }
            catch
            {

                return "";
            }
        }

        public static string GetTopupType(int type)
        {
            string result = "";
            switch (type)
            {
                case 1:
                    result = "Trả trước";
                    break;
                case 2:
                    result = "Trả sau";
                    break;
                case 3:
                    result = "Internet";
                    break;
                case 4:
                    result = "Nạp Edu";
                    break;
                case 5:
                    result = "ĐT cố định";
                    break;
                case 6:
                    result = "Nhà thuốc";
                    break;
                case 7:
                    result = "Tiêm chủng";
                    break;
                case 8:
                    result = "Nạp ShopOne";
                    break;
            }

            return result;
        }
        public static string GetResponeStatus(int type)
        {
            if (type == 0)
                return "Đợi gửi";
            if (type == 1)
                return "Đã gửi";
            if (type > 1)
                return "Thành công";
            if (type < 0)
                return "Thất bại";
            return type.ToString(); ;
        }
        public static string GetCamStatus(int type)
        {
            string result = "";
            switch (type)
            {
                case 0:
                    result = "Khởi tạo";
                    break;
                case 1:
                    result = "Đợi xử lý";
                    break;
                case 2:
                    result = "Đang xử lý";
                    break;
                case 3:
                    result = "Hoàn thành";
                    break;
                case 4:
                    result = "Khóa";
                    break;
                case 5:
                    result = "Đang xử lý";
                    break;
            }
            return result;
        }
        public static string GetTranStatus(int type)
        {
            string result = "";
            switch (type)
            {
                case 1:
                    result = "<div class=\"label label-md label-warning\">Đợi xử lý</div>";
                    break;
                case 2:
                    result = "<div class=\"label label-md label-warning\">Đang xử lý</div>";
                    break;
                case 3:
                    result = "<div class=\"label label-md label-success\">Đã hoàn thành</div>";
                    break;
                case 0:
                    result = "<div class=\"label label-md label-warning\">Không sử dụng</div>";
                    break;
                case -1:
                    result = "<div class=\"label label-md label-warning\">Bỏ qua</div>";
                    break;
                case -2:
                    result = "<div class=\"label label-md label-danger\">Telco khóa</div>";
                    break;
                case -3:
                    result = "<div class=\"label label-md label-warning\">Đợi nạp</div>";
                    break;
                default:
                    result = type.ToString();
                    break;
            }

            return result;
        }
    }
}