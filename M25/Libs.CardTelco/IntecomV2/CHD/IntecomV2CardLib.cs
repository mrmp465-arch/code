using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Libs.API;

namespace Libs.CardTelco.IntecomV2.CHD
{
    public class IntecomV2CardLib
    {
        public class CardRequest
        {
            public string FunctionName { get; set; }     // Xác định tên hàm chức năng - Mặc định là: UseCard
            public string CardSerial { get; set; }      // Số Serial thẻ cào
            public string CardCode { get; set; }        // Mã thẻ cào, được mã hóa TripleDES trước khi truyền lên, với Partnerkey mã hóa được VTC tạo ra và cung cấp cho đối tác khi thực hiện kết nối
            public string PartnerCode { get; set; }     // Định danh đối tác gọi đến hệ thống VTC, được VTC cung cấp khi thực hiện kết nối.
            public string PartnerServiceCode { get; set; }        // Định danh dịch vụ của đối tác, với mỗi dịch vụ được VTC cung cấp một PartnerServiceCode riêng.
            public string CardType { get; set; }     // Loại thẻ, VTC chỉ chấp nhận những loại thẻ sau: -	VCOIN: thẻ Vcoin
            public Int64 TransID { get; set; }        // Là mã giao dịch do đối tác sinh ra và gửi lên để phục vụ tra soát, đối tác phải tự đảm bảo mã giao dịch này là duy nhất.
            public string AccountName { get; set; }       // Là định danh tài khoản khách hàng sử dụng dịch vụ tại hệ thống của đối tác, thông tin này dùng để tra soát và giải quyết khiếu nại khách hàng
            public string ExtentionData { get; set; }     // Trường dữ liệu mở rộng. Truyền null ("ExtentionData":"") nếu ko sử dụng

        }

        public class CardResult
        {
            public Int64 ResponseCode { get; set; }       //Trạng thái xử lý : 01 là Thành công, còn lại xem bảng mã lỗi
            public int Status { get; set; }         //Diễn giải kết quả
            public string Description { get; set; }     //Serial thẻ sử dụng
            public string DataInfo { get; set; }        //Trả về mệnh giá thẻ, được mã hóa TripleDES với Partnerkey giải mã được VTC cung cấp cho đối tác khi thực hiện kết nối.
        }

        public static string Encrypt(string data, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();
            byte[] byteHash;
            byte[] byteBuff;
            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = Encoding.UTF8.GetBytes(BitConverter.ToString(byteHash).ToLower().Replace("-", "").Substring(0, 24));
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Encoding.UTF8.GetBytes(data);
            string encoded =
                Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return encoded;
        }

        public static string Decrypt(string data, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();
            byte[] byteHash;
            byte[] byteBuff;
            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = Encoding.UTF8.GetBytes(BitConverter.ToString(byteHash).ToLower().Replace("-", "").Substring(0, 24));
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Convert.FromBase64String(data);
            string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return plaintext;

        }

    }
}
