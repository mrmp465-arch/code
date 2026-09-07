using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Libs.API
{
    public enum ResponseCode : int
    {
        TransactionSuccessful = 1,
        TransactionSuccessfullNotConfirmYet = 2,
        TransactionRefun = 3,
        TransactionWaitting = 6,
        TransactionFailed = -1,
        TransactionSuspicious = -2,
        PaymentConnectionFailed = -3,
        TransactionConfirmed = -4,
        TransactionCancel = -5,
        CallbackFailed = -6,
        TransactionRejected = -7,
        TransactionPending = -8,
        TransactionProcessing = -9,
        OtpRequired = -11,
        AccountLocked = -49,
        AccountNotExists = -50,
        BalanceNotEnough = -51,
        AccountNameReuired = -52,
        RefCodeReuired = -53,
        IpInvalid = -54,
        LoginFail = -55,

        SystemMaintain = -300,
        SystemError = -301,
        UndefinedError = -302,
        SystemBusy = -303,
        AccessDenied = -310,
        PartnerNotExistsNotActive = -312,
        SignatureInvalid = -313,
        ServiceIsLocked = -314,
        ServiceIsPause = -315,
        ServiceNotExists = -317,
        RequestContentInvalid = -316,
        TransactionDuplicate = -319,
        TransactionNotExists = -320,
        TransactionExpired = -321,
        TransactionInvalid = -322,
        ParameterInvalid = -323,
        TransactionLimit = -324,
        TransactionReview = -325,
        TransactionTimeout = -326,
        ProviderNotFound = -327,
        TransactionIgnore = -328,

        //Mã lỗi gạch thẻ
        CardUsed = -330,
        CardIsLocked = -331,
        CardHasExpired = -332,
        CardNotActivated = -333,
        CardSerialInvalid = -334,
        CardCodeInvalid = -335,
        CardTypeInvalid = -336,
        CardFormatInvalid = -337,
        CardProcessing = 0,
        OrderNotFound = -329,
        // Mã lối ngân hàng
        BankCardInfoInvalid = -350,
        BankOtpInvalid = -351,
        BankCodeInvalid = -352,
        BankServiceNotActive = -354,
        BankAmountLimit = -355,
        BankAmountInvalid = -356,
        BankAccountInvalid = -357,
        BankCodeMaintain = -358,
        //BalanceNotEnough = -359,
        //Mã lỗi BuyCard
        CardQuantityLimit = -370,
        CardOutOfStock = -371,
        CardAmountInvalid = -372,
        CardProviderInvalid = -373,

        //Mã lỗi USD
        SimNotActive = -401
    }

    public class APIResponse
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }

        public APIResponse()
        {

        }

        public APIResponse(int responseCode)
        {
            ResponseCode = responseCode;
            Description = ResponseUtils.Description(ResponseCode);
            ResponseContent = "";
            Signature = "";
        }
    }

    public class ResponseUtils
    {
        public static string Description(int responseCode)
        {
            switch (responseCode)
            {
                case (int)ResponseCode.TransactionSuccessful:
                    return "Transaction is successful";
                case (int)ResponseCode.TransactionCancel:
                    return "Transaction is cancel";
                case (int)ResponseCode.SystemError:
                    return "System provider error";
                case (int)ResponseCode.SystemMaintain:
                    return "System maintain";
                case (int)ResponseCode.SystemBusy:
                    return "System busy try again later";
                case (int)ResponseCode.TransactionFailed:
                    return "Transaction failed";
                case (int)ResponseCode.TransactionReview:
                    return "Transaction review";
                case (int)ResponseCode.TransactionPending:
                    return "Transaction pending";
                case (int)ResponseCode.TransactionProcessing:
                    return "Transaction processing";
                case (int)ResponseCode.PaymentConnectionFailed:
                    return "Payment connection Failed";
                case (int)ResponseCode.SignatureInvalid:
                    return "Signature invalid";
                case (int)ResponseCode.ServiceIsLocked:
                    return "Service is locked";
                case (int)ResponseCode.ServiceIsPause:
                    return "Service is pause";
                case (int)ResponseCode.ServiceNotExists:
                    return "Service not exists";
                case (int)ResponseCode.RequestContentInvalid:
                    return "RequestContent invalid";
                case (int)ResponseCode.TransactionDuplicate:
                    return "Duplicate TransactionID";
                case (int)ResponseCode.TransactionNotExists:
                    return "Transaction not exists";
                case (int)ResponseCode.TransactionInvalid:
                    return "Transaction invalid";
                case (int)ResponseCode.TransactionTimeout:
                    return "Transaction timeout";
                case (int)ResponseCode.TransactionSuspicious:
                    return "Transaction suspicious";
                case (int)ResponseCode.TransactionLimit:
                    return "Transaction limit";
                case (int)ResponseCode.TransactionIgnore:
                    return "Transaction ignore";
                case (int)ResponseCode.AccessDenied:
                    return "Access denied";
                case (int)ResponseCode.ParameterInvalid:
                    return "Parameter invalid";
                case (int)ResponseCode.BalanceNotEnough:
                    return "Balance is not enough";
                case (int)ResponseCode.CardUsed:
                    return "Card used or does not exist";
                case (int)ResponseCode.CardIsLocked:
                    return "Card is locked";
                case (int)ResponseCode.CardHasExpired:
                    return "CardHasExpired";
                case (int)ResponseCode.CardNotActivated:
                    return "Card not activated";
                case (int)ResponseCode.CardSerialInvalid:
                    return "Card serial invalid";
                case (int)ResponseCode.CardCodeInvalid:
                    return "Card code invalid";
                case (int)ResponseCode.CardProcessing:
                    return "Card processing";
                case (int)ResponseCode.AccountNotExists:
                    return "Account not exists";
                case (int)ResponseCode.AccountLocked:
                    return "Account is locked";
                case (int)ResponseCode.AccountNameReuired:
                    return "AccountName is required";
                case (int)ResponseCode.RefCodeReuired:
                    return "RefCode is required";
                case (int)ResponseCode.CardQuantityLimit:
                    return "Quantity cards purchased over limit";
                case (int)ResponseCode.CardOutOfStock:
                    return "Card out of stock";
                case (int)ResponseCode.CardAmountInvalid:
                    return "Card amount invalid";
                case (int)ResponseCode.CardProviderInvalid:
                    return "Card provider invalid";
                case (int)ResponseCode.CardTypeInvalid:
                    return "Card type invalid";
                case (int)ResponseCode.CardFormatInvalid:
                    return "Card format invalid";
                case (int)ResponseCode.ProviderNotFound:
                    return "Provider not found";
                case (int)ResponseCode.PartnerNotExistsNotActive:
                    return "Partner not exists or not active";
                case (int)ResponseCode.IpInvalid:
                    return "IP Invalid";
                case (int)ResponseCode.LoginFail:
                    return "Login Failed";
                case (int)ResponseCode.TransactionRejected:
                    return "Transaction Rejected";
                case (int)ResponseCode.TransactionConfirmed:
                    return "Transaction Confirmed Before";
                case (int)ResponseCode.TransactionSuccessfullNotConfirmYet:
                    return "Transaction Success But Not Confirm Yet";
                case (int)ResponseCode.TransactionRefun:
                    return "Transaction Refun";
                case (int)ResponseCode.CallbackFailed:
                    return "Callback Failed";
                case (int)ResponseCode.SimNotActive:
                    return "Sim Not Active";
                case (int)ResponseCode.UndefinedError:
                    return "Error undefined";
                case (int)ResponseCode.BankAmountInvalid:
                    return "Bank amount invalid";
                case (int)ResponseCode.BankCodeMaintain:
                    return "Bank Code Maintain";
                default:
                    return "";
            }
        }

        public static string Get(APIResponse apiResponse)
        {
            if (string.IsNullOrEmpty(apiResponse.Description))
            {
                apiResponse.Description = Description(apiResponse.ResponseCode);
            }
            if (string.IsNullOrEmpty(apiResponse.ResponseContent))
            {
                apiResponse.ResponseContent = "";
            }
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(apiResponse);
        }

        public static string Response(int responseCode)
        {
            APIResponse apiResponse = new APIResponse(responseCode);
            return new JavaScriptSerializer().Serialize(apiResponse);
        }

        public static string Get(APIResponse apiResponse, Partners partner)
        {
            if (string.IsNullOrEmpty(apiResponse.Description))
            {
                apiResponse.Description = Description(apiResponse.ResponseCode);
            }

            if (string.IsNullOrEmpty(apiResponse.ResponseContent))
            {
                apiResponse.ResponseContent = "";
            }

            // Bổ sung thêm phần chữ ký
            apiResponse.Signature = "";

            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(apiResponse);
        }
    }
}
