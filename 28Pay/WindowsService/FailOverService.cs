using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Libs.Utils;
using Libs.Report;
using Libs.API;

namespace WService
{

    public class FailOverService
    {
        private Timer timer;
        private double INTERVAL = ConfigurationManager.AppSettings["PROCESS_INTERVAL"] == null ? 5 : Convert.ToDouble(ConfigurationManager.AppSettings["PROCESS_INTERVAL"]);
        private bool Is_Run = false;
        private static int TOPRAWCHECK = ConfigurationManager.AppSettings["TOP_RAW_CHECK"] == null ? 20 : Convert.ToInt32(ConfigurationManager.AppSettings["TOP_RAW_CHECK"]);
        private static string CARDTYPECHECK = ConfigurationManager.AppSettings["CARD_TYPE_CHECK"] == null ? string.Empty : ConfigurationManager.AppSettings["CARD_TYPE_CHECK"];
        private static string LISTEMAILTO = ConfigurationManager.AppSettings["LIST_EMAIL_TO"] == null ? "klein.xbom@gmail.com" : ConfigurationManager.AppSettings["LIST_EMAIL_TO"];
        private static string NAMECHECK = ConfigurationManager.AppSettings["NAME_CHECK"] == null ? "PaymentT" : ConfigurationManager.AppSettings["NAME_CHECK"];
        public void StartProcessFailOver()
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

                NLogLogger.Info(new string[] { "Process FailOver Timer Elapsed!" });

                if (!Is_Run)
                {
                    sw.Start();
                    Is_Run = true;
                    ProcessFailOver();
                    Is_Run = false;
                    sw.Stop();
                    NLogLogger.Info(new string[] { string.Format("[Process FailOver] success in {0} ms", sw.ElapsedMilliseconds) });
                }
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Error", ex.Message.Replace("\n", " ") });
            }

        }

        public static void ProcessFailOver()
        {
            var providerCode = string.Empty;
            var cardAPILog = new CardAPILog();
            var listCardType = CARDTYPECHECK.Split('|');

            foreach (var cardType in listCardType)
            {
                var countFail = 0;
                var listLog = cardAPILog.GetTableList(TOPRAWCHECK, string.Empty, DateTime.Now, null, cardType, string.Empty);
                foreach (var log in listLog)
                {
                    if (log.Status != 1)
                    {
                        countFail++;
                        providerCode = log.Provider;
                    }

                }
                //Đếm số lần Fail
                if (countFail == TOPRAWCHECK)
                {
                    var providers = new Providers();
                    var provider = providers.Get(providerCode);

                    if (provider != null)
                    {

                        var productCode = provider.ProductCode;
                        if (productCode.Contains(cardType) && provider.Status == 1)
                        {
                            List<String> Items = productCode.Split('|').Select(i => i.Trim()).Where(i => i != string.Empty).ToList();
                            //Split them all and remove spaces
                            Items.Remove(cardType); //or whichever you want
                            string newProductCode = String.Join("|", Items.ToArray());
                            provider.ProductCode = newProductCode;
                            try
                            {
                                provider.Update();
                            }
                            catch (Exception ex)
                            {
                                NLogLogger.Info(new string[] { "Error not Update Provider", ex.Message.Replace("\n", " ") });
                            }
                            var subject = string.Format("[{0}] Thẻ {1} của nhà cung cấp {2} gặp sự cố !", NAMECHECK.ToUpper(), cardType.ToUpper(), providerCode.ToUpper());
                            var body = string.Format("Thẻ {0} của nhà cung cấp {1} trên cổng {2} đang gặp sự cố, hệ thống đã tự động ngừng cung cấp loại thẻ này và chuyển qua nhà cung cấp dự phòng nếu có.", cardType.ToUpper(), providerCode.ToUpper(), NAMECHECK.ToUpper());
                            EmailService.SendMail(subject, body, LISTEMAILTO);
                        }
                    }
                    else
                    {
                        NLogLogger.Info(new string[] { "Error", "Provider not Found", providerCode });
                    }




                }
            }



        }
    }
}
