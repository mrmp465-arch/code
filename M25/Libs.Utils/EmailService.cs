using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Libs.Utils
{
    public class EmailService
    {
        private static int Mail_Port = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Mail_Port"]) ? int.Parse(ConfigurationManager.AppSettings["Mail_Port"]) : 587;
        private static string Mail_Host = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Mail_Host"]) ? ConfigurationManager.AppSettings["Mail_Host"] : "smtp.gmail.com";
        private static string Mail_Username = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Mail_Username"]) ? ConfigurationManager.AppSettings["Mail_Username"] : "bb2dpay@gmail.com";
        private static string Mail_Password = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Mail_Password"]) ? ConfigurationManager.AppSettings["Mail_Password"] : "RboK/VFENzEnk+GGBjIil9g3oPK6iHfOS8/Wv2A7o8A=";
        private static string Mail_SenderEmail = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Mail_SenderEmail"]) ? ConfigurationManager.AppSettings["Mail_SenderEmail"] : "bb2dpay@gmail.com";
        private static string Mail_SenderName = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Mail_SenderName"]) ? ConfigurationManager.AppSettings["Mail_SenderName"] : "BB2D Payment";

        public static bool SendMail(string subject, string body, string toEmail)
        {
            try
            {
                NLogLogger.Info(new string[] { "SendMail", "To", toEmail });

                SmtpClient client = new SmtpClient();
                client.Port = Mail_Port;
                client.Host = Mail_Host;
                client.Timeout = 10000;

                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(Mail_Username, new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8").Decrypt(Mail_Password));

                var mm = new MailMessage();
                mm.From = new MailAddress(Mail_SenderEmail, Mail_SenderName);
                mm.ReplyTo = new MailAddress(Mail_SenderEmail, Mail_SenderName, System.Text.Encoding.UTF8);
                mm.To.Add(toEmail);
                mm.Subject = string.Format("{0} {1}", subject, DateTime.Now);
                mm.SubjectEncoding = Encoding.UTF8;
                mm.BodyEncoding = Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                mm.Priority = MailPriority.High;
                mm.Headers.Add("Content-Type", "text/html;format=flowed;charset=\"utf-8\";reply-type=original");
                mm.Headers.Add("Content-Transfer-Encoding", "7bit");

                var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                htmlView.TransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;
                mm.AlternateViews.Add(htmlView);

                client.Send(mm);


                return true;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "SendMail", "Error", ex.Message.Replace("\n", " ") });
                return false;
            }
        }
    }
}
