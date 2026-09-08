using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;

namespace Libs.VGGTopup
{
    public class VGGTopupService : APIService
    {
        protected int maxError = 5;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string amountList = "|10000|20000|30000|50000|100000|200000|500000|1000000|2000000|3000000|5000000|";

        public VGGTopupService()
        {

        }

        public override APIResponse Request(APITransaction transaction)
        {
            switch (transaction.CommandCode)
            {
                case "topup":
                    return Topup(transaction);
                case "checkaccount":
                    return CheckAccount(transaction);
                case "checktransaction":
                    return CheckTransaction(transaction);
                default:
                    APIResponse _APIResponse = new APIResponse((int)ResponseCode.AccessDenied);
                    return _APIResponse;
            }
        }

        private APIResponse Topup(APITransaction transaction)
        {
            APIResponse _APIResponse = new APIResponse();
            TopupRequest request = new TopupRequest();

            // Phân tích yêu cầu thành đối tượng
            try
            {
                request = serializer.Deserialize<TopupRequest>(transaction.RequestContent);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "VGGTopup", transaction.TransactionID.ToString(), "Error", "Deserialize(requestContent)", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

            // Kiểm tra mệnh giá nạp tiền
            if (amountList.IndexOf("|" + request.Amount.ToString() + "|") < 0)
            {
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

            // Thêm mới giao dịch
            TopupAPILog _TopupAPILog = new TopupAPILog();
            try
            {
                // Thêm thông tin giao dịch vào CSDL
                _TopupAPILog.PartnerID = transaction.PartnerID;
                _TopupAPILog.TransactionID = transaction.TransactionID;
                _TopupAPILog.RequestID = request.RequestID;
                _TopupAPILog.AccountName = request.AccountName;
                _TopupAPILog.Amount = request.Amount;
                _TopupAPILog.Description = DateTime.Now.ToString() + " _TopupAPILog.Add; \n";
                _TopupAPILog.Add();
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "VGGTopup", transaction.TransactionID.ToString(), "Error", "_TopupAPILog.Add", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.SystemError);
            }

            // Nếu giao dịch bị lặp
            if (_TopupAPILog.ReturnValue < 0)
            {
                return new APIResponse(_TopupAPILog.ReturnValue);
            }

            // Kiểm tra tài khoản
            Accounts _Account = new Accounts();
            _Account = _Account.Get(request.AccountName);

            if (_Account == null)
            {
                _TopupAPILog.Description = _TopupAPILog.Description + DateTime.Now.ToString() + " AccountNotExists; \n";
                _TopupAPILog.Status = (int)ResponseCode.AccountNotExists;
                _TopupAPILog.Update();

                return new APIResponse((int)ResponseCode.AccountNotExists);
            }

            VGGTopup _Topup = new VGGTopup();
            if (transaction.PartnerCode == Constant.Smartlink_PartnerCode)
            {
                _Topup.ServiceID = Constant.Smartlink_ServiceID;
                _Topup.ServiceKey = Constant.Smartlink_ServiceKey;
                _Topup.Description = "Nạp GG từ ngân hàng qua Smartlink";
            }
            else
            {
                _Topup.ServiceID = Constant.ServiceID;
                _Topup.ServiceKey = Constant.ServiceKey;
                _Topup.Description = "Nạp GG từ VTC Intecom";
            }

            _Topup.Description = _Topup.Description + ", mã giao dịch: " + transaction.TransactionID.ToString();

            // GetAccessToken
            string accessToken;
            accessToken = _Topup.GetAccessToken(_Topup.ServiceID, request.AccountName);
            if (string.IsNullOrEmpty(accessToken))
            {
                return new APIResponse((int)ResponseCode.TransactionFailed);
            }

            // Topup tiền vào tài khoản
            _Topup.AccountName = request.AccountName;
            _Topup.AccessToken = accessToken;
            _Topup.Amount = request.Amount;
            _Topup.PartnerTransactionID = request.RequestID;
            _Topup.ReferenceID = _TopupAPILog.TransactionID;
            _Topup.ClientIP = "127.0.0.1";
            NLogLogger.Info(new string[] { "VGGTopup", transaction.TransactionID.ToString(), "Topup", transaction.PartnerCode, "Request", serializer.Serialize(_Topup) });
            if (transaction.PartnerCode == Constant.Smartlink_PartnerCode)
            {
                _Topup.TopupFromSmartlink();
            }
            else
            {
                _Topup.TopupFromIntecom();
            }
            NLogLogger.Info(new string[] { "VGGTopup", transaction.TransactionID.ToString(), "Topup", transaction.PartnerCode, "Response", serializer.Serialize(_Topup) });

            int status = 0;
            if (_Topup.ReturnValue < 0)
            {
                _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                _TopupAPILog.Status = (int)ResponseCode.TransactionFailed;
                _TopupAPILog.Description = _TopupAPILog.Description + DateTime.Now.ToString() + " Topup failed " + _Topup.ReturnValue.ToString() + "; \n";
                status = -99;
            }
            else
            {
                _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                _TopupAPILog.Status = 1;
                _TopupAPILog.Description = _TopupAPILog.Description + DateTime.Now.ToString() + " Transaction successful " + _Topup.ReturnValue.ToString() + "; \n";
                status = Convert.ToInt32(_Topup.ReturnValue);
            }


            _APIResponse.ResponseContent = transaction.TransactionID.ToString();
            _TopupAPILog.Update();

            // insert transaction log   27/01/2014
            try
            {
                _Topup.AddLog(_Topup.ReferenceID, Constant.ServiceID, _Account.AccountID, _Topup.Amount / 1000, "des: " + _Topup.Description, _Topup.ClientIP, status);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "VGGTopup", transaction.TransactionID.ToString(), "Error", ex.ToString() });
            }


            return _APIResponse;
        }

        private APIResponse CheckAccount(APITransaction transaction)
        {
            Accounts _Account = new Accounts();
            _Account = _Account.Get(transaction.RequestContent);

            if (_Account == null)
            {
                return new APIResponse((int)ResponseCode.AccountNotExists);
            }

            APIResponse _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
            _APIResponse.ResponseContent = _Account.AccountID.ToString();
            return _APIResponse;
        }

        private APIResponse CheckTransaction(APITransaction transaction)
        {
            APIResponse _APIResponse = new APIResponse();

            // Kiểm tra trên hệ thống của VGG
            TopupAPILog _TopupAPILog = new TopupAPILog();
            _TopupAPILog = _TopupAPILog.Check(transaction.PartnerID, transaction.RequestContent);

            if (_TopupAPILog == null)
            {
                return new APIResponse((int)ResponseCode.TransactionNotExists);
            }

            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
            _APIResponse.ResponseContent = transaction.TransactionID.ToString();

            return _APIResponse;
        }

    }

    public class TopupRequest
    {
        public string RequestID { get; set; }
        public string AccountName { get; set; }
        public long Amount { get; set; }
    }

}
