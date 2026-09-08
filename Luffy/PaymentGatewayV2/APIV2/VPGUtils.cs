using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.CardTelco;

using Libs.TopupPartner;
using Libs.Utils;

namespace APIV2
{
    public class VPGUtils
    {
        public static string RequestV3(string partnerCode, string provider, int quantity, int amount, string accountName, string refCode, string signature)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Ghi log giao dịch
            NLogLogger.Info(new string[] { "API", "Request", partnerCode, signature });

            string result = "";
            APIResponse _APIResponse = new APIResponse();
            string ip = IPAddress.Get();
            string serviceCode = "buycard";
            string commandCode = "buycard";
            string requestContent = $"{partnerCode}{provider}{amount}{refCode}";
            Payments _payments = new Payments().GetCheckCache(serviceCode);
            if (_payments == null)
            {
                result = ResponseUtils.Response((int)ResponseCode.ServiceNotExists);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
            }

            //cahce 
            Partners _Partner = new Partners().GetCache(partnerCode);
            //Kiểm tra _Partner tồn tại hoặc Active không
            if (_Partner == null || _Partner.Status == 0)
            {
                result = ResponseUtils.Response((int)ResponseCode.PartnerNotExistsNotActive);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                return result;
            }

            //Kiểm tra IP Partner

            //cahce
            PartnerService _partnerService = new PartnerService();
            //Kiểm tra Partner co được add Service ko
            var partnerStatus = _partnerService.GetCache(_Partner.PartnerID, _payments.ServiceID);
            if (partnerStatus == null || partnerStatus.Status == 0)
            {
                result = ResponseUtils.Response((int)ResponseCode.ServiceNotExists);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                return result;
            }


