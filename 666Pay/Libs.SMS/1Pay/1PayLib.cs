using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.SMS._1Pay
{

    public class SmsMo1Pay
    {
        public string access_key { get; set; } // Đại diện cho sản phẩm của merchant khai báo trong hệ thống 1pay.vn
        public string command { get; set; } // Mã tin nhắn, là keyword đầu tiên trong tin nhắn của khách hàng, ví dụ tin nhắn có nội dung "DK Game" gửi 8038 thì command sẽ là DK
        public string mo_message { get; set; } // Nội dung tin nhắn của khách hàng
        public string msisdn { get; set; } // Số điện thoại của khách hàng, bắt đầu bằng 84, ví dụ 8498238193
        public string request_id { get; set; } // Id của tin nhắn, ở dạng String
        public string request_time { get; set; } // Thời gian đầu số nhận được tin nhắn, ở dạng iso, ví dụ: 2013-07-06T22:54:50Z
        public string short_code { get; set; } // Đầu số nhận tin nhắn, ví dụ tin nhắn có nội dung DK Game” gửi 8038 thì short_code sẽ là 8038
        public string signature { get; set; } // access_key=$access_key&command=$command&mo_message=$mo_message&msisdn=$msisdn&request_id=$request_id&request_time=$request_time&short_code=$short_code được hmac bằng thuật toán SHA256

        //SMS Plus
        public string amount { get; set; }
        public string telco { get; set; }
        public string error_code { get; set; }
        public string error_message { get; set; }

    }
    
}
