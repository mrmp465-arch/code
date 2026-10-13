using System;
using Libs.TopupPartner.VNPTEPAY;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libs.TopupPartner;

namespace UnitTest
{
    [TestClass]
    public class VNTPEpayTest
    {
        [TestMethod]
        public void downloadSoftpin()
        {
            EpayService es = new EpayService();
            var requestid = TopupConstant.TopupPartner + DateTime.Now.ToString("yyyyMMddHHmmss");
            var result = es.downloadSoftpin(requestid, "VTT", 10000, 1);
            Console.WriteLine(result);
        }


    }
}
