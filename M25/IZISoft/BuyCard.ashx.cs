using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using ATSFRM.Security.CryptoGraphy.Symetric;
namespace IZISoft
{
    /// <summary>
    /// Summary description for BuyCard2
    /// </summary>
    public class BuyCard2 : IHttpHandler
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static string encryptionKey = "3c4042db80c9512a92a9daf0";
        public void ProcessRequest(HttpContext context)
        {
            var jsonString = String.Empty;
            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }


            if (string.IsNullOrEmpty(jsonString))
            {
                context.Response.Write("99|Data empty");
                return;
            }
            NLogLogger.Info(new string[] { "Imedia", "ProcessRequest", jsonString });
            var requestdata = serializer.Deserialize<RequestData>(jsonString);

            var ResultData = new ResultData();

            var token = login();
            if(String.IsNullOrEmpty(token))
            {
                ResultData.errorcode = -99;
                context.Response.Write(serializer.Serialize(ResultData));
                return;
            }

            BuyCardRequest cardRequest = new BuyCardRequest();
            cardRequest.operation = 1000;
            cardRequest.username = "DUCTAI_API";
            cardRequest.keyBirthdayTime = "2023/04/03 09:39:31.227";
            cardRequest.requestID = requestdata.requestid;
            cardRequest.token = token;
            var productId = convertoProductId(requestdata.telco, requestdata.amount);
            cardRequest.buyItems = new List<BuyCardProduct> { new BuyCardProduct { quantity = requestdata.quantity, productId = productId } };

            cardRequest.signature = cardRequest.username + "|" + cardRequest.requestID + "|" + cardRequest.token + "|" + cardRequest.operation;

            cardRequest.signature = CreateSignRSA(cardRequest.signature);
            NLogLogger.Info(new string[] { "Imedia", "Rquest", serializer.Serialize(cardRequest) });



            var service = new IMediaService.TopupInterfaceClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            System.Net.ServicePointManager.Expect100Continue = false;
            var result = service.requestHandle(serializer.Serialize(cardRequest));
            NLogLogger.Info(new string[] { "Imedia", "result", result });
            var response = serializer.Deserialize<BuyCardRespone>(result);

            if (response.errorCode < 0)
            {
                ResultData.errorcode = response.errorCode;
                ResultData.description = response.errorMessage;
                context.Response.Write(serializer.Serialize(ResultData));
                return;
            }
            if(response.merchantBalance<1500000)
            {
                //TelegramNotify.SendNotify(-845553760, $"Tài khoản mua thẻ sắp hết tiền !!!");
                TelegramNotify.SendWarning("-4882076202", "Tài khoản mua thẻ còn "+ response.merchantBalance.ToString("#,#").Replace(",", "."));
            }    
            ResultData.errorcode = response.errorCode;
            ResultData.description = response.errorMessage;
            var lstCard = new List<ListCard>();
            var DESCrypto = new TripleDESCrypto();
            foreach (var product in response.products)
            {
                foreach (var softpin in product.softpins)
                {
                    var card = new ListCard();
                    card.amount = requestdata.amount;
                    card.telco = requestdata.telco;
                    card.expireDate = softpin.expiryDate;
                    card.serial = softpin.softpinSerial;
                    card.pincode = DESCrypto.DecryptString(softpin.softpinPinCode, encryptionKey);
                    lstCard.Add(card);
                }
            }
            ResultData.listCard = lstCard;

