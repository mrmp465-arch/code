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
using System.Web.Script.Serialization;

namespace Libs.BankGate.Banknet
{
    public class BankNet
    {
        public long TransactionID { get; set; }         
        public string Merchant_trans_id { get; set; }       // Mã giao dịch của Đại lý  – gồm các ký tự dạng số, chiều dài 6 (ví dụ: 000123)
        public string Merchant_code { get; set; }           // Mã code của Đại lý
        public string Selected_bank { get; set; }           // Mã ngân hàng
        public string Service_code { get; set; }            // Mã dịch vụ thanh toán
        public string Trans_id { get; set; }                // Mã giao dịch của hệ thống BankNet
        public string UrlCheckOut { get; set; }             
        public string ResponseCode { get; set; }
        public string Url_success { get; set; }
        public string Url_fail { get; set; }
        public int ReturnValue { get; set; }

        private string WebserviceUrl = ConfigurationManager.AppSettings["BankNet_WebserviceUrl"];
        //private string Merchant_trans_key = ConfigurationManager.AppSettings["BankNet_MerchantTransKey"];
        private string Merchant_trans_key = "ofuodj#$AHB54@&pdhmn";
        private string BankNetUrlReturn = ConfigurationManager.AppSettings["BankNet_UrlReturn"];
        private string Country_code = "vn";
        private string PrivateKey = "8b80027f460BC9b2ef1d677adE4f3b02";

        public BankNet()
        {
            Merchant_code = ConfigurationManager.AppSettings["BankNet_MerchantCode"];
            Service_code = "720";
        }

        public BankNet Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankNet>("sp_pay_Banknet_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public BankNet Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankNet>("sp_pay_Banknet_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Merchant_trans_id", Merchant_trans_id);
            pars[3] = new SqlParameter("@Merchant_code", Merchant_code);
            pars[4] = new SqlParameter("@Selected_bank", Selected_bank);
            pars[5] = new SqlParameter("@Service_code", Service_code);
            pars[6] = new SqlParameter("@Trans_id", Trans_id);
            pars[7] = new SqlParameter("@UrlCheckOut", UrlCheckOut);

            db.ExecuteNonQuerySP("sp_pay_Banknet_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Trans_id", Trans_id);
            pars[3] = new SqlParameter("@UrlCheckOut", UrlCheckOut);
            pars[4] = new SqlParameter("@ResponseCode", ResponseCode);

            db.ExecuteNonQuerySP("sp_pay_Banknet_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public string GetUrlCheckOut(BankGateAPI bankGateAPI, string bankCode)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string Merchant_trans_id = bankGateAPI.TransactionID.ToString("D6");
                if (Merchant_trans_id.Length > 0) Merchant_trans_id = Merchant_trans_id.Substring(Merchant_trans_id.Length - 6);

                string Good_code = bankGateAPI.TransactionID.ToString();
                string Xml_description = bankGateAPI.OrderInfo;
                string Net_cost = bankGateAPI.TotalAmount.ToString();
                string Ship_fee = "0";
                string Tax = "0";
                string Selected_bank = bankCode;
                Url_success = UrlReturn(bankGateAPI.TransactionID, 1);
                Url_fail = UrlReturn(bankGateAPI.TransactionID, -1);
                string Trans_key = Encrypts.MD5(Merchant_trans_id + Merchant_code + Good_code + Net_cost + Ship_fee + Tax + Merchant_trans_key);

                PaymentGateway _PaymentGateway = new PaymentGateway(WebserviceUrl);
                NLogLogger.Info(new string[] { "BankGate", bankGateAPI.TransactionID.ToString(), "BankNet", "Request", Merchant_trans_id, Merchant_code, Country_code, Good_code, Xml_description, Net_cost, Ship_fee, Tax, Url_success, Url_fail, Trans_key, Selected_bank, Service_code });
                string s = _PaymentGateway.Send_GoodInfo_Ext2(Merchant_trans_id, Merchant_code, Country_code, Good_code, Xml_description, Net_cost, Ship_fee, Tax, Url_success, Url_fail, Trans_key, Selected_bank, Service_code);
                NLogLogger.Info(new string[] { "BankGate", bankGateAPI.TransactionID.ToString(), "BankNet", "Webservice", s });

                string[] list = s.Split('|');

                if (list[0] == "010")
                {
                    // Nếu thành công
                    int lenURL = Convert.ToInt32(list[1]);
                    string url = list[2].Substring(0, lenURL);
                    string sign = list[2].Substring(lenURL);

                    BankNet _BankNet = new BankNet();
                    _BankNet.TransactionID = bankGateAPI.TransactionID;
                    _BankNet.Merchant_trans_id = Merchant_trans_id;
                    _BankNet.Selected_bank = bankCode;
                    _BankNet.Trans_id = url.Substring(url.IndexOf("=") + 1);
                    _BankNet.UrlCheckOut = url;
                    _BankNet.Add();

                    // Sai chữ ký
                    if (sign != Encrypts.MD5("010" + lenURL.ToString() + url + Merchant_trans_key))
                    {
                        return ""; 
                    }
                    else
                    {
                        return url;
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankGate", bankGateAPI.TransactionID.ToString(), "Error", "BankNet", ex.Message.Replace("\n", " ") });
                return "";
            }
        }

        public bool ConfirmTransaction(long transactionID, bool status)
        {
            BankNet _BankNet = new BankNet();
            _BankNet = _BankNet.Get(transactionID);

            if (_BankNet == null) return false;

            PaymentGateway _PaymentGateway = new PaymentGateway(WebserviceUrl);
            string Merchant_trans_id = _BankNet.Merchant_trans_id;
            string Trans_id = _BankNet.Trans_id;
            string Trans_result = status ? "0" : "1";
            string Trans_key = Encrypts.MD5(Merchant_trans_id + Trans_id + Merchant_code + Trans_result + Merchant_trans_key);
            NLogLogger.Info(new string[] { "BankGate", "BankNet", "ConfirmTransactionResult", Merchant_trans_id, Trans_id, Merchant_code, Trans_result, Trans_key });
            string s = _PaymentGateway.ConfirmTransactionResult(Merchant_trans_id, Trans_id, Merchant_code, Trans_result, Trans_key);
            NLogLogger.Info(new string[] { "BankGate", "BankNet", "ConfirmTransactionResult", s });
            string[] list = s.Split('|');

            if (list[0] != "00" || list[1] != "310") return false;

            return true;
        }

        public string UrlReturn(long transactionID, int status)
        {
            string transactionTime = DateTime.UtcNow.ToString("yyMMddHHmmss");
            string sign = Encrypts.MD5(transactionID.ToString() + status.ToString() + transactionTime + PrivateKey);
            return BankNetUrlReturn + string.Format("?transactionid={0}&status={1}&transactiontime={2}&sign={3}", transactionID, status, transactionTime, sign);
        }

        public bool CheckUrlReturn(long transactionID, int status, long transactionTime, string sign)
        {
            if (transactionID < 0) return false;

            if (status != 1 && status != -1) return false;

            long minTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(-30).ToString("yyMMddHHmmss"));
            long maxTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(30).ToString("yyMMddHHmmss"));
            if (transactionTime < minTime || transactionTime > maxTime) return false;

            if (sign != Encrypts.MD5(transactionID.ToString() + status.ToString() + transactionTime.ToString() + PrivateKey)) return false;

            return true;
        }

    }

    public class BankNetInfo
    {
        public long TransactionID { get; set; }
        public int Status { get; set; }
        public long TransactionTime { get; set; }
    }
}
