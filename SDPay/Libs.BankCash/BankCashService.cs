using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;
using Libs.Report;

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
                case "checktrans":
                    return CheckTrans(transaction);
                default:
                    return new APIResponse((int)API.ResponseCode.AccessDenied);
            }
        }





        public class OrderRequest
        {
            public string Type { get; set; }
            //public string AccountName { get; set; }
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
        public class CheckOrder
        {

            public int Amount { get; set; }
            public string RefCode { get; set; }

            public string TransactionID { get; set; }
            public DateTime LasTime { get; set; }

        }
        private APIResponse CheckTrans(APITransaction transaction)
        {
            var partnercode = transaction.PartnerCode;
            var refcode = transaction.RequestContent;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            APIResponse result = new APIResponse((int)ResponseCode.TransactionFailed);
            var trans = new BankCashAPI().GetByRefcode(refcode, partnercode);

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
                if (trans.Status == -1)
                {
                    return new APIResponse((int)ResponseCode.TransactionFailed);
                }
                return new APIResponse((int)ResponseCode.CardProcessing);
            }
            //return result;
        }

        private APIResponse Cash(APITransaction transaction)
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
                //    NLogLogger.Info(new string[] { "Cash", transaction.TransactionID.ToString(), "Resquest some thing null: ", serializer.Serialize(transaction.RequestContent) });
                //    return new APIResponse((int)ResponseCode.ParameterInvalid);
                //}
                //Switch Provider
                //var providerCode = new Providers().GetBankCondition(request.Type, transaction.PartnerCode);
                //if (providerCode.Equals("CPI"))
                //{
                //    //return new APIResponse((int)ResponseCode.BankCodeInvalid);
                //    return new APIResponse((int)ResponseCode.ProviderNotFound);
                //}
                var providerCode = "drumbankcash";

                var systemDataConfig = new SystemDataConfig();
                var lstConfig = systemDataConfig.GetListCache();

                var service = systemDataConfig.GetKey(lstConfig, "ServiceOut");
                if (service == "247")
                {
                    providerCode = "24hbankcash";
                }
                if (service == "fast")
                {
                    providerCode = "fastpaybankcash";
                }
                if (transaction.PartnerCode == "paytest")
                {
                    providerCode = "24hbankcash";
                }
                //if (request.BankName.ToUpper() == "MOMO")
                //{
                //    //providerCode = "kzmomocash";
                //    providerCode = "drummomocash";
                //}

                //if(request.BankName.Contains("VBA") || request.BankName.Contains("AGR") || request.BankName.Contains("ACB"))
                //if (request.BankName.Contains("ACB"))
                //{
                //    providerCode = "kzbankcash";
                //}    
                PartnerService _partnerService = new PartnerService();
                //Kiểm tra Partner co được add Service ko
                _partnerService = _partnerService.Get(transaction.PartnerID, transaction.ServiceID);

                //check ip
                if (!string.IsNullOrEmpty(_partnerService.IPAddress))
                {
                    if (!_partnerService.IPAddress.Contains(IPAddress.Get()))
                    {
                        NLogLogger.Info(new string[] { "IpInvalid", _partnerService.IPAddress, IPAddress.Get()});
                        return new APIResponse((int)ResponseCode.IpInvalid);
                    }

                }
                if (!_partnerService.CommandCode.Contains("bank"))
                {
                    return new APIResponse((int)ResponseCode.ServiceIsLocked);
                }
                //NLogLogger.Info(new string[] { "partnerStatus", _partnerService.Quota.ToString(), _partnerService.Occurs.ToString(), transaction.PartnerCode.ToString() });
                if (_partnerService.Quota != 0 && _partnerService.Occurs == 1)
                {

                    //long total = 0; int totaltrans = 0; long total2 = 0;
                    //var data = new Libs.Report.BankCashAPI().ReportCheck("", transaction.PartnerCode, "", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ref totaltrans, ref total);
                    //var data2 = new BankGateAPI().Report("", transaction.PartnerCode, "", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ref totaltrans, ref total2);
                    //NLogLogger.Info(new string[] { "cash check", total.ToString(), total2.ToString(), _partnerService.Quota.ToString() });
                    //if (total+ request.Amount >= _partnerService.Quota + total2)
                    //{
                    //    TelegramNotify.SendTeleV2("-5375462347", "[Out Litmit] Vượt hạn mức rút từ đối tác " + transaction.PartnerCode + "(" + total.ToString("#,#").Replace(",", ".") + "-" + total2.ToString("#,#").Replace(",", ".")  +" Refcode "+request.RefCode + ")");
                    //    NLogLogger.Info(new string[] { "TransactionIgnore", total.ToString(), total2.ToString(), _partnerService.Quota.ToString() });
                    //    result = new APIResponse((int)ResponseCode.TransactionIgnore);
                    //    result.Description = "Vượt hạn mức rút trong ngày";
                    //    return result;


                    //}
                    //if(transaction.PartnerCode=="bp7")
                    //{
                    //    if (total+30000000 + request.Amount >= _partnerService.Quota + total2)
                    //    {
                    //        TelegramNotify.SendTeleV2("-5375462347", "[Warning Out Litmit] Đối tác " + transaction.PartnerCode + " sắp vượt hạn mức (" + total.ToString("#,#").Replace(",", ".") + "-" + total2.ToString("#,#").Replace(",", ".") + ")");
                    //    }
                    //}    
                     
                }
                //check api
                var handler = BankCashFactory.GetHandler(providerCode);
                transaction.ProviderCode = providerCode;
                NLogLogger.Info(new string[] { "Cash Request", providerCode, serializer.Serialize(transaction) });
                result = handler.Cash(transaction);
                NLogLogger.Info(new string[] { "Cash", request.RefCode.ToString(), "Response", serializer.Serialize(result) });
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
            var request = new CheckAccountRequest();
            APIResponse result = null;
            try
            {
                request = serializer.Deserialize<CheckAccountRequest>(transaction.RequestContent);

                bool isNull = request.GetType().GetProperties().All(p => p.GetValue(request) != null);
                if (!isNull)
                {
                    NLogLogger.Info(new string[] { "Cash", transaction.TransactionID.ToString(), "Resquest some thing null: ", serializer.Serialize(transaction.RequestContent) });
                    return new APIResponse((int)ResponseCode.ParameterInvalid);
                }
                //Switch Provider
                var providerCode = new Providers().GetBankCondition(request.Type, transaction.PartnerCode);
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
