using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;
using Libs.Utils;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

namespace Libs.TopupPartner
{
    public class TopupPartnerService : APIService
    {
        public TopupPartnerService()
        {

        }

        public override APIResponse Request(APITransaction transaction)
        {
            switch (transaction.CommandCode)
            {
                case "topupmobile":
                    return TopupMobile(transaction);
                default:
                    return new APIResponse((int)ResponseCode.AccessDenied);
            }
        }

        private APIResponse TopupMobile(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TopupRequest request = new TopupRequest();

            int step = 0;
            try
            {
                step = 1;
                request = serializer.Deserialize<TopupRequest>(transaction.RequestContent);

                // RequestNo chỉ gồm các ký tự a-z, A-Z và 0-9
                if (!new Regex(@"^[a-zA-Z0-9]{4,30}$").Match(request.RequestNo).Success)
                {
                    NLogLogger.Info(new string[] { "TopupPartner", "TopupMobile", transaction.TransactionID.ToString(), "RequestContentInvalid - RequestNo", request.RequestNo });
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                // Giao dịch tối thiểu 5.000 (năm nghìn), tối đa 50.000.000 (năm mươi triệu)
                if (request.Amount < 5000 || request.Amount > 50000000)
                {
                    NLogLogger.Info(new string[] { "TopupPartner", "TopupMobile", transaction.TransactionID.ToString(), "RequestContentInvalid - Amount", request.Amount.ToString() });
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                // Kiểm tra thời gian, không lệch hơn hay kém 10 phút
                long minTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(-10).ToString("yyyyMMddHHmmss"));
                long maxTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(10).ToString("yyyyMMddHHmmss"));
                if (request.RequestTime < minTime || request.RequestTime > maxTime)
                {
                    NLogLogger.Info(new string[] { "TopupPartner", "TopupMobile", transaction.TransactionID.ToString(), "RequestContentInvalid - RequestTime", request.RequestTime.ToString() });
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                // Mobile chỉ gồm các các chữ số, có độ dài 10, 11
                if (!new Regex(@"^[0-9]{10,11}$").Match(request.Mobile).Success)
                {
                    NLogLogger.Info(new string[] { "TopupPartner", "TopupMobile", transaction.TransactionID.ToString(), "RequestContentInvalid - Mobile", request.Mobile });
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                // TopupType: 1-4
                if (request.TopupType < 1 || request.TopupType > 4)
                {
                    NLogLogger.Info(new string[] { "TopupPartner", "TopupMobile", transaction.TransactionID.ToString(), "RequestContentInvalid - TopupType", request.TopupType.ToString() });
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                // Provider: VTT, VMS, VNP
                if (request.Telco.ToUpper().IndexOf("VTT|VMS|VNP") < 0)
                {
                    NLogLogger.Info(new string[] { "TopupPartner", "TopupMobile", transaction.TransactionID.ToString(), "RequestContentInvalid - TopupType", request.TopupType.ToString() });
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                step = 2;   // thêm dữ liệu vào csdl
                TopupMobile _TopupMobile = new TopupMobile();
                _TopupMobile.TransactionID = transaction.TransactionID;
                _TopupMobile.PartnerID = transaction.PartnerID;
                _TopupMobile.Telco = request.Telco;
                _TopupMobile.RequestNo = request.RequestNo;
                _TopupMobile.RequestTime = request.RequestTime;
                _TopupMobile.Provider = "";
                _TopupMobile.Mobile = request.Mobile;
                _TopupMobile.Amount = request.Amount;
                _TopupMobile.TopupType = request.TopupType;
                _TopupMobile.LogContent = "Tạo mới giao dịch";

                _TopupMobile.Add();

                // Nếu thêm giao dịch không thành công
                if (_TopupMobile.ReturnValue < 0)
                {
                    return new APIResponse(_TopupMobile.ReturnValue);
                }

                //step = 3;

                //APIResponse respone = new APIResponse();
                //Banknet.Banknetvn _Banknetvn = new Banknet.Banknetvn();
                //respone = _Banknetvn.AddTransaction(_BankGateAPI, request.BankCode);
                //if (respone.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                //{
                //    respone.ResponseContent = _BankGateAPI.TransactionID.ToString();
                //}
                //else
                //{
                //    _BankGateAPI.Status = respone.ResponseCode;
                //    _BankGateAPI.UpdateStatus();
                //}
                //return respone;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankDirectService", "AddTransaction", step.ToString(), "Error", ex.Message.Replace("\n", " ") });
                switch (step)
                {
                    case 1:
                        return new APIResponse((int)ResponseCode.RequestContentInvalid);
                    default:
                        return new APIResponse((int)ResponseCode.TransactionFailed);
                }
            } return new APIResponse();
        }
    }

    public class TopupRequest
    {
        public string RequestNo { get; set; }
        public long RequestTime { get; set; }
        public string Telco { get; set; }
        public string Mobile { get; set; }
        public int TopupType { get; set; }
        public int Amount { get; set; }
    }
}
