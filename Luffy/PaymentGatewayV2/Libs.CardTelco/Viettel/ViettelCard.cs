using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;

namespace Libs.CardTelco
{
    public class ViettelCard
    {
        protected string webserviceUrl = "https://125.235.33.166:8443/ScratchCardAPI/ScratchCardAPI?wsdl";
        protected string parnerID = "1000000028";
        protected string passphase = "ek8MLnt4BO";
        protected string providerName = "viettel";
        protected string provider = "VGG";
        protected string serviceName = "VGG";
        protected int timeOut = 300000;

        protected JavaScriptSerializer serializer = new JavaScriptSerializer();

        public ViettelCard()
        {

        }

        public APIResponse UseCard(APITransaction transaction)
        {
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
                _CardAPILog.AccountName = request.AccountName;
                _CardAPILog.AccountID = 0;
                _CardAPILog.CardSerial = request.CardSerial;
                _CardAPILog.CardCode = request.CardCode;
                _CardAPILog.CardType = request.CardType;
                _CardAPILog.Amount = 0;
                _CardAPILog.Provider = providerName;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.RequestNo = DateTime.Now.ToString("yyMMddHHmmss") + parnerID + new Random().Next(100, 999).ToString();
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    return new APIResponse(_CardAPILog.ReturnValue); ;
                }

                // Bước: gọi hàm sang Viettel
                step = 3;
                ScratchCardAPIService _ScratchCardAPIService = new ScratchCardAPIService(webserviceUrl);
                _ScratchCardAPIService.Timeout = timeOut;
                System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
                NLogLogger.Info(new string[] { "ViettelCard", transaction.TransactionID.ToString(), "UseCardRequest", parnerID, passphase, _CardAPILog.CardSerial, _CardAPILog.CardCode, _CardAPILog.RequestNo, provider, serviceName });
                pnResponse viettelResponse = _ScratchCardAPIService.topupCard(parnerID, passphase, _CardAPILog.CardSerial, _CardAPILog.CardCode, _CardAPILog.RequestNo, provider, serviceName);
                NLogLogger.Info(new string[] { "ViettelCard", transaction.TransactionID.ToString(), "UseCardResponse", serializer.Serialize(viettelResponse) });

