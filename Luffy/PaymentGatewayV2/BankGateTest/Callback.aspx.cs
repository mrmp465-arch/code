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
    public partial class CallbackUrl : System.Web.UI.Page
    {
        private string partnerCode = "pp";
        private string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
        private string urlService = "http://96.9.75.2:1582/confirm.ashx";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                

            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var requestData = new RequestData()
            {
                PartnerCode = partnerCode,
                ResponseNo = Convert.ToInt64(HttpContext.Current.Request.QueryString["responseno"]),
                ConfirmCode = Convert.ToInt32(ddl.SelectedValue),
                ResquestTime = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
            };
            var signature = requestData.PartnerCode + requestData.ResponseNo + requestData.ConfirmCode + requestData.ResquestTime + partnerKey.Trim();
            signature = Encrypts.MD5(signature.ToLower());
            requestData.Signature = signature;
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
            Response.Write(serviceResponse);
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

    }

    public class RequestData
    {
        public string PartnerCode { get; set; }
        public long ResponseNo { get; set; }
        public int ConfirmCode { get; set; }
        public string ResquestTime { get; set; }
        public string Signature { get; set; }

    }

}