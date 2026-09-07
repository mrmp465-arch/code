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
using static Libs.BankDirect.MDrum.MDrumBankLib;
using Libs.BankDirect.MDrum;
using Libs.Report;
using System.Data.SqlTypes;
using Newtonsoft.Json;
using System.Web;







namespace Libs.BankDirect.MDrumV2
{
    public class MDrumV2Bank : IBankDirectV2Handler
    {

        //private const string username = "DRUM";
        //private const string username2 = "DRUM";
        //private const string username3 = "DRUM3";
        //private const string username4 = "DRUM4";
        private const string urlBaseService = "http://127.0.0.1:9002/MomoService.ashx";
        private const string urlBaseServiceBank = "http://127.0.0.1:9002/BankService.ashx";
        //private const string urlCallback = "https://bankgate.drumpal.info/callback/m32.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //private const string secretKey = "6Gx?zjjj737373872728282ssejedddp";
        //private const string secretKey2 = "6Gx?zjjj737373872728282ssejedddp";
        //private const string secretKey3 = "6Gx?zjjj737373872728282ssejedddp";
        //private const string secretKey4 = "6Gx?zjjj737373872728282ssejedddp";
        private const string serviceIp = "139.180.206.12";
        public APIResponse CheckTrans(APITransaction transaction)
        {
            //var urlService = urlBaseService + string.Format("/v1/orders/{0}/", serializer.Deserialize<BankDirectV2Service.CheckOrderRequest>(transaction.RequestContent).OrderNo);
            //NLogLogger.Info(new string[] { "MDrum", "CheckTrans Request", urlService });
            //var response = Task.Run(async () => await MDrumBankLib.GetTask(urlService)).Result;
            //NLogLogger.Info(new string[] { "MDrum", "CheckTrans Response", response, urlService });

            //if (!string.IsNullOrEmpty(response))
            //{
            //    var resObj = serializer.Deserialize<MDrumBankLib.OrderResponse>(response);
            //    var orderRes = new Order()
            //    {
            //        Status = resObj.status,
            //        Amount = Convert.ToInt32(resObj.note.target_amount),
            //        OrderNo = resObj.code,
            //        Timeout = resObj.timeout
            //    };

            //    //InitBank 



            //    return new APIResponse((int)ResponseCode.TransactionSuccessful)
            //    {
            //        ResponseContent = serializer.Serialize(orderRes)
            //    };

            //}

            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        //lấy tk momo
        public APIResponse GetBanksV2(string ParnerCode, string AccoutName)
        {
            var systemDataConfig = new SystemDataConfig();
            var lstConfig = systemDataConfig.GetListCache();

            if (systemDataConfig.GetKey(lstConfig, "MomoInEnable") == "0")
                return new APIResponse((int)ResponseCode.SystemMaintain);
            //if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour <= 12)
            //{
            //    return new APIResponse((int)ResponseCode.SystemBusy);
            //}
            //if (ParnerCode != "anhx2"&& ParnerCode != "hng")
            //return new APIResponse((int)ResponseCode.SystemBusy);
            var Partner = new Partners().Get(ParnerCode);
            var requestContent = "";
            var signature = Encrypts.MD5(Partner.PartnerCode + "GETLIST" + requestContent + Partner.PublicKey);
            var requestData = new RequestData()
            {
                PartnerCode = Partner.PartnerCode,
                CommandCode = "GETLIST",
                RequestContent = requestContent,
                Signature = signature
            };
            ///NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), urlBaseService)
            var response = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseService, serializer.Serialize(requestData))).Result;


            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                if (resObj.ResponseCode > 0)
                {
                    var bank = serializer.Deserialize<MDrumBankLib.Bank>(resObj.ResponseContent);

                    var listBankObj = new BankGate.Entity.MomoAccount()
                    {

                        MomoName = bank.MomoName,
                        MomoId = bank.MomoId,
                        //QR = $"https://quickchart.io/qr?text=2|99|{bank.MomoId}|{bank.MomoId}|{bank.MomoId}|0|0|5000000||transfer_myqr&size=200"
                    };
                    //listBankObj.Add();
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = serializer.Serialize(listBankObj)
                    };
                }
                else
                {
                    NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), urlBaseService
                });
                    NLogLogger.Info(new string[] { "MDrum", "GetBanks Response", response, urlBaseService
            });
                }

            }

            return new APIResponse((int)ResponseCode.TransactionFailed);

        }
        //lấy tk bank
        public APIResponse GetBanksV3(string ParnerCode, string AccoutName)
        {
            var systemDataConfig = new SystemDataConfig();
            var lstConfig = systemDataConfig.GetListCache();

            if (systemDataConfig.GetKey(lstConfig, "BankInEnable") == "0")
                return new APIResponse((int)ResponseCode.SystemMaintain);
            //if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour <= 12)
            //{
            //    return new APIResponse((int)ResponseCode.SystemBusy);
            //}
            //if (ParnerCode != "anhx2"&& ParnerCode != "hng")
            //return new APIResponse((int)ResponseCode.SystemBusy);
            var result = DataCaching.GetCache<List<BankAccountV4>>("DrumBanksV3_" + ParnerCode);
            if (result != null)
            {
                return new APIResponse((int)ResponseCode.TransactionSuccessful)
                {
                    ResponseContent = serializer.Serialize(result)
                };
            }
            var listBank = new List<BankAccountV4>();
            var _Bank = new BankAccounts();
            var data = _Bank.GetList().OrderBy(x => x.Id).Where(x => x.Status == 1 && x.Type.Contains("IN") && x.StatusExtra == 1 && x.StatusOverIn == 1).ToList();
            var allbank = new PartnerBank().GetList();
            var datapn = allbank.Where(x => x.Code == "order").ToList();
            if (ParnerCode == "sn2")
            {
                datapn = allbank.Where(x => x.Code == "sn").ToList();
            }
            var bankgroup = data.GroupBy(x => x.BankCode).Select(x => x.Key);
            var listBankObj = new List<BankAccountV4>();
            foreach (var item in data)
            {
                if (datapn.Exists(x => x.BankId == item.BankId))
                {
                    var bankitem = new BankAccountV4()
                    {

                        BankName = item.BankCode,
                        Name = getBankName(item.BankCode),
                        BankAccountName = item.BankName,
                        BankAccountNumber = item.BankId,

                    };
                    listBankObj.Add(bankitem);
                }

            }
            bankgroup = bankgroup.OrderByDescending(x => x).ToList();
            foreach (var item in bankgroup)
            {
                var BankCodeInMaintain = systemDataConfig.GetKey(lstConfig, "BankCodeInMaintain");

                // var random = new Random();
                var bankitem = listBankObj.Where(a => a.BankName == item).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                //if (DateTime.Now.Hour % 4 > 1)
                //{
                //    bankitem = listBankObj.Where(a => a.BankName == item).LastOrDefault();
                //}


                if (!BankCodeInMaintain.Contains(item))
                {
                    listBank.Add(bankitem);
                }


            }

            DataCaching.SetCache("DrumBanksV3_" + ParnerCode, listBank, 60 * 2);
            return new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = serializer.Serialize(listBank)
            };

            //return new APIResponse((int)ResponseCode.TransactionFailed);

        }

        public List<BankAccountV2> GetBanksAPI(string partnercode)
        {

            var result = DataCaching.GetCache<List<BankAccountV2>>("DrumBanks_" + partnercode);
            if (result != null)
            {
                return result;
            }
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

            //var Partner = new Partners().Get(partnercode);
            ////check theo partner
            //if (!string.IsNullOrEmpty(Partner.SMSPlusCheckUrl))
            //{
            //    requestData.PartnerCode = Partner.SMSPlusCheckUrl;
            //}

            var response = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseServiceBank, serializer.Serialize(requestData))).Result;

            var systemDataConfig = new SystemDataConfig();
            var lstConfig = systemDataConfig.GetListCache();
            var BankCodeInMaintain = systemDataConfig.GetKey(lstConfig, "BankCodeInMaintain");

            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                if (resObj.ResponseCode > 0)
                {
                    var bank = serializer.Deserialize<List<MDrumBankLib.BankV2>>(resObj.ResponseContent);

                    var bankgroup = bank.GroupBy(x => x.BankCode).Select(x => x.Key);
                    var listBankObj = new List<BankAccountV2>();
                    foreach (var item in bankgroup)
                    {
                        if (!BankCodeInMaintain.ToUpper().Contains(item.ToUpper()))
                        {
                            listBankObj.Add(new BankAccountV2()
                            {

                                BankName = item,
                                Name = getBankName(item)
                            });
                        }
                    }
                    //if ((DateTime.Now.Hour == 21 && DateTime.Now.Minute >= 50) || (DateTime.Now.Hour == 22 && DateTime.Now.Minute <= 5))
                    //{
                    //    listBankObj = listBankObj.Where(x => x.BankName != "VCB").ToList();
                    //}
                    DataCaching.SetCache("DrumBanks_" + partnercode, listBankObj, 60 * 1);
                    return listBankObj;


                }

            }
            return new List<BankAccountV2>();

        }
        public List<BankAccountV3> GetBanksAPIV2(string partnercode)
        {

            var result = DataCaching.GetCache<List<BankAccountV3>>("DrumBanksAPI_" + partnercode);
            if (result != null)
            {
                return result;
            }
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


            var response = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseServiceBank, serializer.Serialize(requestData))).Result;


            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                if (resObj.ResponseCode > 0)
                {
                    var bank = serializer.Deserialize<List<BankAccountV3>>(resObj.ResponseContent);


                    //if ((DateTime.Now.Hour == 21 && DateTime.Now.Minute >= 50) || (DateTime.Now.Hour == 22 && DateTime.Now.Minute <= 5))
                    //{
                    //    listBankObj = listBankObj.Where(x => x.BankName != "VCB").ToList();
                    //}
                    DataCaching.SetCache("DrumBanksAPI_" + partnercode, bank, 20 * 1);
                    return bank;


                }

            }
            return new List<BankAccountV3>();

        }
        public APIResponse GetBanks(string partnercode)
        {
            var result = DataCaching.GetCache<List<BankAccountV2>>("DrumBanksNew_" + partnercode);
            if (result != null)
            {
                //if (partnercode.Contains("mark"))
                //{
                //    result = result.Where(x => x.BankName != "VCB").ToList();
                //    result = result.Where(x => x.BankName != "BIDV").ToList();
                //    result = result.Where(x => x.BankName != "MB").ToList();
                //}
                return new APIResponse((int)ResponseCode.TransactionSuccessful)
                {
                    ResponseContent = serializer.Serialize(result)
                };
            }

            var requestContent = "";
            //var signature = Encrypts.MD5(Partner.PartnerCode + "GETLIST" + requestContent + Partner.PublicKey);
            var requestData = new RequestData()
            {
                PartnerCode = "order",
                CommandCode = "GET_LIST",
                RequestContent = requestContent,
                Signature = ""
            };
            //var Partner = new Partners().GetCache(partnercode);
            //if (!string.IsNullOrEmpty(Partner.SMSPlusCheckUrl))
            //{
            //    requestData.PartnerCode = Partner.SMSPlusCheckUrl;
            //}


            var response = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseServiceBank, serializer.Serialize(requestData))).Result;


            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                if (resObj.ResponseCode > 0)
                {
                    var bank = serializer.Deserialize<List<MDrumBankLib.BankV2>>(resObj.ResponseContent);
                    var bankgroup = bank.GroupBy(x => x.BankCode).Select(x => x.Key);
                    var listBankObj = new List<BankAccountV2>();

                    var systemDataConfig = new SystemDataConfig();
                    var lstConfig = systemDataConfig.GetListCache();
                    var BankCodeInMaintain = systemDataConfig.GetKey(lstConfig, "BankCodeInMaintain");
                    foreach (var item in bankgroup)
                    {


                        if (!BankCodeInMaintain.ToUpper().Contains(item.ToUpper()))
                        {
                            listBankObj.Add(new BankAccountV2()
                            {

                                BankName = item,
                                Name = getBankName(item)
                            });
                        }

                    }
                    listBankObj = listBankObj.OrderBy(x => x.BankName).ToList();

                    //if ((DateTime.Now.Hour == 21 && DateTime.Now.Minute >= 45) || (DateTime.Now.Hour == 22 && DateTime.Now.Minute <= 9))
                    //{
                    //    listBankObj = listBankObj.Where(x => x.BankName != "VCB").ToList();
                    //}

                    //if (DateTime.Now.Hour % 3 == 2 || DateTime.Now.Hour % 3 == 1 || DateTime.Now.Hour == 15 || DateTime.Now.Hour == 18 || DateTime.Now.Hour == 9 | DateTime.Now.Hour == 12)
                    //{
                    //    if (listBankObj.Exists(x => x.BankName == "VPB"))
                    //    {
                    //        listBankObj = listBankObj.Where(x => x.BankName != "VPB").ToList();
                    //        listBankObj.Insert(0, new BankAccountV2()
                    //        {

                    //            BankName = "VPB",
                    //            Name = getBankName("VPB")
                    //        });
                    //    }
                    //}
                    if ((DateTime.Now.Hour == 21 && DateTime.Now.Minute >= 45) || (DateTime.Now.Hour == 22 && DateTime.Now.Minute <= 5))
                    {
                        listBankObj = listBankObj.Where(x => x.BankName != "VCB").ToList();
                    }
                    if ((DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 39) || (DateTime.Now.Hour == 0 && DateTime.Now.Minute <= 29))
                    {
                        listBankObj = listBankObj.Where(x => x.BankName != "VCB").ToList();
                    }
                    if (DateTime.Now.Hour < 3 || DateTime.Now.Hour >= 23)
                    {
                        listBankObj = listBankObj.Where(x => x.BankName != "ICB").ToList();
                    }
                    if (listBankObj.Exists(x => x.BankName == "VPB"))
                    {
                        listBankObj = listBankObj.Where(x => x.BankName != "VPB").ToList();
                        listBankObj.Insert(0, new BankAccountV2()
                        {

                            BankName = "VPB",
                            Name = getBankName("VPB")
                        });
                    }
                    //if ((DateTime.Now.Hour == 13|| DateTime.Now.Hour == 12) && DateTime.Now.Minute >= 30)
                    //{
                    //    if (listBankObj.Exists(x => x.BankName == "VPB"))
                    //    {
                    //        listBankObj = listBankObj.Where(x => x.BankName != "VPB").ToList();
                    //        listBankObj.Insert(0, new BankAccountV2()
                    //        {

                    //            BankName = "VPB",
                    //            Name = getBankName("VPB")
                    //        });
                    //    }
                    //}
                    //if ((int)DateTime.Now.DayOfWeek >= 1 && (int)DateTime.Now.DayOfWeek <= 3)
                    //{
                    //    if (listBankObj.Exists(x => x.BankName == "VPB"))
                    //    {
                    //        listBankObj = listBankObj.Where(x => x.BankName != "VPB").ToList();
                    //        listBankObj.Insert(0, new BankAccountV2()
                    //        {

                    //            BankName = "VPB",
                    //            Name = getBankName("VPB")
                    //        });
                    //    }

                    //}

                    //if (( DateTime.Now.Hour == 11 || DateTime.Now.Hour == 13 || DateTime.Now.Hour == 14) && DateTime.Now.Minute>=50)
                    //{
                    //    listBankObj = listBankObj.OrderBy(x => x.BankName).ToList();

                    //}
                    //if (DateTime.Now.Minute <= 15 && DateTime.Now.Hour > 8 && DateTime.Now.Hour <= 21)
                    //{
                    //    if (listBankObj.Exists(x => x.BankName == "VCB"))
                    //    {
                    //        listBankObj = listBankObj.Where(x => x.BankName != "VCB").ToList();
                    //        listBankObj.Insert(0, new BankAccountV2()
                    //        {

                    //            BankName = "VCB",
                    //            Name = getBankName("VCB")
                    //        });
                    //    }
                    //}

                    //if (DateTime.Now.Minute < 6 && DateTime.Now.Hour >9 && DateTime.Now.Hour <= 20)
                    //{
                    //    if (listBankObj.Exists(x => x.BankName == "SEAB"))
                    //    {
                    //        listBankObj = listBankObj.Where(x => x.BankName != "SEAB").ToList();
                    //        listBankObj.Insert(0, new BankAccountV2()
                    //        {

                    //            BankName = "SEAB",
                    //            Name = getBankName("SEAB")
                    //        });
                    //    }

                    //}

                    DataCaching.SetCache("DrumBanksNew_" + partnercode, listBankObj, 60);
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = serializer.Serialize(listBankObj)
                    };
                }

            }
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
        public List<BankAccountV4> GetBanksAPIV4(string ParnerCode)
        {

            var systemDataConfig = new SystemDataConfig();
            var lstConfig = systemDataConfig.GetListCache();
            var result = DataCaching.GetCache<List<BankAccountV4>>("DrumBanksV4_" + ParnerCode);
            if (result != null)
            {
                return result;
            }
            var listBank = new List<BankAccountV4>();
            var _Bank = new BankAccounts();
            var data = _Bank.GetList().OrderBy(x => x.Id).Where(x => x.Status == 1 && x.Type.Contains("IN") && x.StatusExtra == 1 && x.StatusOverIn == 1).ToList();

            //var allbank = new PartnerBank().GetList();
            //var datapn = allbank.Where(x => x.Code == "order").ToList();


            //NLogLogger.Info("banklist" +serializer.Serialize(data));
            //var bankgroup = data.GroupBy(x => x.BankCode).Select(x => x.Key);
            //var listBankObj = new List<BankAccountV4>();
            var BankCodeInMaintain = systemDataConfig.GetKey(lstConfig, "BankCodeInMaintain");
            foreach (var item in data)
            {
                var bankitem = new BankAccountV4()
                {

                    BankName = item.BankCode,
                    Name = getBankName(item.BankCode),
                    BankAccountName = item.BankName,
                    BankAccountNumber = item.BankId,

                };
                if (!BankCodeInMaintain.Contains(item.BankCode))
                {
                    listBank.Add(bankitem);
                }

            }
            //NLogLogger.Info("banklist" + serializer.Serialize(listBankObj));


            if ((DateTime.Now.Hour == 21 && DateTime.Now.Minute >= 45) || (DateTime.Now.Hour == 22 && DateTime.Now.Minute <= 5))
            {
                listBank = listBank.Where(x => x.BankName != "VCB").ToList();
            }
            if ((DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 39) || (DateTime.Now.Hour == 0 && DateTime.Now.Minute <= 29))
            {
                listBank = listBank.Where(x => x.BankName != "VCB").ToList();
            }
            if (DateTime.Now.Hour < 3 || DateTime.Now.Hour >= 23)
            {
                listBank = listBank.Where(x => x.BankName != "ICB").ToList();
            }
            if (DateTime.Now.Hour == 22 && DateTime.Now.Minute >= 48)
            {
                listBank = listBank.Where(x => x.BankName != "ICB").ToList();
            }

            DataCaching.SetCache("DrumBanksV4_" + ParnerCode, listBank, 60 * 1);
            return listBank;

            //return new APIResponse((int)ResponseCode.TransactionFailed);

        }
        public APIResponse OrderV2(APITransaction transaction)
        {
            try
            {

                var systemDataConfig = new SystemDataConfig();
                var lstConfig = systemDataConfig.GetListCache();

                var tran = serializer.Deserialize<BankDirectV2Service.OrderRequestV2>(transaction.RequestContent);
                tran.BankName = tran.BankName.ToUpper();

                if (tran.BankName.ToUpper() == "MOMO")
                {
                    if (transaction.PartnerCode != "pp")
                    {
                        if (systemDataConfig.GetKey(lstConfig, "MomoInEnable") == "0")
                            return new APIResponse((int)ResponseCode.SystemMaintain);
                    }
                }
                else
                {
                    if (transaction.PartnerCode != "pp")
                    {
                        if (systemDataConfig.GetKey(lstConfig, "BankInEnable") == "0")
                            return new APIResponse((int)ResponseCode.SystemMaintain);
                    }
                    if ((DateTime.Now.Hour == 21 && DateTime.Now.Minute >= 50) || (DateTime.Now.Hour == 22 && DateTime.Now.Minute <= 5))
                    {
                        if (tran.BankName.ToUpperInvariant() == "VCB")
                            return new APIResponse((int)ResponseCode.BankCodeMaintain);
                    }
                    var BankCodeInMaintain = systemDataConfig.GetKey(lstConfig, "BankCodeInMaintain");
                    var BanksAPI = GetBanksAPIV2(transaction.PartnerCode);


                    if (!BanksAPI.Exists(x => x.BankId == tran.BankAccountNumber))
                    {
                        return new APIResponse((int)ResponseCode.BankCodeMaintain);
                    }
                    if (BankCodeInMaintain.Contains(tran.BankName))
                    {
                        return new APIResponse((int)ResponseCode.SystemMaintain);
                    }
                }
                //gọi api
                var requestData = new RequestData()
                {
                    PartnerCode = "order",
                    CommandCode = "GET_ORDER",
                    Source = transaction.PartnerCode,
                    RequestContent = tran.Amount.ToString(),
                    Signature = ""
                };

                var url = urlBaseServiceBank;
                if (tran.BankName.ToUpper() == "MOMO")
                {
                    url = urlBaseService;

                }
                else
                {
                    var bankrequest = new RequestBank
                    {
                        Amount = tran.Amount,
                        BankCode = tran.BankName
                    };
                    if (bankrequest.Amount < 20000)
                        bankrequest.Amount = 20000;
                    requestData.RequestContent = serializer.Serialize(bankrequest);

                }
                if (tran.OrderNo.Length != 10)
                {
                    return new APIResponse((int)ResponseCode.ParameterInvalid);
                }
                var order = new BankGateAPI().Get(tran.OrderNo);
                if (order != null)
                {
                    return new APIResponse((int)ResponseCode.TransactionDuplicate);
                }
                //fake
                var addTran1 = new BankGateAPI()
                {
                    PartnerID = transaction.PartnerID,
                    PartnerCode = transaction.PartnerCode,
                    ProviderCode = transaction.ProviderCode,
                    //OrderNo = "A", // Tự động lấy bằng ID Table
                    OrderInfo = string.Empty,
                    Amount = 0,
                    TotalAmount = 0, // Hứng tiền sau khi xử lý thật
                    Currency = "VND",
                    ReturnUrl = tran.CallbackUrl,
                    RequestTime = 0,
                    Signature = Encrypts.MD5(DateTime.Now.ToString()),
                    LogContent = "Add Order",
                    BankCode = tran.BankName.ToUpper(),
                    FullName = tran.RefCode,
                    Mobile = string.Empty,
                    RefCode = tran.RefCode,
                    //BankAccountName = tran.BankAccountName,
                    //BankAccountNumber = tran.BankAccountNumber,

                };
                //addTran1.BankAccountName = "Test123";
                //addTran1.BankAccountNumber = "Test123";

                //addTran1.OrderNo = "Test123";
                //addTran.Amount = tran.Amount;
                //addTran1.Add();
                //return new APIResponse((int)ResponseCode.TransactionSuccessful);

                //gọi api
                var response = Task.Run(async () => await MDrumBankLib.PostTask(url, serializer.Serialize(requestData))).Result;

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                    if (resObj.ResponseCode < 0)
                    {
                        var key = "DrumBanksV4_" + transaction.PartnerCode;
                        DataCaching.RemoveCache(key);
                        var BanksAPI = GetBanksAPIV4(transaction.PartnerCode);

                        var lstbank = BanksAPI;
                        if (lstbank.Count > 0)
                        {

                            var rand = new Random();
                            tran.BankName = lstbank[rand.Next(lstbank.Count)].BankName;

                            var bankrequest = new RequestBank
                            {
                                Amount = tran.Amount,
                                BankCode = tran.BankName
                            };
                            if (bankrequest.Amount < 20000)
                                bankrequest.Amount = 20000;
                            requestData.RequestContent = serializer.Serialize(bankrequest);
                        }


                        response = Task.Run(async () => await MDrumBankLib.PostTask(url, serializer.Serialize(requestData))).Result;


                        resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                    }
                    if (resObj.ResponseCode > 0)
                    {

                        var bankinfo = serializer.Deserialize<MDrumBankLib.BankAccountReceive>(resObj.ResponseContent);
                        bankinfo.BankId = tran.BankAccountNumber;
                        bankinfo.BankName = tran.BankAccountName;
                        bankinfo.OrderNo = tran.OrderNo;
                        var addTran = new BankGateAPI()
                        {
                            PartnerID = transaction.PartnerID,
                            PartnerCode = transaction.PartnerCode,
                            ProviderCode = transaction.ProviderCode,
                            //OrderNo = "A", // Tự động lấy bằng ID Table
                            OrderInfo = string.Empty,
                            Amount = 0,
                            TotalAmount = 0, // Hứng tiền sau khi xử lý thật
                            Currency = "VND",
                            ReturnUrl = tran.CallbackUrl,
                            RequestTime = 0,
                            Signature = Encrypts.MD5(DateTime.Now.ToString()),
                            LogContent = "Add Order",
                            BankCode = tran.BankName.ToUpper(),
                            FullName = tran.RefCode,
                            Mobile = string.Empty,
                            RefCode = tran.RefCode,
                            //BankAccountName = tran.BankAccountName,
                            //BankAccountNumber = tran.BankAccountNumber,

                        };
                        addTran.BankAccountName = bankinfo.BankName;
                        addTran.BankAccountNumber = bankinfo.BankId;
                        addTran.OrderNo = bankinfo.OrderNo;
                        addTran.Amount = tran.Amount;
                        var add = addTran.Add();

                        if (add > 0)
                        {

                            bankinfo.Amount = tran.Amount;
                            bankinfo.BankCode = tran.BankName;
                            bankinfo.CreateDate = DateTime.Now;
                            bankinfo.Status = "0";
                            bankinfo.Key = add.ToString();
                            if (bankinfo.BankCode.ToUpper() != "MOMO")
                            {
                                bankinfo.QRCodeBase64 = "";
                            }
                            MDrumBankLib.LogBankInfo(bankinfo);

                            addTran.TransactionID = add;


                            //addTran.OrderNo = bankinfo.OrderNo;
                            //addTran.UpdateBank();
                            var orderRes = new Order()
                            {
                                Status = "1",
                                Amount = tran.Amount,
                                RefCode = tran.RefCode,
                                OrderNo = bankinfo.OrderNo,
                                QRCode = bankinfo.QRCodeBase64,
                                //LinkOpenApp = bankinfo.LinkOpenApp,
                                Timeout = 60 * 10,
                                Url = String.Format("https://info.topapp.biz/Pages/Momo.aspx?orderNo={0}", add.ToString()),
                                BankCode = tran.BankName,
                                AccountNumber = bankinfo.MomoId,
                                AccountName = bankinfo.MomoName
                            };

                            if (tran.BankName.ToUpper() != "MOMO")
                            {
                                orderRes.AccountName = bankinfo.BankName;
                                orderRes.AccountNumber = bankinfo.BankId;
                                var VietUrl = $"https://img.vietqr.io/image/{addTran.BankCode}-{bankinfo.BankId}-compact.jpg?amount={orderRes.Amount}&addInfo={HttpUtility.UrlEncode(orderRes.OrderNo)}";

                                orderRes.Url = String.Format("https://info.topapp.biz/Pages/Bank.aspx?orderNo={0}", add.ToString());
                                orderRes.QRCode = VietUrl;


                            }
                            //if (tran.BankName.ToUpper() != "MOMO")
                            //{
                            //    orderRes.BankAccountName = bankinfo.BankName;
                            //    orderRes.BankAccountNumber = bankinfo.BankId;
                            //    orderRes.LinkOpenApp = "";
                            //    orderRes.LinkWebView = "";
                            //    orderRes.QRCode = String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?amount={2}&addInfo={3}", addTran.BankCode, bankinfo.BankId, bankinfo.Amount, bankinfo.OrderNo);
                            //    //orderRes.QRCodeBase64 = bankinfo.QRCodeBase64;
                            //    orderRes.Url = "";
                            //    //if (transaction.PartnerCode.StartsWith("mark"))
                            //    //{
                            //    //    orderRes.QRCodeBase64 = "";
                            //    //}
                            //    orderRes.QRCodeBase64 = "";
                            //}

                            string jsonContent = JsonConvert.SerializeObject(orderRes, new JsonSerializerSettings
                            {
                                StringEscapeHandling = StringEscapeHandling.Default
                            });
                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                ResponseContent = jsonContent
                            };
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


                    }
                    NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), url,response
                    });

                    return new APIResponse((int)ResponseCode.TransactionFailed);

                }
                NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), url
                    });


                return new APIResponse((int)ResponseCode.TransactionFailed);

                //InitBank 



                //return new APIResponse((int)ResponseCode.TransactionFailed);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Order", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.SystemError);
            }
        }
        public APIResponse Order(APITransaction transaction)
        {
            if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 58)
            {
                return new APIResponse((int)ResponseCode.SystemMaintain);
            }
            try
            {

                var systemDataConfig = new SystemDataConfig();
                var lstConfig = systemDataConfig.GetListCache();
                var bankprefix = GetSplit(systemDataConfig.GetKey(lstConfig, "BankPrefix")).Trim();

                var tran = serializer.Deserialize<BankDirectV2Service.OrderRequest>(transaction.RequestContent);
                tran.BankName = tran.BankName.ToUpper();

                if (tran.BankName.ToUpper() == "MOMO")
                {
                    if (transaction.PartnerCode != "pp")
                    {
                        if (systemDataConfig.GetKey(lstConfig, "MomoInEnable") == "0")
                            return new APIResponse((int)ResponseCode.SystemMaintain);
                    }
                }
                else
                {
                    if (transaction.PartnerCode != "pp")
                    {
                        if (systemDataConfig.GetKey(lstConfig, "BankInEnable") == "0")
                            return new APIResponse((int)ResponseCode.SystemMaintain);
                    }
                    if ((DateTime.Now.Hour == 21 && DateTime.Now.Minute >= 45) || (DateTime.Now.Hour == 22 && DateTime.Now.Minute <= 5))
                    {
                        if (tran.BankName.ToUpperInvariant() == "VCB")
                            return new APIResponse((int)ResponseCode.BankCodeMaintain);
                    }
                    if ((DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 39) || (DateTime.Now.Hour == 0 && DateTime.Now.Minute <= 29))
                    {
                        if (tran.BankName.ToUpperInvariant() == "VCB")
                            return new APIResponse((int)ResponseCode.BankCodeMaintain);
                    }
                    if (DateTime.Now.Hour < 3 || DateTime.Now.Hour >= 23)
                    {
                        if (tran.BankName.ToUpperInvariant() == "ICB")
                            return new APIResponse((int)ResponseCode.BankCodeMaintain);
                    }
                    var BankCodeInMaintain = systemDataConfig.GetKey(lstConfig, "BankCodeInMaintain");
                    var BanksAPI = GetBanksAPIV4(transaction.PartnerCode);
                    //random
                    if (tran.BankName.ToLower() == "random" || tran.BankName.ToLower().StartsWith("ch") || string.IsNullOrEmpty(tran.BankName))
                    {

                        var lstbank = BanksAPI;
                        if (lstbank.Count > 0)
                        {

                            var rand = new Random();
                            tran.BankName = lstbank[rand.Next(lstbank.Count)].BankName;
                            //if (DateTime.Now.Second % 2 == 0)
                            //{


                            //        tran.BankName = "VPB";
                            //}

                        }
                        else
                        {
                            return new APIResponse((int)ResponseCode.SystemMaintain);
                        }

                    }
                    if (BankCodeInMaintain.ToUpper().Contains(tran.BankName.ToUpper()))
                    {
                        return new APIResponse((int)ResponseCode.SystemMaintain);
                    }
                }

                //gọi api
                var requestData = new RequestData()
                {
                    PartnerCode = "order",
                    CommandCode = "GET_ORDER",
                    Source = transaction.PartnerCode,
                    RequestContent = tran.Amount.ToString(),
                    Signature = ""
                };

                var url = urlBaseServiceBank;
                if (tran.BankName.ToUpper() == "MOMO")
                {
                    url = urlBaseService;

                }
                else
                {
                    var bankrequest = new RequestBank
                    {
                        Amount = tran.Amount,
                        BankCode = tran.BankName
                    };
                    if (bankrequest.Amount < 20000)
                        bankrequest.Amount = 20000;
                    requestData.RequestContent = serializer.Serialize(bankrequest);

                }



                //gọi api
                var response = Task.Run(async () => await MDrumBankLib.PostTask(url, serializer.Serialize(requestData))).Result;

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                    if (resObj.ResponseCode < 0)
                    {
                        var key = "DrumBanksV4_" + transaction.PartnerCode;
                        DataCaching.RemoveCache(key);
                        var BanksAPI = GetBanksAPIV4(transaction.PartnerCode);

                        var lstbank = BanksAPI;
                        if (lstbank.Count > 0)
                        {

                            var rand = new Random();
                            tran.BankName = lstbank[rand.Next(lstbank.Count)].BankName;

                            var bankrequest = new RequestBank
                            {
                                Amount = tran.Amount,
                                BankCode = tran.BankName
                            };
                            if (bankrequest.Amount < 20000)
                                bankrequest.Amount = 20000;
                            requestData.RequestContent = serializer.Serialize(bankrequest);
                        }

                        response = Task.Run(async () => await MDrumBankLib.PostTask(url, serializer.Serialize(requestData))).Result;
                        resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                    }
                    if (resObj.ResponseCode > 0)
                    {

                        var bankinfo = serializer.Deserialize<MDrumBankLib.BankAccountReceive>(resObj.ResponseContent);
                        var FullOrderNo = bankprefix + " " + bankinfo.OrderNo;

                        if (tran.BankName == "MOMO")
                        {
                            FullOrderNo = bankinfo.OrderNo;
                        }
                        //if (tran.BankName.ToUpper() == "MOMO")
                        //{
                        //    FullOrderNo = bankinfo.OrderNo;
                        //}
                        var addTran = new BankGateAPI()
                        {
                            PartnerID = transaction.PartnerID,
                            PartnerCode = transaction.PartnerCode,
                            ProviderCode = transaction.ProviderCode,
                            //OrderNo = "A", // Tự động lấy bằng ID Table
                            OrderInfo = string.Empty,
                            Amount = 0,
                            TotalAmount = 0, // Hứng tiền sau khi xử lý thật
                            Currency = "VND",
                            ReturnUrl = tran.CallbackUrl,
                            RequestTime = 0,
                            Signature = Encrypts.MD5(DateTime.Now.ToString()),
                            LogContent = "Add Order",
                            BankCode = tran.BankName.ToUpper(),
                            FullName = FullOrderNo,
                            Mobile = string.Empty,
                            Email = FullOrderNo,
                            RefCode = tran.RefCode,
                            //BankAccountName = tran.BankAccountName,
                            //BankAccountNumber = tran.BankAccountNumber,

                        };
                        addTran.BankAccountName = bankinfo.MomoName;
                        addTran.BankAccountNumber = bankinfo.MomoId;
                        if (tran.BankName.ToUpper() != "MOMO")
                        {
                            addTran.BankAccountName = bankinfo.BankName;
                            addTran.BankAccountNumber = bankinfo.BankId;
                        }
                        addTran.OrderNo = bankinfo.OrderNo;
                        addTran.Amount = tran.Amount;
                        var add = addTran.Add();

                        if (add > 0)
                        {

                            bankinfo.Amount = tran.Amount;
                            bankinfo.BankCode = tran.BankName;
                            bankinfo.CreateDate = DateTime.Now;
                            bankinfo.OrderNo = FullOrderNo;
                            bankinfo.Status = "0";
                            bankinfo.Key = add.ToString();
                            if (bankinfo.BankCode.ToUpper() != "MOMO")
                            {
                                bankinfo.QRCodeBase64 = "";
                            }
                            else
                            {
                                //var imageUrl = $"https://quickchart.io/qr?text=2|99|{bankinfo.MomoId}|{bankinfo.MomoId}|{bankinfo.MomoId}|0|0|{bankinfo.Amount}|{HttpUtility.UrlEncode(bankinfo.OrderNo)}|transfer_myqr&size=300";
                                //using (WebClient client = new WebClient())
                                //{
                                //    // Tải ảnh dưới dạng mảng byte
                                //    byte[] imageBytes = client.DownloadData(imageUrl);

                                //    // Chuyển mảng byte sang chuỗi base64
                                //    string base64String = Convert.ToBase64String(imageBytes);
                                //    bankinfo.QRCodeBase64 = base64String;
                                //    //NLogLogger.Info(new string[] { base64String });
                                //}
                            }
                            MDrumBankLib.LogBankInfo(bankinfo);

                            addTran.TransactionID = add;


                            //addTran.OrderNo = bankinfo.OrderNo;
                            //addTran.UpdateBank();

                            var orderRes = new Order()
                            {
                                Status = "1",
                                Amount = tran.Amount,
                                RefCode = tran.RefCode,
                                OrderNo = bankinfo.OrderNo,
                                QRCode = bankinfo.QRCodeBase64,
                                Timeout = 60 * 10,
                                //LinkOpenApp = bankinfo.LinkOpenApp,
                                //LinkWebView = String.Format("https://info.topapp.biz/Pages/Momo.aspx?orderNo={0}", add.ToString()),
                                Url = String.Format("https://info.topapp.biz/Pages/Momo.aspx?orderNo={0}", add.ToString()),
                                BankCode = tran.BankName,
                                AccountNumber = bankinfo.MomoId,
                                AccountName = bankinfo.MomoName
                            };

                            if (tran.BankName.ToUpper() != "MOMO")
                            {
                                orderRes.AccountName = bankinfo.BankName;
                                orderRes.AccountNumber = bankinfo.BankId;
                                var VietUrl = $"https://img.vietqr.io/image/{addTran.BankCode}-{bankinfo.BankId}-compact.jpg?amount={orderRes.Amount}&addInfo={HttpUtility.UrlEncode(orderRes.OrderNo)}";

                                orderRes.Url = String.Format("https://info.topapp.biz/Pages/Bank.aspx?orderNo={0}", add.ToString());
                                orderRes.QRCode = VietUrl;


                            }
                            string jsonContent = JsonConvert.SerializeObject(orderRes, new JsonSerializerSettings
                            {
                                StringEscapeHandling = StringEscapeHandling.Default
                            });
                            return new APIResponse((int)ResponseCode.TransactionSuccessful)
                            {
                                ResponseContent = jsonContent
                            };
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


                    }
                    NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), url,response
                    });

                    return new APIResponse((int)ResponseCode.TransactionFailed);

                }
                NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), url
                    });


                return new APIResponse((int)ResponseCode.TransactionFailed);

                //InitBank 



                //return new APIResponse((int)ResponseCode.TransactionFailed);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Order", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.SystemError);
            }
        }
        //callback tên nv
        public APIResponse CallbackV3(MDrumBankLib.CallbackV3 callback, string partnerCode)
        {
            //var ipRequest = Libs.Utils.IPAddress.Get();
            //if (ipRequest != serviceIp)
            //{
            //    NLogLogger.Info(new string[] { "MDrum", "Callback", "Ip Invalid", ipRequest, serviceIp });
            //    return new APIResponse((int)ResponseCode.IpInvalid);
            //}

            var keyBank = callback.NoteFull.Replace(" ", "").Replace(":", "") + "_" + callback.TimeBankSuccess.ToString("ddHHmmss");
            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);
            if (partnerCode == "pp")
                return apiResponse;
            if (callback.CheckTransId != "1")
            {
                if (callback.NoteFull.Contains("SYS") || callback.NoteFull.Contains("GIAY BAO BI"))
                {
                    TelegramNotify.SendTeleV2("-5063693305", "[SN02] có lệnh nạp chứa ký tự đặc biệt " + callback.Amount.ToString("#,#").Replace(",", ".") + " " + callback.NoteFull + "=> cần kiểm tra trước khi callback ");
                    return apiResponse;
                }
                if (partnerCode == "sn2")
                {
                    if (callback.Amount >= 48000000)
                    {
                        TelegramNotify.SendTeleV2("-5063693305", "[SN02] có lệnh nạp lớn " + callback.Amount.ToString("#,#").Replace(",", ".") + " " + callback.NoteFull + "=> cần kiểm tra trước khi callback ");
                        return apiResponse;
                    }
                    if (!string.IsNullOrEmpty(MDrumBankLib.GetBankSuccess3(keyBank)))
                    {
                        TelegramNotify.SendTeleV2("-5063693305", "[SN02] có lệnh nạp trùng nội dung " + callback.Amount.ToString("#,#").Replace(",", ".") + " " + callback.NoteFull + "=> cần kiểm tra trước khi callback ");
                        return apiResponse;
                    }
                }



            }
            if (partnerCode == "sn2")
            {
                MDrumBankLib.SetBankSuccess3(keyBank);

            }
            //if (callback.Amount > 2000000)
            //{
            //    TelegramNotify.SendTeleV2("-1003950185939", "Có lệnh nạp momo từ đối tác " + partnerCode + ", số tiền " + callback.Amount.ToString("#,#").Replace(",", "."));
            //}

            var isCallback = 1;
            if (string.IsNullOrEmpty(callback.Note) || callback.Note == "[COMMENT_INCORRECT_FORMAT]")
            {
                isCallback = 0;
                callback.Note = callback.NoteFull;
            }

            var Partner = new Partners().GetCache(partnerCode);
            var AcountName = callback.Note;

            var neworder = new BankGateAPI()
            {
                PartnerID = Partner.PartnerID,
                PartnerCode = Partner.PartnerCode,
                ProviderCode = "drumbank",
                OrderNo = callback.Note,
                OrderInfo = callback.BankTransId,
                Amount = Convert.ToDecimal(callback.Amount),
                TotalAmount = Convert.ToDecimal(callback.Amount),
                Currency = "VND",
                ReturnUrl = "",
                RequestTime = 0,
                Signature = callback.NoteFull,
                LogContent = "",//nội dung
                BankCode = callback.PartnerBankCode,//bank code system
                FullName = AcountName,
                Mobile = callback.NoteFull,
                RefCode = callback.BankTransId.ToString(),
                BankAccountName = callback.PartnerBankName,// tên tk nhận
                BankAccountNumber = callback.PartnerBankId, //số tk nhận
                LastTime = callback.TimeBankSuccess,
                Fee = 0,
                Status = (int)ResponseCode.TransactionSuccessful
            };
            var ck = getck(neworder.PartnerCode, neworder.BankCode);
            neworder.Fee = Convert.ToInt64(callback.Amount * ck);
            var addId = neworder.AddV2();
            if (addId < 0)
            {
                //neworder.Status = (int)addId;
                //neworder.Update();
                return new APIResponse((int)addId);
            }
            apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = serializer.Serialize(new DataCallbackV2()
                {
                    BankCode = neworder.BankCode,
                    OrderNo = neworder.OrderNo,
                    BankTransId = neworder.OrderInfo,
                    BankAccountName = neworder.BankAccountName,
                    Amount = callback.Amount,
                    BankAccountNumber = callback.PartnerBankId,
                    TimeBankSuccess = callback.TimeBankSuccess,
                    Type = "bank"
                })
            };
            //NLogLogger.Info(new string[] { "MDrum", "PartnerCallback", apiResponse.ResponseContent });

            var Balancedesc = String.Format("Cộng tiền nạp bank số tiền: {3} mgd: {0}-{1}-{2}-{4}", neworder.TransactionID, neworder.BankCode, neworder.OrderInfo, Convert.ToInt64(callback.Amount).ToString("#,#").Replace(",", "."), callback.NoteFull);

            UpdatePartnerBalance(neworder.PartnerCode, Convert.ToInt64(callback.Amount), neworder.Fee, Balancedesc, "BankIn_" + neworder.TransactionID.ToString());

            if (partnerCode == "sn2")
            {
                var mess = $"<b>Tài khoản</b>: ({callback.PartnerBankCode}) {callback.PartnerBankId} -  {callback.PartnerBankName} %0A<b>Mã giao dịch</b>: {neworder.RefCode} %0A<b>Số tiền</b>: %2B{callback.Amount.ToString("#,#").Replace(",", ".")} %0A<b>Thời gian</b>: {callback.TimeBankSuccess.ToString("dd/MM/yyyy HH:mm:ss")} %0A<b>Nội dung</b>: {callback.NoteFull}";
                Task task = TelegramNotify.SendTelegramMessage("-4907377740", mess);
            }

            //Callback for Partner
            if (!string.IsNullOrEmpty(Partner.SMSPlusUrl) && isCallback == 1)
            {

                apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, Partner.PrivateKey, Partner.SignatureType);
                Task.Run(async () => await MDrumBankLib.CallbackJsonV2(Partner.SMSPlusUrl, serializer.Serialize(apiResponse), neworder.TransactionID, neworder.RefCode).ConfigureAwait(false));
            }


            //return apiResponse;
            return apiResponse;
        }

        public APIResponse CallbackV2(MDrumBankLib.Callback callback, string partnerCode)
        {
            //var ipRequest = Libs.Utils.IPAddress.Get();
            //if (ipRequest != serviceIp)
            //{
            //    NLogLogger.Info(new string[] { "MDrum", "Callback", "Ip Invalid", ipRequest, serviceIp });
            //    return new APIResponse((int)ResponseCode.IpInvalid);
            //}
            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            //if (callback.Amount > 2000000)
            //{
            //    TelegramNotify.SendTeleV2("-1003950185939", "Có lệnh nạp momo từ đối tác " + partnerCode + ", số tiền " + callback.Amount.ToString("#,#").Replace(",", "."));
            //}
            //if (callback.CheckTransId != "1")
            //{

            //    if (callback.Amount <= 5000)
            //    {
            //        //elegramNotify.SendTeleV2("-5063693305", "[SN02] có lệnh nạp lớn " + callback.Amount.ToString("#,#").Replace(",", ".") + " " + callback.NoteFull + "=> cần kiểm tra trước khi callback ");
            //        return apiResponse;
            //    }
            //    if (!callback.Note.Equals("[COMMENT_INCORRECT_FORMAT]"))
            //    {
            //        //TelegramNotify.SendTeleV2("-5063693305", "[SN02] có lệnh nạp chứa ký tự đặc biệt " + callback.Amount.ToString("#,#").Replace(",", ".") + " " + callback.NoteFull + "=> cần kiểm tra trước khi callback ");
            //        return apiResponse;
            //    }

            //}


            var Partner = new Partners().GetCache(partnerCode);
            var AcountName = callback.Note;
            if (string.IsNullOrEmpty(callback.PartnerMomoName))
                callback.PartnerMomoName = " ";
            if (string.IsNullOrEmpty(callback.PartnerMomoId))
                callback.PartnerMomoId = " ";
            var neworder = new BankGateAPI()
            {
                PartnerID = Partner.PartnerID,
                PartnerCode = Partner.PartnerCode,
                ProviderCode = "drummomo",
                OrderNo = callback.Note,
                OrderInfo = callback.MomoTransId,
                Amount = Convert.ToDecimal(callback.Amount),
                TotalAmount = Convert.ToDecimal(callback.Amount),
                Currency = "VND",
                ReturnUrl = "",
                RequestTime = 0,
                Signature = Encrypts.MD5(DateTime.Now.ToString()),
                LogContent = "Add Order",
                BankCode = "MOMO",
                FullName = AcountName,
                Mobile = callback.MomoId,
                RefCode = callback.MomoTransId.ToString(),
                BankAccountName = callback.PartnerMomoName,
                BankAccountNumber = callback.PartnerMomoId,
                LastTime = DateTime.Now,
                Fee = 0,
                Status = (int)ResponseCode.TransactionSuccessful
            };
            var ck = getck(neworder.PartnerCode, neworder.BankCode);
            neworder.Fee = Convert.ToInt64(callback.Amount * ck);
            var addId = neworder.AddV2();
            if (addId < 0)
            {
                //neworder.Status = (int)addId;
                //neworder.Update();
                return new APIResponse((int)addId);
            }
            apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = serializer.Serialize(new DataCallback()
                {
                    RefCode = neworder.RefCode,
                    OrderNo = neworder.FullName,
                    OrderInfo = neworder.OrderInfo,
                    MomoTransId = neworder.OrderInfo,
                    Amount = callback.Amount,
                    Mobile = callback.MomoId,
                    Type = "momo"
                })
            };
            //NLogLogger.Info(new string[] { "MDrum", "PartnerCallback", apiResponse.ResponseContent });

            //Action<string, long, string, string, string> send = UpdatePartnerBalance;
            //var asynSend = send.BeginInvoke(neworder.PartnerCode, Convert.ToInt64(callback.Amount), neworder.BankCode, String.Format("Cộng tiền nạp bank số tiền: {3} mgd: {0}-{1}-{2}", addId, neworder.BankCode, neworder.OrderNo, Convert.ToInt64(callback.Amount).ToString("#,#").Replace(",", ".")), "BankIn_" + addId.ToString(), null, null);
            var Balancedesc = String.Format("Cộng tiền nạp momo số tiền: {3} mgd: {0}-{1}-{2}-{4}", neworder.TransactionID, neworder.BankCode, neworder.OrderInfo, Convert.ToInt64(callback.Amount).ToString("#,#").Replace(",", "."), callback.Note);

            UpdatePartnerBalance(neworder.PartnerCode, Convert.ToInt64(callback.Amount), neworder.Fee, Balancedesc, "BankIn_" + neworder.TransactionID.ToString());

            //Callback for Partner
            if (!string.IsNullOrEmpty(Partner.SMSPlusUrl))
            {

                apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, Partner.PrivateKey, Partner.SignatureType);
                Task.Run(async () => await MDrumBankLib.CallbackJsonV2(Partner.SMSPlusUrl, serializer.Serialize(apiResponse), neworder.TransactionID, neworder.RefCode).ConfigureAwait(false));
            }


            //return apiResponse;
            return apiResponse;
        }
        public APIResponse Callback(MDrumBankLib.Callback callback)
        {
            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            //var signature = Utils.Encrypts.MD5(callback.bank_trans_id + callback.message + callback.amount + PartnerCode);
            //if (signature != callback.signature)
            //{
            //    NLogLogger.Info(new string[] { "KZ", "Callback", "Signature Failed", signature, callback.signature });
            //    return new APIResponse((int)ResponseCode.SignatureInvalid);
            //}
            if (string.IsNullOrEmpty(callback.Note))
            {
                return apiResponse;
            }
            NLogLogger.Info(new string[] { "MRUM", "Callback", "OrderGet", callback.Note });
            var order = new BankGateAPI().Get(callback.Note.ToUpper());
            if (order == null)
            {
                NLogLogger.Info(new string[] { "MRUM", "Callback", "Order NULL", serializer.Serialize(callback) });
                var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                apiResponse.ResponseCode = (int)ResponseCode.OrderNotFound;
                apiResponse.Description = "Code này chưa tồn tại";
                if (callback.CheckTransId != "1")
                {
                    if (!string.IsNullOrEmpty(callback.BankTransId) && callback.Note.Length == 10)
                    {
                        new BankTransaction().UpdateCodeById(callback.TransId, "[ORDER_NOTFOUND]");
                    }
                    if (!string.IsNullOrEmpty(callback.MomoTransId))
                    {
                        new MomoTransaction().UpdateCode(callback.MomoTransId, "[ORDER_NOTFOUND]");
                    }
                }
                return apiResponse;
            }
            if (order.Status == 1)
            {
                apiResponse.ResponseCode = (int)ResponseCode.OrderNotFound;
                apiResponse.Description = "Code này đã thành công rồi xin code mới nhé";
                if (callback.CheckTransId != "1" && !order.LogContent.Contains("update by"))
                {
                    if (!string.IsNullOrEmpty(callback.BankTransId) && callback.Note.Length == 10)
                    {
                        new BankTransaction().UpdateCodeById(callback.TransId, "[ORDER_DUPLICATE]");
                    }
                    if (!string.IsNullOrEmpty(callback.MomoTransId))
                    {
                        new MomoTransaction().UpdateCode(callback.MomoTransId, "[ORDER_DUPLICATE]");
                    }
                }
                return apiResponse;
            }
            if (callback.CheckTransId != "1")
            {
                if (order.Status == 0)
                {
                    //if (!string.IsNullOrEmpty(callback.MomoTransId))
                    //{
                    if (Convert.ToInt64(order.Amount) != callback.Amount && order.PartnerCode != "fast")
                    {
                        if (!string.IsNullOrEmpty(callback.MomoTransId) && callback.Note.Length == 10)
                        {
                            new MomoTransaction().UpdateCode(callback.MomoTransId, "[ORDER_WRONGAMOUNT]");
                            apiResponse.ResponseCode = (int)ResponseCode.OrderNotFound;
                            apiResponse.Description = "Đơn sai số tiền";
                            return apiResponse;
                        }
                        if (!string.IsNullOrEmpty(callback.BankTransId) && callback.Note.Length == 10)
                        {
                            new BankTransaction().UpdateCodeById(callback.TransId, "[ORDER_WRONGAMOUNT]");
                            apiResponse.ResponseCode = (int)ResponseCode.OrderNotFound;
                            apiResponse.Description = "Đơn sai số tiền";
                            return apiResponse;
                        }
                    }

                    //if (order.CreatedTime.AddHours(1) < DateTime.Now && order.PartnerCode != "fast")
                    //{
                    //    if (!string.IsNullOrEmpty(callback.MomoTransId) && callback.Note.Length == 10)
                    //    {
                    //        new MomoTransaction().UpdateCode(callback.MomoTransId, "[ORDER_OVERTIME]");
                    //        apiResponse.ResponseCode = (int)ResponseCode.OrderNotFound;
                    //        apiResponse.Description = "Đơn quá thời gian";
                    //        return apiResponse;
                    //    }
                    //    if (!string.IsNullOrEmpty(callback.BankTransId) && callback.Note.Length == 10)
                    //    {
                    //        new BankTransaction().UpdateCode(callback.BankTransId, "[ORDER_OVERTIME]");
                    //        apiResponse.ResponseCode = (int)ResponseCode.OrderNotFound;
                    //        apiResponse.Description = "Đơn quá thời gian";
                    //        return apiResponse;
                    //    }
                    //}
                    //}

                }
            }
            if (callback.CheckTransId != "1")
            {
                if (order.Status == 0)
                {
                    if (!string.IsNullOrEmpty(callback.BankTransId))
                    {
                        if (callback.PartnerBankId != order.BankAccountNumber)
                        {
                            if (!string.IsNullOrEmpty(callback.BankTransId) && callback.Note.Length == 10)
                            {
                                new BankTransaction().UpdateCode(callback.BankTransId, "[ORDER_WRONGACC]");
                                apiResponse.ResponseCode = (int)ResponseCode.OrderNotFound;
                                apiResponse.Description = "Đơn sai tài khoản";
                                return apiResponse;
                            }
                        }


                    }

                }
            }
            if (callback.Amount > 10000000)
            {
                if (order.BankCode.ToUpper() == "MOMO")
                {
                    TelegramNotify.SendTeleV2("-1003950185939", "Có lệnh nạp momo từ đối tác " + order.PartnerCode + ", số tiền " + callback.Amount.ToString("#,#").Replace(",", "."));
                }
                else
                {
                    TelegramNotify.SendTeleV2("-1003950185939", "Có lệnh nạp bank từ đối tác " + order.PartnerCode + ", số tiền " + callback.Amount.ToString("#,#").Replace(",", "."));
                }
            }

            //if(order.PartnerCode=="bp7" && DateTime.Now.Hour<=14)
            //{
            //    var user = new Users().GetByUserName(order.PartnerCode);
            //    if(user.Balance>=1050000000  )
            //    {
            //        string KeyCache = string.Format("{0}:{1}", "PartnerAmounWarningMax", user.UserName);
            //        if (DataCaching.GetCache<string>(KeyCache) == null)
            //        {
            //            TelegramNotify.SendTeleFast("-4820262837", "Số dư hơn 1b rồi, các anh rút giúp em nhé");

            //            DataCaching.SetCache(KeyCache, "1", 3600);
            //        }
            //    }    

            //}    


            if (string.IsNullOrEmpty(callback.MomoTransId))
                callback.MomoTransId = callback.BankTransId;
            var datacb = new DataCallback()
            {
                RefCode = order.RefCode,
                OrderNo = order.FullName,
                Amount = Convert.ToInt32(callback.Amount),
                Mobile = callback.MomoId,
                MomoName = callback.MomoName,
                OrderInfo = callback.MomoTransId,
                Type = "bank"
            };
            if (order.BankCode == "MOMO")
            {
                datacb.Type = "momo";
            }
            apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
            {

                ResponseContent = serializer.Serialize(datacb)
            };


            if (order.Status != (int)ResponseCode.TransactionSuccessful)
            {
                order.Status = (int)ResponseCode.TransactionSuccessful;
                order.TotalAmount = Convert.ToDecimal(callback.Amount);
                order.LastTime = DateTime.Now;
                order.Mobile = callback.MomoId;
                order.OrderInfo = callback.MomoTransId;
                order.LogContent = "core update";
                //fee
                var ck = getck(order.PartnerCode, order.BankCode);
                order.Fee = Convert.ToInt64(callback.Amount * ck / 100);

                var rw = getrw(order.PartnerCode, order.BankCode);

                order.Reward = Convert.ToInt64(callback.Amount * rw / 100);
                //if (string.IsNullOrEmpty(callback.MomoTransId))
                //    order.OrderInfo = callback.BankTransId;

                //check xem có trùng ko
                if (callback.CheckTransId == "1")
                {
                    NLogLogger.Info(new string[] { "MRUM", "Callback", "GetByOrderInfo", callback.Note });
                    var checkBankgate = new BankGateAPI().GetByOrderInfo(callback.MomoTransId);
                    if (checkBankgate != null)
                    {
                        apiResponse.ResponseCode = -1;
                        apiResponse.Description = String.Format("Mã giao dịch {0} đã thành công với nội dung {1} ", callback.MomoTransId, checkBankgate.OrderNo);
                        return apiResponse;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(MDrumBankLib.GetBankSuccess(order.OrderInfo)))
                    {
                        apiResponse.ResponseCode = -1;
                        apiResponse.Description = String.Format("Mã giao dịch {0} đã thành công với nội dung {1} ", callback.MomoTransId, order.OrderNo);
                        return apiResponse;
                    }
                }
                MDrumBankLib.SetBankSuccess(order.OrderInfo);
                int IsCallback = 1;
                //if (order.BankCode.ToUpper() != "MOMO" )
                //{
                //    if (DateTime.Now > order.CreatedTime.AddHours(6)&& callback.CheckTransId != "1")
                //    {
                //        TelegramNotify.SendTeleV2("-5063693305", "[Chuyển thường] có lệnh chuyển thường " + order.PartnerCode + " " + callback.Amount.ToString("#,#").Replace(",", ".") + " " + callback.Note + "=> báo lại khách hàng ");
                //    }
                //}


                if (Convert.ToInt64(order.Amount) != callback.Amount)
                {
                    string chatid = GetChatId(order.PartnerCode);
                    if (order.PartnerCode == "go99" || order.PartnerCode == "nohu888")
                    {
                        //IsCallback = 0;
                        if (Convert.ToInt64(callback.Amount) % 1000 == 0)
                        {
                            TelegramNotify.SendTeleFast(chatid, "[" + order.PartnerCode + "]: lệnh nạp sai số tiền refcode " + order.RefCode + " , mgd " + order.OrderInfo + " , amount user " + Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", ".") + ", amount " + callback.Amount.ToString("#,#").Replace(",", ".") + " => game lên lệnh mới để ghép nhé");
                            apiResponse = new APIResponse((int)ResponseCode.BankAmountInvalid);
                            apiResponse.Description = Convert.ToInt64(order.Amount).ToString() + " vs " + callback.Amount.ToString();
                            return apiResponse;
                        }
                        else
                        {
                            IsCallback = 0;
                            TelegramNotify.SendTeleFast(chatid, "[" + order.PartnerCode + "]: lệnh nạp sai số tiền refcode " + order.RefCode + " , mgd " + order.OrderInfo + " , amount user " + Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", ".") + ", amount " + callback.Amount.ToString("#,#").Replace(",", ".") + " => game cộng tay giúp");
                        }

                    }
                    if (order.PartnerCode == "go05")
                    {
                        TelegramNotify.SendTeleFast(chatid, "[" + order.PartnerCode + "]: lệnh nạp sai số tiền refcode " + order.RefCode + " , mgd " + order.OrderInfo + " , amount user " + Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", ".") + ", amount " + callback.Amount.ToString("#,#").Replace(",", ".") + " => game lên lệnh mới để ghép nhé");
                        apiResponse = new APIResponse((int)ResponseCode.BankAmountInvalid);
                        apiResponse.Description = Convert.ToInt64(order.Amount).ToString() + " vs " + callback.Amount.ToString();
                        return apiResponse;
                    }
                    if (order.PartnerCode == "shdsn555" || order.PartnerCode == "shdsn444" || order.PartnerCode == "shdsn888" || order.PartnerCode == "shdsn666" || order.PartnerCode == "shdsn777" || order.PartnerCode == "shdsn999" || order.PartnerCode == "hn002" || order.PartnerCode == "hn001")
                    {
                        IsCallback = 0;
                        TelegramNotify.SendTeleFast(chatid, "[" + order.PartnerCode + "]: lệnh nạp sai số tiền refcode " + order.RefCode + " , mgd " + order.OrderInfo + " , amount user " + Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", ".") + ", amount " + callback.Amount.ToString("#,#").Replace(",", ".") + " => game cộng tay giúp");
                    }


                }


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
                    var Balancedesc = String.Format("Cộng tiền nạp bank số tiền: {3} mgd: {0}-{1}-{2}", order.TransactionID, order.BankCode, order.OrderNo + "-" + order.RefCode, Convert.ToInt64(callback.Amount).ToString("#,#").Replace(",", "."));
                    if (order.BankCode == "MOMO")
                    {
                        Balancedesc = String.Format("Cộng tiền nạp momo số tiền: {3} mgd: {0}-{1}-{2}", order.TransactionID, order.BankCode, order.OrderNo + "-" + order.RefCode, Convert.ToInt64(callback.Amount).ToString("#,#").Replace(",", "."));
                    }
                    UpdatePartnerBalance(order.PartnerCode, Convert.ToInt64(callback.Amount), order.Fee, Balancedesc, "BankIn_" + order.TransactionID.ToString());


                    //Callback for Partner
                    if (IsCallback == 1)
                    {
                        if (!string.IsNullOrEmpty(order.ReturnUrl))
                        {

                            var checkOrder = new CheckOrder
                            {
                                LasTime = DateTime.Now,
                                RefCode = order.RefCode,
                                Amount = callback.Amount,
                                TransactionID = order.TransactionID.ToString()
                            };
                            DataCaching.SetCache("CheckOrder:" + order.PartnerCode + order.RefCode, checkOrder, 900);


                            var partner = new Partners().GetCache(order.PartnerCode);
                            apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                            if (order.PartnerCode != "sn1")
                            {
                                var datacb2 = new DataCallbackV3();
                                datacb2.RefCode = datacb.RefCode;
                                datacb2.Amount = datacb.Amount;
                                datacb2.Type = datacb.Type;
                                datacb2.OrderInfo = datacb.OrderInfo;
                                datacb2.OrderNo = datacb.OrderNo;
                                datacb2.ResponseCode = apiResponse.ResponseCode;
                                datacb2.Description = apiResponse.Description;
                                datacb2.Signature = PaymentUtils.Signature(datacb2.ResponseCode.ToString() + datacb2.Description + datacb2.RefCode, partner.PrivateKey, partner.SignatureType);
                                Task.Run(async () => await MDrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(datacb2), order.TransactionID, order.RefCode).ConfigureAwait(false));
                            }
                            else
                            {
                                Task.Run(async () => await MDrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID, order.RefCode + " " + order.OrderNo).ConfigureAwait(false));
                            }

                        }
                    }
                }

            }


            return apiResponse;
        }
        private decimal getck(string PartnerCode, string Type)
        {
            decimal ck = 0;
            //var partner = new Partners().GetCache(PartnerCode);
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
            if (listpartnerDiscount == null)
            {
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }

            if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
            {
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
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


