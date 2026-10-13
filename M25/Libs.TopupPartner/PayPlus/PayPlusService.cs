using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;

namespace Libs.TopupPartner.PayPlus
{
    public class PayPlusService : IBuyCardHandler
    {

        //protected string ServiceUrl = "http://127.0.0.1:1684/default.aspx"; // Service Test

        protected string ServiceUrl = "http://localhost:1681/default.aspx";  // Service Production
        protected string PartnerCode = "pl";
        protected string PartnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        public APIResponse downloadSoftpin(string requestId, string provider, int amount,  int quantity, string partnerCode, string providerCode, ref string providerResponse)
        {

            switch (provider.ToUpper())
            {
                case "VTT":
                    provider = "viettel";
                    break;
                case "VMS":
                    provider = "vms";
                    break;
                case "VNP":
                    provider = "vnp";
                    break;
            }

            RequestData requestData = new RequestData();
            requestData.FunctionName = "buycard";
            requestData.OrderNo = requestId;
            requestData.PartnerCode = PartnerCode;
            requestData.CardType = provider;
            requestData.Amount = amount;
            requestData.Quantity = quantity;
            requestData.RequestTime = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            string data = requestData.OrderNo + requestData.PartnerCode + requestData.CardType + requestData.Amount.ToString() + requestData.Quantity + requestData.RequestTime.ToString() + PartnerKey;
            requestData.Signature = Encrypts.MD5(data);
            NLogLogger.Info(new string[] { "PayPlusBuyCard", "PayPlusCardRequest", serializer.Serialize(requestData) });
            string responseData = PostData(ServiceUrl, serializer.Serialize(requestData));
            providerResponse = responseData;
            NLogLogger.Info(new string[] { "PayPlusBuyCard", "PayPlusCardResponse", responseData });
            if (!string.IsNullOrEmpty(responseData))
            {
                var resultObj = serializer.Deserialize<ResponseData>(responseData);
                List<CardDVO> cardDvos = new List<CardDVO>();
                if (resultObj.errorCode == "0")
                {
                    foreach (var r in resultObj.listCards)
                    {
                        var card = r.Split('|');
                        var cd = new CardDVO();
                        cd.Serial = card[2];
                        cd.Pin = card[3];
                        cd.ExpireDate = Convert.ToDateTime(card[4]);
                        cardDvos.Add(cd);
                    }

                }
               
                return ConvertResultCode(resultObj, cardDvos);
            }
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public int checkStore(string provider, int amount)
        {


            if (provider.Equals("vtt")) provider = "viettel";

            RequestData requestData = new RequestData();
            requestData.FunctionName = "checkstore";
            requestData.PartnerCode = PartnerCode;
            requestData.CardType = provider;
            requestData.Amount = amount;
            requestData.RequestTime = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            string data = requestData.OrderNo + requestData.PartnerCode + requestData.CardType + requestData.Amount.ToString() + requestData.Quantity + requestData.RequestTime.ToString() + PartnerKey;
            requestData.Signature = Encrypts.MD5(data);

            string responseData = PostData(ServiceUrl, serializer.Serialize(requestData));


            return int.Parse(responseData);
        }

        public APIResponse ConvertResultCode(ResponseData result, IList<CardDVO> listCard)
        {

            switch (result.errorCode)
            {
                case "0":

                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = serializer.Serialize(listCard)
                    };

                case "336":

                    return new APIResponse((int)ResponseCode.CardTypeInvalid)
                    {
                        ResponseContent = String.Empty
                    };

                case "371":
                    return new APIResponse((int)ResponseCode.CardOutOfStock)
                    {
                        ResponseContent = String.Empty
                    };
                case "102":
                    return new APIResponse((int)ResponseCode.TransactionDuplicate)
                    {
                        ResponseContent = String.Empty
                    };

                default:
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        ResponseContent = string.Empty,
                        Description = result.message
                    };



            }
        }

        public class RequestData
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

        public class CardDVO
        {
            public string Serial { get; set; }
            public string Pin { get; set; }
            public DateTime ExpireDate { get; set; }
        }

        public class ResponseData
        {
            public string errorCode { get; set; }
            public string message { get; set; }
            public List<string> listCards { get; set; }

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
