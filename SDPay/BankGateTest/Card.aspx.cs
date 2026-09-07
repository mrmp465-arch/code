using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Khoai;
using Libs.Utils;
using static BankGateTest.Card;

namespace BankGateTest
{
    public partial class Card : System.Web.UI.Page
    {

        //string urlService = "https://apicard.coroach.xyz/VPGJsonService.ashx";
        string urlService = "https://apicard.cd99pay.info/VPGJsonService.ashx";
        string partnerKey = "5bef58fd34c61d066c06934a2f031a40";
        string partnerCode = "paytest";
        string serviceCode = "cardtelco";
        string commandCode = "usecard";
        protected void Page_Load(object sender, EventArgs e)
        {

            //var sign = "eLjHhYKSYUH3CAzb2HI+33UL3KlmsYwPQr8WTdljkacBolG4cm3+AB37Rq9b2uZEEwKAP9vYgV3Q/30KTB9wSVV34hkj8X4vmkrSl6ZbrEv97pC7Len6xPNl4Trg1JNcJosfn0Br3CwC+R/x5T2SSf6SbcmmADQi/qfdfZ9LnDMzTOG8bNV+IW/zLwjtfit92VJHrJYspa5GkW8jPJ8eBbYOeOp8SyJuyMDwtBn7KAAjT19h96NgfI5E4tgfKrWg7R3uq1JpjW5ORQC/ku1xSqk9P5yZYvuIIY8NKvdSrtsnwYBU90B8twNobu9lcDJ7yJg2ZanYvpxkmu9pA3Jfpw==";

            // var data = "gpay_trans_id=OS2025092626677386&bank_trace_id=&bank_transaction_id=202509260009859016&account_number=963699286600000003&amount=10000&message=LP123456789&action=CHANGE_BALANCE";

            // bool verified = Verify(data, sign);

            //NLogLogger.Info(verified ? "Chữ ký hợp lệ" : "Chữ ký KHÔNG hợp lệ");
           // buildSign();
        }
        public void buildSign()
        {
            string data = "merchant_code=MANHTHIEN&account_name=HDV LTD&map_id=015091000027&map_type=CMND&account_type=M&bank_code=MSB";

            // Private key ở dạng XML
            var privateKeyXml = "<RSAKeyValue><Modulus>npJjMkkNBJGggKKJG5vp6xWSm9XGzKIDFX53BpDcRrWH2x6TkxNVup/dm5G8ZtjYFdT7HoIa/b7Yw7KiLXrPG3/L/x3OtVqtiMpaTjxzzKv7YEDGGO5Ka4spdwd58tqBabtAtc/hvAv6Nq14UUgEKLrd8ikCrzQCFkAPtsURFoigiNhQTpYqdP+Us138CBJSpyAoxwqjy/bPFf7ZVuzPovQUh4uSjJ9a+UqxAFGw1plUgu+PNa1ozufIAKm/xZRk0Huwg/9VXdpsqw6O06anPgdDvMXTEDzgpcgmFrP2U8dDpNqTkifyesMQ6Pt7PaDFcmzz90JO1mP6B60ZsRs4vQ==</Modulus><Exponent>AQAB</Exponent><P>/KY398IURFVcYtzoWqlwLi2l7HOjfT5KOKLsFNdvQ43+5ZrgQTVzucf8FU4INB9HximyBrXfPCotYtFIq2KtCIQC9fLYEcdXmRB1n9lxLoCJ6cSg4Pljrai8g5a+xUlL2dOzTPcfmLZcnqRcGeSnu/d1P9imW+bQgIsAcurPQGM=</P><Q>oKzDF4d/NuUBz2ICPAoMTyb1DQ4XIInY3fy5pq1l0oVgC2ZWa9mIRV7+NPrhIiDJ+1BHYWhE+DKu4d9ptGAozSbqbhxO6UBGDaWZG06+aATOOcJ5D7tWnPFZog9BqruziosuGybtuoAs5Vs6JiFMH0F7Hv1WGD8sJbxKbBQSnF8=</Q><DP>bF2nN9/I7Z5naAg0qV5vvX7a4lafU0L6dtx6wl29XY50PGt3sMCKiK8cks7Lef/Iu1h7AaHUxGua1/3IW6qRaUumMXQd9VI5Ym/K4+tSPo3nmZs7HgQaGwx6/z4TSW+s1xuUlniQ1uGxTGJXFswanecAJYh5ooWX/OqiIKhSiN0=</DP><DQ>HGPqZPHwLIbydu5ebrVnz2SW0CO5OmqhAzhwpV7mKvieK+V9R7k8NuW+DSZ3OUyJ4/ofYrrF1QU/mXGcf96t1vIVYjdmShitSCQGlaioRED1H4eZVIpJl5mduODxPXXFF6nYD7wlLVsQk8Y21B32EV/EnYkt+ULQTiqjC83QLAk=</DQ><InverseQ>GApoWQqicWSbQv1pka1wVINdsRIsWk8zPH8mAT2IU2a1cfiKbelKfB9n7I3ejAoqHkTz9P/Mntb4DcZDlu7lrK/E0LvE+u2shnGLHm5OmknYf4x9Ey33DkGNoPNmIr3p7Qrl2Azq2b388ncLxFHn3iJtTzI9Q9/TR3Q3OwdBsNM=</InverseQ><D>FIP9/4CCBahNqrbcpEzMu9SDrlO/L7R6T6qX/ap6RU+xLwFKuImU2ttaEYmOZYQCmPiAMNdGZLPvWYpf/yBXcUhcfTo/BymfMAZ2Ho8lsEJD+JUIzCrwR++uQq8d8PSiL6rBWYR2wRyPl8ljuqhlmVtr4au6pC5HYteQ8OWYK2D2Yl6ozdwmDaDZmxVe882a1t1SW1ZaIN6gi/dFq2vUXyY9fWC4fEPJWnF6iJogtj7+H/ew4ROoTGT+7TalHlXHMWWysJ9ZOLUOK395xEJYUHyf3zHIzGwdu4+hDPO9Ga40TdpJ6/CR0ILdn0iwm6hseVmL9c0S2SXVKRvMWkifhQ==</D></RSAKeyValue>";

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKeyXml);

