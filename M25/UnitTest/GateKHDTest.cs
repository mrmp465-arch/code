using System;
using Libs.TopupPartner.VNPTEPAY;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libs.API;
using Libs.CardTelco.Gate.KHD;
using Libs.Utils;

namespace UnitTest
{
    [TestClass]
    public class GateKHDTest
    {
        protected string webserviceUrl = "https://daily.gate.vn/api/card/ws.asmx";
        protected string username = "01222882493";
        protected string password = "r39AxhXK6tY93jSq";

        [TestMethod]
        public void ChargeCard()
        {

        var rq = new RequestData();
            rq.Username = username;
            rq.Password = password;
            rq.Serial = "062821000001060";
            rq.PinCode = "988619811495";
            rq.CardName = "MOBIFONE";
            rq.Timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            rq.Signature = Encrypts.MD5(String.Format("{0}{1}{2}{3}{4}", username, password, rq.Serial, rq.PinCode, rq.CardName));
            
            ws sb = new ws(webserviceUrl);
            //string gateResponse = sb.ChargeCard(rq.Username, rq.Password, rq.Serial, rq.PinCode, rq.CardName, rq.Timestamp.ToString(), rq.Signature); //Output : Trả về giá trị của chuỗi với định dạng (ErrorCode|Description|TransactionID|PartnerTransactionID|CardAmount) 
            //Console.WriteLine(gateResponse);
        }

        public class RequestData
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Serial { get; set; }
            public string PinCode { get; set; }
            public string CardName { get; set; }
            public int Timestamp { get; set; }
            public string Signature { get; set; }

        }

        [TestMethod]
        public void BuyCardEncrypt()
        {

            string sPin = "0F71A84100E8A8E2B083714F5D978011";
            Encryption.GEncryption en = new Encryption.GEncryption();
            var gateResponset = en.GDecrypt(sPin);
            Console.WriteLine(gateResponset);
        }
    }
}
