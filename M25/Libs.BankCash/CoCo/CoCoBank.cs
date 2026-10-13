using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using static Libs.BankCash.BankCashService;
using static Libs.BankCash.CoCo.CoCoBankLib;


namespace Libs.BankCash.CoCo
{
    public class CoCoBank : IBankCashHandler
    {
        // Production MoBo
        protected string ServiceUrl = "http://108.160.140.162:1102/services/TRANSFER_MOMO";
        protected string ServiceUrlCheck = "http://108.160.140.162:1102/services/CHECK_USER_MOMO";
        protected string ServiceBankUrl = "http://108.160.140.162:1102/services/TRANSFER_BANK";
        protected string username = "VPSPEO";
        protected string secretKey = "Ukkoljqwjnaksnuanklafnaknneawrkhoa";
        public APIResponse Check(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CheckAccountRequest request = new CheckAccountRequest();
            request = serializer.Deserialize<CheckAccountRequest>(transaction.RequestContent);

            APIResponse _APIResponse = new APIResponse();
            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;

              
                if (request.Type == "momocash")
                {
                    if (!CheckValidMobile(request.BankAccountNumber))
                        return new APIResponse((int)ResponseCode.ParameterInvalid);
                }
                // Bước: Ghi log giao dịch
               

                // Bước: gọi hàm sang API
                step = 3;


                MobileRespone cashResult = new MobileRespone();
                if (request.Type == "momocash")
                {
                    MobileRequest _cashRequest = new MobileRequest();
                    _cashRequest.userName = this.username;
                    _cashRequest.requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    _cashRequest.accountCheck = request.BankAccountNumber;
                   
                    _cashRequest.authKey = Utils.Encrypts.MD5(_cashRequest.requestTime + "|"  + _cashRequest.userName + "|" + _cashRequest.accountCheck + "|" + secretKey);

                    NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "CoCoRequest", serializer.Serialize(_cashRequest) });
                    cashResult = ProcessCheck(_cashRequest);
                }
                else
                {
                    return new APIResponse((int)ResponseCode.TransactionIgnore);
                }


