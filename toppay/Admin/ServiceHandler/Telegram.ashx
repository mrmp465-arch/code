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
                //if (resObj.message.text.StartsWith("/report"))
                //{

                //    if (!string.IsNullOrEmpty(Partnecode))
                //    {
                //        var FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                //        var ToDate = FromDate.AddDays(1).AddMilliseconds(-1);

                //        if (resObj.message.text.StartsWith("/report yesterday"))
                //        {
                //            FromDate = FromDate.AddDays(-1);
                //            ToDate = ToDate.AddDays(-1);
                //        }
                //        var lstDataBank = new BankGateAPI().ReportDashboard(Partnecode, FromDate, ToDate, 1);
                //        if (lstDataBank != null)
                //        {
                //            var mess1 = String.Format("{0} : {1}/{2} - {3} : {4} - Phí nạp : {5}", "Số lệnh nạp", lstDataBank.Sum(x => x.TotalTransSuccess).ToString(), lstDataBank.Sum(x => x.TotalTrans).ToString(), "Số tiền nạp", lstDataBank.Sum(x => x.TotalAmountSuccess).ToString("N0").Replace(".", ","), lstDataBank.Sum(x => x.TotalFee).ToString("N0").Replace(".", ","));
                //            SendTeleV2(resObj.message.chat.id.ToString(), mess1);

                //        }

                //        var lstDataBank2 = new BankCashAPI().ReportDashboard(Partnecode, FromDate, ToDate);
                //        if (lstDataBank2 != null)
                //        {
                //            var mess2 = String.Format("{0} : {1}/{2} - {3} : {4} - Phí rút : {5}", "Số lệnh rút", lstDataBank2.Sum(x => x.TotalTransSuccess).ToString(), lstDataBank2.Sum(x => x.TotalTrans).ToString(), "sô tiền rút", lstDataBank2.Sum(x => x.TotalAmountSuccess).ToString("N0").Replace(".", ","), lstDataBank2.Sum(x => x.TotalFee).ToString("N0").Replace(".", ","));
                //            SendTeleV2(resObj.message.chat.id.ToString(), mess2);
                //        }


                //    }
                //}
                //if (resObj.message.text.StartsWith("/checkout"))
                //{
                //    //var Partnecode = GetPartnerCode(resObj.message.chat.id.ToString());
                //    if (!string.IsNullOrEmpty(Partnecode))
                //    {
                //        var refcode = resObj.message.text.Replace("/checkout", "").TrimStart().Trim();
                //        var trans = new BankCashAPI().GetByRefcode(refcode, Partnecode);

                //        if (trans == null)
                //        {
                //            SendTeleV2(resObj.message.chat.id.ToString(), "Mã giao dịch " + refcode + " không tồn tại");
                //        }
                //        if (trans.Status >= 1)
                //        {
                //            SendTeleV2(resObj.message.chat.id.ToString(), "Mã giao dịch " + refcode + " đã thành công");
                //        }
                //        else
                //        {
                //            if (trans.Status == -1)
                //            {
                //                SendTeleV2(resObj.message.chat.id.ToString(), "Mã giao dịch " + refcode + " thất bại");
                //                System.Threading.Thread.Sleep(500);
                //                SendTeleV2(resObj.message.chat.id.ToString(), trans.LogContent);
                //            }
                //            else
                //            {
                //                SendTeleV2(resObj.message.chat.id.ToString(), "Mã giao dịch " + refcode + " đang chờ xử lý");
                //            }

                //        }
                //    }
                //}
                //if (resObj.message.text.StartsWith("/checkin"))
                //{
                //    //var Partnecode = GetPartnerCode(resObj.message.chat.id.ToString());
                //    if (!string.IsNullOrEmpty(Partnecode))
                //    {
                //        var refcode = resObj.message.text.Replace("/checkin", "").TrimStart().Trim();
                //        var trans = new BankGateAPI().GetByRefcode(refcode, Partnecode);

                //        if (trans == null)
                //        {
                //            SendTeleV2(resObj.message.chat.id.ToString(), "Mã giao dịch " + refcode + " không tồn tại");
                //        }
                //        if (trans.Status >= 1)
                //        {
                //            SendTeleV2(resObj.message.chat.id.ToString(), "Mã giao dịch " + refcode + " đã thành công ");
                //        }
                //        else
                //        {
                //            SendTeleV2(resObj.message.chat.id.ToString(), "Mã giao dịch " + refcode + " đang chờ xử lý");

                //        }
                //    }
                //}
                if (resObj.message.text.StartsWith("/balance") || resObj.message.text.StartsWith("/bal"))
                {
                    var user = new Users().GetByUserName(Partnecode);
                    if (user != null)
                        SendTeleV2(resObj.message.chat.id.ToString(), "Balance:  " + user.Balance.ToString("N0").Replace(".", ","));
                }
                if (resObj.message.text.StartsWith("/in"))
                {
                    var refcode = resObj.message.text.Replace("/in", "").Trim();
                    //refcode = resObj.message.text.Replace("/bout", "").Trim();
                    //refcode = resObj.message.text.Replace("/df", "").Trim();
                    var checkorder = DataCaching.GetCache<CheckOrder>("CheckOrder:" + Partnecode + refcode);
                    if (checkorder != null)
                    {
                        //NLogLogger.Info("check order by cache");
                        //return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        //{
                        //    ResponseContent = javaScriptSerializer.Serialize(checkorder)
                        //};
                        SendTeleV2(resObj.message.chat.id.ToString(), "Transaction success");

                        return;
                    }


                    var trans = new BankGateAPI().GetByRefcode(refcode, Partnecode);

                    if (trans == null)
                    {
                        //return new APIResponse((int)ResponseCode.TransactionNotExists);
                        SendTeleV2(resObj.message.chat.id.ToString(), "Transaction not found");
                        return;
                    }
                    if (trans.Status >= 1)
                    {
                        SendTeleV2(resObj.message.chat.id.ToString(), "Transaction success");
                        return;
                    }
                    else
                    {

                        SendTeleV2(resObj.message.chat.id.ToString(), "Transaction processing");
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
                        var url = String.Format("http://139.180.147.57:1587/DetailScreen5.aspx?orderNo={0}&refcode={1}&chatid={2}", checkorder.TransactionID, checkorder.RefCode, resObj.message.chat.id.ToString());
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
                        var url = String.Format("http://139.180.147.57:1587/DetailScreen5.aspx?orderNo={0}&refcode={1}&chatid={2}", trans.TransactionID, trans.RefCode, resObj.message.chat.id.ToString());
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
                                    SendTeleV2(resObj.message.chat.id.ToString(), " Bank receiving maintenance ");
                                    return;
                                }
                                else
                                {
                                    SendTeleV2(resObj.message.chat.id.ToString(), "Incorrect recipient account name");
                                    return;
                                }
                            }
                            SendTeleV2(resObj.message.chat.id.ToString(), "Transaction fail");
                            return;
                        }
                        if (trans.Status == -357)
                        {

                            if (trans.LogContent.Contains("TK không"))
                            {
                                SendTeleV2(resObj.message.chat.id.ToString(), " Bank receiving maintenance ");
                                return;
                            }
                            else
                            {
                                SendTeleV2(resObj.message.chat.id.ToString(), "Incorrect recipient account name");
                                return;
                            }
                            return;
                        }
                        SendTeleV2(resObj.message.chat.id.ToString(), "Transaction processing");
                        return;
                    }

                }
                if (resObj.message.text.StartsWith("/out") || resObj.message.text.StartsWith("/bout") || resObj.message.text.StartsWith("/df"))
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
                        SendTeleV2(resObj.message.chat.id.ToString(), "Transaction success");

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
                        SendTeleV2(resObj.message.chat.id.ToString(), "Transaction success");
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
                                    SendTeleV2(resObj.message.chat.id.ToString(), " Bank receiving maintenance ");
                                    return;
                                }
                                else
                                {
                                    SendTeleV2(resObj.message.chat.id.ToString(), "Incorrect recipient account name");
                                    return;
                                }
                            }
                            SendTeleV2(resObj.message.chat.id.ToString(), "Transaction fail");
                            return;
                        }
                        if (trans.Status == -357)
                        {

                            if (trans.LogContent.Contains("TK không"))
                            {
                                SendTeleV2(resObj.message.chat.id.ToString(), " Bank receiving maintenance ");
                                return;
                            }
                            else
                            {
                                SendTeleV2(resObj.message.chat.id.ToString(), "Incorrect recipient account name");
                                return;
                            }
                            return;
                        }
                        SendTeleV2(resObj.message.chat.id.ToString(), "Transaction processing");
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

            var requestUrl = string.Format("https://api.telegram.org/bot8675955812:AAEV_c3dgnUY4JzVGLIRgOdXmpuJJI9tCb4/sendMessage?chat_id={1}&text={0}", message, id);
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