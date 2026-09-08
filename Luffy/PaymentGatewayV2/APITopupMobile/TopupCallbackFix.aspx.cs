using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.Utils;

namespace APITopupMobile
{
    public partial class TopupCallbackFix : System.Web.UI.Page
    {
        private static bool isStar = true;
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        private string providerUrl = "https://dl.guitarpal.info/api/Order/update";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnStart_Click(object sender, EventArgs e)
        {
            if (!isStar) isStar = true;
            while (isStar)
            {
                //Doing
                try
                {
                    var callBackMiss = new TopupMobile3rdLog().GetListMissCallback();
                    if (callBackMiss == null)
                    {
                        isStar = false;
                        NLogLogger.Info(new string[] { "Không có Transaction nào tìm thấy" });
                    }

                    //Callback Nhà cng cấp
                    if (!string.IsNullOrEmpty(providerUrl))
                    {
                        foreach (var cMiss in callBackMiss)
                        {
                            var data = new DataCallbackOrder()
                            {
                                Amount = Convert.ToInt32(cMiss.Amount),
                                OrderId = Convert.ToInt64(cMiss.RequestNo),
                                Status = 1,
                                CardSerial = cMiss.CardSerial,
                                CardCode = cMiss.CardCode,
                                BidRate = cMiss.BidRate,
                                UpdateTime = DateTime.Now,
                                CreatTime = cMiss.CreateTime,
                                Signature = string.Empty
                            };
                            NLogLogger.Info(new string[] { "APITopupMobile", "Process Callback", serializer.Serialize(data) });
                            Task.Run(async () => await CallbackJson(providerUrl, serializer.Serialize(data), cMiss.CardCode).ConfigureAwait(false));
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception exp)
                {
                    NLogLogger.Info(new string[] { "==> exp", exp.Message });
                    isStar = false;
                }

            }
        }

        public async Task<string> CallbackJson(string url, string postData, string code)
        {
            NLogLogger.Info(new string[] { "APITopupMobile", "Callback Missing", "Request", code, url, postData });
            var uri = new Uri(url);
            try
            {
                var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);

                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        NLogLogger.Info(new string[] { "APITopupMobile", "Callback Missing", "Response", code, url, postData, responseContent });
                        return responseContent;
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "APITopupMobile", "Callback", e.Message });
                return string.Empty;
            }

            return string.Empty;

        }
    }
}