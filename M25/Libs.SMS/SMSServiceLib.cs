using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Libs.SMS.SMSHandler;

namespace Libs.SMS
{
  public static  class SMSServiceLib
    {
        public static string HttpGet(string uri)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Timeout = 17000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }

            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    //Handle timeout exception
                    var message = string.Format(SmsResponseConstant.SMS_SYSTEM_ERROR);
                    return string.Format("{{\"status\":{0},\"sms\":\"{1}\",\"type\":\"text\"}}", 0, message);
                }
                else
                {
                    throw;
                }
            }

        }
    }
}
