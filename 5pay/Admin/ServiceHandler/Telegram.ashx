<%@ WebHandler Language="C#" Class="Telegram" %>

using System;
using System.Web;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using Libs.Report;
using System.Globalization;
using Libs.API;
using System.Linq;
using Libs.Utils;
using System.Collections.Specialized;
public class Telegram : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Request.ContentType = "application/json";
        context.Response.ContentType = "application/json";
        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        var jsonString = String.Empty;
        var result = string.Empty;
        //context.Request.InputStream.Position = 0;

        using (var inputStream = new StreamReader(context.Request.InputStream))
        {
            try
            {
                jsonString = inputStream.ReadToEnd();
                //NLogLogger.DebugMessage(jsonString);
                //return;

                if (jsonString.Contains("callback_query"))
                {
                    dynamic resObj = javaScriptSerializer.Deserialize<dynamic>(jsonString);
                    // NLogLogger.Info(jsonString);
                    // Callback từ Inline Button
                    if (resObj.ContainsKey("callback_query"))
                    {
                        HandleCallback(resObj["callback_query"]);
                    }
                }
                else
                {

                    var resObj = javaScriptSerializer.Deserialize<Callback>(jsonString);
                    string message_id = resObj.message.message_id.ToString();

                    if (resObj.message.text.StartsWith("/getgroup"))
                    {
                        SendTeleV2(resObj.message.chat.id.ToString(), resObj.message.chat.id.ToString());
                        return;
                    }
                    if (resObj.message.text.StartsWith("/upgr"))
                    {
                        var partnercode = resObj.message.text.Replace("/upgr", "").Trim();
                        var _Partner = new Partners().Get(partnercode);
                        _Partner.SMSPlusUrl = resObj.message.chat.id.ToString();
                        _Partner.Update();
                        // SendTeleV2(resObj.message.chat.id.ToString(), resObj.message.chat.id.ToString());
                        return;
                    }
                    var Partnecode = GetPartnerCode(resObj.message.chat.id.ToString());
                    //var Partnecode = GetPartnerCode(resObj.message.chat.id.ToString());
                    if (string.IsNullOrEmpty(Partnecode))
                        return;

                    if (resObj.message.text.StartsWith("/balance") || resObj.message.text.StartsWith("/bal"))
                    {
                        var user = new Users().GetByUserName(Partnecode);
                        if (user != null)
                            SendTeleV4(resObj.message.chat.id.ToString(), "Balance " + Partnecode + " :  " + user.Balance.ToString("N0").Replace(".", ","), message_id);
                    }
                    if (resObj.message.text.StartsWith("/in"))
                    {
                        var refcode = resObj.message.text.Replace("/in", "").Trim();
                        //refcode = resObj.message.text.Replace("/bout", "").Trim();
                        //refcode = resObj.message.text.Replace("/df", "").Trim();
                        var checkorder = DataCaching.GetCache<CheckOrder>("CheckOrder:" + Partnecode + refcode);
                        var trans = new BankGateAPI().GetByRefcode(refcode, Partnecode);
                        if (checkorder != null)
                        {

                            SendTeleV4(resObj.message.chat.id.ToString(), "Giao dịch thành công | Transaction Successful | 交易成功", message_id);
                            System.Threading.Thread.Sleep(500);
                            SendTeleV2(resObj.message.chat.id.ToString(), String.Format("{0} | {1} | {2} | {3} | {4} ", trans.TransactionID, trans.RefCode,
                                trans.Amount.ToString("N0").Replace(".", ","), trans.FullName, trans.LastTime.ToString("dd/MM/yy HH:mm:ss")));
                            return;
                        }




                        if (trans == null)
                        {
                            //return new APIResponse((int)ResponseCode.TransactionNotExists);
                            SendTeleV4(resObj.message.chat.id.ToString(), "Không tìm thấy đơn | Transaction Not Found | 未找到该笔交易", message_id);
                            return;
                        }
                        if (trans.Status >= 1)
                        {
                            SendTeleV4(resObj.message.chat.id.ToString(), "Giao dịch thành công | Transaction Successful | 交易成功", message_id);
                            System.Threading.Thread.Sleep(500);
                            SendTeleV2(resObj.message.chat.id.ToString(), String.Format("{0} | {1} | {2} | {3} | {4} ", trans.TransactionID, trans.RefCode,
                               trans.Amount.ToString("N0").Replace(".", ","), trans.FullName, trans.LastTime.ToString("dd/MM/yy HH:mm:ss")));
                            return;
                        }
                        else
                        {

                            SendTeleV4(resObj.message.chat.id.ToString(), "Giao dịch đang chờ xử lý | Transaction Pending | 交易处理中", message_id);
                            return;
                        }

                    }
					
                    if (resObj.message.text.StartsWith("/bill"))
                    {
                        var refcode = resObj.message.text.Replace("/bill", "").Trim();
                        //refcode = resObj.message.text.Replace("/bout", "").Trim();
                        //refcode = resObj.message.text.Replace("/df", "").Trim();
                        //var checkorder = DataCaching.GetCache<CheckOrder>("CheckOutOrder:" + Partnecode + refcode);
                        //if (checkorder != null)
                        //{
                        //    //NLogLogger.Info("check order by cache");
                        //    //return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        //    //{
                        //    //    ResponseContent = javaScriptSerializer.Serialize(checkorder)
                        //    //};
                        //    var url = String.Format("http://139.180.147.57:1587//DetailScreen.aspx?orderNo={0}&refcode={1}&chatid={2}", checkorder.TransactionID, checkorder.RefCode, resObj.message.chat.id.ToString());
                        //    SendBill(url);
                        //    //SendTeleV2(resObj.message.chat.id.ToString(), "Transaction success");

                        //    return;
                        //}


                        var trans = new BankCashAPI().GetByRefcode(refcode, Partnecode);

                        if (trans == null)
                        {
                            //return new APIResponse((int)ResponseCode.TransactionNotExists);
                            SendTeleV4(resObj.message.chat.id.ToString(), "Transaction not found", message_id);
                            return;
                        }
                        if (trans.Status >= 1)
                        {
                            if (!string.IsNullOrEmpty(trans.Mobile))
                            {

                                var url = String.Format("http://139.180.147.57:1587/DetailScreen.aspx?orderNo={0}&refcode={1}&chatid={2}&message_id={3}", trans.TransactionID, trans.RefCode, resObj.message.chat.id.ToString(), message_id);
                                SendBill(url);
                            }
                            //SendTeleV2(resObj.message.chat.id.ToString(), "Transaction success");
                            return;
                        }
                        else
                        {
                            if (trans.Status == -1)
                            {
                                if ((trans.LogContent.Contains("không trùng ") || trans.LogContent.Contains("tìm thấy")))
                                {
                                    if (trans.LogContent.Contains("TK không"))
                                    {
                                        SendTeleV4(resObj.message.chat.id.ToString(), "Ngân hàng nhận tiền lỗi | Receiving Bank Error | 收款银行异常 ", message_id);
                                        return;
                                    }
                                    else
                                    {
                                        SendTeleV4(resObj.message.chat.id.ToString(), "Tài khoản nhận tiền không trùng khớp | Recipient Account Mismatch | 收款账户信息不匹配", message_id);
                                        return;
                                    }
                                }
                                SendTeleV4(resObj.message.chat.id.ToString(), "Lệnh out thất bại do ngân hàng lỗi | Payout Failed Due to Bank Error | 因银行异常，出款失败", message_id);
                                return;
                            }
                            if (trans.Status == -357)
                            {

                                if (trans.LogContent.Contains("TK không"))
                                {
                                    SendTeleV4(resObj.message.chat.id.ToString(), "Ngân hàng nhận tiền lỗi | Receiving Bank Error | 收款银行异常 ", message_id);
                                    return;
                                }
                                else
                                {
                                    SendTeleV4(resObj.message.chat.id.ToString(), "Tài khoản nhận tiền không trùng khớp | Recipient Account Mismatch | 收款账户信息不匹配", message_id);
                                    return;
                                }
                                // return;
                            }
                            SendTeleV4(resObj.message.chat.id.ToString(), "Lệnh đang được xử lý | Transaction Is Being Processed | 订单处理中", message_id);
                            return;
                        }

                    }
                    if (resObj.message.text.StartsWith("/out"))
                    {
                        var refcode = resObj.message.text.Replace("/out", "").Trim();
                        //refcode = resObj.message.text.Replace("/bout", "").Trim();
                        //refcode = resObj.message.text.Replace("/df", "").Trim();
                        var trans = new BankCashAPI().GetByRefcode(refcode, Partnecode);
                        var checkorder = DataCaching.GetCache<CheckOrder>("CheckOutOrder:" + Partnecode + refcode);
                        if (checkorder != null)
                        {
                            //NLogLogger.Info("check order by cache");
                            //return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            //{
                            //    ResponseContent = javaScriptSerializer.Serialize(checkorder)
                            //};
                            SendTeleV4(resObj.message.chat.id.ToString(), "Lệnh out thành công | Payout Successful | 出款成功 ", message_id);
                            System.Threading.Thread.Sleep(500);
                            SendTeleV2(resObj.message.chat.id.ToString(), String.Format("{0} | {1} | {2} |{4} | {5} | {3} ", trans.TransactionID, trans.RefCode,
                            trans.Amount.ToString("N0").Replace(".", ","), trans.LastTime.ToString("dd/MM/yy HH:mm:ss"),
                            trans.BankCode + " - " + trans.BankAccountNumber + " - " + trans.BankAccountName, trans.Mobile));

                            return;
                        }


                        //var trans = new BankCashAPI().GetByRefcode(refcode, Partnecode);

                        if (trans == null)
                        {
                            //return new APIResponse((int)ResponseCode.TransactionNotExists);
                            SendTeleV4(resObj.message.chat.id.ToString(), "Lệnh out không tìm thấy | Payout Order Not Found | 未找到该出款订单 ", message_id);
                            return;
                        }
                        if (trans.Status >= 1)
                        {
                            SendTeleV4(resObj.message.chat.id.ToString(), "Lệnh out thành công | Payout Successful | 出款成功 ", message_id);
                            System.Threading.Thread.Sleep(500);
                            SendTeleV2(resObj.message.chat.id.ToString(), String.Format("{0} | {1} | {2} |{4} | {5} | {3}  ", trans.TransactionID, trans.RefCode,
                            trans.Amount.ToString("N0").Replace(".", ","), trans.LastTime.ToString("dd/MM/yy HH:mm:ss"),
                            trans.BankCode + " - " + trans.BankAccountNumber + " - " + trans.BankAccountName, trans.Mobile));

                            return;
                        }
                        else
                        {
                            if (trans.Status == -1)
                            {
                                if (trans.LogContent.Contains("Lỗi truy vấn"))
                                {
                                    var logmess = GetErrorMessage(trans.LogContent, "Lỗi truy vấn");
                                    SendTeleV4(resObj.message.chat.id.ToString(), "Tài khoản nhận tiền không trùng khớp hoặc không đúng | Recipient Account Invalid or Mismatched | 收款账户信息有误或不匹配 =>" + logmess, message_id);
                                    return;
                                }
                                if (trans.LogContent.Contains("Không tìm thấy"))
                                {
                                    var logmess = GetErrorMessage(trans.LogContent, "Không tìm thấy");
                                    SendTeleV4(resObj.message.chat.id.ToString(), "Tài khoản nhận tiền lỗi | Recipient Account Error | 收款账户异常 =>" + logmess, message_id);
                                    return;
                                }
                                SendTeleV4(resObj.message.chat.id.ToString(), "Lệnh out thất bại do ngân hàng lỗi | Payout Failed Due to Bank Error | 因银行异常，出款失败", message_id);
                                return;
                            }
                            if (trans.Status == -357)
                            {
                                if (trans.LogContent.Contains("Lỗi truy vấn"))
                                {
                                    var logmess = GetErrorMessage(trans.LogContent, "Lỗi truy vấn");
                                    SendTeleV4(resObj.message.chat.id.ToString(), "Tài khoản nhận tiền không trùng khớp hoặc không đúng | Recipient Account Invalid or Mismatched | 收款账户信息有误或不匹配 =>" + logmess, message_id);
                                    return;
                                }
                                if (trans.LogContent.Contains("Không tìm thấy"))
                                {
                                    var logmess = GetErrorMessage(trans.LogContent, "Không tìm thấy");
                                    SendTeleV4(resObj.message.chat.id.ToString(), "Tài khoản nhận tiền lỗi | Recipient Account Error | 收款账户异常。 =>" + logmess, message_id);
                                    return;
                                }
                                SendTeleV4(resObj.message.chat.id.ToString(), "Lệnh out thất bại do ngân hàng lỗi | Payout Failed Due to Bank Error | 因银行异常，出款失败", message_id);
                                return;

                            }
                            SendTeleV4(resObj.message.chat.id.ToString(), "Lệnh đang được xử lý | Transaction Is Being Processed | 订单处理中", message_id);
                            return;
                        }

                    }
                }
                //if (resObj.message.text.StartsWith("/cash"))
                //{
                //    var amount = resObj.message.text.Replace("/cash ", "").Replace(",", "").Replace(",", "");

                //}
            }
            catch (Exception ex)
            {
                //NLogLogger.Info(ex.Message);
            }
        }
    }
    public class CheckOrder
    {

        public int Amount { get; set; }
        public string RefCode { get; set; }

        public string TransactionID { get; set; }
        public DateTime LasTime { get; set; }

    }

    static string GetPartnerCode(string id)
    {
        string partnecode = "";
        var partnelist = new Partners().GetList();
        if (partnelist.Exists(x => x.SMSPlusUrl == id))
            return partnelist.FirstOrDefault(x => x.SMSPlusUrl == id).PartnerCode;


        return partnecode;
    }
    static string GetUserConfirm(string id)
    {
        string partnecode = "";
        var partnelist = new Partners().GetList();
        if (partnelist.Exists(x => x.SMSPlusUrl == id))
            return partnelist.FirstOrDefault(x => x.SMSPlusUrl == id).SMSPlusCheckUrl;


        return partnecode;
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    public class Chat
    {
        public long id { get; set; }
        public string title { get; set; }
        //public string type { get; set; }
        //public bool all_members_are_administrators { get; set; }
    }

    public class Entity
    {
        public int offset { get; set; }
        public int length { get; set; }
        public string type { get; set; }
    }

    public class From
    {
        public long id { get; set; }
        public bool is_bot { get; set; }

        public string username { get; set; }
    }

    public class Message
    {
        public long message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
        public ReplyToMessage reply_to_message { get; set; }
        //public List<Entity> entities { get; set; }
    }

    public class Callback
    {
        public long update_id { get; set; }
        public Message message { get; set; }

    }

    public class ReplyToMessage
    {
        public string text { get; set; }

        public string caption { get; set; }
    }
    private void HandleCallback(dynamic callback)
    {
        string callbackId = callback["id"];
        string data = callback["data"];

        long chatId = Convert.ToInt64(callback["message"]["chat"]["id"]);
        int messageId = Convert.ToInt32(callback["message"]["message_id"]);

        string oldText = callback["message"]["text"];

        string firstName = callback["from"].ContainsKey("first_name")
            ? callback["from"]["first_name"]
            : "";

        string username = callback["from"].ContainsKey("username")
            ? callback["from"]["username"]
            : "";

        if (string.IsNullOrEmpty(username))
        {
            username = firstName;
        }

        string actionText = data.StartsWith("confirm:")
            ? "✅ " + username + " 确认 confirmed"
            : "❌ " + username + " 取消 cancel";

        string newText =
            oldText +
            "\n\n____________________________\n\n" +
            actionText;

        var isConfirm = true;

        string userconfirms = GetUserConfirm(chatId.ToString());
        if (!string.IsNullOrEmpty(userconfirms))
        {
            if (!userconfirms.ToLower().Contains(username.ToLower()))
            {
                isConfirm = false;
                SendTeleV2(chatId.ToString(), "Người bấm " + username + " không có trong danh sách xác nhận");
                NLogLogger.Info(new string[] { "userconfirms", userconfirms, username });
            }
        }
        if (isConfirm)
        {
            EditMessage(chatId, messageId, newText);

            AnswerCallbackQuery(callbackId, "Recorded");
        }

        //EditMessage(chatId, messageId, newText);

        //AnswerCallbackQuery(callbackId, "Recorded");
    }

    private void EditMessage(long chatId, int messageId, string text)
    {
        string BotToken = "8010825769:AAEbExMZtB9twOsAb8uyjM6noeWfWlPm5yg";
        string url =
        "https://api.telegram.org/bot" +
        BotToken +
        "/editMessageText";

        using (var client = new WebClient())
        {
            var data = new NameValueCollection();

            data["chat_id"] = chatId.ToString();
            data["message_id"] = messageId.ToString();
            data["text"] = text;
            data["parse_mode"] = "HTML";

            client.UploadValues(url, "POST", data);
        }
    }

    private void AnswerCallbackQuery(string callbackId, string message)
    {

        string BotToken = "8010825769:AAEbExMZtB9twOsAb8uyjM6noeWfWlPm5yg";
        string url =
           "https://api.telegram.org/bot" +
           BotToken +
           "/answerCallbackQuery";

        using (var client = new WebClient())
        {
            var data = new NameValueCollection();

            data["callback_query_id"] = callbackId;
            data["text"] = message;

            client.UploadValues(url, "POST", data);
        }
    }

    public static void SendBill(string url)
    {

        try
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            NLogLogger.Info(url);
            var requestUrl = url;
            var webclient = new WebClient();

            webclient.DownloadString(requestUrl);
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
        }
    }
    public static void SendTele(string id, string message)
    {

        try
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var requestUrl = string.Format("https://api.telegram.org/bot8010825769:AAEbExMZtB9twOsAb8uyjM6noeWfWlPm5yg/sendMessage?chat_id={1}&text={0}", message, id);
            var webclient = new WebClient();

            webclient.DownloadString(requestUrl);
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
        }
    }
    public static void SendTeleV2(string id, string message)
    {
        Task.Run(() => SendTele(id, message));

        ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
    }

    public static void SendTeleV3(string id, string message, string replyToMessageId)
    {

        try
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var requestUrl = string.Format("https://api.telegram.org/bot8010825769:AAEbExMZtB9twOsAb8uyjM6noeWfWlPm5yg/sendMessage?chat_id={1}&reply_to_message_id={2}&text={0}", message, id, replyToMessageId);
            var webclient = new WebClient();

            webclient.DownloadString(requestUrl);
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
        }
    }
    public static void SendTeleV4(string id, string message, string replyToMessageId)
    {
        Task.Run(() => SendTeleV3(id, message, replyToMessageId));

        ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
    }
    public static string GetErrorMessage(string log, string keyword)
    {
        if (string.IsNullOrWhiteSpace(log))
            return string.Empty;

        // const string keyword = "Lỗi truy vấn";

        int index = log.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
        return index >= 0 ? log.Substring(index) : log;
    }
}