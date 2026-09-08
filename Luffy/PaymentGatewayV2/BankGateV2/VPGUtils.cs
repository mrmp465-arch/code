using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankCash;
using Libs.BankGate;
using Libs.Report;
using Libs.Utils;

namespace BankGateV2
{
    public class VPGUtils
    {
        public static string RequestMomoCheck(string partnerCode, string AccountNumber, string requestime, string signature)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Ghi log giao dịch
            NLogLogger.Info(new string[] { "API", "Request", partnerCode, signature });
            string serviceCode = "bankcash";
            string result = "";
            string commandCode = "check";
            string requestContent = AccountNumber + requestime;
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
                //var jsonContent = AccountNumber;
                _APITransaction.PartnerCode = partnerCode;
                _APITransaction.ServiceCode = serviceCode;
                _APITransaction.CommandCode = commandCode;
                _APITransaction.RequestContent = AccountNumber;
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
                result = ResponseUtils.Response(_APITransaction.ReturnValue);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                return result;
            }


            string privateKey = _Partner.PrivateKey;
            string publicKey = _Partner.PublicKey;
            int signatureType = _Partner.SignatureType;


            // Kiểm tra chữ ký
            //if (!PaymentUtils.CheckSignature(partnerCode + requestContent, signature, publicKey, signatureType))
            //{
            //    _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
            //    _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, _Partner.PrivateKey, _Partner.SignatureType);

            //    _APITransaction.Status = _APIResponse.ResponseCode;
            //    _APITransaction.UpdateStatus();

            //    result = serializer.Serialize(_APIResponse);
            //    NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result, partnerCode + requestContent+ publicKey, signature });
            //    return result;
            //}


