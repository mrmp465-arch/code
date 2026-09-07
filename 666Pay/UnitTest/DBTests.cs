using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libs.API;

namespace UnitTest
{
    [TestClass()]
    public class DBTests
    {
        [TestMethod()]
        public void GetTest()
        {
            //var topup = new TopupMobile3rdLog();
            //topup.Id = 1;
            //topup.Get();
            //Console.WriteLine(topup.Status);

            var order = new Orders();
            order.Id = 147;
            var a = order.Get();
            Console.WriteLine(a.No);

        }

        [TestMethod()]
        public void UserDL2Test()
        {
            var UserID = 46;
            var lstUsers = new Users().GetList();
            lstUsers = lstUsers.Where(e => e.IsTopup == 1 && e.ParentId == UserID || e.UserID == UserID).ToList();
            foreach (var user in lstUsers)
            {
               Console.WriteLine(user.UserName);
            }

        }
    }
}