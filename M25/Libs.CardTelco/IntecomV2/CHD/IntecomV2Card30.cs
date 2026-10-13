using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;
using static Libs.CardTelco.IntecomV2.CHD.IntecomV2CardLib;

namespace Libs.CardTelco.IntecomV2.CHD
{
    public class IntecomV2Card30 : ICardTelcoHandler
    {

        protected string PartnerCode = "intecom30";

        // Production
        protected string ServiceUrl = "http://api.vtcebank.vn:8888/VTCCardAPI/Card";
        protected string PartnerId = "220221";
        protected string PartnerKey = "04f74af0580b08cf644eb80397cfd4c5";
        protected string PartnerServiceCode = "BAONINH_RONGCHIEN";

        public APIResponse UseCard(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;
                UseCardRequest request = new UseCardRequest();
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);

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
                _CardAPILog.Provider = PartnerCode;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.RequestNo = request.RefCode;
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    return new APIResponse(_CardAPILog.ReturnValue);
                }

                // Bước: gọi hàm sang API
                step = 3;
                string telcoCode = "";
                switch (_CardAPILog.CardType.ToLower())
                {

                    case "vcoin":
                        telcoCode = "VCOIN";
                        break;

                }
                CardRequest cardRequest = new CardRequest();
                cardRequest.FunctionName = "UseCard";
                cardRequest.CardType = telcoCode;
                cardRequest.CardSerial = request.CardSerial;
                cardRequest.CardCode = IntecomV2CardLib.Encrypt(request.CardCode, PartnerKey);
                cardRequest.PartnerCode = PartnerId;
                cardRequest.PartnerServiceCode = PartnerServiceCode;
                cardRequest.AccountName = request.AccountName;
                cardRequest.TransID = Convert.ToInt64(transaction.TransactionID);
                cardRequest.ExtentionData = string.Empty;

                NLogLogger.Info(new string[]
                {
                    "IntecomV2Card", transaction.TransactionID.ToString(), "IntecomV2CardRequest",
                    serializer.Serialize(cardRequest)
                });

                CardResult cardResult = ProcessCard(cardRequest);
                NLogLogger.Info(new string[]
                {
                    "IntecomV2Card", transaction.TransactionID.ToString(), "IntecomV2CardResponse",
                    serializer.Serialize(cardResult)
                });

                step = 4;
                // Nếu thành công
                if (cardResult.Status == 1)
                {

                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt64(IntecomV2CardLib.Decrypt(cardResult.DataInfo, PartnerKey));
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else if (cardResult.Status == 0)
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionReview);
                    _CardAPILog.Status = _APIResponse.ResponseCode;
                }
                else if (cardResult.Status == -1)
                {
                    switch (cardResult.ResponseCode)
                    {
                        case -1:
                            //Thẻ đã sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -2:
                            //Thẻ đã được sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -3:
                            //Thẻ hết hạn sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardHasExpired);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -4:
                            //Thẻ chưa kích hoạt
                            _APIResponse = new APIResponse((int)ResponseCode.CardNotActivated);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -5:
                            //Mã đối tác không hợp lệ
                            _APIResponse = new APIResponse((int)ResponseCode.ProviderNotFound);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -6:
                            //Mã thẻ và số Serial không khớp
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -8:
                            //Cảnh báo số lần giao dịch lỗi của một tài khoản
                            _APIResponse = new APIResponse((int)ResponseCode.AccountLocked);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -9:
                            //Thẻ thử quá số lần cho phép
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionLimit);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -10:
                            //CardSerial không hợp lệ
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -11:
                            //Partner bị khóa
                            _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -12:
                            //Thẻ không tồn tại
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -13:
                            //Chữ ký không hợp lệ
                            _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -14:
                            //Mã dịch vụ không tồn tại
                            _APIResponse = new APIResponse((int)ResponseCode.ServiceNotExists);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -15:
                            //Dữ liệu truyền lên không hợp lệ
                            _APIResponse = new APIResponse((int)ResponseCode.ParameterInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -16:
                            //Mã giao dịch không hợp lệ
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -17:
                            //giá trị Amount truyền lên không hợp lệ
                            _APIResponse = new APIResponse((int)ResponseCode.CardAmountInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -18:
                            //Sai do phương pháp mã hóa, key mã hóa hoặc truyền sai Partner ID
                            _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -19:
                            //Sai PartnerServiceCode
                            _APIResponse = new APIResponse((int)ResponseCode.PartnerNotExistsNotActive);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -90:
                            //Sai tên hàm
                            _APIResponse = new APIResponse((int)ResponseCode.ServiceNotExists);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -98:
                        case -99:
                            //Giao dịch thất bại do Lỗi hệ thống
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -999:
                            //Hệ thống Telco tạm ngừng:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemMaintain);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -100:
                            //Giao dịch nghi vấn(xác minh kết quả qua kênh đối soát)
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionReview);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
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
                        NLogLogger.Info(new string[]
                        {
                            "IntecomV2Card", transaction.TransactionID.ToString(), "Error", "Step1",
                            ex.Message.Replace("\n", " ")
                        });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[]
                        {
                            "IntecomV2Card", transaction.TransactionID.ToString(), "Error", "Step2",
                            ex.Message.Replace("\n", " ")
                        });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[]
                        {
                            "IntecomV2Card", transaction.TransactionID.ToString(), "Error", "UseCard",
                            ex.Message.Replace("\n", " ")
                        });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[]
                        {
                            "IntecomV2Card", transaction.TransactionID.ToString(), "Error", "Step4",
                            ex.Message.Replace("\n", " ")
                        });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[]
                        {
                            "IntecomV2Card", transaction.TransactionID.ToString(), "Error", "Step4",
                            ex.Message.Replace("\n", " ")
                        });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }
            }

            if (_CardAPILog.TransactionID > 0)
            {
                _CardAPILog.Update();
            }
            return _APIResponse;
        }

        public APIResponse ReCheck(string transactionId)
        {
            throw new NotImplementedException();
        }

        private CardResult ProcessCard(CardRequest cardRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[] { "IntecomV2Card", "IntecomV2CardRequest", serializer.Serialize(cardRequest) });
            string responseData = PostData(ServiceUrl, serializer.Serialize(cardRequest));
            NLogLogger.Info(new string[] { "IntecomV2Card", "IntecomV2CardResponse", responseData });

            return serializer.Deserialize<CardResult>(responseData);
        }

        private string PostData(string uri, string postData)
        {

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST"; //GET
                //request.Accept = "JSON";
                //request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.7) Gecko/20070914 Firefox/2.0.0.7";
                request.Timeout = 30000;
                //System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = postDatabytes.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(postDatabytes, 0, postDatabytes.Length);

                var webResponse = request.GetResponse();
                dataStream = webResponse.GetResponseStream();
                if (dataStream == null)
                {
                    webResponse.Close();
                    return "{\"ResponseCode\":-5002,\"Status\":-1,\"Description\":\"Ngắt kết nối đến nhà cung cấp.Timeout exception\",\"DataInfo\":\"\"}";
                }

                var sr = new StreamReader(dataStream);
                var response = sr.ReadToEnd().Trim();

                sr.Close();
                dataStream.Close();
                webResponse.Close();

                return response;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    //Handle timeout exception
                    return "{\"ResponseCode\":-5002,\"Status\":-1,\"Description\":\"Ngắt kết nối đến nhà cung cấp.Timeout exception\",\"DataInfo\":\"\"}";
                }

                throw;

            }
        }

    }

}
