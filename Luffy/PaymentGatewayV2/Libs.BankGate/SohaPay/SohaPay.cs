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
using System.Collections;

namespace Libs.BankGate.SohaPay
{
    public class SohaPay
    {
        public long TransactionID { get; set; }             
        public string Site_code { get; set; }               
        public string Order_email { get; set; }             
        public string PaymentType { get; set; }             
        public string Language { get; set; }           
        public string Version { get; set; }            
        public string Order_product_title { get; set; }
        public string Response_code { get; set; }      
        public string Response_message { get; set; }   
        public string Payment_time { get; set; }       
        public string Error_text { get; set; }         
        public int ReturnValue { get; set; }

        private string UrlCheckOut = ConfigurationManager.AppSettings["SohaPay_UrlCheckOut"];
        private string Return_url = ConfigurationManager.AppSettings["SohaPay_Return_url"];
        private string Secure_secret = ConfigurationManager.AppSettings["SohaPay_Secure_secret"];

        public SohaPay()
        {
            Site_code = ConfigurationManager.AppSettings["SohaPay_Site_code"];
            Version = ConfigurationManager.AppSettings["SohaPay_Version"];
            Language = ConfigurationManager.AppSettings["SohaPay_Language"];
            PaymentType = ConfigurationManager.AppSettings["SohaPay_Payment_type"];
            Site_code = ConfigurationManager.AppSettings["SohaPay_Site_code"];
        }

        public SohaPay Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<SohaPay>("sp_pay_SohaPay_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public SohaPay Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<SohaPay>("sp_pay_SohaPay_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Site_code", Site_code);
            pars[3] = new SqlParameter("@Order_email", Order_email);
            pars[4] = new SqlParameter("@PaymentType", PaymentType);
            pars[5] = new SqlParameter("@Language", Language);
            pars[6] = new SqlParameter("@Version", Version);

            db.ExecuteNonQuerySP("sp_pay_SohaPay_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Order_product_title", Order_product_title);
            pars[3] = new SqlParameter("@Response_code", Response_code);
            pars[4] = new SqlParameter("@Response_message", Response_message);
            pars[5] = new SqlParameter("@Payment_time", Payment_time);
            pars[6] = new SqlParameter("@Error_text", Error_text);

            db.ExecuteNonQuerySP("sp_pay_SohaPay_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public string GetUrlCheckOut(BankGateAPI bankGateAPI)
        {
            string secure_hash = "language=" + Language +
                "&order_code=" + bankGateAPI.TransactionID.ToString() +
                "&order_email=" + Order_email +
                "&order_mobile=" + bankGateAPI.Mobile +
                "&payment_type=" + PaymentType +
                "&price=" + bankGateAPI.TotalAmount.ToString() +
                "&return_url=" + Return_url +
                "&site_code=" + Site_code +
                "&transaction_info=" + bankGateAPI.OrderInfo +
                "&version=" + Version;

            secure_hash = SHA256(Secure_secret, secure_hash);

            return UrlCheckOut +
                "?language=" + HttpUtility.UrlEncode(Language) +
                "&order_code=" + HttpUtility.UrlEncode(bankGateAPI.TransactionID.ToString()) +
                "&order_email=" + HttpUtility.UrlEncode(Order_email) +
                "&order_mobile=" + HttpUtility.UrlEncode(bankGateAPI.Mobile) +
                "&payment_type=" + HttpUtility.UrlEncode(PaymentType) +
                "&price=" + HttpUtility.UrlEncode(bankGateAPI.TotalAmount.ToString()) +
                "&return_url=" + HttpUtility.UrlEncode(Return_url) +
                "&site_code=" + HttpUtility.UrlEncode(Site_code) +
                "&transaction_info=" + HttpUtility.UrlEncode(bankGateAPI.OrderInfo) +
                "&version=" + HttpUtility.UrlEncode(Version) +
                "&secure_hash=" + HttpUtility.UrlEncode(secure_hash);
        }

        public bool VerifyReturnUrl()
        {
            string secure_hash = HttpContext.Current.Request["secure_code"];
            Hashtable param = new Hashtable();
            param["order_product_title"] = !string.IsNullOrEmpty(HttpContext.Current.Request["order_product_title"]) ? HttpContext.Current.Request["order_product_title"] : "";
            param["transaction_info"] = !string.IsNullOrEmpty(HttpContext.Current.Request["transaction_info"]) ? HttpContext.Current.Request["transaction_info"] : "";
            param["order_code"] = !string.IsNullOrEmpty(HttpContext.Current.Request["order_code"]) ? HttpContext.Current.Request["order_code"] : "";
            param["order_email"] = !string.IsNullOrEmpty(HttpContext.Current.Request["order_email"]) ? HttpContext.Current.Request["order_email"] : "";
            param["order_session"] = !string.IsNullOrEmpty(HttpContext.Current.Request["order_session"]) ? HttpContext.Current.Request["order_session"] : "";
            param["price"] = !string.IsNullOrEmpty(HttpContext.Current.Request["price"]) ? HttpContext.Current.Request["price"] : "";
            param["site_code"] = !string.IsNullOrEmpty(HttpContext.Current.Request["site_code"]) ? HttpContext.Current.Request["site_code"] : "";
            param["response_code"] = !string.IsNullOrEmpty(HttpContext.Current.Request["response_code"]) ? HttpContext.Current.Request["response_code"] : "";
            param["response_message"] = !string.IsNullOrEmpty(HttpContext.Current.Request["response_message"]) ? HttpContext.Current.Request["response_message"] : "";
            param["payment_type"] = !string.IsNullOrEmpty(HttpContext.Current.Request["payment_type"]) ? HttpContext.Current.Request["payment_type"] : "";
            param["payment_time"] = !string.IsNullOrEmpty(HttpContext.Current.Request["payment_time"]) ? HttpContext.Current.Request["payment_time"] : "";
            param["error_text"] = !string.IsNullOrEmpty(HttpContext.Current.Request["error_text"]) ? HttpContext.Current.Request["error_text"] : "";

            ArrayList keys = new ArrayList();
            keys.AddRange(param.Keys);
            keys.Sort();

            string secure_code = "";
            foreach (string k in keys)
            {
                if (k != "secure_code" && param[k].ToString().Length > 0)
                {
                    secure_code += k + "=" + param[k].ToString() + "&";
                }
            }

            secure_code = secure_code.TrimEnd('&');

            if (secure_hash.ToUpper() == SHA256(Secure_secret, secure_code))
            {
                return true;
            }

            return false;
        }

        string SHA256(string key, string data)
        {
            if ((key.Length % 2) == 1) key += '0';
            byte[] bytes = new byte[key.Length / 2];
            for (int i = 0; i < key.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(key.Substring(i, 2), 16);
            }

            var hmacsha256 = new HMACSHA256(bytes);
            byte[] hashValue = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            string hexHash = "";
            foreach (byte test in hashValue)
            {
                hexHash += test.ToString("X2");
            }
            return hexHash;
        }
    }
}
