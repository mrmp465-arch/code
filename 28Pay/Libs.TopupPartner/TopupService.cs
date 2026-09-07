using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Db;
using Libs.TopupPartner.PayDirect;
using Libs.TopupPartner.VGG;
using Libs.TopupPartner.VNPTEPAY;
using Libs.Utils;
using Libs.Report;
using System.Linq;

namespace Libs.TopupPartner
{
    //public class BuyCardService : APIService
    public class TopupService : IServiceHandler
    {

        //public override APIResponse Request(APITransaction transaction)
        public APIResponse Request(APITransaction transaction)
        {
            switch (transaction.CommandCode)
            {
                case "tranfer":
                    return Tranfer(transaction);
                default:
                    return new APIResponse((int)ResponseCode.AccessDenied);
            }
        }

       
        public class TranferRequest
        {
            public string Provider { get; set; } // CardType
            public int Amount { get; set; }
            public int Quantity { get; set; }
            public string SimTarget { get; set; }
            public string AccountName { get; set; }
            public long AccountId { get; set; }
            public string OrderNo { get; set; }
        }

        private APIResponse Tranfer(APITransaction transaction)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();


            TranferRequest request = new TranferRequest();
            BuyCardAPILog byCardApiLog = new BuyCardAPILog();
            try
            {
                //Get Provider Switch dịch vụ bằng đoạn code này

                request = serializer.Deserialize<TranferRequest>(transaction.RequestContent);

                //Check Parnter Quota
                var partnerStatus = new Partners().GetBuyCardCondition(transaction.PartnerID, transaction.ServiceID);
                switch (partnerStatus)
                {
                    case "F":
                        return new APIResponse((int)ResponseCode.TransactionLimit);
                    case "I":
                        return new APIResponse((int)ResponseCode.ServiceIsLocked);
                }


                //Switch Provider
                var providerCode = new Providers().GetTopupMobileCondition(request.Provider, transaction.PartnerCode);
                if (providerCode.Equals("CPI"))
                {
                    return new APIResponse((int)ResponseCode.CardProviderInvalid);
                }

                //Ghi Log BuyCard
                byCardApiLog.TransactionID = transaction.TransactionID;
                byCardApiLog.TransactionNo = transaction.TransactionID.ToString();
                byCardApiLog.PartnerID = transaction.PartnerID;
                byCardApiLog.PartnerCode = transaction.PartnerCode;
                byCardApiLog.ProviderCode = providerCode;
                byCardApiLog.AccountName = request.AccountName;
                byCardApiLog.AccountId = request.AccountId;
                byCardApiLog.OrderNo = request.OrderNo;
                //byCardApiLog.RequestTime = 0;
                byCardApiLog.Provider = request.Provider;
                byCardApiLog.Amount = request.Amount;
                byCardApiLog.Quantity = 1;
                byCardApiLog.SimTarget = request.SimTarget;
                //byCardApiLog.ErrorCode = -1; // Khoi tao GD
                byCardApiLog.Message = string.Empty;
                byCardApiLog.LogContent = string.Empty;
                byCardApiLog.ListCards = string.Empty;
                //byCardApiLog.CreatedTime = DateTime.Now;
                //byCardApiLog.LastTime = DateTime.Now;
                byCardApiLog.Status = 0;
                byCardApiLog.Add();
                // Nếu thêm giao dịch không hợp lệ
                if (byCardApiLog.ReturnValue < 0)
                {
                    return new APIResponse(byCardApiLog.ReturnValue);
                }

                var handler = TopupFactory.GetHandler(providerCode);
                //NLogLogger.Info(new string[] { "BuyCard", transaction.TransactionID.ToString(), "Info", "ProviderCode", providerCode });
                //PayDirectService payDirectService = new PayDirectService();
                //var result = payDirectService.downloadSoftpin(byCardApiLog.TransactionNo, byCardApiLog.Provider, byCardApiLog.Amount, byCardApiLog.Quantity);
                string responseData = string.Empty;
                APIResponse result = null;
                try
                {
                    result = handler.TranferBalance(byCardApiLog.TransactionNo, byCardApiLog.Provider, byCardApiLog.Amount, byCardApiLog.SimTarget, byCardApiLog.PartnerCode, byCardApiLog.ProviderCode);
                }
                catch (Exception ex)
                {
                    byCardApiLog.Status = (int)ResponseCode.SystemError;
                    byCardApiLog.Update();
                    NLogLogger.Info(new string[] { "Tranfer", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                if (result.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {
                    byCardApiLog.Status = (int)ResponseCode.TransactionSuccessful;
                    byCardApiLog.Update();
                }
                else
                {
                    byCardApiLog.Status = result.ResponseCode;
                    byCardApiLog.LogContent = responseData;
                    byCardApiLog.Update();
                }

                return result;

            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Tranfer", transaction.TransactionID.ToString(), "Error", "Deserialize(requestContent)", ex.Message.Replace("\n", " ") });
                byCardApiLog.Status = (int)ResponseCode.SystemError;
                byCardApiLog.Update();
                return new APIResponse((int)ResponseCode.RequestContentInvalid);

            }

        }

    }
}
