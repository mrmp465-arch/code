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

namespace Libs.CardTelco.C2C.KHD
{
    public class C2CCard7 : ICardTelcoHandler
    {

        // Cấu hình kết nối Production 
        protected string webserviceUrl = "http://itelservice.vn:8888/ITelService/services/CardToCash?wsdl";
        protected string PartnerCode = "tranducchinh";
        protected string Password = "tranducchinh";
        protected string SecretKey = "xoE1HlfY6Zj9tnDmqlW6ozdfsRR5DrRB5UteNjBhvcV1565ssPWmmxhaghs6V2XN";
        protected string KeyPath = "\\private.pem";

        protected string providerName = "c2c7";
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
                _CardAPILog.RequestNo = request.RefCode;
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
                        telcoCode = "MB";
                        break;
                    case "vnp":
                        telcoCode = "VP";
                        break;
                    case "viettel":
                        telcoCode = "VT";
                        break;
                    //case "vcoin":
                    //    telcoCode = "VC";
                    //    break;
                    //case "gate":
                    //    telcoCode = "GT";
                    //    break;
                    //case "bit":
                    //    telcoCode = "BT";
                    //    break;
                    //case "zing":
                    //    telcoCode = "ZX";
                    //    break;
                    
                }

                C2CCardLib.RequestData rq = new C2CCardLib.RequestData();
                rq.telcoCode = telcoCode;
                rq.serial = CryptoUtils.Encrypt(_CardAPILog.CardSerial, SecretKey);
                rq.pinCode = CryptoUtils.Encrypt(_CardAPILog.CardCode, SecretKey);
                rq.partnerTransId = _CardAPILog.TransactionID.ToString();
                rq.partnerCode = PartnerCode;
                rq.clientDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                var originalData = string.Format("{0}{1}{2}{3}{4}{5}", rq.partnerTransId, rq.partnerCode, rq.telcoCode, rq.pinCode, rq.serial, rq.clientDateTime);
                string pathKey = System.Web.HttpContext.Current.Server.MapPath("~/Shakey/C2C") + KeyPath;
                String enSignature = CryptoRSA.GetSignature(originalData, pathKey);
                rq.sign = enSignature;

                // Bước 4: gọi hàm sang Gate
                step = 4;
                var jsonRequest = serializer.Serialize(rq);
                NLogLogger.Info(new string[] { "C2C", transaction.TransactionID.ToString(), "C2CRequest", serializer.Serialize(rq) });
                CardToCashService client = null;
                string c2cResponse = string.Empty;
                try
                {
                    c2cResponse = (client = new CardToCashService(this.webserviceUrl)).exchangeCard(jsonRequest);
                }
                catch (Exception ex)
                {
                    NLogLogger.Info(new string[] { "C2C", transaction.TransactionID.ToString(), "Error", "UseCard", "C2CRequest", ex.Message.Replace("\n", " ") });
                    if (client != null)
                    {
                        client.Abort();
                    }
                    c2cResponse = "{\"resCode\"=\"-5002\"}";
                }

                NLogLogger.Info(new string[] { "C2C", transaction.TransactionID.ToString(), "C2CResponse", c2cResponse });

                // Bước 5: giải mã kết quả trả về
                step = 5;
                //Không có

                // Bước 6: phân tích kết quả
                step = 6;
                C2CCardLib.ResponseData response = C2CCardLib.GateResponse(c2cResponse);

                if (response.resCode.Equals("00"))
                {
                    _APIResponse = new APIResponse((int)API.ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt32(response.cardValue);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = " Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(C2CCardLib.ConvertResponCode(response.resCode));
                    _CardAPILog.Description = c2cResponse;
                    _CardAPILog.Status = _APIResponse.ResponseCode;

                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "C2C", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)API.ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "C2C", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)API.ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "C2C", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Encrypts.Encrypt " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.SystemError);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "C2C", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.TransactionSuspicious);
                        break;
                    case 5:
                        NLogLogger.Info(new string[] { "C2C", transaction.TransactionID.ToString(), "Error", "UseCard", "Step5", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step5 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.TransactionFailed);
                        break;
                    case 6:
                        NLogLogger.Info(new string[] { "C2C", transaction.TransactionID.ToString(), "Error", "UseCard", "Step6", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step6 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.TransactionFailed);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "C2C", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
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



