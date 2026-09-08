using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;

namespace Libs.BankDirect
{

    public class BankDirectV2Service : IServiceHandler
    {

        public APIResponse Request(APITransaction transaction)
        {
            switch (transaction.CommandCode.ToLower())
            {
                case "getbanks":
                    return GetBanksV2(transaction);
                case "getbanksv2":
                    return GetBanksV2(transaction);
                case "checktrans":
                    return new APIResponse();
                case "order":
                    return Order(transaction);
                default:
                    return new APIResponse((int)API.ResponseCode.AccessDenied);
            }
        }

        public class GetBankRequest
        {
            public string Type { get; set; }

        }
        public class GetBankRequestV2
        {
            public string Type { get; set; }
            public string AccountName { get; set; }
        }
        public class CheckOrderRequest
        {
            public string OrderNo { get; set; }

        }

        public class OrderRequest
        {
            public string Type { get; set; }
            public string AccountName { get; set; }
            public string BankName { get; set; }
            public string BankAccountName { get; set; }
            public string BankAccountNumber { get; set; }
            //public string AppCode { get; set; }
            public string RefCode { get; set; }
            public int Amount { get; set; }
            public string CallbackUrl { get; set; }
        }

        //To do : here
        private APIResponse GetBanksV2(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Phân tích yêu cầu thành đối tượng UseCardRequest đặt các điều kiện checking tại đây
            //var request = new GetBankRequestV2();
            APIResponse result = null;
            try
            {
                //var request = serializer.Deserialize<GetBankRequestV2>(transaction.RequestContent);

                //Switch Provider
                var providerCode = new Providers().GetBankCondition(transaction.RequestContent, transaction.PartnerCode);
                if (providerCode.Equals("CPI"))
                {
                    return new APIResponse((int)ResponseCode.ProviderNotFound);
                }
                //NLogLogger.Info(new string[] { "GetBanksV2", providerCode });
                var handler = BankDirectV2Factory.GetHandler(providerCode);

                string responseData = string.Empty;

                result = handler.GetBanksV2(transaction.PartnerCode.ToString(), "test");
                //NLogLogger.Info(new string[] { "GetBanksV2", transaction.TransactionID.ToString(), "Response", serializer.Serialize(result) });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetBanksV2", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

            return result;
        }
        private APIResponse GetBanks(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Phân tích yêu cầu thành đối tượng UseCardRequest đặt các điều kiện checking tại đây
            var request = new GetBankRequest();
            APIResponse result = null;
            try
            {
                request = serializer.Deserialize<GetBankRequest>(transaction.RequestContent);

                //Switch Provider
                var providerCode = new Providers().GetBankCondition(request.Type, transaction.PartnerCode);
                //if (providerCode.Equals("CPI"))
                //{
                //    if (DateTime.Now.Hour >= 4)
                //    {
                //        providerCode = "imobank";
                //    }
                //    else
                //    {

                //    }
                //}
                //if (DateTime.Now.Hour <= 2)
                //{
                //    return new APIResponse((int)ResponseCode.SystemMaintain);
                //}
                var handler = BankDirectV2Factory.GetHandler(providerCode);

                string responseData = string.Empty;

                result = handler.GetBanks();
                NLogLogger.Info(new string[] { "GetBanks", transaction.TransactionID.ToString(), "Response", serializer.Serialize(result) });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetBanks", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

            return result;
        }

        private APIResponse Order(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Phân tích yêu cầu thành đối tượng UseCardRequest đặt các điều kiện checking tại đây
            var request = new OrderRequest();
            APIResponse result = null;
            try
            {
                request = serializer.Deserialize<OrderRequest>(transaction.RequestContent);

                //bool isNull = request.GetType().GetProperties().All(p => p.GetValue(request) != null);
                //if (!isNull)
                //{
                //    NLogLogger.Info(new string[] { "Order", transaction.TransactionID.ToString(), "Resquest some thing null: ", serializer.Serialize(transaction.RequestContent) });
                //    return new APIResponse((int)ResponseCode.ParameterInvalid);
                //}


                //Switch Provider
                var providerCode = new Providers().GetBankCondition(request.Type, transaction.PartnerCode);
                if (providerCode.Equals("CPI"))
                {
                    return new APIResponse((int)ResponseCode.SystemMaintain);

                    //providerCode = "kenbankcash";

                }
                //if (transaction.PartnerCode != "dcp")
                //{
                //    if (transaction.RequestContent.Contains("VCB"))
                //    {
                //        providerCode = "kenbank";
                //    }

                //    if (transaction.PartnerCode == "mjqk")
                //    {
                //        if (transaction.RequestContent.Contains("ACB"))
                //        {
                //            providerCode = "kenbank";
                //        }
                //        if (transaction.RequestContent.Contains("VPB"))
                //        {
                //            providerCode = "kenbank";
                //        }
                //        if (transaction.RequestContent.Contains("VCB"))
                //        {
                //            providerCode = "kenbank";
                //        }
                //    }


                //    if (transaction.RequestContent.Contains("BIDV"))
                //    {
                //        providerCode = "kenbank";
                //    }
                   
                //    //if (transaction.RequestContent.Contains("random"))
                //    //{
                //    //    providerCode = "pushbank";
                //    //}
                //    //if (transaction.RequestContent.Contains("VIETINBANK"))
                //    //{
                //    //    providerCode = "kenbank";
                //    //}
                //    //if (transaction.RequestContent.Contains("TCB"))
                //    //{
                //    //    providerCode = "bpaybank";
                //    //}
                //    if (transaction.RequestContent.Contains("MB"))
                //    {
                //        providerCode = "imobank";
                //    }
                //}

                //if (transaction.RequestContent.Contains("ACB"))
                //    providerCode = "kenbank";
                //VPB
                //MSB


                //if (transaction.RequestContent.Contains("VCB"))
                //    providerCode = "toxbank";
                if (providerCode.Equals("CPI"))
                {
                    return new APIResponse((int)ResponseCode.BankCodeInvalid);
                }

                transaction.ProviderCode = providerCode;
                var handler = BankDirectV2Factory.GetHandler(providerCode);
                result = handler.Order(transaction);
                //NLogLogger.Info(new string[] { "Order", transaction.TransactionID.ToString(), "Response", serializer.Serialize(result) });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Order", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

            return result;
        }


    }




}
