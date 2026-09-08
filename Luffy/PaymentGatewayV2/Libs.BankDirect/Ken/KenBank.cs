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

namespace Libs.BankDirect.Ken
{
    public class KenBank : IBankDirectV2Handler
    {


        private const string urlBaseService = "http://128.199.254.52/api/";


        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string Partner = "THA";

        private const string PartnerCode = "0175f2c7e8df1763305d41060248f9e5";
        public APIResponse CheckTrans(APITransaction transaction)
        {


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse GetBanksV2(string Type, string Code)
        {

            var result = DataCaching.GetCache<List<BankAccount>>("KenBanks");
            //var result = DataCaching.GetCache<string>("KenBanks");
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
                var urlService = urlBaseService + "bi/list?partner=" + Partner;



                var response = Task.Run(async () => await KenBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "Ken", "GetBanks Response", response, urlService });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<KenBankLib.BankResponse>(response);
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
                        //listBankObj = listBankObj.Where(x => x.BankCode != "ACB").ToList();
                        //listBankObj = listBankObj.Where(x => x.BankCode != "MB").ToList();
                        listBankObj = listBankObj.OrderBy(x => x.BankCode).ToList();
                        DataCaching.SetCache("KenBanks", listBankObj, 60 * 1);
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
            if (tran.Amount < 10000)
            {
                return new APIResponse((int)ResponseCode.BankAmountInvalid);
            }
            if (tran.BankName.ToUpper() == "MBB")
            {
                tran.BankName = "MB";
            }
            //if(tran.BankName.ToLower()=="momo")
            //    return new APIResponse((int)ResponseCode.TransactionFailed);
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

            if (addTran.BankCode.ToUpper() == "MOMO")
            {
                chargeType = "momo";
            }
            var add = addTran.Add();

            if (add > 0)
            {
                //var OrderNo = add.ToString();
                //var OrderNo = add.ToString();
                var urlService = $"{urlBaseService}bi/info?partner=THA&uid={add}&amount={tran.Amount}&bank_code={tran.BankName}&type=ctt";

                if (tran.BankName.ToUpper() == "MOMO")
                    urlService = urlService = $"{urlBaseService}mmi/info?partner=THA&uid={add}&amount={tran.Amount}";

                //var sign = md5(amount + chargeType + requestId + signKey)
               NLogLogger.Info(new string[] { "Ken", "Order Request", urlService });
                var response = Task.Run(async () => await KenBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "Ken", "Order Response", response });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<KenBankLib.OrderResponse>(response);


                    if (resObj.status == 1)
                    {
                        if (string.IsNullOrEmpty(resObj.data.account))
                        {
                            resObj.data.account = resObj.data.phone;
                        }

                        if (string.IsNullOrEmpty(resObj.data.account_name))
                        {
                            resObj.data.account_name = resObj.data.name;
                        }

                        addTran.TransactionID = add;
                        addTran.BankAccountName = resObj.data.account_name;
                        addTran.BankAccountNumber = resObj.data.account;
                        addTran.OrderNo = resObj.data.content;
                        addTran.UpdateBank();
                          
                        var orderRes = new Order()
                        {
                            Status = resObj.status.ToString(),
                            Amount = tran.Amount,
                            RefCode = tran.RefCode,
                            OrderNo = resObj.data.content,
                            Timeout = 30,
                            Url = String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?amount={2}&addInfo={3}", addTran.BankCode, resObj.data.account, resObj.data.amount, resObj.data.content),
                            BankName = tran.BankName,
                            BankAccountNumber = resObj.data.account,
                            BankAccountName = resObj.data.account_name
                        };
                        if (addTran.BankCode.ToUpper() == "MOMO")
                        {
                            orderRes.Url = String.Format("https://chart.googleapis.com/chart?cht=qr&chs=300x300&chl=2|99|{0}|{0}|{0}|0|0|{1}|{2}|transfer_myqr", resObj.data.account, resObj.data.amount, resObj.data.content);
                        }
                        var lstBank = DataCaching.GetCache<List<BankAccountV3>>("BankInfoKZ");
                        if (lstBank == null)
                        {
                            lstBank = new List<BankAccountV3>();

                        }
                        if (!lstBank.Exists(x => x.AccountId == resObj.data.account))
                        {
                            lstBank.Add(new BankAccountV3
                            {
                                AccountId = resObj.data.account,
                                AccountName = resObj.data.account_name,
                                BankCode = tran.BankName
                            });
                            DataCaching.SetCache("BankInfoKZ", lstBank, 86400 * 15);
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

        public APIResponse CallbackV2(KenBankLib.Callback callback)
        {

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public APIResponse Callback(KenBankLib.Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            if (string.IsNullOrEmpty(callback.bank_trans_id))
                callback.bank_trans_id = callback.momo_trans_id;
            var signature = Utils.Encrypts.MD5(callback.bank_trans_id + callback.message + callback.amount + PartnerCode);
            if (signature != callback.signature)
            {
                NLogLogger.Info(new string[] { "KZ", "Callback", "Signature Failed", signature, callback.signature });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }


            var order = new BankGateAPI().Get(long.Parse(callback.uid));
            if (order == null)
            {
                NLogLogger.Info(new string[] { "Ken", "Callback", "Order NULL", serializer.Serialize(callback) });
                var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                return apiResponse;
            }
         

            if (order.Status != (int)ResponseCode.TransactionSuccessful)
            {
                order.Status = (int)ResponseCode.TransactionSuccessful;
                order.TotalAmount = Convert.ToDecimal(callback.amount);
                order.LastTime = DateTime.Now;
                order.Mobile = callback.account;
                order.Email = callback.bank_trans_id;
                order.OrderInfo = callback.bank_trans_id;
                order.Update();
                //Action<string, long, string, long> send = UpdatePartnerBalance;
                //var asynSend = send.BeginInvoke(order.PartnerCode, Convert.ToInt64(callback.chargeAmount), order.BankCode, order.TransactionID, null, null);
            }
            apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
            var datacb = new DataCallback()
            {
                Content = order.OrderNo,
                TransId = order.RefCode,
                Amount = Convert.ToInt32(callback.amount),
                BankCode = order.BankCode,
                Mobile = callback.account,
                Result = callback.bank_trans_id,
                mTransId = callback.bank_trans_id
            };

            if (order.PartnerCode == "azt")
            {
                Action<string, long, string, long> send = UpdatePartnerBalance;
                var asynSend = send.BeginInvoke(order.PartnerCode, Convert.ToInt64(callback.amount), order.BankCode, order.TransactionID, null, null);
            }
            //Callback for Partner
            if (!string.IsNullOrEmpty(order.ReturnUrl))
            {
                var Partner = new Partners().Get(order.PartnerCode);
                datacb.Signature = PaymentUtils.Signature(datacb.TransId + datacb.Amount + datacb.Content, Partner.PrivateKey, Partner.SignatureType);
                //NLogLogger.Info(new string[] { "VNPay", "PartnerCallback", serializer.Serialize(datacb), order.ReturnUrl });
                Task.Run(async () => await KenBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID).ConfigureAwait(false));
            }

           

            return apiResponse;


            //return new APIResponse((int)ResponseCode.TransactionFailed);
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


