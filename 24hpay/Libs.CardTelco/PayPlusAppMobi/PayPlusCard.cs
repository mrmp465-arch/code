using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;
using System.Security.Cryptography;
using Libs.Report;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Libs.CardTelco.PayPlusAppMobi
{
    public class PayPlusCard : ICardTelcoHandler
    {

        protected string providerName = string.Empty;
        //string serviceUrl = "http://127.0.0.1:1584/";
        //private string serviceUrl = ConfigurationManager.AppSettings["AppMobiService"] ?? "http://127.0.0.1:1586/"; //MobiNext
        private string serviceUrl = ConfigurationManager.AppSettings["AppMobiService"] ?? "http://127.0.0.1:1591/"; //MyMobi
        public APIResponse UseCard(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

            providerName = transaction.ProviderCode;
            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;
                UseCardRequest request = new UseCardRequest();
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);
                int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000 };
                //int[] listValue = { 50000, 100000, 200000, 300000, 500000 };
                if (!listValue.Contains(request.AmountUser))
                {
                    return new APIResponse((int)ResponseCode.CardAmountInvalid);
                }

                if (!Utils.GlobalHelper.CheckCardCode(request.CardType, request.CardCode))
                {
                    return new APIResponse((int)ResponseCode.CardCodeInvalid);
                }

                if (!Utils.GlobalHelper.CheckCardSerial(request.CardType, request.CardSerial))

                {
                    return new APIResponse((int)ResponseCode.CardSerialInvalid);
                }

                // Bước: Ghi log giao dịch
                step = 2;
                _CardAPILog.TransactionID = transaction.TransactionID;
                _CardAPILog.PartnerID = transaction.PartnerID;
                _CardAPILog.PartnerCode = transaction.PartnerCode;
                _CardAPILog.AccountName = request.AccountName;
                _CardAPILog.AccountID = 0;
                _CardAPILog.CardSerial = request.CardSerial;
                _CardAPILog.CardCode = request.CardCode;
                _CardAPILog.CardType = request.CardType;
                _CardAPILog.Amount = 0;
                _CardAPILog.AmountUser = request.AmountUser;
                _CardAPILog.Provider = providerName;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.RequestNo = request.RefCode;
                _CardAPILog.CallbackUrl = request.CallbackUrl;
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    if (_CardAPILog.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "MobiAppCardTopup", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.CardSerial, request.CardCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse(_CardAPILog.ReturnValue);
                }

                // Bước: gọi hàm sang API
                step = 3;
                string telcoCode = "";
                switch (_CardAPILog.CardType.ToLower())
                {
                    case "vms":
                        telcoCode = "vms";
                        break;
                    case "vnp":
                        telcoCode = "vnp";
                        break;
                    case "viettel":
                        telcoCode = "vtt";
                        break;

                }

                TopupRequest topupRequest = new TopupRequest();
                topupRequest.sim = "simweb";
                topupRequest.simTarget = "simtarget";
                topupRequest.cardSerial = request.CardSerial;
                topupRequest.cardCode = request.CardCode;
                topupRequest.telco = telcoCode;
                topupRequest.clientId = string.Empty;
                topupRequest.amount = request.AmountUser;
                topupRequest.transactionId = transaction.TransactionID;
                topupRequest.partnerCode = transaction.PartnerCode;
                topupRequest.providerCode = providerName;
                topupRequest.slot = 0;

                NLogLogger.Info(new string[] { "MobiAppCardTopup", transaction.TransactionID.ToString(), "MobiAppCardTopupRequest", serializer.Serialize(topupRequest) });
                var tryAgain = 0;
                TopupResponse cardResult = ProcessCard(topupRequest);
                while ((cardResult.code == (int)ResponseCode.ServiceIsLocked || cardResult.code == (int)ResponseCode.TransactionLimit || cardResult.code == (int)ResponseCode.SimNotActive) && tryAgain < 3)
                {
                    if (cardResult.code == (int)ResponseCode.SimNotActive) topupRequest.slot = 1; //Gửi qua USSD
                    cardResult = ProcessCard(topupRequest);
                    tryAgain++;
                    Thread.Sleep(1000);
                }
                NLogLogger.Info(new string[] { "MobiAppCardTopup", transaction.TransactionID.ToString(), "MobiAppCardTopupResponse", serializer.Serialize(cardResult) });

                step = 4;
                // Nếu thành công
                if (cardResult.code == 1)
                {
                    if (_CardAPILog.AmountUser == Convert.ToInt64(cardResult.amount))
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    }
                    else
                    {
                        _APIResponse = new APIResponse((int)ResponseCode.CardAmountInvalid);
                    }
                    _CardAPILog.Amount = Convert.ToInt64(cardResult.amount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount;
                    _CardAPILog.Status = 1;

                    //callback neu time >90s
                    if(DateTime.Now.AddSeconds(-90)>=_CardAPILog.CreatTime)
                    {
                        //CallBack Partner
                        var privateKey = new Partners().Get(_CardAPILog.PartnerCode).PrivateKey;
                        if (!string.IsNullOrEmpty(_CardAPILog.CallbackUrl))
                        {
                            var datacb = new DataCallback()
                            {
                                Amount = _CardAPILog.Amount,
                                RefCode = _CardAPILog.RequestNo,
                                Status = 1,
                                Signature = Libs.Utils.Encrypts.MD5(_CardAPILog.RequestNo + "1" + _CardAPILog.Amount + privateKey)
                            };
                            Task.Run(() => CallbackJson(_CardAPILog.CallbackUrl, serializer.Serialize(datacb), _CardAPILog.PartnerCode, 0, 2).ConfigureAwait(false));
                        }
                    }    
                }
                else
                {
                    switch (cardResult.code)
                    {
                        case 0:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionWaitting);
                            _CardAPILog.Status = 0;
                            break;
                        case -1:
                            //Giao dịch failed
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -7:
                            //Không tìm thấy Transaction --> TH này rất khả năng đưa vào Black list
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionRejected);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;

                        case -335:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;

                        case -323:
                            //Captcha sai
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;

                        case -320:
                            //Không tìm thấy Transaction --> TH này rất khả năng đưa vào Black list
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionNotExists);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -330:
                            //Card Used
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -303:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemBusy);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -326:
                            //Timeout
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionTimeout);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -2:
                            //Nghi vấn
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;

                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                    }
                    _CardAPILog.Description = serializer.Serialize(cardResult);
                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "MobiAppCardTopup", transaction.TransactionID.ToString(), "Error", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "MobiAppCardTopup", transaction.TransactionID.ToString(), "Error", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "MobiAppCardTopup", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "MobiAppCardTopup", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "MobiAppCardTopup", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }
            }

            if (_CardAPILog.TransactionID > 0 && _CardAPILog.Status != 0)
            {
                _CardAPILog.Update();
            }
            return _APIResponse;
        }
        public async Task<string> CallbackJson(string url, string postData, string code, long tranId = 0, int type = 0)
        {
            NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Request", code, tranId.ToString(), url, postData });

            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(url, httpContent).ConfigureAwait(false);

                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Response", code, tranId.ToString(), url, postData, responseContent });
                        try
                        {
                            if (responseContent.Contains("1|"))
                            {
                                NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Process TRUE", responseContent });
                                if (type == 1)
                                {
                                    new TopupMobile3rdLog().UpdateCallback(tranId, 1, null);
                                }
                                else if (type == 2)
                                {
                                    new TopupMobile3rdLog().UpdateCallback(tranId, null, 1);
                                }
                            }
                            else
                            {
                                NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Process FAIL", responseContent });
                                if (type == 1)
                                {
                                    new TopupMobile3rdLog().UpdateCallback(tranId, -1, null);
                                }
                                else if (type == 2)
                                {
                                    new TopupMobile3rdLog().UpdateCallback(tranId, null, -1);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Error", e.Message });
                        }
                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
        public APIResponse ReCheck(string transactionId)
        {
            throw new NotImplementedException();
        }
        public class DataCallback
        {
            public string RefCode { get; set; }
            public int Status { get; set; }
            public long Amount { get; set; }
            public string Signature { get; set; }

        }

        private TopupResponse ProcessCard(TopupRequest topupRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[] { "MobiAppCardTopup", "MobiAppCardTopupRequest", serializer.Serialize(topupRequest) });
            //TopupWebMobile _VPGService = new TopupWebMobile(serviceUrl + "MobiNext.asmx");
            TopupWebMobile _VPGService = new TopupWebMobile(serviceUrl + "MyMobi.asmx");
            _VPGService.Timeout = 300000;
            string serviceResponse = string.Empty;

            try
            {
                serviceResponse = _VPGService.RequestTopup(topupRequest.transactionId.ToString(), topupRequest.telco, topupRequest.partnerCode, topupRequest.providerCode, topupRequest.sim, topupRequest.simTarget, topupRequest.clientId, topupRequest.cardSerial, topupRequest.cardCode, topupRequest.slot, topupRequest.amount);
            }

            catch (TimeoutException e)
            {
                NLogLogger.Info(new string[] { "MobiAppCardTopup", "MobiAppCardTopupRequest", "TimeoutException", topupRequest.transactionId.ToString(), e.Message });
                Thread.ResetAbort();
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The Timeout",
                    amount = 0

                };
            }

            catch (ThreadAbortException e)
            {
                NLogLogger.Info(new string[] { "MobiAppCardTopup", "MobiAppCardTopupRequest", "ThreadAbortException", topupRequest.transactionId.ToString(), e.Message });
                Thread.ResetAbort();
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The Thread Abort",
                    amount = 0

                };
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MobiAppCardTopup", "MobiAppCardTopupRequest", "Exception", topupRequest.transactionId.ToString(), e.Message });
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The Exception",
                    amount = 0

                };
            }

            try
            {
                NLogLogger.Info(new string[] { "MobiAppCardTopup", "MobiAppCardTopupRequest", topupRequest.transactionId.ToString(), serviceResponse });
                return serializer.Deserialize<TopupResponse>(serviceResponse);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "MobiAppCardTopup", "MobiAppCardTopupRequest", topupRequest.transactionId.ToString(), e.Message });
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The Exception Deserialize",
                    amount = 0

                };
            }

        }


    }

}
