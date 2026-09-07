using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel.PeerResolvers;
using System.Text;
using System.Timers;
using System.Web.Script.Serialization;
using APIGameServiceProxy;
using Libs.Utils;
using Libs.Report;
using Libs.API;
using Libs.TopupPartner;

namespace WService
{

    public class AutoBuyCardService
    {
        private Timer timer;
        private double INTERVAL = ConfigurationManager.AppSettings["PROCESS_INTERVAL"] == null ? 5 : Convert.ToDouble(ConfigurationManager.AppSettings["PROCESS_INTERVAL"]); // Min
        private bool Is_Run = false;
        private static int CARDPERREQUEST = ConfigurationManager.AppSettings["CARD_PER_REQUEST"] == null ? 1 : Convert.ToInt32(ConfigurationManager.AppSettings["CARD_PER_REQUEST"]);
        private static string PartnerCode = "pl_auto";
        private static string PartnerKey = "a89d75d1b26fdcf0b403e134e289cdaa";
        private static string ServiceCode = "topuptelco";
        private static string CommandCode = "tranfer";

        private static string ProviderCode_BuyCard = "b2bout";
        private static string ProviderCode_Topup = "b2btranfer";

        private static string URLGAMESERVICE = ConfigurationManager.AppSettings["URL_GAME_SERVICE"] == null ? "http://127.0.0.1:1588/" : ConfigurationManager.AppSettings["URL_GAME_SERVICE"];
        private static string URLVPGSERVICE = ConfigurationManager.AppSettings["URL_VPG_SERVICE"] == null ? "http://127.0.0.1:1598/" : ConfigurationManager.AppSettings["URL_VPG_SERVICE"];
        //private static string URLVPGSERVICE = ConfigurationManager.AppSettings["URL_VPG_SERVICE"] == null ? "https://apicard.atheriz.xyz/" : ConfigurationManager.AppSettings["URL_VPG_SERVICE"];


        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void StartProcessAutoBuyCard()
        {
            timer = new Timer()
            {
                Interval = INTERVAL * (1000 * 60),
            };
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {


                var sw = new Stopwatch();

                NLogLogger.Info(new string[] { "Process AutoBuyCard Timer Elapsed!", "Is Run: " + Is_Run });
                var listConfig = new SystemConfig().GetList();
                var config = listConfig.Find(c => c.Code == "GateAutoService");
                var feature = serializer.Deserialize<List<WServiceAutoBuyCardService>>(config.Feature);
                NLogLogger.Info(new string[] { "Process AutoBuyCard Get Config:", serializer.Serialize(config) });
                if (!Is_Run)
                {
                    NLogLogger.Info(new string[] { "[Process AutoBuyCard] START" });
                    sw.Start();
                    Is_Run = true;
                    if (config.Status == 1)
                    {
                        if (feature.Find(f => f.Code == "AutoBuyCard").Status == 1)
                        {
                            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoBuyCard BEGIN" });
                            ProcessAutoBuyCard();
                            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoBuyCard END" });
                        }

                        if (feature.Find(f => f.Code == "AutoTopup").Status == 1)
                        {
                            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTopup BEGIN" });
                            ProcessAutoTopup();
                            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTopup END" });
                        }

                        if (feature.Find(f => f.Code == "AutoCombine").Status == 1)
                        {
                            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTranfer BEGIN" });
                            ProcessAutoTranfer();
                            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTranfer END" });
                        }

                        if (feature.Find(f => f.Code == "AutoShare").Status == 1)
                        {
                            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoShare BEGIN" });
                            ProcessAutoShare();
                            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoShare END" });
                        }

                    }
                    Is_Run = false;
                    sw.Stop();
                    NLogLogger.Info(new string[] { string.Format("[Process AutoBuyCard] SUCCESS in {0} ms", sw.ElapsedMilliseconds) });
                }

            }
            catch (Exception ex)
            {
                //Is_Run = false;
                NLogLogger.Info(new string[] { "Error", ex.Message.Replace("\n", " ") });
            }

        }

        public static string GenOrderCode()
        {
            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,1,2,3,4,5,6,7,8,9").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i <= 7; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }
            var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return tmp.ToUpper() + timeSpan.ToString();
        }

        public class CardDVO
        {
            public string Serial { get; set; }
            public string Pin { get; set; }
            public DateTime ExpireDate { get; set; }
        }

