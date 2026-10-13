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

namespace Libs.CardTelco.PayPlusAppV2VTT
{
    public class PayPlusCard : ICardTelcoHandler
    {

        protected string providerName = string.Empty;
        string serviceUrl = "http://127.0.0.1:1585/";

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
                int[] listValue = { 10000, 20000, 30000, 50000, 100000, 200000, 300000, 500000, 1000000 };
                //int[] listValue = { 50000, 100000, 200000, 300000, 500000, 1000000 };
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
                _CardAPILog.CreatTime = DateTime.Now;
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    if (_CardAPILog.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.CardSerial, request.CardCode, "Error Insert Data" });
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
                topupRequest.simTarget = "simtarget";
                topupRequest.cardSerial = request.CardSerial;
                topupRequest.cardCode = request.CardCode;
                topupRequest.telco = telcoCode;
                topupRequest.amount = request.AmountUser;
                topupRequest.transactionId = transaction.TransactionID;
                topupRequest.partnerCode = transaction.PartnerCode;
                topupRequest.providerCode = providerName;

                NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", transaction.TransactionID.ToString(), "PayPlusAppVTTTopupRequest", serializer.Serialize(topupRequest) });
                var tryAgain = 0;
                TopupResponse cardResult = ProcessCard(topupRequest);
                while ((cardResult.code == (int)ResponseCode.ServiceIsLocked || cardResult.code == (int)ResponseCode.TransactionLimit || cardResult.code == (int)ResponseCode.SystemBusy) && tryAgain < 3)
                {
                    cardResult = ProcessCard(topupRequest);
                    tryAgain++;
                    Thread.Sleep(1000);
                }
                NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", transaction.TransactionID.ToString(), "PayPlusAppVTTTopupResponse", serializer.Serialize(cardResult) });

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


                }
                else
                {
                    switch (cardResult.code)
                    {
                        case -1:
                            //Giao dịch failed
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -2:
                            //Nghi vấn
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -7:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionRejected);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -300:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemMaintain);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -320:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionNotExists);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -323:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -330:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -335:
                            //Mã nạp thẻ không đúng
                            _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -326:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionTimeout);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -324:
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionLimit);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -317:
                            //Không gửi được Request xuống Client My Viettel
                            _APIResponse = new APIResponse((int)ResponseCode.ServiceNotExists);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -303:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemBusy);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -334:
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -333:
                            _APIResponse = new APIResponse((int)ResponseCode.CardNotActivated);
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
                        NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", transaction.TransactionID.ToString(), "Error", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", transaction.TransactionID.ToString(), "Error", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }
            }

            if (_CardAPILog.TransactionID > 0)
            {
                _CardAPILog.Update();

                //Begin Callback for partner if process over 90s
                if (!string.IsNullOrEmpty(_CardAPILog.CallbackUrl) && (DateTime.Now - _CardAPILog.CreatTime).TotalSeconds >= 90)
                {
                    var privateKey = new Partners().Get(_CardAPILog.PartnerCode).PrivateKey;
                    var datacb = new DataCallback()
                    {
                        Amount = !string.IsNullOrEmpty(_APIResponse.ResponseContent) ? Convert.ToInt32(_APIResponse.ResponseContent) : 0,
                        RefCode = _CardAPILog.RequestNo,
                        Status = _APIResponse.ResponseCode,
                        Signature = Libs.Utils.Encrypts.MD5(_CardAPILog.RequestNo + _APIResponse.ResponseCode + _CardAPILog.Amount + privateKey)
                    };
                    Task.Run(async () => await CallbackJson(_CardAPILog.CallbackUrl, serializer.Serialize(datacb), _CardAPILog.PartnerCode + " " + _CardAPILog.TransactionID).ConfigureAwait(false));
                }
            }
            return _APIResponse;
        }

        public APIResponse ReCheck(string transactionId)
        {
            var log3Rd = new TopupMobile3rdLog().GetByTransactionId(Convert.ToInt64(transactionId));
            return null;
        }

        private TopupResponse ProcessCard(TopupRequest topupRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", "PayPlusAppVTTTopupRequest", serializer.Serialize(topupRequest) });
            //TopupAppVTTService _VPGService = new TopupAppVTTService(serviceUrl + "TopupAppV2VTT.asmx");
            TopupAppVTTService _VPGService = new TopupAppVTTService(serviceUrl + "MyViettel.asmx");
            _VPGService.Timeout = 300000;
            string serviceResponse = string.Empty;

            try
            {
                serviceResponse = _VPGService.RequestTopup(topupRequest.transactionId.ToString(), topupRequest.telco, topupRequest.partnerCode, topupRequest.providerCode, topupRequest.cardSerial, topupRequest.cardCode, topupRequest.simTarget, topupRequest.amount);
            }

            catch (TimeoutException e)
            {
                NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", "PayPlusAppVTTTopupRequest", "TimeoutException", topupRequest.transactionId.ToString(), e.Message });
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
                NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", "PayPlusAppVTTTopupRequest", "ThreadAbortException", topupRequest.transactionId.ToString(), e.Message });
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
                NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", "PayPlusAppVTTTopupRequest", "Exception", topupRequest.transactionId.ToString(), e.Message });
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The Exception",
                    amount = 0

                };
            }

            try
            {
                NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", "PayPlusAppVTTTopupRequest", topupRequest.transactionId.ToString(), serviceResponse });
                return serializer.Deserialize<TopupResponse>(serviceResponse);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "PayPlusAppVTTTopup", "PayPlusAppVTTTopupRequest", topupRequest.transactionId.ToString(), e.Message });
                return new TopupResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The Exception Deserialize",
                    amount = 0

                };
            }

        }

        private async Task<string> CallbackJson(string url, string postData, string code)
        {
            NLogLogger.Info(new string[] { "PayPlusCard", "Callback Partner", "Request", code, url, postData });

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
                        NLogLogger.Info(new string[] { "PayPlusCard", "Callback Partner", "Request", code, url, postData });
                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "PayPlusCard", "Callback Partner", "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }

    }

}
