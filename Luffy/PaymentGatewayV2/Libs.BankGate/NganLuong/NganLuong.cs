using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using Libs.Db;
using Libs.Utils;
using System.Net;
using System.IO;
using System.Xml;

namespace Libs.BankGate.NganLuong
{
    public class NganLuong
    {
        public long TransactionID { get; set; }             // Mã giao dịch
        public int ReturnValue { get; set; }
        public string Merchant_id { get; set; }
        public string Version { get; set; }
        public string Function { get; set; }
        public string Receiver_email { get; set; }
        public string Payment_method { get; set; }
        public string Payment_type { get; set; }
        public string Bank_code { get; set; }
        public string Tax_amount { get; set; }
        public string Discount_amount { get; set; }
        public string Fee_shipping { get; set; }
        public string Return_url { get; set; }
        public string Cancel_url { get; set; }
        public string Buyer_email { get; set; }
        public string Error_code { get; set; }
        public string Token { get; set; }
        public string Checkout_url { get; set; }
        public string Description { get; set; }

        private string Time_limit { get; set; }
        private string Merchant_password = ConfigurationManager.AppSettings["NganLuong_Merchant_Password"];
        private string UrlCheckOut = ConfigurationManager.AppSettings["NganLuong_UrlCheckOut"];

        public NganLuong()
        {
            Merchant_id = ConfigurationManager.AppSettings["NganLuong_Merchant_id"];
            Version = ConfigurationManager.AppSettings["NganLuong_Version"];
            Function = ConfigurationManager.AppSettings["NganLuong_Function"];
            Receiver_email = ConfigurationManager.AppSettings["NganLuong_Receiver_email"];
            Payment_method = ConfigurationManager.AppSettings["NganLuong_Payment_method"];
            Payment_type = ConfigurationManager.AppSettings["NganLuong_Payment_type"];
            Return_url = ConfigurationManager.AppSettings["NganLuong_Return_url"];
            Cancel_url = ConfigurationManager.AppSettings["NganLuong_Cancel_url"];

            Time_limit = ConfigurationManager.AppSettings["NganLuong_Time_limit"];
            Tax_amount = "0";
            Fee_shipping = "0";
            Discount_amount = "0";
            Bank_code = "";
        }

        public NganLuong(long transactionID)
        {
            Merchant_id = ConfigurationManager.AppSettings["NganLuong_Merchant_id"];
            Version = ConfigurationManager.AppSettings["NganLuong_Version"];
            Function = ConfigurationManager.AppSettings["NganLuong_Function"];
            Receiver_email = ConfigurationManager.AppSettings["NganLuong_Receiver_email"];
            Payment_method = ConfigurationManager.AppSettings["NganLuong_Payment_method"];
            Payment_type = ConfigurationManager.AppSettings["NganLuong_Payment_type"];
            Return_url = ConfigurationManager.AppSettings["NganLuong_Return_url"];
            Cancel_url = ConfigurationManager.AppSettings["NganLuong_Cancel_url"];

            Time_limit = ConfigurationManager.AppSettings["NganLuong_Time_limit"];
            Tax_amount = "0";
            Fee_shipping = "0";
            Discount_amount = "0";
            Bank_code = "";

            TransactionID = transactionID;
            Buyer_email = TransactionID.ToString() + "@vgg.vn";
            Buyer_email = "vietha.le@vgg.vn";
        }