        public static void ProcessAutoBuyCard()
        {
            int pageIndex = 1;
            int pageSize = 20;
            int totalRows;

            var packetObj = new Packet();

            var listpacket = packetObj.GetListAutoBuy(pageIndex, pageSize, out totalRows);

            foreach (var packet in listpacket)
            {
                var cardType = string.Empty;
                switch (packet.CardType.ToLower())
                {

                    case "viettel":
                        cardType = "vtt";
                        break;
                    case "vms":
                        cardType = "vms";
                        break;
                    case "vnp":
                        cardType = "vnp";
                        break;
                }

                //switch ((int)packet.CardValue)
                //{
                //    //case 50000:
                //    //    CARDPERREQUEST = CARDPERREQUEST;
                //    //    break;
                //    case 100000:
                //        CARDPERREQUEST = 1;
                //        break;
                //    case 200000:
                //        CARDPERREQUEST = 1;
                //        break;
                //}

                //var orderNo = GenOrderCode();
                var orderNo = ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                //Add Log
                var transactionAutoLog = new TransactionAutoLog()
                {
                    OrderNo = orderNo,
                    Amount = (int)packet.CardValue,
                    CardType = packet.CardType,
                    PartnerCode = "pl_auto",
                    Quantity = CARDPERREQUEST,
                    Sign = string.Empty,
                    RequestTime = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"))
                };

                var id = transactionAutoLog.Add();
                if (id > 0)
                {
                    var handler = BuyCardFactory.GetHandler(ProviderCode_BuyCard);
                    string responseData = string.Empty;
                    var result = handler.downloadSoftpin(orderNo, cardType, (int)packet.CardValue, CARDPERREQUEST, PartnerCode, ProviderCode_BuyCard, ref responseData);
                    switch (result.ResponseCode)
                    {
                        case (int)ResponseCode.TransactionSuccessful:
                        {
                            var listCards = serializer.Deserialize<List<CardDVO>>(result.ResponseContent);
                            var countup = 0;
                            foreach (var card in listCards)
                            {
                                //Insert to DB

                                CardStore obj = new CardStore
                                {
                                    PacketId = packet.Id,
                                    ProviderCode = packet.ProviderCode,
                                    CardType = packet.CardType,
                                    CardValue = (int)packet.CardValue,
                                    IsActive = (bool)packet.IsActive,
                                    IsSold = false,
                                    CardSerial = card.Serial,
                                    CardCode = card.Pin,
                                    ExpireDate = card.ExpireDate
                                };
                                obj.Add();
                                countup++;
                            }
                            //Update Count Up
                            var updatePaket = new Packet() { Id = packet.Id, NumberCardUp = packet.NumberCardUp + countup };
                            updatePaket.Update();

                            //Update Log
                            transactionAutoLog.Id = id;
                            transactionAutoLog.Status = (int)ResponseCode.TransactionSuccessful;
                            transactionAutoLog.ReturnValue = result.ResponseContent;
                            transactionAutoLog.Update();
                            break;
                        }
                        case (int)ResponseCode.CardOutOfStock:
                        {
                            //Update Status
                            var updatePaket = new Packet() { Id = packet.Id, Status = 2 };
                            updatePaket.Update();

                            transactionAutoLog.Id = id;
                            transactionAutoLog.Status = (int)ResponseCode.CardOutOfStock;
                            transactionAutoLog.ReturnValue = result.ResponseContent;
                            transactionAutoLog.Update();
                            break;
                        }

                        case (int)ResponseCode.TransactionNotExists:
                        {
                            //Delete
                            transactionAutoLog.Id = id;
                            transactionAutoLog.Delete();
                            break;
                        }

                        case (int)ResponseCode.TransactionTimeout:
                        {
                            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTranfer", "Request", "TransactionTimeout", orderNo });
                            var topupMobile3rdLog = new TopupMobile3rdLog().GetByTransactionIdSuccess(Convert.ToInt64(orderNo));
                            if (topupMobile3rdLog != null)
                            {
                                try
                                {
                                    var logConten = topupMobile3rdLog.LogContent;
                                    logConten = logConten.Substring(logConten.IndexOf('{'), logConten.Length - logConten.IndexOf('{'));
                                    var apiResponse = serializer.Deserialize<APIResponse>(logConten);
                                    var listCards = serializer.Deserialize<List<CardDVO>>(apiResponse.ResponseContent);
                                    var countup = 0;
                                    foreach (var card in listCards)
                                    {
                                        //Insert to DB

                                        CardStore obj = new CardStore
                                        {
                                            PacketId = packet.Id,
                                            ProviderCode = packet.ProviderCode,
                                            CardType = packet.CardType,
                                            CardValue = (int)packet.CardValue,
                                            IsActive = (bool)packet.IsActive,
                                            IsSold = false,
                                            CardSerial = card.Serial,
                                            CardCode = card.Pin,
                                            ExpireDate = card.ExpireDate
                                        };
                                        obj.Add();
                                        countup++;
                                    }
                                    //Update Count Up
                                    var updatePaket = new Packet() { Id = packet.Id, NumberCardUp = packet.NumberCardUp + countup };
                                    updatePaket.Update();

                                    //Update Log
                                    transactionAutoLog.Id = id;
                                    transactionAutoLog.Status = (int)ResponseCode.TransactionSuccessful;
                                    transactionAutoLog.ReturnValue = result.ResponseContent;
                                    transactionAutoLog.Update();

                                    NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTranfer", "Request", "TransactionTimeout", "Success", serializer.Serialize(listCards) });
                                }
                                catch (Exception e)
                                {
                                    NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTranfer", "Request", "TransactionTimeout", "Exception", e.Message });
                                }

                            }
                            else
                            {
                                NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTranfer", "Request", "TransactionTimeout", "NULLLLLLLLLLLL" });
                            }

                            break;
                        }
                        default:
                            //Update Log
                            transactionAutoLog.Id = id;
                            transactionAutoLog.Status = result.ResponseCode;
                            transactionAutoLog.ReturnValue = string.Empty;
                            transactionAutoLog.Update();
                            break;
                    }
                }

            }

        }

