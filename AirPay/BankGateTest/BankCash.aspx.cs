using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Utils;
using RestSharp;

namespace BankGateTest
{
    public partial class BankCash : System.Web.UI.Page
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string urlService = "https://bankgate.1ppay.info/bankout/cash.ashx";
        string urlService2 = "https://bankgate.1ppay.info/VPGJsonService.ashx";
        //string urlService = "http://localhost:61870/VPGJsonService.ashx";
        string partnerKey = "0baba54aae3c2700686926526edb6e87";
        string partnerCode = "paytest";
        string serviceCode = "bankcash";
        string commandCode = "cash";

        public List<BankAccount> Banks
        {
            get { return (List<BankAccount>)ViewState["Results"]; }
            set { ViewState["Results"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!Page.IsPostBack)
            //Init();
            //buycard();
        }





        protected void CheckOut_Click(object sender, EventArgs e)
        {

            //txtBankName.Text = "momo";
            //txtAccountNumber.Text = Banks.Single(s => s.BankName == ddlBanks.SelectedItem.Text).BankAccountNumber;
            //txtAccountName.Text = Banks.Single(s => s.BankName == ddlBanks.SelectedItem.Text).BankAccountName;
            //txtAmoutTranfer.Text = txtVND.Text + " VNĐ";


            //string commandCode = "order";

            var requestContent = new OrderRequest()
            {
                Amount = Convert.ToInt32(txtAmoutTranfer.Text),
                CallbackUrl = "https://test.1ppay.org/callback.ashx",
                RefCode = "RT001" + DateTime.Now.ToString("MMddHHmmss"),
                PartnerCode = partnerCode,
                AccountName = txtAccountName.Text,
                AccountNumber = txtAccountNumber.Text,
                BankCode = txtBankName.Text,
            };
            var signature = Encrypts.MD5(partnerCode + requestContent.AccountNumber + requestContent.AccountName + requestContent.BankCode + requestContent.Amount + requestContent.RefCode + requestContent.CallbackUrl + partnerKey);

            requestContent.Signature = signature;
            NLogLogger.Info(new string[] { "Bank Test", "Requst Core", urlService, serializer.Serialize(requestContent) });
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestContent));
            NLogLogger.Info(new string[] { "Bank Test", "Response Core", serviceResponse });
            var resObj = serializer.Deserialize<APIResponse>(serviceResponse);

            //var orderResponse = serializer.Deserialize<OrderResponse>(resObj.ResponseContent);

            pnSt1.Visible = false;
            pnSt2.Visible = true;
            lblWarning.Visible = true;
            CheckOut.Visible = false;
            //CheckTran.Visible = true;

            //txtBankName.Text = ddlBanks.SelectedItem.Text;
            //txtAccountNumber.Text = Banks.Single(s => s.BankName == ddlBanks.SelectedItem.Text).AccountNumber;
            //txtAccountName.Text = Banks.Single(s => s.BankName == ddlBanks.SelectedItem.Text).AccountName;
            //txtAmoutTranfer.Text = txtVND.Text + " VNĐ";

            //txtBankName.Text = orderResponse.BankName;
            //txtAccountNumber.Text = orderResponse.BankAccountNumber;
            //txtAccountName.Text = orderResponse.BankAccountName;
            //txtAmoutTranfer.Text = orderResponse.Amount + " VNĐ";

            txtReason.Text = resObj.Description;

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
            public string BankCode { get; set; }
            public string AccountNumber { get; set; }
            public string Signature { get; set; }

            public string RefCode { get; set; }

            public string PartnerCode { get; set; }
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



    }
}