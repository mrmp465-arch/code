using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Card.Data.DTO;
using Card.Data.Service;
using Newtonsoft.Json;
using Card.Data.Api;

namespace Card.Test
{
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void TestMethod1()
        {
           
            var token = ServerProcess.GetUserTokenCache("toan_dl1_api", "toan@123");
            Console.WriteLine(JsonConvert.SerializeObject(token));
        }
    }
}
