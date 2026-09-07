using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;

namespace Libs.TopupPartner.PayPlusOrder
{

    public class PayPlusOrderBizBuyService : IBuyCardHandler
    {

        //protected string ServiceUrl = "http://127.0.0.1:1684/default.aspx"; // Service Test

        protected string ServiceUrl = "http://127.0.0.1:1588/"; // Service Production

        //protected string ProviderCode = "pporder";
        //protected string PartnerCode = "";
        protected string PartnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        public APIResponse downloadSoftpin(string requestId, string provider, int amount, int quantity,
            string partnerCode, string providerCode, ref string providerResponse)
        {

            switch (provider.ToUpper())
            {
                case "VTT":
                    provider = "vtt";
                    break;
                case "VMS":
                    provider = "vms";
                    break;
                case "VNP":
                    provider = "vnp";
                    break;
            }


            CashRequest requestData = new CashRequest();
            requestData.transactionId = requestId;
            requestData.amount = amount;
            requestData.quantity = quantity;
            requestData.telco = provider;
            requestData.providerCode = providerCode;
            requestData.partnerCode = partnerCode;

            NLogLogger.Info(new string[] { "PayPlusOrderBizService", "downloadSoftpin", "Request", serializer.Serialize(requestData) });
            var tryAgain = 0;
            CashResponse responseData = ProcessCard(requestData);
            //Gọi lại nếu bị bị nhà mạng khóa
            while ((responseData.code == (int)ResponseCode.ServiceIsLocked && tryAgain < 3))
            {
                responseData = ProcessCard(requestData);
                tryAgain++;
                Thread.Sleep(1000);
            }

            providerResponse = string.Empty;
            NLogLogger.Info(new string[] { "PayPlusOrderBizService", "downloadSoftpin", "Response", serializer.Serialize(responseData) });
            if (responseData != null)
            {

                List<CardDVO> cardDvos = new List<CardDVO>();
                if (responseData.code == 1)
                {

                    var listCard = serializer.Deserialize<List<CardPPOrg>>(responseData.content);


                    foreach (var l in listCard)
                    {
                        var cd = new CardDVO();
                        cd.Serial = l.Serial;
                        cd.Pin = l.Pin;
                        cd.ExpireDate = DateTime.Now.AddYears(1);
                        cardDvos.Add(cd);
                    }

                }

                return ConvertResultCode(responseData, cardDvos);
            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public int checkStore(string provider, int amount)
        {
            return 0;
        }

        public APIResponse ConvertResultCode(CashResponse result, IList<CardDVO> listCard)
        {

            switch (result.code)
            {
                case 1:

                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = serializer.Serialize(listCard)
                    };

                case -320:
                    return new APIResponse((int)ResponseCode.TransactionNotExists)
                    {
                        ResponseContent = result.message
                    };

                case -326:
                    return new APIResponse((int)ResponseCode.TransactionTimeout)
                    {
                        Description = result.message
                    };

                case -371:
                    return new APIResponse((int)ResponseCode.CardOutOfStock)
                    {
                        ResponseContent = result.message
                    };

                default:
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        ResponseContent = string.Empty,
                        Description = result.message
                    };



            }
        }

        private CashResponse ProcessCard(CashRequest cashRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[] { "PayPlusOrderBizService", "ProcessCard", "Request", serializer.Serialize(cashRequest) });
            APIGameService _VPGService = new APIGameService(ServiceUrl + "APIGame.asmx");
            _VPGService.Timeout = 600000;
            string serviceResponse = string.Empty;
            try
            {
                serviceResponse = _VPGService.RequestByCard(cashRequest.transactionId, cashRequest.partnerCode, cashRequest.providerCode, cashRequest.telco, cashRequest.amount, cashRequest.quantity, cashRequest.clientId);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "PayPlusOrderBizService", "ProcessCard", "Exception", e.Message });
                return new CashResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The operation has timed out",
                    content = ""

                };
            }

            NLogLogger.Info(new string[] { "PayPlusOrderBizService", "ProcessCard", "Response", serviceResponse });
            return serializer.Deserialize<CashResponse>(serviceResponse);
        }


    }

    public class PayPlusOrderBizTopupService : ITopupHandler
    {
        protected string ServiceUrl = "http://localhost:1588/"; // Service Production
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        private TranferResponse ProcessTranfer(TranferRequest tranferRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[]
                { "PayPlusOrderBizService", "ProcessTranfer", "Request", serializer.Serialize(tranferRequest) });
            APIGameService _VPGService = new APIGameService(ServiceUrl + "APIGame.asmx");
            _VPGService.Timeout = 600000;
            string serviceResponse = string.Empty;
            try
            {
                serviceResponse = _VPGService.RequestTranferBalance(tranferRequest.transactionId.ToString(), tranferRequest.partnerCode, tranferRequest.providerCode, tranferRequest.telco, tranferRequest.amount, tranferRequest.simTarget, tranferRequest.clientId);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "PayPlusOrderBizService", "ProcessTranfer", "Exception", e.Message });
                return new TranferResponse()
                {
                    code = (int)ResponseCode.TransactionTimeout,
                    message = "The operation has timed out",
                };
            }

            NLogLogger.Info(new string[] { "PayPlusOrderBizService", "ProcessTranfer", "Response", serviceResponse });
            return serializer.Deserialize<TranferResponse>(serviceResponse);
        }

        public APIResponse TranferBalance(string requestId, string provider, int amount, string simTarget, string partnerCode, string providerCode)
        {

            switch (provider.ToUpper())
            {
                case "VTT":
                    provider = "vtt";
                    break;
                case "VMS":
                    provider = "vms";
                    break;
                case "VNP":
                    provider = "vnp";
                    break;
            }


            TranferRequest requestData = new TranferRequest();
            requestData.transactionId = Convert.ToInt64(requestId);
            requestData.amount = amount;
            requestData.simTarget = simTarget;
            requestData.telco = provider;
            requestData.providerCode = providerCode;
            requestData.partnerCode = partnerCode;

            NLogLogger.Info(new string[] { "PayPlusOrderBizService", "TranferBalance", "Request", serializer.Serialize(requestData) });
            var tryAgain = 0;
            TranferResponse responseData = ProcessTranfer(requestData);
            //Gọi lại nếu bị bị nhà mạng khóa
            while ((responseData.code == (int)ResponseCode.ServiceIsLocked || responseData.code == (int)ResponseCode.TransactionLimit) && tryAgain < 3)
            {
                responseData = ProcessTranfer(requestData);
                tryAgain++;
                Thread.Sleep(1000);
            }

            NLogLogger.Info(new string[] { "PayPlusBuyCard", "TranferBalance", "Response", serializer.Serialize(responseData) });
            if (responseData != null)
            {
                if (responseData.code == (int)ResponseCode.TransactionSuccessful)
                {
                    return new APIResponse((int)ResponseCode.TransactionSuccessful);
                }
                else if (responseData.code == (int)ResponseCode.SystemBusy)
                {
                    return new APIResponse((int)ResponseCode.SystemBusy);
                }
            }

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
    }
}
