using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Libs.TopupPartner;
using Libs.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class BuyCardTest
    {
       


        [TestMethod]

        public void buycardXBom()
        {

            string urlService = "https://apicard.atheriz.xyz/VPGService.asmx";
            //string urlService = "http://localhost:8080//VPGService.asmx";
            string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
            string partnerCode = "pp";
            string serviceCode = "buycard";
            string commandCode = "buycard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new BuyCardRequest()
            {
                Provider = "vnp",
                Amount = 10000,
                Quantity = 1,
                AccountName = "pl001",
                AccountId = 1,
                OrderNo = "001"
            });

            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            VPGService _VPGService = new VPGService(urlService);
            string serviceResponse = string.Empty;
            //serviceResponse = "{\"ResponseCode\":1,\"Description\":\"Thành công\",\"ResponseContent\":[{\"Serial\":\"58607706686\",\"Pin\":\"4546351748422\",\"ExpireDate\":""}],\"Signature\":\"52e1923da198a8cf6108ffe349dacc99\"}";
            serviceResponse = _VPGService.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
            Console.WriteLine(serviceResponse);
            //var objectrs = serializer.Deserialize<BuyCardRespone>(serviceResponse);
            //Console.WriteLine(serializer.Serialize(objectrs));

        }

        [TestMethod]

        public void TranferXBom()
        {

            string urlService = "https://apicard.atheriz.xyz/VPGService.asmx";
            //string urlService = "http://localhost:8080//VPGService.asmx";
            string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
            string partnerCode = "pp";
            string serviceCode = "topuptelco";
            string commandCode = "tranfer";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new TranferRequest()
            {
                Provider = "vtt",
                Amount = 100000,
                AccountName = "pl001",
                AccountId = 1,
                OrderNo = "001",
                //SimTarget = "0987640170"
                //SimTarget = "0838332187" //Bin
                //SimTarget = "0354884815" //Bon
                SimTarget = "0398103845"
            });

            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            VPGService _VPGService = new VPGService(urlService);
            string serviceResponse = string.Empty;
            //serviceResponse = "{\"ResponseCode\":1,\"Description\":\"Thành công\",\"ResponseContent\":[{\"Serial\":\"58607706686\",\"Pin\":\"4546351748422\",\"ExpireDate\":""}],\"Signature\":\"52e1923da198a8cf6108ffe349dacc99\"}";
            serviceResponse = _VPGService.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
            Console.WriteLine(serviceResponse);
            //var objectrs = serializer.Deserialize<BuyCardRespone>(serviceResponse);
            //Console.WriteLine(serializer.Serialize(objectrs));

        }

        [TestMethod]
        public void checkStoreXBom()
        {

            string urlService = "https://apicard.thenhanh.shop/VPGService.asmx";
            string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
            string partnerCode = "pp";
            string serviceCode = "buycard";
            string commandCode = "checkstore";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new BuyCardRequest()
            {
                Provider = "vms",
                Amount = 100000,
                Quantity = 5
                
            });

            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            VPGService _VPGService = new VPGService(urlService);
            string serviceResponse = string.Empty;
            //serviceResponse = "{\"ResponseCode\":1,\"Description\":\"Thành công\",\"ResponseContent\":[{\"Serial\":\"58607706686\",\"Pin\":\"4546351748422\",\"ExpireDate\":""}],\"Signature\":\"52e1923da198a8cf6108ffe349dacc99\"}";
            serviceResponse = _VPGService.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
            Console.WriteLine(serviceResponse);
            //var objectrs = serializer.Deserialize<BuyCardRespone>(serviceResponse);
            //Console.WriteLine(serializer.Serialize(objectrs));

        }


        [TestMethod]

        public void buycardA68()
        {

            string urlService = "http://27.118.16.49:1581/VPGService.asmx";
            string partnerKey = "8a7e933170658b3e369fb61d104d5303";
            string partnerCode = "a68";
            string serviceCode = "buycard";
            string commandCode = "buycard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new BuyCardRequest()
            {
                Provider = "VTT",
                Amount = 20000,
                Quantity = -1,
                AccountName = "dtt01",
                AccountId = 1,
                OrderNo = "XYZXYZXYZ08"
            });

            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);

            VPGService _VPGService = new VPGService(urlService);
            string serviceResponse = string.Empty;
            serviceResponse = _VPGService.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
            Console.WriteLine(serviceResponse);

        }

        [TestMethod]

        public void buycardHandlerTest()
        {
            var handler = BuyCardFactory.GetHandler("undefined");
            string responseData = string.Empty;
            var result = handler.downloadSoftpin("abcd", "vms", 10000, 1, "", "", ref responseData);
            Console.WriteLine("Code:" + result.ResponseCode);
        }

        [TestMethod]

        public void buycardDuDu()
        {

            string urlService = "http://api.dudu5.club/payment/VPGService.asmx";
            string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
            string partnerCode = "dudu";
            string serviceCode = "buycard";
            string commandCode = "buycard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new BuyCardRequest()
            {
                Provider = "VNP",
                Amount = 10000,
                Quantity = 1,
                AccountName = "dd001",
                AccountId = 1,
                OrderNo = "0005"
            });

            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);

            VPGService _VPGService = new VPGService(urlService);
            string serviceResponse = string.Empty;
            serviceResponse = _VPGService.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
            Console.WriteLine(serviceResponse);

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

        public class UseCardRequest
        {
            public string CardSerial { get; set; }
            public string CardCode { get; set; }
            public string CardType { get; set; }
            public string AccountName { get; set; }
            //public string AppCode { get; set; }
            //public string RefCode { get; set; }
        }

        //public class CardtelcoRequest
        //{
        //    public string partnerCode { get; set; }
        //    public string serviceCode { get; set; }
        //    public string commandCode { get; set; }
        //    public string requestContent { get; set; }
        //    public string signature { get; set; }
        //}


        public class BuyCardRequest
        {
            public string Provider { get; set; } // CardType
            public int Amount { get; set; }
            public int Quantity { get; set; }
            public string AccountName { get; set; }
            public long AccountId { get; set; }
            public string OrderNo { get; set; }
        }
        public class TranferRequest
        {
            public string Provider { get; set; } // CardType
            public int Amount { get; set; }
            public string SimTarget { get; set; }
            public string AccountName { get; set; }
            public long AccountId { get; set; }
            public string OrderNo { get; set; }
        }
    }
}
