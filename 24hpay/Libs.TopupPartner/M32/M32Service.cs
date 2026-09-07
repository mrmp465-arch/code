using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;

namespace Libs.TopupPartner.M32
{
    public class M32Service : IBuyCardHandler
    {

        protected string ServiceUrl = "http://139.180.206.12:9999/buycard";

        protected string apiUsername = "peopeo2";
        protected string apiPass = "12686868ghjklmn41421";

        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public APIResponse downloadSoftpin(string requestId, string provider, int amount, int quantity, string partnerCode, string providerCode, ref string providerResponse)
        {

            switch (provider.ToUpper())
            {
                case "VIETTEL":
                case "VTT":
                    provider = "VTT";
                    break;
                case "VMS":
                    provider = "VMS";
                    break;
                case "VNP":
                    provider = "VNP";
                    break;


            }

            RequestData requestData = new RequestData();
            requestData.username = apiUsername;
            requestData.password = apiPass;
            requestData.amount = amount.ToString();
            requestData.quantity = quantity.ToString();
            requestData.requestid = requestId;
            requestData.telco = provider;
            NLogLogger.Info(new string[] { "BuyCard", "Request", serializer.Serialize(requestData) });
            string responseData = PostData(ServiceUrl, serializer.Serialize(requestData));
            NLogLogger.Info(new string[] { "BuyCard", "Response", responseData });
            providerResponse = responseData;
            var response = serializer.Deserialize<ResultData>(responseData);
            if(response.errorcode=="-12")
            {
                TelegramNotify.SendNotify(-845553760, $"Hết thẻ {provider}  mệnh giá {amount} !!!");
            }    

            return ConvertResultCode(response);
        }

        public APIResponse ConvertResultCode(ResultData result)
        {

            switch (result.errorcode)
            {

                case "1":
                    //Thành công
                    List<ListCardOp> listCardDVO = new List<ListCardOp>();
                    foreach (var item in result.listCard)
                        listCardDVO.Add(
                            new ListCardOp
                            {
                                Pin = item.pincode,
                                Serial = item.serial,
                                ExpireDate = item.expireDate
                            }
                            ); ;
                    var listCard = serializer.Serialize(listCardDVO);
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {

                        ResponseContent = listCard
                    };

                case "-1":
                    //Giao dịch thất bại
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        ResponseContent = string.Empty
                    };

                case "-3":

                    //Thông tin xác thực không chính xác
                    return new APIResponse((int)ResponseCode.AccountNotExists)
                    {
                        ResponseContent = string.Empty
                    };

                case "-12":

                    //Hết thẻ
                    return new APIResponse((int)ResponseCode.CardQuantityLimit)
                    {
                        ResponseContent = string.Empty
                    };

                default:
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        ResponseContent = string.Empty
                    };


            }
        }


        public class RequestData
        {
            public string username { get; set; }
            public string password { get; set; }
            public string requestid { get; set; }
            public string telco { get; set; }
            public string amount { get; set; }
            public string quantity { get; set; }

        }

        public class ResultData
        {
            public string errorcode { get; set; }
            public string description { get; set; }
            public string requestid { get; set; }
            public List<ListCard> listCard { get; set; }

        }
        public class ListCard
        {
            public string telco { get; set; }
            public string serial { get; set; }
            public string pincode { get; set; }
            public int amount { get; set; }
            public string expireDate { get; set; }
            
        }
        public class ListCardOp
        {

            public string ExpireDate { get; set; }
            public string Serial { get; set; }
            public string Pin { get; set; }

        }
        public class StatusBuyCard
        {
            public string code { get; set; }
            public string value { get; set; }
        }

        private string PostData(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(ServiceUrl);
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
        public string CreateSignRSA(string data, string privateKey)
        {
            RSACryptoServiceProvider rsaCryptoIPT = new RSACryptoServiceProvider(1024);
            rsaCryptoIPT.FromXmlString(privateKey);
            return Convert.ToBase64String(rsaCryptoIPT.SignData(new ASCIIEncoding().GetBytes(data), new SHA1CryptoServiceProvider()));
        }

        public bool CheckSignRSA(string data, string sign, string publicKey)
        {
            try
            {
                RSACryptoServiceProvider rsacp = new RSACryptoServiceProvider();
                rsacp.FromXmlString(publicKey);
                bool status = rsacp.VerifyData(Encoding.UTF8.GetBytes(data), "SHA1", Convert.FromBase64String(sign));
                return status;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string DecryptString(string key, string toDecrypt)
        {
            byte[] keydata = Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").Replace(" ", "+").ToLower();
            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));
            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Key = tripleDesKey;
            byte[] cryptByte = Convert.FromBase64String(toDecrypt);
            MemoryStream ms = new MemoryStream(cryptByte, 0, cryptByte.Length);
            ICryptoTransform cryptoTransform = tripdes.CreateDecryptor();
            CryptoStream decStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Read);
            StreamReader read = new StreamReader(decStream);
            return (read.ReadToEnd());
        }

        public int checkStore(string provider, int amount)
        {
            throw new NotImplementedException();
        }

       
    }

}


