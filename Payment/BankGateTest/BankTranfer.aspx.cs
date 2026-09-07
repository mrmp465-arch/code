using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        string urlService = "http://45.32.115.186:1592//VPGJsonService.ashx";
        //string urlService = "http://localhost:61870/VPGJsonService.ashx"; 
        string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
        string partnerCode = "pp";
        string serviceCode = "bankdirect";
        string commandCode = "getbanks";

        public List<BankAccount> Banks
        {
            get { return (List<BankAccount>)ViewState["Results"]; }
            set { ViewState["Results"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!Page.IsPostBack)
                //Init();
                //inti2();
        }
        private void inti2()
        {
            string commandCode = "getbanks";
            var requestContent = serializer.Serialize(new GetBankRequest() { Type = "banktranfer", AccountName = "mrx" });
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
            //var BanksV2 = serializer.Deserialize<List<BankAccountV2>>(resObj.ResponseContent);
            //Banks = new List<BankAccount>();
        }
        private void Init()
        {
            string commandCode = "order";

            var requestContent = serializer.Serialize(new OrderRequest()
            {
                Type = "banktranfer",
                AccountName = "toitest",
                Amount = 100000,
                AppCode = "App1",
                CallbackUrl = "https://bankgate.coroach.xyz/VPGJsonService.ashx",
                RefCode = "RTxxx0016",
                BankName = "random",
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
                BankName = "random",
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
            url = orderResponse.LinkWebView;
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
            public string AccountName { get; set; }
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
            public string LinkWebView { get; set; }
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