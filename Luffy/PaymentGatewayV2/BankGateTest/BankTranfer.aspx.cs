using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Utils;

namespace BankGateTest
{
    public partial class BankTranfer : System.Web.UI.Page
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public string url = "";
        string urlService = "https://bank.kudopay.xyz/GetBank.ashx";
        string urlServicem = "https://bank.kudopay.xyz/GetBank.ashx";
        string urlService2 = "https://bank.kudopay.xyz/BankRequest.ashx";
        string urlService3 = "https://bank.namipay.xyz/UsdtRequest.ashx";
        //string urlService = "http://localhost:61870/VPGJsonService.ashx"; 
        string partnerKey = "e1a8d8ca47e0d70990fcef27ee267ced";
        string partnerCode = "dcp";
        string serviceCode = "bankdirect";
        string commandCode = "getbanks";

        public List<BankAccount> Banks
        {
            get { return (List<BankAccount>)ViewState["Results"]; }
            set { ViewState["Results"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", CalcHMACSHA256Hash("4|100000|hynbank12345", "hynbank56789") });
                //UsdtRequest();
                BankRequest();
                //Init();
                //NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", DateTime.Now.DayOfWeek.ToString() });
            }    
                //BankRequest();
                //Init();
        }
        public int GetWeekNumber()
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }
        public static string CalcHMACSHA256Hash(string plaintext, string salt)
        {
            string result = "";
            var enc = Encoding.Default;
            byte[]
            baText2BeHashed = enc.GetBytes(plaintext),
            baSalt = enc.GetBytes(salt);
            System.Security.Cryptography.HMACSHA256 hasher = new HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }
        public  string HmacSha256Digest(string message, string secretKey)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
            byte[] bytes = cryptographer.ComputeHash(messageBytes);
            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
            return base64String;
        }
        public static string HmacSha256DigestV2(string message, string secretKey)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
            byte[] bytes = cryptographer.ComputeHash(messageBytes);
            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
            return base64String;
        }
        public class RequestDataV3
        {
            public string PartnerCode { get; set; }
            public string BankCode { get; set; }
            public string AccountName { get; set; }
            public int Amount { get; set; }
            public string RefCode { get; set; }
            public string Signature { get; set; }

        }
        private void BankRequest()
        {
            var request = new RequestDataV3()
            {
                PartnerCode = "dcp",
                AccountName = "mrx",
                BankCode = "ACB",
                Amount = 20000,
                RefCode = "4cdcc495-caa3",

            };
            request.Signature = Encrypts.MD5(partnerCode + request.BankCode + request.Amount + request.RefCode + partnerKey);
            NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", urlService2,serializer.Serialize(request) });
            var serviceResponse = PostJson(urlService2, serializer.Serialize(request));
            NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", serviceResponse });
        }
        private void UsdtRequest()
        {
            var request = new RequestDataV3()
            {
                PartnerCode = "dcp",
                AccountName = "mrx1111xxxxxxx",
                BankCode = "bep20",
                Amount = 1000,
                RefCode = "RT112",

            };
            request.Signature = Encrypts.MD5(partnerCode + request.BankCode + request.Amount + request.RefCode + partnerKey);
            NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", urlService3, serializer.Serialize(request) });
            var serviceResponse = PostJson(urlService3, serializer.Serialize(request));
            NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", serviceResponse });
        }
        private void Init()
        {


            //string commandCode = "order";

            //var requestContent = serializer.Serialize(new OrderRequest()
            //{
            //    Type = ddlType.SelectedValue,
            //    AccountName = "toitest",
            //    Amount = Convert.ToInt32(txtVND.Text),
            //    AppCode = "App1",
            //    CallbackUrl = "https://appxxx.info/callback/",
            //    RefCode = "RT001",
            //    BankName = ddlBanks.SelectedValue.ToUpper(),
            //    //BankAccountName = "",
            //    //BankAccountNumber = ""
            //});
            var requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var signature = Encrypts.MD5(partnerCode + requestTime + partnerKey);
            var requestData = new RequestDataV2()
            {
                PartnerCode = partnerCode,
                RequestTime = requestTime,
               
                Signature = signature
            };
            NLogLogger.Info(new string[] { "Bank Test", "Requst Core", urlServicem, serializer.Serialize(requestData) });
            var serviceResponse = PostJson(urlServicem, serializer.Serialize(requestData));
            NLogLogger.Info(new string[] { "Bank Test", "Response Core", serviceResponse });
            var resObj = serializer.Deserialize<APIResponse>(serviceResponse);

            txtAmoutTranfer.Text = serviceResponse;
        }

        protected void CheckOut_Click(object sender, EventArgs e)
        {

            //txtBankName.Text = ddlBanks.SelectedItem.Text;
            //txtAccountNumber.Text = Banks.Single(s => s.BankName == ddlBanks.SelectedItem.Text).BankAccountNumber;
            //txtAccountName.Text = Banks.Single(s => s.BankName == ddlBanks.SelectedItem.Text).BankAccountName;
            //txtAmoutTranfer.Text = txtVND.Text + " VNĐ";


            string commandCode = "order";

            var requestContent = serializer.Serialize(new OrderRequest()
            {
                Type = ddlType.SelectedValue,
                AccountName = "toitest",
                Amount = Convert.ToInt32(txtVND.Text),
                AppCode = "App1",
                CallbackUrl = "https://appxxx.info/callback/",
                RefCode = "RT001",
                BankName = ddlBanks.SelectedValue.ToUpper(),
                //BankAccountName = "",
                //BankAccountNumber = ""
            });
            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            var requestData = new RequestData()
            {
                PartnerCode = partnerCode,
                CommandCode = commandCode,
                RequestContent = requestContent,
                ServiceCode = serviceCode,
                Signature = signature
            };
            NLogLogger.Info(new string[] { "Bank Test", "Requst Core", urlService, serializer.Serialize(requestData) });
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
            NLogLogger.Info(new string[] { "Bank Test", "Response Core", serviceResponse });
            var resObj = serializer.Deserialize<APIResponse>(serviceResponse);

            var orderResponse = serializer.Deserialize<OrderResponse>(resObj.ResponseContent);

            pnSt1.Visible = false;
            pnSt3.Visible = true;
            url = orderResponse.Url;
            lblWarning.Visible = true;
            CheckOut.Visible = false;
            CheckTran.Visible = true;

            //txtBankName.Text = ddlBanks.SelectedItem.Text;
            //txtAccountNumber.Text = Banks.Single(s => s.BankName == ddlBanks.SelectedItem.Text).AccountNumber;
            //txtAccountName.Text = Banks.Single(s => s.BankName == ddlBanks.SelectedItem.Text).AccountName;
            //txtAmoutTranfer.Text = txtVND.Text + " VNĐ";

            //txtBankName.Text = orderResponse.BankName;
            //txtAccountNumber.Text = orderResponse.BankAccountNumber;
            //txtAccountName.Text = orderResponse.BankAccountName;
            //txtAmoutTranfer.Text = orderResponse.Amount + " VNĐ";

            //txtReason.Text = orderResponse.OrderNo;

        }

        private string PostJson(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
                                    //request.Accept = "JSON";
            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(postDatabytes, 0, postDatabytes.Length);
            }
            var webResponse = request.GetResponse();
            if (webResponse == null)
            {
                return "Unable to connect to the remote server";
            }
            var sr = new StreamReader(webResponse.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }



        public class GetBankRequest
        {
            public string Type { get; set; }

        }

        public class OrderRequest
        {
            public string Type { get; set; }
            public string AccountName { get; set; }
            public string BankName { get; set; }
            public string BankAccountNumber { get; set; }
            public string BankAccountName { get; set; }
            public string AppCode { get; set; }
            public string RefCode { get; set; }
            public int Amount { get; set; }
            public string CallbackUrl { get; set; }
        }
        public class RequestDataV2
        {
            public string PartnerCode { get; set; }
            public string RequestTime { get; set; }

            public string Signature { get; set; }

        }
        public class RequestData
        {
            public string PartnerCode { get; set; }
            public string ServiceCode { get; set; }
            public string CommandCode { get; set; }
            public string RequestContent { get; set; }
            public string Signature { get; set; }
        }



        public class APIResponse
        {
            public int ResponseCode { get; set; }
            public string Description { get; set; }
            public string ResponseContent { get; set; }
            public string Signature { get; set; }
        }

        class OrderResponse
        {
            public string Status { get; set; }
            public string BankName { get; set; }
            public string Url { get; set; }
            //public string BankAccountName { get; set; }
            public int Amount { get; set; }
            public string OrderNo { get; set; }
            public int Timeout { get; set; }
        }

        [Serializable]
        public class BankAccount
        {
            public int Id { get; set; }
            public string BankName { get; set; }
            public string BankAccountNumber { get; set; }
            public string BankAccountName { get; set; }
        }
        public class BankAccountV2
        {
            public string BankName { get; set; }
            public string Name { get; set; }
        }
        protected void CheckTran_Click(object sender, EventArgs e)
        {

        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlType.SelectedValue == "banktranfer")
            {
                string commandCode = "getbanks";
                var requestContent = serializer.Serialize(new GetBankRequest() { Type = ddlType.SelectedValue });
                var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
                var requestData = new RequestData()
                {
                    PartnerCode = partnerCode,
                    CommandCode = commandCode,
                    RequestContent = requestContent,
                    ServiceCode = serviceCode,
                    Signature = signature
                };

                var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
                var resObj = serializer.Deserialize<APIResponse>(serviceResponse);
                var BanksV2 = serializer.Deserialize<List<BankAccountV2>>(resObj.ResponseContent);
                Banks = new List<BankAccount>();
                foreach (var item in BanksV2)
                {
                    Banks.Add(new BankAccount { BankName = item.Name, BankAccountNumber = item.BankName });
                }
                ddlBanks.DataSource = Banks;
                ddlBanks.DataValueField = "BankAccountNumber";
                ddlBanks.DataTextField = "BankName";
                ddlBanks.DataBind();
            }

            if (ddlType.SelectedValue == "wallet")
            {
                Banks = new List<BankAccount>();
                ddlBanks.DataSource = Banks;
                ddlBanks.DataValueField = "BankAccountNumber";
                ddlBanks.DataTextField = "BankName";
                ddlBanks.DataBind();

                ddlBanks.Items.Insert(0, new ListItem("MOMO", "MOMO"));
            }

        }
    }
}