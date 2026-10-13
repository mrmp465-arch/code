using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass()]
    public class CardAPILogTests
    {
        [TestMethod()]
        public void GetTableListTest()
        {
            var cardAPILog = new CardAPILog();
            var logList = cardAPILog.GetTableList(10, string.Empty, DateTime.Now, null, "viettel", string.Empty);
            foreach (var log in logList)
            {
                Console.WriteLine(log.Provider);
            }

        }
    }
}