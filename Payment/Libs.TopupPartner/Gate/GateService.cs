using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;

namespace Libs.TopupPartner.Gate
{
    public class GateService : IBuyCardHandler
    {

        //Sandbox
        //protected string ServiceUrl = "http://sandbox.gate.vn:47474/daily/api/multicard/ws.asmx";
        //protected string Username = "0903886637";
        //protected string Password = "123456";

        protected string ServiceUrl = "https://daily.gate.vn/api/multicard/ws.asmx";
        protected string Username = "0986909640";
        protected string Password = "@Meoicondoi2018";

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        public APIResponse downloadSoftpin(string requestId, string provider, int amount, int quantity, string partnerCode, string providerCode, ref string providerResponse)
        {


            switch (provider.ToUpper())
            {
                case "VIETTEL":
                case "VTT":
                    provider = "VIETTEL";
                    break;
                case "VMS":
                    provider = "MOBI";
                    break;
                case "VNP":
                    provider = "VINA";
                    break;
                case "VNM":
                    provider = "VN MOBI";
                    break;
                case "GMB":
                    provider = "BEELINE";
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
                case "ECASH":
                    provider = "ECASH";
                    break;
                case "GARENA":
                    provider = "GARENA";
                    break;
            }

            RequestData requestData = new RequestData();
            requestData.CardName = provider;
            requestData.Amount = amount;
            requestData.Quantity = quantity;
            requestData.PartnerTranID = requestId;
            requestData.Username = Username;
            requestData.Password = Password;

            int returnCode = 0;
            string message = string.Empty;
            decimal balance = 0;
            string transID = string.Empty;

            var responseData = ProcessRequest(requestData, ref returnCode, ref message, ref balance, ref transID);

            ResultRefData resultRefData = new ResultRefData()
            {
                ReturnCode = returnCode,
                Message = message,
                Balance = balance,
                TransID = transID
            };
            providerResponse = serializer.Serialize(resultRefData);
            NLogLogger.Info(new string[] { "GateBuyCard", "GateBuyCardResponse", serializer.Serialize(resultRefData) });

            Encryption.GEncryption en = new Encryption.GEncryption();
            IList<CardDVO> cardDvos = new List<CardDVO>();
            if (resultRefData.ReturnCode == 0)
            {

                cardDvos = responseData.AsEnumerable().Select(row =>
                    new CardDVO
                    {
                        Serial = row.Field<string>("Serial"),
                        Pin = en.GDecrypt(row.Field<string>("Pin")),
                        ExpireDate = row.Field<DateTime>("ExpireDate")
                    }).ToList();
            }
            return ConvertResultCode(resultRefData, cardDvos);
        }

        public APIResponse ConvertResultCode(ResultRefData result, IList<CardDVO> listCard)
        {

            switch (result.ReturnCode)
            {
                case 0:

                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = serializer.Serialize(listCard)
                    };
                default:
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        ResponseContent = string.Empty,
                        Description = result.Message
                        
                    };



            }
        }


        public class RequestData
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string CardName { get; set; }
            public Decimal Amount { get; set; }
            public int Quantity { get; set; }
            public string PartnerTranID { get; set; }

        }

        public class ResultRefData
        {
            public int ReturnCode { get; set; }
            public string Message { get; set; }
            public decimal Balance { get; set; }
            public string TransID { get; set; }

        }

        public class CardDVO
        {
            public string Serial { get; set; }
            public string Pin { get; set; }
            public DateTime ExpireDate { get; set; }
        }

        public DataTable ProcessRequest(RequestData requestData, ref int returnCode, ref string message, ref decimal balance, ref string transID)
        {
            NLogLogger.Info(new string[] { "GateBuyCard", "GateBuyCardRequest", serializer.Serialize(requestData) });
            ws _gateService = new ws(ServiceUrl);
            var serviceResponse = _gateService.BuyCard(requestData.Username, requestData.Password, requestData.CardName,
                requestData.Amount, requestData.Quantity, ref returnCode, ref message, ref balance, ref transID,
                requestData.PartnerTranID);
            //NLogLogger.Info(new string[] { "GateBuyCard", "GateBuyCardResponse", serializer.Serialize(serviceResponse) });
            return serviceResponse;

        }

        public int checkStore(string provider, int amount)
        {
            throw new NotImplementedException();
        }

    }


}
