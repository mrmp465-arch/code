using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.Report;
using Libs.API;
using Libs.TopupPartner;

namespace WService
{

    public class AutoBuyCardService
    {
        private Timer timer;
        private double INTERVAL = ConfigurationManager.AppSettings["PROCESS_INTERVAL"] == null ? 5 : Convert.ToDouble(ConfigurationManager.AppSettings["PROCESS_INTERVAL"]);
        private bool Is_Run = false;
        private static int CARDPERREQUEST = ConfigurationManager.AppSettings["CARD_PER_REQUEST"] == null ? 20 : Convert.ToInt32(ConfigurationManager.AppSettings["CARD_PER_REQUEST"]);


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

                NLogLogger.Info(new string[] { "Process AutoBuyCard Timer Elapsed!" });

                if (!Is_Run)
                {
                    sw.Start();
                    Is_Run = true;
                    ProcessAutoBuyCard();
                    Is_Run = false;
                    sw.Stop();
                    NLogLogger.Info(new string[] { string.Format("[Process AutoBuyCard] success in {0} ms", sw.ElapsedMilliseconds) });
                }
            }
            catch (Exception ex)
            {
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

                var orderNo = GenOrderCode();

                //Add Log
                var transactionAutoLog = new TransactionAutoLog()
                {
                    OrderNo = orderNo,
                    Amount = packet.CardValue,
                    CardType = packet.CardType,
                    PartnerCode = "pl_auto",
                    Quantity = CARDPERREQUEST,
                    Sign = string.Empty,
                    RequestTime = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"))
                };

                var id = transactionAutoLog.Add();
                if (id > 0)
                {
                    var handler = BuyCardFactory.GetHandler(packet.ProviderCode);
                    string responseData = string.Empty;
                    var result = handler.downloadSoftpin(orderNo, packet.CardType, packet.CardValue, CARDPERREQUEST, ref responseData);

                    if (result.ResponseCode == (int)ResponseCode.TransactionSuccessful)
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
                                CardValue = packet.CardValue,
                                IsActive = false,
                                IsSold = false,
                                CardSerial = card.Serial,
                                CardCode = card.Pin,
                                ExpireDate = card.ExpireDate
                            };
                            obj.Add();
                            countup++;
                        }
                        //Update Count Up
                        packet.NumberCardUp = packet.NumberCardUp + countup;
                        packet.Update();

                        //Update Log
                        transactionAutoLog.Id = id;
                        transactionAutoLog.Status = (int) ResponseCode.TransactionSuccessful;
                        transactionAutoLog.ReturnValue = result.ResponseContent;
                        transactionAutoLog.Update();
                    }

                    else
                    {
                        //Update Log
                        transactionAutoLog.Id = id;
                        transactionAutoLog.Status = result.ResponseCode;
                        transactionAutoLog.ReturnValue = string.Empty;
                        transactionAutoLog.Update();
                    }
                }

            }

        }
    }
}
