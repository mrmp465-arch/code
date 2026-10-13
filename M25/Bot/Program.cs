using Libs.API;
using Libs.Report;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Bot
{
    class Program
    {

        static void Main(string[] args)
        {
            //ReporBank();

            //ReportSN();
            ReporOtp();
        }
        static void ReporBank()
        {
            var _Bank = new BankAccounts();
            var data = _Bank.GetList().Where(x => x.StatusExtra != -4 && x.BalanceTotal >= 100000).ToList();
            SendTeleV3("-4935229779", "Tổng số dư bank: " + data.Sum(x => x.BalanceTotal).ToString("N0").Replace(".", ","));
            System.Threading.Thread.Sleep(200);
            foreach (var bank in data)
            {
                SendTeleV3("-4935229779", "Tài khoản " + bank.BankCode + " -" + bank.BankId + " -" + bank.BankName + " Số dư : " + bank.BalanceTotal.ToString("N0").Replace(".", ","));
            }

            var _Momo = new MomoAccounts();
            var data2 = _Momo.GetList().Where(a => a.StatusExtra != -4).Where(x => x.Status == 1 || x.Status == 0).ToList();
            SendTeleV3("-4935229779", "Tổng số dư momo: " + data2.Sum(x => x.BalanceTotal).ToString("N0").Replace(".", ","));
            System.Threading.Thread.Sleep(200);


            //foreach (var bank in data2)
            //{
            //    SendTeleV3("-4935229779", "Tài khoản " + bank.MomoId + " -" + bank.MomoId + " Số dư : " + bank.BalanceTotal.ToString("N0").Replace(".", ","));
            //}
        }
        static void ReporOtp()
        {
            //NLogLogger.Info("1");
            var _Bank = new BankAccounts();
            var lstbank = _Bank.GetList();
            if (lstbank.Exists(x => x.Status == 1 && x.StatusExtra != -4 && x.Type == "IN" && x.BalanceTotal >= 100000000))
            {
                foreach (var bank in lstbank.Where(x => x.Status == 1 && x.StatusExtra != -4 && x.Type == "IN" && x.BalanceTotal >= 100000000))
                {
                   // NLogLogger.Info(bank.BankName);
                    var chatid = GetChatId(bank.BankName);
                    SendTeleV3(chatid, "[Rút tiền] Tài khoản " + bank.BankCode + " " + bank.BankName + " " + bank.BankId + " số dư vượt quá 100M => Ae rút hộ ");

                }
            }



            var data = lstbank.Where(x => x.Type.Contains("OUT") && x.Status == 1 && x.StatusExtra != -4 && x.BankCode == "VPB").ToList();

            if (data != null)
            {
                foreach (var bank in data)
                {
                    if (!string.IsNullOrEmpty(bank.AppDeviceId))
                    {
                        var key = string.Format("OTP:{0}", bank.AppDeviceId);
                        var result = OTPDataCaching.GetCache<OTPRequest>(key);
                        if (result == null)
                        {

                            SendTeleV3("-4990524268", "[OTP] Tài khoản VPB:  " + bank.AppDeviceId + " không có OTP => Ae tắt app đi bật lại ");
                        }
                    }

                }
            }
           
        }
        public static string GetChatId(string id)
        {
            string partnecode = "";
            switch (id)
            {
                case "NGUYEN TUAN ANH":
                    partnecode = "-5143507044";
                    break;

                case "MAI VAN TOAN":
                    partnecode = "-5257886157";
                    break;
                case "PHAM HUY HOANG":
                    partnecode = "-5174985140";
                    break;
                case "NGUYEN VAN BAY":
                    partnecode = "-5054103780";
                    break;
                case "NGUYEN DINH TRUONG":
                    partnecode = "-5054103780";
                    break;
                case "TA QUANG LOC":
                    partnecode = "-1003808088014";
                    break;
                case "DO HAI DANG":
                    partnecode = "-5118692942";
                    break;
                case "TRAN DINH THANG":
                    partnecode = "-5141722332";
                    break;
                case "DANG HOANG TRANG":
                    partnecode = "-5285385076";
                    break;
                case "DUONG VAN TIEP":
                    partnecode = "-5285385076";
                    break;
                case "PHAM HUU NGO":
                    partnecode = "-5141722332";
                    break;
                    //case "bp7":
                    //    partnecode = "-4820262837";
                    //    break;

            };
            return partnecode;
        }
        public class OTPRequest
        {
            public string pin { get; set; }
            public string state { get; set; }
            public string otp { get; set; }
            public int time { get; set; }
            public string extra { get; set; }
            public string timeCreate { get; set; }

        }
        static void UpdateMomo()
        {
            if (DateTime.Now.Hour == 1 || DateTime.Now.Hour == 7 || DateTime.Now.Hour == 13 || DateTime.Now.Hour == 20)
            {

            }
        }
        static void ReportSN()
        {
            try
            {


                var currenttime = DateTime.Now.AddHours(-2);
                DateTime fromdate = new DateTime(currenttime.Year, currenttime.Month, currenttime.Day);
                DateTime todate = new DateTime(currenttime.Year, currenttime.Month, currenttime.Day).AddDays(1).AddMilliseconds(-5);
                //BankGateAPI _CardAPILog = new BankGateAPI();

                //var Data = _CardAPILog.ReportDoiSoat("sn2", "", "", fromdate, todate, 1);

                //SendTeleV3("-4604448335", $"Nạp bank tay {currenttime.ToString("dd/MM/yyyy")} : {Data.Sum(x => x.ReturnTotalValue).ToString("N0")}");


                //var Data2 = _CardAPILog.ReportDoiSoat("bp7", "", "", fromdate, todate, 1);
                //SendTeleV3("-4643837912", $"Nạp bank {currenttime.ToString("dd/MM/yyyy")} : {Data2.Sum(x => x.ReturnTotalValue).ToString("N0")}");

                var Data3 = new BankCashAPI().ReportDoiSoat("cn02", "", "", fromdate, todate, 1);
                SendTeleV3("-4818777527", $"Amount Paid on Behalf ({currenttime.ToString("dd/MM/yyyy")}) : {Data3.Sum(x => x.ReturnTotalValue).ToString("N0")}");

                var user = new Users().GetByUserName("cn02");
                if (user != null)
                    SendTeleV3("-4818777527", "Current Balance:  " + user.Balance.ToString("N0").Replace(".", ","));
                //var lisWithdraw = new UserWithdraw().GetList(100, "bp7", 1, fromdate, todate);
                //var TotalDeduct = lisWithdraw.Sum(x => x.Amount);

                //var lisWithdraw2 = new UserDeposit().GetList(100, "bp7", 1, fromdate, todate);
                //var TotalTopup = lisWithdraw2.Sum(x => x.Amount);
                //SendTeleV3("-4643837912", $"Nạp số dư {currenttime.ToString("dd/MM/yyyy")} : {TotalTopup.ToString("N0")}");
                //SendTeleV3("-4643837912", $"Rút số dư {currenttime.ToString("dd/MM/yyyy")} : {TotalDeduct.ToString("N0")}");


                //var Data4 = _CardAPILog.ReportDoiSoat("c1tm", "", "", fromdate, todate, 1);
                //SendTeleV3("-4925611263", $"Nạp bank {currenttime.ToString("dd/MM/yyyy")} : {Data4.Sum(x => x.ReturnTotalValue).ToString("N0")}");

                //var Data5 = new BankCashAPI().ReportDoiSoat("c1tm", "", "", fromdate, todate, 1);
                //SendTeleV3("-4925611263", $"Rút bank {currenttime.ToString("dd/MM/yyyy")} : {Data5.Sum(x => x.ReturnTotalValue).ToString("N0")}");

                //var lisWithdraw3 = new UserWithdraw().GetList(100, "c1tm", 1, fromdate, todate);
                //var TotalDeduct3 = lisWithdraw3.Sum(x => x.Amount);

                //var lisWithdraw4 = new UserDeposit().GetList(100, "c1tm", 1, fromdate, todate);
                //var TotalTopup4 = lisWithdraw4.Sum(x => x.Amount);
                //SendTeleV3("-4925611263", $"Nạp số dư {currenttime.ToString("dd/MM/yyyy")} : {TotalDeduct3.ToString("N0")}");
                //SendTeleV3("-4925611263", $"Rút số dư {currenttime.ToString("dd/MM/yyyy")} : {TotalTopup4.ToString("N0")}");
            }
            catch (Exception e)
            {
                NLogLogger.Info(e.Message);
            }
        }
        //public static void SendTeleV2(string id, string message)
        //{
        //    Task.Run(() => SendTeleV3(id, message));

        //    ////var apiResponsetext = Utilities.HttpRequestGet(requestUrl);
        //}
        public static void SendTeleV3(string id, string message)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var requestUrl = string.Format("https://api.telegram.org/bot7593090326:AAFAMnx9Bw6akr6mw9YLrtohHBzybgoqhpo/sendMessage?chat_id={1}&text={0}", message, id);
                var webclient = new WebClient();

                //NLogLogger.Info(requestUrl);
                webclient.DownloadString(requestUrl);
            }
            catch (Exception e)
            {
                NLogLogger.Info(e.Message);
            }

        }


    }
}