                step = 4;
                // Nếu thành công
                if (cashResult.errorCode == 0)
                {
                  

                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _APIResponse.ResponseContent = cashResult.msg.name;
                   
                }
                else
                {
                    _APIResponse = new APIResponse(CoCoBankLib.ConvertResponCode(cashResult.errorCode));
                   

                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step3", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step4", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "Error", "Cash", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }
            }

           
            return _APIResponse;


        }
        public APIResponse Cash(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderRequest request = new OrderRequest();
            request = serializer.Deserialize<OrderRequest>(transaction.RequestContent);

            APIResponse _APIResponse = new APIResponse();
            var _BankCashAPI = new BankCashAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = CoCoBankLib.GenOrderCode(),
                OrderInfo = string.Empty,
                Amount = request.Amount,
                TotalAmount = request.Amount,
                Currency = "VND",
                ReturnUrl = "",
                RequestTime = 0,
                Signature = "",
                LogContent = "Add Order",
                //Note = request.Note,
                BankCode = request.BankName,
                FullName = request.RefCode,
                Mobile = string.Empty,
                RefCode = request.RefCode,
                BankAccountName = request.BankAccountName,
                BankAccountNumber = request.BankAccountNumber
            };
            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;

                if (request.Amount < 10000 || request.Amount > 20000000)
                {
                    return new APIResponse((int)ResponseCode.BankAmountInvalid);
                }
                if (request.Type == "momocash")
                {
                    if (!CheckValidMobile(request.BankAccountNumber))
                        return new APIResponse((int)ResponseCode.ParameterInvalid);
                }
                // Bước: Ghi log giao dịch
                step = 2;

                _BankCashAPI.ReturnValue = _BankCashAPI.Add();
                // Nếu thêm giao dịch không hợp lệ
                if (_BankCashAPI.ReturnValue < 0)
                {
                    if (_BankCashAPI.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.BankAccountNumber, request.RefCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse((int)_BankCashAPI.ReturnValue);
                }

                // Bước: gọi hàm sang API
                step = 3;


                CashRespone cashResult = new CashRespone();
                if (request.Type == "momocash")
                {
                    CashRequest _cashRequest = new CashRequest();
                    _cashRequest.userName = this.username;
                    _cashRequest.requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    _cashRequest.accountReceive = request.BankAccountNumber;
                    //_cashRequest.comment = request.Note;
                    _cashRequest.amount = request.Amount;
                    _cashRequest.transId = _BankCashAPI.ReturnValue.ToString();
                    _cashRequest.authKey = Utils.Encrypts.MD5(_cashRequest.requestTime + "|" + _cashRequest.transId + "|" + _cashRequest.userName + "|" + _cashRequest.accountReceive + "|" + _cashRequest.amount + "|" + _cashRequest.comment + "|" + secretKey);

                    NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "CoCoRequest", serializer.Serialize(_cashRequest) });
                    cashResult = ProcessCash(_cashRequest);
                }
                else
                {
                    CashBankRequest _cashRequest = new CashBankRequest();
                    _cashRequest.userName = this.username;
                    _cashRequest.requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    _cashRequest.accountId = request.BankAccountNumber;
                    _cashRequest.accountName = request.BankAccountName;
                    _cashRequest.shortBankName = request.BankName;
                    //_cashRequest.comment = request.Note;
                    _cashRequest.amount = request.Amount;
                    _cashRequest.transId = _BankCashAPI.ReturnValue.ToString();
                    _cashRequest.authKey = Utils.Encrypts.MD5(_cashRequest.requestTime + "|" + _cashRequest.transId + "|" + _cashRequest.userName + "|" + _cashRequest.accountId + "|" + _cashRequest.accountName + "|" + _cashRequest.shortBankName + "|" + _cashRequest.amount + "|" + _cashRequest.comment + "|" + secretKey);

                    NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "CoCoRequest", serializer.Serialize(_cashRequest) });
                    cashResult = ProcessBankCash(_cashRequest);
                    //return new APIResponse((int)ResponseCode.TransactionIgnore);
                }


                step = 4;
                // Nếu thành công
                if (cashResult.errorCode == 0)
                {
                    var orderRes = new CashResponeApi()
                    {
                        Status = "Success",
                        Amount = request.Amount,
                        RefCode = request.RefCode,
                        OrderNo = _BankCashAPI.OrderNo.ToString(),
                        Timeout = 120,
                        BankName = request.BankName,
                        BankAccountNumber = cashResult.msg.accountReceive,
                        BankAccountName = cashResult.msg.accountName
                    };

                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _APIResponse.ResponseContent = serializer.Serialize(orderRes);
                    _BankCashAPI.LogContent = "Update Order";
                    _BankCashAPI.Status = 1;

                    Action<string, long, string, long> send = UpdatePartnerBalance;
                    var asynSend = send.BeginInvoke(_BankCashAPI.PartnerCode, request.Amount, _BankCashAPI.BankCode, _BankCashAPI.TransactionID, null, null);
                }
                else
                {
                    _APIResponse = new APIResponse(CoCoBankLib.ConvertResponCode(cashResult.errorCode));
                    _BankCashAPI.Status = _APIResponse.ResponseCode;
                    _BankCashAPI.LogContent = serializer.Serialize(cashResult);

                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step3", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step4", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "CoCoBank", transaction.TransactionID.ToString(), "Error", "Cash", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _BankCashAPI.Status = _APIResponse.ResponseCode;
            }

            if (_BankCashAPI.TransactionID > 0)
            {
                _BankCashAPI.LastTime = DateTime.Now;
                
                _BankCashAPI.Update();
            }
            return _APIResponse;


        }
        private CashRespone ProcessCash(CashRequest requestData)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string responseData = CoCoBankLib.PostJson(ServiceUrl, serializer.Serialize(requestData));
            NLogLogger.Info(new string[] { "CoCoBank", requestData.transId.ToString(), "CoCoBankResponseRaw", responseData });
            return serializer.Deserialize<CashRespone>(responseData);
        }
        private MobileRespone ProcessCheck(MobileRequest requestData)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string responseData = CoCoBankLib.PostJson(ServiceUrlCheck, serializer.Serialize(requestData));
            NLogLogger.Info(new string[] { "CoCoBank", "CoCoBankResponseRaw", responseData });
            return serializer.Deserialize<MobileRespone>(responseData);
        }
        private CashRespone ProcessBankCash(CashBankRequest requestData)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string responseData = CoCoBankLib.PostJson(ServiceBankUrl, serializer.Serialize(requestData));
            NLogLogger.Info(new string[] { "CoCoBank", requestData.transId.ToString(), "CoCoBankResponseRaw", responseData });
            return serializer.Deserialize<CashRespone>(responseData);
        }
        private void UpdatePartnerBalance(string PartnerCode, long Amount, string Type, long TranId)
        {
            NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Type, TranId.ToString() });
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
            if (listpartnerDiscount == null)
                return;
            if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                return;

            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
            decimal ck = _partnerDiscount.DiscountBANKTRANFER;
            if (Type == "MOMO")
                ck = _partnerDiscount.DiscountMOMO;
            if (ck == 0)
                return;

            long realAmount = Convert.ToInt64(Amount * ck);
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Partners().Deduct(realAmount, PartnerCode);

        }
    }
}
