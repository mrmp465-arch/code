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
using Libs.BankDirect_24h;
using System.Web.Routing;
using Newtonsoft.Json;

namespace Libs.BankDirect._24h
{
    public class _24hBank : IBankDirectV2Handler
    {


       
        private const string callbackurl = "https://bankgate.28pay.info/Callback/24hbank.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string Apikey = "cf701cbbe1ed4e1264da1b245f2f057bd65f9b1df06f2d7aeb95fa4b56cf1464";
        private const string urlBaseService = "https://247pay.vip/api/v1/";
        private const string ApiSecret = "COM7rexcAGn97byY3zKmGYMs8BQaS9X8";
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
            var listBankObj = new List<BankAccountV2>();
            //listBankObj.Add(new BankAccountV2()
            //{

            //    BankName = "MB",
            //    Name = "MB"
            //});
            listBankObj.Add(new BankAccountV2()
            {

                BankCode = "ACB",
                Name = "ACB"
            });

            return new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = serializer.Serialize(listBankObj)
            };

            //return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        
        public APIResponse GetBanks(string PartnerCode)
        {
            var listBankObj = new List<BankAccountV2>();
            //listBankObj.Add(new BankAccountV2()
            //{

            //    BankName = "MB",
            //    Name = "MB"
            //});
            listBankObj.Add(new BankAccountV2()
            {

                BankCode = "ACB",
                Name = "ACB"
            });

            return new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = serializer.Serialize(listBankObj)
            };

