using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;


namespace Libs.CardTelco.Appota
{
    public class AppotaCard
    {
        protected string webserviceUrl = "https://api.appota.com/payment/inapp_card?api_key=7dad5913fe29f730acbbaa62c147c7db054360bfc&lang=vi";
        protected string providerBlueSea = "appota";
        protected JavaScriptSerializer serializer = new JavaScriptSerializer();

        public AppotaCard()
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

                // Bước 3: gọi hàm sang Appota
                step = 3;
                string appotaResponse = AppotaUseCard(request.CardCode, request.CardSerial, request.CardType);

                // Bước 4: giải mã kết quả trả về
                step = 4;
                if (appotaResponse == null)
                {
                    _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                    _CardAPILog.Status = _APIResponse.ResponseCode;
                    _CardAPILog.Description = appotaResponse;
                }
                else
                {
                    // Giải mã kết quả
                    AppotaAPIData _AppotaAPIData = new AppotaAPIData();
                    _AppotaAPIData = serializer.Deserialize<AppotaAPIData>(appotaResponse);

                    switch (_AppotaAPIData.error_code)
                    {
                        case 0:
                            //0: Giao dịch thành công.
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                            _CardAPILog.Amount = _AppotaAPIData.data.amount;
                            _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                            _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                            _CardAPILog.Status = 1;
                            break;
                        case 39:
                            //39: Sai mã thẻ
                            _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                            _CardAPILog.Description = _AppotaAPIData.message;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case 1:     //1: Dữ liệu không chính xác.
                        case 3:     //3: Ứng dụng bị khoá hoặc chưa được phân phối chính thức.
                        case 10:    //10: Dịch vụ bị giới hạn theo vùng địa lý.
                        case 40:    //40: Không thể kết nối đến hệ thống nhà cung cấp thẻ.
                        case 91:    //91: Hệ thống không thể truy vấn vào thời điểm này.
                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Description = _AppotaAPIData.message;
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
                        NLogLogger.Info(new string[] { "AppotaCard", transaction.TransactionID.ToString(), "Error", "Deserialize<UseCardRequest>", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "AppotaCard", transaction.TransactionID.ToString(), "Error", "_CardAPILog.Add", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "AppotaCard", transaction.TransactionID.ToString(), "Error", "AppotaUseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - AppotaUseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "AppotaCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "AppotaCard", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
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

        private string AppotaUseCard(string card_code, string card_serial, string vendor)
        {
            // Thay đổi loại thẻ cho phù hợp
            switch (vendor)
            {
                case "vms":
                    vendor = "mobifone";
                    break;
                case "vnp":
                    vendor = "vinaphone";
                    break;
                case "viettel":
                case "fpt":
                case "mega":
                default:
                    break;
            }

            string parameters = "card_code={0}&card_serial={1}&vendor={2}&direct=1";
            parameters = string.Format(parameters, card_code, card_serial, vendor);

            NLogLogger.Info(new string[] { "AppotaCard", "Requesst", parameters });
            string s = HttpPost(webserviceUrl, parameters);
            NLogLogger.Info(new string[] { "AppotaCard", "Response", s });

            return s;
        }

        public string HttpPost(string url, string parameters)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(parameters);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null) return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

    }

    public class AppotaAPIData
    {
        public bool status { get; set; }
        public int error_code { get; set; }
        public CardData data { get; set; }
        public string message { get; set; }
        public string data_signature { get; set; }
    }

    public class CardData
    {
        public string transaction_id { get; set; }
        public string type { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string country_code { get; set; }
        public string target { get; set; }
        public string state { get; set; }
        public string time { get; set; }
        public int sandbox { get; set; }
    }
}
