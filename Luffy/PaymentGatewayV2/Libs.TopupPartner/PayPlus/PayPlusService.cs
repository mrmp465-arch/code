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
using static Libs.TopupPartner.BuyCardService;

namespace Libs.TopupPartner.PayPlus
{
    public class PayPlusService : IBuyCardHandler
    {

       

        protected string ServiceUrl = "https://apicard.coroach.xyz/VPGJsonService.ashx";  // Service Production
        protected string partnerCode = "hyn5";
        protected string partnerKey = "941d69b5bfe82f513072c8545dd40bb2";
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        public APIResponse downloadSoftpin(string requestId, string provider, int amount, int quantity, ref string providerResponse)
        {

            switch (provider.ToUpper())
            {
                case "VTT":
                case "VIETTEL":
                    provider = "VTT";
                    break;
                case "VMS":
                    provider = "VMS";
                    break;
                case "VNP":
                    provider = "VNP";
                    break;
            }

            string commandCode = "buycard";
            var order = new BuyCardRequest();
            order.Provider = provider;
            order.AccountName = requestId;
            order.Amount = amount;
            order.Quantity = 1;
            order.OrderNo = requestId;

            var requestContent = serializer.Serialize(order);
            var signature = Encrypts.MD5(partnerCode + "buycard" + commandCode + requestContent + partnerKey);
            var requestData = new RequestData()
            {
                PartnerCode = partnerCode,
                CommandCode = commandCode,
                RequestContent = requestContent,
                ServiceCode = "buycard",
                Signature = signature
            };
            NLogLogger.Info(new string[] { "Card Test", "Request Core", serializer.Serialize(requestData) });
            var serviceResponse = PostData(ServiceUrl, serializer.Serialize(requestData));
            NLogLogger.Info(new string[] { "Card Test", "Response Core", serviceResponse });
            var response = serializer.Deserialize<APIResponse>(serviceResponse);


            return ConvertResultCode(response);
        }

        public int checkStore(string provider, int amount)
        {



            return -1;
        }

        public APIResponse ConvertResultCode(APIResponse result)
        {

            switch (result.ResponseCode)
            {
                case 1:
                case 0:
                    var listcards= serializer.Deserialize<List<CardDVO>>(result.ResponseContent);
                    var listCard = listcards[0].Serial + "|" + listcards[0].Pin;
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = listCard
                    };

                case -370:

                    return new APIResponse((int)ResponseCode.CardQuantityLimit)
                    {
                        ResponseContent = String.Empty
                    };

                case -371:
                    return new APIResponse((int)ResponseCode.CardOutOfStock)
                    {
                        ResponseContent = String.Empty
                    };
                case -319:
                    return new APIResponse((int)ResponseCode.TransactionDuplicate)
                    {
                        ResponseContent = String.Empty
                    };

                default:
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        ResponseContent = string.Empty,
                        Description =""
                    };



            }
        }

        public class RequestData
        {
            public string PartnerCode { get; set; }
            public string ServiceCode { get; set; }
            public string CommandCode { get; set; }
            public string RequestContent { get; set; }
            public string Signature { get; set; }
        }



      

        public class CardDVO
        {
            public string Serial { get; set; }
            public string Pin { get; set; }
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
