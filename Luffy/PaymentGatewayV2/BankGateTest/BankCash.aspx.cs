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
    public partial class BankCash : System.Web.UI.Page
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string urlService = "https://bank.kudopay.xyz/BankCashV2.ashx";
        string urlService2 = "https://bank.zoroshop.xyz/CheckMoMo.ashx";
        //string urlService = "http://localhost:61870/VPGJsonService.ashx";
        string partnerKey = "e1a8d8ca47e0d70990fcef27ee267ced";
        string partnerCode = "dcp";
        string serviceCode = "bankcash";
        string commandCode = "cash";

        public List<BankAccount> Banks
        {
            get { return (List<BankAccount>)ViewState["Results"]; }
            set { ViewState["Results"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                Init();
        }
        public class RequestDataV3
        {
            public string PartnerCode { get; set; }
            public string AccountNumber { get; set; }
            public string AccountName { get; set; }
           
            
            public string RequesTime { get; set; }
           
            public string Signature { get; set; }

        }
        public class RequestDataV2
        {
            public string PartnerCode { get; set; }
            public string AccountNumber { get; set; }
            public string BankCode { get; set; }
            public string AccountName { get; set; }
            public int Amount { get; set; }
            public string RefCode { get; set; }
            public string Signature { get; set; }
            public string CallbackUrl { get; set; }

        }
        private void Init()
        {
            var request = new RequestDataV2()
            {
                PartnerCode = "dcp",
                AccountName = "Pham Van Huong",
                BankCode= "VCB",
                Amount = 20000,
                AccountNumber = "9762105201",
                RefCode = "RTyyy11100678923",
                CallbackUrl="http://abc.com"
            };
            request.Signature = Encrypts.MD5(partnerCode + request.AccountNumber + request.AccountName+request.BankCode + request.Amount+request.RefCode + partnerKey);
            NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", urlService, serializer.Serialize(request) });
            var serviceResponse = PostJson(urlService, serializer.Serialize(request));
            NLogLogger.Info(new string[] { "BankCash Test", "Response Core", serviceResponse });
        }
        private void Init2()
        {
            var request = new RequestDataV3()
            {
                PartnerCode = "pp",
            
                AccountNumber = "0963645443",
                RequesTime = "RT002",

            };
            request.Signature = Encrypts.MD5(partnerCode + request.AccountNumber + request.RequesTime + partnerKey);
            var serviceResponse = PostJson(urlService2, serializer.Serialize(request));
            NLogLogger.Info(new string[] { "BankCash Test", "Response Core", serviceResponse });
        }
        protected void CheckOut_Click(object sender, EventArgs e)
        {

            txtBankName.Text = "momo";
            txtAccountNumber.Text = Banks.Single(s => s.BankName == ddlBanks.SelectedItem.Text).BankAccountNumber;
            txtAccountName.Text = Banks.Single(s => s.BankName == ddlBanks.SelectedItem.Text).BankAccountName;
            txtAmoutTranfer.Text = txtVND.Text + " VNĐ";


            string commandCode = "order";

            var requestContent = serializer.Serialize(new OrderRequest()
            {
                Type = ddlType.SelectedValue,
                AccountName = "toitest",
                Amount = Convert.ToInt32(txtVND.Text),
                AppCode = "App1",
                Note = "App1",
                CallbackUrl = "https://appxxx.info/callback/",
                RefCode = "RT001",
                BankName = ddlBanks.SelectedItem.Text,
                BankAccountName = txtAccountName.Text,
                BankAccountNumber = ddlBanks.SelectedValue
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

            var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
            var resObj = serializer.Deserialize<APIResponse>(serviceResponse);

            //var orderResponse = serializer.Deserialize<OrderResponse>(resObj.ResponseContent);

            pnSt1.Visible = false;
            pnSt2.Visible = true;
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

            txtReason.Text = resObj.ResponseContent;

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

            public string Note { get; set; }
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
            public string BankAccountNumber { get; set; }
            public string BankAccountName { get; set; }
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

        protected void CheckTran_Click(object sender, EventArgs e)
        {



            string commandCode = "cash";
            var order = new OrderRequest();
            order.Type = ddlType.SelectedValue;
            order.AccountName = "toitest";
            order.Amount = int.Parse(txtAmoutTranfer.Text);
            order.AppCode = "App1";
            order.Note = txtReason.Text;
            order.CallbackUrl = "https://appxxx.info/callback/";
            order.RefCode = "RT001";
            order.BankName = "momo";
            order.BankAccountName = txtAccountName.Text;
            order.BankAccountNumber = txtAccountNumber.Text;
            var requestContent = serializer.Serialize(order);
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

            //var orderResponse = serializer.Deserialize<OrderResponse>(resObj.ResponseContent);

            pnSt1.Visible = false;
            pnSt2.Visible = true;
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

            txtReason.Text = resObj.ResponseContent;
        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
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
            Banks = serializer.Deserialize<List<BankAccount>>(resObj.ResponseContent);

            ddlBanks.DataSource = Banks;
            ddlBanks.DataValueField = "BankAccountNumber";
            ddlBanks.DataTextField = "BankName";
            ddlBanks.DataBind();

            ddlBanks.Items.Insert(0, new ListItem("Chọn ngân hàng", "NA"));
        }
    }
}