                switch (viettelResponse.errorCode)
                {
                    case "00":
                        //00	Giao dịch thành công
                        _CardAPILog.Amount = Convert.ToInt64(viettelResponse.amount);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        if (_CardAPILog.Amount < 0 || _CardAPILog.Amount > 1000000)
                        {
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                        }
                        else
                        {
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                            _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                            _CardAPILog.Status = 1;
                        }
                        break;
                    case "01":
                        //01	Thẻ đã được sử dụng
                        _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "02":
                        //02	Thẻ không tồn tại
                        _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "03":
                        //03	Thông tin tham số đầu vào không đúng định dạng (null, length)
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "04":
                        //04	Mã thẻ và số serial thẻ không khớp
                        _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "05":
                        //05	Đầu thẻ không được hỗ trợ
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "06":
                        //06	Mệnh giá thẻ chưa được cấu hình cho đối tác
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "07":
                        //07	Thẻ đã quá hạn sử dụng
                        _APIResponse = new APIResponse((int)ResponseCode.CardHasExpired);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "08":
                        //08	Trạng thái thẻ không hợp lệ
                        _APIResponse = new APIResponse((int)ResponseCode.CardNotActivated);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "50":
                        //50	Sai parnerId/passphase
                        _APIResponse = new APIResponse((int)ResponseCode.PaymentConnectionFailed);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "51":
                        //51	Giao dịch không được phép (Ip không được phép kết nối đến)
                        _APIResponse = new APIResponse((int)ResponseCode.PaymentConnectionFailed);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "52":
                        //52	Định dạng transId không đúng định dạng hoặc trùng transId
                        _APIResponse = new APIResponse((int)ResponseCode.PaymentConnectionFailed);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "53":
                        //53	Không tìm thấy giao dịch trước đó (trong bản tin truy vấn)
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionNotExists);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "54":
                        //54	Giao dịch đang được thực hiện
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "55":
                        //55	Vượt quá số giao dịch cho phép đồng thời
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "56":
                        //56	Sai parnerId/passphase quá n lần cho phép, khóa đối tác.
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "57":
                        //57	Đối tác đã bị khóa
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "58":
                        //58	Timeout từ hệ thống core
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "59":
                        //59	Lỗi database hệ thống
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "97":
                        //97	Lỗi database core
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "98":
                        //98	Mất kết nối với hệ thông core
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    case "99":
                        //99	Lỗi không nằm trong danh sách mô tả
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                    default:
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        _CardAPILog.Description = serializer.Serialize(viettelResponse);
                        _CardAPILog.Status = _APIResponse.ResponseCode;
                        break;
                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "ViettelCard", transaction.TransactionID.ToString(), "Error", "step1 - Deserialize<UseCardRequest>", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "ViettelCard", transaction.TransactionID.ToString(), "Error", "step2 - _CardAPILog.Add", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "ViettelCard", transaction.TransactionID.ToString(), "Error", "step3 - topupCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - topupCard: " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "ViettelCard", transaction.TransactionID.ToString(), "Error", "step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - step3: " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                }
            }

            if (_CardAPILog.TransactionID > 0)
            {
                _CardAPILog.Update();
            }
            return _APIResponse;
        }

        public APIResponse CheckTransaction(APITransaction transaction)
        {
            APIResponse _APIResponse = new APIResponse();

            UseCardRequest request = new UseCardRequest();
            request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);

            // Kiểm tra trên hệ thống của VGG
            CardAPILog _CardAPILog = new CardAPILog();
            _CardAPILog = _CardAPILog.Check(transaction.PartnerID, request.CardSerial, request.CardType);

            CheckCardResponse checkCardResponse = new CheckCardResponse();

            checkCardResponse.CardSerial = request.CardSerial;
            checkCardResponse.CardType = request.CardType;

            // Nếu không tồn tại giao dịch
            if (_CardAPILog == null)
            {
                checkCardResponse.Status = (int)ResponseCode.TransactionNotExists;
            }
            else if (_CardAPILog.Status != (int)ResponseCode.TransactionSuspicious)
            {
                // Nếu không phải là giao dịch nghi vấn
                checkCardResponse.Status = _CardAPILog.Status;
                checkCardResponse.TransactionID = _CardAPILog.TransactionID;
                if (_CardAPILog.Status == 1)
                {
                    checkCardResponse.Amount = _CardAPILog.Amount;
                }
            }
            else
            {
                // Kết nối đến Viettel
                ScratchCardAPIService _ScratchCardAPIService = new ScratchCardAPIService(webserviceUrl);
                _ScratchCardAPIService.Timeout = 5 * 60 * 1000; // 5 phút
                string transID = DateTime.Now.ToString("yyMMddHHmmss") + parnerID + new Random().Next(100, 999).ToString();
                System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();

                //NLogLogger.Info(new string[] { "ViettelCard", transaction.TransactionID.ToString(), "CheckCardRequest", parnerID, passphase, _CardAPILog.CardSerial, _CardAPILog.CardCode, _CardAPILog.RequestNo, provider, serviceName });
                pnResponse viettelResponse = _ScratchCardAPIService.queryResultTransaction(parnerID, passphase, _CardAPILog.RequestNo, transID);
                NLogLogger.Info(new string[] { "ViettelCard", transaction.TransactionID.ToString(), "CheckCardResponse", serializer.Serialize(viettelResponse) });

                if (viettelResponse.errorCode == "00")
                {
                }
                else
                {

                }
            }

            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
            _APIResponse.ResponseContent = serializer.Serialize(checkCardResponse);
            return _APIResponse;
        }
    }
}