           // return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public APIResponse Order(APITransaction transaction)
        {
            //if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 58)
            //{
            //    return new APIResponse((int)ResponseCode.SystemMaintain);
            //}
            var systemDataConfig = new SystemDataConfig();
            var lstConfig = systemDataConfig.GetListCache();


            if (systemDataConfig.GetKey(lstConfig, "BankInEnable") == "0")
                return new APIResponse((int)ResponseCode.SystemMaintain);

            var tran = serializer.Deserialize<BankDirectV2Service.OrderRequest>(transaction.RequestContent);
            if (tran.Amount < 10000)
            {
                tran.Amount = 10000;
            }
          
            if (tran.BankName.ToLower() == "random" || tran.BankName.ToLower().StartsWith("ch") || string.IsNullOrEmpty(tran.BankName))
            {


                tran.BankName = "Bank";
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
                var urlService = $"{urlBaseService}Cashin/Create";
                var sign= _24hBankLib.ToSha256( tran.BankName + tran.Amount + tran.RefCode   + ApiSecret);
                var requestData = new { apiKey = Apikey, amount = tran.Amount, request_id = tran.RefCode.ToString(), url_callback = callbackurl, chargeType = tran.BankName, vsign = sign };
                
               NLogLogger.Info(new string[] { "24h", "Order Request", urlService , serializer.Serialize(requestData)});
                var response = Task.Run(async () => await MDrumBankLib.PostTask(urlService, serializer.Serialize(requestData))).Result;
                NLogLogger.Info(new string[] { "24h", "Order Response", response });

               

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<_24hBankLib.BankResponse>(response);


                    if (resObj.code == 200)
                    {
                        tran.BankName = resObj.data.bank_code;
                        var bankcodeqr = tran.BankName;

                        if (tran.BankName == "VIKKI")
                            tran.BankName = "DAB";
                        var orderResponse = resObj.data;
                        addTran.TransactionID = add;
                        addTran.BankAccountName = orderResponse.acc_name;
                        addTran.BankAccountNumber = orderResponse.acc_no;
                        addTran.OrderNo = orderResponse.code;
                        addTran.BankCode = tran.BankName;
                        //addTran.UpdateBank();
                        addTran.UpdateBankV2();
                        var bankinfo = new MDrumBankLib.BankAccountReceive();
                        bankinfo.Amount = tran.Amount;
                        bankinfo.BankCode = tran.BankName;
                        bankinfo.CreateDate = DateTime.Now;
                        bankinfo.OrderNo = orderResponse.code;
                        bankinfo.Status = "0";
                        bankinfo.BankId = orderResponse.acc_no;
                        bankinfo.BankName = orderResponse.acc_name;
                        bankinfo.Key = add.ToString();

                        MDrumBankLib.LogBankInfo(bankinfo);
                        var VietUrl = $"https://img.vietqr.io/image/{bankcodeqr}-{bankinfo.BankId}-compact.jpg?amount={tran.Amount}&addInfo={orderResponse.code}";
                        var orderRes = new Order()
                        {
                            Status = "1",
                            Amount = tran.Amount,
                            RefCode = tran.RefCode,
                            OrderNo = orderResponse.code,
                            Timeout = 15,
                            QRCode = VietUrl,
                            Url = orderResponse.acc_no,
                            BankCode = tran.BankName,
                            AccountNumber = orderResponse.acc_no,
                            AccountName = orderResponse.acc_name,
                        };
                        orderRes.Url = String.Format("https://info.28app.info/Pages/Bank.aspx?orderNo={0}", add.ToString());
                        orderRes.QRCode = VietUrl;
                        string jsonContent = JsonConvert.SerializeObject(orderRes, new JsonSerializerSettings
                        {
                            StringEscapeHandling = StringEscapeHandling.Default
                        });
                        return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {
                            ResponseContent = jsonContent
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

        public APIResponse CallbackV2(_24hBankLib.Callback callback)
        {
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public APIResponse Callback(_24hBankLib.Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            var signature = _24hBankLib.ToSha256(callback.type + callback.amount_transfer + callback.request_id+ ApiSecret);
            if (signature != callback.vsign)
            {
                NLogLogger.Info(new string[] { "24", "Callback", "Signature Failed", signature, callback.vsign });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }

            var order = new BankGateAPI().GetByRefcodeV2(callback.request_id);
            if (order == null)
            {
                NLogLogger.Info(new string[] { "24", "Callback", "Order NULL", serializer.Serialize(callback) });
                var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                return apiResponse;
            }

            //set success
            DataCaching.SetCache("LastOrderSuccess", "1", 300);


            if (order.Status != (int)ResponseCode.TransactionSuccessful)
            {
                order.Status = (int)ResponseCode.TransactionSuccessful;
                order.TotalAmount = Convert.ToDecimal(callback.amount_transfer);
                order.LastTime = DateTime.Now;
                order.Mobile = "";

               
                order.OrderInfo = callback.cash_in_id;
                var ck = getck(order.PartnerCode, order.BankCode);
                var rw = getrw(order.PartnerCode, order.BankCode);
                order.Fee = Convert.ToInt64(callback.amount_transfer * ck / 100);
                order.Reward = Convert.ToInt64(callback.amount_transfer * rw / 100);

                var datacb = new DataCallback()
                {
                    RefCode = order.RefCode,
                    OrderNo = order.OrderNo,
                    Type = "bank",
                    OrderInfo = callback.cash_in_id,
                    Amount = Convert.ToInt32(callback.amount_transfer),
                    Fee = order.Fee
                };

               

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
                    var Balancedesc = String.Format("Topup to recharge bank amount: {3} transId: {0}-{1}-{2}", order.TransactionID, order.BankCode, order.OrderNo + "-" + order.RefCode, Convert.ToInt64(callback.amount_transfer).ToString("#,#").Replace(",", "."));
                    if (order.BankCode == "MOMO")
                    {
                        Balancedesc = String.Format("Topup to recharge momo amount: {3} transId: {0}-{1}-{2}", order.TransactionID, order.BankCode, order.OrderNo + "-" + order.RefCode, Convert.ToInt64(callback.amount_transfer).ToString("#,#").Replace(",", "."));
                    }
                    UpdatePartnerBalance(order.PartnerCode, Convert.ToInt64(callback.amount_transfer), order.Fee, Balancedesc, "BankIn_" + order.TransactionID.ToString());

                    
                    var partner = new Partners().GetCache(order.PartnerCode);

                    if (!string.IsNullOrEmpty(partner.SMSUrl))
                    {

                        if (rw > 0)
                        {
                            var TotalR = order.Reward;
                            var Balancedesc2 = String.Format("Cộng tiền hoa hồng nạp bank đối tác {4} số tiền: {3} mgd: {0}-{1}-{2}", order.TransactionID, order.BankCode, order.OrderNo + "-" + order.RefCode, Convert.ToInt64(callback.amount_transfer).ToString("#,#").Replace(",", "."), partner.PartnerCode);
                            UpdatePartnerBalanceReward(partner.SMSUrl, TotalR, Balancedesc2, "RBankIn_" + order.TransactionID.ToString());
                        }
                    }

                    //
                    //if (!string.IsNullOrEmpty(partner.SMSCommand))
                    //{
                    //    var rw=getrw(order.PartnerCode, order.BankCode);
                    //    if (rw > 0)
                    //    {
                    //        var TotalR= Convert.ToInt64(callback.amount_transfer * rw);
                    //        var Balancedesc2 = String.Format("Cộng tiền hoa hồng nạp bank đối tác {4} số tiền: {3} mgd: {0}-{1}-{2}", order.TransactionID, order.BankCode, order.OrderNo + "-" + order.RefCode, Convert.ToInt64(callback.amount_transfer).ToString("#,#").Replace(",", "."), partner.PartnerCode);
                    //        UpdatePartnerBalanceReward(partner.SMSCommand, TotalR, Balancedesc2, "RBankIn_" + order.TransactionID.ToString());
                    //    }
                    //}    

                    //Callback for Partner
                    var checkOrder = new CheckOrder
                    {
                        LastTime = DateTime.Now,
                        RefCode = order.RefCode,
                        Amount = callback.amount_transfer,
                        TransactionID = order.TransactionID.ToString()
                    };
                    DataCaching.SetCache("CheckOrder:" + order.PartnerCode + order.RefCode, checkOrder, 900);
                    if (!string.IsNullOrEmpty(order.ReturnUrl))
                    {
                        apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {

                            ResponseContent = serializer.Serialize(datacb)
                        };

                        datacb.ResponseCode = apiResponse.ResponseCode;
                        datacb.Description = apiResponse.Description;
                        //if (order.PartnerCode == "kuipay" )
                        //{
                        //    datacb.AccountInfo = callback.BankName;
                        //    if (callback.BankName.Contains("MB"))
                        //    {
                        //        var requestaccount = new { BankCode = "MB", BankId = callback.BankId };
                        //        datacb.AccountInfo = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseServiceGetAccount, serializer.Serialize(requestaccount))).Result;
                        //    }
                        //    //mb xử lý lại
                        //}

                        //var partner = new Partners().GetCache(order.PartnerCode);
                        datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                       // apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await MDrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID, order.RefCode + " " + order.OrderNo).ConfigureAwait(false));

                    }

                }
            }



            return apiResponse;
        }
       
        private decimal getck(string PartnerCode, string Type)
        {
            decimal ck = 0;

            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
            if (listpartnerDiscount == null)
            {
                //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }

            if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
            {
                //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }
            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
            ck = _partnerDiscount.DiscountBANKTRANFER;
            if (Type == "MOMO")
                ck = _partnerDiscount.DiscountMOMO;


            return ck;
        }
        private decimal getrw(string PartnerCode, string Type)
        {
            decimal ck = 0;

            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
            if (listpartnerDiscount == null)
            {
                //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }

            if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
            {
                //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }
            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
            ck = _partnerDiscount.RewardBANKTRANFER;
            if (Type == "MOMO")
                ck = _partnerDiscount.RewardMOMO;


            return ck;
        }
        private void UpdatePartnerBalance(string PartnerCode, long Amount, long fee, string TranId, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), fee.ToString(), TranId.ToString(), RefCode });
                //var partner = new Partners().GetCache(PartnerCode);
                //if (string.IsNullOrEmpty(partner.Hotline))
                //{
                //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                //    return;
                //}
                //var user = new Users().GetByUserName(partner.Hotline.Trim());
                //if (user == null)
                //{
                //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                //    return;
                //}
                if (fee == 0)
                {
                    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                long realAmount = Amount - fee;
                // NLogLogger.Info(new string[] { "Bank Topup", realAmount.ToString(), ck.ToString() });
                new Users().Topup(realAmount, PartnerCode, PartnerCode, TranId, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }


        }
        private void UpdatePartnerBalanceReward(string PartnerCode, long realAmount, string TranId, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance Reward", PartnerCode, realAmount.ToString(), TranId.ToString(), RefCode });

                if (realAmount == 0)
                {
                    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                //long realAmount = Amount - fee;
                // NLogLogger.Info(new string[] { "Bank Topup", realAmount.ToString(), ck.ToString() });
                new Users().Topup(realAmount, PartnerCode, PartnerCode, TranId, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }


        }
    }


}


