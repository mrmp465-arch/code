using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;

namespace Libs.CardTelco
{
    public class BlueSeaCard
    {
        protected string webserviceUrl = "http://card.jamo.vn/jamo_cardapi.asmx"; // Replace 01/04/2015
        protected string merchant_code = "VGG20103013";
        protected string merchant_key = "VGG20!0#@13";
        protected int maxError = 5;
        protected string providerBlueSea = "bluesea";
        protected JavaScriptSerializer serializer = new JavaScriptSerializer();

        public BlueSeaCard()
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
                _CardAPILog.Provider = providerBlueSea;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    return new APIResponse(_CardAPILog.ReturnValue); ;
                }

                // Bước: gọi hàm sang  BlueSea
                step = 3;
                jamo_cardapi _jamo_cardapi = new jamo_cardapi(webserviceUrl);
                string cardCode = Encrypts.Encrypt(merchant_key, _CardAPILog.CardCode);
                string card_serial = Encrypts.Encrypt(merchant_key, _CardAPILog.CardSerial);

                NLogLogger.Info(new string[] { "BlueSeaCard", transaction.TransactionID.ToString(), "BlueSeaRequest", _CardAPILog.AccountName, card_serial, cardCode, _CardAPILog.CardType, merchant_code });
                string blueSeaResponse = _jamo_cardapi.receiveCharge(_CardAPILog.AccountName, card_serial, cardCode, _CardAPILog.CardType, merchant_code);
                NLogLogger.Info(new string[] { "BlueSeaCard", transaction.TransactionID.ToString(), "BlueSeaResponse", blueSeaResponse });

                // Bước 4: giải mã kết quả trả về
                step = 4;
                if (blueSeaResponse.Substring(0, 3) == "11$")
                {
                    // Nếu tham số không hợp lệ thì nhận được: 11$tham so khong hop le
                    _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                    _CardAPILog.Status = _APIResponse.ResponseCode;
                    _CardAPILog.Description = blueSeaResponse;
                }
                else
                {
                    // Giải mã kết quả
                    blueSeaResponse = Encrypts.Decrypt(merchant_key, blueSeaResponse);
                    NLogLogger.Info(new string[] { "BlueSeaCard", transaction.TransactionID.ToString(), "BlueSeaResponse", blueSeaResponse });

                    switch (blueSeaResponse.Substring(0, 2))
                    {
                        case "00":
                            // 00$nickname,amount
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                            _CardAPILog.Amount = Convert.ToInt64(blueSeaResponse.Substring(3).Split(',')[1]);
                            _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                            _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                            _CardAPILog.Status = 1;
                            break;
                        case "01":
                            // 01$he thong dang trong thoi gian bao tri
                            _APIResponse = new APIResponse((int)ResponseCode.PaymentConnectionFailed);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "02":
                            //02$Ma nap tien khong hop le
                            _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "03":
                            //03$the nay da duoc su dung
                            _APIResponse = new APIResponse((int)ResponseCode.CardUsed);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "04":
                            // 04$dia chi ip khong duoc xac nhan
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "05":
                            // 05$Account cua ban dang bi khoa
                            _APIResponse = new APIResponse((int)ResponseCode.AccountLocked);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "06":
                            // 06$Co loi xay ra trong qua trinh xu ly.
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "07":
                            // 07$He thong dang ban. Xin vui long thu lai sau
                            _APIResponse = new APIResponse((int)ResponseCode.PaymentConnectionFailed);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "08":
                            //08$The khong ton tai hoac chua duoc kich hoat
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "09":
                            // 09$Tai khoan nap the da bi khoa
                            _APIResponse = new APIResponse((int)ResponseCode.AccountLocked);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "10":
                            // 10$Tham so khong hop le
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "11":
                            //11$ Ma the va so serial khong hop le
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        default:
                            //11$ Ma the va so serial khong hop le
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _CardAPILog.Description = blueSeaResponse;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "BlueSeaCard", transaction.TransactionID.ToString(), "Error", "Deserialize<UseCardRequest>", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "BlueSeaCard", transaction.TransactionID.ToString(), "Error", "_CardAPILog.Add", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "BlueSeaCard", transaction.TransactionID.ToString(), "Error", "jamo_cardapi.receiveCharge", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - jamo_cardapi.receiveCharge " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "BlueSeaCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - jamo_cardapi.receiveCharge " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "BlueSeaCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
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
    }
}
