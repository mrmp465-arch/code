using System;
using System.Web.Script.Serialization;
using Libs.TopupPartner;
using Libs.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class XBomTest
    {
        [TestMethod]

        public void usecard()
        {

            string urlService = "http://27.118.16.46:1581/VPGService.asmx";
            string partnerKey = "5789ffd45c1e1bf014c9141e4cc6098d";
            string partnerCode = "a68";
            string serviceCode = "cardtelco";
            string commandCode = "usecard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                CardSerial = "062971000003033",
                CardCode = "437959854039",
                CardType = "vms",
                AccountName = "a68_0001",
                //AppCode = "xxx",
                //RefCode = "xxx"
            });

            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);

            VPGService _VPGService = new VPGService(urlService);
            string serviceResponse = string.Empty;
            serviceResponse = _VPGService.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
            Console.WriteLine(serviceResponse);

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

    }
}
