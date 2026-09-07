using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;
using System.Security.Cryptography;

using System.Configuration;
using System.Threading;

using System.Threading.Tasks;
using Libs.BankGate.Entity;
using Libs.Report;
using Libs.BankDirect.MDrum;
using System.Web;

namespace Libs.BankDirect.VNPayBank
{
    public class VNPayBank : IBankDirectV2Handler
    {


        private const string urlBaseService = "http://rin.vnm.bz:10007/api/";
        private const string callbackurl = "https://bankgate.fasttransfer.pro//Callback/VNPayCallback.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string Apikey = "5a562e41-a6a0-4288-975f-dd5a269076b8";

        private const string ApiSecret = "!lp08fAc";
        public APIResponse GetBanksV3(string ParnerId, string AccoutName)
        {
            var requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            return new APIResponse((int)ResponseCode.TransactionFailed);


        }
        public APIResponse OrderV2(APITransaction transaction)
        {
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse CheckTrans(APITransaction transaction)
        {


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse GetBanksV2(string ParnerId, string AccoutName)
        {


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        private List<BankAccountV2> GetBanksAPI()
        {

            var result = DataCaching.GetCache<List<BankAccountV2>>("VNBanks");
            if (result != null)
            {
                return result;
            }
            var urlService = urlBaseService + "Bank/getBankAvailable?apiKey=" + Apikey;
            var response = Task.Run(async () => await SimexBankLib.GetTask(urlService)).Result;
            NLogLogger.Info(new string[] { "VNPAY", "GetBanks Response", response, urlService });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<SimexBankLib.BankResponse>(response);
                if (resObj != null)
                {
                    //var bank = serializer.Deserialize<VNPayBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                    var listBankObj = new List<BankAccountV2>();
                    foreach (var item in resObj.data)
                    {
                        listBankObj.Add(new BankAccountV2()
                        {

                            BankName = item.code,
                            Name = item.name
                        });
                    }

                    DataCaching.SetCache("VNBanks", listBankObj, 60 * 1);
                    return listBankObj;
                }

            }
            return new List<BankAccountV2>();

        }
        public APIResponse GetBanks(string PartnerCode)
        {
            var urlService = urlBaseService + "Bank/getBankAvailable?apiKey=" + Apikey;



            // NLogLogger.Info(new string[] { "Bicbic", "GetBanks Request", urlService });
            //var response = Task.Run(async () => await VNPayBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
            var response = Task.Run(async () => await SimexBankLib.GetTask(urlService)).Result;
            NLogLogger.Info(new string[] { "VNPAY", "GetBanks Response", response, urlService });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<SimexBankLib.BankResponse>(response);
                if (resObj != null)
                {
                    //var bank = serializer.Deserialize<VNPayBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                    var listBankObj = new List<BankAccountV2>();
                    foreach (var item in resObj.data)
                    {
                        listBankObj.Add(new BankAccountV2()
                        {

                            BankName = item.code,
                            Name = item.name
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
            if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 58)
            {
                return new APIResponse((int)ResponseCode.SystemMaintain);
            }
            var tran = serializer.Deserialize<BankDirectV2Service.OrderRequest>(transaction.RequestContent);
            if (tran.Amount < 10000)
            {
                tran.Amount = 10000;
            }
            var lstbank = GetBanksAPI();
            if (tran.BankName.ToLower() == "random" || tran.BankName.ToLower().StartsWith("ch") || string.IsNullOrEmpty(tran.BankName))
            {

                var rand = new Random();
                tran.BankName = lstbank[rand.Next(lstbank.Count)].BankName;
            }
            else
            {
                if (!lstbank.Exists(x => x.BankName == tran.BankName))
                    return new APIResponse((int)ResponseCode.SystemMaintain);
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
                FullName = tran.RefCode,
                Mobile = string.Empty,
                RefCode = tran.RefCode,
                //BankAccountName = tran.BankAccountName,
                //BankAccountNumber = tran.BankAccountNumber,

            };

            addTran.Amount = tran.Amount;
            var add = addTran.Add();

            if (add > 0)
            {
                //var OrderNo = add.ToString();
                var urlService = $"{urlBaseService}MM/RegCharge?apiKey={Apikey}&chargeType=bank&amount={tran.Amount}&requestId={add}&subType={tran.BankName}";
                if (tran.BankName.ToUpper() == "MOMO")
                    urlService = $"{urlBaseService}MM/RegCharge?apiKey={Apikey}&chargeType=momo&amount={tran.Amount}&requestId={add}&subType={tran.BankName}";
                //var sign = md5(amount + chargeType + requestId + signKey)
                NLogLogger.Info(new string[] { "VNPAY", "Order Request", urlService });
                var response = Task.Run(async () => await SimexBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "VNPAY", "Order Response", response });

               

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<SimexBankLib.OrderResponse>(response);


                    if (resObj.stt == 1)
                    {

                        addTran.TransactionID = add;
                        addTran.BankAccountName = resObj.data.phoneName;
                        addTran.BankAccountNumber = resObj.data.phoneNum;
                        addTran.OrderNo = resObj.data.code;
                        addTran.UpdateBank();

                        var bankinfo = new MDrumBankLib.BankAccountReceive();
                        bankinfo.Amount = tran.Amount;
                        bankinfo.BankCode = tran.BankName;
                        bankinfo.CreateDate = DateTime.Now;
                        bankinfo.OrderNo = resObj.data.code;
                        bankinfo.Status = "0";
                        bankinfo.BankId = resObj.data.phoneNum;
                        bankinfo.BankName = resObj.data.phoneName;
                        bankinfo.Key = add.ToString();

                        MDrumBankLib.LogBankInfo(bankinfo);

                        var orderRes = new Order()
                        {
                            Status = resObj.stt.ToString(),
                            Amount = tran.Amount,
                            RefCode = tran.RefCode,
                            OrderNo = resObj.data.code,
                            Timeout = 15,
                            QRCode = String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?amount={2}&addInfo={3}", addTran.BankCode, resObj.data.phoneNum, resObj.data.amount, resObj.data.code),
                            Url = String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?amount={2}&addInfo={3}", addTran.BankCode, resObj.data.phoneNum, resObj.data.amount, resObj.data.code),
                            BankName = tran.BankName,
                            BankAccountNumber = resObj.data.phoneNum,
                            BankAccountName = resObj.data.phoneName
                        };
                        orderRes.LinkOpenApp = String.Format("https://momo.opaps.info/Pages/Bank.aspx?orderNo={0}", add.ToString());
                        orderRes.LinkWebView = String.Format("https://momo.opaps.info/Pages/Bank.aspx?orderNo={0}", add.ToString());
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

        public APIResponse CallbackV2(SimexBankLib.Callback callback)
        {
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public APIResponse Callback(SimexBankLib.Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            var signature = Utils.Encrypts.MD5(callback.chargeId + callback.chargeType + callback.chargeCode + callback.chargeAmount + callback.status + callback.requestId + ApiSecret);
            if (signature != callback.signature)
            {
                NLogLogger.Info(new string[] { "VNPAY", "Callback", "Signature Failed", signature, callback.signature });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }

            var order = new BankGateAPI().Get(long.Parse(callback.requestId));
            if (order == null)
            {
                NLogLogger.Info(new string[] { "VNPAY", "Callback", "Order NULL", serializer.Serialize(callback) });
                var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                return apiResponse;
            }
            if (string.IsNullOrEmpty(callback.momoTransId))
                callback.momoTransId = callback.requestId;

            apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = serializer.Serialize(new DataCallback()
                {
                    RefCode = order.RefCode,
                    OrderNo = order.OrderNo,
                    Type = "bank",
                    OrderInfo = callback.momoTransId,
                    Amount = Convert.ToInt32(callback.chargeAmount),
                })
            };

            if (order.Status != (int)ResponseCode.TransactionSuccessful)
            {
                order.Status = (int)ResponseCode.TransactionSuccessful;
                order.TotalAmount = Convert.ToDecimal(callback.chargeAmount);
                order.LastTime = DateTime.Now;
                order.Mobile = "";

               
                order.OrderInfo = callback.momoTransId;
                var ck = getck(order.PartnerCode, order.BankCode);
                order.Fee = Convert.ToInt64(int.Parse(callback.chargeAmount) * ck);

                var resultupdate = order.Update();

                NLogLogger.Info(new string[] { "MRUM", "Callback", "OrderUpdate", order.OrderNo, resultupdate.ToString() });
                if (resultupdate >= 0)
                {
                    //update ordercach
                    try
                    {
                        var bankinfo = MDrumBankLib.GetBankInfo(order.TransactionID.ToString());
                        var FullOrderNo = bankinfo.OrderNo;
                        if (bankinfo != null)
                        {
                            bankinfo.Status = "1";
                            bankinfo.QRCodeBase64 = "";
                            bankinfo.LinkOpenApp = "";
                            MDrumBankLib.LogBankInfo(bankinfo);
                        }
                    }
                    catch
                    {

                    }
                    var Balancedesc = String.Format("Cộng tiền nạp bank số tiền: {3} mgd: {0}-{1}-{2}", order.TransactionID, order.BankCode, order.OrderNo + "-" + order.RefCode, Convert.ToInt64(callback.chargeAmount).ToString("#,#").Replace(",", "."));
                    if (order.BankCode == "MOMO")
                    {
                        Balancedesc = String.Format("Cộng tiền nạp momo số tiền: {3} mgd: {0}-{1}-{2}", order.TransactionID, order.BankCode, order.OrderNo + "-" + order.RefCode, Convert.ToInt64(callback.chargeAmount).ToString("#,#").Replace(",", "."));
                    }
                    UpdatePartnerBalance(order.PartnerCode, Convert.ToInt64(callback.chargeAmount), order.Fee, Balancedesc, "BankIn_" + order.TransactionID.ToString());


                    //Callback for Partner

                    if (!string.IsNullOrEmpty(order.ReturnUrl))
                    {


                        var partner = new Partners().GetCache(order.PartnerCode);
                        apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await MDrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID, order.RefCode + " " + order.OrderNo).ConfigureAwait(false));

                    }

                }
            }





            return apiResponse;
        }
        private decimal getck(string PartnerCode, string Type)
        {
            decimal ck = 0;
            //var partner = new Partners().GetCache(PartnerCode);
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
            if (listpartnerDiscount == null)
            {
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }

            if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
            {
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }
            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
            ck = _partnerDiscount.DiscountBANKTRANFER;
            if (Type == "MOMO")
                ck = _partnerDiscount.DiscountMOMO;
            return ck;
        }

        private void UpdatePartnerBalance(string PartnerCode, long Amount, long fee, string TranId, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), fee.ToString(), TranId.ToString(), RefCode });
                var partner = new Partners().GetCache(PartnerCode);
                if (string.IsNullOrEmpty(partner.Hotline))
                {
                    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return;
                }
                var user = new Users().GetByUserName(partner.Hotline.Trim());
                if (user == null)
                {
                    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return;
                }
                if (fee == 0)
                {
                    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                long realAmount = Amount - fee;
                // NLogLogger.Info(new string[] { "Bank Topup", realAmount.ToString(), ck.ToString() });
                new Users().Topup(realAmount, user.UserName, PartnerCode, TranId, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }


        }
    }


}


