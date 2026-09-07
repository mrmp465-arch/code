using Microsoft.VisualStudio.TestTools.UnitTesting;
using WService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WService.Tests
{
    [TestClass()]
    public class AutoBuyCardServiceTests
    {
        [TestMethod()]
        public void ProcessAutoBuyCardTest()
        {
            //var auto = new AutoBuyCardService();
            AutoBuyCardService.ProcessAutoBuyCard();
        }

        [TestMethod()]
        public void ProcessAutoTopupTest()
        {
            AutoBuyCardService.ProcessAutoTopup();
        }
    }
}