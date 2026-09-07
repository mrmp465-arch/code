using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        string urlService = "https://apicard.coroach.xyz/VPGJsonService.ashx";
        //string urlService = "http://45.32.115.186:1581//VPGJsonService.ashx";
        string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
        string partnerCode = "pp";
        string serviceCode = "cardtelco";
        string commandCode = "usecard";
        protected void Page_Load(object sender, EventArgs e)
        {
            //string qrData = "00020101021226530010vn.zalopay01064503190203001031813906273727886981838620010A00000072701320006970454011899ZP25195O143493990208QRIBFTTA52047399530370454061620005802VN6304D15A";
            //ParseMoMoQR(qrData);
        }

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
                string url = string.Format("https://api.telegram.org/bot7593090326:AAFAMnx9Bw6akr6mw9YLrtohHBzybgoqhpo/sendMessage?chat_id={1}&text={0}&parse_mode=html", message, id);

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