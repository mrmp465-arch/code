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

namespace Libs.CardTelco.Gate.CHD
{
    public class GateCard7 : ICardTelcoHandler
    {

        // Cấu hình kết nối Production 
        protected string webserviceUrl = "https://ops.gate.vn:8888/Igate_WS/Route?wsdl";

        protected string merchantID = "1289";
        protected string secretKey = "63a5ebb2d51a9a4647c9c37b585c1399";
        protected string PrivateKey = "6879a6e9";
        protected string KeyPath = "\\1289.p12";

        protected JavaScriptSerializer serializer = new JavaScriptSerializer();
        protected string providerName = "gate7";


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
                    case "gate":
                        cardType = "CardInputGate";
                        break;
                    case "vms":
                        cardType = "CardInputVMS";
                        break;
                    case "vnp":
                        cardType = "CardInputVNP";
                        break;
                    case "viettel":
                        cardType = "CardInputViettel";
                        break;
                }

                GateCardLib.RequestData rq = new GateCardLib.RequestData();
                rq.MerchantID = merchantID;
                rq.Username = _CardAPILog.AccountName;
                rq.CardSerial = _CardAPILog.CardSerial;
                rq.CardPIN = _CardAPILog.CardCode;
                rq.FunctionName = "CardInput";
                rq.PartnerTransactionID = _CardAPILog.TransactionID.ToString();

                String originalData = String.Format("{0}{1}{2}{3}{4}", rq.MerchantID, rq.Username, rq.CardSerial, rq.CardPIN, secretKey);
                NLogLogger.Info(new string[] { cardType, transaction.TransactionID.ToString(), "GateRequest", merchantID.ToString(), originalData });

                // Bước 4: gọi hàm sang Gate
                step = 4;
                DataSign objSign = new DataSign();
                string pathKey = System.Web.HttpContext.Current.Server.MapPath("~/Shakey/Gate") + KeyPath;
                String enSignature = objSign.SignData(originalData, pathKey, PrivateKey);
                rq.Signature = enSignature;

                IgateRouteService sb = new IgateRouteService(webserviceUrl);
                sb.Timeout = 30000;
                String xmlRequestData = GateCardLib.XmlSerialize(rq);
                string gateResponse = sb.ProcessRequest(cardType, xmlRequestData); //Output : Trả về giá trị của chuỗi với định dạng (ErrorCode|Description|TransactionID|PartnerTransactionID|CardAmount) 
                NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "GateResponse", gateResponse });

                // Bước 5: giải mã kết quả trả về
                step = 5;
                //Không có

                // Bước 6: phân tích kết quả
                step = 6;
                GateCardLib.ResponseData response = GateCardLib.GateResponse(gateResponse);

                if (response.ErrorCode.Equals("00"))
                {
                    _APIResponse = new APIResponse((int)API.ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt32(response.CardAmount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = " Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(GateCardLib.ConvertResponCode(response.ErrorCode));
                    _CardAPILog.Description = gateResponse;
                    _CardAPILog.Status = _APIResponse.ResponseCode;

                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)API.ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)API.ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Encrypts.Encrypt " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.SystemError);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.TransactionSuspicious);
                        break;
                    case 5:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step5", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step5 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.TransactionFailed);
                        break;
                    case 6:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step6", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step6 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)API.ResponseCode.TransactionFailed);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
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
