using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass()]
    public class RetrieveEmailServiceTests
    {
        [TestMethod()]
        public void ProcessRetrieveEmailTest()
        {
          EmailService.RetrieveEmailService.ProcessRetrieveEmail();
        }
    }
}