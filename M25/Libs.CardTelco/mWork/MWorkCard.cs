using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;
namespace Libs.CardTelco.mWork
{
    public class MWorkCard
    {
        protected string webserviceUrl = "https://api.mwork.vn/card-charging/v2";
        protected string providerCode = "mwork";
        protected JavaScriptSerializer serializer = new JavaScriptSerializer();

        // Cấu hình kết nối
        protected string app_code = "vosongtamquoc";
        //protected string refcode = "vgg";
        protected string provider = "vgg";
        protected string apikey = "7d4d4cabf4f589371457b4037360df24";

        public MWorkCard()
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
                _CardAPILog.Provider = providerCode;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    return new APIResponse(_CardAPILog.ReturnValue); ;
                }

                // Bước 3: gọi hàm sang mWork
                step = 3;
                string mWorkResponse = MWorkUseCard(request.CardCode, request.CardSerial, request.CardType, request.RefCode);

                // Bước 4: giải mã kết quả trả về
                step = 4;
                if (string.IsNullOrEmpty(mWorkResponse))
                {
                    _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                    _CardAPILog.Status = _APIResponse.ResponseCode;
                    _CardAPILog.Description = "";
                }
                else
                {
                    // Giải mã kết quả
                    mWorkCardData _mWorkCardData = new mWorkCardData();
                    _mWorkCardData = serializer.Deserialize<mWorkCardData>(mWorkResponse);

                    switch (_mWorkCardData.status)
                    {
                        case "00":
                            //00: Giao dịch thành công.
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                            _CardAPILog.Amount = _mWorkCardData.amount;
                            _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                            _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                            _CardAPILog.Status = 1;
                            break;
                        case "01":
                            //01: Thẻ không hợp lệ hoặc đã được sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardCodeInvalid);
                            _CardAPILog.Description = _mWorkCardData.description;
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case "02":
                            //02: Lỗi kết nối tới hệ thống thẻ cào của nhà mạng
                        case "03":
                            //03: Lỗi không xác định
                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Description = _mWorkCardData.description;
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
                        NLogLogger.Info(new string[] { "MWorkCard", transaction.TransactionID.ToString(), "Error", "Deserialize<UseCardRequest>", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "MWorkCard", transaction.TransactionID.ToString(), "Error", "_CardAPILog.Add", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "MWorkCard", transaction.TransactionID.ToString(), "Error", "MWorkUseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - MWorkUseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "MWorkCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "MWorkCard", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
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

        private string MWorkUseCard(string card_code, string card_serial, string vendor, string refcode)
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
                default:
                    break;
            }

            string parameters = "pin={0}&serial={1}&type={2}&app_code={3}&refcode={4}&provider={5}&apikey={6}";
            parameters = string.Format(parameters, card_code, card_serial, vendor, app_code, refcode, provider, apikey);

            NLogLogger.Info(new string[] { "MWorkCard", "Requesst", parameters });
            string s = HttpGet(webserviceUrl, parameters);
            NLogLogger.Info(new string[] { "MWorkCard", "Response", s });

            return s;
        }

        public string HttpGet(string url, string parameters)
        {
            string Out = String.Empty;
            System.Net.WebRequest req = System.Net.WebRequest.Create(url + (string.IsNullOrEmpty(parameters) ? "" : "?" + parameters));
            System.Net.WebResponse resp = req.GetResponse();
            using (System.IO.Stream stream = resp.GetResponseStream())
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                {
                    Out = sr.ReadToEnd();
                    sr.Close();
                }
            }

            return Out;
        }
    }

    public class mWorkCardData
    {
        public string status { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
    }
}
