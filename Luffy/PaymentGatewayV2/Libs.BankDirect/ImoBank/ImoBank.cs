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
using Libs.BankDirect.HynBank;
using System.Web.Caching;
using System.Configuration;

namespace Libs.BankDirect.ImoBank
{
    public class ImoBank : IBankDirectV2Handler
    {


        private const string urlBaseService = "https://switch.mopay.info/api6/";
        private const string callbackurl = "https://bank.kudopay.xyz/Callback/ImoCallback.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //private const string secretKey = "fd110f27-233b-4267-bb7e-000adca17bb0";
        //private const string secretKey2 = "fd110f27-233b-4267-bb7e-000adca17bb0";
        //private const string pw = "68686868";
        //private const string pw2 = "68686868";

        private const string Apikey = "0f43db36-1aa4-4389-8f03-5218912feed9";

        private const string ApiSecret = "113355a@";
        public APIResponse CheckTrans(APITransaction transaction)
        {


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public string GetBankAPI()
        {
            string cacheKey = "HynBankS";

            var cache = DataCaching.GetCache<string>(cacheKey);
            if (cache != null)
            {
                return cache;
            }
            string str2 = "getbanks";
            string str3 = "hyn5";
            string str4 = "941d69b5bfe82f513072c8545dd40bb2";
            HynBankLib.GetBankRequest request1 = new HynBankLib.GetBankRequest
            {
                Type = "banktranfer"
            };
            string str5 = this.serializer.Serialize(request1);
            string str6 = Encrypts.MD5(str3 + "bankdirect" + str2 + str5 + str4);
            HynBankLib.RequestData data = new HynBankLib.RequestData
            {
                PartnerCode = str3,
                CommandCode = str2,
                RequestContent = str5,
                ServiceCode = "bankdirect",
                Signature = str6
            };
            string input = HynBankLib.PostJson("https://bankgate.coroach.xyz//VPGJsonService.ashx", this.serializer.Serialize(data));

            DataCaching.SetCache<string>(cacheKey, input, 120);
            return input;

        }
        public APIResponse GetBanksV2(string Type, string Code)
        {

            var result = DataCaching.GetCache<List<BankAccount>>("ImoBanks_" + Type);
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
                var response = Task.Run(async () => await ImoBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "IMO", "GetBanks Response", response, urlService });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<ImoBankLib.BankResponse>(response);
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
                        var apibank = GetBankAPI();



                        //if (listBankObj.Exists(x => x.BankCode == "VIETINBANK"))
                        //{

                        //}
                        listBankObj = listBankObj.OrderBy(x => x.BankCode).ToList();
                        //listBankObj = listBankObj.Where(x => x.BankCode != "VCB").ToList();

                        //listBankObj = listBankObj.Where(x => x.BankCode != "BIDV").ToList();
                        //if ((DateTime.Now.Hour >= 8 ) || DateTime.Now.Hour < 3)
                        //{
                        //listBankObj.Add(new BankAccount()
                        //{

                        //    BankCode = "MB",
                        //    DisplayName = "MB",
                        //});
                        if (Type == "mjqk")
                        {
                            if (apibank.Contains("VPB"))
                            {

                                listBankObj.Add(new BankAccount()
                                {

                                    BankCode = "VPB",
                                    DisplayName = "VPB",
                                });

                            }
                        }
                        //}
                        //listBankObj.Add(new BankAccount()
                        //{

                        //    BankCode = "BIDV",
                        //    DisplayName = "BIDV",
                        //});
                        //if (apibank.Contains("VCB"))
                        //{

                        //    listBankObj.Add(new BankAccount()
                        //    {

                        //        BankCode = "VCB",
                        //        DisplayName = "VCB",
                        //    });

                        //}
                        //listBankObj.Add(new BankAccount()
                        //{

                        //    BankCode = "VCB",
                        //    DisplayName = "VCB",
                        //});


                        //if (apibank.Contains("TPB"))
                        //{

                        //    listBankObj.Add(new BankAccount()
                        //    {

                        //        BankCode = "TPB",
                        //        DisplayName = "TPB",
                        //    });

                        //}
                        listBankObj = listBankObj.Where(x => x.BankCode != "BIDV").ToList();
                        listBankObj = listBankObj.Where(x => x.BankCode != "TCB").ToList();

                        //listBankObj.Add(new BankAccount()
                        //{

                        //    BankCode = "MB",
                        //    DisplayName = "MB Quân Đội",
                        //});
                        //listBankObj.Add(new BankAccount()
                        //{

                        //    BankCode = "TCB",
                        //    DisplayName = "TCB",
                        //});
                        //listBankObj.Add(new BankAccount()
                        //{

                        //    BankCode = "TCB",
                        //    DisplayName = "TCB",
                        //});
                        var provider = new Partners().GetCache("dcp");
                        listBankObj = listBankObj.Where(x => x.BankCode != "VCB").ToList();
                        if (DateTime.Now.Hour >= 3 && DateTime.Now.Hour <= 23)
                        {
                            if (apibank.Contains("VPB"))
                            {
                                listBankObj.Add(new BankAccount()
                                {

                                    BankCode = "VCB",
                                    DisplayName = "VCB",
                                });
                            }
                        }
                        listBankObj.Add(new BankAccount()
                        {

                            BankCode = "BIDV",
                            DisplayName = "BIDV",
                        });
                        //var provider = new Partners().GetCache("dcp");
                        //if (DateTime.Now.Hour <= int.Parse(provider.Hotline.ToString()))
                        //{



                        //    listBankObj.Add(new BankAccount()
                        //    {

                        //        BankCode = "BIDV",
                        //        DisplayName = "BIDV",
                        //    });

                        //}
                        //else
                        //{
                        //    listBankObj.Add(new BankAccount()
                        //    {

                        //        BankCode = "TCB",
                        //        DisplayName = "TCB",
                        //    });

                        //}

                        //listBankObj = listBankObj.Where(x => x.BankCode != "VIETINBANK").ToList();
                        //listBankObj.Add(new BankAccount()
                        //{

                        //    BankCode = "VIETINBANK",
                        //    DisplayName = "VIETINBANK",
                        //});

                        listBankObj = listBankObj.Where(x => x.BankCode != "VTB").ToList();

                        DataCaching.SetCache("ImoBanks_" + Type, listBankObj, 60 * 2);
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




            return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public APIResponse Order(APITransaction transaction)
        {


            var tran = serializer.Deserialize<BankDirectV2Service.OrderRequest>(transaction.RequestContent);
            if (tran.Amount < 1)
            {
                return new APIResponse((int)ResponseCode.BankAmountInvalid);
            }
            if (tran.BankName.ToUpper() == "MBB")
            {
                tran.BankName = "MB";
            }
            //InitBank 
            var addTran = new BankGateAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = "A", // Tự động lấy bằng ID Table
                OrderInfo = tran.AccountName,
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
            if (tran.BankName.ToUpper() == "MOMO")
            {
                chargeType = "momo";
            }
            if (tran.BankName.ToUpper() == "VTP")
            {
                chargeType = "vtpay";
            }
            if (tran.BankName.ToUpper() == "BEP20" || tran.BankName.ToUpper() == "TRC20")
            {
                chargeType = "usdt";
            }
            if (tran.BankName.ToUpper() == "SEABANK" || tran.BankName.ToUpper() == "VPB")
            {
                return new APIResponse((int)ResponseCode.BankCodeInvalid);
            }




            var add = addTran.Add();

            if (add > 0)
            {
                //var OrderNo = add.ToString();
                var sign = Encrypts.MD5(tran.Amount + chargeType + add + ApiSecret);
                var urlService = $"{urlBaseService}MM/RegCharge?apiKey={Apikey}&chargeType={chargeType}&amount={tran.Amount}&requestId={add}&subType={tran.BankName}&callback={callbackurl}&sign={sign}";

                if (chargeType == "usdt")
                    urlService = $"{urlBaseService}MM/RegCharge?apiKey={Apikey}&chargeType={chargeType}&amount={tran.Amount}&requestId={add}&subType={tran.BankName}&callback={callbackurl}&phoneSender={addTran.OrderInfo}&sign={sign}";

                if (chargeType == "bank")
                    urlService = $"{urlBaseService}MM/RegCharge?apiKey={Apikey}&chargeType={chargeType}&amount={tran.Amount}&requestId={add}&subType={tran.BankName}&callback={callbackurl}&sign={sign}";

                //NLogLogger.Info(new string[] { "IMO", "Order Request", urlService });

                var response = Task.Run(async () => await ImoBankLib.GetTask(urlService)).Result;
                // NLogLogger.Info(new string[] { "IMO", "Order Response", response });


                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<ImoBankLib.OrderResponse>(response);


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
                        //if (chargeType == "usdt")
                        //    orderRes.Url = orderRes.Url.Remove(orderRes.Url.Length - 1, 1);

                        var lstBank = DataCaching.GetCache<List<BankAccountV3>>("BankInfoIMO");
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
                            DataCaching.SetCache("BankInfoIMO", lstBank, 86400 * 15);
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

        public APIResponse CallbackV2(ImoBankLib.Callback callback)
        {

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public APIResponse Callback(ImoBankLib.Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            var signature = Utils.Encrypts.MD5(callback.chargeId + callback.chargeType + callback.chargeCode + callback.chargeAmount + callback.status + callback.requestId + ApiSecret);
            //if (signature != callback.signature)
            //{
            //    NLogLogger.Info(new string[] { "IMO", "Callback", "Signature Failed", signature, callback.signature });
            //    return new APIResponse((int)ResponseCode.SignatureInvalid);
            //}
            //NLogLogger.Info(new string[] { "VNPAY", "Callback", callback.status });
            if (callback.status == "success")
            {
                var order = new BankGateAPI().Get(long.Parse(callback.requestId));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "IMO", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                string mobile = "";
                if (callback.result.Contains("From"))
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
                    order.OrderInfo = callback.chargeId;

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
                    Mobile = mobile,
                    Result = callback.result,
                    mTransId = callback.momoTransId
                };
                if (string.IsNullOrEmpty(datacb.mTransId))
                    datacb.mTransId = callback.requestId;
                //bank
                if (order.Type == 2)
                    datacb.mTransId = callback.requestId;
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
                    Task.Run(async () => await ImoBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
                }

                //Callback for Partner
                //if (!string.IsNullOrEmpty(order.ReturnUrl))
                //{
                //    var partner = new Partners().Get(order.PartnerCode);
                //    apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                //    Task.Run(async () => await ImoBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(apiResponse)).ConfigureAwait(false));
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


