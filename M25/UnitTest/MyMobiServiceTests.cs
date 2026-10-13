using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIMyMobi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using APIMyMobi.Entity;

namespace UnitTest
{
    [TestClass()]
    public class MyMobiServiceTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        [TestMethod()]
        public void TopupCardTest()
        {
            var result = MyMobiService.TopupCard("081601000000204", "061370983191", "0936999961", 0, "0936999961", "hoangle273", "");
            Console.Write(serializer.Serialize(result));
        }

        [TestMethod()]
        public void LoginTest()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var loginRes = "{\"data\":null,\"errors\":[{\"code\":\"00\",\"impact\":\"\",\"message\":\"Xảy ra lỗi hệ thống. Vui lòng thử lại\"}]}";
            var loginObj = serializer.Deserialize<Login>(loginRes);

            if (loginObj.errors == null)
            {
                Console.Write(serializer.Serialize("sucesss"));
            }
            else
            {
                if (loginRes.Contains("Xảy ra lỗi hệ thống"))
                {
                    Console.Write(serializer.Serialize("Xảy ra lỗi hệ thống"));
                }
                
            }
            
        }
        
    }
}