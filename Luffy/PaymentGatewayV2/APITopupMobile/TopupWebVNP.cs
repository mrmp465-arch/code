using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Services.Configuration;
using FirebaseNet.Messaging;
using Lib.Captcha;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APITopupMobile
{
    internal class TopupWebVNP
    {

        private string urlService = ConfigurationManager.AppSettings["Web_AddPrepaid_Vina"] ?? "http://naptien.vinaphone.com.vn/Home/AddPrepaid";
        private bool ReportIncorrectCaptcha = bool.Parse(ConfigurationManager.AppSettings["Report_Incorrect_Captcha"] ?? "true");


        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public string SendWebTopupCard(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, string sim, string simTarget, string clientId, int slot, int amount)
        {
            //Lấy ra để Process 
            var topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode);
            if (topupProcess == null)
            {
                var chat_id = -318818065;
                switch (providerCode)
                {
                    case "glbxhr":
                        chat_id = -315078196;
                        break;
                    case "datxhr":
                        chat_id = -284948741;
                        break;
                    case "ppxhrv2":
                        chat_id = -267661727;
                        break;
                    case "mrxxhr":
                        chat_id = -155536217;
                        break;

                }

                var tms = Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, string.Format("Có nghi vấn hết đơn hàng {0} hiện tại thẻ mệnh giá {1} không tìm thấy đơn phù hợp. Các anh check giúp em (^_^)", telco, amount)));
                tms.Wait();

                NLogLogger.Info(new string[] { "TopupAppVNTP", "Order Not Found 1", transactionId, amount.ToString(), partnerCode, providerCode });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            if (topupProcess.TransactionID == 0)
            {
                NLogLogger.Info(new string[] { "TopupWebVNP", "Order Not Found 2", transactionId });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            NLogLogger.Info(new string[] { "TopupWebVNP", topupProcess.TransactionID.ToString(), topupProcess.FullName, topupProcess.Mobile });

            if (topupProcess.TopupType == 1)
            {
                var topup = new TopupMobile3rdLog();
                topup.RequestNo = topupProcess.TransactionID;
                topup.TransactionId = Convert.ToInt64(transactionId);
                topup.PartnerCode = partnerCode;
                topup.ProviderCode = providerCode;
                topup.Telco = telco;
                topup.ClientId = clientId;
                topup.Sim = sim;
                topup.SimTarget = topupProcess.Mobile;
                topup.CardSerial = serial;
                topup.CardCode = pin;
                topup.Amount = amount;
                topup.Add();
                topup.Id = topup.ReturnValue;

                try
                {
                    // Vào DB lấy Session đã tạo trước nếu không có khởi tạo mới
                    //var session = new DeCaptchaWebVina().DeCaptcha();

                    var session = new Captcha().GetCaptcha(1);
                    if (session == null) session = new DeCaptchaWebVina().DeCaptcha();

                    //Gen truoc x Captcha
                    var captchaQuanity = 1;
                    for (int i = 0; i < captchaQuanity; i++)
                    {

                        Action<int> send = PreGenCaptCha;
                        send.BeginInvoke(0, null, null);


                    }

                    string parameters = string.Format("PhoneNum={0}&MaThe={1}&Answer={2}", topupProcess.Mobile, pin, session.Value);
                    NLogLogger.Info(new string[] { "TopupWebVNP", transactionId, "Request", parameters });
                    var res = PostForm(urlService, parameters, session.SessionId);
                    NLogLogger.Info(new string[] { "TopupWebVNP", transactionId, "Response", res }); //{"success":true,"ErrMsg":"Bạn đã nạp thẻ thành công! Số tiền: 10000.0đồng!"} trả sau ko có cước
                    //topup.RequestNo = topupProcess.TransactionID;
                    //topup.SimTarget = topupProcess.Mobile;
                    topup.LogContent = res;
                    var response = serializer.Deserialize<CardResponse>(res);
                    var strAmount = Regex.Match(response.ErrMsg, @"\d+").Value;
                    response.Amount = !string.IsNullOrEmpty(strAmount) ? Convert.ToInt32(Regex.Match(response.ErrMsg, @"\d+").Value) : 0;

                    if (response.success)
                    {

                        if (response.Amount == 0)
                        {
                            topupProcess.Topup(-1, 0); // Update trạng thái bị khóa
                            topup.Status = (int)ResponseCode.TransactionRejected;
                            topup.Amount = response.Amount;
                            topup.Update();
                            NLogLogger.Info(new string[] { "TopupWebVNP", "TB ko đúng", topupProcess.TransactionID.ToString(), topupProcess.FullName, topupProcess.Mobile });

                            return string.Format("{0}|{1}", (int)ResponseCode.TransactionRejected, 0); //TransactionRejected = -7,
                        }

                        topup.Status = 1;
                        topup.Amount = response.Amount;
                        topup.Update();
                        topupProcess.Topup(1, response.Amount); // Thanh công update Amount
                        topupProcess.Topup(0, 0); // mở lại cho chạy
                        return string.Format("{0}|{1}", 1, response.Amount);
                    }

                    topup.Status = ConvertResponCode(response.ErrMsg);
                    topup.Amount = 0;
                    topup.Update();
                    topupProcess.Topup(0, 0); //Mở lại cho chạy

                    if (topup.Status == (int) ResponseCode.ServiceIsLocked)
                    {
                        topupProcess.Topup(-1, 0); // Update trạng thái bị khóa

                        var chat_id = -318818065;
                        switch (providerCode)
                        {
                            case "glbxhr":
                                chat_id = -315078196;
                                break;
                            case "datxhr":
                                chat_id = -280811434;
                                break;
                            case "ppxhrv2":
                                chat_id = -267661727;
                                break;
                            case "mrxxhr":
                                chat_id = -155536217;
                                break;
                        }

                        var tms = Task.Run(() => TelegramClient.TelegramSendMessage(chat_id, string.Format("Thuê bao {0} của mạng {1} đã bị Telco khóa nạp. Các anh check đơn giúp em (^_^)", topupProcess.Mobile, topupProcess.Telco)));
                        tms.Wait();
                    }
                    else if (topup.Status == (int)ResponseCode.ParameterInvalid)
                    {
                        topupProcess.Topup(0, 0); // Thất bại update trả trạng thái đợi
                        if (ReportIncorrectCaptcha)
                        {
                            Action<int> send = ReportCaptcha;
                            send.BeginInvoke(session.TaskId, null, null);
                        }
                    }
                    else
                        topupProcess.Topup(0, 0); // Thất bại update trả trạng thái đợi

                    return string.Format("{0}|{1}", ConvertResponCode(response.ErrMsg), 0);
                }
                catch (Exception ex)
                {
                    topupProcess.Topup(0, 0);
                    NLogLogger.Info(new string[] { "SendWebTopupCard", "Error", ex.Message });
                    return string.Format("{0}|{1}", -1, 0);
                }
            }
            else
            {
                topupProcess.Topup(-1, 0);
                return string.Format("{0}|{1}", (int)ResponseCode.ServiceIsLocked, 0);
            }

            return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction

        }
        private static string PostForm(string url, string parameters, string cookie)
        {

            var uri = new Uri(url);
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.Method = "POST";
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.Add(new Cookie("ASP.NET_SessionId", cookie, "/", uri.Host));
            //req.CookieContainer.Add(new Cookie("TS012b5e7b", "014743ac897e51c60c57a0fb3f2128a55b81b75e77419aee9e1ab1a275d81d22dc8b93b1e2f93283f6c0aa353a9d4e0971e1d1c2c938d3d7ed1fee75135d78089e1ed051c2", "/", uri.Host));
            //req.Timeout = 30000;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(parameters);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null) return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();

        }

        public static int ConvertResponCode(string message)
        {
            switch (message)
            {
                case "Mã số thẻ nạp không đúng!":
                    //Mã số thẻ nạp không đúng!
                    NLogLogger.Info(new string[] { "SendWebTopupCard", "LogMessage", message });
                    return (int)ResponseCode.CardCodeInvalid;

                case "Captcha không đúng":
                    NLogLogger.Info(new string[] { "SendWebTopupCard", "LogMessage", message });
                    //Clam Captchar
                    return (int)ResponseCode.ParameterInvalid;

                case "Đã có lỗi xảy ra, nạp thẻ không thành công!":
                case "Thuê bao không được phép nạp tiền!":
                case "Bạn đã nạp thẻ sai quá nhiều, hệ thống tạm khóa chức năng nạp thẻ đối với thuê bao này! Vui lòng thử lại sau 12h nữa!":
                    NLogLogger.Info(new string[] { "SendWebTopupCard", "LogMessage", message });
                    //Update
                    return (int)ResponseCode.ServiceIsLocked;

                default:
                    return (int)ResponseCode.TransactionFailed;
            }
        }

        private void PreGenCaptCha(int id)
        {
            var session = new DeCaptchaWebVina().DeCaptcha();
            session.Type = 1;
            session.Add();
        }

        private void ReportCaptcha(int TaskId)
        {
            var result = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(TaskId);
            NLogLogger.Info(new string[] { "SendWebTopupCard", "ReportCaptcha", TaskId.ToString(), result.ToString() });
        }
    }

    public class CardResponse
    {
        //{"success":true,"ErrMsg":"Bạn đã nạp thẻ thành công!"}
        public bool success { get; set; }
        public string ErrMsg { get; set; }
        public int Amount { get; set; }
    }




}