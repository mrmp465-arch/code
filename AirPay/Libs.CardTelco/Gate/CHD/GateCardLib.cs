using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;

namespace Libs.CardTelco.Gate.CHD
{
    public class GateCardLib
    {
        public class RequestData
        {
            public string FunctionName { get; set; }
            public string MerchantID { get; set; }
            public string Username { get; set; }
            public string CardSerial { get; set; }
            public string CardPIN { get; set; }
            public string Signature { get; set; }
            public string PartnerTransactionID { get; set; }
            public string TelcoServiceCode { get; set; }

        }

        public class ResponseData
        {
            public string ErrorCode { get; set; }
            public string Description { get; set; }
            public string TransactionID { get; set; }
            public string PartnerTransactionID { get; set; }
            public string CardAmount { get; set; }
            public string VendorTransactionID { get; set; }
        }

        public static string XmlSerialize(object serializingObject)
        {
            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineHandling = System.Xml.NewLineHandling.Replace;
            settings.NewLineChars = "\\n";

            System.IO.StringWriter sWriter = new System.IO.StringWriter();
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sWriter, settings);
            System.Xml.Serialization.XmlSerializerNamespaces namespaces =
                new System.Xml.Serialization.XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(serializingObject.GetType());

            serializer.Serialize(writer, serializingObject, namespaces);
            return sWriter.ToString();
        }

        public static ResponseData GateResponse(string data)
        {
            //data = "ErrorCode|Description|TransactionID|PartnerTransactionID|CardAmount";
            string[] s = data.Split('|');

            if (s.Count() == 2)
            {
                return new ResponseData()
                {
                    ErrorCode = s[0],
                    Description = s[1]
                };
            }
            else if (s.Count() == 3)
            {
                return new ResponseData()
                {
                    ErrorCode = s[0],
                    Description = s[1],
                    TransactionID = s[2]
                };
            }
            else if (s.Count() == 4)
            {
                return new ResponseData()
                {
                    ErrorCode = s[0],
                    Description = s[1],
                    TransactionID = s[2],
                    PartnerTransactionID = s[3]
                };
            }
            else if (s.Count() == 6)
            {
                return new ResponseData()
                {
                    ErrorCode = s[0],
                    Description = s[1],
                    TransactionID = s[2],
                    PartnerTransactionID = s[3],
                    CardAmount = s[4],
                    VendorTransactionID = s[5]
                };
            }

            return new ResponseData()
            {
                ErrorCode = "999", // TrungDT tự Define
                Description = "Không phân tích được ResponseData"
            };
        }

