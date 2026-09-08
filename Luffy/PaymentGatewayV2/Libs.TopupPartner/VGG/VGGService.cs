using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;

namespace Libs.TopupPartner.VGG
{
    public class VGGService
    {

        protected string ServiceUrl = "http://api.vgg.vn/CardResellerService/default.aspx";
        protected string PartnerCode = "vgg";
        protected string PartnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";

        public string downloadSoftpin(string requestId, string provider, int amount, int quantity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            RequestData requestData = new RequestData();
            requestData.FunctionName = "buycard";
            requestData.OrderNo = requestId;
            requestData.PartnerCode = PartnerCode;
            requestData.ProviderCode = provider;
            requestData.Amount = amount;
            requestData.Quantity = quantity;
            requestData.RequestTime = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            string data = requestData.OrderNo + requestData.PartnerCode + requestData.ProviderCode + requestData.Amount.ToString() + requestData.Quantity + requestData.RequestTime.ToString() + PartnerKey;
            requestData.Signature = Encrypts.MD5(data);
            string responseData = PostData(ServiceUrl, serializer.Serialize(requestData));
            return responseData;
        }

        public int checkStore(string provider, int amount)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            RequestData requestData = new RequestData();
            requestData.FunctionName = "checkstore";
            requestData.PartnerCode = PartnerCode;
            requestData.ProviderCode = provider;
            requestData.Amount = amount;
            requestData.RequestTime = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            string data = requestData.OrderNo + requestData.PartnerCode + requestData.ProviderCode + requestData.Amount.ToString() + requestData.Quantity + requestData.RequestTime.ToString() + PartnerKey;
            requestData.Signature = Encrypts.MD5(data);
            string responseData = PostData(ServiceUrl, serializer.Serialize(requestData));
            return int.Parse(responseData);
        }

        public class RequestData
        {
            public string FunctionName { get; set; }
            public string OrderNo { get; set; }
            public string PartnerCode { get; set; }
            public string ProviderCode { get; set; }
            public int Amount { get; set; }
            public int Quantity { get; set; }
            public string Signature { get; set; }
            public string PrivateKey { get; set; }
            public string PublicKey { get; set; }
            public long RequestTime { get; set; }

        }

        private string PostData(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(ServiceUrl);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
            request.Accept = "JSON";
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
}
