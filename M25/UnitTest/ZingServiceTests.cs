using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using APIGame.Entity;

namespace UnitTest
{
    [TestClass()]
    public class ZingServiceTests
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        [TestMethod()]
        public void GetTopupTest()
        {
            var login = new PaymentMZingCardResult();
            var result = "{\"returnCode\":-4,\"returnMessage\":\"Thời gian truy cập hết hạn hoặc không chính xác\",\"returnMessage_\":\"INVALID_TOKEN\",\"data\":null}";
            login = serializer.Deserialize<PaymentMZingCardResult>(result);
            Console.WriteLine(login.returnCode);
        }

        //[TestMethod()]
        public void TopupCardTest()
        {
            var topup = ZingService.TopupCard("AB0070795613", "AXTXT67NM", "zing", "trungdtdev", "trunghoa", "1", "1", 7, "{\"serverID\":\"20148\",\"roleID\":\"155211256\",\"productID\":\"com.vng.jxm.item2\",\"roleName\":\"155211256\",\"amount\":\"50000\"}");
            Console.WriteLine(serializer.Serialize(topup));
        }

        [TestMethod()]
        public void GetRoleMTest()
        {
            //var getTopup = ZingService.GetServerM("trungdtdev", "trunghoa", 7);
            var login = ZingService.GetLoginM("Dinonguyen296", "Dino@296", 8);
            var getServer = ZingService.GetServerM("Dinonguyen296", "Dino@296", 8);
            var getRole = ZingService.GetRoleM("Dinonguyen296", "Dino@296", 8, "30010323");
            Console.WriteLine(serializer.Serialize(getRole));
        }

        [TestMethod()]
        public void TopupMTest()
        {
            //var getTopup = ZingService.TopupM("XB0073299788", "DPR83P4DW", "trungdtdev", "trunghoa", "1", "1", 7, "{\"serverID\":\"20148\",\"roleID\":\"155211256\",\"productID\":\"com.vng.jxm.item2\",\"roleName\":\"155211256\",\"amount\":\"50000\"}");
            var getTopup = ZingService.TopupM("XB0073299788", "DPR83P4DW", "Dinonguyen296", "Dino@296", "1", "1", 8, "{\"serverID\":\"30010323\",\"roleID\":\"8260001001394-1586653049251.vng\",\"productID\":\"com.pp.dt3q.item8\",\"roleName\":\"30010323\",\"amount\":\"2000000\"}");
            
            Console.WriteLine(getTopup);
        }

        [TestMethod()]
        public void GetLoginMTest()
        {
            var login = ZingService.GetLoginM("Dinonguyen296", "Dino@296", 8);
            //var login = ZingService.GetLoginM("trungdtdev", "trunghoa", 7);
            Console.WriteLine(login);
        }

        [TestMethod()]
        public void GetServerMTest()
        {
            var login = ZingService.GetLoginM("Dinonguyen296", "Dino@296", 8);
            var getServer = ZingService.GetServerM("Dinonguyen296", "Dino@296", 8);
            Console.WriteLine(serializer.Serialize(getServer));
        }


    }
}