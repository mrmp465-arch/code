using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class AsynTest
    {
        [TestMethod]
        public void PostAppForm()
        {
            //DoTaskOne();
            //DoTaskTwo();

            //Console.WriteLine(AccessTheWebAsync().Result);
            //Console.WriteLine(DoIndependentWork());
            Console.WriteLine(AccessTheWebAsync().Result);


        }

        public async Task<int> AccessTheWebAsync()
        {
            HttpClient client = new HttpClient();

            // GetStringAsync returns a Task<string>. That means that when you await the
            Task<string> getStringTask = client.GetStringAsync("https://bb2d.com/");

            // You can do work here that doesn't rely on the string from GetStringAsync.
            DoIndependentWork();

            //  - The await operator then retrieves the string result from getStringTask.
            string urlContents = await getStringTask;

            // The return statement specifies an integer result.
         
            // Any methods that are awaiting AccessTheWebAsync retrieve the length value.
            return urlContents.Length;
        }

        public void DoIndependentWork()
        {
            Console.WriteLine("DoIndependentWork");
        }
    }
}