            try
            {

                var handler = ServiceFactory.GetHandler(serviceCode);
                _APIResponse = handler.Request(_APITransaction);
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionTimeout)
                {
                    TelegramNotify.SendNotify(partnerCode, string.Empty, "-326 (Transaction Timeout)", 0, 4);
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
        public static string RequestMomoCash(string partnerCode, string AccountNumber, string AccountName, int Amount, string RefCode, string BankCode, string signature, string url)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Ghi log giao dịch
            NLogLogger.Info(new string[] { "API", "Request", partnerCode, signature });
            string serviceCode = "bankcash";
            string result = "";
            string commandCode = "cash";
            string requestContent = AccountNumber + AccountName + BankCode + Amount + RefCode;
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
            if (_Partner.PartnerCode == "azt")
            {
                Partners _PartnerNotCache = new Partners().Get(partnerCode);
                if (_PartnerNotCache.Balance < _PartnerNotCache.Deposit)
                {
                    result = ResponseUtils.Response((int)ResponseCode.TransactionIgnore);
                    NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                    return result;
                }
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
            //check giới hạn ngày
            if (partnerStatus.Quota != 0 && partnerStatus.Occurs == 1 && Amount >= 1000000)
            {
                long total = 0; int totaltrans = 0; long total2 = 0;
                long totalcard = 0;
                var datacard = new CardAPILog().Report("", _Partner.PartnerCode, "", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ref totaltrans, ref totalcard);
                var data = new Libs.BankCash.BankCashAPI().Report2("", _Partner.PartnerCode, "", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ref totaltrans, ref total);
                var data2 = new Libs.BankDirect.BankGateAPI().Report("", _Partner.PartnerCode, "", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ref totaltrans, ref total2);
                if (total + Amount >= partnerStatus.Quota + total2 + totalcard)
                {
                    result = ResponseUtils.Response((int)ResponseCode.SystemMaintain);
                    NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                    TelegramNotify.SendWarning("1690000254", $"Đối tác {_Partner.PartnerCode} rút vượt nạp");
                    return result;
                }

            }
            if (partnerCode == "panpan"  && DateTime.Now.Hour>=20)
            {
                try
                {
                    long total = 0; int totaltrans = 0;
                    var data = new Libs.BankCash.BankCashAPI().Report("", _Partner.PartnerCode, "", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ref totaltrans, ref total);
                    if (total >= 160000000)
                    {
                        result = ResponseUtils.Response((int)ResponseCode.SystemMaintain);

                        return result;
                    }
                }
                catch
                {

                }
                
            }

            // Thêm mới giao dịch, 
            APITransaction _APITransaction = new APITransaction();
            try
            {
                var jsonContent = $"\"CallbackUrl\":\"{url}\",\"BankName\":\"momo\",\"AppCode\":\"\",\"Type\":\"momocash\",\"BankAccountName\":\"{AccountName}\",\"RefCode\":\"{RefCode}\",\"Note\":\"{RefCode}\",\"Amount\":{Amount},\"BankAccountNumber\":\"{AccountNumber}\",\"AccountName\":\"{AccountNumber}\"";
                if (BankCode.ToUpper() != "MOMO")
                {
                    jsonContent = $"\"CallbackUrl\":\"{url}\",\"BankName\":\"{BankCode}\",\"AppCode\":\"\",\"Type\":\"bankcash\",\"BankAccountName\":\"{AccountName}\",\"RefCode\":\"{RefCode}\",\"Note\":\"{RefCode}\",\"Amount\":{Amount},\"BankAccountNumber\":\"{AccountNumber}\",\"AccountName\":\"{AccountNumber}\"";
                }
                _APITransaction.PartnerCode = partnerCode;
                _APITransaction.ServiceCode = serviceCode;
                _APITransaction.CommandCode = commandCode;
                _APITransaction.RequestContent = "{" + jsonContent + "}";
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
                result = ResponseUtils.Response(_APITransaction.ReturnValue);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                return result;
            }


            string privateKey = _Partner.PrivateKey;
            string publicKey = _Partner.PublicKey;
            int signatureType = _Partner.SignatureType;


            // Kiểm tra chữ ký
            if (!PaymentUtils.CheckSignature(partnerCode + requestContent, signature, publicKey, signatureType))
            {
                _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
                _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, _Partner.PrivateKey, _Partner.SignatureType);

                _APITransaction.Status = _APIResponse.ResponseCode;
                _APITransaction.UpdateStatus();

                result = serializer.Serialize(_APIResponse);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                return result;
            }


            try
            {

                var handler = ServiceFactory.GetHandler(serviceCode);
                _APIResponse = handler.Request(_APITransaction);
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionTimeout)
                {
                    TelegramNotify.SendNotify(partnerCode, string.Empty, "-326 (Transaction Timeout)", 0, 4);
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
        public static string RequestMomo(string partnerCode, string requestime, string type, string signature)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Ghi log giao dịch
            //NLogLogger.Info(new string[] { "API", "Request", partnerCode, signature });
            string serviceCode = "bankdirect";
            string result = "";
            string commandCode = "getbanksv2";
            string requestContent = requestime;
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
                //_APITransaction = _APITransaction.Add();
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
            //if (_APITransaction.ReturnValue != 0)
            //{
            //    result = ResponseUtils.Response(_APITransaction.ReturnValue);
            //    NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
            //    return result;
            //}


            string privateKey = _Partner.PrivateKey;
            string publicKey = _Partner.PublicKey;
            int signatureType = _Partner.SignatureType;


            // Kiểm tra chữ ký
            if (!PaymentUtils.CheckSignature(partnerCode + requestContent, signature, publicKey, signatureType))
            {
                _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
                _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, _Partner.PrivateKey, _Partner.SignatureType);

                //_APITransaction.Status = _APIResponse.ResponseCode;
                //_APITransaction.UpdateStatus();

                result = serializer.Serialize(_APIResponse);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, partnerCode + requestContent + publicKey, result });
                return result;
            }


            try
            {

                var handler = ServiceFactory.GetHandler(serviceCode);
                _APITransaction.RequestContent = type;
                _APIResponse = handler.Request(_APITransaction);
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionTimeout)
                {
                    TelegramNotify.SendNotify(partnerCode, string.Empty, "-326 (Transaction Timeout)", 0, 4);
                }


            }
            catch (Exception ex)
            {
                _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                NLogLogger.Info(new string[] { "API", "Error", _APITransaction.TransactionID.ToString(), "VPGService.cs", "(APIService)obj", ex.Message.Replace("\n", " ") });
            }

            //_APITransaction.Status = _APIResponse.ResponseCode < 0 ? _APIResponse.ResponseCode : 1;
            //_APITransaction.UpdateStatus();

            // Chữ ký
            _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, privateKey, signatureType);
            result = serializer.Serialize(_APIResponse);
            //NLogLogger.Info(new string[] { "API", "Response", _APITransaction.TransactionID.ToString(), result });
            return result;
        }
        public static string Order(string partnerCode, int Amount, string RefCode, string BankCode, string Url, string AccountName, string signature, string type = "banktranfer")
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Ghi log giao dịch
            //NLogLogger.Info(new string[] { "API", "Request", partnerCode, signature });
            string serviceCode = "bankdirect";
            string result = "";
            string commandCode = "order";
            string requestContent = BankCode + Amount + RefCode;
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
                var jsonContent = $"\"CallbackUrl\":\"{Url}\",\"BankName\":\"{BankCode}\",\"AppCode\":\"\",\"Type\":\"{type}\",\"RefCode\":\"{RefCode}\",\"Note\":\"{RefCode}\",\"Amount\":{Amount},\"AccountName\":\"{AccountName}\"";

                _APITransaction.PartnerCode = partnerCode;
                _APITransaction.ServiceCode = serviceCode;
                _APITransaction.CommandCode = commandCode;
                _APITransaction.RequestContent = "{" + jsonContent + "}";
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
                result = ResponseUtils.Response(_APITransaction.ReturnValue);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                return result;
            }


            string privateKey = _Partner.PrivateKey;
            string publicKey = _Partner.PublicKey;
            int signatureType = _Partner.SignatureType;


            // Kiểm tra chữ ký
            if (!PaymentUtils.CheckSignature(partnerCode + requestContent, signature, publicKey, signatureType))
            {
                _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
                _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, _Partner.PrivateKey, _Partner.SignatureType);

                _APITransaction.Status = _APIResponse.ResponseCode;
                _APITransaction.UpdateStatus();

                result = serializer.Serialize(_APIResponse);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                return result;
            }


            try
            {

                var handler = ServiceFactory.GetHandler(serviceCode);
                _APIResponse = handler.Request(_APITransaction);
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionTimeout)
                {
                    TelegramNotify.SendNotify(partnerCode, string.Empty, "-326 (Transaction Timeout)", 0, 4);
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
                result = ResponseUtils.Response(_APITransaction.ReturnValue);
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
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                return result;
            }


            try
            {

                var handler = ServiceFactory.GetHandler(serviceCode);
                _APIResponse = handler.Request(_APITransaction);
                if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionTimeout)
                {
                    TelegramNotify.SendNotify(partnerCode, string.Empty, "-326 (Transaction Timeout)", 0, 4);
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
    }
}