        public static int ConvertResponCode(string responseStatus)
        {
            switch (responseStatus)
            {
                case "02":
                    //02: Tham số đầu vào (PartnerTransactionID) không hợp lệ.
                    return (int)API.ResponseCode.ParameterInvalid;
                case "03":
                    //03: Tham số đầu vào (Phone) không hợp lệ.
                    return (int)API.ResponseCode.ParameterInvalid;
                case "05":
                    //05: Tham số đầu vào (Username) không hợp lệ.
                    return (int)API.ResponseCode.ParameterInvalid;
                case "07":
                    //07: Tham số đầu vào (Serial) không hợp lệ.
                    return (int)API.ResponseCode.ParameterInvalid;
                case "08":
                    //08: Tham số đầu vào (PIN) không hợp lệ.
                    return (int)API.ResponseCode.ParameterInvalid;
                case "09":
                    //09: Tham số đầu vào (Email) không hợp lệ.
                    return (int)API.ResponseCode.ParameterInvalid;
                case "10":
                    //10: Thẻ đã quá hạn sử dụng
                    return (int)ResponseCode.CardHasExpired;
                case "11":
                    //11:  Trạng thái thẻ không phù hợp.
                    return (int)ResponseCode.CardFormatInvalid;
                case "12":
                    //-12: Lỗi hệ thống telco.
                    return (int)ResponseCode.SystemError;
                case "14":
                    //14: Trùng giao dịch.
                    return (int)ResponseCode.TransactionDuplicate;
                case "15":
                    //-15: Thẻ đã đƣợc sử dụng.
                    return (int)ResponseCode.CardUsed;
                case "16":
                    //16: Thẻ không tồn tại hoặc chƣa đƣợc kích hoạt.
                    return (int)ResponseCode.CardNotActivated;
                case "17":
                    //17:  Thông tin mã thẻ không đúng định dạng.
                    return (int)ResponseCode.CardFormatInvalid;
                case "18":
                    //18:  Mã thẻ và số serial thẻ không khớp.
                    return (int)ResponseCode.CardSerialInvalid;
                case "19":
                    //19: Thẻ đã hết hạn sử dụng.
                    return (int)ResponseCode.CardHasExpired;
                case "20":
                    //20:  Chữ ký điện tử không hợp lệ.
                    return (int)ResponseCode.SignatureInvalid;
                case "21":
                    //21:  Thẻ đã bị khóa.
                    return (int)ResponseCode.CardIsLocked;
                case "22":
                    //22: Tài khoản bị tạm khóa vì nhập thẻ sai nhiều lần.
                    return (int)ResponseCode.AccountLocked;
                case "23":
                    //23:  Serial va PIN khong ton tai
                    return (int)ResponseCode.CardSerialInvalid;
                case "24":
                    //24: Đối tác kết nối telco không hợp lệ
                    return (int)ResponseCode.PartnerNotExistsNotActive;
                case "25":
                    //25: Thẻ không tồn tại
                    return (int)ResponseCode.CardProviderInvalid;
                case "26":
                    //26: Đầu thẻ không đƣợc hỗ trợ
                    return (int)ResponseCode.CardTypeInvalid;
                case "27":
                    //27: Mệnh giá thẻ chƣa đƣợc cấu hình cho đối tác
                    return (int)ResponseCode.CardAmountInvalid;
                case "28":
                    //28:  Lỗi không gọi đƣợc nhà mạng
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "29":
                    //29: Timeout từ hệ thống core
                    return (int)ResponseCode.TransactionTimeout;
                case "30":
                    //30:  Lỗi database hệ thống
                    return (int)ResponseCode.SystemError;
                case "31":
                    //31: Mất kết nối với hệ thông core
                    return (int)ResponseCode.SystemError;
                case "32":
                    //32: Thẻ đã bị khóa hoặc đã hết hạn sử dụng.
                    return (int)ResponseCode.CardIsLocked;
                case "81":
                    //81: Đại lý không hợp pháp.
                    return (int)ResponseCode.AccessDenied;
                case "96":
                    //96: Đối tác bị khóa..
                    return (int)ResponseCode.AccountLocked;
                case "97":
                    //97: Đối tác không hợp lệ.
                    return (int)ResponseCode.AccountNotExists;
                case "98":
                    //98: Địa chỉ IP không hợp lệ.
                    return (int)ResponseCode.ServiceIsPause;
                case "99":
                    //99: Giao dịch thất bại.
                    return (int)ResponseCode.TransactionFailed;
                case "100":
                    //100: Dịch vụ đang bảo trì.
                    return (int)ResponseCode.SystemMaintain;
                case "101":
                    //101: Dịch vụ không hợp lệ.
                    return (int)ResponseCode.ServiceNotExists;
                case "991":
                    //991: Gọi Telco bị timeout vui lòng liên hệ nha cung cấp để đƣợc hỗ trợ..
                    return (int)ResponseCode.TransactionTimeout;
                case "990":
                    //990: Lỗi hệ thống Telco vui lòng liên hệ nha cung cấp để đƣợc hỗ trợ.
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "-1":
                    //-1: Dữ liệu đầu vào không hợp lệ.
                    return (int)ResponseCode.ParameterInvalid;
                case "-100":
                    //-100: Xử lý giao dich thất bại.
                    return (int)ResponseCode.TransactionFailed;
                default:
                    return (int)API.ResponseCode.SystemError;
            }
        }
    }
}
