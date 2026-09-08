using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;

namespace Libs.BankCash
{

    public class BankCashService : IServiceHandler
    {

        public APIResponse Request(APITransaction transaction)
        {
            switch (transaction.CommandCode.ToLower())
            {

                case "cash":
                    return Cash(transaction);
                case "check":
                    return Check(transaction);
                default:
                    return new APIResponse((int)API.ResponseCode.AccessDenied);
            }
        }



        public class OrderRequest2
        {
            public string Type { get; set; }
            public string AccountName { get; set; }
            public string BankName { get; set; }
            public string BankAccountName { get; set; }
            public string BankAccountNumber { get; set; }
            public string AppCode { get; set; }
            public string RefCode { get; set; }
            public int Amount { get; set; }
            public string CallbackUrl { get; set; }

            public string Note { get; set; }
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

            public string Note { get; set; }
        }
        public class CheckAccountRequest
        {
            public string Type { get; set; }
            public string BankName { get; set; }
            public string BankAccountNumber { get; set; }
            //public string AppCode { get; set; }

        }
        //To do : here



        private APIResponse Cash(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Phân tích yêu cầu thành đối tượng UseCardRequest đặt các điều kiện checking tại đây
            var request = new OrderRequest();
            APIResponse result = null;
            try
            {
                request = serializer.Deserialize<OrderRequest>(transaction.RequestContent);

                bool isNull = request.GetType().GetProperties().All(p => p.GetValue(request) != null);
                if (!isNull)
                {
                    NLogLogger.Info(new string[] { "Cash", transaction.TransactionID.ToString(), "Resquest some thing null: ", serializer.Serialize(transaction.RequestContent) });
                    return new APIResponse((int)ResponseCode.ParameterInvalid);
                }
                //Switch Provider
                var providerCode = new Providers().GetBankCashCondition(request.Type, transaction.PartnerCode);
                if (providerCode.Equals("CPI"))
                {
                    return new APIResponse((int)ResponseCode.BankCodeInvalid);
                    //providerCode = "kenbankcash";

                }
                //if (request.BankName.ToUpper() != "MOMO")
                //{
                //    if (transaction.PartnerCode != "dcp")
                //    {
                //        if (request.Amount < 1000000)
                //        {
                //            providerCode = "kenbankcash";

                //            if ((DateTime.Now.Second % 3==0) )
                //                providerCode = "pushbankcash";
                //            if(DateTime.Now.Hour<=9)
                //            {
                //                providerCode = "pushbankcash";
                //            }    
                //        }

                //        if (request.Amount >= 1000000)
                //        {
                //            if (DateTime.Now.Second % 4== 0)
                //            {
                //                providerCode = "kenbankcash";
                //            }
                //            else
                //            {
                //                providerCode = "pushbankcash";
                //            }

                //            if (DateTime.Now.Hour <= 7)
                //            {
                //                providerCode = "pushbankcash";
                //            }
                //            if (request.Amount > 2000000)
                //            {
                //                providerCode = "kenbankcash";
                //            }
                          
                //        }
                //        if (transaction.PartnerCode == "xmen")
                //        {
                //            providerCode = "kenbankcash";
                //        }

                //        var provider = new Providers().Get("kenbankcash");
                //        if (provider.Status == 0)
                //            providerCode = "pushbankcash";

                //        var provider2 = new Providers().Get("pushbankcash");
                //        if (provider2.Status == 0)
                //            providerCode = "kenbankcash";

                //    }
                //    if (transaction.PartnerCode == "dcp")
                //    {
                //        providerCode = "kenbankcash";
                //    }
                //}
                //else
                //{
                //    //if (request.Amount >= 5000000)
                //    //{
                //    //    providerCode = "kenmomocash";
                //    //}
                //    providerCode = "pushmomocash";

                //}
                //if (transaction.PartnerCode == "panda" || transaction.PartnerCode == "panpan")
                //{
                //    if (request.BankAccountNumber == "0358822660")

                //        return new APIResponse((int)ResponseCode.TransactionRejected);
                //}

                //var request = serializer.Deserialize<OrderRequest>(transaction.RequestContent);
                var handler = BankCashFactory.GetHandler(providerCode);
                transaction.ProviderCode = providerCode;
                result = handler.Cash(transaction);
                NLogLogger.Info(new string[] { "Cash", transaction.TransactionID.ToString(), "Response", serializer.Serialize(result) });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Cash", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

            return result;
        }
        private APIResponse Check(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Phân tích yêu cầu thành đối tượng UseCardRequest đặt các điều kiện checking tại đây
            var request = transaction.RequestContent;
            APIResponse result = null;
            try
            {
                //request = serializer.Deserialize<CheckAccountRequest>(transaction.RequestContent);

                //bool isNull = request.GetType().GetProperties().All(p => p.GetValue(request) != null);
                //if (!isNull)
                //{
                //    NLogLogger.Info(new string[] { "Cash", transaction.TransactionID.ToString(), "Resquest some thing null: ", serializer.Serialize(transaction.RequestContent) });
                //    return new APIResponse((int)ResponseCode.ParameterInvalid);
                //}
                //Switch Provider
                var providerCode = new Providers().GetBankCondition("momocash", transaction.PartnerCode);
                if (providerCode.Equals("CPI"))
                {
                    return new APIResponse((int)ResponseCode.BankCodeInvalid);
                }
                var handler = BankCashFactory.GetHandler(providerCode);
                transaction.ProviderCode = providerCode;
                result = handler.Check(transaction);
                NLogLogger.Info(new string[] { "Cash", transaction.TransactionID.ToString(), "Response", serializer.Serialize(result) });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Cash", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

            return result;
        }

    }




}
