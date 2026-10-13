using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.CardTelco.PayDirect
{
    public class CardResult
    {
        public string status { get; set; }              //Trạng thái xử lý : 01 là Thành công, còn lại xem bảng mã lỗi
        public string description { get; set; }         //Diễn giải kết quả
        public string cardSerial { get; set; }          //Serial thẻ sử dụng
        public string cardCode { get; set; }            //Mã số bí mật thẻ sử
        public string amount { get; set; }              //Mệnh giá thẻ sử dụng.0: nếu giao dịch Thất bại. >0: nếu giao dịch Thành công.
        public string transRef { get; set; }
    }

    public class CardRequest
    {
        public string issuer { get; set; }          // Loại thẻ cần sử dụng. Các giá trị hợp lệ gồm: MOBI, VINA, VT, VCOIN, GATE, GM, BIT, ZING
        public string cardSerial { get; set; }      // Serial thẻ sử dụng
        public string cardCode { get; set; }        // Mã số bí mật thẻ sử dụng
        public string amount { get; set; }          // Mệnh giá thẻ sử dụng
        public string transRef { get; set; }        // Mã giao dịch trên hệ thống của Đối tác (duy nhất, tối đa 30 ký tự).
        public string partnerCode { get; set; }     // Mã đối tác trên hệ thống gạch thẻ
        public string password { get; set; }        // Mật khẩu của Đối tác trên hệ thống gạch thẻ.
        public string accountId { get; set; }       // Tên tài khoản game cần nạp tiền
        public string serviceCode { get; set; }     // Mã dịch vụ cần nạp
        public string signature { get; set; }       // MD5 (issuer + cardCode + transRef + partnerCode + password + secretKey)
    }

    public class CardData
    {
        public string Serial { get; set; }
        public string Code { get; set; }
        public int Value { get; set; }
        public string Type { get; set; }
    }
}

