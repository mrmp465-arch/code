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
using static Libs.BankDirect.KZ.KZBankLib;

namespace Libs.BankDirect.KZ
{
    public class KZBank : IBankDirectV2Handler
    {


        private const string urlBaseService = " http://144.126.242.45/api/";


        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string Partner = "GRB";

        private const string PartnerCode = "08bdda2ce7a3ffe45d45bbd1f499fb96";

        public APIResponse CheckTrans(APITransaction transaction)
        {


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse GetBanksV2(string ParnerId, string AccoutName)
        {
            var requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var urlService = urlBaseService + "mmi/info?uid=" + requestTime + "&amount=20000&partner=" + Partner;

            // NLogLogger.Info(new string[] { "Bicbic", "GetBanks Request", urlService });
            //var response = Task.Run(async () => await KZBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
            var response = Task.Run(async () => await KZBankLib.GetTask(urlService)).Result;
            NLogLogger.Info(new string[] { "KZ", "GeMomo Response", response, urlService });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<KZBankLib.MomoResponse>(response);
                if (resObj != null)
                {
                    //var bank = serializer.Deserialize<KZBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                    var listBankObj = new List<BankAccountV2>();
                    if (resObj.status == 1)
                    {
                        listBankObj.Add(new BankAccountV2()
                        {

                            BankName = resObj.data.phone,
                            Name = resObj.data.name
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
        public APIResponse GetBanksV3(string ParnerId, string AccoutName)
        {
            var requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            return new APIResponse((int)ResponseCode.TransactionFailed);


        }
        public string GetBanksDrum()
        {

            string urlBaseServiceBank = "http://127.0.0.1:9002/BankService.ashx";
            //var Partner = new Partners().Get("order");
            var requestContent = "";
            //var signature = Encrypts.MD5(Partner.PartnerCode + "GETLIST" + requestContent + Partner.PublicKey);
            var requestData = new RequestData()
            {
                PartnerCode = "order",
                CommandCode = "GET_LIST",
                RequestContent = requestContent,
                Signature = ""
            };


            //NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), urlBaseServiceBank
            //    });
            var response = Task.Run(async () => await KZBankLib.PostTask(urlBaseServiceBank, serializer.Serialize(requestData))).Result;

            return response;
        }
        public APIResponse GetBanks(string PartnerCode)
        {
            var urlService = urlBaseService + "bi/list?partner=" + Partner;

            var result = DataCaching.GetCache<List<BankAccountV2>>("KZBanksV2");
            if (result != null)
            {
                return new APIResponse((int)ResponseCode.TransactionSuccessful)
                {
                    ResponseContent = serializer.Serialize(result)
                };
            }

            // NLogLogger.Info(new string[] { "Bicbic", "GetBanks Request", urlService });
            //var response = Task.Run(async () => await KZBankLib.PostTask(urlService, serializer.Serialize(bankRequest))).Result;
            var response = Task.Run(async () => await KZBankLib.GetTask(urlService)).Result;
            //NLogLogger.Info(new string[] { "KZ", "GetBanks Response", response, urlService });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<KZBankLib.BankResponse>(response);
                if (resObj != null)
                {
                    //var bank = serializer.Deserialize<KZBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                    var listBankObj = new List<BankAccountV2>();
                    foreach (var item in resObj.data)
                    {
                        listBankObj.Add(new BankAccountV2()
                        {

                            BankName = item.code,
                            Name = item.name
                        });
                    }



                    listBankObj = listBankObj.OrderBy(x => x.BankName).ToList();

                    var provider = new Providers().GetCache("drumbank");
                    if (provider.Status == 1)
                    {
                        listBankObj = listBankObj.Where(x => x.BankName != "TPB").ToList();
                        listBankObj = listBankObj.Where(x => x.BankName != "MB").ToList();
                        listBankObj = listBankObj.Where(x => x.BankName != "VCB").ToList();
                        listBankObj = listBankObj.Where(x => x.BankName != "BIDV").ToList();
                        try
                        {
                            var drumbank = GetBanksDrum();
                            if (!string.IsNullOrEmpty(drumbank))
                            {
                                if (drumbank.Contains("TPB"))
                                {
                                    listBankObj.Insert(1, new BankAccountV2()
                                    {

                                        BankName = "TPB",
                                        Name = "TPB"
                                    });
                                }
                                if (drumbank.Contains("MB"))
                                {
                                    listBankObj.Insert(1, new BankAccountV2()
                                    {

                                        BankName = "MB",
                                        Name = "MB Quân đội"
                                    });
                                }

                                if (drumbank.Contains("BIDV"))
                                {
                                    listBankObj.Insert(1, new BankAccountV2()
                                    {

                                        BankName = "BIDV",
                                        Name = "BIDV"
                                    });
                                }
                                if (drumbank.Contains("VCB"))
                                {
                                    listBankObj.Insert(1, new BankAccountV2()
                                    {

                                        BankName = "VCB",
                                        Name = "Vcb Ngoại Thương VN"
                                    });
                                }
                            }
                        }
                        catch
                        {

                        }
                        if (DateTime.Now.Hour > 15 || DateTime.Now.Hour < 8)
                        {
                            listBankObj = listBankObj.Where(x => x.BankName != "VIETINBANK").ToList();

                        }
                    }
                    DataCaching.SetCache("KZBanksV2", listBankObj, 60 * 3);
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = serializer.Serialize(listBankObj)
                    };
                }

                else
                {
                    try
                    {
                        var listBankObj = new List<BankAccountV2>();
                        var drumbank = GetBanksDrum();
                        if (!string.IsNullOrEmpty(drumbank))
                        {
                            if (drumbank.Contains("TPB"))
                            {
                                listBankObj.Insert(1, new BankAccountV2()
                                {

                                    BankName = "TPB",
                                    Name = "TPB"
                                });
                            }
                            if (drumbank.Contains("MB"))
                            {
                                listBankObj.Insert(0, new BankAccountV2()
                                {

                                    BankName = "MB",
                                    Name = "MB Quân đội"
                                });
                            }

                            if (drumbank.Contains("BIDV"))
                            {
                                listBankObj.Insert(1, new BankAccountV2()
                                {

                                    BankName = "BIDV",
                                    Name = "BIDV"
                                });
                            }
                            if (drumbank.Contains("VCB"))
                            {
                                listBankObj.Insert(1, new BankAccountV2()
                                {

                                    BankName = "VCB",
                                    Name = "Vcb Ngoại Thương VN"
                                });
                            }
                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                ResponseContent = serializer.Serialize(listBankObj)
                            };
                        }
                    }
                    catch
                    {

                    }
                }


            }
            else
            {
                try
                {
                    var listBankObj = new List<BankAccountV2>();
                    var drumbank = GetBanksDrum();
                    if (!string.IsNullOrEmpty(drumbank))
                    {
                        if (drumbank.Contains("TPB"))
                        {
                            listBankObj.Insert(1, new BankAccountV2()
                            {

                                BankName = "TPB",
                                Name = "TPB"
                            });
                        }
                        if (drumbank.Contains("MB"))
                        {
                            listBankObj.Insert(0, new BankAccountV2()
                            {

                                BankName = "MB",
                                Name = "MB Quân đội"
                            });
                        }

                        if (drumbank.Contains("BIDV"))
                        {
                            listBankObj.Insert(1, new BankAccountV2()
                            {

                                BankName = "BIDV",
                                Name = "BIDV"
                            });
                        }
                        if (drumbank.Contains("VCB"))
                        {
                            listBankObj.Insert(1, new BankAccountV2()
                            {

                                BankName = "VCB",
                                Name = "Vcb Ngoại Thương VN"
                            });
                        }
                        return new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {
                            ResponseContent = serializer.Serialize(listBankObj)
                        };
                    }
                }
                catch
                {

                }
            }
            return new APIResponse((int)ResponseCode.TransactionFailed);

        }
        private List<BankAccountV2> GetBanksAPI()
        {

            var result = DataCaching.GetCache<List<BankAccountV2>>("KZBanks");
            if (result != null)
            {
                return result;
            }
            var urlService = urlBaseService + "bi/list?partner=" + Partner;

            var response = Task.Run(async () => await KZBankLib.GetTask(urlService)).Result;
            // NLogLogger.Info(new string[] { "KZ", "GetBanks Response", response, urlService });

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<KZBankLib.BankResponse>(response);
                if (resObj != null)
                {
                    //var bank = serializer.Deserialize<KZBankLib.Bank>(Encrypts.Base64Decode(resObj.destinationInfo));

                    var listBankObj = new List<BankAccountV2>();
                    foreach (var item in resObj.data)
                    {
                        listBankObj.Add(new BankAccountV2()
                        {

                            BankName = item.code,
                            Name = item.name
                        });
                    }


                    //if (DateTime.Now.Hour <= 6 || DateTime.Now.Hour >= 21)
                    //{
                    //    listBankObj = listBankObj.Where(x => x.BankName != "VCB").ToList();

                    //}
                    DataCaching.SetCache("KZBanks", listBankObj, 60 * 5);
                    return listBankObj;
                }

                return new List<BankAccountV2>{new BankAccountV2()
                {

                    BankName = "VCB",
                    Name = "VCB",
                } };

            }

            return new List<BankAccountV2>();

        }
        public APIResponse Order(APITransaction transaction)
        {
            var tran = serializer.Deserialize<BankDirectV2Service.OrderRequest>(transaction.RequestContent);
            if (tran.Amount < 10000)
            {
                return new APIResponse((int)ResponseCode.BankAmountInvalid);
            }
            if (tran.BankName.ToLower() == "random" || tran.BankName.ToLower().StartsWith("ch") || string.IsNullOrEmpty(tran.BankName))
            {
                var lstbank = GetBanksAPI();
                var rand = new Random();
                tran.BankName = lstbank[rand.Next(lstbank.Count)].BankName;
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
                var urlService = $"{urlBaseService}bi/info?partner=GRB&uid={add}&amount={tran.Amount}&bank_code={tran.BankName}&type=ctt";
                if (tran.BankName.ToUpper() == "MOMO")
                    urlService = urlService = $"{urlBaseService}mmi/info?partner=GRB&uid={add}&amount={tran.Amount}";
                //NLogLogger.Info(new string[] { "KZ", "Order Request", urlService });
                var response = Task.Run(async () => await KZBankLib.GetTask(urlService)).Result;
                //NLogLogger.Info(new string[] { "KZ", "Order Response", response });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<KZBankLib.OrderResponse>(response);


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
                            //Timeout = 15,
                            QRCode = String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?amount={2}&addInfo={3}", addTran.BankCode, resObj.data.account, resObj.data.amount, resObj.data.content),
                            Url = String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?amount={2}&addInfo={3}", addTran.BankCode, resObj.data.account, resObj.data.amount, resObj.data.content),
                            //BankName = tran.BankName,
                            //BankAccountNumber = resObj.data.account,
                            //BankAccountName = resObj.data.account_name
                        };
                        if (tran.BankName.ToUpper() == "MOMO")

                        {
                            orderRes.QRCode = String.Format("https://chart.googleapis.com/chart?cht=qr&chs=300x300&chl=2|99|{0}|{0}|{0}|0|0|{1}|{2}|transfer_myqr", resObj.data.account, resObj.data.account, resObj.data.content);
                            orderRes.Url = resObj.data.deep_link;
                        }
                        else
                        {
                            var lstbank = GetBanksAPI();
                            //if (lstbank.Exists(x => x.BankName == orderRes.BankName))
                            //    orderRes.BankName = lstbank.FirstOrDefault(x => x.BankName == orderRes.BankName).Name;

                            var lstBankCache = DataCaching.GetCache<List<BankAccountV3>>("BankInfoKZ");
                            if (lstBankCache == null)
                            {
                                lstBankCache = new List<BankAccountV3>();

                            }
                            //if (!lstBankCache.Exists(x => x.AccountId == resObj.data.account))
                            //{
                            //    lstBankCache.Add(new BankAccountV3
                            //    {
                            //        AccountId = resObj.data.account,
                            //        AccountName = resObj.data.account_name,
                            //        BankCode = tran.BankName
                            //    });
                            //    DataCaching.SetCache("BankInfoKZ", lstBankCache, 86400 * 15);
                            //}

                            //bank
                        }

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

        public APIResponse CallbackV2(KZBankLib.Callback callback)
        {
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse OrderV2(APITransaction transaction)
        {
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public APIResponse Callback(KZBankLib.Callback callback)
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
                NLogLogger.Info(new string[] { "KZ", "Callback", "Order NULL", serializer.Serialize(callback) });
                var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                return apiResponse;
            }

            apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = serializer.Serialize(new DataCallback()
                {
                    RefCode = order.RefCode,
                    OrderNo = order.OrderNo,
                    Mobile = callback.account,
                    Amount = Convert.ToInt32(callback.amount),
                })
            };

            if (order.Status != (int)ResponseCode.TransactionSuccessful)
            {
                order.Status = (int)ResponseCode.TransactionSuccessful;
                order.TotalAmount = Convert.ToDecimal(callback.amount);
                order.LastTime = DateTime.Now;
                order.Mobile = callback.account;
                order.OrderInfo = callback.bank_trans_id;
                order.Update();

                Action<string, long, string, string, string> send = UpdatePartnerBalance;
                var asynSend = send.BeginInvoke(order.PartnerCode, Convert.ToInt64(callback.amount), order.BankCode, String.Format("Cộng tiền nạp bank số tiền: {3} mgd: {0}-{1}-{2}", order.TransactionID, order.BankCode, order.OrderNo, Convert.ToInt64(callback.amount).ToString("#,#").Replace(".", ",")), "BankIn_" + order.TransactionID.ToString(), null, null);

                //Callback for Partner
                if (!string.IsNullOrEmpty(order.ReturnUrl))
                {
                    var partner = new Partners().GetCache(order.PartnerCode);
                    apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                    Task.Run(async () => await KZBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID, order.RefCode + " " + order.OrderNo).ConfigureAwait(false));
                }

            }



            return apiResponse;
        }
        private void UpdatePartnerBalance(string PartnerCode, long Amount, string Type, string TranId, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Type, TranId.ToString(), RefCode });
                var partner = new Partners().GetCache(PartnerCode);
                if (string.IsNullOrEmpty(partner.Hotline))
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return;
                }
                var user = new Users().GetByUserName(partner.Hotline.Trim());
                if (user == null)
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return;
                }
                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
                if (listpartnerDiscount == null)
                {
                    TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                    return;

                var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
                decimal ck = _partnerDiscount.DiscountBANKTRANFER;
                if (Type == "MOMO")
                    ck = _partnerDiscount.DiscountMOMO;
                if (ck == 0)
                {
                    TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                long realAmount = Amount - Convert.ToInt64(Amount * ck);
                //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
                new Users().Topup(realAmount, user.UserName, PartnerCode, TranId, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }


        }
    }


}