        public static void ProcessAutoTranfer()
        {
            var service = new APIGameService(URLGAMESERVICE + "APIGame.asmx");
            string tranId = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTranfer", "Request", "gate", tranId, "127.0.0.1" });
            service.Timeout = 600000;
            try
            {
                var result = service.RequestTranferAccountBalance(tranId.ToString(), "gate", 1, "127.0.0.1");
                NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTranfer", "Response", "gate", tranId, "127.0.0.1", serializer.Serialize(result) });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTranfer", "Exception", "gate", tranId, "127.0.0.1", e.Message });
            }
        }

        public static void ProcessAutoShare()
        {
            var service = new APIGameService(URLGAMESERVICE + "APIGame.asmx");
            string tranId = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoShare", "Request", "gate", tranId, "127.0.0.1" });
            service.Timeout = 600000;
            try
            {
                var result = service.RequestTranferAccountBalance(tranId.ToString(), "gate", 2, "127.0.0.1");
                NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoShare", "Response", "gate", tranId, "127.0.0.1", serializer.Serialize(result) });
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoShare", "Exception", "gate", tranId, "127.0.0.1", e.Message });
            }
        }

        public static void ProcessAutoTopup()
        {
            //Lấy đơn để Topup
            IDictionary<int, string> dictTelco = new Dictionary<int, string>();
            dictTelco.Add(1, "vtt");
            dictTelco.Add(2, "vms");
            dictTelco.Add(3, "vnp");
            foreach (var d in dictTelco)
            {
                TopupMobileLog topupProcess;
                topupProcess = new TopupMobileLog().GetProcess(d.Value, 10000, PartnerCode, ProviderCode_Topup);
                if (topupProcess != null)
                {
                    var topuppAmount = 0;
                    var amount = topupProcess.Amount - topupProcess.AmountTopupSuccess;
                    if (amount >= 500000)
                    {
                        topuppAmount = 500000;
                    }
                    else if (amount >= 200000)
                    {
                        topuppAmount = 200000;
                    }
                    else if (amount >= 100000)
                    {
                        topuppAmount = 100000;
                    }
                    else if (amount >= 50000)
                    {
                        topuppAmount = 50000;
                    }
                    else if (amount >= 20000)
                    {
                        topuppAmount = 20000;
                    }
                    else if (amount >= 10000)
                    {
                        topuppAmount = 10000;
                    }
                    string tranId = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                    var topup = new TopupMobile3rdLog();
                    topup.RequestNo = topupProcess.TransactionID;
                    topup.TransactionId = Convert.ToInt64(tranId);
                    topup.PartnerCode = PartnerCode;
                    topup.ProviderCode = ProviderCode_Topup;
                    topup.Telco = d.Value;
                    topup.ClientId = "127.0.0.1";
                    topup.Sim = topupProcess.Mobile;
                    topup.SimTarget = string.Empty;
                    topup.CardSerial = string.Empty;
                    topup.CardCode = string.Empty;
                    topup.Amount = topuppAmount;
                    topup.BidRate = topupProcess.BidRate;
                    topup.CreateTime = DateTime.Now;
                    topup.Add();
                    topup.Id = topup.ReturnValue;

                    var service = new VPGServiceProxy.VPGService(URLVPGSERVICE + "VPGService.asmx");

                    NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTopup", "Request", "gate", tranId, "127.0.0.1" });
                    service.Timeout = 600000;
                    var result = string.Empty;
                    try
                    {
                        string requestContent = serializer.Serialize(new TranferRequest()
                        {
                            Provider = d.Value,
                            Amount = topuppAmount,
                            AccountName = "AutoPL",
                            OrderNo = tranId,
                            SimTarget = topupProcess.Mobile
                        });
                        var signature = Encrypts.MD5(PartnerCode + ServiceCode + CommandCode + requestContent + PartnerKey);
                        result = service.Request(PartnerCode, ServiceCode, CommandCode, requestContent, signature);
                        NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTopup", "Response", "gate", tranId, "127.0.0.1", result });
                    }
                    //catch (WebException e)
                    //{
                    //    if (e.Status == WebExceptionStatus.Timeout)
                    //    {
                    //        var buyCardlog = new BuyCard().Get(tranId);
                    //        if (buyCardlog != null)
                    //        {
                    //            var buyCardlogOrg = new BuyCard().Get(buyCardlog.TransactionID);
                    //            if (buyCardlogOrg != null)
                    //            {
                    //                if (buyCardlogOrg.Status == (int)ResponseCode.TransactionSuccessful)
                    //                {
                    //                    buyCardlog.Status = (int)ResponseCode.TransactionSuccessful;
                    //                    buyCardlog.Update();
                    //                    result = serializer.Serialize(new APIResponse((int)ResponseCode.TransactionSuccessful));
                    //                }
                    //            }
                    //        }


                    //    }
                    //    NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTopup", "WebException", "gate", tranId, "127.0.0.1", e.Message });
                    //}

                    catch (Exception e)
                    {

                        NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTopup", "Exception", "gate", tranId, "127.0.0.1", e.Message });
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        var resultObj = serializer.Deserialize<APIRespone>(result);
                        topup.LogContent = serializer.Serialize(result);
                        switch (resultObj.ResponseCode)
                        {
                            case (int)ResponseCode.TransactionSuccessful:
                                topup.Status = (int)ResponseCode.TransactionSuccessful;
                                topup.Amount = topuppAmount;
                                topup.Update();
                                topupProcess.Topup(1, topuppAmount, topup.BidRate); // Thanh công update Amount
                                NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTopup", "Topup", topuppAmount.ToString(), topupProcess.Mobile, "Success" });
                                break;

                            case (int)ResponseCode.SystemBusy:
                                topup.Status = (int)ResponseCode.SystemBusy;
                                topup.Amount = 0;
                                topup.Update();
                                topupProcess.Topup(-3, 0); //Đưa về đợi nạp vì ko bắt được kết quả đầu vào
                                NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTopup", "Topup", topuppAmount.ToString(), topupProcess.Mobile, "System Busy" });
                                break;

                            default:
                                topup.Status = resultObj.ResponseCode;
                                topup.Amount = 0;
                                topup.Update();
                                topupProcess.Topup(0, 0);
                                NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTopup", "Topup", topuppAmount.ToString(), topupProcess.Mobile, "Fail" });
                                break;
                        }

                    }

                }
                else
                {
                    NLogLogger.Info(new string[] { "[Process AutoBuyCard]", "ProcessAutoTopup", d.Value, "Order not found" });
                }
            }
        }
    }

    public class APIRespone
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }

    public class TranferRequest
    {
        public string Provider { get; set; } // CardType
        public int Amount { get; set; }
        public string SimTarget { get; set; }
        public string AccountName { get; set; }
        public long AccountId { get; set; }
        public string OrderNo { get; set; }
    }
}

