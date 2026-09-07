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
        string urlService = "https://bankgate.5pay.info/bankin/order.ashx";


        string urlgetbank = "https://bankgate.5pay.info/bankin/info.ashx";
        string partnerKey = "05042606d7783827ff864c0a45abe5a4";
        string partnerCode = "paytest";
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
                getbank();
            //inti2();
        }
        private void getbank()
        {
            string commandCode = "getbanks";
            //var requestContent = serializer.Serialize(new GetBankRequest() { Type = "momov2", AccountName="mrx" });
            //requestDatavar signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            var requestData = new RequestData()
            {
                PartnerCode = partnerCode,

            };
            NLogLogger.Info(new string[] { "Bank Test", "Requst Core", urlgetbank, serializer.Serialize(requestData) });
            var serviceResponse = PostJson(urlgetbank, serializer.Serialize(requestData));
            NLogLogger.Info(new string[] { "Bank Test", "Response Core", serviceResponse });
            var resObj = serializer.Deserialize<APIResponse>(serviceResponse);
            var BanksV2 = serializer.Deserialize<List<BankAccountV2>>(resObj.ResponseContent);

            ddlBanks.DataSource = BanksV2;
            ddlBanks.DataTextField = "Name";
            ddlBanks.DataValueField = "BankCode";
            ddlBanks.DataBind();
            //ddlBanks.Items.Add(new ListItem("Ví momo:", "MOMO"));
            ddlBanks.Items.Insert(0, new ListItem("Chọn ngân hàng:", "random"));
            //Banks = new List<BankAccount>();
        }
        private void Order()
        {
            string commandCode = "order";

            var requestContent = new OrderRequest()
            {

                Amount = int.Parse(txtVND.Text),
                CallbackUrl = "https://test.5pay.info/callback.ashx",
                RefCode = DateTime.Now.ToString("MMddHHmmss"),
                BankCode = "random",
                PartnerCode = partnerCode,
            };
            var signature = Encrypts.MD5(partnerCode + ddlBanks.SelectedValue + requestContent.Amount + requestContent.RefCode + requestContent.CallbackUrl + partnerKey);
            requestContent.Signature = signature;
            NLogLogger.Info(new string[] { "Bank Test", "Requst Core", urlService, serializer.Serialize(requestContent) });
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestContent));
            NLogLogger.Info(new string[] { "Bank Test", "Response Core", serviceResponse });
            var resObj = serializer.Deserialize<APIResponse>(serviceResponse);
            if (resObj.ResponseCode > 0)
            {
                var orderResponse = serializer.Deserialize<OrderResponse>(resObj.ResponseContent);
                url = orderResponse.Url;
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", $" PopupManager('{orderResponse.Url}');", true);
            }
            else
            {
                lblWarning.Text = resObj.Description;
                lblWarning.Visible = true;
            }


            //Response.Redirect(orderResponse.Url);
        }

        protected void CheckOut_Click(object sender, EventArgs e)
        {

            //txtBankName.Text = ddlBanks.SelectedItem.Text;

            Order();

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
            public string Signature { get; set; }
            public string BankCode { get; set; }
            public string PartnerCode { get; set; }
            //public string BankAccountName { get; set; }
            //public string AppCode { get; set; }
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
            public string Url { get; set; }
            //public string BankAccountName { get; set; }
            public int Amount { get; set; }
            public string OrderNo { get; set; }
            //public int Timeout { get; set; }
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
            public string BankCode { get; set; }
            public string Name { get; set; }
        }
        protected void CheckTran_Click(object sender, EventArgs e)
        {

        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {


        }
    }
}