        public NganLuong Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<NganLuong>("sp_pay_NganLuong_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public NganLuong Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<NganLuong>("sp_pay_NganLuong_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[16];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Merchant_id", Merchant_id);
            pars[3] = new SqlParameter("@Version", Version);
            pars[4] = new SqlParameter("@Function", Function);
            pars[5] = new SqlParameter("@Receiver_email", Receiver_email);
            pars[6] = new SqlParameter("@Payment_method", Payment_method);
            pars[7] = new SqlParameter("@Payment_type", Payment_type);
            pars[8] = new SqlParameter("@Tax_amount", Tax_amount);
            pars[9] = new SqlParameter("@Discount_amount", Discount_amount);
            pars[10] = new SqlParameter("@Fee_shipping", Fee_shipping);
            pars[11] = new SqlParameter("@Return_url", Return_url);
            pars[12] = new SqlParameter("@Cancel_url", Cancel_url);
            pars[13] = new SqlParameter("@Time_limit", Time_limit);
            pars[14] = new SqlParameter("@Buyer_email", Buyer_email);
            pars[15] = new SqlParameter("@Bank_code", Bank_code);

            db.ExecuteNonQuerySP("sp_pay_NganLuong_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Error_code", Error_code);
            pars[3] = new SqlParameter("@Token", Token);
            pars[4] = new SqlParameter("@Checkout_url", Checkout_url);
            pars[5] = new SqlParameter("@Description", Description);

            db.ExecuteNonQuerySP("sp_pay_NganLuong_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public string GetParamPost(BankGateAPI bankGateAPI)
        {
            String request = "";

            request += "function=" + Function;
            request += "&cur_code=" + "";
            request += "&version=" + Version;
            request += "&merchant_id=" + Merchant_id;
            request += "&receiver_email=" + Receiver_email;
            request += "&merchant_password=" + Encrypts.MD5(Merchant_password);
            request += "&order_code=" + bankGateAPI.TransactionID;
            request += "&total_amount=" + bankGateAPI.TotalAmount;
            request += "&payment_method=" + Payment_method;
            request += "&bank_code=" + Bank_code;
            request += "&payment_type=" + Payment_type;
            request += "&order_description=" + bankGateAPI.OrderInfo;
            request += "&tax_amount=" + Tax_amount;
            request += "&fee_shipping=" + Fee_shipping;
            request += "&discount_amount=" + Discount_amount;
            request += "&return_url=" + Return_url;
            request += "&cancel_url=" + Cancel_url;
            request += "&buyer_fullname=" + bankGateAPI.FullName;
            request += "&buyer_email=" + Buyer_email;
            request += "&buyer_mobile=" + bankGateAPI.Mobile;

            return request;
        }

        public string HttpPost(string postData)
        {

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);

            // Prepare web request...
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(UrlCheckOut);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = data.Length;
            Stream newStream = myRequest.GetRequestStream();
            // Send the data.

            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string output = reader.ReadToEnd();
            response.Close();
            return output;
        }

        public NganLuong GetUrlCheckout(NganLuong nganLuong, BankGateAPI bankGateAPI)
        {

            string requestinfo = nganLuong.GetParamPost(bankGateAPI);
            string result = nganLuong.HttpPost(requestinfo);
            result = result.Replace("&", "&amp;");
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(result);
            XmlNodeList root = dom.DocumentElement.ChildNodes;

            nganLuong.Checkout_url = root.Item(4).InnerText;
            nganLuong.Description = root.Item(2).InnerText;
            nganLuong.Error_code = root.Item(0).InnerText;
            nganLuong.Token = root.Item(1).InnerText;

            return nganLuong;
        }

        // Lấy thông tin giao dịch
        public ResponseCheckOrder GetTransactionDetail(string token)
        {


            String request = "";
            request += "function=" + "GetTransactionDetail";
            request += "&version=" + Version;
            request += "&merchant_id=" + Merchant_id;
            request += "&merchant_password=" + Encrypts.MD5(Merchant_password);
            request += "&token=" + token;
            String result = HttpPost(request);
            result = result.Replace("&", "&amp;");
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(result);
            XmlNodeList root = dom.DocumentElement.ChildNodes;

            ResponseCheckOrder objResult = new ResponseCheckOrder();


            objResult.errorCode = root.Item(0).InnerText;
            objResult.token = root.Item(1).InnerText;
            objResult.description = root.Item(2).InnerText;
            objResult.transactionStatus = root.Item(3).InnerText;
            objResult.order_code = root.Item(5).InnerText;
            objResult.paymentAmount = root.Item(6).InnerText; //total_amount
            objResult.payerName = root.Item(16).InnerText; //buyer_fullname
            objResult.payerEmail = root.Item(17).InnerText; //buyer_email
            objResult.payerMobile = root.Item(18).InnerText; //buyer_mobile
            objResult.transactionId = root.Item(21).InnerText;

            return objResult;
        }

        public string NganLuongBankCode(string name)
        {
            switch (name)
            {
                case "vietcombank":
                    return "VCB";
                case "techcombank":
                    return "TCB";
                case "vib":
                    return "VIB";
                case "eximbank":
                    return "EXB";
                case "vietinbank":
                    return "VTB";
                case "mb":
                    return "MB";
                case "hdbank":
                    return "HDB";
                case "maritimebank":
                    return "MSB";
                case "acb":
                    return "ACB";
                case "sacombank":
                    return "SCB";
                case "navibank":
                    return "NVB";
                case "vietabank":
                    return "VAB";
                case "vpbank":
                    return "VPB";
                case "gpbank":
                    return "GPB";
                case "oceanbank":
                    return "OJB";
                case "dongabank":
                    return "DAB";
                case "daiabank":
                case "tienphongbank":
                    return "TPB";
                case "agribank":
                    return "AGB";
                case "abbank":
                case "saigonbank":
                case "namabank":
                case "phuongdongbank":
                case "bacabank":
                default:
                    return "";
            }
//BIDV	Ngân hàng Đầu tư và Phát triển Việt Nam (BIDV)
//PGB	Ngân Hàng TMCP Xăng Dầu Petrolimex (PGBank)
//SHB	Ngân hàng TMCP Sài Gòn - Hà Nội (SHB)
//SB	Ngân hàng TMCP Đông Nam Á (SeaBank)
        }

    }

