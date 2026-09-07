using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;
using System.Reflection;

/// <summary>
/// Summary description for VPGUtils
/// </summary>
public class VPGUtils
{
	public VPGUtils()
	{
		//
		// TODO: Add constructor logic here
		//
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
        
        //cahce 
        Partners _Partner = new Partners().GetCache(partnerCode); 
        //Kiểm tra _Partner tồn tại hoặc Active không
        if (_Partner == null || _Partner.Status == 0)
        {
            return ResponseUtils.Response((int)ResponseCode.PartnerNotExistsNotActive);
        }

        //Kiểm tra IP Partner

        //cahce
        PartnerService _partnerService = new PartnerService();
        //Kiểm tra Partner co được add Service ko
        var partnerStatus = _partnerService.GetCache(_Partner.PartnerID, _payments.ServiceID);
        if (partnerStatus == null || partnerStatus.Status == 0)
        {
            return ResponseUtils.Response((int)ResponseCode.ServiceNotExists);
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

        // Tải dll
        Payments _Payment = new Payments();
        APIService _APIService;
        try
        {
            if (HttpContext.Current.Application["ServiceList"] == null)
            {
                HttpContext.Current.Application["ServiceList"] = _Payment.GetList();
            }
            List<Payments> list = (List<Payments>)HttpContext.Current.Application["ServiceList"];
            for (int i = 0; i < list.Count; i++)
            {
                _Payment = list[i];
                if (_Payment.ServiceCode == serviceCode)
                {
                    Assembly assembly = Assembly.Load(_Payment.ClassData);
                    object obj = assembly.CreateInstance(_Payment.ClassName);
                    _APIService = (APIService)obj;
                    _APIResponse = _APIService.Request(_APITransaction);
                    break;
                }
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