using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Libs.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class CardServiceTest
    {
        string ServiceUrl = "http://35.187.251.197:1684/default.aspx";
        string PartnerCode = "pl";
        string PartnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";


        [TestMethod]
        public void buycardService()
        {

            var requestId = "0016";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BuyCardRequest requestData = new BuyCardRequest();
            requestData.FunctionName = "buycard";
            requestData.OrderNo = requestId;
            requestData.PartnerCode = PartnerCode;
            requestData.CardType = "viettel";
            requestData.Amount = 10000;
            requestData.Quantity = 2;
            requestData.RequestTime = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            string data = requestData.OrderNo + requestData.PartnerCode + requestData.CardType + requestData.Amount.ToString() + requestData.Quantity + requestData.RequestTime.ToString() + PartnerKey;
            requestData.Signature = Encrypts.MD5(data);
            string responseData = PostData(ServiceUrl, serializer.Serialize(requestData));           
            Console.WriteLine(responseData);           

        }

        public class BuyCardRespone
        {
            public int ResponseCode { get; set; }
            public string Description { get; set; }
            public List<Cards> ResponseContent { get; set; }
            public string Signature { get; set; }
        }

        public class Cards
        {
            public string Serial { get; set; }
            public string Pin { get; set; }
            public DateTime ExpireDate { get; set; }
        }

        public class BuyCardRequest
        {
            public string FunctionName { get; set; }
            public string OrderNo { get; set; }
            public string PartnerCode { get; set; }
            public string CardType { get; set; }
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
