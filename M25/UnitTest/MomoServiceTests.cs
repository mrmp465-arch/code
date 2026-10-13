using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIMomo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace APIMomo.Tests
{
    [TestClass()]
    public class MomoServiceTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        [TestMethod()]
        public void DoLoginTest()
        {

            var login = MomoService.DoLogin("0912440644", "868481", 1);
            Console.WriteLine(serializer.Serialize(login));
        }
    }
}