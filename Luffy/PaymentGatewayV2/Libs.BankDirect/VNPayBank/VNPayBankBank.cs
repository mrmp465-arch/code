using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;

using System.Threading.Tasks;
using Libs.BankGate.Entity;
using BankAccount = Libs.BankGate.Entity.BankAccount;
using Libs.Report;
using System.Linq;

namespace Libs.BankDirect.VNPayBank
{
    public class VNPayBank : IBankDirectV2Handler
    {


        private const string urlBaseService = "https://api.ipay.vin/api";
        private const string callbackurl = "https://bank.namipay.xyz/Callback/VNPayCallback.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string secretKey = "e907b6f4-8ac6-4602-ad0e-fa4f8867603f";
        private const string secretKey2 = "e907b6f4-8ac6-4602-ad0e-fa4f8867603f";
        private const string pw = "112233";
        private const string pw2 = "112233";
        public APIResponse CheckTrans(APITransaction transaction)
        {


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse GetBanksV2(string Type, string Code)
        {

            var result = DataCaching.GetCache<List<BankAccount>>("VNPayBanks");
            //var result = DataCaching.GetCache<string>("ToxBanks");
            if (result != null)
            {
                return new APIResponse((int)ResponseCode.TransactionSuccessful)
                {
                    ResponseContent = serializer.Serialize(result)
                };
                //NLogLogger.Info(new string[] { "APIAloPays", "Token", "", "", token});
            }
            else
            {
                var urlService = urlBaseService + "?c=GetBankAvailable&apiKey=" + secretKey;

                var response = Task.Run(async () => await VNPayBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "VNPAY", "GetBanks Response", response, urlService });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<VNPayBankLib.BankResponse>(response);
                    if (resObj != null)
                    {
                        //var bank = serializer.Deserialize<VNPayBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                        var listBankObj = new List<BankAccountV2>();
                        foreach (var item in resObj.data)
                        {
                            listBankObj.Add(new BankAccountV2()
                            {

                                BankCode = item.code,
                                BankName = item.name
                                //B = item.name
                            });
                        }
                        listBankObj = listBankObj.Where(x => x.BankCode != "mb").ToList();
                        if (DateTime.Now.Hour <= 6 || DateTime.Now.Hour >= 21)
                        {

                            listBankObj = listBankObj.Where(x => x.BankCode != "vcb").ToList();
                        }
                        DataCaching.SetCache("VNPayBanks", listBankObj, 60 * 5);
                        return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {
                            ResponseContent = serializer.Serialize(listBankObj)
                        };
                    }



                }
            }

            
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse GetBanks()
        {


            var urlService = urlBaseService + "?c=GetBankAvailable&apiKey=" + secretKey;



            // NLogLogger.Info(new string[] { "Bicbic", "GetBanks Request", urlService });
            //var response = Task.Run(async () => await VNPayBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
            var response = Task.Run(async () => await VNPayBankLib.GetTask(urlService)).Result;
            NLogLogger.Info(new string[] { "VNPAY", "GetBanks Response", response, urlService });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<VNPayBankLib.BankResponse>(response);
                if (resObj != null)
                {
                    //var bank = serializer.Deserialize<VNPayBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                    var listBankObj = new List<BankAccount>();
                    foreach (var item in resObj.data)
                    {
                        listBankObj.Add(new BankAccount()
                        {

                            BankCode = item.code,
                            //B = item.name
                        });
                    }


                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = serializer.Serialize(listBankObj)
                    };
                }



            }


            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public APIResponse Order(APITransaction transaction)
        {


            var tran = serializer.Deserialize<BankDirectV2Service.OrderRequest>(transaction.RequestContent);
            if (tran.Amount < 10000)
            {
                return new APIResponse((int)ResponseCode.BankAmountInvalid);
            }
            //InitBank 
            var addTran = new BankGateAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = "A", // Tự động lấy bằng ID Table
                OrderInfo = string.Empty,
                Amount = 0,
                TotalAmount = 0, // Hứng tiền sau khi xử lý thật
                Currency = "VND",
                ReturnUrl = tran.CallbackUrl,
                RequestTime = 0,
                Signature = Encrypts.MD5(DateTime.Now.ToString()),
                LogContent = "Add Order",
                BankCode = tran.BankName,
                FullName = tran.AccountName,
                Mobile = string.Empty,
                RefCode = tran.RefCode,
                //BankAccountName = tran.BankAccountName,
                //BankAccountNumber = tran.BankAccountNumber,

            };

            addTran.Amount = tran.Amount;
            string chargeType = "bank";

            if (addTran.BankCode.ToUpper()=="MOMO")
            {
                chargeType = "momo";
            }    
            var add = addTran.Add();

