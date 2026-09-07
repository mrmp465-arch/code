using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lib.Captcha.Anticaptcha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass()]
    public class AnticaptchaServiceTests
    {
        [TestMethod()]
        public void NoCaptchaTaskProxylessTest()
        {
            var result = new AnticaptchaService().NoCaptchaTaskProxyless("https://vtcgame.vn/nap-vcoin/qua-the-cao.html", "6LeFvJsUAAAAAOEszZLFnZKuLn_lFLZHtnPP5djw");
            Console.WriteLine(result);
        }
    }
}