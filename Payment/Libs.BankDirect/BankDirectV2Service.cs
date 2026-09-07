using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;
using System.CodeDom;
using Newtonsoft.Json;

namespace Libs.BankDirect
{

    public class BankDirectV2Service : IServiceHandler
    {

        public APIResponse Request(APITransaction transaction)
        {
            switch (transaction.CommandCode.ToLower())
            {
                case "getbanks":
                    return GetBanks(transaction);
                case "getbanksv2":
                    return GetBanksV2(transaction);
                case "checktrans":
                    return CheckTrans(transaction);
                case "order":
                    return Order(transaction);
                case "orderv2":
                    return OrderV2(transaction);
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
        public class OrderRequestV2
        {
            public string Type { get; set; }
            //public string AccountName { get; set; }
            public string BankName { get; set; }
            public string BankAccountName { get; set; }
            public string BankAccountNumber { get; set; }
            public string OrderNo { get; set; }
            public string RefCode { get; set; }
            public int Amount { get; set; }
            public string CallbackUrl { get; set; }
        }
        public class OrderRequest
        {
            public string Type { get; set; }
            //public string AccountName { get; set; }
            public string BankName { get; set; }
            //public string BankAccountName { get; set; }
            //public string BankAccountNumber { get; set; }
            //public string AppCode { get; set; }
            public string RefCode { get; set; }
            public int Amount { get; set; }
            public string CallbackUrl { get; set; }
        }
        private APIResponse CheckTrans(APITransaction transaction)
        {
            var partnercode = transaction.PartnerCode;
            var refcode = transaction.RequestContent;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            APIResponse result = new APIResponse((int)ResponseCode.TransactionFailed);

            NLogLogger.Info(new string[] { "API", "CheckTrans", transaction.RequestContent });

            var checkorder = DataCaching.GetCache<CheckOrder>("CheckOrder:" + partnercode + refcode);
            if (checkorder != null)
            {
                //NLogLogger.Info("check order by cache");
                return new APIResponse((int)ResponseCode.TransactionSuccessful)
                {
                    ResponseContent = serializer.Serialize(checkorder)
                };
            }


            var trans = new BankGateAPI().GetByRefcode(refcode, partnercode);

            if (trans == null)
            {
                return new APIResponse((int)ResponseCode.TransactionNotExists);
            }
            if (trans.Status >= 1)
            {
                var checkOrder = new CheckOrder
                {
                    LasTime = trans.LastTime,
                    RefCode = trans.RefCode,
                    Amount = Convert.ToInt32(trans.TotalAmount),
                    TransactionID = trans.TransactionID.ToString()
                };
                return new APIResponse((int)ResponseCode.TransactionSuccessful)
                {
                    ResponseContent = serializer.Serialize(checkOrder)
                };
            }
            else
            {
                return new APIResponse((int)ResponseCode.CardProcessing);
            }
            //return result;
        }


        //To do : here
        private APIResponse GetBanksV2(APITransaction transaction)
        {
            //return new APIResponse((int)ResponseCode.SystemBusy);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Phân tích yêu cầu thành đối tượng UseCardRequest đặt các điều kiện checking tại đây
            var request = new GetBankRequestV2();
            APIResponse result = null;
            try
            {
                
                request = serializer.Deserialize<GetBankRequestV2>(transaction.RequestContent);
               
                var type = request.Type;
                //Switch Provider
                if (request.Type == "momov2")
                {
                    if (transaction.PartnerCode == "anhx2")
                        return new APIResponse((int)ResponseCode.SystemBusy);
                }

                //type = "momov2";
                var providerCode = new Providers().GetBankCondition(type, transaction.PartnerCode);

                if (providerCode.Equals("CPI"))
                {
                    return new APIResponse((int)ResponseCode.ProviderNotFound);
                }

                var handler = BankDirectV2Factory.GetHandler(providerCode);

                string responseData = string.Empty;
                NLogLogger.Info(new string[] { "GetBanksV2 providerCode", providerCode, request.Type });
                if (request.Type == "momov2")
                {
                    result = handler.GetBanksV2(transaction.PartnerCode.ToString(), request.AccountName);

                }
                else
                {
                    result = handler.GetBanksV3(transaction.PartnerCode.ToString(), request.AccountName);
                }
                   
                NLogLogger.Info(new string[] { "GetBanksV2", transaction.TransactionID.ToString(), providerCode, "Response", serializer.Serialize(result) });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetBanksV2", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", ""), transaction.RequestContent });
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
                //request = serializer.Deserialize<GetBankRequest>(transaction.RequestContent);

                //Switch Provider
                var providerCode = new Providers().GetBankCondition("banktranfer", transaction.PartnerCode);
                if (providerCode.Equals("CPI"))
                {
                    return new APIResponse((int)ResponseCode.ProviderNotFound);
                }

                var handler = BankDirectV2Factory.GetHandler(providerCode);

                string responseData = string.Empty;

                result = handler.GetBanks(transaction.PartnerCode.ToString());
                NLogLogger.Info(new string[] { "GetBanks", transaction.TransactionID.ToString(), "Response", serializer.Serialize(result) });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetBanks", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

            return result;
        }
        private APIResponse OrderV2(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Phân tích yêu cầu thành đối tượng UseCardRequest đặt các điều kiện checking tại đây
            var request = new OrderRequestV2();
            APIResponse result = null;
            try
            {
                request = serializer.Deserialize<OrderRequestV2>(transaction.RequestContent);

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
                    return new APIResponse((int)ResponseCode.ProviderNotFound);
                }
              


                transaction.ProviderCode = providerCode;
                var handler = BankDirectV2Factory.GetHandler(providerCode);
                result = handler.OrderV2(transaction);
                if (result.ResponseCode != 1)
                    NLogLogger.Info(new string[] { "Order", transaction.TransactionID.ToString(), "Response", serializer.Serialize(result) });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Order", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
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
                    return new APIResponse((int)ResponseCode.ProviderNotFound);

                }
                //if (transaction.PartnerCode=="pp")
                //{
                //    providerCode = "simex";
                //}
                //var partner = new Partners().GetCache("cardzoro");
                //var provider = new Providers().GetCache("drumbank");
                //if (provider.SignatureType == 0)
                //NLogLogger.Info(new string[] { "Order", transaction.TransactionID.ToString(), "partner: ", serializer.Serialize(partner) });
                // if (partner.Status == 1)
                //if (provider.Status == 1)
                //{
                //    if (!request.BankName.ToLower().Equals("momo"))
                //    {
                //        //if (transaction.PartnerCode == "hai" )
                //        //{
                //        //    providerCode = "drumbank";

