using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;

namespace Libs.CardTelco._1Pay.KHD
{
    public class _1PayCardLib
    {
        public class RequestData
        {
            public string telco { get; set; }
            public string serial { get; set; }
            public string cardCode { get; set; }
            public string tranref { get; set; }
            public string partnerCode { get; set; }
            public string password { get; set; }
            public string signature { get; set; }
            public string importCode { get; set; }

        }

        public class ResponseData
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public string Amount { get; set; }
        }

        public static ResponseData GateResponse(string data)
        {
            //data = "Code|Message|Amount";
            string[] s = data.Split('|');

            if (s.Count() == 2)
            {
                return new ResponseData()
                {
                    Code = s[0],
                    Message = s[1]
                };
            }
            else if (s.Count() == 3)
            {
                return new ResponseData()
                {
                    Code = s[0],
                    Message = s[1],
                    Amount = s[2]
                };
            }
           
            return new ResponseData()
            {
                Code = "999", // TrungDT tự Define
                Message = "Không phân tích được ResponseData"
            };
        }

        public static int ConvertResponCode(string responseStatus)
        {
            switch (responseStatus)
            {
                case "00":
                    //00: Mã số nạp tiền không tồn tại hoặc đã được sử dụng.
                    return (int)API.ResponseCode.CardUsed;
                case "02":
                    //02: Giao dịch nghi vấn (Timeout từ telco, chưa xử lý xong).
                    return (int)API.ResponseCode.TransactionSuspicious;
                case "10":
                case "11":
                    //10: Partner không tồn tại
                    //11: Partner bị khóa
                    return (int)API.ResponseCode.PartnerNotExistsNotActive;
                case "14":
                    //14: Sai password
                case "15":
                    //15: Sai địa chỉ IP
                case "20":
                    //20 : Sai độ dài mã số nạp tiền
                case "25":
                    //25: Mã giao dịch không hợp lệ (>0 và < 30 ký tự)
                case "28":
                    //28: Mã số nạp tiền không đúng định dạng (chỉ bao gồm ký tự số)
                    return (int)API.ResponseCode.ParameterInvalid;
                case "16":
                    //16: Sai chữ ký.
                    //19: Mã hóa không hợp lệ
                    return (int)API.ResponseCode.SignatureInvalid;
                case "17":
                    //17: Mã thẻ không hợp lệ
                    return (int)ResponseCode.CardCodeInvalid;
                case "18":
                    //18: Thẻ bị treo
                    return (int)ResponseCode.CardHasExpired;
                case "22":
                    //22: Mã số nạp tiền đang chờ Đơn vị phát hành xử lý
                    return (int)ResponseCode.TransactionReview;
                case "26":
                    //26: Mã giao dịch không tồn tại
                case "29":
                    //29: Không tìm thấy giao dịch
                    return (int)ResponseCode.TransactionNotExists;
                case "30":
                    //30: Quá hạn mức
                    return (int)ResponseCode.BankAmountLimit;
                case "52":
                    //52: Đơn vị phát hành không hỗ trợ nghiệp vụ này
                    return (int)ResponseCode.ServiceNotExists;
                case "70":
                case "97":
                    //70,97: Hệ thống gạch thẻ đang bận
                    return (int)ResponseCode.ServiceNotExists;
                case "98":
                    //98: Dịch vụ đang tạm đóng
                    return (int)ResponseCode.SystemMaintain;

                default:
                    return (int)API.ResponseCode.SystemError;
            }
        }
    }
}
