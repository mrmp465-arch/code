using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Db;
using Libs.Utils;

namespace Libs.BankGate.PiPay
{
    public class PiPay
    {
        public string urlcheckout = ConfigurationManager.AppSettings["PiPay_UrlCheckOut"] ?? "https://onlinepayment-test.pipay.com/starttransaction";
        public string urlverify = ConfigurationManager.AppSettings["PiPay_UrlVerify"] ?? "https://onlinepayment-test.pipay.com/rest-api/verifyTransaction";
        private string midConfig = ConfigurationManager.AppSettings["PiPay_MID"] ?? "101303";
        private string sidConfig = ConfigurationManager.AppSettings["PiPay_SID"] ?? "2633";
        private string didConfig = ConfigurationManager.AppSettings["PiPay_DID"] ?? "3511";
        private string confirmUrl = ConfigurationManager.AppSettings["PiPay_ConfirmUrl"] ?? "http://96.9.75.2:1582/Pages/PiPay/PiPayReturn.aspx";
        private string cancelUrl = ConfigurationManager.AppSettings["PiPay_cancelUrl"] ?? "http://96.9.75.2:1582/Pages/PiPay/PiPayCancel.aspx";
        public string providerCode = "pipay";

        public long TransactionID { get; set; }
        public int mid { get; set; }
        public string lang { get; set; }
        public long orderId { get; set; }
        public string orderDesc { get; set; }
        public decimal orderAmount { get; set; }
        public string currency { get; set; }
        public string payerPhone { get; set; }
        public string trType { get; set; }
        public string payMethod { get; set; }
        public string confirmURL { get; set; }
        public string cancelURL { get; set; }
        public string var1 { get; set; }
        public int sid { get; set; }
        public string did { get; set; }
        public System.DateTime orderDate { get; set; }
        public string status { get; set; }
        public string transID { get; set; }
        public string processorID { get; set; }
        public System.DateTime lastTime { get; set; }
        public long ReturnValue { get; set; }

        public string GetForm(BankGateAPI _bankGateApi)
        {

            var pipay = new PiPay()
            {
                TransactionID = _bankGateApi.TransactionID,
                mid = Convert.ToInt32(midConfig),
                lang = "en",
                orderId = _bankGateApi.TransactionID,
                orderDesc = _bankGateApi.OrderInfo,
                orderAmount = _bankGateApi.TotalAmount,
                currency = "USD",
                payerPhone = _bankGateApi.Mobile,
                trType = "1",
                payMethod = "wallet",
                confirmURL = confirmUrl,
                cancelURL = cancelUrl,
                var1 = _bankGateApi.OrderNo,
                sid = Convert.ToInt32(sidConfig),
                did = didConfig,
                orderDate = DateTime.Now
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[] { "BankGate", "PiPay", "Data", serializer.Serialize(pipay) });
            // Add DB Pipay Log
            var result = pipay.Add();
            if (result == 0)
            {
                var str = string.Format(
                    @"<input type='hidden' name='mid' value='{0}'/>
                    <input type='hidden' name='lang' value='{1}'/>
                    <input type='hidden' name='orderid' value='{2}'/>
                    <input type='hidden' name='orderDesc' value='{3}'/>
                    <input type='hidden' name='orderAmount' value='{4}'/>
                    <input type='hidden' name='currency' value='{5}'/>
                    <input type='hidden' name='payerPhone ' value='{6}'/>
                    <input type='hidden' name='sid' value='{7}'/>
                    <input type='hidden' name='did' value='{8}'/>
                    <input type='hidden' name='orderDate' value='{9}'/>            
                    <input type='hidden' name='payMethod' value='{10}'/>            
                    <input type='hidden' name='trType' value='{11}'/>            
                    <input type='hidden' name='confirmURL' value='{12}'/>
                    <input type='hidden' name='cancelURL' value = '{13}' / >
                    <input type='hidden' name='var1' value='{14}'/>
                    <input type='hidden' name='digest' value='{15}'/>
                    <input type='hidden' name='digest' value='0'/>
            ", pipay.mid, pipay.lang, pipay.orderId, pipay.orderDesc, pipay.orderAmount, pipay.currency,
                    pipay.payerPhone, pipay.sid, pipay.did, pipay.orderDate.ToString("yyyy-mm-dd hh:mm:ss"), pipay.payMethod, pipay.trType, pipay.confirmURL, pipay.cancelURL, pipay.var1,
                    Encrypts.MD5(pipay.mid.ToString() + pipay.orderId.ToString() + pipay.orderAmount.ToString()));

                return str;
            }
            else
            {
                return string.Empty;
            }

        }

        public string PostJson(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST"; //GET
            //request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.7) Gecko/20070914 Firefox/2.0.0.7";
            request.Timeout = 30000;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = postDatabytes.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(postDatabytes, 0, postDatabytes.Length);
            var webResponse = request.GetResponse();
            dataStream = webResponse.GetResponseStream();
            if (dataStream == null)
            {
                webResponse.Close();
                return string.Empty;
            }

            var sr = new StreamReader(dataStream);
            var response = sr.ReadToEnd().Trim();

            sr.Close();
            dataStream.Close();
            webResponse.Close();

            return response;
        }

        public long Add()
        {

            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[16];
            pars[0] = new SqlParameter("@ReturnValue", ReturnValue) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@mid", mid);
            pars[3] = new SqlParameter("@lang", lang);
            pars[4] = new SqlParameter("@orderId", orderId);
            pars[5] = new SqlParameter("@orderDesc", orderDesc);
            pars[6] = new SqlParameter("@orderAmount", orderAmount);
            pars[7] = new SqlParameter("@currency", currency);
            pars[8] = new SqlParameter("@trType", trType);
            pars[9] = new SqlParameter("@payMethod", payMethod);
            pars[10] = new SqlParameter("@confirmURL", confirmURL);
            pars[11] = new SqlParameter("@cancelURL", cancelURL);
            pars[12] = new SqlParameter("@var1", var1);
            pars[13] = new SqlParameter("@sid", sid);
            pars[14] = new SqlParameter("@did", did);
            pars[15] = new SqlParameter("@orderDate", orderDate);
            db.ExecuteNonQuerySP("sp_pay_PiPay_Insert", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;

        }

        public PiPay Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<PiPay>("sp_pay_PiPay_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public long Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[19];
            pars[0] = new SqlParameter("@ReturnValue", ReturnValue) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@mid", mid);
            pars[3] = new SqlParameter("@lang", lang);
            pars[4] = new SqlParameter("@orderId", orderId);
            pars[5] = new SqlParameter("@orderDesc", orderDesc);
            pars[6] = new SqlParameter("@orderAmount", orderAmount);
            pars[7] = new SqlParameter("@currency", currency);
            pars[8] = new SqlParameter("@trType", trType);
            pars[9] = new SqlParameter("@payMethod", payMethod);
            pars[10] = new SqlParameter("@confirmURL", confirmURL);
            pars[11] = new SqlParameter("@cancelURL", cancelURL);
            pars[12] = new SqlParameter("@var1", var1);
            pars[13] = new SqlParameter("@sid", sid);
            pars[14] = new SqlParameter("@did", did);
            pars[15] = new SqlParameter("@orderDate", orderDate);
            pars[16] = new SqlParameter("@status", status);
            pars[17] = new SqlParameter("@transID", transID);
            pars[18] = new SqlParameter("@processorID", processorID);
            db.ExecuteNonQuerySP("sp_pay_PiPay_Update", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }

    }


}
