using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Services.Configuration;
using APITopupMobile.Service;
using FirebaseNet.Messaging;
using Libs.API;
using Libs.Report;
using Libs.Utils;

namespace APITopupMobile
{
    internal class TopupAppVTTV2
    {
        private const string ServiceUrl = "http://192.64.115.34:21080/";

        private const string CallBackUrl = "http://149.28.130.246:1583/";
        //private string ServerCallBackUrl = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerCallBackUrl"]) ? ConfigurationManager.AppSettings["ServerCallBackUrl"] : "http://35.240.137.60:1583";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public string SendAppVTTV2Topup(string transactionId, string telco, string partnerCode, string providerCode, string serial, string pin, int amount)
        {

            //Lấy ra để Process 

            var topupProcess = new TopupMobileLog().GetProcess(telco, amount, partnerCode, providerCode);
            if (topupProcess == null)
            {
                NLogLogger.Info(new string[] { "SendAppVTTV2Topup", "Order Not Found 1", transactionId });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            if (topupProcess.TransactionID == 0)
            {
                NLogLogger.Info(new string[] { "SendAppVTTV2Topup", "Order Not Found 2", transactionId });
                return string.Format("{0}|{1}", -320, 0); //Không tìm thấy transaction
            }

            var topupType = Convert.ToInt32(topupProcess.TopupType);
            switch (topupType)
            {
                case 1:
                case 2:
                    topupType = 1;
                    break;
                case 3:
                    topupType = 0;
                    break;
            }

            var topup = new TopupMobile3rdLog();
            topup.RequestNo = topupProcess.TransactionID; //Id của bảng Order
            topup.TransactionId = Convert.ToInt64(transactionId);
            topup.PartnerCode = partnerCode;
            topup.ProviderCode = providerCode;
            topup.Telco = telco;
            topup.CardSerial = serial;
            topup.CardCode = pin;
            topup.Amount = amount;
            topup.SimTarget = topupProcess.Mobile;
            topup.Add();
            topup.Id = topup.ReturnValue;


            try
            {

                NLogLogger.Info(new string[] { "SendAppVTTV2Topup", "Request", serializer.Serialize(topup) });
                var parameters = new Dictionary<string, string>();
                parameters.Add("transid", topup.Id.ToString());
                parameters.Add("seri", serial);
                parameters.Add("code", pin);
                parameters.Add("phone", topupProcess.Mobile);
                parameters.Add("type", topupType.ToString());
                parameters.Add("amount", amount.ToString());
                parameters.Add("callback", CallBackUrl + "SendAppVTTV2Topup.ashx");

                var responseJSON = PostCard(ServiceUrl + "api/myvt/topup", parameters).Result;
                NLogLogger.Info(new string[] { "SendAppVTTV2Topup", "Response", topup.Id.ToString(), responseJSON.ToString() });
                if (!string.IsNullOrEmpty(responseJSON))
                {
                    var response = serializer.Deserialize<ResponseData>(responseJSON);

                    if (response.code != 0)
                    {
                        switch (response.data.state)
                        {
                            case -1: //-1: Thẻ sai, nội dung chi tiết ở message
                                topupProcess.Topup(0, 0);
                                topup.Status = (int)ResponseCode.CardCodeInvalid;
                                topup.Update();
                                return string.Format("{0}|{1}", (int)ResponseCode.CardCodeInvalid, 0);
                            case 0: //Thẻ chưa được gạch do tài khoản myviettel không thể nạp cho thuê bao ISDN. Cái này có rất nhiều vấn đề. Có thể do thuê bao ISDN không thể thanh toán bằng phương thức thẻ cào, có thể do tài khoản myviettel không đủ khả năng thanh toán cho  thuê bao đó. Phương án giải quyết cho vấn đề này là tạo 1 đơn mới sử dụng thẻ của đơn này nhưng ghép thanh toán cho thuê bao khác.
                                topupProcess.Topup(0, 0);
                                topup.Status = (int)ResponseCode.TransactionRejected;
                                topup.Update();
                                return string.Format("{0}|{1}", (int)ResponseCode.TransactionRejected, 0);
                            case 1: //Thẻ nạp thành công. Nhưng có 2 vấn đề . Nạp thành công trả về mệnh giá(đối với thuê bao trả sau thì message luôn luôn có mệnh giá) .Nạp thành công nhưng nhà mạng không trả mệnh giá(nạp cho thuê bao trả trước).Cái này thì tùy anh xử lý. Mệnh giá thực của thẻ nằm ở "msg" được trả kèm
                                var strAmount = Regex.Match(response.data.msg, @"\d+").Value;
                                int amountCard = !string.IsNullOrEmpty(strAmount) ? Convert.ToInt32(strAmount) : 0;
                                topupProcess.Topup(1, amountCard);
                                topup.Status = (int)ResponseCode.TransactionSuccessful;
                                topup.Update();
                                return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuccessful, amountCard);
                            case 2: //Nạp bị nghi vấn. Nhận callback
                                topupProcess.Topup(1, amount);
                                topup.Status = (int)ResponseCode.TransactionSuspicious;
                                topup.Update();
                                return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuspicious, amount);
                            default:
                                topupProcess.Topup(0, 0);
                                topup.Status = (int)ResponseCode.TransactionFailed;
                                topup.Update();
                                return string.Format("{0}|{1}", (int)ResponseCode.TransactionFailed, 0);
                        }
                    }
                    else
                    {
                        topupProcess.Topup(0, 0);
                        topup.Status = (int)ResponseCode.TransactionFailed;
                        topup.Update();
                        return string.Format("{0}|{1}", (int)ResponseCode.TransactionSuspicious, amount);
                    }
                }


                NLogLogger.Info(new string[] { "SendAppVTTV2Topup", "Response failed", topup.Id.ToString() });
                //Update Service Not Exists
                topupProcess.Topup(0, 0);
                topup.Status = (int)ResponseCode.ServiceNotExists;
                topup.Update();
                return string.Format("{0}|{1}", (int)ResponseCode.ServiceNotExists, 0);

            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "SendAppVTTV2Topup", "Response failed Exeption", topup.Id.ToString(), exp.Message });
                //Update Service Not Exists
                topupProcess.Topup(0, 0);
                topup.Status = (int)ResponseCode.ServiceNotExists;
                topup.Update();
                return string.Format("{0}|{1}", (int)ResponseCode.ServiceNotExists, 0);
            }

        }


        public async Task<string> PostCard(string url, Dictionary<string, string> postData)
        {
            try
            {
                var httpContent = new FormUrlEncodedContent(postData);
                //var client = new HttpClient(new HttpClientHandler{ClientCertificateOptions = ClientCertificateOption.Automatic});
                var client = new HttpClient();
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var response = await client.PostAsync(url, httpContent);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "SendAppVTTV2Topup", "PostCard", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }



    public class Data
    {
        public string msg { get; set; }
        public string code { get; set; }
        public string transid { get; set; }
        public int remain { get; set; }
        public int state { get; set; }
        public string seri { get; set; }
    }

    public class ResponseData
    {
        public string msg { get; set; }
        public int code { get; set; }
        public Data data { get; set; }
    }


}