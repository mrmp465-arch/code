using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using Libs.Db;
using Libs.Utils;

namespace Libs.BankGate.Smartlink
{
    public class Smartlink
    {
        public long TransactionID { get; set; }             // Mã giao dịch
        public string Merchant { get; set; }                // Mã merchant
        public string TicketNo { get; set; }                // Thông tin đơn hàng
        public int ResponseCode { get; set; }               // Kết quả giao dịch, trả về từ Smartlink
        public string TransactionNo { get; set; }           // Mã số giao dịch của Smartlink
        public string CardType { get; set; }                // Thông tin loại thẻ
        public string BatchNo { get; set; }                 // Thông tin ngày thanh toán
        public string AcqResponseCode { get; set; }         // Mã kết quả giao dịch từ ngân hàng
        public string Message { get; set; }                 // Thông tin lỗi của giao dịch nếu có
        public string AdditionalData { get; set; }          // Thông tin bổ sung là 6 số đầu của thẻ để phân biệt thẻ
        public int ReturnValue { get; set; }

        private string UrlCheckOut = ConfigurationManager.AppSettings["Smartlink_UrlCheckOut"];
        private string SecretKey = ConfigurationManager.AppSettings["Smartlink_SecretKey"];
        private string vpc_Version = ConfigurationManager.AppSettings["Smartlink_Version"];
        private string vpc_Locale = ConfigurationManager.AppSettings["Smartlink_Locale"];
        private string vpc_Command = ConfigurationManager.AppSettings["Smartlink_Command"];
        private string vpc_AccessCode = ConfigurationManager.AppSettings["Smartlink_AccessCode"];
        private string vpc_Currency = ConfigurationManager.AppSettings["Smartlink_Currency"];
        private string vpc_ReturnURL = ConfigurationManager.AppSettings["Smartlink_ReturnURL"];
        private string vpc_BackURL = ConfigurationManager.AppSettings["Smartlink_BackURL"];

        public Smartlink()
        {
            Merchant = ConfigurationManager.AppSettings["Smartlink_Merchant"];
            TicketNo = "";
        }

        public Smartlink Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Smartlink>("sp_pay_Smartlink_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public Smartlink Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Smartlink>("sp_pay_Smartlink_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Merchant", Merchant);
            pars[3] = new SqlParameter("@TicketNo", TicketNo);
            pars[4] = new SqlParameter("@ResponseCode", ResponseCode);
            pars[5] = new SqlParameter("@TransactionNo", TransactionNo);
            pars[6] = new SqlParameter("@CardType", CardType);
            pars[7] = new SqlParameter("@BatchNo", BatchNo);
            pars[8] = new SqlParameter("@AcqResponseCode", AcqResponseCode);
            pars[9] = new SqlParameter("@Message", Message);
            pars[10] = new SqlParameter("@AdditionalData", AdditionalData);

            db.ExecuteNonQuerySP("sp_pay_Smartlink_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public string GetUrlCheckOut(BankGateAPI bankGateAPI)
        {
            //md5_input = Secure_Secret + vpc_AccessCode + vpc_Amount + vpc_Currency + vpc_Locale + vpc_MerchTxnRef + vpc_Merchant + vpc_OrderInfo + vpc_ReturnURL + vpc_TicketNo + vpc_Version;
            string vpc_SecureHash = SecretKey + vpc_AccessCode + bankGateAPI.TotalAmount * 100 + vpc_BackURL + vpc_Command + vpc_Currency + vpc_Locale + bankGateAPI.TransactionID + Merchant + bankGateAPI.OrderNo + vpc_ReturnURL + TicketNo + vpc_Version;
            vpc_SecureHash = Encrypts.MD5(vpc_SecureHash).ToUpper();

            return UrlCheckOut +
                "?vpc_Version=" + HttpUtility.UrlEncode(vpc_Version) +
                "&vpc_Locale=" + HttpUtility.UrlEncode(vpc_Locale) +
                "&vpc_Command=" + HttpUtility.UrlEncode(vpc_Command) +
                "&vpc_Merchant=" + HttpUtility.UrlEncode(Merchant) +
                "&vpc_AccessCode=" + HttpUtility.UrlEncode(vpc_AccessCode) +
                "&vpc_MerchTxnRef=" + HttpUtility.UrlEncode(bankGateAPI.TransactionID.ToString()) +
                "&vpc_Amount=" + HttpUtility.UrlEncode((bankGateAPI.TotalAmount * 100).ToString()) +
                "&vpc_Currency=" + HttpUtility.UrlEncode(vpc_Currency) +
                "&vpc_OrderInfo=" + HttpUtility.UrlEncode(bankGateAPI.OrderNo) +
                "&vpc_ReturnURL=" + HttpUtility.UrlEncode(vpc_ReturnURL) +
                "&vpc_TicketNo=" + HttpUtility.UrlEncode(TicketNo) +
                "&vpc_BackURL=" + HttpUtility.UrlEncode(vpc_BackURL) +
                "&vpc_SecureHash=" + HttpUtility.UrlEncode(vpc_SecureHash);
        }


    }
}
