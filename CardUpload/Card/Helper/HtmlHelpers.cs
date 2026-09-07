using Card.Data.DTO;
using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Card.CMS.Helper
{
    public static class HtmlHelpers
    {
        public static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
          secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);
        public static string GetGroupyName(int? id, List<Groups> lstdata)
        {
            try
            {
                var obj = lstdata.Where(x => x.GroupID == id.GetValueOrDefault()).FirstOrDefault();
                if (obj == null)
                    return "N/A";
                return obj.Name;
            }
            catch
            {

                return "N/A";
            }
        }
        public static string GetTopupType(int type,string telco)
        {
            string result = "Nạp Sò";
            if(telco== "ZING")
                result = "VLTK Miễn Phí";
            if (telco == "VTC")
                result = "Nạp Vcoin";
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

                case 11:
                    result = "VLTK - Công Thành Chiến";
                    if (telco == "GARENA")
                        result = "Nạp Free Fire";
                    break;
                case 12:
                    result = "Võ Lâm Truyền Kỳ 1";
                    break;
                case 13:
                    result = "Kiếm Thế";
                    break;
                case 14:
                    result = "Tân Thiên Long 3D";
                    break;
                case 15:
                    result = "Võ Lâm Truyền Kỳ 2";
                    break;
            }

            return result;
        }
        public static string GetTelcoName(string telco)
        {
            var result = telco;
            switch (telco)
            {
                case "VTT":
                    result = "Viettel";
                    break;
                case "VNP":
                    result = "Vina";
                    break;
                case "VMS":
                    result = "Mobi";
                    break;
               

            }

            return result;
        }
        public static string GetTranStatusNormal(int type)
        {
            string result = "";
            switch (type)
            {
                case 1:
                    result = "Đợi xử lý";
                    break;
                case 2:
                    result = "Đang xử lý";
                    break;
                case 3:
                    result = "Hoàn thành";
                    break;
                case 0:
                    result = "Không sử dụng";
                    break;
                case -1:
                    result = "Bỏ qua";
                    break;
                case -2:
                    result = "Telco khóa";
                    break;
                case -3:
                    result = "Đợi nạp";
                    break;
                case -4:
                    result = "Telco hết lượt";
                    break;
                case -6:
                    result = "Đợi review";
                    break;
                default:
                    result = type.ToString();
                    break;
            }

            return result;
        }
        public static string GetTranType(int type)
        {
            string result = "momo";
            if(type==2)
                result = "bank";
            return result;

        }
            public static string GetTranStatus(int type)
        {
            string result = "";
            switch (type)
            {
                case 0:
                    result = "<div class=\"label label-md label-info\">Chưa xử dụng</div>";
                    break;
                case 2:
                    result = "<div class=\"label label-md label-warning\">Sai mệnh giá</div>";
                    break;
                case 1:
                    result = "<div class=\"label label-md label-success\">Đã sử dụng</div>";
                    break;
               
                case -1:
                    result = "<div class=\"label label-md label-danger\">Thẻ sai</div>";
                    break;
                case -2:
                    result = "<div class=\"label label-md label-danger\">Telco khóa</div>";
                    break;
                case -3:
                    result = "<div class=\"label label-md label-danger\">Đợi nạp</div>";
                    break;
                case -7:
                    result = "<div class=\"label label-md label-default\">Trùng mã</div>";
                    break;
                    
                case -6:
                    result = "<div class=\"label label-md label-warning\">Đợi review</div>";
                    break;
                case -9:
                    result = "<div class=\"label label-md label-info\">Chờ xử lý</div>";
                    break;
                default:
                    result = type.ToString();
                    break;
            }

            return result;
        }
        public static string GetBuyCardtatus(int type)
        {
            string result = "";
            switch (type)
            {
                case 0:
                    result = "<div >Đang xử lý</div>";
                    break;
                case 2:
                    result = "<div class=\"label label-md label-warning\">Sai mệnh giá</div>";
                    break;
                case 1:
                    result = "<div >Thành công</div>";
                    break;

                case -1:
                    result = "<div class=\"label label-md label-danger\">Thẻ sai</div>";
                    break;
                case -2:
                    result = "<div >Hết thẻ</div>";
                    break;
                case -3:
                    result = "<div >Số dư không đủ</div>";
                    break;
                case -7:
                    result = "<div class=\"label label-md label-default\">Trùng mã</div>";
                    break;

                case -6:
                    result = "<div class=\"label label-md label-warning\">Đợi review</div>";
                    break;
                case -99:
                    result = "<div >Lỗi hệ thống/div>";
                    break;
                default:
                    result = type.ToString();
                    break;
            }

            return result;
        }
        public static string GetTranOrder(int type)
        {
            string result = "";
            switch (type)
            {
                case 0:
                    result = "<div class=\"label label-md label-info\">Đợi xử lý</div>";
                    break;
               
                case 1:
                    result = "<div class=\"label label-md label-success\">Thành công</div>";
                    break;

                case -1:
                    result = "<div class=\"label label-md label-danger\">Thất bại</div>";
                    break;
                case -2:
                    result = "<div class=\"label label-md label-default\">Đã hủy</div>";
                    break;
                case 3:
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