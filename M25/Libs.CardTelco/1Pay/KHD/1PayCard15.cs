using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.Web.Hosting;

namespace Libs.CardTelco._1Pay.KHD
{
    public class _1PayCard15 : ICardTelcoHandler
    {

        // Cấu hình kết nối Production 
        protected string webserviceUrl = "http://103.255.238.41:9090/VinaCardService.svc?wsdl";
        protected string PartnerCode = "XBOM";
        protected string Password = "xbom@sjtuw";
        protected string SecretKey = "BF15F86B3761DC5DACDCDBB8";

        protected string providerName = "1pay15";
        protected JavaScriptSerializer serializer = new JavaScriptSerializer();

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
                _CardAPILog.PartnerCode = transaction.PartnerCode;
                _CardAPILog.AccountName = request.AccountName;
                _CardAPILog.AccountID = 0;
                _CardAPILog.CardSerial = request.CardSerial;
                _CardAPILog.CardCode = request.CardCode;
                _CardAPILog.CardType = request.CardType;
                _CardAPILog.Amount = 0;
                _CardAPILog.Provider = providerName;
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
                string cardType = "";
                switch (_CardAPILog.CardType.ToLower())
                {

                    case "vms":
                        cardType = "MOBI";
                        break;
                    case "vnp":
                        cardType = "VINA";
                        break;
                    case "viettel":
                        cardType = "VT";
                        break;
                }

                _1PayCardLib.RequestData rq = new _1PayCardLib.RequestData();
                rq.telco = cardType;
                rq.serial = _CardAPILog.CardSerial;
                rq.cardCode = CryptoUtils.EncryptTripleDES(_CardAPILog.CardCode, SecretKey);
                rq.tranref = _CardAPILog.TransactionID.ToString();
                rq.partnerCode = PartnerCode;
                rq.password = CryptoUtils.HashMD5(Password).ToUpper();
                rq.importCode = string.Empty;
                var originalData = string.Format("{0}{1}{2}{3}{4}{5}", rq.telco, rq.cardCode, rq.tranref, rq.partnerCode, rq.password, SecretKey);
                rq.signature = CryptoUtils.HashMD5(originalData);

                // Bước 4: gọi hàm sang Gate
                step = 4;
                NLogLogger.Info(new string[] { "1Pay", transaction.TransactionID.ToString(), "1PayRequest", serializer.Serialize(rq) });
                VinaCardServiceClient client = null;
                string _1PayResponse = string.Empty;
                try
                {
                    _1PayResponse = (client = new VinaCardServiceClient()).VerifyCard(
                        rq.telco,
                        rq.serial,
                        rq.cardCode,
                        rq.tranref,
                        rq.partnerCode,
                        rq.password,
                        rq.signature,
                        rq.importCode
                    );
                    client.Close();
                }
                catch (Exception ex)
                {
                    NLogLogger.Info(new string[] { "1Pay", transaction.TransactionID.ToString(), "Error", "UseCard", "1PayRequest", ex.Message.Replace("\n", " ") });
                    if (client != null)
                    {
                        client.Abort();
                    }
                    _1PayResponse = "-5001|Ngắt kết nối 1Pay";
                }

                NLogLogger.Info(new string[] { "1Pay", transaction.TransactionID.ToString(), "1PayResponse", _1PayResponse });

                // Bước 5: giải mã kết quả trả về
                step = 5;
                //Không có

                // Bước 6: phân tích kết quả
                step = 6;
                _1PayCardLib.ResponseData response = _1PayCardLib.GateResponse(_1PayResponse);

                if (response.Code.Equals("01"))
                {
                    _APIResponse = new APIResponse((int)API.ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt32(response.Amount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = " Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(_1PayCardLib.ConvertResponCode(response.Code));
                    _CardAPILog.Description = _1PayResponse;
                    _CardAPILog.Status = _APIResponse.ResponseCode;

                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "1Pay", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)API.ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "1Pay", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)API.ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "1Pay", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Encrypts.Encrypt " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.SystemError);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "1Pay", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.TransactionSuspicious);
                        break;
                    case 5:
                        NLogLogger.Info(new string[] { "1Pay", transaction.TransactionID.ToString(), "Error", "UseCard", "Step5", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step5 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.TransactionFailed);
                        break;
                    case 6:
                        NLogLogger.Info(new string[] { "1Pay", transaction.TransactionID.ToString(), "Error", "UseCard", "Step6", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step6 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.TransactionFailed);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "1Pay", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.SystemError);
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
    }
}



