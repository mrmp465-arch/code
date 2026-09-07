using Libs.Report;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankGateV2.bankin
{
    /// <summary>
    /// Summary description for getbank1
    /// </summary>
    public class getbank : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var partnerCode = HttpContext.Current.Request.QueryString["partnerCode"];

            if (partnerCode == "kuipay2")
            {
                var listBank = new List<BankAccountV9>();
                listBank.Add(
                    new BankAccountV9
                    {
                        BankName = "CONG TY TNHH PHAT TRIEN VA DICH VU HDV",
                        BankCode = "SHBVN",
                        BankId = "HDV666666",
                        QR = "https://img.vietqr.io/image/SHBVN-HDV666666-compact.jpg",
                    }
                );
                listBank.Add(
                   new BankAccountV9
                   {
                       BankName = "CONG TY TNHH PHAT TRIEN VA DICH VU HDV",
                       BankCode = "SHBVN",
                       BankId = "HDV999999",
                       QR = "https://img.vietqr.io/image/SHBVN-HDV999999-compact.jpg",
                   }
               );
                string jsonContent = JsonConvert.SerializeObject(listBank, new JsonSerializerSettings
                {
                    StringEscapeHandling = StringEscapeHandling.Default
                });
                context.Response.Write(jsonContent);
            }
        }
        public class BankAccountV9
        {

            public string BankName { get; set; }

            public string BankId { get; set; }
            public string BankCode { get; set; }
            public string QR { get; set; }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}