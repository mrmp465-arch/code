using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass()]
    public class TelegramClientTests
    {
        [TestMethod()]
        public void TelegramClientTest()
        {
            var tele = new TelegramClient();
        }

        [TestMethod()]
        public void TelegramSendMessageTest()
        {
            var t = Task.Run(() => TelegramClient.TelegramSendMessage(-318818065, "Xin chào! Em là BB2D Bot"));
            t.Wait();
            Console.WriteLine("ok");
        }

        [TestMethod()]
        public void TelegramSendMessageNotify()
        {
            var partners = new Dictionary<string, int>();
            partners.Add("ssg", -301212470);
            partners.Add("khan3", -241414683);
            partners.Add("mrht", -250062167);
            partners.Add("nupa", -248182822);
            partners.Add("hng2", -242121840);
            partners.Add("mrx", -279274321);
            partners.Add("hng", -229242074);
            //partners.Add("glb", -262646958);
            partners.Add("big", -312581781);
            partners.Add("nut", -228950608);
            partners.Add("tcar", -305533893);

            //string content = "Hôm nay sản lượng rất là nhiều các anh cứ đẩy mạnh mẽ nhé :)";
            //string content = "Hệ thống hiện tại đang hết sản lượng thẻ VTT thẻ nhỏ. Để đảm bảo dịch vụ chạy ổn định các anh đẩy giúp các thẻ mệnh giá từ 50k trở lên ! Cảm ơn";
            //string content = "Hệ thống hiện tại đang hết sản lượng thẻ VTT các anh tạm nghỉ chút, cho đến khi có thông báo lại";
            //string content = "Hệ thống bảo trì trong vòng x phút và Reject mọi request";
            //string content = "Hệ thống đã sãn sàng các anh đẩy lại thẻ giúp";
            //string content = "Các anh ơi hôm nay sản lượng đầu vào thấp hơn hôm qua nhiều các anh xem đẩy mạnh mẽ lên ợ !";
            //string content = "Hiện tại hệ thống Viettel đang chập chờn!. Để tránh các lỗi phát sinh bên BB2D tạm đóng thẻ Viettel. Chúng tôi sẽ thông báo lại khi hệ thống ổn định !";
            string content = "Hệ thống Viettel đã ổn định, chúng tôi đã mở lại thẻ viettel! Thông báo các anh nắm thông tin";
            //string content = "Cập nhật thêm tình hình bên Viettel update nâng cấp nên dẫn đến việc chập chờn như hiện tại, các anh thông cảm ! Co thông tin mới sẽ báo cáo các anh sau.";
            //string content = "Hệ thống MobiFon đã hoạt động lại. Thông báo các anh nắm thông tin!";
            //string content = "Dear các anh! BB2D chính thức mở thanh toán cho thẻ MobiFone gạch nhanh (1s-3s) như vina, viettel. Thông báo để các anh nắm thông tin, chi tiết các anh trao đổi với đầu mổi conteact để có thêm thông tin chi tiết.";
            //string content = "Sản lượng thẻ Viettel dưới 50k đang hết, các anh đầy giúp thẻ từ 50k trở lên !";
            // string content = "Dear các anh! Như đã thông báo hệ thống chính thức áp dụng từ 0:00:00 11/10/2018 không xử lý thẻ Viettel nhỏ dưới 50K . Thông báo các anh nắm thông tin";
            //string content = "Tình hình VinaPhone đang gặp vấn đề về kết nối! BB2D sẽ tạm đóng thanh toán VinaPhone. Thông báo các anh nắm thông tin. ";

            foreach (var p in partners)
            {
                var t = Task.Run(() => TelegramClient.TelegramSendMessage(p.Value, content));
                t.Wait();
                Console.WriteLine(t.Status);
                Thread.Sleep(1000);
            }


        }
    }
}