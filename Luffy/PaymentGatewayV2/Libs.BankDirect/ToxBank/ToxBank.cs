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

namespace Libs.BankDirect.ToxBank
{
    public class ToxBank : IBankDirectV2Handler
    {


        private const string urlBaseService = "http://mopay2.vnm.bz:10007/api/";
        private const string callbackurl = "https://bank.namipay.xyz/Callback/VNPayCallbackV2.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //private const string secretKey = "fd110f27-233b-4267-bb7e-000adca17bb0";
        //private const string secretKey2 = "fd110f27-233b-4267-bb7e-000adca17bb0";
        //private const string pw = "68686868";
        //private const string pw2 = "68686868";

        private const string Apikey = "136bf507-eb74-4edf-a3dc-a9a01c58c35c";

        private const string ApiSecret = "Tox1BM";
        public APIResponse CheckTrans(APITransaction transaction)
        {


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse GetBanksV2(string Type, string Code)
        {

            var result = DataCaching.GetCache<List<BankAccount>>("ToxBanks");
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
                var urlService = urlBaseService + "Bank/getBankAvailable?apiKey=" + Apikey;


                // NLogLogger.Info(new string[] { "Bicbic", "GetBanks Request", urlService });
                //var response = Task.Run(async () => await VNPayBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
                var response = Task.Run(async () => await ToxBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "VNPAY", "GetBanks Response", response, urlService });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<ToxBankLib.BankResponse>(response);
                    if (resObj != null)
                    {
                        //var bank = serializer.Deserialize<VNPayBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                        var listBankObj = new List<BankAccount>();
                        foreach (var item in resObj.data)
                        {
                            listBankObj.Add(new BankAccount()
                            {

                                BankCode = item.code,
                                DisplayName = item.name
                            });
                        }

                        //if (DateTime.Now.Hour <= 6 || DateTime.Now.Hour >= 21)
                        //{
                        //    listBankObj = listBankObj.Where(x => x.BankCode != "VCB").ToList();

                        //}
                        //else
                        //{
                        //    listBankObj = listBankObj.Where(x => x.BankCode != "BIDV").ToList();
                        //}
                        listBankObj = listBankObj.Where(x => x.BankCode != "ACB").ToList();
                        listBankObj = listBankObj.Where(x => x.BankCode != "MB").ToList();
                        listBankObj = listBankObj.OrderBy(x => x.BankCode).ToList();
                        DataCaching.SetCache("ToxBanks", listBankObj, 60*1);
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

            var urlService = urlBaseService + "Bank/getBankAvailable?apiKey=" + Apikey;


            // NLogLogger.Info(new string[] { "Bicbic", "GetBanks Request", urlService });
            //var response = Task.Run(async () => await VNPayBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
            var response = Task.Run(async () => await ToxBankLib.GetTask(urlService)).Result;
            NLogLogger.Info(new string[] { "VNPAY", "GetBanks Response", response, urlService });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<ToxBankLib.BankResponse>(response);
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

                    //if (DateTime.Now.Hour <= 6 || DateTime.Now.Hour >= 21)
                    //{
                    //    listBankObj = listBankObj.Where(x => x.BankCode != "VCB").ToList();

                    //}
                    //else
                    //{
                    //    listBankObj = listBankObj.Where(x => x.BankCode != "BIDV").ToList();
                    //}
                    //listBankObj = listBankObj.Where(x => x.BankCode != "MB").ToList();
                    listBankObj = listBankObj.Where(x => x.BankCode != "ACB").ToList();
                    listBankObj = listBankObj.OrderBy(x => x.BankCode).ToList();
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
                var urlService = $"{urlBaseService}MM/RegCharge?apiKey={Apikey}&chargeType=bank&amount={tran.Amount}&requestId={add}&subType={tran.BankName}";
                if (tran.BankName.ToUpper() == "MOMO")
                    urlService = $"{urlBaseService}MM/RegCharge?apiKey={Apikey}&chargeType=momo&amount={tran.Amount}&requestId={add}&subType={tran.BankName}";
                //var sign = md5(amount + chargeType + requestId + signKey)
                NLogLogger.Info(new string[] { "VNPAY", "Order Request", urlService });
                var response = Task.Run(async () => await ToxBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "VNPAY", "Order Response", response });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<ToxBankLib.OrderResponse>(response);


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
                            Url = resObj.data.qr_url,
                            BankName = tran.BankName,
                            BankAccountNumber = resObj.data.phoneNum,
                            BankAccountName = resObj.data.phoneName
                        };
                        var lstBank = DataCaching.GetCache<List<BankAccountV3>>("BankInfo");
                        if (lstBank == null)
                        {
                            lstBank = new List<BankAccountV3>();

                        }
                        if (!lstBank.Exists(x => x.AccountId == resObj.data.phoneNum))
                        {
                            lstBank.Add(new BankAccountV3
                            {
                                AccountId = resObj.data.phoneNum,
                                AccountName = resObj.data.phoneName,
                                BankCode = tran.BankName
                            });
                            DataCaching.SetCache("BankInfo", lstBank, 86400 * 15);
                        }
                        return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {
                            ResponseContent = serializer.Serialize(orderRes)
                        };
                    }
                    else
                    {
                        return new APIResponse((int)ResponseCode.TransactionFailed)
                        {
                            Description = resObj.msg
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

        public APIResponse CallbackV2(ToxBankLib.Callback callback)
        {
            
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public APIResponse Callback(ToxBankLib.Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            var signature = Utils.Encrypts.MD5(callback.chargeId + callback.chargeType + callback.chargeCode + callback.chargeAmount + callback.status + callback.requestId + ApiSecret);
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
                string mobile = "";
                if(callback.result.Contains("From"))
                {
                    try
                    {
                        mobile = callback.result.Replace("From: ", "").Split(' ')[0];
                    }
                    catch
                    {

                    }
                    
                    
                }    

                if (order.Status != (int)ResponseCode.TransactionSuccessful)
                {
                    order.Status = (int)ResponseCode.TransactionSuccessful;
                    order.TotalAmount = Convert.ToDecimal(callback.chargeAmount);
                    order.LastTime = DateTime.Now;
                    order.Mobile = mobile;
                    order.Email = callback.result;
                    order.OrderInfo = callback.momoTransId;
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
                    BankCode = order.BankCode,
                    Mobile=mobile,
                    Result= callback.result,
                    mTransId = callback.momoTransId
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
                    Task.Run(async () => await ToxBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb),order.TransactionID).ConfigureAwait(false));
                }

                //Callback for Partner
                //if (!string.IsNullOrEmpty(order.ReturnUrl))
                //{
                //    var partner = new Partners().Get(order.PartnerCode);
                //    apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                //    Task.Run(async () => await ToxBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(apiResponse)).ConfigureAwait(false));
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


