using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using Libs.Db;
using Libs.Utils;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Collections;

namespace Libs.BankGate.BaoKim
{
    public class BaoKim
    {
        public long TransactionID { get; set; }         // Mã giao dịch
        public long Merchant_id { get; set; }           // Mã merchant
        public string Business { get; set; }            // Email cua merchant nhận tiền
        public string ResponseNo { get; set; }          // Mã giao dịch của Bảo Kim
        public long Created_on { get; set; }            // Thời điểm tạo giao dịch trên Bảo Kim, 
        public int Payment_type { get; set; }           // Hình thức thanh toán: 1: thanh toán trực tiếp, 2: thanh toán an toàn
        public int Transaction_status { get; set; }     // Trạng thái giao dịch
        public double Total_amount { get; set; }         // Tổng số tiền người mua thanh toán
        public double Net_amount { get; set; }           // Số tiền người bán thực nhận
        public double Fee_amount { get; set; }           // Phí dịch vụ baokim thu
        public string Customer_name { get; set; }       // Tên người thanh toán
        public string Customer_email { get; set; }      // Email người thanh toán
        public string Customer_phone { get; set; }      // Số điện thoại người thanh toán
        public string Customer_address { get; set; }    // Địa chỉ người thanh toán
        public int ReturnValue { get; set; }

        private string UrlCheckOut = ConfigurationManager.AppSettings["BaoKim_UrlCheckOut"];
        private string BaoKim_ReturnURL = ConfigurationManager.AppSettings["BaoKim_ReturnURL"];
        public string BaoKim_Key = ConfigurationManager.AppSettings["BaoKim_Key"];

        public BaoKim()
        {
            Business = ConfigurationManager.AppSettings["BaoKim_Business"];
            Merchant_id = Convert.ToInt64(ConfigurationManager.AppSettings["BaoKim_MerchantID"]);
        }

        public BaoKim Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BaoKim>("sp_pay_BaoKim_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public BaoKim Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BaoKim>("sp_pay_BaoKim_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Business", Business);
            pars[3] = new SqlParameter("@Merchant_id", Merchant_id);

            db.ExecuteNonQuerySP("sp_pay_BaoKim_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[13];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@ResponseNo", ResponseNo);
            pars[3] = new SqlParameter("@Created_on", Created_on);
            pars[4] = new SqlParameter("@Payment_type", Payment_type);
            pars[5] = new SqlParameter("@Transaction_status", Transaction_status);
            pars[6] = new SqlParameter("@Total_amount", Total_amount);
            pars[7] = new SqlParameter("@Net_amount", Net_amount);
            pars[8] = new SqlParameter("@Fee_amount", Fee_amount);
            pars[9] = new SqlParameter("@Customer_name", Customer_name);
            pars[10] = new SqlParameter("@Customer_email", Customer_email);
            pars[11] = new SqlParameter("@Customer_phone", Customer_phone);
            pars[12] = new SqlParameter("@Customer_address", Customer_address);

            db.ExecuteNonQuerySP("sp_pay_BaoKim_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public string GetUrlCheckOut(BankGateAPI bankGateAPI)
        {
            string business = Business;
            string merchant_id = Merchant_id.ToString();
            string order_description = bankGateAPI.OrderInfo;
            string order_id = bankGateAPI.TransactionID.ToString();
            string total_amount = bankGateAPI.TotalAmount.ToString();
            string url_success = BaoKim_ReturnURL;

            string checksum = business + merchant_id + order_description + order_id + total_amount + url_success;
            checksum = GetHmacSHA1(checksum, BaoKim_Key).ToUpper();

            return UrlCheckOut
                + "?business=" + HttpUtility.UrlEncode(business)
                + "&merchant_id=" + HttpUtility.UrlEncode(merchant_id)
                + "&order_description=" + HttpUtility.UrlEncode(order_description)
                + "&order_id=" + HttpUtility.UrlEncode(order_id)
                + "&total_amount=" + HttpUtility.UrlEncode(total_amount)
                + "&url_success=" + HttpUtility.UrlEncode(url_success)
                + "&checksum=" + HttpUtility.UrlEncode(checksum);
        }

        public string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();

            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }

            String md5String = s.ToString();
            return md5String;
        }

        public string GetHmacSHA1(string data, string key)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            byte[] keyByte = encoding.GetBytes(key);

            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);

            byte[] messageBytes = encoding.GetBytes(data);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);

            string encrypted = this.ByteToString(hashmessage);
            return encrypted;
        }

        private string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }

        public bool VerifyResponseUrl(NameValueCollection get_params)
        {
            //Sắp xếp các phần tử trong mảng tham số trả về theo key để mã hóa
            ICollection keyCollection = get_params.Keys;
            string[] keys = new string[keyCollection.Count];
            keyCollection.CopyTo(keys, 0);
            Array.Sort(keys);

            string str_combined = "";
            string checksum = "";
            foreach (string key in keys)
            {
                if (String.Compare(key, "checksum", true) != 0)
                {
                    Object value = get_params[key];
                    str_combined += value.ToString();
                }
                else
                {
                    checksum = get_params[key].ToString();
                }
            }

            //Mã hóa tạo check sum, so sánh với checksum gửi về từ BaoKim.vn
            string verify_checksum = GetHmacSHA1(str_combined, BaoKim_Key);

            return String.Compare(verify_checksum, checksum, true) == 0;
        }
    }
}