                // Ký với SHA256
                byte[] signatureBytes = rsa.SignData(dataBytes, CryptoConfig.MapNameToOID("SHA256"));

                string signatureBase64 = Convert.ToBase64String(signatureBytes);
                NLogLogger.Info("signatureBase64: " + signatureBase64);
                //Console.WriteLine("Data: " + data);
                //Console.WriteLine("Signature: " + signatureBase64);
            }
        }
        public static bool Verify(string data, string signatureBase64)
        {
            var publicKeyXml = "<RSAKeyValue><Modulus>0cNJLuLV75oN3PEHTsxMUEm0NLJq77oDwliN8rNlubvFn01njMd5e669GzTNbNYMropGjksefC6AnLwT94DVHj/xB5LxU6Dp5sBXRTPqWXWXcnI5hvdqx5VZxZCKkqUBrp7JOV40wcRkicef9F5HpzHli/wM54yXhnQkKdyZpzgmr9lKQRIEnUCq+rMg1wNom0hIAHdUO1QsXywG/7oRyyfE0YJiETHrJjW7zWO7aAa97O7BwV6wGHbOKYVBMG7gCuWkbiJE42kC2xmQS3Il1v47+UC4Dg2ly9ow++k9uVLRoPKJx6Dt1U9+F1i11HNyJIKBkbxI1TJZzY93HocZfw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signature = Convert.FromBase64String(signatureBase64);

            using (var rsa = new RSACryptoServiceProvider())
            {
                // Load public key
                rsa.FromXmlString(publicKeyXml);

                // Hash lại dữ liệu theo SHA256
                byte[] hash;
                using (var sha256 = SHA256.Create())
                {
                    hash = sha256.ComputeHash(dataBytes);
                }

                // Verify SHA256withRSA
                return rsa.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA256"), signature);
            }
        }
        //public void Verify(string  signatureBase64)
        //{
        //    var publicKeyXml = "<RSAKeyValue><Modulus>npJjMkkNBJGggKKJG5vp6xWSm9XGzKIDFX53BpDcRrWH2x6TkxNVup/dm5G8ZtjYFdT7HoIa/b7Yw7KiLXrPG3/L/x3OtVqtiMpaTjxzzKv7YEDGGO5Ka4spdwd58tqBabtAtc/hvAv6Nq14UUgEKLrd8ikCrzQCFkAPtsURFoigiNhQTpYqdP+Us138CBJSpyAoxwqjy/bPFf7ZVuzPovQUh4uSjJ9a+UqxAFGw1plUgu+PNa1ozufIAKm/xZRk0Huwg/9VXdpsqw6O06anPgdDvMXTEDzgpcgmFrP2U8dDpNqTkifyesMQ6Pt7PaDFcmzz90JO1mP6B60ZsRs4vQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        //    var data = "gpay_trans_id=OS2025092626677386&bank_trace_id=&bank_transaction_id=202509260009859016&account_number=963699286600000003&amount=10000&message=LP123456789&action=CHANGE_BALANCE";

        //    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        //    byte[] signatureBytes = Convert.FromBase64String(signatureBase64);

        //    using (var rsa = new RSACryptoServiceProvider())
        //    {
        //        rsa.FromXmlString(publicKeyXml);

        //        // Hash trước bằng SHA256
        //        byte[] hash;
        //        using (var sha = SHA256.Create())
        //            hash = sha.ComputeHash(dataBytes);

        //        // Verify chữ ký
        //        bool verified = rsa.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA256"), signatureBytes);

        //        NLogLogger.Info(verified ? "Chữ ký hợp lệ" : "Chữ ký KHÔNG hợp lệ");
        //    }
        //}
        public class QRField
        {
            public string ID;
            public int Length;
            public string Value;
        }

        public static Dictionary<string, QRField> ParseEMVCo(string qrData)
        {
            var fields = new Dictionary<string, QRField>();
            int index = 0;

            while (index + 4 <= qrData.Length)
            {
                string id = qrData.Substring(index, 2);
                int len = int.Parse(qrData.Substring(index + 2, 2));

                int start = index + 4;
                int remaining = qrData.Length - start;
                int safeLen = Math.Min(len, remaining);

                string value = qrData.Substring(start, safeLen);
                fields[id] = new QRField { ID = id, Length = len, Value = value };

                index += 4 + safeLen;
            }

            return fields;
        }

        public static void ParseMoMoQR(string qrData)
        {
            var tags = new Dictionary<string, QRField>();
            int index = 0;
            string tag62Raw = "";

            // 🔍 Phân tích EMVCo bình thường cho đến khi gặp tag62
            while (index + 4 <= qrData.Length)
            {
                string id = qrData.Substring(index, 2);
                int len = int.Parse(qrData.Substring(index + 2, 2));

                int start = index + 4;
                int remaining = qrData.Length - start;
                int safeLen = Math.Min(len, remaining);

                string value = qrData.Substring(start, safeLen);
                tags[id] = new QRField { ID = id, Length = len, Value = value };

                index += 4 + safeLen;

                if (id == "62")
                {
                    tag62Raw = qrData.Substring(start); // lấy toàn bộ phần sau của tag62
                    break;
                }
            }

            // 🔢 Số tài khoản proxy: tag38
            string proxyId = "";
            if (tags.TryGetValue("38", out var tag38))
            {
                var subFields = ParseEMVCo(tag38.Value);
                foreach (var sub in subFields.Values)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(sub.Value, @"(99)?ZP\d+O\d+");
                    if (match.Success)
                    {
                        proxyId = match.Value;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(proxyId) && !proxyId.StartsWith("99"))
                {
                    proxyId = "99" + proxyId;
                }
            }

            // 💰 Số tiền: tag54
            string amount = tags.ContainsKey("54") ? tags["54"].Value : "";

            // 🏦 Mã ngân hàng: tìm 970xxx trong tag38
            string bankCode = "";
            if (tag38 != null)
            {
                var binMatch = System.Text.RegularExpressions.Regex.Match(tag38.Value, @"970\d{3}");
                if (binMatch.Success)
                {
                    bankCode = binMatch.Value;
                }
            }

            // 📝 Nội dung: tìm thủ công trong tag62Raw
            string description = "";
            int idx08 = tag62Raw.IndexOf("08");
            if (idx08 >= 0 && idx08 + 4 <= tag62Raw.Length)
            {
                description = tag62Raw.Substring(idx08 + 4); // bỏ qua ID + Length → lấy hết
            }

            // ✅ Log kết quả
            NLogLogger.Info("🔢 Số tài khoản: " + proxyId);
            NLogLogger.Info("🏦 Ngân hàng: " + bankCode);
            NLogLogger.Info("💰 Số tiền: " + amount + " VND");
            NLogLogger.Info("📝 Nội dung: " + description);
        }




        public async Task SendTelegramMessage(string id, string message)
        {
            using (var client = new HttpClient())
            {
                string url = string.Format("https://api.telegram.org/bot8881845685:AAHUx2_QaWyiFaNoC0N5w-dles0_qeBuMlU/sendMessage?chat_id={1}&text={0}&parse_mode=html", message, id);

                int maxRetries = 3;
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            NLogLogger.Info("Message sent successfully.");
                            break;
                        }
                        else
                        {
                            NLogLogger.Info($"Failed attempt {attempt}: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogLogger.Info($"Exception attempt {attempt}: {ex.Message}");
                        if (attempt == maxRetries)
                            throw;
                    }

                    await Task.Delay(1000); // Delay 1s trước khi thử lại
                }
            }
        }
        protected void CheckOut_Click(object sender, EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                CardSerial = txtSerial.Text,
                CardCode = txtPin.Text,
                CardType = txtTelco.SelectedValue,
                AccountName = txtAccountName.Text,
                AppCode = "Web Demo",
                RefCode = DateTime.Now.ToString("yyyyMMddHHmmss"),
                CallBackUrl= "https://bankgate.coroach.xyz/VPGJsonService.ashx",
                AmountUser = Convert.ToInt32(txtAmout.SelectedValue)
            });
            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            var requestData = new RequestData()
            {
                PartnerCode = partnerCode,
                CommandCode = commandCode,
                RequestContent = requestContent,
                ServiceCode = serviceCode,
                Signature = signature
            };
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
            NLogLogger.Info(new string[] { "Card Test", "Response Core", serviceResponse });

            var resPonse = serializer.Deserialize<APIResponse>(serviceResponse);

            if (resPonse != null)
            {
                pnInfo.Visible = false;
                pnError.Visible = true;
                lbError.Text = resPonse.Description + " " + resPonse.ResponseContent;
            }

        }

        private string PostJson(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
                                    //request.Accept = "JSON";
            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(postDatabytes, 0, postDatabytes.Length);
            }
            var webResponse = request.GetResponse();
            if (webResponse == null)
            {
                return "Unable to connect to the remote server";
            }
            var sr = new StreamReader(webResponse.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        public class UseCardRequest
        {
            public string CardSerial { get; set; }
            public string CardCode { get; set; }
            public string CardType { get; set; }
            public string AccountName { get; set; }
            public string AppCode { get; set; }

            public string CallBackUrl { get; set; }

            
            public string RefCode { get; set; }
            public int Amount { get; set; }
            public int AmountUser { get; set; }
        }

        public class RequestData
        {
            public string PartnerCode { get; set; }
            public string ServiceCode { get; set; }
            public string CommandCode { get; set; }
            public string RequestContent { get; set; }
            public string Signature { get; set; }

        }

        public class APIResponse
        {
            public int ResponseCode { get; set; }
            public string Description { get; set; }
            public string ResponseContent { get; set; }
            public string Signature { get; set; }
        }
        static Dictionary<string, string> ParseEMVQR(string input)
        {
            var result = new Dictionary<string, string>();
            int i = 0;
            while (i + 4 <= input.Length)
            {
                string tag = input.Substring(i, 2);
                int length = int.Parse(input.Substring(i + 2, 2));
                i += 4;

                if (i + length <= input.Length)
                {
                    string value = input.Substring(i, length);
                    result[tag] = value;
                    i += length;
                }
                else
                {
                    break;
                }
            }
            return result;
        }
        public void getBankinfo(string qrData)
        {
            var root = ParseEMVQR(qrData);

            if (root.TryGetValue("38", out string tag38Value))
            {
                var tag38 = ParseEMVQR(tag38Value);

                if (tag38.TryGetValue("01", out string vietQRpayload))
                {
                    // Cần tiếp tục parse TLV trong payload nếu còn, nhưng ở đây đang ở dạng: 000697...
                    var payloadFields = ParseEMVQR(vietQRpayload);

                    if (payloadFields.TryGetValue("00", out string bankCode) &&
                        payloadFields.TryGetValue("01", out string accountNumber))
                    {
                        NLogLogger.Info($"Mã ngân hàng: {bankCode}");          // ✅ 970415
                        NLogLogger.Info($"Số tài khoản: {accountNumber}");      // ✅ 0110097856
                        NLogLogger.Info(new string[] { "Card Test", "Mã ngân hàng", bankCode.ToString(), accountNumber });
                    }
                    else
                    {
                        NLogLogger.Info("Không tách được mã ngân hàng và tài khoản.");
                    }
                }
            }



        }


    }
}