            // Thêm mới giao dịch, 
            APITransaction _APITransaction = new APITransaction();
            try
            {
                var jsonContent = $"\"Provider\":\"{provider}\",\"Amount\":{amount},\"Quantity\":{quantity},\"AccountName\":\"{accountName}\",\"AppCode\":\"\",\"OrderNo\":\"{refCode}\"";
                _APITransaction.PartnerCode = partnerCode;
                _APITransaction.ServiceCode = serviceCode;
                _APITransaction.CommandCode = commandCode;

                _APITransaction.Signature = signature;
                _APITransaction.IpAddress = ip;

                _APITransaction.RequestContent = "{" + jsonContent + "}";


                _APITransaction = _APITransaction.Add();
            }
            catch (Exception ex)
            {
                // Nếu thêm mới giao dịch không thành công
                result = ResponseUtils.Response((int)ResponseCode.SystemError);
                NLogLogger.Info(new string[] { "API", "Error", "VPGService.cs", "_APITransaction.Add", ex.Message.Replace("\n", " ") });
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, "0", result });
                return result;
            }

            // Nếu giao dịch không hợp lệ
            if (_APITransaction.ReturnValue != 0)
            {

                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                return result;
            }


            string privateKey = _Partner.PrivateKey;
            string publicKey = _Partner.PublicKey;
            int signatureType = _Partner.SignatureType;


            // Kiểm tra chữ ký
            if (!PaymentUtils.CheckSignature(requestContent, signature, publicKey, signatureType))
            {
                _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
                _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, _Partner.PrivateKey, _Partner.SignatureType);

                _APITransaction.Status = _APIResponse.ResponseCode;
                _APITransaction.UpdateStatus();

                result = serializer.Serialize(_APIResponse);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result, requestContent + publicKey });
                return result;
            }

            // Tải dll
            //Payments _Payment = new Payments();
            //APIService _APIService;
            try
            {

                var handler = ServiceFactory.GetHandler(serviceCode);
                _APIResponse = handler.Request(_APITransaction);

               
               
           
             
              
                
            }
            catch (Exception ex)
            {
                _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                NLogLogger.Info(new string[] { "API", "Error", _APITransaction.TransactionID.ToString(), "VPGService.cs", "(APIService)obj", ex.Message.Replace("\n", " ") });
            }

            _APITransaction.Status = _APIResponse.ResponseCode < 0 ? _APIResponse.ResponseCode : 1;
            _APITransaction.UpdateStatus();

            // Chữ ký
            _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, privateKey, signatureType);
            result = serializer.Serialize(_APIResponse);
            NLogLogger.Info(new string[] { "API", "Response", _APITransaction.TransactionID.ToString(), result });
            return result;
        }
        public static string RequestV2(string partnerCode, string cardCode, string cardSeri, int amountUser, string cardType, string accountName, string refCode, string url, string signature)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Ghi log giao dịch
           // NLogLogger.Info(new string[] { "API", "Request", partnerCode, cardSeri,cardCode, cardType, signature });

            string result = "";
            APIResponse _APIResponse = new APIResponse();
            string ip = IPAddress.Get();
            string serviceCode = "cardtelco";
            string commandCode = "usecard";
            string requestContent = $"{partnerCode}{cardCode}{cardSeri}{refCode}";
            Payments _payments = new Payments().GetCheckCache(serviceCode);
            if (_payments == null)
            {
                result = ResponseUtils.Response((int)ResponseCode.ServiceNotExists);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
            }

            //cahce 
            Partners _Partner = new Partners().GetCache(partnerCode);
            //Kiểm tra _Partner tồn tại hoặc Active không
            if (_Partner == null || _Partner.Status == 0)
            {
                result = ResponseUtils.Response((int)ResponseCode.PartnerNotExistsNotActive);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                return result;
            }

            //Kiểm tra IP Partner

            //cahce
            PartnerService _partnerService = new PartnerService();
            //Kiểm tra Partner co được add Service ko
            var partnerStatus = _partnerService.GetCache(_Partner.PartnerID, _payments.ServiceID);
            if (partnerStatus == null || partnerStatus.Status == 0)
            {
                result = ResponseUtils.Response((int)ResponseCode.ServiceNotExists);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                return result;
            }


            // Thêm mới giao dịch, 
            APITransaction _APITransaction = new APITransaction();
            try
            {
                var jsonContent = $"\"CardSerial\":\"{cardSeri}\",\"CardCode\":\"{cardCode}\",\"CardType\":\"{cardType}\",\"AccountName\":\"{accountName}\",\"AppCode\":\"\",\"RefCode\":\"{refCode}\",\"Amount\":0,\"AmountUser\":{amountUser},\"CallbackUrl\":\"{url}\"";
                _APITransaction.PartnerCode = partnerCode;
                _APITransaction.ServiceCode = serviceCode;
                _APITransaction.CommandCode = commandCode;

                _APITransaction.Signature = signature;
                _APITransaction.IpAddress = ip;

                _APITransaction.RequestContent = "{" + jsonContent + "}";


                _APITransaction = _APITransaction.Add();
            }
            catch (Exception ex)
            {
                // Nếu thêm mới giao dịch không thành công
                result = ResponseUtils.Response((int)ResponseCode.SystemError);
                NLogLogger.Info(new string[] { "API", "Error", "VPGService.cs", "_APITransaction.Add", ex.Message.Replace("\n", " ") });
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, "0", result });
                return result;
            }

            // Nếu giao dịch không hợp lệ
            if (_APITransaction.ReturnValue != 0)
            {

                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                return result;
            }


            string privateKey = _Partner.PrivateKey;
            string publicKey = _Partner.PublicKey;
            int signatureType = _Partner.SignatureType;


            // Kiểm tra chữ ký
            if (!PaymentUtils.CheckSignature(requestContent, signature, publicKey, signatureType))
            {
                _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
                _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, _Partner.PrivateKey, _Partner.SignatureType);

                _APITransaction.Status = _APIResponse.ResponseCode;
                _APITransaction.UpdateStatus();

                result = serializer.Serialize(_APIResponse);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result, requestContent + publicKey });
                return result;
            }

            // Tải dll
            //Payments _Payment = new Payments();
            //APIService _APIService;
            try
            {

                var handler = ServiceFactory.GetHandler(serviceCode);
                _APIResponse = handler.Request(_APITransaction);

                //callback
                if (partnerCode == "im0808")
                {

                    var request = serializer.Deserialize<UseCardRequest>(_APITransaction.RequestContent);
                    if (request != null)
                    {
                        int amount = 0;
                        if (_APIResponse.ResponseCode == 1 || _APIResponse.ResponseCode == -372)
                            amount = int.Parse(_APIResponse.ResponseContent);
                        if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionSuccessful && request.CardType== "gate")
                        {
                            if (!string.IsNullOrEmpty(request.CallbackUrl))
                            {
                                var datacb = new Libs.CardTelco.PayPlusAppV2VTT.DataCallback()
                                {
                                    Amount = amount,
                                    RefCode = request.RefCode,
                                    Status = _APIResponse.ResponseCode,
                                    Signature = Libs.Utils.Encrypts.MD5(request.RefCode + _APIResponse.ResponseCode + amount + privateKey)
                                };
                               
                                Task.Run(() => CallbackJson(request.CallbackUrl, serializer.Serialize(datacb), partnerCode, 0, 2).ConfigureAwait(false));
                            }
                        }
                    }

                }
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionTimeout)
                {

                    //if (_APIResponse.ResponseContent != "ntnet")
                    //{
                    var request = serializer.Deserialize<UseCardRequest>(_APITransaction.RequestContent);
                    if (request != null)
                    {
                        var keyCache = String.Format("RedisProviderTimeout:{0}", _APIResponse.ResponseContent);
                        var dataCache = DataCaching.GetCache<string>(keyCache);
                        var timeoutCount = 0;
                        if (dataCache != null)
                            timeoutCount = int.Parse(dataCache);
                        if (timeoutCount > 5)
                        {
                            TelegramNotify.SendNotify(partnerCode + " - " + _APIResponse.ResponseContent, string.Empty, String.Format("-326 (Transaction Timeout) {0} - ", request.CardType), 0, 4);
                            timeoutCount = 0;
                        }
                        else
                        {
                            timeoutCount++;
                        }
                        DataCaching.SetCache(keyCache, timeoutCount.ToString(), 300);
                    }

                    //}
                    _APIResponse.ResponseContent = "";
                }
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionSuspicious || _APIResponse.ResponseCode == (int)ResponseCode.SystemError)
                {

                    //if (_APIResponse.ResponseContent != "ntnet")
                    //{
                    var request = serializer.Deserialize<UseCardRequest>(_APITransaction.RequestContent);
                    if (request != null)
                    {
                        var keyCache = String.Format("RedisProviderFail:{0}", _APIResponse.ResponseContent);
                        var dataCache = DataCaching.GetCache<string>(keyCache);
                        var timeoutCount = 0;
                        if (dataCache != null)
                            timeoutCount = int.Parse(dataCache);
                        if (timeoutCount > 10)
                        {
                            TelegramNotify.SendNotify(partnerCode + " - " + _APIResponse.ResponseContent, string.Empty, String.Format("-1 (Transaction Fail) {0} - ", request.CardType), 0, 4);
                            timeoutCount = 0;
                        }
                        else
                        {
                            timeoutCount++;
                        }
                        DataCaching.SetCache(keyCache, timeoutCount.ToString(), 300);
                    }

                    //}
                    _APIResponse.ResponseContent = "";
                }
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionFailed)
                {

                    //if (_APIResponse.ResponseContent != "ntnet")
                    //{
                    var request = serializer.Deserialize<UseCardRequest>(_APITransaction.RequestContent);
                    if (request != null)
                    {
                        if (request.CardType.ToLower() == "zing")
                            TelegramNotify.SendNotify(partnerCode + " - " + _APIResponse.ResponseContent, string.Empty, String.Format("-1 (Transaction Fail) {0} - {1}", request.CardType, request.CardSerial), 0, 4);
                    }

                    //}
                    _APIResponse.ResponseContent = "";
                }
                if (_APIResponse.ResponseCode == (int)ResponseCode.ProviderNotFound)
                {


                    var request = serializer.Deserialize<UseCardRequest>(_APITransaction.RequestContent);
                    if (request != null)
                        TelegramNotify.SendNotify(partnerCode, string.Empty, String.Format("-327 (Provider Not Found) {0}  ", request.CardType), 0, 4);


                }
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionFailed)
                {


                    var request = serializer.Deserialize<UseCardRequest>(_APITransaction.RequestContent);
                    if (request != null && request.CardType.ToLower() == "zing")
                        TelegramNotify.SendNotify(partnerCode, string.Empty, String.Format("Lỗi -1 zing  seri {0}  ", request.CardType), 0, 4);


                }
            }
            catch (Exception ex)
            {
                _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                NLogLogger.Info(new string[] { "API", "Error", _APITransaction.TransactionID.ToString(), "VPGService.cs", "(APIService)obj", ex.Message.Replace("\n", " ") });
            }

            _APITransaction.Status = _APIResponse.ResponseCode < 0 ? _APIResponse.ResponseCode : 1;
            _APITransaction.UpdateStatus();

            // Chữ ký
            _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, privateKey, signatureType);
            result = serializer.Serialize(_APIResponse);
            NLogLogger.Info(new string[] { "API", "Response", _APITransaction.TransactionID.ToString(), result });
            return result;
        }
        public static string Request(string partnerCode, string serviceCode, string commandCode, string requestContent, string signature)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Ghi log giao dịch
            NLogLogger.Info(new string[] { "API", "Request", partnerCode, serviceCode, commandCode, requestContent, signature });

            string result = "";
            APIResponse _APIResponse = new APIResponse();
            string ip = IPAddress.Get();

            Payments _payments = new Payments().GetCheckCache(serviceCode);
            if (_payments == null)
            {
                result = ResponseUtils.Response((int)ResponseCode.ServiceNotExists);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
            }

            //cahce 
            Partners _Partner = new Partners().GetCache(partnerCode);
            //Kiểm tra _Partner tồn tại hoặc Active không
            if (_Partner == null || _Partner.Status == 0)
            {
                result = ResponseUtils.Response((int)ResponseCode.PartnerNotExistsNotActive);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                return result;
            }

            //Kiểm tra IP Partner

            //cahce
            PartnerService _partnerService = new PartnerService();
            //Kiểm tra Partner co được add Service ko
            var partnerStatus = _partnerService.GetCache(_Partner.PartnerID, _payments.ServiceID);
            if (partnerStatus == null || partnerStatus.Status == 0)
            {
                result = ResponseUtils.Response((int)ResponseCode.ServiceNotExists);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                return result;
            }


            // Thêm mới giao dịch, 
            APITransaction _APITransaction = new APITransaction();
            try
            {
                _APITransaction.PartnerCode = partnerCode;
                _APITransaction.ServiceCode = serviceCode;
                _APITransaction.CommandCode = commandCode;
                _APITransaction.RequestContent = requestContent;
                _APITransaction.Signature = signature;
                _APITransaction.IpAddress = ip;
                _APITransaction = _APITransaction.Add();
            }
            catch (Exception ex)
            {
                // Nếu thêm mới giao dịch không thành công
                result = ResponseUtils.Response((int)ResponseCode.SystemError);
                NLogLogger.Info(new string[] { "API", "Error", "VPGService.cs", "_APITransaction.Add", ex.Message.Replace("\n", " ") });
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, "0", result });
                return result;
            }

            // Nếu giao dịch không hợp lệ
            if (_APITransaction.ReturnValue != 0)
            {

                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                return result;
            }


            string privateKey = _Partner.PrivateKey;
            string publicKey = _Partner.PublicKey;
            int signatureType = _Partner.SignatureType;


            // Kiểm tra chữ ký
            if (!PaymentUtils.CheckSignature(partnerCode + serviceCode + commandCode + requestContent, signature, publicKey, signatureType))
            {
                _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
                _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, _Partner.PrivateKey, _Partner.SignatureType);

                _APITransaction.Status = _APIResponse.ResponseCode;
                _APITransaction.UpdateStatus();

                result = serializer.Serialize(_APIResponse);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result, partnerCode + serviceCode + commandCode + requestContent + publicKey });
                return result;
            }

            // Tải dll
            //Payments _Payment = new Payments();
            //APIService _APIService;
            try
            {
                //if (HttpContext.Current.Application["ServiceList"] == null)
                //{
                //    HttpContext.Current.Application["ServiceList"] = _Payment.GetList();
                //}
                //List<Payments> list = (List<Payments>)HttpContext.Current.Application["ServiceList"];
                //for (int i = 0; i < list.Count; i++)
                //{
                //    _Payment = list[i];
                //    if (_Payment.ServiceCode == serviceCode)
                //    {
                //        Assembly assembly = Assembly.Load(_Payment.ClassData);
                //        object obj = assembly.CreateInstance(_Payment.ClassName);
                //        _APIService = (APIService)obj;
                //        _APIResponse = _APIService.Request(_APITransaction);
                //        break;
                //    }
                //}

                //switch (serviceCode)
                //{
                //    case "cardtelco":
                //        {
                //            _APIService = new CardTelcoService();
                //            _APIResponse = _APIService.Request(_APITransaction);
                //            break;
                //        }
                //    case "buycard":
                //        {
                //            _APIService = new BuyCardService();
                //            _APIResponse = _APIService.Request(_APITransaction);
                //            break;
                //        }
                //}
                //NLogLogger.Info(new string[] { "ServiceCode", serviceCode });
                var handler = ServiceFactory.GetHandler(serviceCode);
                _APIResponse = handler.Request(_APITransaction);

                //callback
                //if (partnerCode == "hyn3")
                //{
                //    var request = serializer.Deserialize<UseCardRequest>(_APITransaction.RequestContent);
                //    if (request != null)
                //    {
                //        int amount = 0;
                //        if (_APIResponse.ResponseCode == 1)
                //            amount = int.Parse(_APIResponse.ResponseContent);
                //        if (_APIResponse.ResponseCode != (int)ResponseCode.TransactionTimeout)
                //        {
                //            if (!string.IsNullOrEmpty(request.CallbackUrl))
                //            {
                //                var datacb = new Libs.CardTelco.PayPlusAppV2VTT.DataCallback()
                //                {
                //                    Amount = amount,
                //                    RefCode = request.RefCode,
                //                    Status = _APIResponse.ResponseCode,
                //                    Signature = Libs.Utils.Encrypts.MD5(request.RefCode + _APIResponse.ResponseCode + amount + privateKey)
                //                };
                //                Task.Run(() => CallbackJson(request.CallbackUrl, serializer.Serialize(datacb), partnerCode, 0, 2).ConfigureAwait(false));
                //            }
                //        }
                //    }

                //}
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionTimeout)
                {

                    //if (_APIResponse.ResponseContent != "ntnet")
                    //{
                    var request = serializer.Deserialize<UseCardRequest>(_APITransaction.RequestContent);
                    if (request != null)
                    {
                        var keyCache = String.Format("RedisProviderTimeout:{0}", _APIResponse.ResponseContent);
                        var dataCache = DataCaching.GetCache<string>(keyCache);
                        var timeoutCount = 0;
                        if (dataCache != null)
                            timeoutCount = int.Parse(dataCache);
                        if (timeoutCount > 12)
                        {
                            TelegramNotify.SendNotify(partnerCode + " - " + _APIResponse.ResponseContent, string.Empty, String.Format("-326 (Transaction Timeout) {0} - ", request.CardType), 0, 4);
                            timeoutCount = 0;
                        }
                        else
                        {
                            timeoutCount++;
                        }
                        DataCaching.SetCache(keyCache, timeoutCount.ToString(), 300);
                    }

                    //}
                    _APIResponse.ResponseContent = "";
                }
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionFailed || _APIResponse.ResponseCode == (int)ResponseCode.TransactionSuspicious || _APIResponse.ResponseCode == (int)ResponseCode.SystemError)
                {

                    //if (_APIResponse.ResponseContent != "ntnet")
                    //{
                    var request = serializer.Deserialize<UseCardRequest>(_APITransaction.RequestContent);
                    if (request != null)
                    {
                        var keyCache = String.Format("RedisProviderFail:{0}", _APIResponse.ResponseContent);
                        var dataCache = DataCaching.GetCache<string>(keyCache);
                        var timeoutCount = 0;
                        if (dataCache != null)
                            timeoutCount = int.Parse(dataCache);
                        if (timeoutCount > 10)
                        {
                            TelegramNotify.SendNotify(partnerCode + " - " + _APIResponse.ResponseContent, string.Empty, String.Format("-1 (Transaction Fail) {0} - ", request.CardType), 0, 4);
                            timeoutCount = 0;
                        }
                        else
                        {
                            timeoutCount++;
                        }
                        DataCaching.SetCache(keyCache, timeoutCount.ToString(), 300);
                    }

                    //}
                    _APIResponse.ResponseContent = "";
                }
                if (_APIResponse.ResponseCode == (int)ResponseCode.ProviderNotFound)
                {


                    var request = serializer.Deserialize<UseCardRequest>(_APITransaction.RequestContent);
                    if (request != null)
                        TelegramNotify.SendNotify(partnerCode, string.Empty, String.Format("-327 (Provider Not Found) {0}  ", request.CardType), 0, 4);


                }

            }
            catch (Exception ex)
            {
                _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                NLogLogger.Info(new string[] { "API", "Error", _APITransaction.TransactionID.ToString(), "VPGService.cs", "(APIService)obj", ex.Message.Replace("\n", " ") });
            }

            _APITransaction.Status = _APIResponse.ResponseCode < 0 ? _APIResponse.ResponseCode : 1;
            _APITransaction.UpdateStatus();

            // Chữ ký
            _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, privateKey, signatureType);
            result = serializer.Serialize(_APIResponse);
            NLogLogger.Info(new string[] { "API", "Response", _APITransaction.TransactionID.ToString(), result });
            return result;
        }
        public static async Task<string> CallbackJson(string url, string postData, string code, long tranId = 0, int type = 0)
        {
            NLogLogger.Info(new string[] { "Callback", "Callback Type", type.ToString(), "Request", code, tranId.ToString(), url, postData });
            System.Threading.Thread.Sleep(1000);
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
                        NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Response", code, tranId.ToString(), url, postData, responseContent });

                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "TopupAppVTT", "Callback Type", type.ToString(), "Error", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }
}