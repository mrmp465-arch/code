using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIGame;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using HtmlAgilityPack;

namespace APIGame.Tests
{
    [TestClass()]
    public class GateServiceTests
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod()]
        public void GetTopupTest()
        {
            //var result = GateService.GetTopup(1, "0325581017", "123@456");
            //Console.WriteLine(result.IsTopup);

        }

        [TestMethod()]
        public void TopupCardTest()
        {
            var result = GateService.TopupCard(1, "CA01239064", "3133213459", "0325581017", "0325581017", "123@456");
            Console.WriteLine(serializer.Serialize(result));
        }


        [TestMethod()]
        public void ProxyTest()
        {
            Console.WriteLine("To enable your free eval account and get CUSTOMER, " + "YOURZONE and YOURPASS, please contact sales@brightdata.com");
            Console.WriteLine("Performing request(s)");
            var client = new Client("vn");
            // Put full scraping sequence below:
            Console.WriteLine(client.DownloadString("https://pay.gate.vn/"));
            // client.DownloadString(...second request...);
        }

        [TestMethod()]
        public void BuyCardMasterTest()
        {
            var result = GateService.BuyCardMaster(1, "vnp", 10000, 1, "0336881335", "123@456");
            Console.WriteLine(serializer.Serialize(result));
        }

        [TestMethod()]
        public void ExportCardTest()
        {

            var Json =
                "[{\"Id\":1,\"Name\":\"Tự động mua mã thẻ\",\"Code\":\"AutoBuyCard\",\"Status\":1},\r\n{\"Id\":2,\"Name\":\"Tự động gộp tiền\",\"Code\":\"AutoCombine\",\"Status\":1},\r\n{\"Id\":3,\"Name\":\"Tự động san tiền\",\"Code\":\"AutoShare\",\"Status\":1}]";


            var result =
                "Ten the\tSo Serial\tPin Code\tNgay het han\tNgay in\tMa giao dich\r\nVina 10000\t59000019613055\t06906687982820\t\t3/18/2022 7:03:27 PM\tGP20220318070300695070\r\n";

            //Bóc thẻ trả về
            char[] lineDelimiter = new char[] { '\n' };
            string[] line = result.Trim().Split(lineDelimiter);
            char[] delimiter = new char[] { '\t' };
            var cardList = new List<CardDVO>();
            for (int i = 1; i < line.Length; i++)
            {
                string[] colum = line[i].Split(delimiter);

                cardList.Add(new CardDVO()
                {
                    Name = colum[0],
                    Serial = colum[1],
                    Pin = colum[2]
                });
            }

            Console.WriteLine(serializer.Serialize(cardList));
        }

        [TestMethod()]
        public void ExportCard1Test()
        {

            var result = "2022-03-31 19:19:54 Add log | 2022-03-31 19:23:16 {\"ResponseCode\":1,\"Description\":\"Transaction is successful\",\"ResponseContent\":\"[{\\\"Name\\\":\\\"viettel 50000\\\",\\\"Serial\\\":\\\"10008752831919\\\",\\\"Pin\\\":\\\"118127824328215\\\"}]\",\"Signature\":\"\"}";
            Console.WriteLine(result.Length);
            Console.WriteLine(result.IndexOf('{'));
            Console.WriteLine(result.Length - result.IndexOf('{'));
            result = result.Substring(result.IndexOf('{'), result.Length - result.IndexOf('{'));
            Console.WriteLine(result);
        }

        [TestMethod()]
        public void Getbalance()
        {

            var result = "exdaAc";
            var strAmount = Regex.Match(result, @"\d+").Value.Trim().Replace(",", "");
            Console.WriteLine(strAmount);
        }

        [TestMethod()]
        public void CheckHistory()
        {
            //var timeRequest = DateTime.Now;
            var timeRequest = DateTime.ParseExact("02/06/2022 11:10:30", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine("Time Request:" + timeRequest.ToString());


            var result = "\r\n\r\n<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head id=\"Head1\"><meta http-equiv=\"Content-Type\" content=\"text/html;charset=UTF-8\" /><meta http-equiv=\"Content-Language\" content=\"en-US\" /><meta name=\"DISTRIBUTION\" content=\"GLOBAL\" /><meta name=\"AUTHOR\" content=\"pay.gate.vn\" /><meta name=\"COPYRIGHT\" content=\"Copyright (c) by FPT Online\" /><meta name=\"RATING\" content=\"GENERAL\" /><title>\r\n\tGate Pay - Thanh toan Bac Gate cho cac san pham, game FPT Online\r\n</title><meta name=\"keywords\" content=\"bac gate, the gate, the cao, nap bac gate nhanh, nap bac vao game nhanh, nap the nhanh, nap bac bao dien thoai nhanh, chuyen bac gate vao game, thanh toan dich vu, mua the online, nap tien choi game, game FPT Online, gatepay, gate pay, nap the gate, nap bac gate the cao, san pham, nap the game, mua the cao di dong, nap game, mua the game, nap tien sim 3g, san pham FPT Online\" /><meta name=\"description\" content=\"Nạp Bạc Gate vào game, thanh toán Bạc Gate game FPT Online và thanh toán nhiều dịch vụ tiện ích\" />\r\n    <!--Css-->\r\n    <link rel=\"stylesheet\" type=\"text/css\" href=\"/Contents/css/style.css\" /><link rel=\"stylesheet\" type=\"text/css\" href=\"/Contents/css/jquery.mCustomScrollbar.css\" />\r\n    <!--JavaScript-->\r\n\r\n    <script type=\"text/javascript\" src=\"/Contents/js/jquery-1.7.2.min.js\"></script>\r\n\r\n    <!--Thư viên Jquery-->\r\n\r\n   \r\n\r\n    <script type=\"text/javascript\" src=\"/Contents/js/slidebar.tab.js\"></script>\r\n\r\n    <!--Slider Tab-->\r\n\r\n    <script type=\"text/javascript\" src=\"/Contents/js/modernizr.custom.53451.js\"></script>\r\n\r\n    <script type=\"text/javascript\" src=\"/Contents/js/jquery.gallery.js\"></script>\r\n\r\n    <script type=\"text/javascript\" src=\"/Contents/js/jquery.mCustomScrollbar.js\"></script>\r\n\r\n    <!--Scroll-bar Main-->\r\n\r\n    <script type=\"text/javascript\" src=\"/Contents/js/jquery-ui-1.8.21.custom.min.js\"></script>\r\n\r\n    <script type=\"text/javascript\" src=\"/Contents/js/jquery.easing.1.3.min.js\"></script>\r\n\r\n    <script type=\"text/javascript\" src=\"/Contents/js/jquery.mousewheel.min.js\"></script>\r\n\r\n    <script type=\"text/javascript\" src=\"/Contents/js/functions.js\"></script>\r\n\r\n    <script type=\"text/javascript\" src=\"/Contents/js/Library.js\"></script>\r\n\r\n    \r\n\r\n\r\n     \r\n</head>\r\n<body>\r\n    <form name=\"form1\" method=\"post\" action=\"./\" id=\"form1\">\r\n<div>\r\n<input type=\"hidden\" name=\"__VIEWSTATE\" id=\"__VIEWSTATE\" value=\"/wEPDwULLTEzNDc2NDYyMjIPZBYCZg9kFgICAw9kFgQCAQ9kFgJmDw8WAh4LTmF2aWdhdGVVcmwF9wFodHRwczovL3BzcC5nYXRlLnZuL29hdXRoLmh0bWw/Y2FsbGJhY2tfdXJsPWh0dHBzOi8vcGF5LmdhdGUudm4vbG9nb3V0LmFzcHgmYXBwPW1vYm8mYWN0aW9uPWRhbmcteHVhdCZhY2Nlc3NfdG9rZW49ZXlKbllYUmxYMmxrSWpvaU5qWXpNakEwTlRBMUlpd2ljMmxuYm1GMGRYSmxJam9pYzJsbmJtRjBkWEpsTGpGbVlqUmhOak13WXpBMVpERTRZMlkwWVRjME0ySXpObU5qWXpNelpEVTFJaXdpWTJoaGJtNWxiQ0k2SWpFdGJXVWlmUT09ZGQCBw9kFgJmDxQrAAIPFgQeC18hRGF0YUJvdW5kZx4LXyFJdGVtQ291bnQCCmRkFgJmD2QWFAIBD2QWAmYPFQUBMRMwMi8wNi8yMDIyIDExOjE5OjMxCkNNMDEzMzk3ODQFVGjhursHNTAwLDAwMGQCAg9kFgJmDxUFATITMDIvMDYvMjAyMiAxMToxODoyNQpDTjAwNjA2MTcyBVRo4bq7CTEsMDAwLDAwMGQCAw9kFgJmDxUFATMTMDIvMDYvMjAyMiAxMToxMzoxNApDTTAxMzM2ODkxBVRo4bq7BzUwMCwwMDBkAgQPZBYCZg8VBQE0EzAyLzA2LzIwMjIgMTE6MDk6MzcKQ00wMTMzNjg4OQVUaOG6uwc1MDAsMDAwZAIFD2QWAmYPFQUBNRMwMi8wNi8yMDIyIDExOjA3OjE5CkNNMDEzMzY4NzIFVGjhursHNTAwLDAwMGQCBg9kFgJmDxUFATYTMDEvMDYvMjAyMiAwODoyMjowNQpDTTAxMzM5MDA3BVRo4bq7BzUwMCwwMDBkAgcPZBYCZg8VBQE3EzAxLzA2LzIwMjIgMDg6MTg6NDMKQ00wMTMzOTA0NwVUaOG6uwc1MDAsMDAwZAIID2QWAmYPFQUBOBMwMS8wNi8yMDIyIDA4OjEyOjE2CkNFMDI3MTk4OTkFVGjhursGNTAsMDAwZAIJD2QWAmYPFQUBORMwMS8wNi8yMDIyIDA4OjEwOjM5CkNKMDI1ODA1NTkFVGjhursHMTAwLDAwMGQCCg9kFgJmDxUFAjEwEzAxLzA2LzIwMjIgMDc6NDA6NDMKQ0UwMjcxNDEwNQVUaOG6uwY1MCwwMDBkGAEFHGN0bDAwJFBTUENvbnRlbnQkbHZDYXNoSW5wdXQPFCsADmRkZGRkZGQ8KwAKAAIKZGRkZgL/////D2RaTj70Bliq6HwAjjCsWzQHmNfoSg==\" />\r\n</div>\r\n\r\n<div>\r\n\r\n\t<input type=\"hidden\" name=\"__VIEWSTATEGENERATOR\" id=\"__VIEWSTATEGENERATOR\" value=\"2E9304A0\" />\r\n</div>\r\n    <div class=\"container inner\">\r\n        \r\n\r\n<script language=\"javascript\" type=\"text/javascript\">\r\n\r\n\r\n\r\n    function popup(url) {\r\n        var h = 530;\r\n        var w = 600;\r\n        var left = (screen.width / 2) - (w / 2);\r\n        var top = (screen.height / 2) - (h / 2);\r\n\r\n        newwindow = window.open(url, 'name', 'scrollbars=yes,height=' + h + ',width=' + w + ',top=' + top + ',left=' + left);\r\n        if (window.focus) {\r\n            newwindow.focus()\r\n        }\r\n        return false;\r\n    }\r\n\r\n\r\n    $(document).ready(function() {\r\n        var napbacnhanh = document.location.pathname.toLowerCase().search(\"/noauth/paymentexpress/\");\r\n        var napbacnhanh2 = document.location.pathname.toLowerCase().search(\"/business/topupmobile/\");\r\n        var chuyenbacgame = document.location.pathname.toLowerCase().search(\"/business/transfergame/\");\r\n        var thanhtoandichvu = document.location.pathname.toLowerCase().search(\"/business/partnerservices/\");\r\n        var muathe = document.location.pathname.toLowerCase().search(\"/business/shoppingcard/\");\r\n        var trangchu = document.location.pathname.toLowerCase();\r\n        var signin = document.location.pathname.toLowerCase().search(\"/signin/\");\r\n        var menu = $(\".banner .block-right\").children('li');\r\n        menu.removeClass(\"selected\");\r\n        if (napbacnhanh2 > -1 && document.location.href.toLowerCase().search(\"type=mobile\") > -1) {\r\n            var b = menu.find(\":contains('Thanh toán dịch vụ')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n        else if (napbacnhanh > -1 || napbacnhanh2 > -1) {\r\n            var b = menu.find(\":contains('Nạp bạc nhanh')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n        else if (chuyenbacgame > -1) {\r\n            var b = menu.find(\":contains('Chuyển bạc vào game')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n        else if (thanhtoandichvu > -1) {\r\n            var b = menu.find(\":contains('Thanh toán dịch vụ')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n        else if (muathe > -1) {\r\n            var b = menu.find(\":contains('Thanh toán dịch vụ')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n        else {\r\n            var b = menu.find(\":contains('Tài khoản')\");\r\n            b.parent().addClass(\"selected\");\r\n        };\r\n\r\n        if (trangchu == \"/\" || trangchu == \"/default.aspx\" || signin >= 0) {\r\n            menu.removeClass(\"selected\");\r\n        };\r\n    });\r\n    //setInterval(function() { FOConnect.logout(); return false; }, 600000);\r\n    function getLoign() {\r\n        var url = `${window.location.protocol}//${window.location.hostname}`;\r\n        if (window.location.port != 80 || window.location.port != 443) {\r\n            url = `${url}:${window.location.port}`;\r\n        }\r\n        \r\n        return popup(\"https://psp.gate.vn/oauth.html?callback_url=\" + url + \"/loginpsp.aspx&app=mobo&action=dang-nhap\");\r\n    }\r\n</script>\r\n\r\n<div id=\"topnav-wide\">\r\n    <div class=\"topnav\">\r\n        <ul class=\"block-left\">\r\n            <li><a href=\"/\">Trang chủ</a></li>\r\n            <li><a target=\"_blank\" href=\"https://diendan.gate.vn/game/forum.php\">Diễn đàn</a></li>\r\n            <li><a target=\"_blank\" href=\"/Help/\">Hỗ trợ</a></li>\r\n        </ul>\r\n        \r\n        <ul class=\"taikhoan block-right\">\r\n            <li>\r\n                <p>\r\n                    Chào <a href=\"/Business/\" style=\"padding-left: 0px\"><span class=\"ten-TK\">\r\n                        0824064612</span> </a><span style=\"color: Red\">\r\n                            [ VIP ]</span></p>\r\n            </li>\r\n           \r\n            <li><a id=\"Header_hlLogout\" href=\"https://psp.gate.vn/oauth.html?callback_url=https://pay.gate.vn/logout.aspx&amp;app=mobo&amp;action=dang-xuat&amp;access_token=eyJnYXRlX2lkIjoiNjYzMjA0NTA1Iiwic2lnbmF0dXJlIjoic2lnbmF0dXJlLjFmYjRhNjMwYzA1ZDE4Y2Y0YTc0M2IzNmNjYzMzZDU1IiwiY2hhbm5lbCI6IjEtbWUifQ==\">Thoát</a></li>\r\n        </ul>\r\n        \r\n    </div>\r\n</div>\r\n<!--End topnav-->\r\n<div id=\"banner-wide\">\r\n    <div class=\"banner\">\r\n        <div class=\"logo\">\r\n            <a href=\"/\" title=\"Cổng thanh toán pay gate\">\r\n                <img src=\"/contents/images/logo.jpg\" alt=\"pay.gate.vn\" title=\"Cổng thanh toán pay gate\" /></a>\r\n        </div>\r\n        <ul class=\"block-right\">\r\n            <li><a href=\"/mua-the-nhanh/\" title=\"Mua thẻ nhanh\">Mua thẻ nhanh</a></li>\r\n            <li><a href=\"/Business/TopupMobile/\" title=\"Nạp bạc nhanh\">Nạp bạc nhanh</a></li>\r\n            \r\n            <li><a href=\"/Business/TopupMobile/?type=mobile\" title=\"Thanh toán dịch vụ\">Thanh toán\r\n                dịch vụ</a></li>\r\n            <li><a href=\"/Business/\" title=\"Tài khoản\">Tài khoản</a></li>\r\n            <!--<li><a target=\"_blank\" title=\"Mua thẻ online\" href=\"http://daily.gate.vn/Home/policy\">\r\n                Mua thẻ online</a></li>-->\r\n        </ul>\r\n    </div>\r\n</div>\r\n<!--End banner-->\r\n\r\n        <div id=\"control-wide\">\r\n            <div class=\"control\">\r\n                \r\n    <h1 class=\"title title-c1\">\r\n        Lịch sử giao dịch</h1>\r\n\r\n            </div>\r\n        </div>\r\n        <div id=\"content-wide\">\r\n            <div class=\"content\">\r\n                <div class=\"main-content block-left\">\r\n                    <div class=\"content-inner\">\r\n                        \r\n \r\n\r\n<script type=\"text/javascript\" language=\"javascript\">\r\n    $(document).ready(function() {\r\n\r\n        var thongtin = document.location.pathname.toLowerCase();\r\n        var capnhat = document.location.pathname.toLowerCase().search(\"/business/update/\");\r\n        var lichsu = document.location.pathname.toLowerCase().search(\"/history/cashtransfer\");\r\n        var hinhthucxacminh = document.location.pathname.toLowerCase().search(\"/business/transfermethod/\");\r\n        var mobile = document.location.pathname.toLowerCase().search(\"/business/airtimesms/\");\r\n        var changepass = document.location.pathname.toLowerCase().search(\"business/passwordlevel2/change/\");\r\n\r\n        $(\".left-nav\").children('li').removeClass(\"selected\");\r\n        var a = $(\".left-nav\").children('li');\r\n\r\n        if (thongtin == '/business/') {\r\n            var b = a.find(\":contains('Thông tin')\");\r\n            b.parent().addClass(\"selected\");\r\n        };\r\n\r\n        if (capnhat >= 0) {\r\n            var b = a.find(\":contains('Cập nhập')\");\r\n            b.parent().addClass(\"selected\");\r\n        };\r\n\r\n        if (hinhthucxacminh >= 0) {\r\n            var b = a.find(\":contains('Hình thức xác minh')\");\r\n            b.parent().addClass(\"selected\");\r\n        };\r\n\r\n        if (mobile >= 0) {\r\n            var b = a.find(\":contains('Chức năng Mobile')\");\r\n            b.parent().addClass(\"selected\");\r\n        };\r\n\r\n        if (document.location.pathname.toLowerCase().search(\"/business/transfer/\") >= 0) {\r\n            var b = a.find(\":contains('Chuyển bạc cá nhân')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n\r\n        if (document.location.pathname.toLowerCase().search(\"/business/minigames/\") >= 0) {\r\n            var b = a.find(\":contains('MiniGames')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n\r\n        if (document.location.pathname.toLowerCase().search(\"/business/importgame/\") >= 0) {\r\n            var b = a.find(\":contains('Tài khoản game MU')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n\r\n        if (document.location.pathname.toLowerCase().search(\"/business/listgameid/\") >= 0) {\r\n            var b = a.find(\":contains('Tài khoản game MU')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n\r\n        if (document.location.pathname.toLowerCase().search(\"/business/listgameactivated/\") >= 0) {\r\n            var b = a.find(\":contains('Kích hoạt TK Game')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n\r\n        if (changepass >= 0) {\r\n            var b = a.find(\":contains('Thay đổi MK cấp 2')\");\r\n            b.parent().addClass(\"selected\");\r\n        }\r\n        \r\n    });\r\n</script>\r\n\r\n<ul class=\"left-nav block-left\">\r\n    <li><a href=\"/Business/\" class=\"label-tt\">Thông tin</a>\r\n        <p class=\"icon-tooltip icon07\">\r\n            <span></span>\r\n        </p>\r\n    </li>\r\n    <li><a href=\"/Business/Transfer\" class=\"label-tt\">Chuyển bạc cá nhân</a>\r\n        <p class=\"icon-tooltip icon10\">\r\n            <span></span>\r\n        </p>\r\n    </li>\r\n    <li><a href=\"/Business/TransferMethod/\" class=\"label-tt\">Bảo mật giao dịch</a>\r\n        <p class=\"icon-tooltip icon06\">\r\n            <span></span>\r\n        </p>\r\n    </li>\r\n\r\n    <!--\r\n\t<li><a href=\"/Business/ListGameActivated/\" class=\"label-tt\" title=\"Kích hoạt tài khoản Game\">\r\n        Kích hoạt TK Game</a>\r\n        <p class=\"icon-tooltip icon11\">\r\n            <span></span>\r\n        </p>\r\n    </li>\r\n\t-->\r\n</ul>\r\n\r\n\r\n                        \r\n    <div id=\"navigation\" class=\"info-card block-right\">\r\n        <ul>\r\n            <li class=\"the-cao\"><a href=\"#first\">Nạp bạc</a></li>\r\n            <li class=\"the-cao\"><a href=\"/Business/History/CashTransfer/CashOutput/\">Chuyển bạc</a></li>\r\n        </ul>\r\n        <div id=\"first\" class=\"navigation-cont\">\r\n            <p class=\"insert\">\r\n            </p>\r\n            <table class=\"tinygridview\">\r\n                <thead>\r\n                    <tr>\r\n                        <td>\r\n                            STT\r\n                        </td>\r\n                        <td>\r\n                            Ngày\r\n                        </td>\r\n                        <td>\r\n                            Tham chiếu\r\n                        </td>\r\n                        <td>\r\n                            Nguồn nạp bạc\r\n                        </td>\r\n                        <td>\r\n                            Mệnh giá\r\n                        </td>\r\n                    </tr>\r\n                </thead>\r\n                <tbody>\r\n                    \r\n                            \r\n                            <tr>\r\n                                <td>\r\n                                    1\r\n                                </td>\r\n                                <td>\r\n                                    02/06/2022 11:19:31\r\n                                </td>\r\n                                <td>\r\n                                    CM01339784\r\n                                </td>\r\n                                <td>\r\n                                    Thẻ\r\n                                </td>\r\n                                <td>\r\n                                    500,000\r\n                                </td>\r\n                            </tr>\r\n                        \r\n                            <tr>\r\n                                <td>\r\n                                    2\r\n                                </td>\r\n                                <td>\r\n                                    02/06/2022 11:18:25\r\n                                </td>\r\n                                <td>\r\n                                    CN00606172\r\n                                </td>\r\n                                <td>\r\n                                    Thẻ\r\n                                </td>\r\n                                <td>\r\n                                    1,000,000\r\n                                </td>\r\n                            </tr>\r\n                        \r\n                            <tr>\r\n                                <td>\r\n                                    3\r\n                                </td>\r\n                                <td>\r\n                                    02/06/2022 11:13:14\r\n                                </td>\r\n                                <td>\r\n                                    CM01336891\r\n                                </td>\r\n                                <td>\r\n                                    Thẻ\r\n                                </td>\r\n                                <td>\r\n                                    500,000\r\n                                </td>\r\n                            </tr>\r\n                        \r\n                            <tr>\r\n                                <td>\r\n                                    4\r\n                                </td>\r\n                                <td>\r\n                                    02/06/2022 11:09:37\r\n                                </td>\r\n                                <td>\r\n                                    CM01336889\r\n                                </td>\r\n                                <td>\r\n                                    Thẻ\r\n                                </td>\r\n                                <td>\r\n                                    500,000\r\n                                </td>\r\n                            </tr>\r\n                        \r\n                            <tr>\r\n                                <td>\r\n                                    5\r\n                                </td>\r\n                                <td>\r\n                                    02/06/2022 11:07:19\r\n                                </td>\r\n                                <td>\r\n                                    CM01336872\r\n                                </td>\r\n                                <td>\r\n                                    Thẻ\r\n                                </td>\r\n                                <td>\r\n                                    500,000\r\n                                </td>\r\n                            </tr>\r\n                        \r\n                            <tr>\r\n                                <td>\r\n                                    6\r\n                                </td>\r\n                                <td>\r\n                                    01/06/2022 08:22:05\r\n                                </td>\r\n                                <td>\r\n                                    CM01339007\r\n                                </td>\r\n                                <td>\r\n                                    Thẻ\r\n                                </td>\r\n                                <td>\r\n                                    500,000\r\n                                </td>\r\n                            </tr>\r\n                        \r\n                            <tr>\r\n                                <td>\r\n                                    7\r\n                                </td>\r\n                                <td>\r\n                                    01/06/2022 08:18:43\r\n                                </td>\r\n                                <td>\r\n                                    CM01339047\r\n                                </td>\r\n                                <td>\r\n                                    Thẻ\r\n                                </td>\r\n                                <td>\r\n                                    500,000\r\n                                </td>\r\n                            </tr>\r\n                        \r\n                            <tr>\r\n                                <td>\r\n                                    8\r\n                                </td>\r\n                                <td>\r\n                                    01/06/2022 08:12:16\r\n                                </td>\r\n                                <td>\r\n                                    CE02719899\r\n                                </td>\r\n                                <td>\r\n                                    Thẻ\r\n                                </td>\r\n                                <td>\r\n                                    50,000\r\n                                </td>\r\n                            </tr>\r\n                        \r\n                            <tr>\r\n                                <td>\r\n                                    9\r\n                                </td>\r\n                                <td>\r\n                                    01/06/2022 08:10:39\r\n                                </td>\r\n                                <td>\r\n                                    CJ02580559\r\n                                </td>\r\n                                <td>\r\n                                    Thẻ\r\n                                </td>\r\n                                <td>\r\n                                    100,000\r\n                                </td>\r\n                            </tr>\r\n                        \r\n                            <tr>\r\n                                <td>\r\n                                    10\r\n                                </td>\r\n                                <td>\r\n                                    01/06/2022 07:40:43\r\n                                </td>\r\n                                <td>\r\n                                    CE02714105\r\n                                </td>\r\n                                <td>\r\n                                    Thẻ\r\n                                </td>\r\n                                <td>\r\n                                    50,000\r\n                                </td>\r\n                            </tr>\r\n                        \r\n                        \r\n                </tbody>\r\n                \r\n            </table>\r\n            <!-- page view -->\r\n            <div class=\"paging\">\r\n                <p>\r\n                    <span style=\"margin-right: 20px\">Trang</span>  \r\n                    <a href=\"?Page=1\" class=\"page-first\" name=\"page-prev\"></a>\r\n                    <a href=\"?Page=1\" class=\"page-prev\" name=\"page-prev\"></a>\r\n                            <a href=\"#\" class=\"page-number first-child\"></a>\r\n                            <a>[1]<span></span></a><a href=?Page=2>2<span></span></a>&nbsp;&nbsp;<a href=?Page=3>3<span></span></a>&nbsp;&nbsp;<a href=?Page=4>4<span></span></a>&nbsp;&nbsp;<a href=?Page=5>5<span></span></a>&nbsp;&nbsp;<a href=?Page=6>6<span></span></a>&nbsp;&nbsp;<a>... <span></span></a>\r\n                            <a href=\"#\" class=\"page-number last-child\"></a>\r\n                    <a href=\"?Page=2\" class=\"page-next\" name=\"page-next\"></a>\r\n                    <a href=\"?Page=8\" class=\"page-last\"  name=\"page-prev\"></a>\r\n                </p>\r\n            </div>\r\n            <!-- // page view -->\r\n        </div>\r\n    </div>\r\n\r\n                    </div>\r\n                    <div class=\"ghi-chu\">\r\n                        \r\n                        \r\n                    </div>\r\n                </div>\r\n                \r\n\r\n\r\n\r\n<div class=\"info-acc block-right\">\r\n    <h2 class=\"title\">\r\n        Số dư</h2>\r\n      <p>\r\n        Số dư bạc: <span>\r\n           0</span></p>\r\n   \r\n    \r\n    <a href=\"/Business/History/CashTransfer/CashInput/\" class=\"lich-su\">Lịch sử giao dịch</a>\r\n    \r\n    <a href=\"https://psp.gate.vn/\" target=\"_blank\" class=\"change\">Thay đổi mật khẩu</a>\r\n</div>\r\n\r\n            </div>\r\n        </div>\r\n        \r\n<div id=\"footer-wide\">\r\n    <div class=\"footer\">\r\n        <div class=\"copy-right\">\r\n            \r\n\r\n            <ul class=\"block-left\">\r\n                <li>CTY CP DỊCH VỤ TRỰC TUYẾN GATE</li>\r\n                <li>Địa chỉ: 15-17, Nguyễn Cơ Thạch, phường An Lợi Đông, Thành phố Thủ Đức, Tp. Hồ Chí Minh. </li>\r\n                <!--<li>Điện thoại: (08)3.5268994 - (08)3.5268995 - (08)3.5268996 - (08)3.5268997 </li>\r\n                <li>Fax : (08)3.5268998 </li>-->\r\n            </ul>\r\n            <ul class=\"block-right\">\r\n                <li>\r\n                    <img src=\"/Contents/images/phone.png\" style=\"margin-right: 10px;\">Hotline hỗ trợ khách hàng: 0985 19 3539 </li>\r\n                <li>\r\n                    <img src=\"/Contents/images/maail.png\" style=\"margin-right: 10px; width: 14px;height: 11px\"><a href=\"mailto:hotro@gate.vn\">\r\n                        <strong>hotro@gate.vn</strong> </a></li>\r\n            </ul>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"overlay\" style=\"display: none;\">\r\n</div>\r\n<div id=\"popup_login\" class=\"popup_page\" style=\"display: none;\">\r\n    <div class=\"close_x\">\r\n    </div>\r\n    <h2 class=\"header_pop\">Đăng nhập\r\n    </h2>\r\n    <div class=\"cont_p\">\r\n        <div class=\"block_table\">\r\n            <iframe name=\"if_idp\" id=\"if_idp\" src=\"\" frameborder=\"0\" height=\"125\" width=\"382\"\r\n                scrolling=\"No\" marginwidth=\"0\" marginheight=\"0\" vspace=\"0\" hspace=\"0\" allowtransparency=\"true\"\r\n                style=\"height: 125px;\"></iframe>\r\n        </div>\r\n        <div class=\"cls\">\r\n        </div>\r\n    </div>\r\n</div>\r\n<!--End footer-->\r\n\r\n<script type=\"text/javascript\">\r\n    var _gaq = _gaq || [];\r\n    _gaq.push(['_setAccount', 'UA-16990178-1']);\r\n    _gaq.push(['_trackPageview']);\r\n\r\n    (function () {\r\n        var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;\r\n        ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';\r\n        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);\r\n    })();\r\n</script>\r\n\r\n<script>\r\n    (function (i, s, o, g, r, a, m) {\r\n        i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {\r\n            (i[r].q = i[r].q || []).push(arguments)\r\n        }, i[r].l = 1 * new Date(); a = s.createElement(o),\r\n        m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)\r\n    })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');\r\n\r\n    ga('create', 'UA-61517746-2', 'auto');\r\n    ga('send', 'pageview');\r\n\r\n</script>\r\n\r\n\r\n    </div>\r\n    </form>\r\n\t<!-- Load Facebook SDK for JavaScript -->\r\n      <div id=\"fb-root\"></div>\r\n      <script>\r\n        window.fbAsyncInit = function() {\r\n          FB.init({\r\n            xfbml            : true,\r\n            version          : 'v7.0'\r\n          });\r\n        };\r\n\r\n        (function(d, s, id) {\r\n        var js, fjs = d.getElementsByTagName(s)[0];\r\n        if (d.getElementById(id)) return;\r\n        js = d.createElement(s); js.id = id;\r\n        js.src = 'https://connect.facebook.net/vi_VN/sdk/xfbml.customerchat.js';\r\n        fjs.parentNode.insertBefore(js, fjs);\r\n      }(document, 'script', 'facebook-jssdk'));</script>\r\n\r\n      <!-- Your Chat Plugin code -->\r\n      <div class=\"fb-customerchat\"\r\n        attribution=setup_tool\r\n        page_id=\"468763316489693\"\r\n  logged_in_greeting=\"Xin chào, hãy để lại lời nhắn GATE sẽ phản hồi bạn ngay khi online nhé.\"\r\n  logged_out_greeting=\"Xin chào, hãy để lại lời nhắn GATE sẽ phản hồi bạn ngay khi online nhé.\">\r\n      </div>\r\n</body>\r\n</html>\r\n";
            var doc = new HtmlDocument();
            doc.LoadHtml(result);

            List<List<string>> table = doc.DocumentNode.SelectSingleNode("//table[@class='tinygridview']")
                .Descendants("tr")
                .Skip(1)
                .Where(tr => tr.Elements("td").Count() > 1)
                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                .ToList();
            foreach (var t in table)
            {
                if (t.Contains("CM01339784"))
                {
                    var timeRecharge = DateTime.ParseExact(t[1], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    var amount = t[4];
                    var diffInSeconds = (timeRecharge - timeRequest).TotalSeconds;
                    if (diffInSeconds > 0 && diffInSeconds <= 120)
                    {
                        result = "Đã nạp thành công " + amount;
                    }
                    else
                    {
                        result = "Thẻ đã được sử dụng";
                    }

                    Console.WriteLine("Time Recharge:" + timeRecharge.ToString());
                    Console.WriteLine("Amout:" + amount);
                    Console.WriteLine("DiffIn Seconds:" + diffInSeconds);
                    Console.WriteLine(result);

                }
            }

            Console.WriteLine(serializer.Serialize(table));
        }

    }

    public class CardDVO
    {
        public string Name { get; set; }
        public string Serial { get; set; }
        public string Pin { get; set; }
    }
    class Client : WebClient
    {
        public static string username = "lum-customer-hl_37347aa4-zone-datacenter";
        public static string password = "wud8xp4slx75";
        public static int port = 22225;
        public static string user_agent = "Mozilla/5.0 (Windows NT 5.1; rv:31.0) Gecko/20100101 Firefox/31.0";
        public string session_id = new Random().Next().ToString();

        public Client(string country = null)
        {
            this.Proxy = new WebProxy("zproxy.lum-superproxy.io", port);
            var login = username + (country != null ? "-country-" + country : "") + "-session-" + session_id;
            this.Proxy.Credentials = new NetworkCredential(login, password);
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            request.UserAgent = user_agent;
            request.ConnectionGroupName = session_id;
            return request;
        }
    }
}