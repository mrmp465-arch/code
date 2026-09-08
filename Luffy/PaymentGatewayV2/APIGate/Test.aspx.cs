using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace APIGate
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var service = new GateService.CardToCashClient();
                CardRequest cardRequest = new CardRequest();
                cardRequest.partnerCode = "GateC2cHN";
                cardRequest.serial = "CK01981877";
                cardRequest.pinCode = "7958472174";

                cardRequest.serial = Encrypt(cardRequest.serial);
                cardRequest.pinCode = Encrypt(cardRequest.pinCode);
                cardRequest.clientDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                cardRequest.partnerTransId = "123";
                cardRequest.telcoCode = "GT";

                cardRequest.sign = cardRequest.partnerTransId + cardRequest.partnerCode + cardRequest.telcoCode + cardRequest.pinCode + cardRequest.serial + cardRequest.clientDateTime;

                cardRequest.sign = CreateSignRSA(cardRequest.sign);
                NLogLogger.Info(new string[] { "MoBo", "MoBoRequest", serializer.Serialize(cardRequest) });
                
                var result = service.exchangeCard(serializer.Serialize(cardRequest));

                NLogLogger.Info(new string[] { "MoBo", "result", result });
            }
        }
        public static string Encrypt(string data)
        {
            data = data.Trim();
            string key = "iMi1c4vKxA6R8ja909Tdn9cnRLzTO2V2OGvA47O7UIe5fhDln8OjLE105bxYakFV";
            if (string.IsNullOrEmpty(data))
                return "Input string is empty!";

            byte[] keydata = Encoding.ASCII.GetBytes(key);

            string md5String = BitConverter.ToString(new

            MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();

            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));

            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();

            tripdes.Mode = CipherMode.ECB;

            tripdes.Key = tripleDesKey;

            tripdes.GenerateIV();

            MemoryStream ms = new MemoryStream();

            CryptoStream encStream = new CryptoStream(ms, tripdes.CreateEncryptor(), CryptoStreamMode.Write);

            encStream.Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetByteCount(data));

            encStream.FlushFinalBlock();

            byte[] cryptoByte = ms.ToArray();

            ms.Close();

            encStream.Close();

            return Convert.ToBase64String(cryptoByte, 0, cryptoByte.GetLength(0)).Trim();
        }


        public string CreateSignRSA(string data)
        {
            RSACryptoServiceProvider rsaCryptoIPT = new RSACryptoServiceProvider(1024);
            var privateKey = "<RSAKeyValue><Modulus>fHUZxEOqslaXGZknptnnPOl9gfcTq9O72LqImGOnu8vzKxustdUSHHF/djJpQ5IpvBvfKHS7KDJnM0XTfbbhr+cum6gCGMbvWds7f+PG7RtpgbyIODbEcQLd+EF04b4ZMEWa8qK2CvvfZuzct8yYmbHXQWtfN4v49+IYl30NNTc=</Modulus><Exponent>AQAB</Exponent><P>zovIF/sqsIlvngQn1N9L7qsPnGoAn5Goybp8H//QbZsh7zymiABet1LAnBsrP5tcx5zhhmK2Jp6PFZI5BKN8Ow==</P><Q>mkG011m8EccD+C7houR00eK3jVgqZLQ7YhlTCHJG69Gfz7FxTVhhsD5xc//T+o/OS72M9ZBf77Gr9c2zEt6nNQ==</Q><DP>A+k/s7yM1WT02SApBO3piS5yEDstodfnQ1KlsRFuB/VNEjOqz556LKyDezFEg2LUwe7lkl7iJh8QhI8NR1o1Kw==</DP><DQ>OGXXkXi8ex9xQxcY5e5zC53BkErwzPrcT0SFCCyH44Yz0MLGcwniRYa/AjNBtHVJboWx+MI9YSxre6YmV7rSFQ==</DQ><InverseQ>bGcM1miNe/MovfmN38xtaLwFWJThU7ZQX8ZPWEgV0KRLH0P3xMkWGWb6a1xfSFssKQGm29IT5lVmvPq2RQaO2w==</InverseQ><D>Io6NMf6r5cq/N79b0CtO1o3qaGzHTH2E+f+JL+7zwuQ1R7HueaqI1db50kLbEYUDC5UdmGkZEmc0Wns35v8hmMbEhrg2dt3rlPp0CBWjp+wueJaKYV2037xRuOmV6cacTjAczMAa+DtHn8vwCiS2N/HPrO4Hsx6tVIm2Svjt7Wk=</D></RSAKeyValue>";
            rsaCryptoIPT.FromXmlString(privateKey);
            return Convert.ToBase64String(rsaCryptoIPT.SignData(new ASCIIEncoding().GetBytes(data), new SHA1CryptoServiceProvider()));
        }

        public static string Encryption(string strText)
        {
            var publicKey = "<RSAKeyValue><Modulus>fHUZxEOqslaXGZknptnnPOl9gfcTq9O72LqImGOnu8vzKxustdUSHHF/djJpQ5IpvBvfKHS7KDJnM0XTfbbhr+cum6gCGMbvWds7f+PG7RtpgbyIODbEcQLd+EF04b4ZMEWa8qK2CvvfZuzct8yYmbHXQWtfN4v49+IYl30NNTc=</Modulus><Exponent>AQAB</Exponent><P>zovIF/sqsIlvngQn1N9L7qsPnGoAn5Goybp8H//QbZsh7zymiABet1LAnBsrP5tcx5zhhmK2Jp6PFZI5BKN8Ow==</P><Q>mkG011m8EccD+C7houR00eK3jVgqZLQ7YhlTCHJG69Gfz7FxTVhhsD5xc//T+o/OS72M9ZBf77Gr9c2zEt6nNQ==</Q><DP>A+k/s7yM1WT02SApBO3piS5yEDstodfnQ1KlsRFuB/VNEjOqz556LKyDezFEg2LUwe7lkl7iJh8QhI8NR1o1Kw==</DP><DQ>OGXXkXi8ex9xQxcY5e5zC53BkErwzPrcT0SFCCyH44Yz0MLGcwniRYa/AjNBtHVJboWx+MI9YSxre6YmV7rSFQ==</DQ><InverseQ>bGcM1miNe/MovfmN38xtaLwFWJThU7ZQX8ZPWEgV0KRLH0P3xMkWGWb6a1xfSFssKQGm29IT5lVmvPq2RQaO2w==</InverseQ><D>Io6NMf6r5cq/N79b0CtO1o3qaGzHTH2E+f+JL+7zwuQ1R7HueaqI1db50kLbEYUDC5UdmGkZEmc0Wns35v8hmMbEhrg2dt3rlPp0CBWjp+wueJaKYV2037xRuOmV6cacTjAczMAa+DtHn8vwCiS2N/HPrO4Hsx6tVIm2Svjt7Wk=</D></RSAKeyValue>";
            var testData = Encoding.UTF8.GetBytes(strText);

            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    // client encrypting data with public key issued by server                    
                    rsa.FromXmlString(publicKey.ToString());

                    var encryptedData = rsa.Encrypt(testData, true);

                    var base64Encrypted = Convert.ToBase64String(encryptedData);

                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

      
        public class CardRequest
        {
            public string partnerCode { get; set; }
            public string serial { get; set; }
            public string telcoCode { get; set; }
            public string pinCode { get; set; }
            public string clientDateTime { get; set; }
            public string sign { get; set; }
            public string partnerTransId { get; set; }
        }
    }
}