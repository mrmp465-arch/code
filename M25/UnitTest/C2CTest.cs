using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libs.CardTelco.C2C.KHD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass()]
    public class C2CTest
    {
        [TestMethod()]
        public void GenerateKeysTest()
        {
            var rs = CryptoRSA.GetSignature("AVC", @"D:\C2C\private.pem");
            Console.WriteLine(rs);

            //var rs = CryptoRSA.RsaEncryptWithPrivate("AVC", "D:\\C2C\\private.ber");
            //Console.WriteLine(rs);

        }
    }
}