            context.Response.Write(serializer.Serialize(ResultData));
            return;
        }

        public string login()
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                var token = "";
                var dataCache = DataCaching.GetCache<string>("ImediaToken");
                if (dataCache != null)
                {
                    token = dataCache.ToString();
                    return token;
                }

                LoginRequest cardRequest = new LoginRequest();
                cardRequest.operation = 1400;
                cardRequest.username = "DUCTAI_API";
                cardRequest.merchantPass = "1253215560539";


                cardRequest.signature = cardRequest.username + "|" + cardRequest.merchantPass;

                cardRequest.signature = CreateSignRSA(cardRequest.signature);
                NLogLogger.Info(new string[] { "Imedia", "Rquest", serializer.Serialize(cardRequest) });



                var service = new IMediaService.TopupInterfaceClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                System.Net.ServicePointManager.Expect100Continue = false;
                var result = service.requestHandle(serializer.Serialize(cardRequest));
                NLogLogger.Info(new string[] { "Imedia", "result", result });
                var response = serializer.Deserialize<LoginRespone>(result);

                if(response.errorCode==0)
                {
                    DataCaching.SetCache("ImediaToken", response.token, 86400/2);
                    return response.token;
                }   

                return "";

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "IZISoftTopup", "Response failed Exeption", exp.Message, exp.StackTrace });
                return "";
            }
        }
        public class RequestData
        {

            public string requestid { get; set; }
            public string telco { get; set; }
            public int amount { get; set; }
            public int quantity { get; set; }

        }
        public class ResultData
        {
            public int errorcode { get; set; }
            public int merchantBalance { get; set; }
            public string description { get; set; }
            public string requestid { get; set; }
            public List<ListCard> listCard { get; set; }

        }
        public class ListCard
        {
            public string telco { get; set; }
            public string serial { get; set; }
            public string expireDate { get; set; }
            public string pincode { get; set; }
            public int amount { get; set; }
        }
        private int convertoProductId(string telco, int value)
        {
            if (telco == "VTT")
            {
                switch (value)
                {
                    case 10000:
                        return 1;
                    case 20000:
                        return 2;
                    case 30000:
                        return 3;
                    case 50000:
                        return 4;
                    case 100000:
                        return 5;
                    case 200000:
                        return 6;
                    case 300000:
                        return 7;
                    case 500000:
                        return 8;
                }
            }
            if (telco == "VNP")
            {
                switch (value)
                {
                    case 10000:
                        return 17;
                    case 20000:
                        return 18;
                    case 30000:
                        return 19;
                    case 50000:
                        return 20;
                    case 100000:
                        return 21;
                    case 200000:
                        return 22;
                    case 300000:
                        return 23;
                    case 500000:
                        return 24;
                }
            }
            if (telco == "VMS")
            {
                switch (value)
                {
                    case 10000:
                        return 25;
                    case 20000:
                        return 26;
                    case 30000:
                        return 27;
                    case 50000:
                        return 28;
                    case 100000:
                        return 29;
                    case 200000:
                        return 30;
                    case 300000:
                        return 31;
                    case 500000:
                        return 32;
                }
            }
            return 0;
        }

        public string CreateSignRSA(string data)
        {
            RSACryptoServiceProvider rsaCryptoIPT = new RSACryptoServiceProvider(1024);
            //var privateKey = "<RSAKeyValue><Modulus>743WHG3HM5tetq0oq2mVXjYkRLRYsXXH2BBbGXF8dm+WezAcO7IYlxMYSzUVwoGawy8Hzh7G4zEL3dV7KrzYngANlrQgqxDylbX34kzPRn69U9WDBHaH4okM+lcKMrDkdJmRuCbagnqwEIQpPLj0GMjRGfRXheLnYrQyRdfr3rM=</Modulus><Exponent>AQAB</Exponent><P>/AzjDv+fr2Ih2YYIi1HZ7xMqmAUV3GeEyXWUXheslEHfJxu9DMtc5pObabRC6mAsLqdlU9s4jRMqh1yzcKfEjw==</P><Q>807R5QXBHKh36gVhOfrCKBOvou59N8gho6D0m1Ty6uIFB2rUe638Dj15MfHehf5UPzbnbskfj4AdHuBOKUn9nQ==</Q><DP>cBrXPsuJXbta7OIFmNnOAdzXfAf/Ain00JoAZJ1JACQQOdfHjRJCfre2TxyDCrW90P5ZPiPqEi0tJEmh8gBclw==</DP><DQ>N8Ipce3WqqWlDXl8JZhk5GBWkOVMxvrTUrdxNyPJo7B2bJO77DgcGntWCe8fCuAVGIORmB75X56BjfDjmKy/NQ==</DQ><InverseQ>y1L6Jvamp1GNxU9S43dCO3PD28P2fHkV+EkM5Sqp57dDe4Sn6MU+5STyRJesv7fBPPnlGXLc8UgF4IL1L0+heQ==</InverseQ><D>6XI1Z3rrlzUf9bGFYpYAA9GLQpDlpfp7h+lYfdEEU36nDOFzghEquX7YO+I9lFEs+mzIlGuVsi1HvSSfZKSoCl6SmsbVUWkZEYPLhSG5ikpsb5DZnMsT5W/yl8N9HcZsO9C1Cyto0EgvZxnNOjdf4RswZhq63C7cI4lUrO4g4NE=</D></RSAKeyValue>";
            var privateKey = "<RSAKeyValue><Modulus>cuC6C3ymPKtUJOPOXTvrlxrppK4wTMRtuYrB1sffV+8rDOddTtc1zepYSmKnYQ0h6mtdq3+9nj8D4/udQbnLqQMPXukSkvO6E5WJoIcMDEXqp8o0E1BrpPjM99ejZOAh3NLiEuEtfuyrZ3aQ9xwDK0CLdJLDPVMxDy+bhiwpgxU=</Modulus><Exponent>AQAB</Exponent><P>rNr+OEtmN3iQrxf1ufOVMV3MVpZNj01IPdhUlQ8GuRCHBBWodZ9BlVITIOddzqW0iwtQ6wrCM8dE1dWLkG0C/w==</P><Q>qiKCie4po4n12w62Dg9RQN9nAkAzwACNn2rX5ZT1mBWDuOQ3zaFCu4apxoWUfBF8fWxYcIWCmoxVnRdwPp896w==</Q><DP>KqgcZm4M+pFxOxRxmoMHBVi9bXzIBR4wPrrdHK/Jm+/9Mb/ag7RabewENnGCT9XKuSmAvZA5HqgFT7PULj8Ipw==</DP><DQ>WLNVS4QXwWOUAnlRAVOLET9n/Qxr7pZbP9n8ZX2b7YaJ/kM9tL92gz5aFV5fY6/aL6nlEgUJannvyM8Hfh6XsQ==</DQ><InverseQ>Z5sitpewujrlxie4DGmtwb6F1JylDhZyIePJfo/ZwstvwqOmYGPGJpufLwdc1+pDIYnrRKBREFgtqTCTkyE4Pg==</InverseQ><D>D6K2r8mPdsJ6+WpoNBhrwG1RT7DFsi0qIg7YwzsBEx7iZSc4c/qmwBgdXEKBNZxr+VQTs7v3OwqopOciSEfZiEDLTclL0DgVA4WBOn6l6Bgfi7HCYpT0UbQV1TAbM0gCzl1c9Ua5coKy8bP52w4BoVTwnMAul9S9r53ew8ZYrSE=</D></RSAKeyValue>";
            rsaCryptoIPT.FromXmlString(privateKey);
            byte[] signature = rsaCryptoIPT.SignData(new ASCIIEncoding().GetBytes(data), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
            //return Convert.ToBase64String(rsaCryptoIPT.SignData(new ASCIIEncoding().GetBytes(data), new SHA1CryptoServiceProvider()));
        }
      
    public class LoginRequest
        {
            public int operation { get; set; } //1400
            public string username { get; set; }
            public string merchantPass { get; set; }
            public string signature { get; set; }

        }
        public class LoginRespone
        {
            public int errorCode { get; set; }
            public string errorMessage { get; set; }
            public string token { get; set; }
          
        }
        public class BuyCardRespone
        {
            public int errorCode { get; set; }
            public string errorMessage { get; set; }
            public long merchantBalance { get; set; }
            public string token { get; set; }
           
            public List<Product> products { get; set; }
            public string requestID { get; set; }
            public int sysTransId { get; set; }
            public string signature { get; set; }
        }
        public class Product
        {
            public int productId { get; set; }
            public int productValue { get; set; }
            public object categoryName { get; set; }
            public object serviceProviderName { get; set; }
            public double commission { get; set; }
            public List<Softpin> softpins { get; set; }
        }
        public class Softpin
        {
            public int softpinId { get; set; }
            public string softpinSerial { get; set; }
            public string softpinPinCode { get; set; }
            public string expiryDate { get; set; }
        }
        public class BuyCardRequest
        {
            public int operation { get; set; } //1000
            public string username { get; set; }
            public string keyBirthdayTime { get; set; }
            public string requestID { get; set; }

            public List<BuyCardProduct> buyItems { get; set; }
            public string signature { get; set; }
            public string token { get; set; }
        }
        public class BuyCardProduct
        {
            public int productId { get; set; }
            public int quantity { get; set; }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
       
    }
}