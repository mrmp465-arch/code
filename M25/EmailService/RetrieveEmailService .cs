using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.BankGate;
using System.Globalization;
using System.IO;
using EAGetMail;

namespace EmailService
{

    public class RetrieveEmailService
    {
        private Timer timer;
        private double INTERVAL = ConfigurationManager.AppSettings["PROCESS_INTERVAL"] == null ? 5 : Convert.ToDouble(ConfigurationManager.AppSettings["PROCESS_INTERVAL"]);
        private bool Is_Run = false;


        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        static string _generateFileName(int sequence)
        {
            DateTime currentDateTime = DateTime.Now;
            return string.Format("{0}-{1:000}-{2:000}.eml",
                currentDateTime.ToString("yyyyMMddHHmmss", new CultureInfo("en-US")),
                currentDateTime.Millisecond,
                sequence);
        }



        public void StartProcessRetrieveEmail()
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
                    ProcessRetrieveEmail();
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

        public static void ProcessRetrieveEmail()
        {
            RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");
            var listEmail = new Libs.Report.BankAccount().Get(1);


            foreach (var email in listEmail)
            {

                //string localInbox = string.Format("{0}\\inbox", Directory.GetCurrentDirectory());
                //// If the folder is not existed, create it.
                //if (!Directory.Exists(localInbox))
                //{
                //    Directory.CreateDirectory(localInbox);
                //}

                //MailServer oServer = new MailServer("pop.gmail.com", email.Email, rijndaelKey.Decrypt(email.EmailPass), ServerProtocol.Pop3);

                //// Enable SSL/TLS connection, most modern email server require SSL/TLS by default
                //oServer.SSLConnection = true;
                //oServer.Port = 995;

                //// if your server doesn't support SSL/TLS, please use the following codes
                //// oServer.SSLConnection = false;
                //// oServer.Port = 110;

                //MailClient oClient = new MailClient("TryIt");
                //oClient.Connect(oServer);

                ////int totalcount = oClient.GetMailCount();

                ////string range = "";
                ////if (totalcount > 10)
                ////{
                ////    range = String.Format("{0}:{1}", totalcount - 10, totalcount);
                ////}
                ////else
                ////{
                ////    range = String.Format("*:{0}", totalcount);
                ////}

                ////oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.SeqRange;
                ////oClient.GetMailInfosParam.SeqRange = range;


                //MailInfo[] infos = oClient.GetMailInfos();
                //Console.WriteLine("Total {0} email(s)\r\n", infos.Length);
                //for (int i = 0; i < infos.Length; i++)
                //{
                //    MailInfo info = infos[i];
                //    Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}", info.Index, info.Size, info.UIDL);

                //    // Receive email from POP3 server
                //    Mail oMail = oClient.GetMail(info);

                //    if (oMail.From.ToString() == "no-reply@momo.vn")
                //    {
                //        if (oMail.Subject.Contains("Bạn nhận được tiền từ"))
                //        {
                //            Console.WriteLine("From: {0}", oMail.From.ToString());
                //            Console.WriteLine("Subject: {0}\r\n", oMail.Subject);

                //            // Generate an unqiue email file name based on date time.
                //            string fileName = _generateFileName(i + 1);
                //            string fullPath = string.Format("{0}\\{1}", localInbox, fileName);

                //            // Save email to local disk
                //            oMail.SaveAs(fullPath, true);

                //            // Mark email as deleted from POP3 server.
                //            oClient.Delete(info);
                //        }
                //    }


                //}

                //// Quit and expunge emails marked as deleted from POP3 server.
                //oClient.Quit();
                //Console.WriteLine("Completed!");

                bool isUidlLoaded = false;
                bool isLeaveCopy = true; // leave a copy of message on server.

                // UIDL is the identifier of every email on POP3/IMAP4/Exchange server, to avoid retrieve
                // the same email from server more than once, we record the email UIDL retrieved every time
                // if you delete the email from server every time and not to leave a copy of email on
                // the server, then please remove all the function about uidl.
                // UIDLManager wraps the function to write/read uidl record from a text file.
                UIDLManager oUIDLManager = new UIDLManager();

                try
                {
                    // Create a folder named "inbox" under current directory
                    // to save the email retrieved.
                    string localInbox = string.Format("{0}\\inbox", Directory.GetCurrentDirectory());
                    string uidlFile = string.Format("{0}\\uidl.txt", localInbox);

                    // If the folder is not existed, create it.
                    if (!Directory.Exists(localInbox))
                    {
                        Directory.CreateDirectory(localInbox);
                    }

                    // Load existed uidl records to UIDLManager
                    oUIDLManager.Load(uidlFile);
                    isUidlLoaded = true;

                    MailServer oServer = new MailServer("pop.gmail.com", email.Email, rijndaelKey.Decrypt(email.EmailPass), ServerProtocol.Pop3);

                    // Enable SSL/TLS connection, most modern email server require SSL/TLS by default
                    oServer.SSLConnection = true;
                    oServer.Port = 995;

                    // if your server doesn't support SSL/TLS, please use the following codes
                    // oServer.SSLConnection = false;
                    // oServer.Port = 110;

                    MailClient oClient = new MailClient("TryIt");
                    oClient.Connect(oServer);

                    MailInfo[] infos = oClient.GetMailInfos();
                    Console.WriteLine("Total {0} email(s)\r\n", infos.Length);

                    // Remove the local uidl that is not existed on the server,
                    oUIDLManager.SyncUIDL(oServer, infos);
                    oUIDLManager.Update();

                    for (int i = 0; i < infos.Length; i++)
                    {
                        MailInfo info = infos[i];
                        if (oUIDLManager.FindUIDL(oServer, info.UIDL) != null)
                        {
                            // This email has been downloaded before
                            continue;
                        }

                        Console.WriteLine("Retrieving {0}/{1}...", i + 1, infos.Length);

                        Mail oMail = oClient.GetMail(info);
                        
                        Console.WriteLine("From: {0}", oMail.From.ToString());
                        Console.WriteLine("Subject: {0}\r\n", oMail.Subject);

                        // Generate an unqiue email file name based on date time.
                        string fileName = _generateFileName(i + 1);
                        string fullPath = string.Format("{0}\\{1}", localInbox, fileName);

                        // Save email to local disk
                        oMail.SaveAs(fullPath, true);

                        if (isLeaveCopy)
                        {
                            // Add uidl to uidl file to avoid we retrieve it next time.
                            oUIDLManager.AddUIDL(oServer, info.UIDL, fileName);
                        }
                        else
                        {
                            Console.WriteLine("Deleting ...");
                            oClient.Delete(info);

                            // Remove UIDL from local uidl file.
                            oUIDLManager.RemoveUIDL(oServer, info.UIDL);
                        }
                    }

                    // Quit and expunge emails marked as deleted from POP3 server.
                    oClient.Quit();
                    Console.WriteLine("Completed!");
                }
                catch (Exception ep)
                {
                    Console.WriteLine(ep.Message);
                }

                // Update the uidl list to local uidl file and then we can load it next time.
                if (isUidlLoaded)
                {
                    oUIDLManager.Update();
                }
            }
        }

    }

}

