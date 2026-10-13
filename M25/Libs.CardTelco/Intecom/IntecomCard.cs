using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;
using Libs.Utils;
using System.Web.Script.Serialization;

namespace Libs.CardTelco.Intecom
{
    public class IntecomCard
    {
        // Sandbox
        //protected string webserviceUrl = "http://sandbox2.vtcebank.vn/WSCardTelco/card.asmx";
        //int PartnerID = 920130506;
        //string PartnerKey = "920130506!@#123";

        // Intecom
        protected string webserviceUrl = "http://api.vtcebank.vn:8888/VMSCardAPI/card.asmx";
        int PartnerID = 665046;
        string PartnerKey = "665046@VGG";

        protected string providerIntecom = "intecom";
        string requestXML = "<?xml version=\"1.0\" encoding=\"utf-16\"?> <CardRequest>  <Function>$FunctionName$</Function>  <CardID>$CardID$</CardID> <CardCode>$CardCode$</CardCode> <Description>$Description$</Description> </CardRequest>";
        protected JavaScriptSerializer serializer = new JavaScriptSerializer();

        public IntecomCard()
        {

        }

        public APIResponse UseCard(APITransaction transaction)
        {
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

            int step = 0;
            try
            {
                // Bước 1: Phân tích yêu cầu thành đối tượng
                step = 1;
                UseCardRequest request = new UseCardRequest();
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);

                // Bước 2: Ghi log giao dịch
                step = 2;

                _CardAPILog.TransactionID = transaction.TransactionID;
                _CardAPILog.PartnerID = transaction.PartnerID;
                _CardAPILog.AccountName = request.AccountName;
                _CardAPILog.AccountID = 0;
                _CardAPILog.CardSerial = request.CardSerial;
                _CardAPILog.CardCode = request.CardCode;
                _CardAPILog.CardType = request.CardType;
                _CardAPILog.Amount = 0;
                _CardAPILog.Provider = providerIntecom;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    return new APIResponse(_CardAPILog.ReturnValue); ;
                }

                // Bước 3: mã hóa dữ liệu
                step = 3;
                string telcoCode = "";
                switch (_CardAPILog.CardType.ToLower())
                {
                    case "vms":
                        telcoCode = "VMS";
                        break;
                    case "vnp":
                        telcoCode = "GPC";
                        break;
                    case "viettel":
                        telcoCode = "VTEL";
                        break;
                }
                string description = telcoCode + "|" + transaction.TransactionID.ToString() + "|" + _CardAPILog.AccountName;

                card _card = new card(webserviceUrl);
                string requestData = requestXML.Replace("$FunctionName$", "UseCard").Replace("$CardID$", _CardAPILog.CardSerial).Replace("$CardCode$", _CardAPILog.CardCode).Replace("$Description$", description);

                NLogLogger.Info(new string[] { "IntecomCard", transaction.TransactionID.ToString(), "IntecomCardRequest", PartnerID.ToString(), requestData });
                requestData = Encrypts.Encrypt(PartnerKey, requestData);

                // Bước 4: gọi hàm sang Intecom
                step = 4;

                string vcoinResponse = _card.Request(PartnerID, requestData);
                NLogLogger.Info(new string[] { "IntecomCard", transaction.TransactionID.ToString(), "IntecomCardResponse", vcoinResponse });

                // Bước 5: giải mã kết quả trả về
                step = 5;

                vcoinResponse = Encrypts.Decrypt(PartnerKey, vcoinResponse);
                NLogLogger.Info(new string[] { "IntecomCard", transaction.TransactionID.ToString(), "IntecomCardResponse", vcoinResponse });

                // Bước 6: phân tích kết quả
                step = 6;
                VcoinResponse response = new VcoinResponse(vcoinResponse);

                if (response.ResponseStatus > 0)
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = response.ResponseStatus;
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = " Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(ConvertResponCode(response.ResponseStatus));
                    _CardAPILog.Description = vcoinResponse;
                    _CardAPILog.Status = _APIResponse.ResponseCode;

                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "IntecomCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "IntecomCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "IntecomCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Encrypts.Encrypt " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "IntecomCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 5:
                        NLogLogger.Info(new string[] { "IntecomCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step5", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step5 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 6:
                        NLogLogger.Info(new string[] { "IntecomCard", transaction.TransactionID.ToString(), "Error", "UseCard", "Step6", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step6 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "IntecomCard", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error " + ex.Message.Replace("\n", " ");
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

        private int ConvertResponCode(int responseStatus)
        {
            switch (responseStatus)
            {
                case -1:
                    //-1: Thẻ đã sử dụng
                    return (int)ResponseCode.CardUsed;
                case -2:
                    //-2: Thẻ đã bị khóa
                    return (int)ResponseCode.CardIsLocked;
                case -3:
                    //-3: Thẻ hết hạn sử dụng
                    return (int)ResponseCode.CardHasExpired;
                case -4:
                    //-4: Thẻ chưa kích hoạt
                    return (int)ResponseCode.CardNotActivated;
                case -5:
                    //-5: TransID không hợp lệ
                    return (int)ResponseCode.SystemError;
                case -6:
                    //-6: Mã thẻ và số Serial không khớp
                    return (int)ResponseCode.CardSerialInvalid;
                case -8:
                    //-8: Cảnh báo số lần giao dịch lỗi của một tài khoản
                    return (int)ResponseCode.TransactionFailed;
                case -9:
                    //-9:  Thẻ  thử quá  số  lần cho  phép
                    return (int)ResponseCode.TransactionFailed;
                case -10:
                    //-10: CardID không hợp lệ
                    return (int)ResponseCode.CardSerialInvalid;
                case -11:
                    //-11: CardCode không hợp lệ
                    return (int)ResponseCode.CardCodeInvalid;
                case -12:
                    //-12: Thẻ không tồn tại
                    return (int)ResponseCode.CardSerialInvalid;
                case -13:
                    //-13:  Sai cấu trúc Description
                    return (int)ResponseCode.SystemError;
                case -14:
                    //-14:  Mã dịch  vụ  không  tồn  tại
                    return (int)ResponseCode.SystemError;
                case -15:
                    //-15:  Thiếu  thông  tin khách  hàng
                    return (int)ResponseCode.SystemError;
                case -16:
                    //-16:  Mã  giao dịch không hợp  lệ
                    return (int)ResponseCode.SystemError;
                case -90:
                    //-90:  Sai   tên hàm
                    return (int)ResponseCode.SystemError;
                default:
                    return (int)ResponseCode.SystemError;
            }
        }
    }
}