                //        //}
                //        //if (transaction.PartnerCode.Contains("mark"))
                //        //{
                //        //    if (DateTime.Now.Minute % 3 == 0)
                //        //        providerCode = "drumbank";

                //        //}
                //        request.BankName = request.BankName.ToUpper();
                //        if (request.BankName.Contains("RANDOM") || request.BankName.Contains("BIDV") || request.BankName.Contains("MB")||request.BankName.Contains("VCB") || request.BankName.Contains("ACB")|| request.BankName.Contains("TPB")||string.IsNullOrEmpty(request.BankName))
                //        {
                //            providerCode = "drumbank";
                //        }
                //        //if (request.BankName.Contains("ACB"))
                //        //{
                //        //    providerCode = "drumbank";
                //        //}
                //    }


                //}


                transaction.ProviderCode = providerCode;
                var handler = BankDirectV2Factory.GetHandler(providerCode);
                result = handler.Order(transaction);
                result.ResponseContent= result.ResponseContent.Replace(@"\u0026", "&");
                NLogLogger.Info(new string[] { "Order", request.RefCode.ToString(), "Response", serializer.Serialize(result) });

            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Order", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

            return result;
        }


    }
    public class CheckOrder
    {

        public int Amount { get; set; }

        public long Fee { get; set; }
        public string RefCode { get; set; }
        public string TransactionID { get; set; }
        public DateTime LasTime { get; set; }
    }


}


