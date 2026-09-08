using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using System.Text.RegularExpressions;
using Libs.Utils;

namespace Libs.BankDirect
{
    public class BankDirectService : APIService
    {
        public BankDirectService()
        {

        }

        public override APIResponse Request(APITransaction transaction)
        {
            switch (transaction.CommandCode)
            {
                case "addtransaction":
                    return AddTransaction(transaction);
                case "verifycard":
                    return VerifyCard(transaction);
                case "verifyotp":
                    return VerifyOTP(transaction);
                default:
                    return new APIResponse((int)ResponseCode.AccessDenied);
            }
        }

        private APIResponse AddTransaction(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            AddTransactionRequest request = new AddTransactionRequest();

            int step = 0;
            try
            {
                step = 1;
                request = serializer.Deserialize<AddTransactionRequest>(transaction.RequestContent);

                // OrderNo chỉ gồm các ký tự a-z, A-Z và 0-9
                if (!new Regex(@"^[a-zA-Z0-9]{4,30}$").Match(request.OrderNo).Success)
                {
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                // Giao dịch tối thiểu 10.000 (mười nghìn), tối đa 100.000.000 (một trăm triệu)
                if (request.Amount < 10000 || request.Amount > 100000000)
                {
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                // Kiểm tra thời gian, không lệch hơn hay kém 10 phút
                long minTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(-10).ToString("yyyyMMddHHmmss"));
                long maxTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(10).ToString("yyyyMMddHHmmss"));
                if (request.RequestTime < minTime || request.RequestTime > maxTime)
                {
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                if (string.IsNullOrEmpty(request.BankCode))
                {
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                step = 2;   // thêm dữ liệu vào csdl
                BankGateAPI _BankGateAPI = new BankGateAPI();
                _BankGateAPI.PartnerID = transaction.PartnerID;
                _BankGateAPI.OrderNo = request.OrderNo;
                _BankGateAPI.OrderInfo = request.OrderInfo;
                _BankGateAPI.Amount = request.Amount;
                _BankGateAPI.FullName = request.FullName;
                _BankGateAPI.Mobile = request.Mobile;
                _BankGateAPI.RequestTime = request.RequestTime;
                _BankGateAPI.Signature = "";

                //Rem đỡ lỗi V2
                //_BankGateAPI.ServiceID = 13;    // dịch vụ thanh toán qua Banknetvn
                //_BankGateAPI.TotalAmount = TotalAmount(_BankGateAPI.Amount, request.BankCode);
                //_BankGateAPI = _BankGateAPI.Add();

                //// Nếu thêm giao dịch không thành công
                //if (_BankGateAPI.ReturnValue < 0)
                //{
                //    return new APIResponse(_BankGateAPI.ReturnValue);
                //}

                step = 3;

                APIResponse respone = new APIResponse();
                Banknet.Banknetvn _Banknetvn = new Banknet.Banknetvn();
                respone = _Banknetvn.AddTransaction(_BankGateAPI, request.BankCode);
                if (respone.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {
                    respone.ResponseContent = _BankGateAPI.TransactionID.ToString();
                }
                else
                {
                    _BankGateAPI.Status = respone.ResponseCode;
                    _BankGateAPI.UpdateStatus();
                }
                return respone;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankDirectService", "AddTransaction", step.ToString() , "Error", ex.Message.Replace("\n", " ") });
                switch (step)
                {
                    case 1:
                        return new APIResponse((int)ResponseCode.RequestContentInvalid);
                    default:
                        return new APIResponse((int)ResponseCode.TransactionFailed);
                }
            }
        }

        private APIResponse VerifyCard(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            VerifyCardRequest request = new VerifyCardRequest();

            int step = 0;
            try
            {
                step = 1;   // kiểm tra dữ liệu đầu vào
                request = serializer.Deserialize<VerifyCardRequest>(transaction.RequestContent);

                BankGateAPI _BankGateAPI = new BankGateAPI();
                _BankGateAPI.TransactionID = request.TransactionID;
                _BankGateAPI = _BankGateAPI.Get();

                if (_BankGateAPI == null)
                {
                    return new APIResponse((int)ResponseCode.TransactionNotExists);
                }

                if (_BankGateAPI.Status != 0)
                {
                    return new APIResponse((int)ResponseCode.TransactionDuplicate);
                }

                if (_BankGateAPI.CreatedTime.AddHours(1) < DateTime.Now)
                {
                    return new APIResponse((int)ResponseCode.TransactionExpired);
                }

                step = 2;   // gọi hàm kiểm tra thông tin thẻ

                APIResponse respone = new APIResponse();
                Banknet.Banknetvn _Banknetvn = new Banknet.Banknetvn();
                respone = _Banknetvn.VerifyCard(request);

                return respone;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankDirectService", "VerifyCard", step.ToString(), "Error", ex.Message.Replace("\n", " ") });
                switch (step)
                {
                    case 1:
                        return new APIResponse((int)ResponseCode.RequestContentInvalid);
                    default:
                        return new APIResponse((int)ResponseCode.TransactionFailed);
                }
            }
        }

        private APIResponse VerifyOTP(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            VerifyOTPRequest request = new VerifyOTPRequest();

            int step = 0;
            try
            {
                step = 1;   // kiểm tra dữ liệu đầu vào
                request = serializer.Deserialize<VerifyOTPRequest>(transaction.RequestContent);

                BankGateAPI _BankGateAPI = new BankGateAPI();
                _BankGateAPI.TransactionID = request.TransactionID;
                _BankGateAPI = _BankGateAPI.Get();

                if (_BankGateAPI == null)
                {
                    return new APIResponse((int)ResponseCode.TransactionNotExists);
                }

                if (_BankGateAPI.Status != 0)
                {
                    return new APIResponse((int)ResponseCode.TransactionDuplicate);
                }

                if (_BankGateAPI.CreatedTime.AddHours(1) < DateTime.Now)
                {
                    return new APIResponse((int)ResponseCode.TransactionExpired);
                }

                step = 2;   // gọi hàm kiểm tra mã xác thực OTP
                APIResponse respone = new APIResponse();
                Banknet.Banknetvn _Banknetvn = new Banknet.Banknetvn();
                respone = _Banknetvn.VerifyOTP(request);

                step = 3;   // cập nhật giao dịch
                if (respone.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {
                    _BankGateAPI.Status = 1;
                }
                else
                {
                    _BankGateAPI.Status = respone.ResponseCode;
                }
                respone.ResponseContent = _BankGateAPI.TransactionID.ToString();
                _BankGateAPI.UpdateStatus();

                return respone;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankDirectService", "VerifyCard", step.ToString(), "Error", ex.Message.Replace("\n", " ") });
                switch (step)
                {
                    case 1:
                        return new APIResponse((int)ResponseCode.RequestContentInvalid);
                    default:
                        return new APIResponse((int)ResponseCode.TransactionFailed);
                }
            }
        }

        private int TotalAmount(int amount, string bank)
        {
            long bigAmount = amount;
            return Convert.ToInt32(bigAmount * 1011 / 1000 + 1760);
        }

    }

    public class AddTransactionRequest
    {
        public string OrderNo { get; set; }
        public string OrderInfo { get; set; }
        public int Amount { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public long RequestTime { get; set; }
        public string BankCode { get; set; }
    }

    public class VerifyCardRequest
    {
        public long TransactionID { get; set; }
        public string CardNumber { get; set; }
        public string FullName { get; set; }
        public int CardMonth { get; set; }
        public int CardYear { get; set; }
        public string OTPType { get; set; }
    }

    public class VerifyOTPRequest
    {
        public long TransactionID { get; set; }
        public string OTP { get; set; }
    }

    public class ConfirmTransactionRequest
    {
        public long TransactionID { get; set; }
        public int Result { get; set; }
    }
}
