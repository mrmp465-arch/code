using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;

namespace Libs.CardTelco.C2C.KHD
{
    public class C2CCardLib
    {
        public class RequestData
        {
            public string partnerTransId { get; set; }
            public string partnerCode { get; set; }
            public string telcoCode { get; set; }
            public string pinCode { get; set; }
            public string serial { get; set; }
            public string clientDateTime { get; set; } // yyyyMMddHHmmss
            public string sign { get; set; }
        }

        public class ResponseData
        {
            public string resCode { get; set; }
            public string partnerTransId { get; set; }
            public string partnerCode { get; set; }
            public string pinCode { get; set; }
            public string serial { get; set; }
            public string cardValue { get; set; }
            public string serverDateTime { get; set; }
            public string description { get; set; }
            public string sign { get; set; }

        }

        public static ResponseData GateResponse(string data)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var response = serializer.Deserialize<ResponseData>(data);
            return response;
        }

        public static int ConvertResponCode(string responseStatus)
        {
            switch (responseStatus)
            {
                case "01":
                    //01: Mã đối tác không đúng.
                    return (int)API.ResponseCode.PartnerNotExistsNotActive;
                case "02":
                    //02: Tài khoản đối tác chưa active..
                    return (int)API.ResponseCode.AccountNotExists;
                case "05":
                    //05: Chữ ký điện tử không hợp lệ.
                    return (int)API.ResponseCode.SignatureInvalid;
                case "08":
                    //08: Giao dịch nghi vấn cần kiểm tra (timeout). Trường hợp một mã thẻ gửi nhiều request liên tục. Nếu request trước đã được tiếp nhận và đang xử lý, các request sau sẽ nhận được mã lỗi 08 với mô tả TRANS PROCESSING PREVIOUS mô tả này xem như request thất bại.
                    return (int)API.ResponseCode.TransactionSuspicious;
                case "09":
                case "10":
                    //14: Lỗi hệ thống INTERNAL, EXTERNAL tại C2C Service.
                    return (int)API.ResponseCode.SystemError;
                case "11":
                    //11: Không được phép truy xuất C2C Service
                    return (int)API.ResponseCode.AccessDenied;
                case "12":
                    //20 : Không tìm thấy giao dịch.
                    return (int)API.ResponseCode.TransactionNotExists;
                case "14":
                    //14: Giao dịch thất bại do lỗi từ telco: TELCO ERROR
                case "23":
                    //Giao dịch thất bại do lỗi từ telco: LỖI HỆ THỐNG
                    return (int)API.ResponseCode.SystemError;
                case "15":
                    //15: Mã giao dịch đối tác (partnerTransId) gửi bị trùng lặp.
                    return (int)API.ResponseCode.TransactionDuplicate;
                case "26":
                    //26: Telco bảo trì / hệ thống bận / hệ thống quá tải / hệ thống tạm khóa.
                    return (int)API.ResponseCode.SystemMaintain;
                case "28":
                    //28: Mã thẻ không hợp lệ
                    return (int)ResponseCode.CardCodeInvalid;
                case "29":
                    //29: Mã thẻ cào không đúng định dạng
                    return (int)ResponseCode.ParameterInvalid;
                case "30":
                    //30: Số serial không hợp lệ
                    return (int)ResponseCode.CardSerialInvalid;
                case "31":
                    //31: Mã giao dịch không tồn tại
                    return (int)ResponseCode.CardUsed;
                case "32":
                    //32: Thẻ chưa kích hoạt
                    return (int)ResponseCode.CardNotActivated;
                case "33":
                    //33: Mã thẻ và số serial không khớp
                    return (int)ResponseCode.CardFormatInvalid;
                case "34":
                    //70: Thẻ đã hết hạn sử dụng
                    return (int)ResponseCode.CardHasExpired;
                case "35":
                    //35: Thẻ đã bị khóa
                    return (int)ResponseCode.CardIsLocked;
                default:
                    return (int)API.ResponseCode.SystemError;
            }
        }
    }
}
