using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.TopupPartner.VGG;
using Libs.Utils;

namespace Libs.TopupPartner.XBom
{
    public class VPGService : IBuyCardHandler
    {

        protected string urlService = "http://27.118.16.46:1581/VPGService.asmx";
        protected string partnerKey = "fe6c36987d38baca22df84dcd92d056c";
        protected string partnerCode = "a68bc";
        protected string serviceCode = "buycard";
        protected string commandCode = "buycard";

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public APIResponse downloadSoftpin(string requestId, string provider, int amount, int quantity, string partnerCode, string providerCode, ref string providerResponse)
        {
            switch (provider.ToUpper())
            {
                case "VTT":
                    provider = "VTT";
                    break;
                case "VMS":
                    provider = "VMS";
                    break;
                case "VNP":
                    provider = "VNP";
                    break;
                case "VNM":
                    provider = "VNM";
                    break;
                case "GMB":
                    provider = "GMB";
                    break;
                case "SFO":
                    provider = "SFO";
                    break;
                case "ZING":
                    provider = "ZING";
                    break;
                case "GATE":
                    provider = "GATE";
                    break;
                case "VCOIN":
                    provider = "VCOIN";
                    break;

            }

            string requestContent = serializer.Serialize(new RequestData()
            {
                Provider = provider,
                Amount = amount,
                Quantity = quantity,
                AccountName = requestId,
                OrderNo = requestId
            });

            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            var responseData = ProcessRequest(partnerCode, serviceCode, commandCode, requestContent, signature);
            providerResponse = responseData;
            var response = serializer.Deserialize<ResultData>(responseData);
            return ConvertResultCode(response);
        }

        public APIResponse ConvertResultCode(ResultData result)
        {

            // 2 hệ thống giống nhau nên Code và Message dùng chung :)) và ko cần chuyển về CardDVO để Serialize
            switch (result.ResponseCode)
            {
                case 1:
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = result.ResponseContent
                    };
                default:
                    return new APIResponse(result.ResponseCode)
                    {
                        ResponseContent = string.Empty
                    };
            }
        }

        //public int checkStore(string provider, int amount)
        //{
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    VGGService.RequestData requestData = new VGGService.RequestData();
        //    requestData.FunctionName = "checkstore";
        //    requestData.PartnerCode = PartnerCode;
        //    requestData.ProviderCode = provider;
        //    requestData.Amount = amount;
        //    requestData.RequestTime = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        //    string data = requestData.OrderNo + requestData.PartnerCode + requestData.ProviderCode + requestData.Amount.ToString() + requestData.Quantity + requestData.RequestTime.ToString() + PartnerKey;
        //    requestData.Signature = Encrypts.MD5(data);
        //    string responseData = PostData(ServiceUrl, serializer.Serialize(requestData));
        //    return int.Parse(responseData);
        //}

        public string ProcessRequest(string partnerCode, string serviceCode, string commandCode, string requestContent, string signature)
        {
            NLogLogger.Info(new string[] { "XBomBuyCard", "XBomBuyCardRequest", requestContent });
            VPGAPIService _vpgService = new VPGAPIService(urlService);
            var serviceResponse = _vpgService.Request(partnerCode, serviceCode, commandCode, requestContent, signature);
            NLogLogger.Info(new string[] { "XBomBuyCard", "XBomBuyCardResponse", serializer.Serialize(serviceResponse) });
            return serviceResponse;
        }

        public int checkStore(string provider, int amount)
        {
            throw new NotImplementedException();
        }

        public APIResponse TranferBalance(string requestId, string provider, int amount, string simTarget, string partnerCode, string providerCode)
        {
            throw new NotImplementedException();
        }
    }

    public class CardDVO
    {
        public string Serial { get; set; }
        public string Pin { get; set; }
        public DateTime ExpireDate { get; set; }
    }
    public class RequestData
    {
        public string Provider { get; set; } // CardType
        public int Amount { get; set; }
        public int Quantity { get; set; }
        public string AccountName { get; set; }
        public long AccountId { get; set; }
        public string OrderNo { get; set; }

    }

    public class ResultData
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }


}

