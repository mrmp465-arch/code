using Libs.BankDirect.MDrum;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Report;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetUsdtRate
    /// </summary>
    public class GetUsdtRate : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var lstDataBank = new UserDeposit().GetList(10, "cn02", 1, DateTime.Now.AddMonths(-3), DateTime.Now.AddHours(1));
            var lastRate = lstDataBank.FirstOrDefault(m => m.Status == 1 && m.UserName == "cn02" && m.Amount / m.Money > 20000);
            var priceCache = DataCaching.GetCache<string>("UsdtRate");
            if (priceCache == null)
            {
                var content = @"{
                ""page"": 1,
                ""rows"": 5,
                ""payTypes"": [""BANK""],
                ""asset"": ""USDT"",
                ""fiat"": ""VND"",
                ""tradeType"": ""SELL""
                }";
                string url = "https://p2p.binance.com/bapi/c2c/v2/friendly/c2c/adv/search";

                var result = Task.Run(async () => await PostTask(url, content)).Result;

                var resObj = serializer.Deserialize<BNRespone>(result);
                var price = long.Parse(resObj.data.LastOrDefault().adv.price);
                DataCaching.SetCache("UsdtRate", price.ToString(), 60*3);
                price -= 200;
                var rate = new RateInfo
                {
                    CurrentRate = price,
                    LastRate = (int)(lastRate.Amount / lastRate.Money),
                };
                context.Response.Write(serializer.Serialize(rate));
            }
            else
            {
                var price = long.Parse(priceCache);
                price -= 200;
                var rate = new RateInfo
                {
                    CurrentRate = price,
                    LastRate = (int)(lastRate.Amount / lastRate.Money),
                };
                context.Response.Write(serializer.Serialize(rate));
            }




        }
        public static async Task<string> PostTask(string url, string postData)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var uri = new Uri(url);

            using (var httpContent = new StringContent(postData, Encoding.UTF8, "application/json"))
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(60);
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");

                try
                {
                    var response = await client.PostAsync(uri, httpContent);
                    if (response.Content != null)
                    {
                        var encoding = response.Content.Headers.ContentEncoding;

                        if (encoding.Contains("gzip"))
                        {
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var decompressed = new GZipStream(stream, CompressionMode.Decompress))
                            using (var reader = new StreamReader(decompressed))
                            {
                                return await reader.ReadToEndAsync();
                            }
                        }
                        else
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new[] { "MDrum", "Exception Post", e.Message });
                }
            }

            return string.Empty;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public class Adv
        {
            public string price { get; set; }
        }


        public class Datum
        {
            public Adv adv { get; set; }

        }

        public class BNRespone
        {
            public string code { get; set; }

            public List<Datum> data { get; set; }

            public bool success { get; set; }
        }
        public class RateInfo
        {
            public long CurrentRate { get; set; }
            public int LastRate { get; set; }

        }

    }
}