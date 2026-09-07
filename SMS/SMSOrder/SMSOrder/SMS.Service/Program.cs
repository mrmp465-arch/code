using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using SMS.Data.Api;
using SMS.Data.Factory;
using SMS.Utility;
using static SMS.Utility.Enums;

namespace SMS.Service
{
    static class Program
    {

        static void Main()
        {



            //SendByTelCo("VMS", (int)TelcoType.VMS);
            //System.Threading.Thread.Sleep(100);
            //SendByTelCo("VTT", (int)TelcoType.VTT);
            //System.Threading.Thread.Sleep(100);
            //SendByTelCo("VNP", (int)TelcoType.VNP);

            //SendAll();
            if(ServerProcess.GetSystemStatusCache()=="1")
            {
                SendMOMO();
            }    
        }
        static void SendMOMO()
        {

           
            var lstSMS = new List<SMSRequest>();
            var SMSdata = AbstractDAOFactory.Instance().SMSLogsService().GetTopSMS(10, -1);
            if(SMSdata.Count>0)
            {
                foreach (var obj in SMSdata)
                {
                    lstSMS.Add(new SMSRequest
                    {
                        MomoId = obj.Number,
                        TransId = obj.Id.ToString(),
                        Content = obj.Contents.Replace("\n", " "),
                        CallbackUrl = "http://127.0.0.1:1596/api/SMS/CallBack"
                    }
                    );
                }

                var result = ServerProcess.SendMultiSMS(lstSMS);
                if (result.ResponseCode > 0)
                {
                    AbstractDAOFactory.Instance().SMSLogsService().UpdateMultiRespone(SMSdata.Select(x => x.Id).ToList(), 0, 1, "");
                }
                else
                {
                    ServerProcess.SetSystemStatusCache(0);
                }
            }    
           


        }
        //static void SendAll()
        //{
        //    var sw = new Stopwatch();
        //    sw.Start();
        //    var allPort = ServerProcess.GetAllPortInfo(Config.sn);
        //    if (allPort == null || allPort.ResponseContent.Count == 0)
        //    {
        //        NLogLogger.DebugMessage("----Get Port Fail----");
        //        return;
        //    }
        //    var lstPort = allPort.ResponseContent.Where(x => x.State == 1).ToList();



        //    var number = (int)lstPort.Count() * Config.PercentUsePort / 100;
        //    NLogLogger.DebugMessage("---Port  Number: " + number + " List:[ " + string.Join(",", lstPort.Select(x => x.Number).ToArray()) + "]----");
        //    if (number < 1)
        //    {
        //        NLogLogger.DebugMessage("---Port  Not Enough----");
        //        return;
        //    }

        //    lstPort.Shuffle();//trộn thứ tự trong list
        //    var SMSdata = AbstractDAOFactory.Instance().SMSLogsService().GetTopSMS(number, -1);
        //    if (SMSdata.Count() > 0)
        //    {
        //        var lstPortVMS = new List<int>();
        //        var lstsmsParamVMS = new List<SmsParam>();
        //        foreach (var item in SMSdata.Select((value, i) => new { i, value }))
        //        {

        //            lstPortVMS.Add(lstPort[item.i].Position.Port);
        //            lstsmsParamVMS.Add(
        //                new SmsParam
        //                {
        //                    number = item.value.Number,
        //                    user_id = item.value.Id,
        //                    text_param = new List<String> { item.value.Contents }

        //                }
        //            );

        //        }
        //        var result = SendMultiSMS(lstPortVMS, lstsmsParamVMS);
        //        //NLogLogger.DebugMessage("Resut:" + result);
        //        if (result > 0)
        //        {
        //            AbstractDAOFactory.Instance().SMSLogsService().UpdateMultiRespone(SMSdata.Select(x => x.Id).ToList(), 0, 1, "");
        //        }
        //    }

        //    sw.Stop();
        //    NLogLogger.DebugMessage(string.Format("Send SMS time: {0} ms", sw.ElapsedMilliseconds));
        //}
        //static void SendByTelCo(string telco, int telcoType)
        //{
        //    var sw = new Stopwatch();
        //    sw.Start();
        //    var allPort = ServerProcess.GetAllPortInfo(Config.sn);
        //    if (allPort == null || allPort.ResponseContent.Count == 0)
        //    {
        //        NLogLogger.DebugMessage("----Get Port Fail----");
        //        return;
        //    }






        //    //lstPort.Shuffle();//trộn thứ tự trong list
        //    var SMSdata = AbstractDAOFactory.Instance().SMSLogsService().GetTopSMS(10, telcoType);

        //    if (SMSdata.Count() > 0)
        //    {

        //        foreach (var item in SMSdata.Select((value, i) => new { i, value }))
        //        {
        //            var lstPort = allPort.ResponseContent.Where(x => x.State == 1 && x.Position.Port == 24 && x.Count < 495).ToList();
        //            var lstLockNumber = ServerProcess.GetNumberLockCache();
        //            if (lstLockNumber.Count > 0)
        //            {
        //                lstPort = lstPort.Where(x => !lstLockNumber.Contains(x.Number)).ToList();
        //            }
        //            var lstLock4TNumber = ServerProcess.GetNumberLock4TCache();
        //            if (lstLock4TNumber.Count > 0)
        //            {
        //                lstPort = lstPort.Where(x => !lstLock4TNumber.Contains(x.Number)).ToList();
        //            }
        //            if (lstPort.Count == 0)
        //            {
        //                NLogLogger.DebugMessage("----Get Port Fail----");
        //                return;
        //            }

        //            //var number = (int)lstPort.Count() ;
        //            var port = lstPort.FirstOrDefault();

        //            var resultSend = ServerProcess.SendSMS(port.Position.Port, item.value.Number, item.value.Contents, (int)item.value.Id);
        //            System.Threading.Thread.Sleep(10000);
        //            if (resultSend > 0)
        //            {
        //                AbstractDAOFactory.Instance().SMSLogsService().UpdateMultiRespone(SMSdata.Select(x => x.Id).ToList(), 0, 1, "");
        //            }

        //        }
        //        //var result = SendMultiSMS(lstPortVMS, lstsmsParamVMS);
        //        //NLogLogger.DebugMessage("Resut:" + result);

        //    }

        //    sw.Stop();
        //    NLogLogger.DebugMessage(string.Format("Send {1} SMS time: {0} ms", sw.ElapsedMilliseconds, telco));
        //}
        //static int SendMultiSMS(List<int> ports, List<SmsParam> smsParam)
        //{
        //    return ServerProcess.SendMultiSMS(ports, smsParam, Config.sn).ResponseCode;
        //}
        //static void Shuffle<T>(this IList<T> list)
        //{
        //    RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        //    int n = list.Count;
        //    while (n > 1)
        //    {
        //        byte[] box = new byte[1];
        //        do provider.GetBytes(box);
        //        while (!(box[0] < n * (Byte.MaxValue / n)));
        //        int k = (box[0] % n);
        //        n--;
        //        T value = list[k];
        //        list[k] = list[n];
        //        list[n] = value;
        //    }
        //}
    }

}