    public class ResponseCheckOrder
    {
        private string error_code = string.Empty;

        public string errorCode
        {
            get { return error_code; }
            set { error_code = value; }
        }
        private string error_description = string.Empty;

        public string description
        {
            get { return error_description; }
            set { error_description = value; }
        }
        private string time_limit = string.Empty;

        public string timeLimit
        {
            get { return time_limit; }
            set { time_limit = value; }
        }
        private string _token = string.Empty;

        public string token
        {
            get { return _token; }
            set { _token = value; }
        }
        private string transaction_id = string.Empty;

        public string transactionId
        {
            get { return transaction_id; }
            set { transaction_id = value; }
        }
        private string amount = string.Empty;

        public string paymentAmount
        {
            get { return amount; }
            set { amount = value; }
        }
        private string _order_code = string.Empty;

        public string order_code
        {
            get { return _order_code; }
            set { _order_code = value; }
        }
        private string transaction_type = string.Empty;

        public string transactionType
        {
            get { return transaction_type; }
            set { transaction_type = value; }
        }
        private string transaction_status = string.Empty;

        public string transactionStatus
        {
            get { return transaction_status; }
            set { transaction_status = value; }
        }
        private string payer_name = string.Empty;

        public string payerName
        {
            get { return payer_name; }
            set { payer_name = value; }
        }
        private string payer_email = string.Empty;

        public string payerEmail
        {
            get { return payer_email; }
            set { payer_email = value; }
        }
        private string payer_mobile = string.Empty;

        public string payerMobile
        {
            get { return payer_mobile; }
            set { payer_mobile = value; }
        }
        private string receiver_name = string.Empty;

        public string merchantName
        {
            get { return receiver_name; }
            set { receiver_name = value; }
        }
        private string receiver_address = string.Empty;

        public string merchantAddress
        {
            get { return receiver_address; }
            set { receiver_address = value; }
        }
        private string receiver_mobile = string.Empty;

        public string merchantMobile
        {
            get { return receiver_mobile; }
            set { receiver_mobile = value; }
        }
        private string payment_method = string.Empty;

        public string paymentMethod
        {
            get { return payment_method; }
            set { payment_method = value; }
        }
    }
}
