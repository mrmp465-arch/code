using Libs.API;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using static BankGateV2.bankin.Info;
using static BankGateV2.ServiceHandler.GetQRInfo;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetQRInfo
    /// </summary>
    public class GetQRInfo2 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            var jsonString = String.Empty;
            var result = string.Empty;
            context.Request.InputStream.Position = 0;

            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            var request = javaScriptSerializer.Deserialize<RequestGetQR>(jsonString);
            //NLogLogger.Info(new string[] { "VPGJsonService", "Request", jsonString });
            int type = 2;
            if (request.qrtext.Contains("zalopay"))
            {
                type = 4;
            }
            if (request.qrtext.Contains("momo"))
            {
                type = 3;
            }


            if (type == 1)
            {
                var bankinfo = getBankinfo(request.qrtext);
                context.Response.Write(serializer.Serialize(bankinfo));
            }
            if (type == 2)
            {
                var bankinfo = getBankinfo2(request.qrtext);
                context.Response.Write(serializer.Serialize(bankinfo));
            }
            if (type == 3)
            {
                var bankinfo = getBankinfo3(request.qrtext);
                context.Response.Write(serializer.Serialize(bankinfo));
            }
            //zalo
            if (type == 4)
            {
                var bankinfo = getBankinfo4(request.qrtext);
                context.Response.Write(serializer.Serialize(bankinfo));
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public class RequestGetQR
        {
            public string qrtext { get; set; }
            // public int type { get; set; }

            //public string Signature { get; set; }

        }
        public class BankInfo
        {
            public string bankAccountNumber { get; set; }
            public string bankCode { get; set; }
            public string bankName { get; set; }
            public int amount { get; set; }
            public string content { get; set; }
            public string countryCode { get; set; }
            public int responsecode { get; set; }
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
        public BankInfo getBankinfo(string qrData)
        {
            var result = new BankInfo();
            result.responsecode = -1;
            try
            {


                var root = ParseEMVQR(qrData);

                string tag38Value;
                if (root.TryGetValue("38", out tag38Value))
                {
                    var tag38 = ParseEMVQR(tag38Value);
                    string vietQRpayload;
                    if (tag38.TryGetValue("01", out vietQRpayload))
                    {
                        // Cần tiếp tục parse TLV trong payload nếu còn, nhưng ở đây đang ở dạng: 000697...
                        var payloadFields = ParseEMVQR(vietQRpayload);

                        string bankCode;
                        string accountNumber;

                        if (payloadFields.TryGetValue("00", out bankCode) &&
                            payloadFields.TryGetValue("01", out accountNumber))
                        {
                            result.bankAccountNumber = accountNumber;
                            result.countryCode = "VN";
                            var lstbankcode = new BankCodeTranfer().GetListCache();
                            result.bankCode = lstbankcode.FirstOrDefault(x => x.bin == bankCode).code;
                            result.bankName = lstbankcode.FirstOrDefault(x => x.bin == bankCode).shortName;
                            result.amount = 0;
                            result.content = "";
                            result.responsecode = 1;
                        }

                    }
                }
            }
            catch { }

            return result;

        }
        public BankInfo getBankinfo2(string qrData)
        {
            var result = new BankInfo();
            result.responsecode = -1;
            try
            {

                var parsed = ParseQR2(qrData);

                string value;
                parsed.TryGetValue("38.01", out value);
                NLogLogger.Info("bankvalue: " + value);

                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Substring(4); // Bỏ '0006' → còn '9704220113VQRQABUMX4058'
                }

                // Tách mã ngân hàng và số tài khoản
                string bankCode = value.Substring(0, 6);             // ✅ 970422
                string accountNumber = value.Substring(6);           // ✅ 0113VQRQABUMX4058

                // Nếu cần bỏ mã chi nhánh đầu (4 ký tự), có thể cắt:
                if (accountNumber.Length > 4)
                {
                    accountNumber = accountNumber.Substring(4);       // ✅ VQRQABUMX4058
                }
                try
                {
                    parsed.TryGetValue("54", out value);
                    string amount = value;

                    parsed.TryGetValue("62.08", out value);
                    string content = value;
                    result.amount = int.Parse(amount);
                    result.content = content;
                    if (string.IsNullOrEmpty(content))
                    {
                        result.content = "";
                    }
                }

                catch { }

                result.bankAccountNumber = accountNumber;
                result.countryCode = "VN";
                var lstbankcode = new BankCodeTranfer().GetListCache();
                result.bankCode = lstbankcode.FirstOrDefault(x => x.bin == bankCode).code;
                result.bankName = lstbankcode.FirstOrDefault(x => x.bin == bankCode).shortName;

                result.responsecode = 1;
            }
            catch { }

            return result;

        }
        public class QRField
        {
            public string ID;
            public int Length;
            public string Value;
        }

        public static Dictionary<string, QRField> ParseEMVCoMM(string qrData)
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
        public BankInfo getBankinfo3(string qrData)
        {
            var result = new BankInfo();
            result.responsecode = -1;
            try
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
                QRField tag38;
                if (tags.TryGetValue("38", out tag38))
                {
                    var subFields = ParseEMVCoMM(tag38.Value);
                    foreach (var sub in subFields.Values)
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(sub.Value, @"(99)?MM\d+O\d+");
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

                result.bankAccountNumber = proxyId;
                result.countryCode = "VN";
                var lstbankcode = new BankCodeTranfer().GetListCache();
                result.bankCode = lstbankcode.FirstOrDefault(x => x.bin == bankCode).code;
                result.bankName = lstbankcode.FirstOrDefault(x => x.bin == bankCode).shortName;
                try
                {
                    result.amount = int.Parse(amount);
                }
                catch
                {
                    result.amount = 0;
                }
                result.content = description;
                result.responsecode = 1;
            }
            catch { }

            return result;

        }
        public BankInfo getBankinfo4(string qrData)
        {
            var result = new BankInfo();
            result.responsecode = -1;
            try
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
                QRField tag38;
                if (tags.TryGetValue("38", out tag38))
                {
                    var subFields = ParseEMVCoMM(tag38.Value);
                    foreach (var sub in subFields.Values)
                    {
                        //NLogLogger.Info("sub.Value:  " + sub.Value);
                        var match = Regex.Match(sub.Value, @"(99)?ZP\d+[A-Z]\d+");
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
                //string description = "";
                //int idx08 = tag62Raw.IndexOf("08");
                //if (idx08 >= 0 && idx08 + 4 <= tag62Raw.Length)
                //{
                //    description = tag62Raw.Substring(idx08 + 4); // bỏ qua ID + Length → lấy hết
                //}

                result.bankAccountNumber = proxyId;
                result.countryCode = "VN";
                var lstbankcode = new BankCodeTranfer().GetListCache();
                result.bankCode = lstbankcode.FirstOrDefault(x => x.bin == bankCode).code;
                result.bankName = lstbankcode.FirstOrDefault(x => x.bin == bankCode).shortName;
                try
                {
                    result.amount = int.Parse(amount);
                }
                catch
                {
                    result.amount = 0;
                }

                result.content = "";
                result.responsecode = 1;
            }
            catch
            {
            }

            return result;

        }
        public Dictionary<string, string> ParseQR2(string qr)
        {
            var result = new Dictionary<string, string>();
            int i = 0;

            while (i < qr.Length)
            {
                string tag = qr.Substring(i, 2);
                int length = int.Parse(qr.Substring(i + 2, 2));
                string value = qr.Substring(i + 4, length);
                result[tag] = value;
                i += 4 + length;

                // Phân tích thêm cấp lồng (sub-TLV) nếu cần
                if (tag == "38")
                {
                    int j = 0;
                    while (j < value.Length)
                    {
                        string subTag = value.Substring(j, 2);
                        int subLen = int.Parse(value.Substring(j + 2, 2));
                        string subValue = value.Substring(j + 4, subLen);

                        result[$"38.{subTag}"] = subValue;
                        j += 4 + subLen;
                    }
                }

                if (tag == "62")
                {
                    int j = 0;
                    while (j < value.Length - 4)
                    {
                        string subTag = value.Substring(j, 2);
                        int subLen = int.Parse(value.Substring(j + 2, 2));
                        string subValue = value.Substring(j + 4, subLen);

                        result[$"62.{subTag}"] = subValue;
                        j += 4 + subLen;
                    }
                }
            }

            return result;
        }
    }

}

