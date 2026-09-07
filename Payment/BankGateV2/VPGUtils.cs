using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankGate;
using Libs.Utils;
using Libs.Report;
namespace BankGateV2
{
    public class VPGUtils
    {
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

            //if (commandCode== "getbanksv2")
            //{
            //    result = ResponseUtils.Response((int)ResponseCode.ServiceNotExists);
            //    //NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
            //    return result;
            //}
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
                if (commandCode != "getbanks" && commandCode != "getbanksv2")
                {
                    _APITransaction = _APITransaction.Add();
                }
                else
                {
                    _APITransaction.TransactionID = 1;
                }
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
            if (commandCode != "getbanks" && commandCode != "getbanksv2")
            {
                if (_APITransaction.ReturnValue != 0)
                {
                    result = ResponseUtils.Response(_APITransaction.ReturnValue);
                    NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                    return result;
                }
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
                if (commandCode != "getbanks" && commandCode != "getbanksv2")
                {
                    _APITransaction.UpdateStatus();
                }
                result = serializer.Serialize(_APIResponse);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, _APITransaction.TransactionID.ToString(), result });
                return result;
            }


            try
            {

                var handler = ServiceFactory.GetHandler(serviceCode);
                _APIResponse = handler.Request(_APITransaction);
                //if (_APIResponse.ResponseCode == (int)ResponseCode.TransactionTimeout)
                //{
                //    TelegramNotify.SendNotify(partnerCode, string.Empty, "-326 (Transaction Timeout)", 0, 4);
                //}


            }
            catch (Exception ex)
            {
                _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                NLogLogger.Info(new string[] { "API", "Error", _APITransaction.TransactionID.ToString(), "VPGService.cs", "(APIService)obj", ex.Message.Replace("\n", " ") });
            }

            _APITransaction.Status = _APIResponse.ResponseCode < 0 ? _APIResponse.ResponseCode : 1;
            if (commandCode != "getbanks" && commandCode != "getbanksv2")
            {
                _APITransaction.UpdateStatus();
            }
            // Chữ ký
            _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, privateKey, signatureType);
            result = serializer.Serialize(_APIResponse);
            if (_APIResponse.ResponseCode != 1)
                NLogLogger.Info(new string[] { "API", "Response", _APITransaction.TransactionID.ToString(), result });
            return result;
        }
        public static string RequestGetMomo(string partnerCode, string type, string signature, string requestime)
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
                _APITransaction.RequestContent = "{\"Type\":\"momov2\"}";
                _APIResponse = handler.Request(_APITransaction);

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
        public static string RequestGetBank(string partnerCode, string type, string signature)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Ghi log giao dịch
            //NLogLogger.Info(new string[] { "API", "Request", partnerCode, signature });
            string serviceCode = "bankdirect";
            string result = "";
            string commandCode = "getbanks";
            string requestContent = "";
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
            //if (!PaymentUtils.CheckSignature(partnerCode + requestContent, signature, publicKey, signatureType))
            //{
            //    _APIResponse = new APIResponse((int)ResponseCode.SignatureInvalid);
            //    _APIResponse.Signature = PaymentUtils.Signature(_APIResponse.ResponseCode.ToString() + _APIResponse.Description + _APIResponse.ResponseContent, _Partner.PrivateKey, _Partner.SignatureType);

            //    //_APITransaction.Status = _APIResponse.ResponseCode;
            //    //_APITransaction.UpdateStatus();

            //    result = serializer.Serialize(_APIResponse);
            //    NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, partnerCode + requestContent + publicKey, result });
            //    return result;
            //}


            try
            {

                var handler = ServiceFactory.GetHandler(serviceCode);
                _APITransaction.RequestContent = type;
                _APIResponse = handler.Request(_APITransaction);

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
        public static string Order(string partnerCode, int Amount, string RefCode, string BankCode, string Url, string signature, string type = "banktranfer")
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Ghi log giao dịch
            //NLogLogger.Info(new string[] { "API", "Request", partnerCode, signature });
            string serviceCode = "bankdirect";
            string result = "";
            string commandCode = "order";
            string requestContent = BankCode + Amount + RefCode + Url;
            APIResponse _APIResponse = new APIResponse();
            string ip = IPAddress.Get();

            Payments _payments = new Payments().GetCheckCache(serviceCode);
            if (RefCode.Length > 50)
            {
                result = ResponseUtils.Response((int)ResponseCode.ParameterInvalid);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                return result;
            }
            if (_payments == null)
            {
                result = ResponseUtils.Response((int)ResponseCode.ServiceNotExists);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                return result;
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
                var jsonContent = $"\"CallbackUrl\":\"{Url}\",\"BankName\":\"{BankCode}\",\"AppCode\":\"\",\"Type\":\"{type}\",\"RefCode\":\"{RefCode}\",\"Note\":\"{RefCode}\",\"Amount\":{Amount},\"AccountName\":\"{RefCode}\"";

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
            if (_APIResponse.ResponseCode < 1)
            {
                NLogLogger.Info(new string[] { "API", "Response", _APITransaction.TransactionID.ToString(), result });
            }
            return result;
        }


        public static string RequestCash(string partnerCode, string AccountNumber, string AccountName, int Amount, string RefCode, string BankCode, string signature, string url, string content)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Ghi log giao dịch
            // NLogLogger.Info(new string[] { "API", "Request", partnerCode, signature });
            string serviceCode = "bankcash";
            string result = "";
            string commandCode = "cash";
            string requestContent = AccountNumber + AccountName + BankCode + Amount + RefCode + url;
            if (partnerCode == "cn02")
                requestContent = AccountNumber + BankCode + Amount + RefCode + url;
            APIResponse _APIResponse = new APIResponse();
            string ip = IPAddress.Get();

            if (RefCode.Length > 50)
            {
                result = ResponseUtils.Response((int)ResponseCode.ParameterInvalid);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                return result;
            }

            Payments _payments = new Payments().GetCheckCache(serviceCode);
            if (_payments == null)
            {
                result = ResponseUtils.Response((int)ResponseCode.ServiceNotExists);
                NLogLogger.Info(new string[] { "API", "Response", partnerCode, serviceCode, result });
                return result;
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

            //check ip
            if (!string.IsNullOrEmpty(_partnerService.IPAddress))
            {
                if (!_partnerService.IPAddress.Contains(IPAddress.Get()))
                {
                    NLogLogger.Info(new string[] { "IpInvalid", _partnerService.IPAddress, IPAddress.Get() });
                    return ResponseUtils.Response((int)ResponseCode.IpInvalid);
                    //return new APIResponse((int)ResponseCode.IpInvalid);
                }

            }


            // Thêm mới giao dịch, 
            APITransaction _APITransaction = new APITransaction();
            try
            {
                var jsonContent = $"\"CallbackUrl\":\"{url}\",\"BankName\":\"momo\",\"AppCode\":\"\",\"Type\":\"momocash\",\"BankAccountName\":\"{AccountName}\",\"RefCode\":\"{RefCode}\",\"Note\":\"{content}\",\"Amount\":{Amount},\"BankAccountNumber\":\"{AccountNumber}\",\"AccountName\":\"{AccountNumber}\"";
                if (BankCode.ToUpper() != "MOMO")
                {
                    jsonContent = $"\"CallbackUrl\":\"{url}\",\"BankName\":\"{BankCode}\",\"AppCode\":\"\",\"Type\":\"bankcash\",\"BankAccountName\":\"{AccountName}\",\"RefCode\":\"{RefCode}\",\"Note\":\"{content}\",\"Amount\":{Amount},\"BankAccountNumber\":\"{AccountNumber}\",\"AccountName\":\"{AccountNumber}\"";
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

            //check giới hạn ngày
            //check giới hạn ngày
            //NLogLogger.Info(new string[] { "partnerStatus", _partnerService.Quota.ToString(), _partnerService.Occurs.ToString(), transaction.PartnerCode.ToString() });
            //if (_partnerService.Quota != 0 && _partnerService.Occurs == 1)
            //{

            //    long total = 0; int totaltrans = 0; long total2 = 0;
            //    var data = new Libs.Report.BankCashAPI().ReportCheck("", partnerCode, "", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ref totaltrans, ref total);
            //    var data2 = new BankGateAPI().Report("", partnerCode, "", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ref totaltrans, ref total2);
            //    NLogLogger.Info(new string[] { "cash check", total.ToString(), total2.ToString(), _partnerService.Quota.ToString() });
            //    if (total + Amount >= _partnerService.Quota + total2)
            //    {
            //        TelegramClient.SendTeleV2("-4006848376", "[Out Litmit] Vượt hạn mức rút từ đối tác " + partnerCode + "(" + total.ToString("#,#").Replace(",", ".") + "-" + total2.ToString("#,#").Replace(",", ".") + " Refcode " + RefCode + ")");
            //        NLogLogger.Info(new string[] { "TransactionIgnore", total.ToString(), total2.ToString(), _partnerService.Quota.ToString() });
            //        result = ResponseUtils.Response((int)ResponseCode.TransactionIgnore);
            //        //result.Description = "Vượt hạn mức rút trong ngày";
            //        return result;
            //    }

            //}

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
    }
}