            if (add > 0)
            {
                //var OrderNo = add.ToString();
                var urlService = $"{urlBaseService}?c=RegCharge&apiKey={secretKey}&chargeType={chargeType}&amount={tran.Amount}&requestId={add}&subType={tran.BankName}&callback={callbackurl}";


                NLogLogger.Info(new string[] { "VNPAY", "Order Request", urlService });
                var response = Task.Run(async () => await VNPayBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "VNPAY", "Order Response", response });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<VNPayBankLib.OrderResponse>(response);


                    if (resObj.stt == 1)
                    {
                        addTran.TransactionID = add;
                        addTran.BankAccountName = resObj.data.phoneName;
                        addTran.BankAccountNumber = resObj.data.phoneNum;
                        addTran.OrderNo = resObj.data.code;
                        addTran.UpdateBank();
                        var orderRes = new Order()
                        {
                            Status = resObj.stt.ToString(),
                            Amount = tran.Amount,
                            RefCode = tran.RefCode,
                            OrderNo = resObj.data.code,
                            Timeout = 30,
                            Url = "",
                            BankName = tran.BankName,
                            BankAccountNumber = resObj.data.phoneNum,
                            BankAccountName = resObj.data.phoneName
                        };

                        return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {
                            ResponseContent = serializer.Serialize(orderRes)
                        };
                    }



                }
                return new APIResponse((int)ResponseCode.TransactionFailed);
            }
            else
            {
                switch (add)
                {
                    case (int)ResponseCode.AccessDenied:
                        return new APIResponse((int)ResponseCode.AccessDenied);
                    case (int)ResponseCode.TransactionDuplicate:
                        return new APIResponse((int)ResponseCode.TransactionDuplicate);
                }
            }


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public APIResponse CallbackV2(VNPayBankLib.Callback callback)
        {
            //var ipRequest = Libs.Utils.IPAddress.Get();
            //if (ipRequest != serviceIp)
            //{
            //    NLogLogger.Info(new string[] { "M32", "Callback", "Ip Invalid", ipRequest, serviceIp });
            //    return new APIResponse((int)ResponseCode.IpInvalid);
            //}
            var partnercode = "pp";
            if(callback.chargeCode.StartsWith("ld ") || callback.chargeCode.StartsWith("Ld ") || callback.chargeCode.StartsWith("LD "))
            {
                 partnercode = "azt";
            }
            if (callback.chargeCode.StartsWith("MAY ") || callback.chargeCode.StartsWith("May ") || callback.chargeCode.StartsWith("may "))
            {
                partnercode = "mb86";
            }
            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);
            //if (callback.chargeCode.ToLower().Contains("chuyen tien"))
            //{
            //    return new APIResponse((int)ResponseCode.TransactionFailed);
            //}
            var Partner = new Partners().Get(partnercode);
            //var selectSecretKey = secretKey;

            var signature = Utils.Encrypts.MD5(callback.chargeId + callback.chargeType + callback.chargeCode + callback.chargeAmount + callback.status + callback.requestId + pw2);
            if (signature != callback.signature)
            {
                NLogLogger.Info(new string[] { "VNPAY", "Callback", "Signature Failed", callback.chargeId + callback.chargeType + callback.chargeCode + callback.chargeAmount + callback.status + callback.requestId + pw2, signature, callback.signature });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }

            //if(callback.message.Length>15)
            //{
            //    callback.message = callback.message.Replace("Tiềnvàotiềnra,cótiềnlàđạigia😄", "");
            //}    
            var AcountName = callback.chargeCode;

            var neworder = new BankGateAPI()
            {
                PartnerID = Partner.PartnerID,
                PartnerCode = Partner.PartnerCode,
                ProviderCode = "vnpaybank",
                OrderNo = callback.chargeCode,
                OrderInfo = callback.momoTransId,
                Amount = Convert.ToDecimal(callback.chargeAmount),
                TotalAmount = Convert.ToDecimal(callback.chargeAmount),
                Currency = "VND",
                ReturnUrl = "",
                RequestTime = 0,
                Signature = Encrypts.MD5(DateTime.Now.ToString()),
                LogContent = "Add Order " + callback.chargeCode,
                BankCode = callback.bank,
                FullName = AcountName,
                Mobile = "",
                RefCode = callback.chargeId,
                BankAccountName = string.Empty,
                BankAccountNumber = string.Empty,
                LastTime = DateTime.Now,
                Status = (int)ResponseCode.TransactionSuccessful
            };
            //if (partnercode == "pp")
            //    neworder.Status = 0;
            //var keyCache = String.Format("MomoAccount:{0}", callback.account_receive);
            //var dataCache = DataCaching.GetCache<string>(keyCache);
            //if (dataCache != null)
            //{
            //    neworder.BankAccountName = dataCache.ToString();
            //}
            var addId = neworder.AddV2();
            if (addId < 0)
            {
                //neworder.Status = (int)addId;
                //neworder.Update();
                return new APIResponse((int)addId);
            }
            //apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            //{
            //    ResponseContent = serializer.Serialize(new DataCallback()
            //    {
            //        RefCode = neworder.OrderInfo,
            //        Mobile = neworder.BankCode,
            //        OrderNo = neworder.FullName,
            //        Amount = Convert.ToInt32(callback.chargeAmount),
            //    })
            //};
            var datacb = new DataCallback()
            {
                Content = neworder.OrderNo,
                Mobile = "",
                TransId = neworder.OrderInfo,
                Amount = Convert.ToInt32(callback.chargeAmount),
                BankCode = neworder.BankCode,
            };
            datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount + datacb.Content, Partner.PrivateKey, Partner.SignatureType);
            //Action<string, long, string, long> send = UpdatePartnerBalance;
            //var asynSend = send.BeginInvoke(neworder.PartnerCode, Convert.ToInt64(callback.chargeAmount), neworder.BankCode, neworder.TransactionID, null, null);
            //NLogLogger.Info(new string[] { "VNPay", "PartnerCallback", apiResponse.ResponseContent, Partner.SMSPlusUrl });
            //Callback for Partner
            var url = Partner.SMSPlusCheckUrl;
         

            if (!string.IsNullOrEmpty(url) && Partner.PartnerCode!="pp")
            {

                //apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, Partner.PrivateKey, Partner.SignatureType);
                Task.Run(async () => await VNPayBankLib.CallbackJson(url, serializer.Serialize(datacb)).ConfigureAwait(false));
            }



            return apiResponse;
        }

        public APIResponse Callback(VNPayBankLib.Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            var signature = Utils.Encrypts.MD5(callback.chargeId + callback.chargeType + callback.chargeCode + callback.chargeAmount + callback.status + callback.requestId + pw);
            if (signature != callback.signature)
            {
                NLogLogger.Info(new string[] { "VNPAY", "Callback", "Signature Failed", signature, callback.signature });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }
            //NLogLogger.Info(new string[] { "VNPAY", "Callback", callback.status });
            if (callback.status == "success")
            {
                var order = new BankGateAPI().Get(long.Parse(callback.requestId));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "VNPAY", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }

               

                if (order.Status != (int)ResponseCode.TransactionSuccessful)
                {
                    order.Status = (int)ResponseCode.TransactionSuccessful;
                    order.TotalAmount = Convert.ToDecimal(callback.chargeAmount);
                    order.LastTime = DateTime.Now;
                    order.Mobile = "";
                    order.OrderInfo = callback.chargeId+"|"+callback.momoTransId;
                    order.Update();
                    //Action<string, long, string, long> send = UpdatePartnerBalance;
                    //var asynSend = send.BeginInvoke(order.PartnerCode, Convert.ToInt64(callback.chargeAmount), order.BankCode, order.TransactionID, null, null);
                }
                apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                var datacb = new DataCallback()
                {
                    Content = order.OrderNo,
                    TransId = order.RefCode,
                    Amount = Convert.ToInt32(callback.chargeAmount),
                    BankCode = order.BankCode
                };

                if (order.PartnerCode == "azt")
                {
                    Action<string, long, string, long> send = UpdatePartnerBalance;
                    var asynSend = send.BeginInvoke(order.PartnerCode, Convert.ToInt64(callback.chargeAmount), order.BankCode, order.TransactionID, null, null);
                }
                //Callback for Partner
                if (!string.IsNullOrEmpty(order.ReturnUrl))
                {
                    var Partner = new Partners().Get(order.PartnerCode);
                    datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount + datacb.Content, Partner.PrivateKey, Partner.SignatureType);
                    //NLogLogger.Info(new string[] { "VNPay", "PartnerCallback", serializer.Serialize(datacb), order.ReturnUrl });
                    Task.Run(async () => await VNPayBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb),order.TransactionID).ConfigureAwait(false));
                }

                //Callback for Partner
                //if (!string.IsNullOrEmpty(order.ReturnUrl))
                //{
                //    var partner = new Partners().Get(order.PartnerCode);
                //    apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                //    Task.Run(async () => await VNPayBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(apiResponse)).ConfigureAwait(false));
                //}

                return apiResponse;
            }
            
            return new APIResponse((int)ResponseCode.TransactionFailed);
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

            if (ck == 0)
                return;

            long realAmount = Amount - Convert.ToInt64(Amount * ck);
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Partners().Topup(realAmount, PartnerCode, $"Cộng tiền nạp bank mã giao dịch {TranId}");

        }

    }


}


