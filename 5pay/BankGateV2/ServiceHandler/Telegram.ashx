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



                var resObj = javaScriptSerializer.Deserialize<Callback>(jsonString);

                if (resObj.message.text.StartsWith("/getgroup"))
                {
                    SendTeleV2(resObj.message.chat.id.ToString(), resObj.message.chat.id.ToString());
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
                        SendTeleV2(resObj.message.chat.id.ToString(), "Balance " +Partnecode +" :  " + user.Balance.ToString("N0").Replace(".", ","));
                }
                if (resObj.message.text.StartsWith("/in"))
                {
                    var refcode = resObj.message.text.Replace("/in", "").Trim();
                    //refcode = resObj.message.text.Replace("/bout", "").Trim();
                    //refcode = resObj.message.text.Replace("/df", "").Trim();
                    var checkorder = DataCaching.GetCache<CheckOrder>("CheckOrder:" + Partnecode + refcode);
                    if (checkorder != null)
                    {

                        SendTeleV2(resObj.message.chat.id.ToString(), "Giao dịch thành công | Transaction success | 交易成功");

                        return;
                    }


                    var trans = new BankGateAPI().GetByRefcode(refcode, Partnecode);

                    if (trans == null)
                    {
                        //return new APIResponse((int)ResponseCode.TransactionNotExists);
                        SendTeleV2(resObj.message.chat.id.ToString(), "Không tìm thấy đơn | Transaction not found | 没有找到应用程序");
                        return;
                    }
                    if (trans.Status >= 1)
                    {
                          SendTeleV2(resObj.message.chat.id.ToString(), "Giao dịch thành công | Transaction success | 交易成功");
                        return;
                    }
                    else
                    {

                        SendTeleV2(resObj.message.chat.id.ToString(), "Giao dịch đang chờ xử lý | Transaction processing | 交易待处理");
                        return;
                    }

                }
                if (resObj.message.text.StartsWith("/bill"))
                {
                    var refcode = resObj.message.text.Replace("/bill", "").Trim();
                    //refcode = resObj.message.text.Replace("/bout", "").Trim();
                    //refcode = resObj.message.text.Replace("/df", "").Trim();
                    var checkorder = DataCaching.GetCache<CheckOrder>("CheckOutOrder:" + Partnecode + refcode);
                    if (checkorder != null)
                    {
                        //NLogLogger.Info("check order by cache");
                        //return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        //{
                        //    ResponseContent = javaScriptSerializer.Serialize(checkorder)
                        //};
                        var url = String.Format("http://139.180.147.57:1587//DetailScreen3.aspx?orderNo={0}&refcode={1}&chatid={2}", checkorder.TransactionID, checkorder.RefCode, resObj.message.chat.id.ToString());
                        SendBill(url);
                        //SendTeleV2(resObj.message.chat.id.ToString(), "Transaction success");

                        return;
                    }


                    var trans = new BankCashAPI().GetByRefcode(refcode, Partnecode);

                    if (trans == null)
                    {
                        //return new APIResponse((int)ResponseCode.TransactionNotExists);
                        SendTeleV2(resObj.message.chat.id.ToString(), "Transaction not found");
                        return;
                    }
                    if (trans.Status >= 1)
                    {
                        var url = String.Format("http://139.180.147.57:1587//DetailScreen3.aspx?orderNo={0}&refcode={1}&chatid={2}", trans.TransactionID, trans.RefCode, resObj.message.chat.id.ToString());
                        SendBill(url);
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
                                    SendTeleV2(resObj.message.chat.id.ToString(), "Ngân hàng nhận tiền lỗi | The bank accepts the money for the error | 银行接受了因错误造成的损失。 ");
                                    return;
                                }
                                else
                                {
                                    SendTeleV2(resObj.message.chat.id.ToString(), "Tài khoản nhận tiền không trùng khớp | The receiving account does not match | 收款账户不匹配。");
                                    return;
                                }
                            }
                            SendTeleV2(resObj.message.chat.id.ToString(), "Lệnh out thất bại do ngân hàng lỗi | The  order failed due to a bank error |由於銀行故障，退出指令失敗。");
                            return;
                        }
                        if (trans.Status == -357)
                        {

                            if (trans.LogContent.Contains("TK không"))
                            {
                                SendTeleV2(resObj.message.chat.id.ToString(), "Ngân hàng nhận tiền lỗi | The bank accepts the money for the error | 银行接受了因错误造成的损失。 ");
                                return;
                            }
                            else
                            {
                                SendTeleV2(resObj.message.chat.id.ToString(), "Tài khoản nhận tiền không trùng khớp | The receiving account does not match | 收款账户不匹配。");
                                return;
                            }
                            // return;
                        }
                        SendTeleV2(resObj.message.chat.id.ToString(), "Lệnh đang được xử lý | The order is being processed.| 订单正在处理中。");
                        return;
                    }

                }
                if (resObj.message.text.StartsWith("/out"))
                {
                    var refcode = resObj.message.text.Replace("/out", "").Trim();
                    //refcode = resObj.message.text.Replace("/bout", "").Trim();
                    //refcode = resObj.message.text.Replace("/df", "").Trim();
                    var checkorder = DataCaching.GetCache<CheckOrder>("CheckOutOrder:" + Partnecode + refcode);
                    if (checkorder != null)
                    {
                        //NLogLogger.Info("check order by cache");
                        //return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        //{
                        //    ResponseContent = javaScriptSerializer.Serialize(checkorder)
                        //};
                        SendTeleV2(resObj.message.chat.id.ToString(), "Lệnh out thành công | Transaction Successful | 输出命令成功 ");

                        return;
                    }


                    var trans = new BankCashAPI().GetByRefcode(refcode, Partnecode);

                    if (trans == null)
                    {
                        //return new APIResponse((int)ResponseCode.TransactionNotExists);
                        SendTeleV2(resObj.message.chat.id.ToString(), "Lệnh out không tìm thấy | Transaction Not Found | 未找到 out 命令 ");
                        return;
                    }
                    if (trans.Status >= 1)
                    {
                        SendTeleV2(resObj.message.chat.id.ToString(), "Lệnh out thành công | Transaction Successful | 输出命令成功 ");
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
                                    SendTeleV2(resObj.message.chat.id.ToString(), "Ngân hàng nhận tiền lỗi | The bank accepts the money for the error | 银行接受了因错误造成的损失。 ");
                                    return;
                                }
                                else
                                {
                                    SendTeleV2(resObj.message.chat.id.ToString(), "Tài khoản nhận tiền không trùng khớp | The receiving account does not match | 收款账户不匹配。");
                                    return;
                                }
                            }
                            SendTeleV2(resObj.message.chat.id.ToString(), "Lệnh out thất bại do ngân hàng lỗi | The  order failed due to a bank error |由於銀行故障，退出指令失敗。");
                            return;
                        }
                        if (trans.Status == -357)
                        {

                            if (trans.LogContent.Contains("TK không"))
                            {
                                SendTeleV2(resObj.message.chat.id.ToString(), "Ngân hàng nhận tiền lỗi | The bank accepts the money for the error | 银行接受了因错误造成的损失。 ");
                                return;
                            }
                            else
                            {
                                SendTeleV2(resObj.message.chat.id.ToString(), "Tài khoản nhận tiền không trùng khớp | The receiving account does not match | 收款账户不匹配。");
                                return;
                            }

                        }
                        SendTeleV2(resObj.message.chat.id.ToString(), "Lệnh đang được xử lý | The order is being processed.| 订单正在处理中。");
                        return;
                    }

                }
                //if (resObj.message.text.StartsWith("/cash"))
                //{
                //    var amount = resObj.message.text.Replace("/cash ", "").Replace(",", "").Replace(",", "");

                //}
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
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

}